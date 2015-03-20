using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Web;
using BP.En;
using BP.DA;
using BP.WF;
using BP.Sys;
using BP.Port;
using BP;
namespace CCFlow.WF.UC
{
    public partial class KeySearch : BP.Web.UC.UCBase3
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.Title = "流程数据查询";
            //TextBox tb = new TextBox();
            //tb.ID = "TB_Key";
            //Button btn = new Button();
            //btn.ID = "Btn_Search";
            //btn.Click += new EventHandler(btn_Click);
        }

        void btn_Click(object sender, EventArgs e)
        {
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string text = this.TextBox1.Text.Trim();
            if (string.IsNullOrEmpty(text))
                return;
            Button btn = sender as Button;
            string sql = "";
            switch (btn.ID)
            {
                case "Btn_ByWorkID":
                    int workid = 0;
                    try
                    {
                        workid = int.Parse(text);
                    }
                    catch
                    {
                        this.Alert("您输入的不是一个WorkID" + text);
                        return;
                    }
                    /*zqp 2014 12 5修改，查询语句不正确*/
                    if (this.CheckBox1.Checked)
                        sql = "SELECT A.*,B.Name as FlowName FROM V_FlowData a,WF_Flow b  WHERE A.FK_Flow=B.No AND A.OID=" + workid + " AND FlowEmps LIKE '@%" + WebUser.No + "%'";
                    else
                        sql = "SELECT A.*,B.Name as FlowName FROM V_FlowData a,WF_Flow b  WHERE A.FK_Flow=B.No AND A.OID=" + workid;
                    break;
                case "Btn_ByTitle":
                    if (this.CheckBox1.Checked)
                        sql = "SELECT A.*,B.Name as FlowName FROM V_FlowData a,WF_Flow b  WHERE A.FK_Flow=B.No AND a.Title LIKE '%" + text + "%' AND FlowEmps LIKE '@%" + WebUser.No + "%'";
                    else
                        sql = "SELECT A.*,B.Name as FlowName FROM V_FlowData a,WF_Flow b  WHERE A.FK_Flow=B.No AND a.Title LIKE '%" + text + "%'";
                    break;
                default:
                    break;
            }

            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
            {
                this.Pub1.Clear();
                this.Pub1.AddH3("&nbsp;&nbsp;竟然没有查出任何东西，真不思议。");
                return;
            }

            this.Pub1.Clear();
            this.Pub1.AddTable();
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("Idx");
            this.Pub1.AddTDTitle("流程");
            this.Pub1.AddTDTitle("标题");
            this.Pub1.AddTDTitle("发起人");
            this.Pub1.AddTDTitle("发起日期");
            this.Pub1.AddTDTitle("状态");
            this.Pub1.AddTDTitle("参与人");
            this.Pub1.AddTREnd();
            int idx = 1;
            foreach (DataRow dr in dt.Rows)
            {
                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(idx++);
                this.Pub1.AddTD(dr["FlowName"].ToString());
                this.Pub1.AddTD("<A href=\"javascript:OpenIt('" + dr["FK_Flow"] + "','" + dr["FlowEndNode"] + "','" + dr["OID"] + "');\" >" + dr["Title"].ToString() + "</a>");
                this.Pub1.AddTD(dr["FlowStarter"].ToString());
                this.Pub1.AddTD(dr["FlowStartRDT"].ToString());
                switch (int.Parse(dr["WFState"].ToString()))
                {
                    case 0:
                        this.Pub1.AddTD("未完成");
                        break;
                    case 1:
                        this.Pub1.AddTD("已完成");
                        break;
                    default:
                        this.Pub1.AddTD("未知");
                        break;
                }
                this.Pub1.AddTDBigDoc(dr["FlowEmps"].ToString());
                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTableEnd();
        }
    }
}