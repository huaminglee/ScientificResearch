using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System;
using BP.Web;
using BP.WF;
using BP.En;
using BP.Sys;
namespace CCFlow.WF
{
    public partial class WF_Chart : BP.Web.WebPage
    {
        #region attrs
        public Int64 WorkID
        {
            get
            {
                try
                {
                    return Int64.Parse(this.Request.QueryString["WorkID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        public Int64 FID
        {
            get
            {
                try
                {
                    return Int64.Parse(this.Request.QueryString["FID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        public int FK_Node
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["FK_Node"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public new string DoType
        {
            get
            {
                return this.Request.QueryString["DoType"];
            }
        }
        #endregion attrs

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.WorkID != 0)
            {
                string url =  "./WorkOpt/OneWork/ChartTrack.aspx?FID=" + this.FID + "&FK_Flow=" + this.FK_Flow + "&WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node;
                this.Response.Redirect(url, true);
                return;
            }

            switch (this.DoType)
            {
                case "Chart": //流程图形.
                    FlowChart(this.FK_Flow);
                    this.Title = "流程图";
                    break;
                case "DT": // 流程运行数据。。
                    FlowDT(this.FK_Flow, this.WorkID);
                    break;
                case "ALS": // 流程数据分析。
                    FlowALS(this.FK_Flow);
                    break;
                default:
                    //    throw new Exception("参数不完整.");
                    break;
            }
        }
        public void FlowChart(string fk_flow)
        {
        }
        public void FlowDT(string fk_flow, Int64 workid)
        {
        }
        public void FlowALS(string fk_flow)
        {
        }
    }
}