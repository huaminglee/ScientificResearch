using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF.Template;
using BP.WF;
using BP.En;
using BP.DA;
using BP.Sys;
using BP.Web;


namespace CCFlow.WF.CCForm
{
    public partial class WF_Frm : BP.Web.WebPage
    {
        #region 属性
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public int FK_Node
        {
            get
            {
                try
                {
                    string nodeid = this.Request.QueryString["NodeID"];
                    if (nodeid == null)
                        nodeid = this.Request.QueryString["FK_Node"];
                    return int.Parse(nodeid);
                }
                catch
                {
                    if (string.IsNullOrEmpty(this.FK_Flow))
                        return 0;
                    else
                        return int.Parse(this.FK_Flow); // 0; 有可能是流程调用流程表单。
                }
            }
        }
        public string WorkID
        {
            get
            {
                return this.Request.QueryString["WorkID"];
            }
        }
        public int OID
        {
            get
            {
                string cworkid = this.Request.QueryString["CWorkID"];
                if (string.IsNullOrEmpty(cworkid) == false && int.Parse(cworkid) != 0)
                    return int.Parse(cworkid);

                string oid = this.Request.QueryString["WorkID"];
                if (oid == null || oid == "")
                    oid = this.Request.QueryString["OID"];
                if (oid == null || oid == "")
                    oid = "0";
                return int.Parse(oid);
            }
        }

