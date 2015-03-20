using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.En;
using BP.Web;
namespace CCFlow.WF.MapDef
{
    public partial class WF_MapDef_FrmAttachment : BP.Web.WebPage
    {
        #region 属性.
        /// <summary>
        /// 类型
        /// </summary>
        public string UploadType
        {
            get
            {
                string s = this.Request.QueryString["UploadType"];
                if (s == null)
                    s = "1";
                return s;
            }
        }
        public string FK_MapData
        {
            get
            {
                string s = this.Request.QueryString["FK_MapData"];
                if (s == null)
                    s = "test";
                return s;
            }
        }
        public string Ath
        {
            get
            {
                return this.Request.QueryString["Ath"];
            }
        }
        public int FK_Node
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["FK_Node"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            FrmAttachment ath = new FrmAttachment();
            ath.FK_MapData = this.FK_MapData;
            ath.NoOfObj = this.Ath;
            ath.FK_Node = this.FK_Node;

            if (this.FK_Node==0)
                ath.MyPK = this.FK_MapData + "_" + this.Ath;
            else
                ath.MyPK = this.FK_MapData + "_" + this.Ath+"_"+this.FK_Node;

            int i = ath.RetrieveFromDBSources();
            if (i==0 && this.FK_Node != 0)
            {
                ath.FK_MapData = this.FK_MapData;
                ath.NoOfObj = this.Ath;
                ath.FK_Node = this.FK_Node;
                ath.MyPK = this.FK_MapData + "_" + this.Ath;
                ath.RetrieveFromDBSources();

                //插入一个新的.
                ath.MyPK = this.FK_MapData + "_" + this.Ath+"_"+this.FK_Node;
                ath.FK_Node = this.FK_Node;
                ath.FK_MapData = this.FK_MapData;
                ath.NoOfObj = this.Ath;
                ath.DirectInsert();
            }

            this.Title = "附件属性设置";
            this.Pub1.AddTable();
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("项目");
            this.Pub1.AddTDTitle("采集");
            this.Pub1.AddTDTitle("说明");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("编号");
            TextBox tb = new TextBox();
            tb.ID = "TB_" + FrmAttachmentAttr.NoOfObj;
            tb.Text = ath.NoOfObj;
            if (this.Ath != null)
                tb.Enabled = false;

            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("标示号只能英文字母数字或下滑线.");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("名称");
            tb = new TextBox();
            tb.ID = "TB_" + FrmAttachmentAttr.Name;
            tb.Text = ath.Name;
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("附件的中文名称.");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("文件格式");
            tb = new TextBox();
            tb.ID = "TB_" + FrmAttachmentAttr.Exts;
            tb.Text = ath.Exts;
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("实例:doc,docx,xls,多种格式用逗号分开.");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("保存到");
            tb = new TextBox();
            tb.ID = "TB_" + FrmAttachmentAttr.SaveTo;
            tb.Text = ath.SaveTo;
            tb.Columns = 60;
            this.Pub1.AddTD("colspan=2", tb);
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("类别");
            tb = new TextBox();
            tb.ID = "TB_" + FrmAttachmentAttr.Sort;
            tb.Text = ath.Sort;
            tb.Columns = 60;
            this.Pub1.AddTD("colspan=2", tb);
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("colspan=3", "帮助:类别可以为空,设置的格式为:类别名1,类别名2,类别名3");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("高度");
            BP.Web.Controls.TB mytb = new BP.Web.Controls.TB();
            mytb.ID = "TB_" + FrmAttachmentAttr.H;
            mytb.Text = ath.H.ToString();
            mytb.ShowType = BP.Web.Controls.TBType.Float;
            this.Pub1.AddTD("colspan=1", mytb);
            this.Pub1.AddTD("对傻瓜表单有效");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("宽度");
            mytb = new BP.Web.Controls.TB();
            mytb.ID = "TB_" + FrmAttachmentAttr.W;
            mytb.Text = ath.W.ToString();
            mytb.ShowType = BP.Web.Controls.TBType.Float;
            mytb.Columns = 60;
            this.Pub1.AddTD("colspan=1", mytb);
            this.Pub1.AddTD("对傻瓜表单有效");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("自动控制");
            CheckBox cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsAutoSize;
            cb.Text = "自动控制高度与宽度(对傻瓜表单有效)";
            cb.Checked = ath.IsAutoSize;
            this.Pub1.AddTD("colspan=2", cb);
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsNote;
            cb.Text = "是否增加备注列";
            cb.Checked = ath.IsNote;
            this.Pub1.AddTD(cb);

            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsShowTitle;
            cb.Text = "是否显示标题列";
            cb.Checked = ath.IsShowTitle;
            this.Pub1.AddTD(cb);
            this.Pub1.AddTD("");
            this.Pub1.AddTREnd();


            GroupFields gfs = new GroupFields(ath.FK_MapData);

            this.Pub1.AddTR1();
            this.Pub1.AddTD("显示在分组");
            BP.Web.Controls.DDL ddl = new BP.Web.Controls.DDL();
            ddl.ID = "DDL_GroupField";
            ddl.BindEntities(gfs, GroupFieldAttr.OID, GroupFieldAttr.Lab, false, BP.Web.Controls.AddAllLocation.None);
            ddl.SetSelectItem(ath.GroupID);
            this.Pub1.AddTD("colspan=2", ddl);

            this.Pub1.AddTREnd();
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("colspan=3", "权限控制");
            this.Pub1.AddTREnd();


            this.Pub1.AddTR();
            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsDownload;
            cb.Text = "是否可下载";
            cb.Checked = ath.IsDownload;
            this.Pub1.AddTD(cb);

            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsDelete;
            cb.Text = "是否可删除";
            cb.Checked = ath.IsDelete;
            this.Pub1.AddTD(cb);

            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsUpload;
            cb.Text = "是否可上传";
            cb.Checked = ath.IsUpload;
            this.Pub1.AddTD(cb);
            this.Pub1.AddTREnd();

          
            this.Pub1.AddTR1();
            this.Pub1.AddTD("数据显示控制方式");
            ddl = new BP.Web.Controls.DDL();
            ddl.ID = "DDL_CtrlWay";
            ddl.Items.Clear();
            ddl.Items.Add(new ListItem("按主键","0"));
            ddl.Items.Add(new ListItem("FID", "1"));
            ddl.Items.Add(new ListItem("ParentWorkID", "2"));
            ddl.SetSelectItem( (int)ath.HisCtrlWay);
            this.Pub1.AddTD("colspan=2", ddl);
            this.Pub1.AddTREnd();

            this.Pub1.AddTR1();
            this.Pub1.AddTD("数据上传控制方式");
            ddl = new BP.Web.Controls.DDL();
            ddl.ID = "DDL_AthUploadWay";
            ddl.Items.Clear();
            ddl.Items.Add(new ListItem("继承模式", "0"));
            ddl.Items.Add(new ListItem("协作模式", "1"));
            ddl.SetSelectItem((int)ath.AthUploadWay);
            this.Pub1.AddTD("colspan=2", ddl);
            this.Pub1.AddTREnd();

            this.Pub1.AddTR1();
            this.Pub1.AddTD("展现方式");
            ddl = new BP.Web.Controls.DDL();
            ddl.ID = "DDL_"+FrmAttachmentAttr.FileShowWay;
            ddl.Items.Clear();
            ddl.Items.Add(new ListItem("Table方式", "0"));
            ddl.Items.Add(new ListItem("图片轮播方式", "1"));
            ddl.Items.Add(new ListItem("自由模式", "2"));

            ddl.SetSelectItem((int)ath.FileShowWay);
            this.Pub1.AddTD("colspan=2", ddl);
            this.Pub1.AddTREnd();


            #region WebOffice控制方式.

            this.Pub1.AddTR1();
            this.Pub1.AddTDTitle("colspan=3", "WebOffice控制方式.");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableWF;
            cb.Text = "是否启用weboffice？";
            cb.Checked = ath.IsWoEnableWF;
            this.Pub1.AddTD(cb);

            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableSave;
            cb.Text = "是否启用保存？";
            cb.Checked = ath.IsWoEnableSave;
            this.Pub1.AddTD(cb);

            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableReadonly;
            cb.Text = "是否只读？";
            cb.Checked = ath.IsWoEnableReadonly;
            this.Pub1.AddTD(cb);
            this.Pub1.AddTREnd();
             


            this.Pub1.AddTR();
            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableRevise;
            cb.Text = "是否启用修订？";
            cb.Checked = ath.IsWoEnableRevise;
            this.Pub1.AddTD(cb);

            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableViewKeepMark;
            cb.Text = "是否查看用户留痕？";
            cb.Checked = ath.IsWoEnableViewKeepMark;
            this.Pub1.AddTD(cb);

            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnablePrint;
            cb.Text = "是否打印？";
            cb.Checked = ath.IsWoEnablePrint;
            this.Pub1.AddTD(cb);
            this.Pub1.AddTREnd();
             

            this.Pub1.AddTR();
            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableOver;
            cb.Text = "是否启用套红？";
            cb.Checked = ath.IsWoEnableOver;
            this.Pub1.AddTD(cb);

            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableSeal;
            cb.Text = "是否启用签章？";
            cb.Checked = ath.IsWoEnableSeal;
            this.Pub1.AddTD(cb);

            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableTemplete;
            cb.Text = "是否启用模板文件？";
            cb.Checked = ath.IsWoEnableTemplete;
            this.Pub1.AddTD(cb);
          
            this.Pub1.AddTREnd();
            this.Pub1.AddTR();
            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableCheck;
            cb.Text = "是否记录节点信息？";
            cb.Checked = ath.IsWoEnableCheck;
            this.Pub1.AddTD(cb);
            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableInsertFlow;
            cb.Text = "是否启用插入流程？";
            cb.Checked = ath.IsWoEnableInsertFlow;
            this.Pub1.AddTD(cb);
            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableInsertFengXian;
            cb.Text = "是否启用插入风险点？";
            cb.Checked = ath.IsWoEnableInsertFengXian;
            this.Pub1.AddTD(cb);
            this.Pub1.AddTR();
            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableMarks;
            cb.Text = "是否进入留痕模式？";
            cb.Checked = ath.IsWoEnableMarks;
            this.Pub1.AddTD(cb);
            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableDown;
            cb.Text = "是否启用下载？";
            cb.Checked = ath.IsWoEnableDown;
            this.Pub1.AddTD(cb);
            this.Pub1.AddTD("");
            this.Pub1.AddTREnd();
      

            this.Pub1.AddTREnd();
            #endregion WebOffice控制方式.

            #region 快捷键生成规则.
            this.Pub1.AddTR1();
            this.Pub1.AddTDTitle("colspan=3", "快捷键生成规则.");
            this.Pub1.AddTREnd();


            this.Pub1.AddTR();
            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.FastKeyIsEnable;
            cb.Text = "是否启用生成快捷键？(启用就会按照规则生成放在附件的同一个目录里面)";
            cb.Checked = ath.FastKeyIsEnable; 
            this.Pub1.AddTD("colspan=3", cb);
            this.Pub1.AddTREnd();


            this.Pub1.AddTR();
            tb=new BP.Web.Controls.TB();
            tb.ID = "TB_" + FrmAttachmentAttr.FastKeyGenerRole;
            tb.Text = ath.FastKeyGenerRole;
            tb.Columns = 30;
            this.Pub1.AddTD("colspan=3", tb);
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("colspan=3", "格式:*FiledName.*OID");
            this.Pub1.AddTREnd(); 
            #endregion 快捷键生成规则.

            #region 保存按钮.
            this.Pub1.AddTR();
            this.Pub1.AddTD("");
            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.Text = "  Save  ";
            btn.CssClass = "Btn";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.AddTD(btn);

            if (this.Ath != null)
            {
                btn = new Button();
                btn.ID = "Btn_Delete";
                btn.Text = "  Delete  ";
                btn.CssClass = "Btn";
                btn.Attributes["onclick"] = " return confirm('您确认吗？');";
                btn.Click += new EventHandler(btn_Click);
                this.Pub1.AddTD(btn);
            }
            else
            {
                this.Pub1.AddTD();
            }
            this.Pub1.AddTREnd();
            #endregion 保存按钮.

            this.Pub1.AddTableEnd();
        }

