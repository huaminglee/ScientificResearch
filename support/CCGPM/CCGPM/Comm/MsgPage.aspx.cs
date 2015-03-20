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
using BP.Web.UC;

namespace BP.Web.Comm
{
	/// <summary>
	/// ErrPage ��ժҪ˵����
	/// </summary>
	public partial class MsgPage : WebPage
	{
        private string ErrorId
        {
            get
            {
                if (ViewState["ErrorId"] == null)
                    return "info";
                else
                    return ViewState["ErrorId"].ToString();
            }
            set
            {
                ViewState["ErrorId"] = value;
            }
        }	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			
			this.Label1.Controls.Add(this.GenerLabel("��Ϣ��"));

            if (!this.IsPostBack)
            {
                try
                {
                    this.ErrorId = this.Page.Request.QueryString["errorid"];
                }
                catch
                {
                    this.ErrorId = "info";
                }
                this.UCSys1.Add(this.Msg);
            }
		}
        private string Msg
        {
            get
            {
                string msg = Session["info"] as string;
                if (msg == null)
                {
                    msg =  "@û���ҵ���Ϣ��������;�������ҵ�����"; // "@û���ҵ���Ϣ��������;�������ҵ�����";
                    BP.DA.Log.DefaultLogWriteLineWarning("@ session info msg lost .");
                }
                return msg;
            }
        }
		 

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//this.IsAuthenticate=false ;
			//
			// CODEGEN���õ����� ASP.NET Web ���������������ġ�
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

		private void Btn1_Click(object sender, System.EventArgs e)
		{
			this.WinClose();
		}
	}
}
