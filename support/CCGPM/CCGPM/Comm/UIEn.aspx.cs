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
using BP.Web.Comm.UC;
using BP.En;
using BP.DA;
using BP.Web;
using BP.Web.Controls;
using BP.Sys;

namespace BP.Web.Comm
{
    /// <summary>
    /// UIEnNew ��ժҪ˵�� 
    /// </summary>
    public partial class UIEn : WebPage
    {
        #region ����
        /// <summary>
        /// �����ɣ�
        /// </summary>
        public new string EnsName
        {
            get
            {
                if (this.Request.QueryString["EnsName"] == null)
                {
                    string s = this.Request.QueryString["EnName"];
                    if (s == null)
                        return "BP.Port.Emps";

                    Entity en = ClassFactory.GetEn(s);
                    return en.GetNewEntities.ToString();
                }
                else
                    return this.Request.QueryString["EnsName"];
            }
        }
        /// <summary>
        /// ������
        /// </summary>
        public new string EnName
        {
            get
            {
                return this.Request.QueryString["EnName"];
            }
        }
        /// <summary>
        /// �õ�һ���µ��������ݣ�
        /// </summary>
        public Entity GetEnDa
        {
            get
            {
                Entity en = this.GetEns.GetNewEntity;
                if (en.PKCount == 1)
                {
                    if (this.Request.QueryString["PK"] != null)
                    {
                        en.PKVal = this.Request.QueryString["PK"];
                    }
                    else
                    {
                        if (this.Request.QueryString[en.PK] == null)
                            return en;
                        else
                            en.PKVal = this.Request.QueryString[en.PK];
                    }
                    if (en.IsExits == false)
                        throw new Exception("@��¼������,����û�б���.");
                    else
                        en.RetrieveFromDBSources();
                    return en;
                }
                else if (en.IsMIDEntity)
                {
                    string val = this.Request.QueryString["MID"];
                    if (val == null)
                        val = this.Request.QueryString["PK"];
                    if (val == null)
                    {
                        return en;
                    }
                    else
                    {
                        en.SetValByKey("MID", val);
                        en.RetrieveFromDBSources();
                        return en;
                    }
                }

                Attrs attrs = en.EnMap.Attrs;
                foreach (Attr attr in attrs)
                {
                    if (attr.IsPK)
                    {
                        string str = this.Request.QueryString[attr.Key];
                        if (str == null)
                        {
                            if (en.IsMIDEntity)
                            {
                                en.SetValByKey("MID", this.Request.QueryString["PK"]);
                                continue;
                            }
                            else
                            {
                                throw new Exception("@û�а�����ֵ[" + attr.Key + "]�������.");
                            }
                        }

                        en.SetValByKey(attr.Key, this.Request.QueryString[attr.Key]);
                    }
                }
                if (en.IsExits == false)
                {
                    throw new Exception("@����û�м�¼.");
                }
                else
                {
                    en.RetrieveFromDBSources();
                }
                return en;
            }
        }
        public Entities _GetEns = null;
        public BP.Web.Controls.Btn Btn_New
        {
            get
            {
                return this.ToolBar1.GetBtnByID(NamesOfBtn.New);
            }
        }
        public BP.Web.Controls.Btn Btn_Copy
        {
            get
            {
                return this.ToolBar1.GetBtnByID(NamesOfBtn.Copy);
            }
        }
        public BP.Web.Controls.Btn Btn_Delete
        {
            get
            {
                return this.ToolBar1.GetBtnByID(NamesOfBtn.Delete);
            }
        }
        public BP.Web.Controls.Btn Btn_Adjunct
        {
            get
            {
                return this.ToolBar1.GetBtnByID(NamesOfBtn.Adjunct);
            }
        }
        /// <summary>
        /// ��ǰ��ʵ�弯�ϣ�
        /// </summary>
        public Entities GetEns
        {
            get
            {
                if (_GetEns == null)
                {
                    if (this.EnName != null)
                    {
                        Entity en = ClassFactory.GetEn(EnName);
                        _GetEns = en.GetNewEntities;
                    }
                    else
                    {
                        _GetEns = ClassFactory.GetEns(EnsName);
                    }
                }
                return _GetEns;
            }
        }
        public Entity _CurrEn = null;
        public Entity CurrEn
        {
            get
            {
                if (_CurrEn == null)
                {
                    _CurrEn = this.GetEnDa;
                }
                return _CurrEn;
            }
            set
            {
                _CurrEn = value;
            }
        }
        /// <summary>
        /// �Ƿ�Readonly.
        /// </summary>
        public bool IsReadonly
        {
            get
            {
                return (bool)this.ViewState["IsReadonly"];
            }
            set
            {
                ViewState["IsReadonly"] = value;
            }
        }
        #endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {
            string url = this.Request.RawUrl;
            url = url.Replace("/Comm/", "/Comm/RefFunc/");
            this.Response.Redirect(url,true);
            return;

