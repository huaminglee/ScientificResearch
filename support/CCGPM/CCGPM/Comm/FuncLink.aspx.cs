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
using BP.En;
using BP.Port;
using BP.Sys;
using BP.DA;

namespace BP.Web.Comm
{
	/// <summary>
	/// FuncLink ��ժҪ˵����
	/// </summary>
	public partial class FuncLink : WebPage
	{
		public string Flag
		{
			get
			{
				return this.Request.QueryString["Flag"];
			}
		}
		protected void Page_Load(object sender, System.EventArgs e)
		{
			//string msg="";
			try
			{
				switch(this.Flag)
				{
					case "To�ؾ�OfGS":
//						string station=WebUser.HisEmp.HisStation.No;
//						if (station=="000000" || station=="00000000" || station=="0000000000" || station=="000000000000"  )
//							this.Response.Redirect("UIEns.aspx?EnsName=BP.En.�ؾ�&IsReadonly=1",false);
//						else
//							this.ToErrorPage("��û�в����˹��ܵ�Ȩ�ޣ�");
						break;						 
					case "DeleteEn":
						Entity en =ClassFactory.GetEn(this.Request.QueryString["MainEnsName"]) ;
						//Entity en =ens.GetNewEntity;
						en.PKVal=this.Request.QueryString[en.PK];
						en.Delete();						
						//this.Response.Write("@ɾ���ɹ�.");
						//this.WinClose();
						//this.Alert("ɾ���ɹ�!!!");
						//this.WinClose();
 						this.ToErrorPage("ɾ���ɹ�!!!");
						//ToErrorPage(
						//this.Alert("ɾ���ɹ�,��Ҫ��ˢ��ҳ��.");
						break;
					case "DeleteEns":
						break;
					case "DeleteEnssd":
						break;
					case "DeleteEsdsn":
						break;
					default:
						break;
				}
			}
			catch(Exception ex)
			{
				this.ToErrorPage(ex.Message);
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
	}
}
