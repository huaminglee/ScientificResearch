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

namespace BP.Web.WF.Portal
{
	/// <summary>
	/// SendSysErr ��ժҪ˵����
	/// </summary>
	public partial class UIAsk : WebPage
	{

        protected void Page_Load(object sender, System.EventArgs e)
        {
            this.GenerCaption("�ύ����");
          //  this.GenerLabel(this.Label1, "�ύ����");
            //this.BPToolBar1.ButtonClick += new System.EventHandler(this.BPToolBar1_ButtonClick);
            if (this.IsPostBack == false)
            {
                string msg = Session["info"] as string;
                if (msg != null)
                    msg = msg.Replace("<BR>", "\n");

                this.TB_Docs.Text = " ��������:\n    �����ڼ䷢���������⣬������\n   " + msg + "\n" + this.TB_Docs.Text + "\n\n       " + Web.WebUser.Name;

                //this.TB_Sender.Text=WebUser.Name;
                this.TB_Title.Text = WebUser.Name + "���ֵ�����,�뾡����������";
                //this.BPToolBar1.AddLab("ss","Ѱ�����");
                this.BPToolBar1.AddBtn(BP.Web.Controls.NamesOfBtn.Send);
                this.BPToolBar1.AddBtn(BP.Web.Controls.NamesOfBtn.Cancel);
                this.BPToolBar1.AddSpt("sa");
                this.BPToolBar1.AddBtn(BP.Web.Controls.NamesOfBtn.Help);

                this.TB_Docs.Attributes["Width"] = "100%";
                this.TB_Docs.Attributes["Height"] = "100%";
                this.TB_Docs.Attributes["TextMode"] = "MultiLine";
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

		 

		protected void BPToolBar1_ButtonClick_1(object sender, System.EventArgs e)
		{
			try
			{
                BP.Web.Controls.ToolbarBtn btn = (BP.Web.Controls.ToolbarBtn)sender;
				switch(btn.ID)
				{
					case BP.Web.Controls.NamesOfBtn.Send:
                        //Sys.FAQ faq = new BP.Sys.FAQ(); // = new BP.Sys.Ask();
                        //faq.Title=this.TB_Title.Text ;
                        //faq.Asker =WebUser.No ;
                        //faq.Doc=this.TB_Docs.Text ;
                        //faq.RDT=DA.DataType.CurrentDataTime ;
                        //faq.Insert();
                        this.ToCommMsgPage("���������Ѿ����ͳɹ�, ���ǻᾡ��Ĵ�����, �����Ե��û�ʹ������ȥ�ҵ���������, ��л��������");
						//string script= "<script language=JavaScript>alert('���������Ѿ����ͳɹ�,���ǻᾡ��Ĵ���������л��������');</script>";
						//this.ToMsgPage( 
						//this.Response.Write(  script );
						//this.WinClose();
						break;
					case BP.Web.Controls.NamesOfBtn.Help:
						this.Helper();
						break;
					default:
						break;
				}
			}
			catch(Exception ex)
			{
				this.Alert(ex.Message);
			}
		}
	}
}
