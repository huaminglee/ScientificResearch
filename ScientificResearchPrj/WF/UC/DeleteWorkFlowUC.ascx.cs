using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF.Template;
using BP.WF;
using BP.Sys;
using BP.Port;
using BP.Web.Controls;
using BP.DA;
using BP.En;
using BP.Web;

namespace CCFlow.WF.UC
{
    public partial class DeleteWorkFlowUC : BP.Web.UC.UCBase3
    {
        #region 属性。
        public string FromUrl
        {
            get
            {
                return this.Request.QueryString["FromUrl"];
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public int FK_Node
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_Node"]);
            }
        }
        public int FID
        {
            get
            {

                return int.Parse(this.Request.QueryString["FID"]);
            }
        }
        public Int64 WorkID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["WorkID"]);
            }
        }
        public DDL DDL1
        {
            get
            {
                return this.ToolBar1.GetDDLByID("DDL1");
            }
        }
        public TextBox TB1
        {
            get
            {
                return this.Pub1.GetTextBoxByID("TB_Doc");
            }
        }
        #endregion 属性。

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.Title = "工作删除";
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            BP.WF.Node nd = new BP.WF.Node(gwf.FK_Node);
          
            if (nd.HisRunModel != RunModel.SubThread)
            {
                this.ToolBar1.Add("<b>删除方式:</b>");
                this.ToolBar1.AddDDL("DDL1");
                this.DDL1.Attributes["onchange"] = "OnChange(this);";
            }

            this.ToolBar1.AddBtn("Btn_OK", "确定");
            this.ToolBar1.GetBtnByID("Btn_OK").Attributes["onclick"] = " return confirm('您确定要执行吗?');";
            this.ToolBar1.GetBtnByID("Btn_OK").Click += new EventHandler(ReturnWork_Click);
            this.ToolBar1.AddBtn("Btn_Cancel", "取消");
            this.ToolBar1.GetBtnByID("Btn_Cancel").Click += new EventHandler(ReturnWork_Click);
            string appPath = this.Request.ApplicationPath;
            TextBox tb = new TextBox();
            tb.TextMode = TextBoxMode.MultiLine;
            tb.ID = "TB_Doc";
            tb.Rows = 15;
            tb.Columns = 50;
            this.Pub1.Add(tb);
            if (this.IsPostBack == false
                && nd.HisRunModel!= RunModel.SubThread)
            {
                if (nd.HisDelWorkFlowRole == DelWorkFlowRole.DeleteAndWriteToLog)
                {
                    /*删除并记录日志 */
                    SysEnum se = new SysEnum(BtnAttr.DelEnable, (int)DelWorkFlowRole.DeleteAndWriteToLog);
                    this.DDL1.Items.Add(new ListItem(se.Lab, se.IntKey.ToString()));
                }

                if (nd.HisDelWorkFlowRole == DelWorkFlowRole.DeleteByFlag)
                {
                    /*逻辑删除 */
                    SysEnum se = new SysEnum(BtnAttr.DelEnable, (int)DelWorkFlowRole.DeleteByFlag);
                    this.DDL1.Items.Add(new ListItem(se.Lab, se.IntKey.ToString()));
                }

                if (nd.HisDelWorkFlowRole == DelWorkFlowRole.ByUser)
                {
                    /*让用户来决定.*/
                    SysEnums ses = new SysEnums(BtnAttr.DelEnable);
                    foreach (SysEnum se in ses)
                    {
                        DelWorkFlowRole role = (DelWorkFlowRole)se.IntKey;
                        if (role == DelWorkFlowRole.None)
                            continue;
                        if (role == DelWorkFlowRole.ByUser)
                            continue;
                        this.DDL1.Items.Add(new ListItem(se.Lab, se.IntKey.ToString()));
                    }
                }
            }
        }
        void ReturnWork_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            switch (btn.ID)
            {
                case "Btn_Cancel":
                    if (this.FromUrl == null)
                        this.Response.Redirect("MyFlow.aspx?FK_Flow=" + this.FK_Flow + "&WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node, true);
                    else
                        this.Response.Redirect(this.FromUrl+".aspx?FK_Flow=" + this.FK_Flow + "&WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node, true);
                    return;
                default:
                    break;
            }

            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            BP.WF.Node nd = new BP.WF.Node(gwf.FK_Node);
            if (nd.HisRunModel == RunModel.SubThread)
            {
                BP.WF.Dev2Interface.Flow_DeleteSubThread(this.FK_Flow, this.WorkID, this.Pub1.GetTextBoxByID("TB_Doc").Text);
                this.ToMsg("子线城删除成功.","Info");
                return;
            }

            try
            {
                string info = this.TB1.Text;
                BP.WF.DelWorkFlowRole role = (BP.WF.DelWorkFlowRole)this.DDL1.SelectedItemIntVal;
                string rInfo = "";
                switch (role)
                {
                    case DelWorkFlowRole.DeleteAndWriteToLog:
                        rInfo=BP.WF.Dev2Interface.Flow_DoDeleteFlowByWriteLog(this.FK_Flow, this.WorkID, info, true);
                        break;
                    case DelWorkFlowRole.DeleteByFlag:
                        rInfo = BP.WF.Dev2Interface.Flow_DoDeleteFlowByFlag(this.FK_Flow, this.WorkID, info, true);
                        break;
                    case DelWorkFlowRole.DeleteReal:
                        rInfo = BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(this.FK_Flow, this.WorkID, true);
                        break;
                    default:
                        throw new Exception("@没有涉及到的删除情况。");
                }
                this.ToMsg(rInfo, "info");
            }
            catch (Exception ex)
            {
                this.ToMsg(ex.Message, "info");
            }
        }
        public void ToMsg(string msg, string type)
        {
            this.Session["info"] = msg;
            this.Response.Redirect("MyFlowInfo.aspx?FK_Flow=" + this.FK_Flow + "&FK_Type=" + type + "&WorkID=" + this.WorkID, false);
        }
    }
}