            #region �������;
            this.Response.Expires = -1;
            this.Response.ExpiresAbsolute = DateTime.Now.AddMonths(-1);
            this.Response.CacheControl = "no-cache";
            #endregion �������

            try
            {
                #region �ж�Ȩ��
                UAC uac = this.CurrEn.HisUAC;
                if (uac.IsView == false)
                    throw new Exception("@�Բ�����û�в鿴��Ȩ�ޣ�");

                this.IsReadonly = !uac.IsUpdate;  //�Ƿ�����޸ĵ�Ȩ�ޣ�
                if (this.Request.QueryString["IsReadonly"] == "1"
                    || this.Request.QueryString["Readonly"] == "1")
                    this.IsReadonly = true;
                #endregion

              //  this.ToolBar1.DivInfoBlockBegin();

                this.ToolBar1.Add("&nbsp;&nbsp;");

                this.ToolBar1.InitFuncEn(uac, this.CurrEn);

              //  this.ToolBar1.DivInfoBlockEnd();

                this.UCEn1.IsReadonly = this.IsReadonly;
                this.UCEn1.IsShowDtl = true;
                this.UCEn1.HisEn = this.CurrEn;

                //if (this.IsReadonly)
                //    this.ToolBar1.Enabled = false;

                string pk = this.Request.QueryString["PK"];
                if (pk == null)
                    pk = this.Request.QueryString[this.CurrEn.PK];

                this.UCEn1.Bind(this.CurrEn, this.CurrEn.ToString(), this.IsReadonly, true);
            }
            catch (Exception ex)
            {
                this.Response.Write(ex.Message);
                Entity en = ClassFactory.GetEns(this.EnsName).GetNewEntity;
                en.CheckPhysicsTable();
                return;
            }

            this.Title = this.CurrEn.EnDesc;


            #region �����¼�
            if (this.Btn_DelFile != null)
                this.Btn_DelFile.Click += new ImageClickEventHandler(Btn_DelFile_Click);

            if (this.ToolBar1.IsExit(NamesOfBtn.New))
                this.ToolBar1.GetBtnByID(NamesOfBtn.New).Click += new System.EventHandler(this.ToolBar1_ButtonClick);

            if (this.ToolBar1.IsExit(NamesOfBtn.Save))
                this.ToolBar1.GetBtnByID(NamesOfBtn.Save).Click += new System.EventHandler(this.ToolBar1_ButtonClick);

            if (this.ToolBar1.IsExit(NamesOfBtn.SaveAndClose))
                this.ToolBar1.GetBtnByID(NamesOfBtn.SaveAndClose).Click += new System.EventHandler(this.ToolBar1_ButtonClick);

            if (this.ToolBar1.IsExit(NamesOfBtn.SaveAndNew))
                this.ToolBar1.GetBtnByID(NamesOfBtn.SaveAndNew).Click += new System.EventHandler(this.ToolBar1_ButtonClick);

            if (this.ToolBar1.IsExit(NamesOfBtn.Delete))
                this.ToolBar1.GetBtnByID(NamesOfBtn.Delete).Click += new System.EventHandler(this.ToolBar1_ButtonClick);


            AttrFiles fls = this.CurrEn.EnMap.HisAttrFiles;
            foreach (AttrFile fl in fls)
            {
                if (this.UCEn1.IsExit("Btn_DelFile" + fl.FileNo))
                    this.UCEn1.GetImageButtonByID("Btn_DelFile" + fl.FileNo).Click  += new ImageClickEventHandler(Btn_DelFile_X_Click);
            }
            #endregion �����¼�
        }


