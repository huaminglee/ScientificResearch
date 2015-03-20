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
using BP.Port;

namespace BP.Web.WF
{
	/// <summary> 
	/// StopWorkFlow 的摘要说明。
	/// </summary>
	public partial class StopWorkFlow : PageBase
	{
		#region 控件
		#endregion

		#region 变量
		/// <summary>
		/// 工作ID
		/// </summary>
        public Int64 WorkID
		{
			get
			{
                return Int64.Parse(this.Request.QueryString["WorkID"]); 
			}
		}
		/// <summary>
		/// 流程编号
		/// </summary>
		public string  FlowNo
		{
			get
			{
				return  this.Request.QueryString["FK_Flow"]; 
			}
		}
		#endregion

		#region contral 
		public BP.Web.Controls.ToolbarBtn Btn_StopWorkFlow
		{
			get
			{
				return this.BPToolBar1.GetBtnByKey("Btn_StopWorkFlow");
			}
		}
		public BP.Web.Controls.ToolbarBtn Btn_ComeBackFlow
		{
			get
			{
				return this.BPToolBar1.GetBtnByKey("Btn_ComeBackFlow");
			}
		}
		public BP.Web.Controls.ToolbarBtn Btn_DeleteFlowByFlag
		{
			get
			{
				return this.BPToolBar1.GetBtnByKey("Btn_DeleteFlowByFlag");
			}
		}
		public BP.Web.Controls.ToolbarBtn Btn_DeleteWFByRealReal
		{
			get
			{
				return this.BPToolBar1.GetBtnByKey("Btn_DeleteWFByRealReal");
			}
		}
		
		/// <summary>
		/// Btn_Cancel
		/// </summary>
		public BP.Web.Controls.ToolbarBtn Btn_Cancel
		{
			get
			{
				return this.BPToolBar1.GetBtnByKey("Btn_Cancel");
			}
		}
		#endregion 

        private void SetState()
        {
            try
            {
                this.Btn_ComeBackFlow.Enabled = false;
                this.Btn_DeleteFlowByFlag.Enabled = false;
                this.Btn_DeleteWFByRealReal.Enabled = false;
                this.Btn_StopWorkFlow.Enabled = false;
                Int64 workId = Int64.Parse(this.Request.QueryString["WorkID"]);
                //int nodeId=int.Parse(this.Request.QueryString["WorkID"]);
                string flowNo = this.Request.QueryString["FK_Flow"];
                if (workId == 0)
                {
                    this.Alert("@您没有选择流程，操作无效。", false);
                    this.WinClose();
                    return;
                }
                WorkFlow wf = new WorkFlow(new Flow(flowNo), workId);
                if (wf.IsComplete)
                {
                    this.Alert("@流程已经完成，操作无效。", false);
                    this.WinClose();
                    return;
                }
                GenerWorkFlow gwf = new GenerWorkFlow();
                //workId,flowNo
                gwf.WorkID = workId;
                gwf.FK_Flow = flowNo;
                if (gwf.IsExits == false)
                {
                    wf.DoDeleteWorkFlowByReal(true);
                    throw new Exception("系统出现错误，请与管理员联系：错误原因是当前的流程[" + flowNo + " id=" + workId + "],没有完成，但是流程表里已经不存在这此信息，此流程已经成为无效的流程，可能是测试信息，系统已经删除它。");
                }
                else
                {
                    gwf.Retrieve();
                }

                if (gwf.WFState == WFState.Complete )
                {
                    this.Alert("@流程已经完成,不能够对此操作.", false);
                    this.WinClose();
                    return;
                }
                else if (gwf.WFState == 0)
                {
                    this.Btn_DeleteFlowByFlag.Enabled = true;
                    this.Btn_StopWorkFlow.Enabled = true;
                }
                else
                {
                    throw new Exception("error ");
                }
                this.Label1.Text = "当前流程状态:" + gwf.WFState;

                Flow fl = new Flow(gwf.FK_Flow);
                //显示日志信息		
                StartWork sw = (StartWork)fl.HisStartNode.HisWork;
                sw.OID = workId;
                if (sw.IsExits == false)
                {
                    gwf.Delete();
                    throw new Exception("@开始节点已经物理删除.流程出现错误, 此条流程已经失效, 请你关闭窗口返回系统,刷新记录.");
                }
                sw.Retrieve();
                

                // 判断流程能不能够删除的权限.

                /* 
                 * 如果是  4 涉税审批流程. 
                 * 就让征收科室来处理,
                 * 否则让mdg 处理.
                 * */
                if (fl.FK_FlowSort == "4")
                {
                    /* 法征科 */
                    if (WebUser.FK_Dept == "000003")
                    {
                        this.Btn_DeleteWFByRealReal.Enabled = true;
                    }
                }
                else
                {
                    if (WebUser.FK_Dept == "000001")
                    {
                        this.Btn_DeleteWFByRealReal.Enabled = true;
                    }
                }
                //this.UCWFRpt1.BindDataV2(wf);
            }
            catch (Exception ex)
            {
                this.Alert(ex);
                this.WinClose();
            }
        }
		/// <summary>
		/// ss
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			this.BPToolBar1.ButtonClick += new System.EventHandler(this.Btn_Click_Click);
			if (this.IsPostBack==false)
			{
				this.BPToolBar1.AddBtn("Btn_StopWorkFlow","强制中止");
				this.BPToolBar1.AddBtn("Btn_ComeBackFlow","恢复使用");
				this.BPToolBar1.AddBtn("Btn_DeleteFlowByFlag","逻辑删除");
				this.BPToolBar1.AddBtn("Btn_DeleteWFByRealReal","物理删除");
				this.BPToolBar1.AddBtn(BP.Web.Controls.NamesOfBtn.Close);
				this.SetState();
			}
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
 		 
