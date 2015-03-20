using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Data;
//using ULCode.QDA;
using System.Text;
using BP.DA;

namespace wwwroot.App_Ctrl
{
    public partial class SelectUser_Jq : System.Web.UI.Page
    {
        /// 对外6个接口
        private bool SelOnlyOne
        {
            get { return Request.QueryString["SelOnlyOne"] != null; }
        }
        private string SelUsers
        {
            get
            {
                return Convert.ToString(Request.QueryString["In"]);
            }
        }

        private DataTable GetAllStations()
        {
            string sSql = "select A.No,'('+B.Name+')'+A.Name as Name from Port_Station A"
                          + " inner join Port_StationType B on A.StaGrade=B.No"
                          + " order by A.FK_StationType";
            if (BP.Sys.SystemConfig.AppCenterDBType == BP.DA.DBType.MySQL)
            {
                sSql = "select A.No,CONCAT('(',B.Name,')',A.Name) as Name from Port_Station A"
                        + " inner join Port_StationType B on A.StaGrade=B.No"
                        + " order by A.FK_StationType";
            }
            return DBAccess.RunSQLReturnTable(sSql);
        }

        private DataTable GetAllEmps()
        {
            string sSql = "select * from Port_Emp";
            return DBAccess.RunSQLReturnTable(sSql);
        }

        /*初始化装载部分*/
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindListCtrl(this.GetAllStations(), this.ddlStation, "Name", "No", null, "0#--所有职位--", null);
                this.LoadSelectedEmployees();
            }
        }
        private void LoadSelectedEmployees()
        {
            string selUsers = this.SelUsers;
            if (!String.IsNullOrEmpty(selUsers))
            {
                DataTable dt = this.GetAllEmps();

                if (dt != null)
                {
                    for (int i = dt.Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow dr = dt.Rows[i];
                        if (!String.Format(",{0},", selUsers).Contains(String.Format(",{0},", dr["No"])))
                        {
                            dt.Rows.RemoveAt(i);
                            dr.Delete();
                        }
                    }
                }

                BindListCtrl(dt, this.lbRight, "Name", "No", null, null, null);
            }
        }


        /// <summary>
        /// 直接将Sql语句（格式：id+name，两个字段）
        /// </summary>
        /// <param name="dt">DataTable数据类型</param>
        /// <param name="listCtrl">列表控件</param>
        /// <param name="defaultValue">默认值(格式：值#文本)</param>
        /// <param name="SelectedValue">选择值</param>
        public static void BindListCtrl(System.Data.DataTable dt, object listCtrl, string textField, string valueField, string textFormat, string defaultValue, string SelectedValue)
        {
            BindListCtrl(GetListItems(dt, textField, valueField), listCtrl, textFormat, defaultValue, SelectedValue);
        }
        /// <summary>
        /// 直接将Sql语句（格式：id+name，两个字段）
        /// </summary>
        /// <param name="sSql">listItems对象数组</param>
        /// <param name="listCtrl">列表控件</param>
        /// <param name="defaultValue">默认值(格式：值#文本)</param>
        /// <param name="SelectedValue">选择值</param>
        public static void BindListCtrl(ListItem[] listItems, object listCtrl, string textFormat, string defaultValue, string SelectedValue)
        {
            if (String.IsNullOrEmpty(textFormat))
            {
                textFormat = "{0}";
            }
            if (listCtrl is ListControl)
            {
                ListControl lc = (ListControl)listCtrl;
                lc.Items.Clear();
                if (!String.IsNullOrEmpty(defaultValue))
                {
                    string[] arr_d = defaultValue.Split('#');
                    lc.Items.Add(new ListItem(arr_d[1], arr_d[0]));
                }
                foreach (ListItem li in listItems)
                {
                    li.Text = String.Format(textFormat, li.Text);
                }
                lc.Items.AddRange(listItems);
                if (!String.IsNullOrEmpty(SelectedValue))
                {
                    lc.SelectedValue = SelectedValue;
                }
            }
            else if (listCtrl is HtmlSelect)
            {
                HtmlSelect lc = (HtmlSelect)listCtrl;
                lc.Items.Clear();
                if (!String.IsNullOrEmpty(defaultValue))
                {
                    string[] arr_d = defaultValue.Split('#');
                    lc.Items.Add(new ListItem(arr_d[1], arr_d[0]));
                }
                foreach (ListItem li in listItems)
                {
                    li.Text = String.Format(textFormat, li.Text);
                }
                lc.Items.AddRange(listItems);
                if (!String.IsNullOrEmpty(SelectedValue))
                {
                    lc.Value = SelectedValue;
                }
            }
        }

        public static ListItem[] GetListItems(DataTable dt, string textField, string valueField)
        {
            if (dt == null) return null;
            List<ListItem> li = new List<ListItem>();
            foreach (DataRow dr in dt.Rows)
            {
                li.Add(new ListItem(Convert.ToString(dr[textField]), Convert.ToString(dr[valueField])));
            }
            return li.ToArray();
        }      
    }
}