        void btn_Click(object sender, EventArgs e)
        {
            FrmAttachment ath = new FrmAttachment();
            if (this.FK_Node == 0)
                ath.MyPK = this.FK_MapData + "_" + this.Ath;
            else
                ath.MyPK = this.FK_MapData + "_" + this.Ath + "_" + this.FK_Node;

            ath.RetrieveFromDBSources();

            Button btn = sender as Button;
            if (btn.ID == "Btn_Delete")
            {
                //ath.MyPK = this.FK_MapData + "_" + this.Ath;
                ath.Delete();
                this.WinClose("删除成功.");
                return;
            }

            ath.MyPK = this.FK_MapData + "_" + this.Ath;
            if (this.Ath != null)
                ath.RetrieveFromDBSources();
            ath = this.Pub1.Copy(ath) as FrmAttachment;
            ath.FK_MapData = this.FK_MapData;
            ath.FK_Node = this.FK_Node;
            if (string.IsNullOrEmpty(this.Ath)==false)
                ath.NoOfObj = this.Ath;

            if (this.FK_Node == 0)
                ath.MyPK = this.FK_MapData + "_" + ath.NoOfObj;
            else
                ath.MyPK = this.FK_MapData + "_" + ath.NoOfObj + "_" + this.FK_Node;

            GroupFields gfs1 = new GroupFields(this.FK_MapData);
            if (gfs1.Count == 1)
            {
                GroupField gf = (GroupField)gfs1[0];
                ath.GroupID = gf.OID;
            }
            else
            {
                ath.GroupID = this.Pub1.GetDDLByID("DDL_GroupField").SelectedItemIntVal;
            }

            //对流程的特殊判断.
            ath.HisCtrlWay = (AthCtrlWay)this.Pub1.GetDDLByID("DDL_CtrlWay").SelectedItemIntVal;
            ath.AthUploadWay = (AthUploadWay)this.Pub1.GetDDLByID("DDL_AthUploadWay").SelectedItemIntVal;
            ath.FileShowWay = (FileShowWay)this.Pub1.GetDDLByID("DDL_FileShowWay").SelectedItemIntVal; //文件展现方式.
            

            ath.IsWoEnableWF = this.Pub1.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableWF).Checked;
            ath.IsWoEnableSave = this.Pub1.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableSave).Checked;
            ath.IsWoEnableReadonly = this.Pub1.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableReadonly).Checked;
            ath.IsWoEnableRevise = this.Pub1.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableRevise).Checked;
            ath.IsWoEnableViewKeepMark = this.Pub1.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableViewKeepMark).Checked;
            ath.IsWoEnablePrint = this.Pub1.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnablePrint).Checked;
            ath.IsWoEnableSeal = this.Pub1.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableSeal).Checked;
            ath.IsWoEnableOver = this.Pub1.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableOver).Checked;
            ath.IsWoEnableTemplete = this.Pub1.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableTemplete).Checked;
            ath.IsWoEnableCheck = this.Pub1.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableCheck).Checked;
            ath.IsWoEnableInsertFengXian = this.Pub1.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableInsertFengXian).Checked;
            ath.IsWoEnableInsertFlow = this.Pub1.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableInsertFlow).Checked;
            ath.IsWoEnableMarks = this.Pub1.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableMarks).Checked;
            ath.IsWoEnableDown = this.Pub1.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableDown).Checked;


            ath.FastKeyIsEnable = this.Pub1.GetCBByID("CB_" + FrmAttachmentAttr.FastKeyIsEnable).Checked;
            ath.FastKeyGenerRole = this.Pub1.GetTBByID("TB_" + FrmAttachmentAttr.FastKeyGenerRole).Text;

            if (ath.FastKeyIsEnable == true)
                if (ath.FastKeyGenerRole.Contains("*OID") == false)
                    throw new Exception("@快捷键生成规则必须包含*OID,否则会导致文件名重复.");

            if (this.Ath == null)
            {
                ath.UploadType = (AttachmentUploadType)int.Parse(this.UploadType);

                if (this.FK_Node == 0)
                    ath.MyPK = this.FK_MapData + "_" + ath.NoOfObj;
                else
                    ath.MyPK = this.FK_MapData + "_" + ath.NoOfObj + "_" + this.FK_Node;

                if (ath.IsExits == true)
                {
                    this.Alert("附件编号(" + ath.NoOfObj + ")已经存在。");
                    return;
                }
                ath.Insert();
            }
            else
            {
                ath.NoOfObj = this.Ath;
                if (this.FK_Node == 0)
                    ath.MyPK = this.FK_MapData + "_" + this.Ath;
                else
                    ath.MyPK = this.FK_MapData + "_" + this.Ath + "_" + this.FK_Node;

                ath.Update();
            }
            this.WinCloseWithMsg("保存成功");
        }
    }
}