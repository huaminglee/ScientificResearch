using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Silverlight;

namespace OrganizationalStructure
{
    public partial class FrmDept : ChildWindow
    {
        /// <summary>
        /// 执行类型
        /// </summary>
        public string doType = "EditDept";
        /// <summary>
        /// 当前部门
        /// </summary>
        public string currDeptNo = "";
        //声明委托
        public delegate void ReFreshParent();
        public event ReFreshParent ReFreshParentEve; //声明事件
        public FrmDept()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 初始化部门信息
        /// </summary>
        /// <param name="doType">执行类型</param>
        /// <param name="deptNo">部门编号</param>
        public void InitDeptInfo(string doType, string deptNo)
        {
            this.doType = doType;
            this.currDeptNo = deptNo;

            if (this.doType == "EditDept")
                this.TB_No.IsReadOnly = true;
            else
                this.TB_No.IsReadOnly = false;

            string sql = "SELECT No,Name,FK_DeptType,Leader FROM Port_Dept WHERE No='" + this.currDeptNo + "'"; // 部门信息.
            sql += "@ SELECT FK_Dept,FK_Duty FROM Port_DeptDuty WHERE FK_Dept='" + this.currDeptNo + "'"; // 当前部门的职务.
            sql += "@ SELECT FK_Dept,FK_Station FROM Port_DeptStation WHERE FK_Dept='" + this.currDeptNo + "'"; // 当前部门的岗位.
            sql += "@ SELECT No,Name FROM Port_DeptType "; // 部门类型.
            sql += "@ SELECT No,Name FROM Port_Duty "; //职务.
            sql += "@ SELECT No,Name FROM Port_Station "; //岗位
            sql += "@ SELECT No,Name FROM Port_StationType "; // 岗位类型.
            OS.OSSoapClient da = BP.Glo.GetOSServiceInstance();
            da.RunSQLReturnTableSAsync(sql);
            da.RunSQLReturnTableSCompleted += new EventHandler<OS.RunSQLReturnTableSCompletedEventArgs>(InitDeptInfoEvent);
        }
        public DataTable Port_Dept = null;
        public DataTable Port_DeptDuty = null;
        public DataTable Port_DeptStation = null;
        public DataTable Port_DeptType = null;
        public DataTable Port_Duty = null;
        public DataTable Port_Station = null;
        public DataTable Port_StationType = null;

        void InitDeptInfoEvent(object sender, OS.RunSQLReturnTableSCompletedEventArgs e)
        {
            DataSet ds = new DataSet();
            ds.FromXml(e.Result);
            Port_Dept = ds.Tables[0]; //部门信息。
            Port_DeptDuty = ds.Tables[1]; //当前部门的职务.

            Port_DeptStation = ds.Tables[2]; //当前部门的岗位.
            Port_DeptType = ds.Tables[3]; //部门类型.

            Port_Duty = ds.Tables[4]; //职务.
            Port_Station = ds.Tables[5]; //岗位.
            Port_StationType = ds.Tables[6]; //岗位类型.

            #region 绑定基础信息.
            if (this.doType == "EditDept")
            {
                if (Port_Dept.Rows.Count == 1)
                {
                    this.TB_No.Text = Port_Dept.Rows[0]["No"];
                    this.TB_Name.Text = Port_Dept.Rows[0]["Name"];
                    this.TB_Leader.Text = Port_Dept.Rows[0]["Leader"] == null ? "" : Port_Dept.Rows[0]["Leader"];
                }

                //绑定部门的类型.
                if (Port_Dept.Rows.Count == 1)
                    BP.Glo.Ctrl_DDL_BindDataTable(this.DDL_DeptType, Port_DeptType, Port_Dept.Rows[0]["FK_DeptType"]);
                else
                    BP.Glo.Ctrl_DDL_BindDataTable(this.DDL_DeptType, Port_DeptType, null);
            }
            else
            {
                this.Btn_Delete.IsEnabled = false;
                this.TB_No.Text = "";
                this.TB_Name.Text = "";
                this.TB_Leader.Text = "";

                BP.Glo.Ctrl_DDL_BindDataTable(this.DDL_DeptType, Port_DeptType, null);
            }
            #endregion 绑定基础信息.

            #region 绑定部门与职务对应信息.
            this.LB_Duty.Items.Clear();
            foreach (DataRow dr in Port_Duty.Rows)
            {
                CheckBox lb = new CheckBox();
                lb.Tag = dr["No"].ToString();
                lb.Name = dr["No"].ToString();
                lb.Content = dr["Name"].ToString();
                foreach (DataRow drIt in Port_DeptDuty.Rows)
                {
                    if (string.IsNullOrEmpty(drIt["FK_Duty"]))
                        continue;

                    if (drIt["FK_Duty"].ToString() == dr["No"].ToString())
                    {
                        lb.IsChecked = true;
                        break;
                    }
                }
                this.LB_Duty.Items.Add(lb);
            }
            #endregion 绑定部门与职务对应信息.

            #region 绑定部门与岗位对应信息.
            this.LB_Station.Items.Clear();
            foreach (DataRow dr in Port_Station.Rows)
            {
                CheckBox lb = new CheckBox();
                lb.Tag = dr["No"].ToString();
                lb.Name = "ST" + dr["No"].ToString();
                lb.Content = dr["Name"].ToString();
                foreach (DataRow drIt in Port_DeptStation.Rows)
                {
                    if (drIt["FK_Station"].ToString() == dr["No"].ToString())
                    {
                        lb.IsChecked = true;
                        break;
                    }
                }

                try
                {
                    this.LB_Station.Items.Add(lb);
                }
                catch
                {
                }
            }
            #endregion 绑定部门与岗位对应信息.
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.TB_Name.Text))
            {
                MessageBox.Show("部门名称不能为空", "系统提示", MessageBoxButton.OK);
                return;
            }
            //部门的属性.
            string attrs = "^Name=" + this.TB_Name.Text + "^FK_DeptType=" + BP.Glo.GetDDLValOfString(this.DDL_DeptType);
            attrs += "^Leader=" + this.TB_Leader.Text;
            // 岗位集合.
            string stations = "";
            foreach (CheckBox li in this.LB_Station.Items)
            {
                if (li.IsChecked == false)
                    continue;
                stations += li.Tag.ToString() + ",";
            }

