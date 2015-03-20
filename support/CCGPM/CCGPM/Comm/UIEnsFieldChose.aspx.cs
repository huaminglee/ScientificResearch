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
using BP.Web;
using BP.Web.Controls;
 

namespace BP.Web.WF.Comm
{
	/// <summary>
	/// UIEnsFieldChose ��ժҪ˵����
	/// </summary>
	public partial class UIEnsFieldChose : WebPage
	{
        public new string EnsName
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
				this.BPToolBar1.AddBtn(NamesOfBtn.Save);
				//this.BPToolBar1.AddBtn(NamesOfBtn.SaveAndClose);
				this.BPToolBar1.AddBtn(NamesOfBtn.Close);
				this.BPToolBar1.AddBtn(NamesOfBtn.Help);
				this.BindTree();
			}
			this.BPToolBar1.ButtonClick+=new EventHandler(BPToolBar1_ButtonClick);
		}
		 
		public void BindTree()
		{
			this.CBL1.Items.Clear();
			Entity en = ClassFactory.GetEns(this.EnsName).GetNewEntity;


		this.Label1.Text=this.GenerCaption(en.EnDesc+":�ֶ�ѡ��");
			BP.Sys.CField cf = new BP.Sys.CField(WebUser.No,this.EnsName);
 

			foreach(Attr attr in en.EnMap.Attrs)
			{
				//				if (attr.MyFieldType==FieldType.Enum)
				//					continue;

				if (attr.UIVisible==false)
					continue;

				if (attr.IsPK)
					continue;

				if (cf.Attrs=="" || cf.Attrs.Length==0)
					this.CBL1.Items.Add( new ListItem(attr.Desc,attr.Key )  ); 
				else
				{
					if (cf.Attrs.IndexOf("@"+attr.Key+"@") >=0)
					{
						ListItem li =  new ListItem(attr.Desc,attr.Key ) ;
						li.Selected=true;
						this.CBL1.Items.Add(  li  ); 
					}
					else
					{
						this.CBL1.Items.Add( new ListItem(attr.Desc,attr.Key )  ); 
					}
				}
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

		private void BPToolBar1_ButtonClick(object sender, EventArgs e)
		{
			try
			{
				ToolbarBtn btn = (ToolbarBtn)sender ; 
				switch (btn.ID)
				{
					case NamesOfBtn.Save:
						this.Save();
						return;
					case NamesOfBtn.SaveAndClose:
						this.Save();
						this.WinClose();
						return;
					case NamesOfBtn.Close:
						this.WinClose();
						return;
				}
			}
			catch(Exception ex)
			{
				this.ResponseWriteRedMsg(ex);
			}
		}
		private void Save()
		{
			string strs="@";
			foreach(ListItem li in this.CBL1.Items)
			{
				if (li.Selected==false)
					continue;

				strs+=li.Value+"@";
			}


			BP.Sys.CField cf = new BP.Sys.CField();
			cf.FK_Emp=WebUser.No;
			cf.EnsName=this.EnsName;
			cf.Attrs=strs;
			cf.Save();
			//this.ResponseWriteBlueMsg_SaveOK();

			Entity en = ClassFactory.GetEns(this.EnsName).GetNewEntity;

            this.Label1.Text = this.GenerCaption(en.EnDesc + ":�ֶ�ѡ��");

			//this.GenerLabel(this.Label1,en.EnDesc+"���ֶ�ѡ��");

		}
	}
}
