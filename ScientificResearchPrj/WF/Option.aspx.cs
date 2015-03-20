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
	/// Option ��ժҪ˵����
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
				this.BPToolBar1.AddLab("slb","ѡ��ִ������");
				this.BPToolBar1.AddToolbarCheckBtnGroup("ToolbarCheckBtnGroup1");

				this.ToolbarCheckBtnGroup1.Add("Btn_StopWorkFlow","����");
				this.ToolbarCheckBtnGroup1.Add("Btn_ComeBackFlow","�ָ�ʹ��");
				this.ToolbarCheckBtnGroup1.Add("Btn_DeleteFlowByFlag","�߼�ɾ��");
				this.ToolbarCheckBtnGroup1.Add("Btn_DeleteWFByRealReal","����ɾ��");

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
					help="&nbsp;&nbsp;�������������̾������������й����У���Ҫ��ʱͣ���������磺��һ����˰�˽��д������ܳ�ʱ��û���ҵ��ˡ�";
					help+="<br>&nbsp;&nbsp;��������:<br>1)��������Ҫ�����ԭ��<br>2)����ȷ����ť��";
					break;
				case "Btn_ComeBackFlow":
					help="&nbsp;&nbsp;�������Թ�������ָ̻�����״̬��";
					help+="<br>&nbsp;&nbsp;��������:<br>1)��������Ҫ�ָ�ʹ�õ�ԭ��<br>2)����ȷ����ť��";
					break;
				case "Btn_DeleteFlowByFlag":
					help="&nbsp;&nbsp;�������Դ�������һ��ɾ����ǣ��߼�ɾ���������ϵͳ���ݴ������ݿ��С�";
					help+="<br>&nbsp;&nbsp;��������:<br>1)�������߼�ɾ����ԭ��<br>2)����ȷ����ť��";
					break;
				case "Btn_DeleteWFByRealReal":
					help="&nbsp;&nbsp;�������Դ���������ɾ�������׵�ɾ�����̵���Ϣ��";
					help+="<br>&nbsp;&nbsp;��������:<br>1)������ɾ�����̵�ԭ��<br>2)����ȷ����ť��";
					break;
				default:
					break;
			}
			this.Label1.Text=help;
		}

		#region Web ������������ɵĴ���
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: �õ����� ASP.NET Web ���������������ġ�
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// �����֧������ķ��� - ��Ҫʹ�ô���༭���޸�
		/// �˷��������ݡ�
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
			this.ResponseWriteBlueMsg("ִ�гɹ���");
			this.WinClose();
		}

		protected void Button2_Click(object sender, System.EventArgs e)
		{
			this.WinClose();
		}
	}
}
