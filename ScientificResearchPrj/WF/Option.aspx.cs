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
using BP.Web.Controls;
using BP.WF;
using BP.DA;

namespace BP.Web.WF.WF
{
	/// <summary>
	/// Option 的摘要说明。
	/// </summary>
	public partial class Option : BP.Web.WebPage
	{
		public ToolbarCheckBtnGroup ToolbarCheckBtnGroup1
		{
			get
			{
				return (ToolbarCheckBtnGroup)this.BPToolBar1.GetToolbarCheckBtnGroupByKey("ToolbarCheckBtnGroup1") ;
			}
		}
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			
			if (this.IsPostBack==false)
			{
				this.BPToolBar1.AddLab("slb","选定执行内容");
				this.BPToolBar1.AddToolbarCheckBtnGroup("ToolbarCheckBtnGroup1");

				this.ToolbarCheckBtnGroup1.Add("Btn_StopWorkFlow","挂起");
				this.ToolbarCheckBtnGroup1.Add("Btn_ComeBackFlow","恢复使用");
				this.ToolbarCheckBtnGroup1.Add("Btn_DeleteFlowByFlag","逻辑删除");
				this.ToolbarCheckBtnGroup1.Add("Btn_DeleteWFByRealReal","物理删除");

				//this.BPToolBar1.AddSpt("sd");
				//this.BPToolBar1.AddBtn(BP.Web.Controls.NamesOfBtn.Help);

				this.ToolbarCheckBtnGroup1.Items[0].Selected=true;
				this.ToolbarCheckBtnGroup1.AutoPostBack=true;
				this.SetState();
			}
			this.BPToolBar1.ButtonClick+=new EventHandler(BPToolBar1_ButtonClick);

		}
		public void SetState()
		{
			string id=this.ToolbarCheckBtnGroup1.SelectedCheckButton.ID;
			string help="";
			switch(id)
			{
				case "Btn_StopWorkFlow":
					help="&nbsp;&nbsp;帮助：挂起流程就是流程在运行过程中，需要暂时停下来。比如：对一个纳税人进行处罚，很长时间没有找到人。";
					help+="<br>&nbsp;&nbsp;操作步骤:<br>1)请输入需要挂起的原因。<br>2)按下确定按钮。";
					break;
				case "Btn_ComeBackFlow":
					help="&nbsp;&nbsp;帮助：对挂起的流程恢复正常状态。";
					help+="<br>&nbsp;&nbsp;操作步骤:<br>1)请输入需要恢复使用的原因。<br>2)按下确定按钮。";
					break;
				case "Btn_DeleteFlowByFlag":
					help="&nbsp;&nbsp;帮助：对此流程做一个删除标记，逻辑删除后的流程系统数据存在数据库中。";
					help+="<br>&nbsp;&nbsp;操作步骤:<br>1)请输入逻辑删除的原因。<br>2)按下确定按钮。";
					break;
				case "Btn_DeleteWFByRealReal":
					help="&nbsp;&nbsp;帮助：对此流程物理删除，彻底的删除流程的信息。";
					help+="<br>&nbsp;&nbsp;操作步骤:<br>1)请输入删除流程的原因。<br>2)按下确定按钮。";
					break;
				default:
					break;
			}
			this.Label1.Text=help;
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

		private void BPToolBar1_ButtonClick(object sender, EventArgs e)
		{
			//switch(this.ToolbarCheckBtnGroup1.
			this.SetState();
		}

		protected void Button1_Click(object sender, System.EventArgs e)
		{
			string id=this.ToolbarCheckBtnGroup1.SelectedCheckButton.ID;
			string help="";
            Int64 WorkID = Int64.Parse(this.Request.QueryString["WorkID"]);
			string fk_flow=  this.Request.QueryString["FK_Flow"] ;
			Flow fl = new Flow(fk_flow);
			WorkFlow wf = new WorkFlow(fl, WorkID);
			//Node nd = new Node(t
			switch(id)
			{
                //case "Btn_StopWorkFlow":
                //    wf.DoStopWorkFlow(this.TextBox1.Text);
                //    break;
				case "Btn_ComeBackFlow":
                    wf.DoComeBackWorkFlow(this.TextBox1.Text);
					break;
                //case "Btn_DeleteFlowByFlag":
                //    wf.DoDeleteWorkFlowByFlag(this.TextBox1.Text);
                //    break;
                //case "Btn_DeleteWFByRealReal":
                //    wf.DoDeleteWorkFlowByReal(); 
                //    break;
				default:
					break;
			}
			//this.Label1.Text=help;
			this.ResponseWriteBlueMsg("执行成功。");
			this.WinClose();
		}

		protected void Button2_Click(object sender, System.EventArgs e)
		{
			this.WinClose();
		}
	}
}
