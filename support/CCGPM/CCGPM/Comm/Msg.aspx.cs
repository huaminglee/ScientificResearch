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
using BP.Web.Controls;
using BP.DA;
using BP.Port;

namespace BP.Web.Comm
{
	/// <summary>
	/// Style ��ժҪ˵����
	/// </summary>
	public partial class UIMsg: PageBase
	{
		protected BP.Web.Controls.DDL DDL1;
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (this.IsPostBack==false)
			{
				this.BPToolBar1.AddLab("msg","ϵͳ��Ϣ");


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
