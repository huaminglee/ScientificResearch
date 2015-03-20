using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.Web;
using BP.DA;
using BP.En;

namespace CCFlow.WF.SDKComponents
{
    public partial class SubFlowDtl : BP.Web.UC.UCBase3
    {
        #region 属性.
        public int FID
        {
            get
            {
                return int.Parse(this.Request.QueryString["FID"]);
            }
        }
        public int WorkID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["WorkID"]);
                }
                catch
                {
                    return int.Parse(this.Request.QueryString["OID"]);
                }
            }
        }
        /// <summary>
        /// 节点编号
        /// </summary>
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
                    return DBAccess.RunSQLReturnValInt("SELECT FK_Node FROM WF_GenerWorkFlow WHERE WorkID=" + this.WorkID);
                }
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            //查询出来所有子流程的数据.
            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.Retrieve(GenerWorkFlowAttr.PWorkID, this.WorkID);

            this.AddTable();
            this.AddTR();
            this.AddTDTitle("序");
            this.AddTDTitle("标题");
            this.AddTDTitle("停留节点");
            this.AddTDTitle("状态");
            this.AddTDTitle("处理人");
            this.AddTDTitle("处理时间");
            this.AddTDTitle("信息");
            //this.AddTDTitle("操作");
            this.AddTREnd();

            int idx = 0;
            foreach (GenerWorkFlow item in gwfs)
            {
                idx++;
                this.AddTR();
                this.AddTDIdx(idx);
                this.AddTD("style='word-break:break-all;'", 
                    "<a href='"+Glo.CCFlowAppPath+"WF/WFRpt.aspx?WorkID="+item.WorkID+"&FK_Flow="+item.FK_Flow+"' target=_blank >"+item.Title+"</a>");

                this.AddTD(item.NodeName);

                if ( item.WFState== WFState.Complete)
                this.AddTD("已完成");
                else
                    this.AddTD("未完成");

                this.AddTD(item.TodoEmps);
                this.AddTD(item.RDT);
                this.AddTD(item.FlowNote);
                this.AddTREnd();
            }
            this.AddTableEnd();
        }
    }
}