		private void Btn_Click_Click(object sender, System.EventArgs e)
		{
			string msg="" ; 
			try
			{
                Int64 workId = Int64.Parse(this.Request.QueryString["WorkID"]);
				GenerWorkFlow gwf = new GenerWorkFlow(workId);
				WorkFlow wf = new WorkFlow(new Flow(gwf.FK_Flow), workId);
				BP.Web.Controls.ToolbarBtn  btn = (BP.Web.Controls.ToolbarBtn)sender;
				string title, docs;
				switch(btn.ID)
				{
					case "Btn_StopWorkFlow":
						msg="@终止流程出现错误.:";
					//	wf.DoStopWorkFlow(this.TB1.Text);
						// 发送消息到相关人员。
						title="强制终止["+gwf.Title+"]流程通知";
						docs=this.TB1.Text;
						//WFPubClass.SendMsg( new WorkNodes(gwf.HisFlow,workId),title,docs);

						this.ResponseWriteBlueMsg("@强制终止流程成功.并发送系统消息到流程上的相关人员。"); 
						break;
					case "Btn_DeleteFlowByFlag":
						msg="@逻辑删除流程出现错误.:";
//						wf.DoDeleteWorkFlowByFlag(this.TB1.Text);
						// 发送消息到相关人员。
						title="逻辑删除["+gwf.Title+"]流程通知";
						docs=this.TB1.Text;
					//	WFPubClass.SendMsg( new WorkNodes(gwf.HisFlow,workId),title,docs);
						this.ResponseWriteBlueMsg("@逻辑删除流程成功,并发送系统消息到流程上的相关人员.");
						break;
					case "Btn_ComeBackFlow":
						msg="@恢复使用流程出现错误.:";
                        wf.DoComeBackWorkFlow(this.TB1.Text); 

						// 发送消息到相关人员。
						title="恢复使用流程["+gwf.Title+"]流程通知";
						docs=this.TB1.Text;						
						//WFPubClass.SendMsg( new WorkNodes(gwf.HisFlow,workId),title,docs);
						this.ResponseWriteBlueMsg("@恢复使用流程成功,并发送系统消息到流程上的相关人员");
						break;
					case "Btn_DeleteWFByRealReal":
						msg="@物理删除流程出现错误.:";
                        wf.DoDeleteWorkFlowByReal(true);
						// 发送消息到相关人员。
						title="物理删除["+gwf.Title+"]流程通知";
						docs=this.TB1.Text;						
						 
						 
						this.ResponseWriteBlueMsg("@物理删除流程成功...");
						this.WinClose();
						break;
					case "Btn_Close":
						this.WinClose();
						return;					 
					default:
						break;
				}
				this.SetState();				
			}
			catch(Exception ex)
			{
				BP.DA.Log.DefaultLogWriteLine(BP.DA.LogType.Error,msg+ex.Message) ; 
				this.ResponseWriteRedMsg(msg+ex.Message) ; 
			}		
		}
	}
}
