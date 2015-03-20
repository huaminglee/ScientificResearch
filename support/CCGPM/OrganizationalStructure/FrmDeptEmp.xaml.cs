using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Silverlight;
using BP;

namespace OrganizationalStructure
{
    public partial class FrmDeptEmp : ChildWindow
    {
        /// <summary>
        /// 当前的部门编号
        /// </summary>
        public string FK_Dept = "";
        public string doType = "Edit";
        public string EmpNo = null;
        //声明委托
        public delegate void ReFreshParent();
        //声明事件
        public event ReFreshParent ReFreshParentEve; 
        /// <summary>
        /// 部门人员信息
        /// </summary>
        public FrmDeptEmp()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 绑定人员信息
        /// </summary>
        /// <param name="doType">执行类型</param>
        /// <param name="fk_dept">部门编号</param>
        /// <param name="empNo">人员编号</param>
        public void InitEmp(string doType, string fk_dept, string empNo)
        {
            this.doType = doType;
            this.FK_Dept = fk_dept;
            this.EmpNo = empNo;

            string sql = "SELECT * FROM Port_Emp WHERE No='" + this.EmpNo + "'"; // 人员信息.
            sql += "@ SELECT FK_Dept,FK_Emp,Leader,FK_Duty,DutyLevel FROM Port_DeptEmp WHERE FK_Dept='" + this.FK_Dept + "' AND FK_Emp='" + this.EmpNo + "'"; // 人员在当前部门的信息.
            sql += "@ SELECT No,Name FROM Port_Station WHERE No IN (SELECT FK_Station FROM Port_DeptEmpStation WHERE FK_Dept='" + this.FK_Dept + "' AND FK_Emp='" + this.EmpNo + "')"; // 当前人员在此部门的岗位集合.

            sql += "@ SELECT No,Name FROM Port_Duty WHERE No IN (SELECT FK_Duty FROM Port_DeptDuty WHERE FK_Dept='" + this.FK_Dept + "')"; // 当前部门的职务集合.
            sql += "@ SELECT No,Name FROM Port_Station WHERE No IN (SELECT FK_Station FROM Port_DeptStation WHERE FK_Dept='" + this.FK_Dept + "')"; // 当前部门的岗位集合.

            
            OS.OSSoapClient da = BP.Glo.GetOSServiceInstance();
            da.RunSQLReturnTableSAsync(sql);
            da.RunSQLReturnTableSCompleted += new EventHandler<OS.RunSQLReturnTableSCompletedEventArgs>(InitDeptInfoEvent);
        }

        void InitDeptInfoEvent(object sender, OS.RunSQLReturnTableSCompletedEventArgs e)
        {
            DataSet ds = new DataSet();
            ds.FromXml(e.Result);

            DataTable Port_Emp = ds.Tables[0]; //部门信息。
            DataTable Port_DeptEmp = ds.Tables[1]; //当前人员与部门的信息.

            DataTable StationsOfEmp = ds.Tables[2]; //当前部门的岗位.

            DataTable DutysOfDept = ds.Tables[3]; //当前部门的职务
            DataTable StationsOfDept = ds.Tables[4]; //当前部门的岗位.

            if (DutysOfDept.Rows.Count == 0 || StationsOfDept.Rows.Count == 0)
            {
                if (MessageBox.Show("此部门下没有岗位与职务的对应关系，所以您不能增加人员。\t\n 您现在要维护此部门的岗位与职务的对应关系吗？",
                     "错误", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    FrmDept fd = new FrmDept();
                    fd.InitDeptInfo("EditDept", this.FK_Dept);
                    fd.Show();
                    this.DialogResult = false;
                }
                return;
            }

            #region 绑定基础信息.
            if (Port_DeptEmp.Rows.Count == 1)
            {
                this.TB_No.Text = Port_Emp.Rows[0]["No"];
                this.TB_Name.Text = Port_Emp.Rows[0]["Name"];
                this.TB_EmpNo.Text = Port_Emp.Rows[0]["EmpNo"] == null ? "" : Port_Emp.Rows[0]["EmpNo"];
                this.TB_Tel.Text = Port_Emp.Rows[0]["Tel"] == null ? "" : Port_Emp.Rows[0]["Tel"];
                this.TB_Email.Text = Port_Emp.Rows[0]["Email"] == null ? "" : Port_Emp.Rows[0]["Email"];

                //绑定它的职务.
                Glo.Ctrl_DDL_BindDataTable(this.DDL_Duty, DutysOfDept, Port_DeptEmp.Rows[0]["FK_Duty"]);

                // 它的领导,
                this.TB_Leader.Text = Port_DeptEmp.Rows[0]["Leader"] == null ? "" : Port_DeptEmp.Rows[0]["Leader"];
                //职务级别
                this.TB_Level.Text = Port_DeptEmp.Rows[0]["DutyLevel"] == null ? "" : Port_DeptEmp.Rows[0]["DutyLevel"];
            }
            else
            {
                // 绑定职务集合.
                Glo.Ctrl_DDL_BindDataTable(this.DDL_Duty, DutysOfDept, null);
            }
            #endregion 绑定基础信息.

            #region 绑定部门与岗位对应信息.
            this.LB_Station.Items.Clear();
            foreach (DataRow dr in StationsOfDept.Rows)
            {
                CheckBox lb = new CheckBox();
                lb.Tag = dr["No"].ToString();
                lb.Name = dr["No"].ToString();
                lb.Content = dr["Name"].ToString();
                foreach (DataRow drIt in StationsOfEmp.Rows)
                {
                    if (drIt["No"].ToString() == dr["No"].ToString())
                    {
                        lb.IsChecked = true;
                        break;
                    }
                }
                this.LB_Station.Items.Add(lb);
            }
            #endregion 绑定部门与岗位对应信息.
        }
        /// <summary>
        /// 按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            string err = "";
            if (this.TB_No.Text.Length == 0)
                err += "人员编号不能为空.";

            if (this.TB_Name.Text.Length == 0)
                err += "人员名称不能为空.";

            if (BP.Glo.IsNum(this.TB_Level.Text) == false)
                err += "职务级别必须是数字类型.";

            if (string.IsNullOrEmpty(err) == false)
                throw new Exception(err);

            string attrs = "^Name=" + this.TB_Name.Text + "^FK_Duty=" + BP.Glo.GetDDLValOfString(this.DDL_Duty);
            attrs += "^FK_Dept=" + this.FK_Dept;
            attrs += "^DutyLevel=" + this.TB_Level.Text;
            attrs += "^No=" + this.TB_No.Text;
            attrs += "^EmpNo=" + this.TB_EmpNo.Text;
            attrs += "^Leader=" + this.TB_Leader.Text;
            attrs += "^Tel=" + this.TB_Tel.Text;
            attrs += "^Email=" + this.TB_Email.Text;

            // 岗位集合.
            string stations = "";
            foreach (CheckBox li in this.LB_Station.Items)
            {
                if (li.IsChecked == false)
                    continue;
                stations += li.Tag.ToString() + ",";
            }

            OS.OSSoapClient da = BP.Glo.GetOSServiceInstance();
            if (this.doType == "Edit")
            {
                da.Emp_EditAsync(this.TB_No.Text, this.FK_Dept, attrs, stations);
                da.Emp_EditCompleted += new EventHandler<OS.Emp_EditCompletedEventArgs>(da_Emp_EditCompleted);
            }

            if (this.doType == "New")
            {
                da.Emp_NewAsync(this.TB_No.Text, this.FK_Dept, attrs, stations);
                da.Emp_NewCompleted += new EventHandler<OS.Emp_NewCompletedEventArgs>(da_Emp_NewCompleted);
            }
        }
        void da_Emp_NewCompleted(object sender, OS.Emp_NewCompletedEventArgs e)
        {
         //    MessageBox.Show(e.Result, "新建成功", MessageBoxButton.OK);
            //刷新父窗体
            if (ReFreshParentEve != null) ReFreshParentEve();
            this.DialogResult = true;
        }
        void da_Emp_EditCompleted(object sender, OS.Emp_EditCompletedEventArgs e)
        {
          //  MessageBox.Show(e.Result, "编辑成功", MessageBoxButton.OK);
            //刷新父窗体
            if (ReFreshParentEve != null) ReFreshParentEve();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Del_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("您确定要删除吗？", "提示", MessageBoxButton.OKCancel)
                == MessageBoxResult.Cancel)
                return;