        public ImageButton Btn_DelFile
        {
            get
            {
                return this.UCEn1.FindControl("Btn_DelFile") as ImageButton;
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

        public void DelFile(string id)
        {

        }

        private void Btn_DelFile_X_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton btn = sender as ImageButton;

            string id = btn.ID.Replace("Btn_DelFile", "");
            SysFileManager sf = new SysFileManager();

       //     Entity en = this.UCEn1.GetEnData(this.GetEns.GetNewEntity);

            string sql = "DELETE FROM " + sf.EnMap.PhysicsTable + " WHERE " + SysFileManagerAttr.EnName + "='" + this.GetEns.GetNewEntity.ToString() + "' AND RefVal='" + this.PKVal + "' AND " + SysFileManagerAttr.AttrFileNo + "='" + id + "'";
            BP.DA.DBAccess.RunSQL(sql);
            this.Response.Redirect("UIEn.aspx?EnsName=" + this.EnsName + "&PK=" + this.PKVal, true);
        }

        private void Btn_DelFile_Click(object sender, ImageClickEventArgs e)
        {
            Entity en = this.UCEn1.GetEnData(this.GetEns.GetNewEntity);
            string file = en.GetValStringByKey("MyFilePath") + "//" + en.PKVal + "." + en.GetValStringByKey("MyFileExt");
            try
            {
                System.IO.File.Delete(file);
            }
            catch
            {

            }
            en.SetValByKey("MyFileExt", "");
            en.SetValByKey("MyFileName", "");
            en.SetValByKey("MyFilePath", "");
            en.Update();
            this.Response.Redirect("UIEn.aspx?EnsName=" + this.EnsName + "&PK=" + this.PKVal, true);
        }

        private void ToolBar1_ButtonClick(object sender, System.EventArgs e)
        {
            Btn btn = (Btn)sender;
            try
            {
                switch (btn.ID)
                {
                    case NamesOfBtn.Copy:
                        Copy();
                        break;
                    //case NamesOfBtn.Help:
                    //  //  this.Helper(this.GetEns.GetNewEntity.EnMap.Helper);
                    //    break;
                    case NamesOfBtn.New:
                        //   New();
                        this.Response.Redirect("UIEn.aspx?EnsName=" + this.EnsName, true);
                        break;
                    case NamesOfBtn.SaveAndNew:
                        try
                        {
                            this.Save();
                        }
                        catch (Exception ex)
                        {
                            this.Alert(ex.Message);
                            //this.ResponseWriteBlueMsg(ex.Message);
                            return;
                        }
                        this.Response.Redirect("UIEn.aspx?EnsName=" + this.EnsName, true);
                        break;
                    case NamesOfBtn.SaveAndClose:
                        try
                        {
                            this.Save();
                            this.WinClose();
                        }
                        catch (Exception ex)
                        {
                            this.Alert(ex.Message);
                            return;
                        }
                        break;
                    case NamesOfBtn.Save:
                        try
                        {
                            this.Save();
                        }
                        catch (Exception ex)
                        {
                            this.Alert(ex.Message);
                            return;
                        }
                        this.Response.Redirect("UIEn.aspx?EnsName=" + this.EnsName + "&PK=" + this.PKVal, true);
                        break;
                    case NamesOfBtn.Delete:
                        try
                        {
                            Entity en = this.UCEn1.GetEnData(this.GetEns.GetNewEntity);
                            if (this.PKVal != null)
                                en.PKVal = this.PKVal;
                            en.Delete();
                            this.ToCommMsgPage("ɾ���ɹ�!!!");
                            return;
                        }
                        catch (Exception ex)
                        {
                            this.ToCommMsgPage("ɾ���ڼ���ִ���: \t\n" + ex.Message);
                            //this.ToMsgPage("ɾ���ɹ�!!!");
                            return;
                        }
                        return;
                    case NamesOfBtn.Close:
                        this.WinClose();
                        break;
                    case "Btn_EnList":
                        this.EnList();
                        break;
                    case NamesOfBtn.Export:
                        //this.ExportDGToExcel_OpenWin(this.UCEn1,"" );
                        break;
                    case NamesOfBtn.Adjunct:
                        this.InvokeFileManager(this.GetEnDa);
                        break;
                    default:
                        throw new Exception("@û���ҵ�" + btn.ID);
                }
            }
            catch (Exception ex)
            {
                this.ResponseWriteRedMsg(ex.Message);
            }
        }
        public object PKVal
        {
            get
            {
                object obj = ViewState["MyPK"];
                if (obj == null)
                    obj = this.Request.QueryString["PK"];

                if (obj == null)
                    obj = this.Request.QueryString["OID"];

                if (obj == null)
                    obj = this.Request.QueryString["No"];

                if (obj == null)
                    obj = this.Request.QueryString["MyPK"];

                return obj;
            }
            set
            {
                this.ViewState["MyPK"] = value;
            }
        }

        #region ����
        /// <summary>
        /// new
        /// </summary>
        public void New()
        {
            this.Response.Redirect("UIEn.aspx?EnsName=" + this.EnsName, true);
            return;

            this.CurrEn = this.GetEns.GetNewEntity;
            this.PKVal = null;

            if (this.CurrEn.EnMap.Attrs.Contains("No"))
            {
                Attr attr = this.CurrEn.EnMap.GetAttrByKey("No");

                if (attr.UIIsReadonly || this.CurrEn.EnMap.IsAutoGenerNo)
                {
                    if (this.CurrEn.GetValStringByKey("No") == "")
                    {
                        this.CurrEn.SetValByKey("No", this.CurrEn.GenerNewNoByKey("No"));
                        //string val = SystemConfig.GetConfigXmlEns(ConfigKeyEns.IsInsertBeforeNew, CurrEn.ToString());
                        //if (val == "1")
                        //{
                        //    //CurrEn.SetValByKey("No",dr[attr.Key]);
                        //    CurrEn.Insert();
                        //}
                    }
                }
            }

            if (this.ToolBar1.IsExit(NamesOfBtn.Adjunct) == true)
                this.Btn_Adjunct.Enabled = false;

            if (this.ToolBar1.IsExit(NamesOfBtn.Delete) == true)
                this.Btn_Delete.Enabled = false;

            //if (this.ToolBar1.IsExitsContral(NamesOfBtn.Copy) == true)
            //    this.Btn_Copy.Enabled = false;

            this.UCEn1.Bind(this.CurrEn, this.CurrEn.ToString(), false, false);

            this.PKVal = this.CurrEn.PKVal;
        }
        public void Copy()
        {
            try
            {
                this.PKVal = null;
                Entity en = this.UCEn1.GetEnData(this.GetEns.GetNewEntity);
                en.Copy();
                this.UCEn1.Bind(en, en.ToString(), this.IsReadonly, true);
            }
            catch (Exception ex)
            {
                this.ResponseWriteRedMsg(ex);
            }
        }
        /// <summary>
        /// delete
        /// </summary>
        public void Delete()
        {
            Entity en = this.GetEnDa;
            en.PKVal = this.PKVal;
            en.Delete();
            this.WinClose();
        }
        public void Save()
        {
            Entity en = this.UCEn1.GetEnData(this.GetEns.GetNewEntity);
            if (this.PKVal != null)
                en.PKVal = this.PKVal;

            this.CurrEn = en;
            en.Save();
            this.PKVal = en.PKVal;

            #region ���� ʵ�帽��
            try
            {
                if (en.EnMap.Attrs.Contains("MyFileName"))
                {
                    HtmlInputFile file = this.UCEn1.FindControl("file") as HtmlInputFile;
                    if (file != null && file.Value.IndexOf(".") != -1)
                    {
                        BP.Sys.EnCfg cfg = new EnCfg(en.ToString());
                        if (System.IO.Directory.Exists(cfg.FJSavePath) == false)
                            System.IO.Directory.CreateDirectory(cfg.FJSavePath);

                        /* �������������ֶΡ�*/
                        string fileName = file.PostedFile.FileName;
                        fileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);

                        string filePath = cfg.FJSavePath;
                        en.SetValByKey("MyFilePath", filePath);

                        string ext = "";
                        if (fileName.IndexOf(".") != -1)
                            ext = fileName.Substring(fileName.LastIndexOf(".") + 1);

                        en.SetValByKey("MyFileExt", ext);
                        en.SetValByKey("MyFileName", fileName);
                        en.SetValByKey("WebPath", cfg.FJWebPath + en.PKVal + "." + ext);

                        string fullFile = filePath + "/" + en.PKVal + "." + ext;

                        file.PostedFile.SaveAs(fullFile);
                        file.PostedFile.InputStream.Close();
                        file.PostedFile.InputStream.Dispose();
                        file.Dispose();

                        System.IO.FileInfo info = new System.IO.FileInfo(fullFile);
                        en.SetValByKey("MyFileSize", BP.DA.DataType.PraseToMB(info.Length));
                        if (DataType.IsImgExt(ext))
                        {
                            System.Drawing.Image img = System.Drawing.Image.FromFile(fullFile);
                            en.SetValByKey("MyFileH", img.Height);
                            en.SetValByKey("MyFileW", img.Width);
                            img.Dispose();
                        }
                        en.Update();
                    }
                }
            }
            catch (Exception ex)
            {
                this.Alert("���渽�����ִ���" + ex.Message);
            }
            #endregion


            #region ���� ���� ����
            try
            {
                AttrFiles fils = en.EnMap.HisAttrFiles;
                SysFileManagers sfs = new SysFileManagers(en.ToString(), en.PKVal.ToString());
                foreach (AttrFile fl in fils)
                {
                    HtmlInputFile file = (HtmlInputFile)this.UCEn1.FindControl("F" + fl.FileNo);
                    if (file.Value.Contains(".") == false)
                        continue;

                    SysFileManager enFile = sfs.GetEntityByKey(SysFileManagerAttr.AttrFileNo, fl.FileNo) as SysFileManager;
                    SysFileManager enN = null;
                    if (enFile == null)
                    {
                        enN = this.FileSave(null, file, en);
                    }
                    else
                    {
                        enFile.Delete();
                        enN = this.FileSave(null, file, en);
                    }

                    enN.AttrFileNo = fl.FileNo;
                    enN.AttrFileName = fl.FileName;
                    enN.EnName = en.ToString();
                    enN.Update();
                }
            }
            catch (Exception ex)
            {
                this.Alert("���渽�����ִ���" + ex.Message);
            }
            #endregion
        }
        /// <summary>
        /// �ļ�����
        /// </summary>
        /// <param name="fileNameDesc"></param>
        /// <param name="File1"></param>
        /// <returns></returns>
        private SysFileManager FileSave(string fileNameDesc, HtmlInputFile File1, Entity myen)
        {
            SysFileManager en = new SysFileManager();
            en.EnName = myen.ToString();
            // en.FileID = this.RefPK + "_" + count.ToString();
            EnCfg cfg = new EnCfg(en.EnName);

            string filePath = cfg.FJSavePath; // BP.Sys.SystemConfig.PathOfFDB + "\\" + this.EnName + "\\";
            if (System.IO.Directory.Exists(filePath) == false)
                System.IO.Directory.CreateDirectory(filePath);

            string ext = System.IO.Path.GetExtension(File1.PostedFile.FileName);
            ext = ext.Replace(".", "");
            en.MyFileExt = ext;
            if (fileNameDesc == "" || fileNameDesc == null)
                en.MyFileName = System.IO.Path.GetFileNameWithoutExtension(File1.PostedFile.FileName);
            else
                en.MyFileName = fileNameDesc;
            en.RDT = DataType.CurrentData;
            en.RefVal = myen.PKVal.ToString();
            en.MyFilePath = filePath;
            en.Insert();

            string fileName = filePath + en.OID + "." + en.MyFileExt;
            File1.PostedFile.SaveAs(fileName);

            File1.PostedFile.InputStream.Close();
            File1.PostedFile.InputStream.Dispose();
            File1.Dispose();

            System.IO.FileInfo fi = new System.IO.FileInfo(fileName);
            en.MyFileSize = DataType.PraseToMB(fi.Length);

            if (DataType.IsImgExt(en.MyFileExt))
            {
                System.Drawing.Image img = System.Drawing.Image.FromFile(fileName);
                en.MyFileH = img.Height;
                en.MyFileW = img.Width;
                img.Dispose();
            }
            en.WebPath = cfg.FJWebPath + en.OID + "." + en.MyFileExt;
            en.Update();
            return en;
        }

        public void EnList()
        {
            this.Response.Redirect("/Comm/UIEns.aspx?EnsName=" + this.EnsName, true);
        }
        #endregion
    }
}
