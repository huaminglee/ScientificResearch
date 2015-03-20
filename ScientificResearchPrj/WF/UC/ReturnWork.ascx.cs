using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using BP.WF;
using BP.Sys;
using BP.Port;
using BP.Web.Controls;
using BP.DA;
using BP.En;
using BP.Web;
namespace CCFlow.WF.UC
{
    public partial class UCReturnWork : BP.Web.UC.UCBase3
    {
        #region 属性
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
        public string TBClientID
        {
            get
            {
                try
                {
                    return TB1.ClientID;
                }
                catch
                {
                    return "ss";
                }
            }
        }
        public string Info
        {
            get
            {
                string str = this.Request.QueryString["Info"];
                if (string.IsNullOrEmpty(str))
                    return null;
                else
                    return str;
            }
        }
        #endregion 属性

        protected void Page_Load(object sender, EventArgs e)
        {
            #region 找到可以退回的节点.
            DataTable dt = null;
            if (this.IsPostBack == false)
            {
                dt = BP.WF.Dev2Interface.DB_GenerWillReturnNodes(this.FK_Node, this.WorkID, this.FID);
                if (dt.Rows.Count == 0)
                {
                    this.Pub1.Clear();
                    this.Pub1.AddFieldSet("退回错误", "系统没有找到可以退回的节点.");
                    return;
                }
            }
            #endregion 找到可以退回的节点.

            this.Page.Title = "工作退回";
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            this.ToolBar1.Add("<b>退回到:</b>");
            this.ToolBar1.AddDDL("DDL1");
            this.DDL1.Attributes["onchange"] = "OnChange(this);";
            this.ToolBar1.AddBtn("Btn_OK", "确定");
            this.ToolBar1.GetBtnByID("Btn_OK").Attributes["onclick"] = " return confirm('您确定要执行吗?');";
            this.ToolBar1.GetBtnByID("Btn_OK").Click += new EventHandler(ReturnWork_Click);

            // if (this.Request.QueryString["IsEUI"] == null)
            //{
            this.ToolBar1.AddBtn("Btn_Cancel", "取消");
            this.ToolBar1.GetBtnByID("Btn_Cancel").Click += new EventHandler(ReturnWork_Click);
            //}

            string appPath = this.Request.ApplicationPath;
            if (nd.IsBackTracking)
            {
                /*如果允许原路退回*/
                CheckBox cb = new CheckBox();
                cb.ID = "CB_IsBackTracking";
                cb.Text = "退回后是否要原路返回?";
                this.ToolBar1.Add(cb);
            }


            TextBox tb = new TextBox();
            tb.TextMode = TextBoxMode.MultiLine;
            tb.ID = "TB_Doc";
            tb.Rows = 12;
            tb.Columns = 60;

            this.Pub1.Add(tb);

            //新增
            this.Pub1.Add("<div style='float:left;display:block;width:100%'><a href=javascript:TBHelp('TB_Doc')><img src='" + BP.WF.Glo.CCFlowAppPath + "WF/Img/Emps.gif' align='middle' border=0 />选择词汇</a>&nbsp;&nbsp;</div>");

            if (this.IsPostBack == false)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    this.DDL1.Items.Add(new ListItem(dr["RecName"] + "=>" + dr["Name"].ToString(), dr["No"].ToString() + "@" + dr["Rec"].ToString()));
                }

                WorkNode wn = new WorkNode(this.WorkID, this.FK_Node);
                WorkNode pwn = wn.GetPreviousWorkNode();

                this.DDL1.SetSelectItem(pwn.HisNode.NodeID);
                this.DDL1.Enabled = true;
                Work wk = pwn.HisWork;
                if (this.Info != null)
                {
                    this.TB1.Text = this.Info;
                    //不能把审核信息保存到表单里，因为下次他的审核就不是这个信息了. 
                    //string sql = "UPDATE "+wn.HisWork.EnMap.PhysicsTable+" SET "+;
                }
                else
                {
                    /*检查是否启用了审核组件，看看审核信息里填写了什么？*/
                    BP.Sys.FrmWorkCheck fwc = new FrmWorkCheck(this.FK_Node);
                    if (fwc.HisFrmWorkCheckSta == FrmWorkCheckSta.Enable)
                    {
                        this.TB1.Text = BP.WF.Dev2Interface.GetCheckInfo(this.FK_Flow, this.WorkID, this.FK_Node);
                        if (tb.Text == "同意")
                            tb.Text = "";

                    }
                    else
                    {
                        string info = this.DDL1.SelectedItem.Text;
                        string recName = info.Substring(0, info.IndexOf('='));
                        string nodeName = info.Substring(info.IndexOf('>') + 1);
                        this.TB1.Text = string.Format("{0}同志: \n  您处理的“{1}”工作有错误，需要您重新办理．\n\n此致!!!   \n\n  {2}", recName,
                            nodeName, WebUser.Name + "\n  " + BP.DA.DataType.CurrentDataTime);
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
                    BP.WF.Node curNd = new BP.WF.Node(this.FK_Node);
                    if (curNd.FormType == NodeFormType.SheetTree)
                    {
                        this.WinClose();
                    }
                    else
                    {
                        if (this.FID == 0)
                            this.Response.Redirect("../MyFlow.aspx?FK_Flow=" + this.FK_Flow + "&WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&FID=" + this.FID, true);
                        else
                        {
                            string from = this.Request.QueryString["FromUrl"];
                            if (from == "FHLFlow")
                                this.Response.Redirect("../FHLFlow.aspx?FK_Flow=" + this.FK_Flow + "&WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&FID=" + this.FID, true);
                            else
                                this.Response.Redirect("../MyFlow.aspx?FK_Flow=" + this.FK_Flow + "&WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&FID=" + this.FID, true);
                        }
                    }
                    return;
                default:
                    break;
            }

            try
            {
                string returnInfo = this.TB1.Text;
                string reNodeEmp = this.DDL1.SelectedItemStringVal;
                bool IsBackTracking = false;
                try
                {
                    IsBackTracking = this.ToolBar1.GetCBByID("CB_IsBackTracking").Checked;
                }
                catch
                {
                }

                string[] strs = reNodeEmp.Split('@');
                //执行退回api.
                string rInfo = BP.WF.Dev2Interface.Node_ReturnWork(this.FK_Flow, this.WorkID, this.FID,
                    this.FK_Node, int.Parse(strs[0]), strs[1], returnInfo, IsBackTracking);
                this.ToMsg(rInfo, "info");
                return;
            }
            catch (Exception ex)
            {
                this.ToMsg(ex.Message, "info");
            }
        }
        public void ToMsg(string msg, string type)
        {
            string rowUrl = this.Request.RawUrl;
            if (rowUrl.Contains("&IsClient=1"))
            {
                /*说明这是vsto调用的.*/
                return;
            }

            this.Session["info"] = msg;
            this.Application["info" + WebUser.No] = msg;
            BP.WF.Glo.SessionMsg = msg;
            this.Response.Redirect(BP.WF.Glo.CCFlowAppPath + "WF/MyFlowInfo.aspx?FK_Flow=" + this.FK_Flow + "&FK_Type=" + type + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID, false);
        }
    }

}