using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using BP.Web;
using BP.En;
using BP.DA;
using BP.Sys;
using BP.Web.Controls;

namespace BP.Web.Comm.UI
{
    public partial class UIEnDtl : WebPage
    {
        #region ����
        public new string EnsName
        {
            get
            {
                string str = this.Request.QueryString["EnsName"];
                if (str == null)
                    return "BP.Edu.BTypes";
                return str;
            }
        }
        public string MainEnsName
        {
            get
            {
                return this.Request.QueryString["MainEnsName"];
            }
        }
        public string RefKey
        {
            get
            {
                return this.Request.QueryString["RefKey"];
            }
        }
        public string RefVal
        {
            get
            {
                string s= this.Request.QueryString["RefVal"];
                if (s != null)
                    return s;

                s = this.Request.QueryString["PK"];
                if (s != null)
                    return s;


                s = this.Request.QueryString["No"];
                if (s != null)
                    return s;

                s = this.Request.QueryString["OID"];
                if (s != null)
                    return s;

                s = this.Request.QueryString["MyPK"];
                if (s != null)
                    return s;

                return s;
            }
        }
        public new Entity HisEn
        {
            get
            {
                return this.HisEns.GetNewEntity;
            }
        }
        public new Entities HisEns
        {
            get
            {
                Entities ens = ClassFactory.GetEns(this.EnsName);
                return ens;
            }
        }
        public int PageSize
        {
            get
            {
                return 12;
            }
        }
        #endregion ����

        protected void Page_Load(object sender, EventArgs e)
        {
            //if (this.IsPostBack == false)
            //{
            this.ToolBar1.AddBtn(NamesOfBtn.Save);
           // this.ToolBar1.AddBtn(NamesOfBtn.SaveAndClose);
            //this.ToolBar1.AddBtn(NamesOfBtn.Close);
            this.ToolBar1.AddBtn(NamesOfBtn.Delete);
            //   this.ToolBar1.AddBtn(NamesOfBtn.Excel_S);
            this.ToolBar1.AddBtn(NamesOfBtn.Excel, "����Excel");
            //   this.ToolBar1.AddBtn(NamesOfBtn.Close);
            // }

            if (this.ToolBar1.IsExit(NamesOfBtn.Save))
                this.ToolBar1.GetBtnByID(NamesOfBtn.Save).Click += new EventHandler(ToolBar1_ButtonClick);

            if (this.ToolBar1.IsExit(NamesOfBtn.SaveAndClose))
                this.ToolBar1.GetBtnByID(NamesOfBtn.SaveAndClose).Click += new EventHandler(ToolBar1_ButtonClick);

            if (this.ToolBar1.IsExit(NamesOfBtn.Delete))
                this.ToolBar1.GetBtnByID(NamesOfBtn.Delete).Click += new EventHandler(ToolBar1_ButtonClick);

            if (this.ToolBar1.IsExit(NamesOfBtn.Excel))
                this.ToolBar1.GetBtnByID(NamesOfBtn.Excel).Click += new EventHandler(ToolBar1_ButtonClick);

            this.Bind();
        }

