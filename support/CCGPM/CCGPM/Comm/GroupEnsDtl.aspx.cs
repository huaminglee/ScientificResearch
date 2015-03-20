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
using BP.DA;
using BP.En;
using BP.Web;
using BP.Web.Controls;
using BP.Sys; 
using System.Collections.Specialized;


namespace BP.Web.WF.Comm
{
	/// <summary>
	/// GroupEnsDtl ��ժҪ˵����
	/// </summary>
	public partial class GroupEnsDtl : WebPage
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
            if (this.IsPostBack == false)
            {
                this.Label1.Text = this.GenerCaption("��ϸ");
                this.BindData();
            }
		}
		/// <summary>
		/// ����
		/// </summary>
		public string FK_Dept
		{
			get
			{
				return (string)ViewState["FK_Dept"];
			}
			set
			{
				this.ViewState["FK_Dept"]=value;
			}
		}
        public void BindData()
        {
            string ensname = this.Request.QueryString["EnsName"];
            Entities ens = ClassFactory.GetEns(ensname);
            QueryObject qo = new QueryObject(ens);
            string url = this.Request.RawUrl;
            string[] strs = url.Split('&');
            int i = 0;
            int strsLen = strs.Length;
            //string[] mystrs= ssd
            foreach (string str in strs)
            {
                string[] mykey = str.Split('=');
                string key = mykey[0];
                string val = mykey[1];

                if (key == "FK_Dept")
                {
                    /* ȡ����Ĳ���ֵ��*/
                    if (this.FK_Dept == null)
                    {
                        this.FK_Dept = val;
                    }
                    else
                    {
                        if (this.FK_Dept.Length > val.Length)
                        {
                        }
                        else
                        {
                            this.FK_Dept = val;
                        }
                    }
                }
            }

            //			if (this.Request.RawUrl.IndexOf("FK_Dept=")!=-1)
            //				strsLen=strsLen-1;

            foreach (string str in strs)
            {
                //string mystr = str.Replace("DDL_");
                if (str.IndexOf("EnsName") != -1)
                    continue;

                string[] mykey = str.Split('=');
                string key = mykey[0];
                string val = mykey[1];

                if (key == "FK_Dept")
                {
                    val = "all";
                }
                qo.AddWhere(key, val);
                i++;
                if (i <= strsLen - 2)
                    qo.addAnd();
            }

            /* ���������� */
            if (this.FK_Dept != null)
            {
                qo.addAnd();
                qo.AddWhere("FK_Dept", " = ", this.FK_Dept);
            }
            qo.DoQuery();
            this.UCSys1.DataPanelDtl(ens,null);
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
