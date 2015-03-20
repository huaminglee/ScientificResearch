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
using BP.Sys;
using BP.Web.Controls;

namespace BP.Web.Comm.Port
{
	/// <summary>
	/// FAQ ��ժҪ˵����
	/// </summary>
	public partial class UIFAQ : WebPage
	{
        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (this.RefOID == 0)
            {
                this.Label1.Text = this.GenerCaption("ϵͳXP=><a href='Ask.aspx' >�������</a>");
                //	this.GenerLabel(this.Label1,"");
                this.Bind();
            }
            else
            {
                this.Label1.Text = this.GenerCaption("ϵͳXP=><a href='FAQ.aspx' >�����б�</a>=><a href='Ask.aspx' >�������</a>");
                //this.GenerLabel(this.Label1,"");
                this.BindFAQ();
                this.UCSys1.GetBtnByID("Btn_Submint").Click += new EventHandler(btn_Click);
            }
        }
		public void BindFAQ()
		{
            //FAQ en = new FAQ(this.RefOID) ;
            //if (this.IsPostBack==false)
            //    en.Update(FAQAttr.ReadNum, en.ReadNum+1) ;

            //this.UCSys1.Controls.Clear();
            //this.UCSys1.AddTable("width='100%'");
            //this.UCSys1.Add("<TR>");
            //this.UCSys1.AddTDBar("<b>�����:</b>"+en.AskerText +"&nbsp;&nbsp;&nbsp;&nbsp;<b>ʱ��:</b>"+en.RDT +"&nbsp;&nbsp;&nbsp;&nbsp;<b>�Ķ��˴�:</b>"+en.ReadNum ) ;
            //this.UCSys1.Add("</TR>");

            //this.UCSys1.Add("<TR>");
            //this.UCSys1.AddTDBar("<b>����:</b>"+en.Title ) ;
            //this.UCSys1.Add("</TR>");

            //this.UCSys1.Add("<TR>");
            //this.UCSys1.AddTD( en.DocsHtml+"<br><br><br>" ) ;
            //this.UCSys1.Add("</TR>");

            //FAQDtls dtls = new FAQDtls(this.RefOID);
            //int i =0;
            //foreach(FAQDtl dtl in dtls)
            //{
            //    i++;
            //    this.UCSys1.Add("<TR>");

            //    if (dtl.Answer==WebUser.No)
            //        this.UCSys1.AddTDBar("��"+i+"¥, &nbsp;&nbsp;" +dtl.AnswerText +"&nbsp;&nbsp;" +dtl.RDT+" &nbsp;<a  href=\"javascript:DeleteDtl('"+dtl.OID+"')\" ><img src='../../Images/Btn/Delete.gif' border=0 >ɾ��</a><a href=\"javascript:Edit( '"+dtl.Docs+"',  '"+dtl.OID+"')\" ><img src='../../Images/Btn/Edit.gif' border=0 >�޸�</a>") ;
            //    else
            //        this.UCSys1.AddTDBar("��"+i+"¥, &nbsp;&nbsp;" +dtl.AnswerText +"&nbsp;&nbsp;" +dtl.RDT+" &nbsp;<a  href=\"javascript:Replay('"+dtl.Docs+"')\"   ><img src='../../Images/Btn/Replay.gif' border=0 >�ظ�</a>") ;

            //    this.UCSys1.Add("</TR>");

            //    this.UCSys1.Add("<TR>");
            //    this.UCSys1.AddTD( dtl.DocsHtml+"<br><br><br>" );
            //    this.UCSys1.Add("</TR>");
            //}

            //this.UCSys1.Add("<TR>");
            //this.UCSys1.AddTDTitle("�ظ�");
            //this.UCSys1.Add("</TR>");

            //this.UCSys1.Add("<TR>");
            //this.UCSys1.Add("<TD Height='200px' >");
			
            //TB tb = new TB();
            //tb.ID="TB_Docs";

            //tb.Text="\n\n\n\n\n\n\n--------\n "+WebUser.Name;
            //tb.TextMode=TextBoxMode.MultiLine;
            //tb.Columns=0;
            //tb.Rows=0;
            //tb.Attributes["Width"]="100%";
            //tb.Attributes["Height"]="100%"; 
            //tb.Attributes["style"]="height:100%;width:100%;";
            //this.UCSys1.Add(tb) ;

            //this.UCSys1.Add("</TD>");
            //this.UCSys1.Add("</TR>");

            //this.UCSys1.Add("<TR>");
            //this.UCSys1.Add("<TD>&nbsp;&nbsp;&nbsp;&nbsp;");
            //Btn btn = new Btn();
            //btn.ID="Btn_Submint";
            //btn.Text="�ύ";
            ////btn.Click+=new EventHandler(btn_Click);
            //this.UCSys1.Add( btn) ;
            //this.UCSys1.Add("</TD>");
            //this.UCSys1.Add("</TR>");
            //this.UCSys1.Add("</Table>");
		}
		public void Bind()
		{
            //this.UCSys1.AddTable();
            //FAQs ens = new FAQs();
            //ens.RetrieveAll(FAQAttr.OID);
            //int i = 0 ;
            //this.UCSys1.AddTable();
            //this.UCSys1.Add("<TR>");
            //this.UCSys1.AddTDTitle( "ID" );
            //this.UCSys1.AddTDTitle( "�����" );
            //this.UCSys1.AddTDTitle( "�������" );
            //this.UCSys1.AddTDTitle( "����" );
            //this.UCSys1.AddTDTitle( "�Ķ�/�ظ���" );
            //this.UCSys1.Add("</TR>");
            //foreach(FAQ en in ens)
            //{
            //    i++;
            //    this.UCSys1.Add("<TR title='"+en.Docs+"' onmouseover='TROver(this)' onmouseout='TROut(this)' >");
            //    this.UCSys1.AddTD( i  );
            //    this.UCSys1.AddTD( en.AskerText  );
            //    this.UCSys1.AddTD( en.RDT  );
            //    this.UCSys1.Add( "<TD class='TD' width='50%' ><a href='FAQ.aspx?RefOID="+en.OID+"' >"+en.Title + "</a></td>");
            //    this.UCSys1.AddTD(  en.ReadNum+"/"+en.DtlNum   );
            //    this.UCSys1.Add("</TR>");
            //}
            //this.UCSys1.Add("</Table>");
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

		private void btn_Click(object sender, EventArgs e)
		{
            //FAQDtl dtl = new FAQDtl();
            //dtl.FK_FAQ=this.RefOID;
            //dtl.Answer=WebUser.No;
            //dtl.Docs=this.UCSys1.GetTBByID("TB_Docs").Text;
            //dtl.Insert();

            //FAQ en = new FAQ(this.RefOID);
            //en.OID=this.RefOID;
            //en.Update(FAQAttr.DtlNum, en.DtlNum+1);

            //this.BindFAQ();
		}
	}
}
