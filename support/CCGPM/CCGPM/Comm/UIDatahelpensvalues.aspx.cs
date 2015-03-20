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
using BP.Sys;
using BP.DA;
using BP.En;

namespace BP.Web.Comm.UI
{
	/// <summary>
	/// HelperOfTB ��ժҪ˵����
	/// </summary>
	public partial class UIDataHelpEnsValues : PageBase
	{
		protected System.Web.UI.WebControls.Label Label1;
		  
		/// <summary>
		/// ������Key. 
		/// ����ens �İ���
		/// </summary>
		public string RefKey 
		{
			get
			{
				return this.Request.QueryString["RefKey"];
			}
		}
		/// <summary>
		/// ������RefText. 
		/// ����ens �İ���
		/// </summary>
		public string RefText
		{
			get
			{
				return this.Request.QueryString["RefText"];
			}			 
		}
		/// <summary>
		/// ������
		/// </summary>
		public string EnsName
		{
			get
			{
				return this.Request.QueryString["EnsName"];
			}
		}	 
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (this.IsPostBack==false)
			{				 
				//this.BPToolBar1.AddSpt("spt1");
				this.BPToolBar1.AddBtn(NamesOfBtn.SelectAll);
				this.BPToolBar1.AddBtn(NamesOfBtn.SelectNone);

				this.BPToolBar1.AddBtn("Btn_Help");
				this.BPToolBar1.AddSpt("spt2");			  
				this.BPToolBar1.AddBtn(NamesOfBtn.Close);
				this.BPToolBar1.AddBtn(NamesOfBtn.Confirm);				 
				this.BPToolBar1.AddSpt("spt3");			 
				this.SetData();
			}       
		}
		/// <summary>
		/// ����
		/// </summary>
		public void SetData()
		{
			En.Entities ens = ClassFactory.GetEns(this.EnsName) ; 
			ens.RetrieveAll();
			foreach(En.Entity en in ens)
			{
				this.CBL1.Items.Add( new ListItem(en.GetValStringByKey(this.RefText),en.GetValStringByKey(this.RefKey) ));
			}
		}		 

		#region Web ������������ɵĴ���
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: �õ����� ASP.NET Web ���������������ġ�
			//
			this.BPToolBar1.ButtonClick+=new EventHandler(BPToolBar1_ButtonClick);
			//this.BPToolBar2.ButtonClick+=new EventHandler(BPToolBar2_ButtonClick);
			//this.BPToolBar1.CheckChange+=new EventHandler(BPToolBar1_CheckChange);
			InitializeComponent();
			base.OnInit(e);
		}		
		/// <summary>
		/// �����֧������ķ��� - ��Ҫʹ�ô���༭���޸�.
		/// �˷��������ݡ�
		/// </summary>
		private void InitializeComponent()
		{    
		}
		#endregion

		private void BPToolBar1_ButtonClick(object sender, System.EventArgs e)
		{
			try
			{
				string id="";
				 
				ToolbarBtn btn = (ToolbarBtn)sender;
				id=btn.ID ;					
				 
				switch(id)
				{
					case NamesOfBtn.SelectNone:						 
						this.CBL1.SelectNone();
						break;
					case NamesOfBtn.SelectAll:
						this.CBL1.SelectAll();
						break;
					case NamesOfBtn.Confirm:
						this.Confirm();						
						break;
					case NamesOfBtn.Close:
						this.WinClose();
						break;
					case NamesOfBtn.New:
						break;
					 
					case NamesOfBtn.Help:
						this.Helper();
						break;
					default:
						this.SetData();
						break;
				}
			}
			catch(Exception ex)
			{
				this.ResponseWriteRedMsg(ex);
			}
		}
		private void Confirm()
		{
			try
			{
				string str=",";
				foreach(ListItem li in this.CBL1.Items)
				{
					if (li.Selected)
					{
						str+=li.Value+",";
					}
				}
				string clientscript = "<script language='javascript'> window.returnValue = '"+str+"'; window.close(); </script>";
				this.Page.Response.Write(clientscript);
			}
			catch(System.Exception ex)
			{
				this.ResponseWriteRedMsg(ex.Message);			 		
			}
		} 
	}
}