        /// <summary>
        /// 延续流程ID
        /// </summary>
        public int CWorkID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["CWorkID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 父流程ID
        /// </summary>
        public int PWorkID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["PWorkID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 流程ID
        /// </summary>
        public int FID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["FID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        public int OIDPKVal
        {
            get
            {

                if (ViewState["OIDPKVal"] == null)
                    return 0;
                return int.Parse(ViewState["OIDPKVal"].ToString());
            }
            set
            {
                ViewState["OIDPKVal"] = value;
            }
        }
        public string FK_MapData
        {
            get
            {
                string s = this.Request.QueryString["FK_MapData"];
                if (s == null)
                    return "ND101";
                return s;
            }
        }
        public bool IsEdit
        {
            get
            {
                if (this.Request.QueryString["IsEdit"] == "0")
                    return false;
                return true;
            }
        }
        public string SID
        {
            get
            {
                return this.Request.QueryString["PWorkID"];
            }
        }
        public bool IsPrint
        {
            get
            {
                if (this.Request.QueryString["IsPrint"] == "1")
                    return true;
                return false;
            }
        }
        private FrmNode _HisFrmNode = null;
        public FrmNode HisFrmNode
        {
            get
            {
                if (_HisFrmNode == null)
                    _HisFrmNode = new FrmNode();
                return _HisFrmNode;
            }
            set
            {
                _HisFrmNode = value;
            }
        }

        private string _height = "";
        public string Height
        {
            get { return _height; }
            set { _height = value; }
        }

        private string _width = "";
        public string Width
        {
            get { return _width; }
            set { _width = value; }
        }
        #endregion 属性

        protected void Page_Load(object sender, EventArgs e)
        {

#warning 没有缓存经常预览与设计不一致

            if (this.Request.QueryString["IsTest"] == "1")
                BP.Sys.SystemConfig.DoClearCash_del();

            if (this.Request.QueryString["IsLoadData"] == "1")
                this.UCEn1.IsLoadData = true;

            MapData md = new MapData();
            md.No = this.FK_MapData;
            if (md.RetrieveFromDBSources() == 0 && md.Name.Length > 3)
            {
                /*如果没有找到，就可能是dtl。*/
                if (md.HisFrmType == FrmType.Url)
                {
                    string no = Request.QueryString["NO"];
                    string urlParas = "OID=" + this.OID + "&NO=" + no + "&WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&UserNo=" + WebUser.No + "&SID=" + this.SID;
                    /*如果是URL.*/
                    if (md.Url.Contains("?") == true)
                        this.Response.Redirect(md.Url + "&" + urlParas, true);
                    else
                        this.Response.Redirect(md.Url + "?" + urlParas, true);
                    return;
                }

                /* 没有找到此map. */
                MapDtl dtl = new MapDtl(this.FK_MapData);
                GEDtl dtlEn = dtl.HisGEDtl;
                dtlEn.SetValByKey("OID", this.FID);

                if (dtlEn.EnMap.Attrs.Count <= 0)
                {
                    md.RepairMap();
                    this.Response.Redirect(this.Request.RawUrl, true);
                    return;
                }

                int i = dtlEn.RetrieveFromDBSources();

                string[] paras = this.RequestParas.Split('&');
                foreach (string str in paras)
                {
                    if (string.IsNullOrEmpty(str) || str.Contains("=") == false)
                        continue;

                    string[] kvs = str.Split('=');
                    dtlEn.SetValByKey(kvs[0], kvs[1]);
                }
                Width = md.MaxRight + md.MaxLeft * 2 + 10 + "";
                if (float.Parse(Width) < 500)
                    Width = "900";
                Height = md.MaxEnd + "";

                this.UCEn1.Add("<div id=divCCForm style='width:" + Width + "px;height:" + Height + "px' >");
                if (md.HisFrmType == FrmType.FreeFrm)
                    this.UCEn1.BindCCForm(dtlEn, this.FK_MapData, !this.IsEdit, 0);

                if (md.HisFrmType == FrmType.Column4Frm)
                    this.UCEn1.BindCCForm(dtlEn, this.FK_MapData, !this.IsEdit, 0);

                this.AddJSEvent(dtlEn);
                this.UCEn1.Add("</div>");
            }
            else
            {
                /*如果没有找到，就可能是dtl。*/
                if (md.HisFrmType == FrmType.Url)
                {
                    string no = Request.QueryString["NO"];
                    string urlParas = "OID=" + this.OID + "&NO=" + no + "&WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&UserNo=" + WebUser.No + "&SID=" + this.SID;
                    /*如果是URL.*/
                    if (md.Url.Contains("?") == true)
                        this.Response.Redirect(md.Url + "&" + urlParas, true);
                    else
                        this.Response.Redirect(md.Url + "?" + urlParas, true);
                    return;
                }

                if (md.HisFrmType == FrmType.WordFrm)
                {
                    string no = Request.QueryString["NO"];
                    string urlParas = "OID=" + this.OID + "&NO=" + no + "&WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&UserNo=" + WebUser.No + "&SID=" + this.SID + "&FK_MapData=" + this.FK_MapData + "&OIDPKVal=" + this.OIDPKVal + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow;
                    /*如果是URL.*/
                    string requestParas = this.RequestParas;
                    string[] parasArrary = this.RequestParas.Split('&');
                    foreach (string str in parasArrary)
                    {
                        if (string.IsNullOrEmpty(str) || str.Contains("=") == false)
                            continue;
                        string[] kvs = str.Split('=');
                        if (urlParas.Contains(kvs[0]))
                            continue;
                        urlParas += "&" + kvs[0] + "=" + kvs[1];
                    }
                    if (md.Url.Contains("?") == true)
                        this.Response.Redirect("WordFrm.aspx?1=2" + "&" + urlParas, true);
                    else
                        this.Response.Redirect("WordFrm.aspx" + "?" + urlParas, true);
                    return;
                }

                if (md.HisFrmType == FrmType.ExcelFrm)
                {
                    this.Response.Redirect("FrmExcel.aspx?1=2" + this.RequestParas, true);
                    return;
                }

                GEEntity en = md.HisGEEn;

                #region 求出pk 值.
                int pk = this.OID;
                string nodeid = this.FK_Node.ToString();
                if (nodeid != "0" && string.IsNullOrEmpty(this.FK_Flow) == false)
                {
                    /*说明是流程调用它， 就要判断谁是表单的PK.*/
                    FrmNode fn = new FrmNode(this.FK_Flow, this.FK_Node, this.FK_MapData);
                    switch (fn.WhoIsPK)
                    {
                        case WhoIsPK.FID:
                            pk = this.FID;
                            if (pk == 0)
                                throw new Exception("@没有接收到参数FID");
                            break;
                        case WhoIsPK.CWorkID: /*延续流程ID*/
                            pk = this.CWorkID;
                            if (pk == 0)
                                throw new Exception("@没有接收到参数CWorkID");
                            break;
                        case WhoIsPK.PWorkID: /*父流程ID*/
                            pk = this.PWorkID;
                            if (pk == 0)
                                throw new Exception("@没有接收到参数PWorkID");
                            break;
                        case WhoIsPK.OID:
                        default:
                            break;
                    }

                    //Node nd = new Node(int.Parse(nodeid));
                    //if (nd.HisRunModel == RunModel.SubThread
                    //    && nd.HisSubThreadType == SubThreadType.UnSameSheet
                    //    && this.FK_MapData != "ND" + nd.NodeID)
                    //{
                    //    /*如果是子线程, 并且是异表单节点.*/
                    //    pk = this.FID; // 是子线程，并且是异表单的子线程，并且不是节点表单。这样设置是为了到合流点上能够按FID进行表单数据汇总.
                    //}
                }
                en.SetValByKey("OID", pk);
                #endregion 求出pk 值.

                if (en.EnMap.Attrs.Count <= 0)
                {
                    md.RepairMap(); //让他刷新一下,重新进入.
                    this.Response.Redirect(this.Request.RawUrl, true);
                    return;
                }

                int i = en.RetrieveFromDBSources();

                if (i == 0)
                {
                    en.ResetDefaultValAllAttr();
                    en.DirectInsert();
                }

                string[] paras = this.RequestParas.Split('&');
                foreach (string str in paras)
                {
                    if (string.IsNullOrEmpty(str) || str.Contains("=") == false)
                        continue;

                    string[] kvs = str.Split('=');
                    en.SetValByKey(kvs[0], kvs[1]);
                }

                if (en.ToString() == "0")
                {
                    en.SetValByKey("OID", pk);
                }
                this.OIDPKVal = pk;


                #region 处理表单权限控制方案
                Width = md.MaxRight + md.MaxLeft * 2 + 10 + "";
                if (float.Parse(Width) < 500)
                    Width = "900";
                Height = md.MaxEnd + "";
                this.UCEn1.Add("<div id=divCCForm style='width:" + Width + "px;height:" + Height + "px' >");
                if (nodeid != null)
                {
                    this.UCEn1.FK_Node = this.FK_Node;
                    /*处理表单权限控制方案*/
                    this.HisFrmNode = new FrmNode();
                    int ii = this.HisFrmNode.Retrieve(FrmNodeAttr.FK_Frm, this.FK_MapData,
                       FrmNodeAttr.FK_Node, int.Parse(nodeid));

                    if (ii == 0 || this.HisFrmNode.FrmSln == 0)
                    {
                        /*说明没有配置,或者方案编号为默认就不用处理,*/
                        this.UCEn1.BindCCForm(en, this.FK_MapData, !this.IsEdit, 0);
                    }
                    else
                    {
                        BP.Sys.FrmFields fls = new BP.Sys.FrmFields(this.FK_MapData, this.HisFrmNode.FrmSln);
                        //fls.Retrieve(FrmFieldAttr.FK_MapData, this.FK_MapData,
                        //    FrmFieldAttr.FK_Node, this.HisFrmNode.FrmSln);

                        //求出集合.
                        MapAttrs mattrs = new MapAttrs(this.FK_MapData);
                        foreach (BP.Sys.FrmField item in fls)
                        {
                            foreach (MapAttr attr in mattrs)
                            {
                                if (attr.KeyOfEn != item.KeyOfEn)
                                    continue;

                                if (item.IsSigan)
                                    item.UIIsEnable = false;

                                //if (attr.UIContralType == UIContralType.DDL)
                                //    attr.UIIsEnable = item.UIIsEnable;
                                //else
                                attr.UIIsEnable = item.UIIsEnable;

                                attr.UIVisible = item.UIVisible;
                                attr.IsSigan = item.IsSigan;
                                attr.DefValReal = item.DefVal;
                            }
                        }

                        #region 设置默认值.
                        bool isHave = false;
                        foreach (MapAttr attr in mattrs)
                        {
                            if (attr.UIIsEnable)
                                continue;

                            if (attr.DefValReal.Contains("@") == false)
                                continue;

                            en.SetValByKey(attr.KeyOfEn, attr.DefVal);
                            isHave = true;
                        }
                        if (isHave)
                            en.DirectUpdate(); //让其直接更新.
                        #endregion 设置默认值.

                        //按照当前方案绑定表单.
                        this.UCEn1.BindCCForm(en, md, mattrs, this.FK_MapData, !this.IsEdit, 0);

                        #region 检查必填项
                        string scriptCheckFrm = "";
                        scriptCheckFrm += "\t\n<script type='text/javascript' >";
                        scriptCheckFrm += "\t\n function CheckFrmSlnIsNull(){ ";
                        scriptCheckFrm += "\t\n var isPass = true;";
                        scriptCheckFrm += "\t\n var alloweSave = true;";
                        scriptCheckFrm += "\t\n var erroMsg = '提示信息:';";

                        //表单权限设置为必填项
                        //查询出来，需要不为空的
                        Paras ps = new Paras();
                        ps.SQL = "SELECT KeyOfEn, Name FROM Sys_FrmSln WHERE FK_MapData=" + ps.DBStr + "FK_MapData AND FK_Node=" + ps.DBStr + "FK_Node AND IsNotNull=" + ps.DBStr + "IsNotNull";
                        ps.Add(BP.Sys.FrmFieldAttr.FK_MapData, this.FK_MapData);
                        ps.Add(BP.Sys.FrmFieldAttr.FK_Node, this.FK_Node);
                        ps.Add(BP.Sys.FrmFieldAttr.IsNotNull, 1);

                        //查询
                        System.Data.DataTable dtKeys = DBAccess.RunSQLReturnTable(ps);
                        // 检查数据是否完整.
                        foreach (System.Data.DataRow dr in dtKeys.Rows)
                        {
                            string key = dr[0].ToString();
                            string name = dr[1].ToString();
                            BP.Web.Controls.TB TB_NotNull = this.UCEn1.GetTBByID("TB_" + key);
                            if (TB_NotNull != null)
                            {
                                scriptCheckFrm += "\t\n try{  ";
                                scriptCheckFrm += "\t\n var element = document.getElementById('" + TB_NotNull.ClientID + "');";
                                //验证输入的正则格式
                                scriptCheckFrm += "\t\n if(element && element.readOnly == true) return;";
                                scriptCheckFrm += "\t\n isPass = EleSubmitCheck(element,'.{1}','" + name + "，不能为空。');";
                                scriptCheckFrm += "\t\n  if(isPass == false){";
                                scriptCheckFrm += "\t\n    alloweSave = false;";
                                scriptCheckFrm += "\t\n    erroMsg += '" + name + "，不能为空。';";
                                scriptCheckFrm += "\t\n  }";
                                scriptCheckFrm += "\t\n } catch(e) { ";
                                scriptCheckFrm += "\t\n  alert(e.name  + e.message);  return false;";
                                scriptCheckFrm += "\t\n } ";
                            }
                            else
                            {

                            }
                        }

                        //scriptCheckFrm += "\t\n if(alloweSave == false){";
                        //scriptCheckFrm += "\t\n     alert(erroMsg);";
                        //scriptCheckFrm += "\t\n  } ";
                        scriptCheckFrm += "\t\n return alloweSave; } ";
                        scriptCheckFrm += "\t\n</script>";
                        #endregion
                        //检查必填项
                        this.UCEn1.Add(scriptCheckFrm);

                    }
                }
                else
                {
                    this.UCEn1.BindCCForm(en, this.FK_MapData, !this.IsEdit, 0);
                }
                this.UCEn1.Add("</div>");
                #endregion

                if (md.IsHaveCA)
                {
                    #region 检查是否有ca签名.
                    if (md.IsHaveCA == true)
                    {
                        this.TB_SealData.Text = en.GetValStringByKey("SealData");
                    }
                    #endregion 检查是否有ca签名.
                }
                this.AddJSEvent(en);
            }

            Session["Count"] = null;
            this.Btn_Save.Click += new EventHandler(Btn_Save_Click);
            this.Btn_Save.Visible = this.HisFrmNode.IsEdit;
            this.Btn_Save.Enabled = this.HisFrmNode.IsEdit;
            this.Btn_Save.BackColor = System.Drawing.Color.White;
            Node curNd = new Node();
            curNd.NodeID = this.FK_Node;
            curNd.RetrieveFromDBSources();

            if (curNd.FormType == NodeFormType.SheetTree)
            {
                this.Btn_Save.Visible = true;
                this.Btn_Save.Enabled = true;
                this.Btn_Print.Enabled = false;
                this.Btn_Print.Visible = false;
            }
            else
            {
                this.Btn_Print.Visible = this.HisFrmNode.IsPrint;
                this.Btn_Print.Enabled = this.HisFrmNode.IsPrint;
                this.Btn_Print.Attributes["onclick"] = "window.open('Print.aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&FK_MapData=" + this.FK_MapData + "&WorkID=" + this.OID + "', '', 'dialogHeight: 350px; dialogWidth:450px; center: yes; help: no'); return false;";
            }
        }
        public void AddJSEvent(Entity en)
        {
            Attrs attrs = en.EnMap.Attrs;
            foreach (Attr attr in attrs)
            {
                if (attr.UIIsReadonly || attr.UIVisible == false)
                    continue;
                if (attr.IsFKorEnum)
                {
                    var ddl = this.UCEn1.GetDDLByID("DDL_" + attr.Key);
                }
            }
        }
        /// <summary>
        /// 保存点
        /// </summary>
        public void SaveNode()
        {
            Node nd = new Node(this.FK_Node);
            Work wk = nd.HisWork;
            wk.OID = this.FID;
            if (wk.OID == 0)
                wk.OID = this.OID;
            wk.RetrieveFromDBSources();
            wk = this.UCEn1.Copy(wk) as Work;
            try
            {
                wk.BeforeSave(); //调用业务逻辑检查。
            }
            catch (Exception ex)
            {
                if (BP.Sys.SystemConfig.IsDebug)
                    wk.CheckPhysicsTable();
                throw new Exception("@在保存前执行逻辑检查错误。@技术信息:" + ex.Message);
            }


            wk.Rec = WebUser.No;
            wk.SetValByKey("FK_Dept", WebUser.FK_Dept);
            wk.SetValByKey("FK_NY", BP.DA.DataType.CurrentYearMonth);
            FrmEvents fes = nd.MapData.FrmEvents;
            fes.DoEventNode(FrmEventList.SaveBefore, wk);
            try
            {
                wk.Update();
                fes.DoEventNode(FrmEventList.SaveAfter, wk);
            }
            catch (Exception ex)
            {
                try
                {
                    wk.CheckPhysicsTable();
                }
                catch (Exception ex1)
                {
                    throw new Exception("@保存错误:" + ex.Message + "@检查物理表错误：" + ex1.Message);
                }

                this.UCEn1.AlertMsg_Warning("错误", ex.Message + "@有可能此错误被系统自动修复,请您从新保存一次.");
                return;
            }
            // this.Response.Redirect("Frm.aspx?OID=" + wk.GetValStringByKey("OID") + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.OID + "&FID=" + this.FID + "&FK_MapData=" + this.FK_MapData, true);
            return;
        }
        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.FK_MapData.Replace("ND", "") == this.FK_Node.ToString())
                {
                    this.SaveNode();
                    return;
                }

                MapData md = new MapData(this.FK_MapData);
                GEEntity en = md.HisGEEn;
                en.SetValByKey("OID", this.OIDPKVal);
                int i = en.RetrieveFromDBSources();
                en = this.UCEn1.Copy(en) as GEEntity;
                FrmEvents fes = md.FrmEvents;
                //new FrmEvents(this.FK_MapData);
                fes.DoEventNode(FrmEventList.SaveBefore, en);

                #region 检查是否有ca签名.
                if (md.IsHaveCA == true)
                    en.SetValByKey("SealData", this.TB_SealData.Text);
                #endregion 检查是否有ca签名.

                if (i == 0)
                    en.Insert();
                else
                    en.Update();
                fes.DoEventNode(FrmEventList.SaveAfter, en);

                //this.Response.Redirect("Frm.aspx?OID=" + en.GetValStringByKey("OID") + "&FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&FK_MapData=" + this.FK_MapData, true);
            }
            catch (Exception ex)
            {
                this.UCEn1.AddMsgOfWarning("error:", ex.Message);
            }
        }
    }
}