﻿using System;
using System.IO;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using BP.Web;
using BP.Sys;
using BP.DA;
using BP.WF.Template;
using BP.WF;

namespace CCFlow.WF.CCForm
{
    public partial class WF_CCForm_AttachmentUpload : BP.Web.WebPage
    {
        #region 属性.
        /// <summary>
        /// ath.
        /// </summary>
        public string NoOfObj
        {
            get
            {
                return this.Request.QueryString["NoOfObj"];
            }
        }
        public string PKVal
        {
            get
            {
                return this.Request.QueryString["PKVal"];
            }
        }
        public string IsReadonly
        {
            get
            {
                return this.Request.QueryString["IsReadonly"];
            }
        }
        public string DelPKVal
        {
            get
            {
                return this.Request.QueryString["DelPKVal"];
            }
        }
        public string FK_FrmAttachment
        {
            get
            {
                return this.Request.QueryString["FK_FrmAttachment"];
            }
        }
        public string FK_FrmAttachmentExt
        {
            get
            {
                return "ND" + this.FK_Node + "_DocMultiAth"; // this.Request.QueryString["FK_FrmAttachment"];
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public int _fk_node = 0;
        public int FK_Node
        {
            get
            {
                if (_fk_node == 0 && !string.IsNullOrEmpty(this.Request.QueryString["FK_Node"]))
                    return int.Parse(this.Request.QueryString["FK_Node"]);

                return _fk_node;
            }
            set
            {
                _fk_node = value;
            }
        }
        public Int64 WorkID
        {
            get
            {
                string str = this.Request.QueryString["WorkID"];
                if (string.IsNullOrEmpty(str))
                    str = this.Request.QueryString["OID"];

                if (string.IsNullOrEmpty(str))
                    str = this.Request.QueryString["PKVal"];

                return Int64.Parse(str);
            }
        }
        public string FK_MapData
        {
            get
            {
                string fk_mapdata = this.Request.QueryString["FK_MapData"];
                if (string.IsNullOrEmpty(fk_mapdata))
                    fk_mapdata = "ND" + FK_Node;
                return fk_mapdata;
            }
        }
        public string Ath
        {
            get
            {
                return this.Request.QueryString["Ath"];
            }
        }
        public string IsCC
        {
            get
            {
                string paras = this.Request.QueryString["Paras"];
                if (string.IsNullOrEmpty(paras) == false)
                    if (paras.Contains("IsCC=1") == true)
                        return "1";
                return "ssss";
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            #region 功能执行.
            if (this.DoType == "Del")
            {
                FrmAttachmentDB delDB = new FrmAttachmentDB();
                delDB.MyPK = this.DelPKVal == null ? this.MyPK : this.DelPKVal;

                delDB.DirectDelete();
            }
            if (this.DoType == "Down")
            {
                FrmAttachmentDB downDB = new FrmAttachmentDB();

                downDB.MyPK = this.DelPKVal == null ? this.MyPK : this.DelPKVal;
                downDB.Retrieve();

                string downpath = GetRealPath(downDB.FileFullName);
                BP.Sys.PubClass.DownloadFile(downpath, downDB.FileName);
                this.WinClose();
                return;
            }

            if (this.DoType == "WinOpen")
            {
                FrmAttachmentDB downDB = new FrmAttachmentDB();
                downDB.MyPK = this.MyPK;
                downDB.Retrieve();
                Response.ContentType = "Application/pdf";
                string downpath = GetRealPath(downDB.FileFullName);
                Response.WriteFile(downpath);
                Response.End();
                return;
            }
            #endregion 功能执行.

            #region 处理权限控制.
            BP.Sys.FrmAttachment athDesc = new BP.Sys.FrmAttachment();
            athDesc.MyPK = this.FK_FrmAttachment;
            if (this.FK_Node == null || this.FK_Flow == null)
            {
                athDesc.RetrieveFromDBSources();
            }
            else
            {
                #region 判断是否可以查询出来，如果查询不出来，就可能是公文流程。
                athDesc.MyPK = this.FK_FrmAttachment;
                if (athDesc.RetrieveFromDBSources() == 0 && string.IsNullOrEmpty(this.FK_Flow) == false)
                {
                    /*如果没有查询到它,就有可能是公文多附件被删除了.*/
                    athDesc.MyPK = this.FK_FrmAttachment;
                    athDesc.NoOfObj = "DocMultiAth";
                    athDesc.FK_MapData = this.FK_MapData;
                    athDesc.Exts = "*.*";

                    //存储路径.
                    string path = Server.MapPath("/DataUser/UploadFile/");
                    path += "\\F" + this.FK_Flow + "MultiAth";
                    athDesc.SaveTo = path;
                    athDesc.IsNote = false; //不显示note字段.


                    //位置.
                    athDesc.X = (float)94.09;
                    athDesc.Y = (float)333.18;
                    athDesc.W = (float)626.36;
                    athDesc.H = (float)150;

                    //多附件.
                    athDesc.UploadType = AttachmentUploadType.Multi;
                    athDesc.Name = "公文多附件(系统自动增加)";
                    athDesc.SetValByKey("AtPara",
                        "@IsWoEnablePageset=1@IsWoEnablePrint=1@IsWoEnableViewModel=1@IsWoEnableReadonly=0@IsWoEnableSave=1@IsWoEnableWF=1@IsWoEnableProperty=1@IsWoEnableRevise=1@IsWoEnableIntoKeepMarkModel=1@FastKeyIsEnable=0@IsWoEnableViewKeepMark=1@FastKeyGenerRole=@IsWoEnableTemplete=1");
                    athDesc.Insert();

                    //有可能在其其它的节点上没有这个附件，所以也要循环增加上它.
                    BP.WF.Nodes nds = new BP.WF.Nodes(this.FK_Flow);
                    foreach (BP.WF.Node nd in nds)
                    {
                        athDesc.FK_MapData = "ND" + nd.NodeID;
                        athDesc.MyPK = athDesc.FK_MapData + "_" + athDesc.NoOfObj;
                        if (athDesc.IsExits == true)
                            continue;

                        athDesc.Insert();
                    }

                    //重新查询一次，把默认值加上.
                    athDesc.RetrieveFromDBSources();
                }
                #endregion 判断是否可以查询出来，如果查询不出来，就可能是公文流程。

                #region 处理权限方案。
                /*首先判断是否具有权限方案*/
                string at = BP.Sys.SystemConfig.AppCenterDBVarStr;
                Paras ps = new BP.DA.Paras();
                ps.SQL = "SELECT FrmSln FROM WF_FrmNode WHERE FK_Node=" + at + "FK_Node AND FK_Flow=" + at + "FK_Flow AND FK_Frm=" + at + "FK_Frm";
                ps.Add("FK_Node", this.FK_Node);
                ps.Add("FK_Flow", this.FK_Flow);
                ps.Add("FK_Frm", this.FK_MapData);
                DataTable dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count == 0)
                {
                    athDesc.RetrieveFromDBSources();
                }
                else
                {
                    int sln = int.Parse(dt.Rows[0][0].ToString());
                    if (sln == 0)
                    {
                        athDesc.RetrieveFromDBSources();
                    }
                    else
                    {
                        int result = athDesc.Retrieve(FrmAttachmentAttr.FK_MapData, this.FK_MapData,
                             FrmAttachmentAttr.FK_Node, this.FK_Node, FrmAttachmentAttr.NoOfObj, this.Ath);

                        if (result == 0) /*如果没有定义，就获取默认的.*/
                            athDesc.RetrieveFromDBSources();
                        //  throw new Exception("@该流程表单在该节点("+this.FK_Node+")使用的是自定义的权限控制，但是没有定义该附件的权限。");
                    }
                }
                #endregion 处理权限方案。
            }

            BP.Sys.FrmAttachmentDBs dbs = new BP.Sys.FrmAttachmentDBs();
            if (athDesc.HisCtrlWay == AthCtrlWay.PWorkID)
            {
                string pWorkID = BP.DA.DBAccess.RunSQLReturnValInt("SELECT PWorkID FROM WF_GenerWorkFlow WHERE WorkID=" + this.PKVal, 0).ToString();
                if (pWorkID == null || pWorkID == "0")
                    pWorkID = this.PKVal;

                if (athDesc.AthUploadWay == AthUploadWay.Inherit)
                {
                    /* 继承模式 */
                    BP.En.QueryObject qo = new BP.En.QueryObject(dbs);
                    qo.AddWhere(FrmAttachmentDBAttr.RefPKVal, pWorkID);
                    qo.addOr();
                    qo.AddWhere(FrmAttachmentDBAttr.RefPKVal, int.Parse(this.PKVal));
                    qo.addOrderBy("RDT");
                    qo.DoQuery();
                }

                if (athDesc.AthUploadWay == AthUploadWay.Interwork)
                {
                    /*共享模式*/
                    dbs.Retrieve(FrmAttachmentDBAttr.RefPKVal, pWorkID);
                }
            }
            else
            {
                int num = dbs.Retrieve(FrmAttachmentDBAttr.FK_FrmAttachment, this.FK_FrmAttachment,
                      FrmAttachmentDBAttr.RefPKVal, this.PKVal, "RDT");
                if (num == 0 && this.IsCC == "1")
                {
                    CCList cc = new CCList();
                    int nnn = cc.Retrieve(CCListAttr.FK_Node, this.FK_Node, CCListAttr.WorkID, this.WorkID, CCListAttr.CCTo, WebUser.No);
                    if (cc.NDFrom != 0)
                    {
                        this._fk_node = cc.NDFrom;

                        dbs.Retrieve(FrmAttachmentDBAttr.FK_FrmAttachment, this.FK_FrmAttachmentExt,
                            FrmAttachmentDBAttr.FK_MapData, "ND" + cc.NDFrom, FrmAttachmentDBAttr.RefPKVal, this.WorkID.ToString());

                        //重新设置文件描述。
                        athDesc.Retrieve(FrmAttachmentAttr.FK_MapData, this.FK_MapData, FrmAttachmentAttr.NoOfObj, "DocMultiAth");
                    }
                }
            }

            #endregion 处理权限控制.

            #region 生成表头表体.
            this.Title = athDesc.Name;
            if (athDesc.FileShowWay == FileShowWay.Pict)
            {

                this.Pub1.Add("<div class='slideBox' id='" + athDesc.MyPK + "' style='width:" + athDesc.W + "px;height:" + athDesc.H + "px; position:relative;  overflow:hidden;'>");
                this.Pub1.Add("<ul class='items'>");
                foreach (FrmAttachmentDB db in dbs)
                {
                    if (db.FileExts.ToUpper() == "JPG" || db.FileExts.ToUpper() == "JPEG" || db.FileExts.ToUpper() == "GIF" || db.FileExts.ToUpper() == "PNG" || db.FileExts.ToUpper() == "BMP")
                    {
                        this.Pub1.Add("<li> <a  title='" + db.MyNote + "'><img src = '" + db.FileFullName + "' width=" + athDesc.W + " height=" + athDesc.H + "/></a></li>");
                    }
                }

                this.Pub1.Add("</ul>");
                this.Pub1.Add("</div>");
                this.Pub1.Add("<script>$(function(){$('#" + athDesc.MyPK + "').slideBox({duration : 0.3,easing : 'linear',delay : 5,hideClickBar : false,clickBarRadius : 10});})</script>");
                /* 如果是图片轮播，就在这里根据数据输出轮播的html代码.*/
                return;
            }
            float athWidth = athDesc.W - 20;
            // 执行装载模版.
            if (dbs.Count == 0 && athDesc.IsWoEnableTemplete == true)
            {
                /*如果数量为0,就检查一下是否有模版如果有就加载模版文件.*/
                string templetePath = BP.Sys.SystemConfig.PathOfDataUser + "AthTemplete\\" + athDesc.NoOfObj.Trim();
                if (Directory.Exists(templetePath) == false)
                    Directory.CreateDirectory(templetePath);

                /*有模版文件夹*/
                DirectoryInfo mydir = new DirectoryInfo(templetePath);
                FileInfo[] fls = mydir.GetFiles();
                if (fls.Length == 0)
                    throw new Exception("@流程设计错误，该多附件启用了模版组件，模版目录:" + templetePath + "里没有模版文件.");

                foreach (FileInfo fl in fls)
                {
                    if (System.IO.Directory.Exists(athDesc.SaveTo) == false)
                        System.IO.Directory.CreateDirectory(athDesc.SaveTo);

                    int oid = BP.DA.DBAccess.GenerOID();
                    string saveTo = athDesc.SaveTo + "\\" + oid + "." + fl.Name.Substring(fl.Name.LastIndexOf('.') + 1);
                    if (saveTo.Contains("@") == true || saveTo.Contains("*") == true)
                    {
                        /*如果有变量*/
                        saveTo = saveTo.Replace("*", "@");
                        if (saveTo.Contains("@") && this.FK_Node != null)
                        {
                            /*如果包含 @ */
                            BP.WF.Flow flow = new BP.WF.Flow(this.FK_Flow);
                            BP.WF.Data.GERpt myen = flow.HisGERpt;
                            myen.OID = this.WorkID;
                            myen.RetrieveFromDBSources();
                            saveTo = BP.WF.Glo.DealExp(saveTo, myen, null);
                        }
                        if (saveTo.Contains("@") == true)
                            throw new Exception("@路径配置错误,变量没有被正确的替换下来." + saveTo);
                    }
                    fl.CopyTo(saveTo);

                    FileInfo info = new FileInfo(saveTo);
                    FrmAttachmentDB dbUpload = new FrmAttachmentDB();

                    dbUpload.CheckPhysicsTable();
                    dbUpload.MyPK = athDesc.FK_MapData + oid.ToString();
                    dbUpload.NodeID = FK_Node.ToString();
                    dbUpload.FK_FrmAttachment = this.FK_FrmAttachment;

                    if (athDesc.AthUploadWay == AthUploadWay.Inherit)
                    {
                        /*如果是继承，就让他保持本地的PK. */
                        dbUpload.RefPKVal = this.PKVal.ToString();
                    }

                    if (athDesc.AthUploadWay == AthUploadWay.Interwork)
                    {
                        /*如果是协同，就让他是PWorkID. */
                        string pWorkID = BP.DA.DBAccess.RunSQLReturnValInt("SELECT PWorkID FROM WF_GenerWorkFlow WHERE WorkID=" + this.PKVal, 0).ToString();
                        if (pWorkID == null || pWorkID == "0")

                            pWorkID = this.PKVal;
                        dbUpload.RefPKVal = pWorkID;
                    }

                    dbUpload.FK_MapData = athDesc.FK_MapData;
                    dbUpload.FK_FrmAttachment = this.FK_FrmAttachment;

                    dbUpload.FileExts = info.Extension;
                    dbUpload.FileFullName = saveTo;
                    dbUpload.FileName = fl.Name;
                    dbUpload.FileSize = (float)info.Length;

                    dbUpload.RDT = DataType.CurrentDataTime;
                    dbUpload.Rec = BP.Web.WebUser.No;
                    dbUpload.RecName = BP.Web.WebUser.Name;
                    //if (athDesc.IsNote)
                    //    dbUpload.MyNote = this.Pub1.GetTextBoxByID("TB_Note").Text;
                    //if (athDesc.Sort.Contains(","))
                    //    dbUpload.Sort = this.Pub1.GetDDLByID("ddl").SelectedItemStringVal;

                    dbUpload.Insert();

                    dbs.AddEntity(dbUpload);
                }
                //BP.Sys.FrmAttachmentDBs dbs = new BP.Sys.FrmAttachmentDBs();
            }

            #region 处理权限问题.
            // 处理权限问题, 有可能当前节点是可以上传或者删除，但是当前节点上不能让此人执行工作。
            bool isDel = athDesc.IsDelete;
            bool isUpdate = athDesc.IsUpload;
            if (isDel == true || isUpdate == true)
            {
                if (this.WorkID != 0
                    && string.IsNullOrEmpty(this.FK_Flow) == false
                    && this.FK_Node != 0)
                {
                    isDel = BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(this.FK_Flow, this.FK_Node, this.WorkID, WebUser.No);
                    if (isDel == false)
                        isUpdate = false;
                }
            }
            #endregion 处理权限问题.

            if (athDesc.FileShowWay == FileShowWay.Free)
            {
                this.Pub1.AddTable("border='0' cellspacing='0' cellpadding='0' style='width:" + athWidth + "px'");

                foreach (FrmAttachmentDB db in dbs)
                {
                    this.Pub1.AddTR();
                    if (CanEditor(db.FileExts))
                    {
                        if (athDesc.IsWoEnableWF)
                        {
                            this.Pub1.AddTD("<a href=\"javascript:OpenOfiice('" + this.FK_FrmAttachment + "','" +
                                 this.WorkID + "','" + db.MyPK + "','" + this.FK_MapData + "','" + this.Ath +
                                 "','" + this.FK_Node + "')\"><img src='../Img/FileType/" + db.FileExts + ".gif' border=0 onerror=\"src='../Img/FileType/Undefined.gif'\" />" + db.FileName + "</a>");
                        }
                        else
                        {
                            this.Pub1.AddTD("<a href=\"javascript:OpenFileView('" + this.PKVal + "','" + db.MyPK +
                                        "')\"><img src='../Img/FileType/" + db.FileExts + ".gif' border=0 onerror=\"src='../Img/FileType/Undefined.gif'\" />" + db.FileName + "</a>");
                        }
                    }
                    else if (db.FileExts.ToUpper() == "JPG" || db.FileExts.ToUpper() == "JPEG" ||
                             db.FileExts.ToUpper() == "GIF" || db.FileExts.ToUpper() == "PNG" ||
                             db.FileExts.ToUpper() == "BMP" || db.FileExts.ToUpper() == "PDF" || db.FileExts.ToUpper() == "CEB")
                    {
                        this.Pub1.AddTD("<a href=\"javascript:OpenFileView('" + this.PKVal + "','" + db.MyPK +
                                        "')\"><img src='../Img/FileType/" + db.FileExts + ".gif' border=0 onerror=\"src='../Img/FileType/Undefined.gif'\" />" + db.FileName + "</a>");
                    }
                    else
                    {
                        this.Pub1.AddTD("<a href='AttachmentUpload.aspx?DoType=Down&MyPK=" + db.MyPK +
                                        "' target=_blank ><img src='../Img/FileType/" + db.FileExts + ".gif' border=0 onerror=\"src='../Img/FileType/Undefined.gif'\" />" + db.FileName + "</a>");
                    }

                    if (athDesc.IsDownload)
                        this.Pub1.AddTD("<a href=\"javascript:Down('" + this.FK_FrmAttachment + "','" + this.PKVal + "','" + db.MyPK + "')\">下载</a>");
                    else
                        this.Pub1.AddTD("");

                    if (this.IsReadonly != "1")
                    {
                        if (athDesc.IsDelete)
                            this.Pub1.AddTD("style='border:0px'", "<a href=\"javascript:Del('" + this.FK_FrmAttachment + "','" + this.PKVal + "','" + db.MyPK + "')\">删除</a>");
                        else
                            this.Pub1.AddTD("");
                    }
                    else
                    {
                        this.Pub1.AddTD("");
                        this.Pub1.AddTD("");
                    }

                    this.Pub1.AddTREnd();
                }
                AddFileUpload(isUpdate, athDesc);
                this.Pub1.AddTableEnd();
                return;
            }

            this.Pub1.AddTable("border='0' cellspacing='0' cellpadding='0' style='width:" + athWidth+ "px'");
            if (athDesc.IsShowTitle == true)
            {
                this.Pub1.AddTR("style='border:0px'");

                this.Pub1.AddTDTitleExt("序号");
                if (athDesc.Sort.Contains(","))
                    this.Pub1.AddTD("style='background:#f4f4f4; font-size:12px; padding:3px;'", "类别");
                this.Pub1.AddTDTitleExt("文件名");
                this.Pub1.AddTDTitleExt("大小KB");
                this.Pub1.AddTDTitleExt("上传时间");
                this.Pub1.AddTDTitleExt("上传人");
                if (athDesc.IsDownload)
                    this.Pub1.AddTDTitleExt("下载");
                this.Pub1.AddTDTitleExt("操作");
                this.Pub1.AddTREnd();
            }

            int i = 0;
            foreach (FrmAttachmentDB db in dbs)
            {
                i++;
                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(i);
                if (athDesc.Sort.Contains(","))
                    this.Pub1.AddTD(db.Sort);

                // this.Pub1.AddTDIdx(i++);
                if (athDesc.IsDownload)
                {
                    if (athDesc.IsWoEnableWF && CanEditor(db.FileExts))
                    {
                        this.Pub1.AddTD("<a href=\"javascript:OpenOfiice('" + this.FK_FrmAttachment + "','" + this.WorkID + "','" + db.MyPK + "','" + this.FK_MapData + "','" + this.Ath + "','" + this.FK_Node + "')\"><img src='../Img/FileType/" + db.FileExts + ".gif' border=0 onerror=\"src='../Img/FileType/Undefined.gif'\" />" + db.FileName + "</a>");
                    }
                    else if (db.FileExts.ToUpper() == "JPG" || db.FileExts.ToUpper() == "JPEG" || db.FileExts.ToUpper() == "GIF" || db.FileExts.ToUpper() == "PNG" || db.FileExts.ToUpper() == "BMP" || db.FileExts.ToUpper() == "PDF" || db.FileExts.ToUpper() == "CEB")
                    {
                        this.Pub1.AddTD("<a href=\"javascript:OpenFileView('" + this.PKVal + "','" + db.MyPK + "')\"><img src='../Img/FileType/" + db.FileExts + ".gif' border=0 onerror=\"src='../Img/FileType/Undefined.gif'\" />" + db.FileName + "</a>");
                    }
                    else
                    {
                        this.Pub1.AddTD("<a href='AttachmentUpload.aspx?DoType=Down&MyPK=" + db.MyPK + "' target=_blank ><img src='../Img/FileType/" + db.FileExts + ".gif' border=0 onerror=\"src='../Img/FileType/Undefined.gif'\" />" + db.FileName + "</a>");
                    }
                }
                else
                    this.Pub1.AddTD("<img src='../Img/FileType/" + db.FileExts + ".gif' border=0 onerror=\"src='../Img/FileType/Undefined.gif'\" />" + db.FileName);

                this.Pub1.AddTD(db.FileSize);
                this.Pub1.AddTD(db.RDT);
                this.Pub1.AddTD(db.RecName);
                if (athDesc.IsDownload)
                    this.Pub1.AddTD("<a href=\"javascript:Down('" + this.FK_FrmAttachment + "','" + this.PKVal + "','" + db.MyPK + "')\">下载</a>");

                if (this.IsReadonly != "1")
                {
                    string op = null;
                    if (isDel == true)
                    {
                        op = "<a href=\"javascript:Del('" + this.FK_FrmAttachment + "','" + this.PKVal + "','" + db.MyPK + "')\">删除</a>";
                    }
                    this.Pub1.AddTD(op);
                }
                else
                    this.Pub1.AddTD("");
                this.Pub1.AddTREnd();
            }
            if (i == 0)
            {
                this.Pub1.AddTR();
                this.Pub1.AddTD("0");
                if (athDesc.Sort.Contains(","))
                    this.Pub1.AddTD("&nbsp&nbsp");
                this.Pub1.AddTD("style='width:100px'", "<span style='color:red;' >上传附件</span>");
                this.Pub1.AddTD("&nbsp&nbsp");
                this.Pub1.AddTD("&nbsp&nbsp");
                this.Pub1.AddTD("&nbsp&nbsp");
                this.Pub1.AddTD("&nbsp&nbsp");
                this.Pub1.AddTD("&nbsp&nbsp");
                this.Pub1.AddTREnd();
            }

            AddFileUpload(isUpdate, athDesc);
            this.Pub1.AddTableEnd();
            #endregion 生成表头表体.
        }

        private void AddFileUpload(bool isUpdate, FrmAttachment athDesc)
        {
            if (isUpdate == true && this.IsReadonly != "1")
            {
                this.Pub1.AddTR();
                if (athDesc.IsNote)
                    this.Pub1.AddTDBegin("colspan=8");
                else
                    this.Pub1.AddTDBegin("colspan=7");
                this.Pub1.Add("文件:");

                System.Web.UI.WebControls.FileUpload fu = new System.Web.UI.WebControls.FileUpload();
                fu.ID = "file";
                fu.BorderStyle = BorderStyle.NotSet;
                fu.Attributes["onchange"] = "UploadChange('Btn_Upload');";
                this.Pub1.Add(fu);
                if (athDesc.Sort.Contains(","))
                {
                    string[] strs = athDesc.Sort.Split(',');
                    BP.Web.Controls.DDL ddl = new BP.Web.Controls.DDL();
                    ddl.ID = "ddl";
                    foreach (string str in strs)
                    {
                        if (str == null || str == "")
                            continue;
                        ddl.Items.Add(new ListItem(str, str));
                    }
                    this.Pub1.Add(ddl);
                }

                if (athDesc.IsNote)
                {
                    TextBox tb = new TextBox();
                    tb.ID = "TB_Note";
                    tb.Attributes["Width"] = "90%";
                    tb.Attributes["style"] = "display:none;";
                    // tb.Attributes["class"] = "TBNote";
                    tb.Columns = 30;
                    // this.Pub1.Add("&nbsp;备注:");
                    this.Pub1.Add(tb);
                }
                //Button btn = new Button();
                //btn.Text = "上传";
                //btn.ID = "Btn_Upload";
                //btn.CssClass = "Btn";
                //btn.Click += new EventHandler(btn_Click);
                //btn.Attributes["style"] = "display:none;";
                //this.Pub1.Add(btn);
                this.Pub1.AddTDEnd();
                this.Pub1.AddTREnd();
            }
        }

        private string GetRealPath(string fileFullName)
        {
            bool isFile = false;
            string downpath = "";
            try
            {
                //如果相对路径获取不到可能存储的是绝对路径
                FileInfo downInfo = new FileInfo(Server.MapPath("~/" + fileFullName));
                isFile = true;
                downpath = Server.MapPath("~/" + fileFullName);
            }
            catch (Exception)
            {
                FileInfo downInfo = new FileInfo(fileFullName);
                isFile = true;
                downpath = fileFullName;
            }
            if (!isFile)
            {
                throw new Exception("没有找到下载的文件路径！");
            }

            return downpath;
        }

        private bool CanEditor(string fileType)
        {
            try
            {
                string fileTypes = BP.Sys.SystemConfig.AppSettings["OpenTypes"];
                if (string.IsNullOrEmpty(fileTypes) == true)
                    fileTypes = "doc,docx,pdf,xls,xlsx";

                if (fileTypes.Contains(fileType.ToLower()))
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        protected void btn_Click(object sender, EventArgs e)
        {
            BP.Sys.FrmAttachment athDesc = new BP.Sys.FrmAttachment(this.FK_FrmAttachment);

            System.Web.UI.WebControls.FileUpload fu = this.Pub1.FindControl("file") as System.Web.UI.WebControls.FileUpload;
            if (fu.HasFile == false || fu.FileName.Length <= 2)
            {
                this.Alert("请选择上传的文件.");
                return;
            }
            string exts = System.IO.Path.GetExtension(fu.FileName).ToLower().Replace(".", "");

            //如果有上传类型限制，进行判断格式
            if (athDesc.Exts == "*.*" || athDesc.Exts == "")
            {
                /*任何格式都可以上传*/
            }
            else
            {
                if (athDesc.Exts.ToLower().Contains(exts) == false)
                {
                    this.Alert("您上传的文件，不符合系统的格式要求，要求的文件格式:" + athDesc.Exts + "，您现在上传的文件格式为:" + exts);
                    return;
                }
            }

            string savePath = athDesc.SaveTo;

            if (savePath.Contains("@") == true || savePath.Contains("*") == true)
            {
                /*如果有变量*/
                savePath = savePath.Replace("*", "@");
                GEEntity en = new GEEntity(athDesc.FK_MapData);
                en.PKVal = this.PKVal;
                en.Retrieve();
                savePath = BP.WF.Glo.DealExp(savePath, en, null);

                if (savePath.Contains("@") && this.FK_Node != null)
                {
                    /*如果包含 @ */
                    BP.WF.Flow flow = new BP.WF.Flow(this.FK_Flow);
                    BP.WF.Data.GERpt myen = flow.HisGERpt;
                    myen.OID = this.WorkID;
                    myen.RetrieveFromDBSources();
                    savePath = BP.WF.Glo.DealExp(savePath, myen, null);
                }
                if (savePath.Contains("@") == true)
                    throw new Exception("@路径配置错误,变量没有被正确的替换下来." + savePath);
            }
            else
            {
                //savePath = athDesc.SaveTo + "\\" + this.PKVal;
            }

            //替换关键的字串.
            savePath = savePath.Replace("\\\\", "\\");
            try
            {

                savePath = Server.MapPath("~/" + savePath);

            }
            catch (Exception)
            {
                savePath = savePath;

            }
            try
            {

                if (System.IO.Directory.Exists(savePath) == false)
                {
                    System.IO.Directory.CreateDirectory(savePath);
                    //System.IO.Directory.CreateDirectory(athDesc.SaveTo);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("@创建路径出现错误，可能是没有权限或者路径配置有问题:" + Server.MapPath("~/" + savePath) + "===" + savePath + "@技术问题:" + ex.Message);
            }

            //int oid = BP.DA.DBAccess.GenerOID();
            string guid = BP.DA.DBAccess.GenerGUID();

            string fileName = fu.FileName.Substring(0, fu.FileName.LastIndexOf('.'));
            //string ext = fu.FileName.Substring(fu.FileName.LastIndexOf('.') + 1);
            string ext = System.IO.Path.GetExtension(fu.FileName);

            //string realSaveTo = Server.MapPath("~/" + savePath) + "/" + guid + "." + fileName + "." + ext;

            //string realSaveTo = Server.MapPath("~/" + savePath) + "\\" + guid + "." + fu.FileName.Substring(fu.FileName.LastIndexOf('.') + 1);
            //string saveTo = savePath + "/" + guid + "." + fileName + "." + ext;



            string realSaveTo = savePath + "/" + guid + "." + fileName + ext;

            string saveTo = realSaveTo;

            try
            {
                fu.SaveAs(realSaveTo);
            }
            catch (Exception ex)
            {
                this.Response.Write("@文件存储失败,有可能是路径的表达式出问题,导致是非法的路径名称:" + ex.Message);
                return;
            }

            FileInfo info = new FileInfo(realSaveTo);
            FrmAttachmentDB dbUpload = new FrmAttachmentDB();

            dbUpload.MyPK = guid; // athDesc.FK_MapData + oid.ToString();
            dbUpload.NodeID = FK_Node.ToString();
            dbUpload.FK_FrmAttachment = this.FK_FrmAttachment;

            if (athDesc.AthUploadWay == AthUploadWay.Inherit)
            {
                /*如果是继承，就让他保持本地的PK. */
                dbUpload.RefPKVal = this.PKVal.ToString();
            }

            if (athDesc.AthUploadWay == AthUploadWay.Interwork)
            {
                /*如果是协同，就让他是PWorkID. */
                string pWorkID = BP.DA.DBAccess.RunSQLReturnValInt("SELECT PWorkID FROM WF_GenerWorkFlow WHERE WorkID=" + this.PKVal, 0).ToString();
                if (pWorkID == null || pWorkID == "0")
                    pWorkID = this.PKVal;

                dbUpload.RefPKVal = pWorkID;
            }

            dbUpload.FK_MapData = athDesc.FK_MapData;
            dbUpload.FK_FrmAttachment = this.FK_FrmAttachment;

            dbUpload.FileExts = info.Extension;
            dbUpload.FileFullName = saveTo;
            dbUpload.FileName = fu.FileName;
            dbUpload.FileSize = (float)info.Length;

            dbUpload.RDT = DataType.CurrentDataTimess;
            dbUpload.Rec = BP.Web.WebUser.No;
            dbUpload.RecName = BP.Web.WebUser.Name;
            if (athDesc.IsNote)
                dbUpload.MyNote = this.Pub1.GetTextBoxByID("TB_Note").Text;

            if (athDesc.Sort.Contains(","))
                dbUpload.Sort = this.Pub1.GetDDLByID("ddl").SelectedItemStringVal;

            dbUpload.UploadGUID = guid;
            dbUpload.Insert();

            //   this.Response.Redirect("AttachmentUpload.aspx?FK_FrmAttachment=" + this.FK_FrmAttachment + "&PKVal=" + this.PKVal, true);
            this.Response.Redirect(this.Request.RawUrl, true);

        }
    }
}