            // 职务集合.
            string dutys = "";
            foreach (CheckBox li in this.LB_Duty.Items)
            {
                if (li.IsChecked == false)
                    continue;
                dutys += li.Tag.ToString() + ",";
            }

            OS.OSSoapClient da = BP.Glo.GetOSServiceInstance();
            if (this.doType == "EditDept")
            {
                /*如果是编辑*/
                da.Dept_EditAsync(this.TB_No.Text, attrs, stations, dutys, (bool)CB_AddStation.IsChecked);
                da.Dept_EditCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(da_Dept_EditCompleted);
            }

            if (this.doType == "CrateSameLevel")
            {
                /*如果是建立同级部门*/
                da.Dept_CreateSameLevelAsync(this.currDeptNo, attrs, stations, dutys);
                da.Dept_CreateSameLevelCompleted += new EventHandler<OS.Dept_CreateSameLevelCompletedEventArgs>(da_Dept_CreateSameLevelCompleted);
            }

            if (this.doType == "CrateSubLevel")
            {
                /*如果是建立下级部门*/
                da.Dept_CreateSubLevelAsync(this.currDeptNo, attrs, stations, dutys);
                da.Dept_CreateSubLevelCompleted += new EventHandler<OS.Dept_CreateSubLevelCompletedEventArgs>(da_Dept_CreateSubLevelCompleted);
            }
        }

        void da_Dept_CreateSubLevelCompleted(object sender, OS.Dept_CreateSubLevelCompletedEventArgs e)
        {
            if (e.Result.Contains("err"))
            {
                MessageBox.Show(e.Result, "系统提示", MessageBoxButton.OK);
                return;
            }
            //刷新父窗体
            if (ReFreshParentEve != null) ReFreshParentEve();
            this.DialogResult = true;
        }
        void da_Dept_CreateSameLevelCompleted(object sender, OS.Dept_CreateSameLevelCompletedEventArgs e)
        {
            if (e.Result.Contains("err"))
            {
                MessageBox.Show(e.Result, "系统提示", MessageBoxButton.OK);
                return;
            }
            //刷新父窗体
            if (ReFreshParentEve != null) ReFreshParentEve();
            this.DialogResult = true;
        }
        void da_Dept_EditCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            //刷新父窗体
            if (ReFreshParentEve != null) ReFreshParentEve();
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("您确定要删除吗？", "提示", MessageBoxButton.OKCancel)
                == MessageBoxResult.Cancel)
                return;

            OS.OSSoapClient da = BP.Glo.GetOSServiceInstance();
            da.Dept_DeleteAsync(this.TB_No.Text, true);
            da.Dept_DeleteCompleted += new EventHandler<OS.Dept_DeleteCompletedEventArgs>(da_Dept_DeleteCompleted);
        }

        void da_Dept_DeleteCompleted(object sender, OS.Dept_DeleteCompletedEventArgs e)
        {
            if (e.Result.Contains("err"))
            {
                MessageBox.Show(e.Result, "执行错误", MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show(e.Result, "删除成功", MessageBoxButton.OK);
                //刷新父窗体
                if (ReFreshParentEve != null) ReFreshParentEve();
                this.DialogResult = true;
            }
        }
        //查询岗位
        private void BT_SearchStation_Click(object sender, RoutedEventArgs e)
        {
            string sql = "SELECT FK_Dept,FK_Station FROM Port_DeptStation WHERE FK_Dept='" + this.currDeptNo + "'"; // 当前部门的岗位.
            sql += "@ SELECT No,Name FROM Port_Station WHERE Name like '%" + TB_StationName.Text + "%'"; //岗位
            OS.OSSoapClient da = BP.Glo.GetOSServiceInstance();
            da.RunSQLReturnTableSAsync(sql);
            da.RunSQLReturnTableSCompleted += new EventHandler<OS.RunSQLReturnTableSCompletedEventArgs>(InitDeptStationInfoEvent);
        }

        void InitDeptStationInfoEvent(object sender, OS.RunSQLReturnTableSCompletedEventArgs e)
        {
            DataSet ds = new DataSet();
            ds.FromXml(e.Result);
            Port_DeptStation = ds.Tables[0]; //当前部门的岗位。
            Port_Station = ds.Tables[1]; //岗位.

            #region 绑定部门与岗位对应信息.
            this.LB_Station.Items.Clear();
            foreach (DataRow dr in Port_Station.Rows)
            {
                CheckBox lb = new CheckBox();
                lb.Tag = dr["No"].ToString();
                lb.Name = "ST" + dr["No"].ToString();
                lb.Content = dr["Name"].ToString();
                foreach (DataRow drIt in Port_DeptStation.Rows)
                {
                    if (drIt["FK_Station"].ToString() == dr["No"].ToString())
                    {
                        lb.IsChecked = true;
                        break;
                    }
                }

                try
                {
                    this.LB_Station.Items.Add(lb);
                }
                catch
                {
                }
            }
            #endregion 绑定部门与岗位对应信息.
        }
    }
}

