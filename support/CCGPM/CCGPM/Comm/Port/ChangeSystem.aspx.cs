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

namespace BP.Web.Comm
{
	/// <summary>
	/// ChangeSystem ��ժҪ˵����
	/// </summary>
	public partial class ChangeSystem : WebPage
	{

        protected void Page_Load(object sender, System.EventArgs e)
        {
            this.Label1.Text = this.GenerCaption("�л�ϵͳ");
            //this.GenerLabel(this.Label1,"�л�ϵͳ");
            this.UCSys1.BindSystems();
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
