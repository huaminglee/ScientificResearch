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

namespace BP.Web.WF.Comm
{
	/// <summary>
	/// FilePreview ��ժҪ˵����
	/// </summary>
	public partial class FilePreview : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			string mapfilepath = Session["PreviewFilePath"] as string;
			if( mapfilepath ==null || mapfilepath=="" )
			{
				this.Response.Write( "û�м�������Դ��");
				return ;
			}

			string ext = System.IO.Path.GetExtension( mapfilepath ).ToLower();
			this.Response.Redirect( mapfilepath );
			/*
			switch( ext )
			{
				case ".gif":
				case ".jpg":
					this.Response.WriteFile( mapfilepath );
					break;
				case ".rtf":
				case ".doc":
				case ".txt":
					this.Response.Redirect( mapfilepath );
					break;
				default:
					this.Response.Write( "û���ṩ�Դ����ļ���Ԥ����");
					break;
			}
			*/

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