            OS.OSSoapClient da = BP.Glo.GetOSServiceInstance();
            da.Emp_DeleteAsync(this.TB_No.Text, this.FK_Dept);
            da.Emp_DeleteCompleted += new EventHandler<OS.Emp_DeleteCompletedEventArgs>(da_Emp_DeleteCompleted);
        }

        void da_Emp_DeleteCompleted(object sender, OS.Emp_DeleteCompletedEventArgs e)
        {
            if (e.Result.Contains("error") == true)
            {
                MessageBox.Show(e.Result, "删除失败", MessageBoxButton.OK);
                return;
            }
            else
            {
                MessageBox.Show(e.Result, "成功", MessageBoxButton.OK);
                //刷新父窗体
                if (ReFreshParentEve != null) ReFreshParentEve();
                this.DialogResult = true;
            }
        }

        private void Btn_Copy_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("目前不支持此功能", "信息", MessageBoxButton.OK);
        }
        //姓名转拼音
        private void TB_Name_LostFocus(object sender, RoutedEventArgs e)
        {
            OS.OSSoapClient da = BP.Glo.GetOSServiceInstance();
            da.ParseStringToPinyinAsync( this.TB_Name.Text);
            da.ParseStringToPinyinCompleted += new EventHandler<OS.ParseStringToPinyinCompletedEventArgs>(da_ParseStringToPinyin_Completed);
        }

        void da_ParseStringToPinyin_Completed(object sender, OS.ParseStringToPinyinCompletedEventArgs e)
        {
            if (e.Result != null)
                this.TB_No.Text = e.Result.ToLower();
        }

        private void Btn_Move_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("目前不支持此功能", "信息", MessageBoxButton.OK);
        }
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_ResetPass_Click(object sender, RoutedEventArgs e)
        {
            OS.OSSoapClient da = BP.Glo.GetOSServiceInstance();
            da.DoAsync("ResetPassword", this.EmpNo, true);
            da.DoCompleted += new EventHandler<OS.DoCompletedEventArgs>(da_DoCompleted);
        }

        void da_DoCompleted(object sender, OS.DoCompletedEventArgs e)
        {
            if (e.Result != null && e.Result.Contains("err"))
            {
                MessageBox.Show(e.Result, "执行错误", MessageBoxButton.OK);
                return;
            }
            MessageBox.Show("当前用户可以以123登录.", "密码重置成功", MessageBoxButton.OK);
        }
        private void Btn_SaveClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.OKButton_Click(sender, e);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "保存失败", MessageBoxButton.OK);
                return;
            }

            ////刷新父窗体
            //if (ReFreshParentEve != null)
            //    ReFreshParentEve();

            this.DialogResult = true;
        }
    }
}