        public void Bind()
        {
            #region ���ɱ���
            Entity en = this.HisEn;
            Map map = this.HisEn.EnMap;
            Attrs attrs = map.Attrs;

            this.ucsys1.AddTable();
            this.ucsys1.AddTR();
            this.ucsys1.AddTDTitle();

            string str1 = "<INPUT id='checkedAll' onclick='SelectAll(this);' type='checkbox' name='checkedAll'>";
            this.ucsys1.AddTDTitle(str1);
            foreach (Attr attr in attrs)
            {
                if (attr.UIVisible == false)
                    continue;

                this.ucsys1.AddTDTitle(attr.Desc);
            }
            this.ucsys1.AddTDTitle();
            this.ucsys1.AddTREnd();
            #endregion ���ɱ���

            this.Title = en.EnDesc;

            Entities dtls = this.HisEns;
            QueryObject qo = new QueryObject(dtls);
            qo.AddWhere(this.RefKey, this.RefVal);

            #region ���ɷ�ҳ
            this.ucsys2.Clear();
            try
            {
                this.ucsys2.BindPageIdx(qo.GetCount(), BP.Sys.SystemConfig.PageSize, this.PageIdx, "UIEnDtl.aspx?EnsName=" + this.EnsName + "&RefVal=" + this.RefVal + "&RefKey=" + this.RefKey + "&MainEnsName=" + this.MainEnsName);
                qo.DoQuery(en.PK, this.PageSize , this.PageIdx, false);
            }
            catch
            {
                dtls.GetNewEntity.CheckPhysicsTable();
                //   this.Response.Redirect("Ens.aspx?EnsName=" + this.EnsName + "&RefPKVal=" + this.RefPKVal, true);
                return;
            }
            #endregion ���ɷ�ҳ

            en.PKVal = "0";
            dtls.AddEntity(en);

            DDL ddl = new DDL();
            CheckBox cb = new CheckBox();
            bool is1 = false;

            bool isFJ = false;
            if (attrs.Contains("MyFileName"))
                isFJ = true;

            #region ��������
            int i = 0;
            foreach (Entity dtl in dtls)
            {
                i++;
                if (dtl.PKVal == "0" || dtl.PKVal == "")
                {
                    this.ucsys1.AddTRSum();
                    this.ucsys1.AddTD("colspan=2", "<b>*</B>");
                }
                else
                {
                    is1 = this.ucsys1.AddTR(is1, "ondblclick=\"WinOpen( 'UIEn.aspx?EnsName=" + this.EnsName + "&PK=" + dtl.PKVal + "', 'cd' )\"");
                    //  is1 = this.ucsys1.AddTR(is1);
                    this.ucsys1.AddTDIdx(i);
                    cb = new CheckBox();
                    cb.ID = "CB_" + dtl.PKVal;
                    this.ucsys1.AddTD(cb);
                }

                foreach (Attr attr in attrs)
                {
                    if (attr.UIVisible == false)
                        continue;

                    if (attr.Key == "OID")
                        continue;

                    string val = dtl.GetValByKey(attr.Key).ToString();
                    switch (attr.UIContralType)
                    {
                        case UIContralType.TB:
                            TB tb = new TB();
                            this.ucsys1.AddTD(tb);
                            tb.LoadMapAttr(attr);
                            tb.ID = "TB_" + attr.Key + "_" + dtl.PKVal;
                            tb.Attributes["style"] = "width:" + attr.UIWidth + "px;border-width:0px;";
                            switch (attr.MyDataType)
                            {
                                case DataType.AppMoney:
                                case DataType.AppRate:
                                    tb.TextExtMoney = decimal.Parse(val);
                                    break;
                                default:
                                    tb.Text = val;
                                    break;
                            }

                            if (attr.IsNum && attr.IsFKorEnum == false)
                            {
                                if (tb.Enabled)
                                {
                                    // OnKeyPress="javascript:return VirtyNum(this);"
                                    //  tb.Attributes["OnKeyDown"] = "javascript:return VirtyNum(this);";
                                    // tb.Attributes["onkeyup"] += "javascript:C" + dtl.PKVal + "();C" + attr.Key + "();";
                                    tb.Attributes["class"] = "TBNum";
                                }
                                else
                                {
                                    //   tb.Attributes["onpropertychange"] += "C" + attr.Key + "();";
                                    tb.Attributes["class"] = "TBNumReadonly";
                                }
                            }
                            break;
                        case UIContralType.DDL:
                            ddl = new DDL();
                            ddl.ID = "DDL_" + attr.Key + "_" + dtl.PKVal;

                            if (attr.IsEnum)
                                ddl.BindSysEnum(attr.UIBindKey);
                            else
                            {
                                ddl.BindEntities(attr.HisFKEns, attr.UIRefKeyValue, attr.UIRefKeyText);
                            }

                            this.ucsys1.AddTD(ddl);
                            ddl.SetSelectItem(val);
                            break;
                        case UIContralType.CheckBok:
                            cb = new CheckBox();
                            cb.ID = "CB_" + attr.Key + "_" + dtl.PKVal;
                             cb.Text = attr.Desc;
                            if (val == "1")
                                cb.Checked = true;
                            else
                                cb.Checked = false;
                            this.ucsys1.AddTD("nowarp=true",cb);
                            break;
                        default:
                            break;
                    }
                }

                if (isFJ)
                {
                    string ext = dtl.GetValStrByKey("MyFileExt");
                    if (ext == "")
                        this.ucsys1.AddTD();
                    else
                        this.ucsys1.AddTD("<img src='../Images/FileType/" + ext + ".gif' border=0/>" + dtl.GetValStrByKey("MyFileName"));
                }
                else
                {
                    this.ucsys1.AddTD();
                }
                this.ucsys1.AddTREnd();
            }

            #region ���ɺϼ�
            //this.ucsys1.AddTRSum();
            //this.ucsys1.AddTD("colspan=2", "�ϼ�");
            //foreach (Attr attr in attrs)
            //{
            //    if (attr.UIVisible == false)
            //        continue;
            //    if (attr.IsNum && attr.IsFKorEnum == false)
            //    {
            //        TB tb = new TB();
            //        tb.ID = "TB_" + attr.Key;
            //        tb.Text = attr.DefaultVal.ToString();
            //        tb.ShowType = attr.HisTBType;
            //        tb.ReadOnly = true;
            //        tb.Font.Bold = true;
            //        tb.BackColor = System.Drawing.Color.FromName("infobackground");

            //        switch (attr.MyDataType)
            //        {
            //            case DataType.AppRate:
            //            case DataType.AppMoney:
            //                tb.TextExtMoney = dtls.GetSumDecimalByKey(attr.Key);
            //                break;
            //            case DataType.AppInt:
            //                tb.TextExtInt = dtls.GetSumIntByKey(attr.Key);
            //                break;
            //            case DataType.AppFloat:
            //                tb.TextExtFloat = dtls.GetSumFloatByKey(attr.Key);
            //                break;
            //            default:
            //                break;
            //        }
            //        this.ucsys1.AddTD(tb);
            //    }
            //    else
            //    {
            //        this.ucsys1.AddTD();
            //    }
            //}
            //this.ucsys1.AddTD();
            //this.ucsys1.AddTREnd();
            #endregion ���ɺϼ�

            #endregion ��������
            this.ucsys1.AddTableEnd();
        }
        public void Save(bool isclose)
        {
            Entities dtls = ClassFactory.GetEns(this.EnsName);
            Entity en = dtls.GetNewEntity;
            QueryObject qo = new QueryObject(dtls);
            qo.AddWhere(this.RefKey, this.RefVal);
            int num = qo.DoQuery(en.PK, this.PageSize, this.PageIdx, false);

            // qo.DoQuery(en.PK, 12, this.PageIdx, false);
            Map map = dtls.GetNewEntity.EnMap;
            foreach (Entity dtl in dtls)
            {
                this.ucsys1.Copy(dtl, dtl.PKVal.ToString(), map);
                dtl.SetValByKey(this.RefKey, this.RefVal);
                dtl.Update();
            }

            // BP.Sys.MapDtl
            en = this.ucsys1.Copy(en, "0", map);
            en.PKVal = "";
            bool isInsert = false;
            if (en.IsBlank == false)
            {
                if (en.IsNoEntity)
                {
                    if (en.EnMap.IsAutoGenerNo)
                        en.SetValByKey("No", en.GenerNewNoByKey("No"));
                }

                en.SetValByKey(this.RefKey, this.RefVal);
                en.Insert();
                isInsert = true;
            }

            if (isclose)
            {
                this.WinClose();
                return;
            }

            int pageIdx = this.PageIdx;
            if (isInsert == true && num == this.PageSize)
                pageIdx++;

            this.Response.Redirect("UIEnDtl.aspx?EnsName=" + this.EnsName + "&RefVal=" + this.RefVal + "&RefKey=" + this.RefKey + "&PageIdx=" + pageIdx + "&MainEnsName=" + this.MainEnsName, true);
        }
        public void ExpExcel()
        {
            //Entity mdtl = this.HisEn;
            //this.GenerLabel(this.Label1, mdtl.EnDesc);
            //this.Title = mdtl.Name;

            //GEDtls dtls = this.HisEns;
            //QueryObject qo = null;
            //qo = new QueryObject(dtls);
            //qo.DoQuery();

            ////DataTable dt = dtls.ToDataTableDesc();
            //// this.GenerExcel(dtls.ToDataTableDesc(), mdtl.Name + ".xls");
            //this.GenerExcel_pri_Text(dtls.ToDataTableDesc(), mdtl.Name + "@" + WebUser.No + "@" + DataType.CurrentData + ".xls");

            //this.ExportDGToExcelV2(dtls, this.Title + ".xls");
            //dtls.GetNewEntity.CheckPhysicsTable();
            //this.Response.Redirect("Ens.aspx?EnsName=" + this.EnsName + "&RefPKVal=" + this.RefPKVal, true);
        }
        void ToolBar1_ButtonClick(object sender, EventArgs e)
        {
            Btn btn = sender as Btn;
            switch (btn.ID)
            {
                case NamesOfBtn.New:
                case NamesOfBtn.Save:
                case NamesOfBtn.SaveAndNew:
                    this.Save(false);
                    break;
                case NamesOfBtn.SaveAndClose:
                    this.Save(true);
                    break;
                case NamesOfBtn.Delete:
                    Entities dtls = ClassFactory.GetEns(this.EnsName);
                    QueryObject qo = new QueryObject(dtls);
                    qo.AddWhere(this.RefKey, this.RefVal);
                    qo.DoQuery("OID", BP.Sys.SystemConfig.PageSize, this.PageIdx, false);
                    foreach (Entity dtl in dtls)
                    {
                        CheckBox cb = this.ucsys1.GetCBByID("CB_" + dtl.PKVal);
                        if (cb == null)
                            continue;

                        if (cb.Checked)
                            dtl.Delete();
                    }
                    this.ucsys1.Clear();
                    this.Bind();
                    break;
                case NamesOfBtn.Excel:
                    this.ExpExcel();
                    break;
                default:
                    BP.Sys.PubClass.Alert("��ǰ�汾��֧�ִ˹��ܡ�");
                    break;
            }
        }
        /// <summary>
        /// �����еļ���
        /// </summary>
        /// <param name="pk"></param>
        /// <param name="attrs"></param>
        /// <param name="attr"></param>
        /// <returns></returns>
        public string GenerAutoFull(string pk, MapAttrs attrs, MapAttr attr)
        {
            return "";
            //string left = "\n  document.forms[0]." + this.ucsys1.GetTBByID("TB_" + attr.Key + "_" + pk).ClientID + ".value = ";
            //string right = attr.AutoFullDoc;
            //foreach (MapAttr mattr in attrs)
            //{
            //    string tbID = "TB_" + mattr.Key + "_" + pk;
            //    TB tb = this.ucsys1.GetTBByID(tbID);
            //    if (tb == null)
            //        continue;
            //    right = right.Replace("@" + mattr.Name, " parseFloat( document.forms[0]." + this.ucsys1.GetTBByID(tbID).ClientID + ".value.replace( ',' ,  '' ) ) ");
            //    right = right.Replace("@" + mattr.Key, " parseFloat( document.forms[0]." + this.ucsys1.GetTBByID(tbID).ClientID + ".value.replace( ',' ,  '' ) ) ");
            //}

            //string s = left + right;
            //s += "\t\n  document.forms[0]." + this.ucsys1.GetTBByID("TB_" + attr.Key + "_" + pk).ClientID + ".value= VirtyMoney(document.forms[0]." + this.ucsys1.GetTBByID("TB_" + attr.Key + "_" + pk).ClientID + ".value ) ;";
            //return s += " C" + attr.Key + "();";
        }
        public string GenerSum(MapAttr mattr, GEDtls dtls)
        {
            return "";
            //string left = "\n  document.forms[0]." + this.ucsys1.GetTBByID("TB_" + mattr.Key).ClientID + ".value = ";
            //string right = "";
            //int i = 0;
            //foreach (GEDtl dtl in dtls)
            //{
            //    string tbID = "TB_" + mattr.Key + "_" + dtl.PKVal;
            //    TB tb = this.ucsys1.GetTBByID(tbID);
            //    if (i == 0)
            //        right += " parseFloat( document.forms[0]." + tb.ClientID + ".value.replace( ',' ,  '' ) )  ";
            //    else
            //        right += " +parseFloat( document.forms[0]." + tb.ClientID + ".value.replace( ',' ,  '' ) )  ";

            //    i++;
            //}

            //string s = left + right + " ;";
            //switch (mattr.MyDataType)
            //{
            //    case BP.DA.DataType.AppMoney:
            //    case BP.DA.DataType.AppRate:
            //        return s += "\t\n  document.forms[0]." + this.ucsys1.GetTBByID("TB_" + mattr.Key).ClientID + ".value= VirtyMoney(document.forms[0]." + this.ucsys1.GetTBByID("TB_" + mattr.Key).ClientID + ".value ) ;";
            //    default:
            //        return s;
            //}
        }

    }
}