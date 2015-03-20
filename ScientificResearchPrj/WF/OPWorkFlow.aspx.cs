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
	/// StopWorkFlow ��ժҪ˵����
	/// </summary>
	public partial class StopWorkFlow : PageBase
	{
		#region �ؼ�
		#endregion

		#region ����
		/// <summary>
		/// ����ID
		/// </summary>
        public Int64 WorkID
		{
			get
			{
                return Int64.Parse(this.Request.QueryString["WorkID"]); 
			}
		}
		/// <summary>
		/// ���̱��
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
                    this.Alert("@��û��ѡ�����̣�������Ч��", false);
                    this.WinClose();
                    return;
                }
                WorkFlow wf = new WorkFlow(new Flow(flowNo), workId);
                if (wf.IsComplete)
                {
                    this.Alert("@�����Ѿ���ɣ�������Ч��", false);
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
                    throw new Exception("ϵͳ���ִ����������Ա��ϵ������ԭ���ǵ�ǰ������[" + flowNo + " id=" + workId + "],û����ɣ��������̱����Ѿ������������Ϣ���������Ѿ���Ϊ��Ч�����̣������ǲ�����Ϣ��ϵͳ�Ѿ�ɾ������");
                }
                else
                {
                    gwf.Retrieve();
                }

                if (gwf.WFState == WFState.Complete )
                {
                    this.Alert("@�����Ѿ����,���ܹ��Դ˲���.", false);
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
                this.Label1.Text = "��ǰ����״̬:" + gwf.WFState;

                Flow fl = new Flow(gwf.FK_Flow);
                //��ʾ��־��Ϣ		
                StartWork sw = (StartWork)fl.HisStartNode.HisWork;
                sw.OID = workId;
                if (sw.IsExits == false)
                {
                    gwf.Delete();
                    throw new Exception("@��ʼ�ڵ��Ѿ�����ɾ��.���̳��ִ���, ���������Ѿ�ʧЧ, ����رմ��ڷ���ϵͳ,ˢ�¼�¼.");
                }
                sw.Retrieve();
                

                // �ж������ܲ��ܹ�ɾ����Ȩ��.

                /* 
                 * �����  4 ��˰��������. 
                 * �������տ���������,
                 * ������mdg ����.
                 * */
                if (fl.FK_FlowSort == "4")
                {
                    /* ������ */
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
				this.BPToolBar1.AddBtn("Btn_StopWorkFlow","ǿ����ֹ");
				this.BPToolBar1.AddBtn("Btn_ComeBackFlow","�ָ�ʹ��");
				this.BPToolBar1.AddBtn("Btn_DeleteFlowByFlag","�߼�ɾ��");
				this.BPToolBar1.AddBtn("Btn_DeleteWFByRealReal","����ɾ��");
				this.BPToolBar1.AddBtn(BP.Web.Controls.NamesOfBtn.Close);
				this.SetState();
			}
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
						msg="@��ֹ���̳��ִ���.:";
					//	wf.DoStopWorkFlow(this.TB1.Text);
						// ������Ϣ�������Ա��
						title="ǿ����ֹ["+gwf.Title+"]����֪ͨ";
						docs=this.TB1.Text;
						//WFPubClass.SendMsg( new WorkNodes(gwf.HisFlow,workId),title,docs);

						this.ResponseWriteBlueMsg("@ǿ����ֹ���̳ɹ�.������ϵͳ��Ϣ�������ϵ������Ա��"); 
						break;
					case "Btn_DeleteFlowByFlag":
						msg="@�߼�ɾ�����̳��ִ���.:";
//						wf.DoDeleteWorkFlowByFlag(this.TB1.Text);
						// ������Ϣ�������Ա��
						title="�߼�ɾ��["+gwf.Title+"]����֪ͨ";
						docs=this.TB1.Text;
					//	WFPubClass.SendMsg( new WorkNodes(gwf.HisFlow,workId),title,docs);
						this.ResponseWriteBlueMsg("@�߼�ɾ�����̳ɹ�,������ϵͳ��Ϣ�������ϵ������Ա.");
						break;
					case "Btn_ComeBackFlow":
						msg="@�ָ�ʹ�����̳��ִ���.:";
                        wf.DoComeBackWorkFlow(this.TB1.Text); 

						// ������Ϣ�������Ա��
						title="�ָ�ʹ������["+gwf.Title+"]����֪ͨ";
						docs=this.TB1.Text;						
						//WFPubClass.SendMsg( new WorkNodes(gwf.HisFlow,workId),title,docs);
						this.ResponseWriteBlueMsg("@�ָ�ʹ�����̳ɹ�,������ϵͳ��Ϣ�������ϵ������Ա");
						break;
					case "Btn_DeleteWFByRealReal":
						msg="@����ɾ�����̳��ִ���.:";
                        wf.DoDeleteWorkFlowByReal(true);
						// ������Ϣ�������Ա��
						title="����ɾ��["+gwf.Title+"]����֪ͨ";
						docs=this.TB1.Text;						
						 
						 
						this.ResponseWriteBlueMsg("@����ɾ�����̳ɹ�...");
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
