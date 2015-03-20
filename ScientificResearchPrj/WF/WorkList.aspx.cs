using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BP.WF;
using BP.En;

namespace BP.Web.WF
{
	/// <summary>
	/// WorkList 的摘要说明。
	/// </summary>
    public partial class WorkList : BP.Web.WebPage
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            this.Page.RegisterClientScriptBlock("s",
          "<link href='./Comm/Style/Table" + BP.Web.WebUser.Style + ".css' rel='stylesheet' type='text/css' />");

            string fk_flow = this.Request.QueryString["FK_Flow"];
            BP.WF.Flow fl = new BP.WF.Flow(fk_flow);
            this.BindIt(fl);
           
            ///this.GenerLabel(this.Label1, fl.Name);
            // this.UCFlow1.BindWorkList(fl);
        }
        /// <summary>
        /// 流程
        /// </summary>
        /// <param name="fl"></param>
        public void BindIt(BP.WF.Flow fl)
        {
            GenerWorkFlows gwfs = new GenerWorkFlows();
            QueryObject qo = new QueryObject(gwfs);
            qo.AddWhereInSQL(GenerWorkFlowAttr.WorkID, " SELECT WorkID FROM WF_GenerWorkFlow WHERE FK_Node IN ( SELECT FK_Node FROM WF_GenerWorkerlist WHERE FK_Emp='" + Web.WebUser.No + "' AND FK_Flow='" + fl.No + "' AND WORKID=WF_GenerWorkFlow.WORKID AND ISENABLE=1 ) ");
            qo.addOrderBy(GenerWorkFlowAttr.FK_Node, GenerWorkFlowAttr.WorkID);
            qo.DoQuery();


            if (gwfs.Count == 0)
            {
                this.UCFlow1.AddMsgOfInfo("流程" + fl.Name, "没有您[" + WebUser.No + WebUser.Name + "]的当前待办工作.");
            }

            if (gwfs.Count == 1)
            {
                this.Response.Redirect("MyFlow.aspx?FK_Flow=" + fl.No + "&WorkID=" + gwfs[0].GetValByKey(GenerWorkFlowAttr.WorkID).ToString(), true);
                return;
            }

            this.UCFlow1.AddTable();
            this.UCFlow1.AddCaptionLeft(fl.Name);
            this.UCFlow1.AddTR();
            this.UCFlow1.AddTDTitle("IDX");
            this.UCFlow1.AddTDTitle( "标题");
            this.UCFlow1.AddTDTitle("停留节点");
            this.UCFlow1.AddTDTitle("发起日期");
            // this.UCFlow1.AddTDTitle("预先警日期");
            this.UCFlow1.AddTDTitle("应完成日期");
            this.UCFlow1.AddTDTitle("记录人");
            this.UCFlow1.AddTDTitle("工作序号");
            this.UCFlow1.AddTREnd();
            int i = 0;
            foreach (GenerWorkFlow gwf in gwfs)
            {
                i++;
                //this.AddTR("onmouseover='TROver(this)' onmouseout='TROut(this)' onclick=\"WinOpen('MyFlow.aspx?FK_Flow="+fl.No+"&WorkID="+dr[WorkAttr.OID ].ToString()+"' )\" " );
                this.UCFlow1.AddTR("title='在列表中根据标题选择您的代办工作' onmouseover='TROver(this)' onmouseout='TROut(this)' onclick=\"javascript:window.location.href='MyFlow.aspx?FK_Flow=" + fl.No + "&WorkID=" + gwf.WorkID + "'\" ");
                this.UCFlow1.AddTDIdx(i);
                this.UCFlow1.AddTD(gwf.Title);
                this.UCFlow1.AddTD(gwf.NodeName);
                this.UCFlow1.AddTD(gwf.RDT);
                this.UCFlow1.AddTD("");

                // this.UCFlow1.AddTD(gwf.w);
                // this.UCFlow1.AddTD(gwf.RDT);

                this.UCFlow1.AddTD(gwf.StarterName);
                //this.UCFlow1.AddTD(gwf.FK_Taxpayer);
                //this.UCFlow1.AddTD(gwf.TaxpayerName);
                this.UCFlow1.AddTD(gwf.WorkID);
                this.UCFlow1.AddTREnd();
            }
            this.UCFlow1.AddTableEnd();
        }

        #region Web 窗体设计器生成的代码
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: 该调用是 ASP.NET Web 窗体设计器所必需的。
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// 设计器支持所需的方法 - 不要使用代码编辑器修改
        /// 此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion
    }
}
