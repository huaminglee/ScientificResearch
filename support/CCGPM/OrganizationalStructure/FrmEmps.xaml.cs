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
    public partial class FrmEmps : ChildWindow
    {
        //声明委托
        public delegate void ReFreshParent();
        //声明事件
        public event ReFreshParent ReFreshParentEve;
        private string _FK_Dept = "";
        public FrmEmps()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 初始化人员信息
        /// </summary>
        public void InitEmps(string FK_Dept)
        {
            _FK_Dept = FK_Dept;
            //获取所有人员
            string sql = "SELECT Top 200 No,Name,EmpNo FROM Port_Emp";
            sql += "@ select FK_Emp from Port_DeptEmp where FK_Dept = '" + _FK_Dept + "'";
            OS.OSSoapClient da = BP.Glo.GetOSServiceInstance();
            da.RunSQLReturnTableSAsync(sql);
            da.RunSQLReturnTableSCompleted += new EventHandler<OS.RunSQLReturnTableSCompletedEventArgs>(InitDeptInfoEvent);
        }

        void InitDeptInfoEvent(object sender, OS.RunSQLReturnTableSCompletedEventArgs e)
        {
            DataSet ds = new DataSet();
            ds.FromXml(e.Result);
            string isHaveEmpNos = "";//已包含的人员编号集合
            DataTable Port_Emp = ds.Tables[0]; //人员信息。
            DataTable dt_Emps = ds.Tables[1]; //包含在部门中的人员
            List<CheckListBoxModel> emps = new List<CheckListBoxModel>();
            //整理本部门已包含人员
            foreach (DataRow empNoRow in dt_Emps.Rows)
            {
                isHaveEmpNos += empNoRow["FK_Emp"] + ",";
            }
            //添加项
            foreach (DataRow row in Port_Emp.Rows)
            {
                CheckListBoxModel emp = new CheckListBoxModel();
                emp.ID = row["No"].ToString();
                emp.ModelName = row["No"].ToString() + "(" + row["EmpNo"] + ")-" + row["Name"];
                if (isHaveEmpNos.Contains(row["No"].ToString() + ","))
                {
                    emp.IsSelected = true;
                }
                emps.Add(emp);
            }
            CKB_Emps.ItemsSource = emps.ToList();
            CKB_Emps.UpdateLayout();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            string empNos = "";
            IEnumerable<BP.CheckListBoxModel> list = (IEnumerable<BP.CheckListBoxModel>)CKB_Emps.ItemsSource;
            IEnumerable<BP.CheckListBoxModel> selectedList = list.Where(a => a.IsSelected == true);

            //获取人员编号集合
            foreach (BP.CheckListBoxModel emp in selectedList)
            {
                if (empNos.Length == 0)
                    empNos += emp.ID;
                else
                    empNos += "^" + emp.ID;
            }
            if (empNos == "")
            {
                MessageBox.Show("请选择人员。","系统提示", MessageBoxButton.OK);
                return;
            }

            OS.OSSoapClient da = BP.Glo.GetOSServiceInstance();
            da.Dept_Emp_RelatedAsync(empNos, _FK_Dept);
            da.Dept_Emp_RelatedCompleted +=new EventHandler<OS.Dept_Emp_RelatedCompletedEventArgs>(da_Dept_Emp_RelatedCompleted);
        }

        void da_Dept_Emp_RelatedCompleted(object sender, OS.Dept_Emp_RelatedCompletedEventArgs e)
        {
            if (e.Result.Contains("error") == true)
            {
                MessageBox.Show(e.Result, "关联失败", MessageBoxButton.OK);
                return;
            }
            ReFreshParentEve();
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        //查询
        private void Btn_Query_Click(object sender, RoutedEventArgs e)
        {
            string info = TB_Content.Text;
            //获取所有人员
            string sql = "SELECT Top 200 No,Name,EmpNo FROM Port_Emp WHERE No like '%" + info + "%' OR Name like '%" + info + "%' OR EmpNo like '%" + info + "%'";
            sql += "@ select FK_Emp from Port_DeptEmp where FK_Dept = '" + _FK_Dept + "'";
            OS.OSSoapClient da = BP.Glo.GetOSServiceInstance();
            da.RunSQLReturnTableSAsync(sql);
            da.RunSQLReturnTableSCompleted += new EventHandler<OS.RunSQLReturnTableSCompletedEventArgs>(InitDeptInfoEvent);
        }
    }
}

