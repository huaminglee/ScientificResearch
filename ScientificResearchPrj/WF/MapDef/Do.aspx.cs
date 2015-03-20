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
using BP.Sys;
using BP.DA;
using BP.En;
using BP.Web;
using BP.Web.UC;
namespace CCFlow.WF.MapDef
{
    public partial class Comm_MapDef_Do : BP.Web.WebPage
    {
        public new string DoType
        {
            get
            {
                return this.Request.QueryString["DoType"];
            }
        }
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
        public string IDX
        {
            get
            {
                return this.Request.QueryString["IDX"];
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                switch (this.DoType)
                {
                    case "DownTempFrm":
                        MapData md = new MapData(this.FK_MapData);
                        DataSet ds = md.GenerHisDataSet();
                        string name = "ccflow表单模板." + md.Name + "." + md.No + ".xml";
                        string file = this.Request.PhysicalApplicationPath + "\\Temp\\" + this.FK_MapData + ".xml";
                        ds.WriteXml(file);
                        this.Response.Redirect("../../Temp/" + this.FK_MapData + ".xml", true);
                        this.WinClose();
                        break;
                    case "CCForm":
                        this.Application.Clear();
                        if (WebUser.NoOfRel != "admin")
                        {
                            BP.Port.Emp emp = new BP.Port.Emp("admin");
                            BP.Web.WebUser.SignInOfGener(emp);
                        }

                        MapAttr mattr = new MapAttr();
                        mattr.MyPK = this.Request.QueryString["MyPK"];
                        int i = mattr.RetrieveFromDBSources();
                        mattr.KeyOfEn = this.Request.QueryString["KeyOfEn"];
                        mattr.FK_MapData = this.Request.QueryString["FK_MapData"];
                        mattr.MyDataType = int.Parse(this.Request.QueryString["DataType"]);

                        if (!string.IsNullOrEmpty(this.Request.QueryString["UIBindKey"] + ""))
                            mattr.UIBindKey = this.Request.QueryString["UIBindKey"];
                        mattr.UIContralType = (UIContralType)int.Parse(this.Request.QueryString["UIContralType"]);
                        mattr.LGType = (BP.En.FieldTypeS)int.Parse(this.Request.QueryString["LGType"]);
                        if (i == 0)
                        {
                            mattr.Name = System.Web.HttpUtility.UrlDecode(this.Request.QueryString["KeyName"],System.Text.Encoding.GetEncoding("GB2312"));
                            mattr.UIIsEnable = true;
                            mattr.UIVisible = true;
                            if (mattr.LGType == FieldTypeS.Enum)
                                mattr.DefVal = "0";
                            mattr.Insert();
                        }
                        else
                        {
                            mattr.Update();
                        }

                        switch (mattr.LGType)
                        {
                            case BP.En.FieldTypeS.Enum:
                                this.Response.Redirect("EditEnum.aspx?MyPK=" + mattr.FK_MapData + "&RefNo=" + mattr.MyPK, true);
                                return;
                            case BP.En.FieldTypeS.Normal:
                                this.Response.Redirect("EditF.aspx?DoType=Edit&MyPK=" + mattr.FK_MapData + "&RefNo=" + mattr.MyPK + "&FType=" + mattr.MyDataType + "&GroupField=0", true);
                                return;
                            case BP.En.FieldTypeS.FK:
                                this.Response.Redirect("EditTable.aspx?DoType=Edit&MyPK=" + mattr.FK_MapData + "&RefNo=" + mattr.MyPK + "&FType=" + mattr.MyDataType + "&GroupField=0", true);
                                return;
                            default:
                                break;
                        }
                        break;
                    case "DobackToF":
                        MapAttr ma = new MapAttr(this.RefNo);
                        switch (ma.LGType)
                        {
                            case FieldTypeS.Normal:
                                this.Response.Redirect("EditF.aspx?RefNo=" + this.RefNo, true);
                                return;
                            case FieldTypeS.FK:
                                this.Response.Redirect("EditTable.aspx?RefNo=" + this.RefNo, true);
                                return;
                            case FieldTypeS.Enum:
                                this.Response.Redirect("EditEnum.aspx?RefNo=" + this.RefNo, true);
                                return;
                            default:
                                return;
                        }

                        break;
                    case "AddEnum":
                        SysEnumMain sem1 = new SysEnumMain(this.Request.QueryString["EnumKey"]);
                        MapAttr attrAdd = new MapAttr();
                        attrAdd.KeyOfEn = sem1.No;
                        if (attrAdd.IsExit(MapAttrAttr.FK_MapData, this.MyPK, MapAttrAttr.KeyOfEn, sem1.No))
                        {
                            BP.Sys.PubClass.Alert("字段已经存在 [" + sem1.No + "]。");
                            BP.Sys.PubClass.WinClose();
                            return;
                        }

                        attrAdd.FK_MapData = this.MyPK;
                        attrAdd.Name = sem1.Name;
                        attrAdd.UIContralType = UIContralType.DDL;
                        attrAdd.UIBindKey = sem1.No;
                        attrAdd.MyDataType = BP.DA.DataType.AppInt;
                        attrAdd.LGType = FieldTypeS.Enum;
                        attrAdd.DefVal = "0";
                        attrAdd.UIIsEnable = true;
                        if (this.IDX == null || this.IDX == "")
                        {
                            MapAttrs attrs1 = new MapAttrs(this.MyPK);
                            attrAdd.IDX = 0;
                        }
                        else
                        {
                            attrAdd.IDX = int.Parse(this.IDX);
                        }
                        attrAdd.Insert();
                        this.Response.Redirect("EditEnum.aspx?MyPK=" + this.MyPK + "&RefNo=" + attrAdd.MyPK, true);
                        this.WinClose();
                        return;
                    case "DelEnum":
                        string eKey = this.Request.QueryString["EnumKey"];
                        SysEnumMain sem = new SysEnumMain();
                        sem.No = eKey;
                        sem.Delete();
                        this.WinClose();
                        return;

                    case "AddSysEnum":
                        this.AddFEnum();
                        break;
                    case "AddSFTable":
                        this.AddSFTable();
                        break;
                    case "AddSFTableAttr":
                        SFTable sf = new SFTable(this.Request.QueryString["RefNo"]);
                        this.Response.Redirect("EditTable.aspx?MyPK=" + this.MyPK + "&SFKey=" + sf.No, true);
                        this.WinClose();
                        return;
                    case "AddFG": /*执行一个插入列组的命令.*/
                        switch (this.RefNo)
                        {
                            case "IsPass":
                                MapDtl dtl = new MapDtl(this.FK_MapData);
                                dtl.IsEnablePass = true; /*更新是否启动审核分组字段.*/
                                MapAttr attr = new MapAttr();
                                attr.FK_MapData = this.FK_MapData;
                                attr.KeyOfEn = "Check_Note";
                                attr.Name = "审核意见";
                                attr.MyDataType = DataType.AppString;
                                attr.UIContralType = UIContralType.TB;
                                attr.UIIsEnable = true;
                                attr.UIIsLine = true;
                                attr.MaxLen = 4000;
                                attr.ColSpan = 4; // 默认为4列。
                                attr.IDX = 1;
                                attr.Insert();

                                attr = new MapAttr();
                                attr.FK_MapData = this.FK_MapData;
                                attr.KeyOfEn = "Checker";
                                attr.Name = "审核人";// "审核人";
                                attr.MyDataType = DataType.AppString;
                                attr.UIContralType = UIContralType.TB;
                                attr.MaxLen = 50;
                                attr.MinLen = 0;
                                attr.UIIsEnable = true;
                                attr.UIIsLine = false;
                                attr.DefVal = "@WebUser.Name";
                                attr.UIIsEnable = false;
                                attr.IsSigan = true;
                                attr.IDX = 2;
                                attr.Insert();

                                attr = new MapAttr();
                                attr.FK_MapData = this.FK_MapData;
                                attr.KeyOfEn = "IsPass";
                                attr.Name = "通过否?";// "审核人";
                                attr.MyDataType = DataType.AppBoolean;
                                attr.UIContralType = UIContralType.CheckBok;
                                attr.UIIsEnable = true;
                                attr.UIIsLine = false;
                                attr.UIIsEnable = false;
                                attr.IsSigan = true;
                                attr.DefVal = "1";
                                attr.IDX = 2;
                                attr.DefVal = "0";
                                attr.Insert();

                                attr = new MapAttr();
                                attr.FK_MapData = this.FK_MapData;
                                attr.KeyOfEn = "Check_RDT";
                                attr.Name = "审核日期"; // "审核日期";
                                attr.MyDataType = DataType.AppDateTime;
                                attr.UIContralType = UIContralType.TB;
                                attr.UIIsEnable = true;
                                attr.UIIsLine = false;
                                attr.DefVal = "@RDT";
                                attr.UIIsEnable = false;
                                attr.IDX = 3;
                                attr.Insert();

                                /* 处理批次ID*/
                                attr = new MapAttr();
                                attr.FK_MapData = this.FK_MapData;
                                attr.KeyOfEn = "BatchID";
                                attr.Name = "BatchID";// this.ToE("IsPass", "是否通过");// "审核人";
                                attr.MyDataType = DataType.AppInt;
                                attr.UIIsEnable = false;
                                attr.UIIsLine = false;
                                attr.UIIsEnable = false;
                                attr.UIVisible = false;
                                attr.IDX = 2;
                                attr.DefVal = "0";
                                attr.Insert();

                                dtl.Update();
                                this.WinClose();
                                return;
                            case "Eval": /* 质量评价 */
                                attr = new MapAttr();
                                attr.FK_MapData = this.FK_MapData;
                                attr.KeyOfEn = "EvalEmpNo";
                                attr.Name = "被评价人员编号";
                                attr.MyDataType = DataType.AppString;
                                attr.UIContralType = UIContralType.TB;
                                attr.MaxLen = 50;
                                attr.MinLen = 0;
                                attr.UIIsEnable = true;
                                attr.UIIsLine = false;
                                attr.UIIsEnable = false;
                                attr.IsSigan = true;
                                attr.IDX = 1;
                                attr.Insert();

                                attr = new MapAttr();
                                attr.FK_MapData = this.FK_MapData;
                                attr.KeyOfEn = "EvalEmpName";
                                attr.Name = "被评价人员名称";
                                attr.MyDataType = DataType.AppString;
                                attr.UIContralType = UIContralType.TB;
                                attr.MaxLen = 50;
                                attr.MinLen = 0;
                                attr.UIIsEnable = true;
                                attr.UIIsLine = false;
                                attr.UIIsEnable = false;
                                attr.IsSigan = true;
                                attr.IDX = 2;
                                attr.Insert();

                                attr = new MapAttr();
                                attr.FK_MapData = this.FK_MapData;
                                attr.KeyOfEn = "EvalCent";
                                attr.Name = "工作得分";
                                attr.MyDataType = DataType.AppFloat;
                                attr.UIContralType = UIContralType.TB;
                                attr.MaxLen = 50;
                                attr.MinLen = 0;
                                attr.UIIsEnable = true;
                                attr.UIIsLine = false;
                                attr.UIIsEnable = true;
                                attr.IDX = 3;
                                attr.Insert();

                                attr = new MapAttr();
                                attr.FK_MapData = this.FK_MapData;
                                attr.KeyOfEn = "EvalNote";
                                attr.Name = "评价信息";
                                attr.MyDataType = DataType.AppString;
                                attr.UIContralType = UIContralType.TB;
                                attr.MaxLen = 50;
                                attr.MinLen = 0;
                                attr.UIIsEnable = true;
                                attr.UIIsEnable = true;
                                attr.IDX = 4;
                                attr.Insert();
                                this.WinClose();
                                return;
                            default:
                                break;
                        }
                        break;
                    case "AddFGroup":
                        this.AddFGroup();
                        return;
                    case "AddF":
                    case "ChoseFType":
                        this.AddF();
                        break;
                    case "Up":
                        MapAttr attrU = new MapAttr(this.RefNo);
                        if (this.Request.QueryString["IsDtl"] != null)
                            attrU.DoDtlUp();
                        else
                            attrU.DoUp();

                        this.WinClose();
                        break;
                    case "Down":
                        MapAttr attrD = new MapAttr(this.RefNo);
                        if (this.Request.QueryString["IsDtl"] == null)
                            attrD.DoDown();
                        else
                            attrD.DoDtlDown();
                        this.WinClose();
                        break;
                    case "Jump":
                        MapAttr attrFrom = new MapAttr(this.Request.QueryString["FromID"]);
                        MapAttr attrTo = new MapAttr(this.Request.QueryString["ToID"]);
                        attrFrom.DoJump(attrTo);
                        this.WinClose();
                        break;
                    case "MoveTo":
                        string toID = this.Request.QueryString["ToID"];
                        int toGFID = int.Parse(this.Request.QueryString["ToGID"]);
                        int fromGID = int.Parse(this.Request.QueryString["FromGID"]);
                        string fromID = this.Request.QueryString["FromID"];
                        MapAttr fromAttr = new MapAttr();
                        fromAttr.MyPK = fromID;
                        fromAttr.Retrieve();
                        if (toGFID == fromAttr.GroupID && fromAttr.MyPK == toID)
                        {
                            /* 如果没有移动. */
                            this.WinClose();
                            return;
                        }
                        if (toGFID != fromAttr.GroupID && fromAttr.MyPK == toID)
                        {
                            MapAttr toAttr = new MapAttr(toID);
                            fromAttr.Update(MapAttrAttr.GroupID, toAttr.GroupID, MapAttrAttr.IDX, toAttr.IDX);
                            this.WinClose();
                            return;
                        }
                        this.Response.Redirect(this.Request.RawUrl.Replace("MoveTo", "Jump"), true);
                        return;
                    case "Edit":
                        Edit();
                        break;
                    case "Del":
                        MapAttr attrDel = new MapAttr();
                        attrDel.MyPK = this.RefNo;
                        attrDel.Delete();
                        this.WinClose();
                        break;
                    case "GFDoUp":
                        GroupField gf = new GroupField(this.RefOID);
                        gf.DoUp();
                        gf.Retrieve();
                        if (gf.Idx == 0)
                        {
                            this.WinClose();
                            return;
                        }
                        int oidIdx = gf.Idx;
                        gf.Idx = gf.Idx - 1;
                        GroupField gfUp = new GroupField();
                        if (gfUp.Retrieve(GroupFieldAttr.EnName, gf.EnName, GroupFieldAttr.Idx, gf.Idx) == 1)
                        {
                            gfUp.Idx = oidIdx;
                            gfUp.Update();
                        }
                        gf.Update();
                        this.WinClose();
                        break;
                    case "GFDoDown":
                        GroupField mygf = new GroupField(this.RefOID);
                        mygf.DoDown();
                        mygf.Retrieve();
                        int oidIdx1 = mygf.Idx;
                        mygf.Idx = mygf.Idx + 1;
                        GroupField gfDown = new GroupField();
                        if (gfDown.Retrieve(GroupFieldAttr.EnName, mygf.EnName, GroupFieldAttr.Idx, mygf.Idx) == 1)
                        {
                            gfDown.Idx = oidIdx1;
                            gfDown.Update();
                        }
                        mygf.Update();
                        this.WinClose();
                        break;
                    case "AthDoUp":
                        FrmAttachment frmAth = new FrmAttachment(this.MyPK);
                        if (frmAth.RowIdx > 0)
                        {
                            frmAth.RowIdx = frmAth.RowIdx - 1;
                            frmAth.Update();
                        }
                        this.WinClose();
                        break;
                    case "AthDoDown":
                        FrmAttachment frmAthD = new FrmAttachment(this.MyPK);
                        if (frmAthD.RowIdx < 10)
                        {
                            frmAthD.RowIdx = frmAthD.RowIdx + 1;
                            frmAthD.Update();
                        }
                        this.WinClose();
                        break;
                    case "DtlDoUp":
                        MapDtl dtl1 = new MapDtl(this.MyPK);
                        if (dtl1.RowIdx > 0)
                        {
                            dtl1.RowIdx = dtl1.RowIdx - 1;
                            dtl1.Update();
                        }
                        this.WinClose();
                        break;
                    case "DtlDoDown":
                        MapDtl dtl2 = new MapDtl(this.MyPK);
                        if (dtl2.RowIdx < 10)
                        {
                            dtl2.RowIdx = dtl2.RowIdx + 1;
                            dtl2.Update();
                        }
                        this.WinClose();
                        break;
                    case "M2MDoUp":
                        MapM2M ddtl1 = new MapM2M(this.MyPK);
                        if (ddtl1.RowIdx > 0)
                        {
                            ddtl1.RowIdx = ddtl1.RowIdx - 1;
                            ddtl1.Update();
                        }
                        this.WinClose();
                        break;
                    case "M2MDoDown":
                        MapM2M ddtl2 = new MapM2M(this.MyPK);
                        if (ddtl2.RowIdx < 10)
                        {
                            ddtl2.RowIdx = ddtl2.RowIdx + 1;
                            ddtl2.Update();
                        }
                        this.WinClose();
                        break;
                    case "FrameDoUp":
                        MapFrame frame1 = new MapFrame(this.MyPK);
                        if (frame1.RowIdx > 0)
                        {
                            frame1.RowIdx = frame1.RowIdx - 1;
                            frame1.Update();
                        }
                        this.WinClose();
                        break;
                    case "FrameDoDown":
                        MapFrame frame2 = new MapFrame(this.MyPK);
                        if (frame2.RowIdx < 10)
                        {
                            frame2.RowIdx = frame2.RowIdx + 1;
                            frame2.Update();
                        }
                        this.WinClose();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                this.Pub1.AddMsgOfWarning("错误:", ex.Message + " <br>" + this.Request.RawUrl);
            }
        }
        public void Edit()
        {
            MapAttr attr = new MapAttr(this.RefNo);
            switch (attr.MyDataType)
            {
                case BP.DA.DataType.AppString:
                    //  this.Response.Redirect("EditF.aspx?RefOID="+this
                    break;
                default:
                    break;
            }
        }
        public string GroupField
        {
            get
            {
                return this.Request.QueryString["GroupField"];
            }
        }
        public void AddF()
        {
            this.Title = "增加新字段向导";

            this.Pub1.AddFieldSet("新增普通字段");
            this.Pub1.AddUL();
            this.Pub1.AddLi("<a href='EditF.aspx?DoType=Add&MyPK=" + this.MyPK + "&FType=" + BP.DA.DataType.AppString + "&IDX=" + this.IDX + "&GroupField=" + this.GroupField + "'><b>字符型</b></a> - <font color=Note>如:姓名、地址、邮编、电话</font>");
            this.Pub1.AddLi("<a href='EditF.aspx?DoType=Add&MyPK=" + this.MyPK + "&FType=" + BP.DA.DataType.AppInt + "&IDX=" + this.IDX + "&GroupField=" + this.GroupField + "'><b>整数型</b></a> - <font color=Note>如:年龄、个数。</font>");
            this.Pub1.AddLi("<a href='EditF.aspx?DoType=Add&MyPK=" + this.MyPK + "&FType=" + BP.DA.DataType.AppMoney + "&IDX=" + this.IDX + "&GroupField=" + this.GroupField + "'><b>金额型</b></a> - <font color=Note>如:单价、薪水。</font>");
            this.Pub1.AddLi("<a href='EditF.aspx?DoType=Add&MyPK=" + this.MyPK + "&FType=" + BP.DA.DataType.AppFloat + "&IDX=" + this.IDX + "&GroupField=" + this.GroupField + "'><b>浮点型</b></a> - <font color=Note>如：身高、体重、长度。</font>");
            this.Pub1.AddLi("<a href='EditF.aspx?DoType=Add&MyPK=" + this.MyPK + "&FType=" + BP.DA.DataType.AppDate + "&IDX=" + this.IDX + "&GroupField=" + this.GroupField + "'><b>日期型</b></a> - <font color=Note>如：出生日期、发生日期。</font>");
            this.Pub1.AddLi("<a href='EditF.aspx?DoType=Add&MyPK=" + this.MyPK + "&FType=" + BP.DA.DataType.AppDateTime + "&IDX=" + this.IDX + "&GroupField=" + this.GroupField + "'><b>日期时间型</b></a> - <font color=Note>如：发生日期时间</font>");
            this.Pub1.AddLi("<a href='EditF.aspx?DoType=Add&MyPK=" + this.MyPK + "&FType=" + BP.DA.DataType.AppBoolean + "&IDX=" + this.IDX + "&GroupField=" + this.GroupField + "'><b>Boole型(是/否)</b></a> - <font color=Note>如：是否完成、是否达标</font>");
            this.Pub1.AddULEnd();
            this.Pub1.AddFieldSetEnd();

            this.Pub1.AddFieldSet("新增枚举字段(用来表示，状态、类型...的数据。)");
            this.Pub1.AddUL();
            this.Pub1.AddLi("<a href='Do.aspx?DoType=AddSysEnum&MyPK=" + this.MyPK + "&IDX=" + this.IDX + "&GroupField=" + this.GroupField + "'><b>枚举型</b></a> -  比如：性别:男/女。请假类型：事假/病假/婚假/产假/其它。");
            this.Pub1.AddULEnd();
            this.Pub1.AddFieldSetEnd();

            this.Pub1.AddFieldSet("新增外键字段(字典表，通常只有编号名称两个列)");
            this.Pub1.AddUL();
            this.Pub1.AddLi("<a href='Do.aspx?DoType=AddSFTable&MyPK=" + this.MyPK + "&FType=Class&IDX=" + this.IDX + "&GroupField=" + this.GroupField + "'><b>外键型</b></a> -  比如：岗位、税种、行业、经济性质。");
            this.Pub1.AddULEnd();
            this.Pub1.AddFieldSetEnd();

            this.Pub1.AddFieldSet("<div onclick=\"javascript:HidShowSysField();\" >系统约定字段-隐藏/显示</div> ");
            string info = DataType.ReadTextFile2Html(BP.Sys.SystemConfig.PathOfData + "SysFields.txt");
            this.Pub1.Add("<div id='SysField' style='display:none' >" + info + "</div>");
            this.Pub1.AddFieldSetEnd();
        }

        public void AddFEnum()
        {
            this.Title = "增加新字段向导";
            this.Pub1.AddTable();
            this.Pub1.AddCaptionLeft("<a href='Do.aspx?DoType=AddF&MyPK=" + this.MyPK + "&IDX=" + this.IDX + "'>增加新字段向导</a> - <a href='SysEnum.aspx?DoType=New&MyPK=" + this.MyPK + "&IDX=" + this.IDX + "' ><img src='../Img/Btn/New.gif' />新建枚举</a>");
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("IDX");
            this.Pub1.AddTDTitle("编号(点击增加到表单)");
            this.Pub1.AddTDTitle("名称");
            this.Pub1.AddTDTitle("操作");
            this.Pub1.AddTDTitle();
            this.Pub1.AddTREnd();

            BP.Sys.SysEnumMains sems = new SysEnumMains();
            QueryObject qo = new QueryObject(sems);
            this.Pub2.BindPageIdx(qo.GetCount(), pageSize, this.PageIdx, "Do.aspx?DoType=AddSysEnum&MyPK=" + this.MyPK + "&IDX=&GroupField");
            qo.DoQuery("No", pageSize, this.PageIdx);

            bool is1 = false;
            int idx = 0;
            foreach (BP.Sys.SysEnumMain sem in sems)
            {
                BP.Web.Controls.DDL ddl = null;
                try
                {
                    ddl = new BP.Web.Controls.DDL();
                    ddl.BindSysEnum(sem.No);
                }
                catch
                {
                    sem.Delete();
                }
                idx++;
                is1 = this.Pub1.AddTR(is1);
                this.Pub1.AddTDIdx(idx);
                this.Pub1.AddTD("<a  href=\"javascript:AddEnum('" + this.MyPK + "','" + this.IDX + "','" + sem.No + "')\" >" + sem.No + "</a>");
                this.Pub1.AddTD(sem.Name);
                this.Pub1.AddTD("[<a href='SysEnum.aspx?DoType=Edit&MyPK=" + this.MyPK + "&IDX=" + this.IDX + "&RefNo=" + sem.No + "' >编辑</a>]");
                this.Pub1.AddTD(ddl);
                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTableEnd();
        }
        /// <summary>
        /// 增加分组.
        /// </summary>
        public void AddFGroup()
        {
            this.Pub1.AddFieldSet("插入列组");

            this.Pub1.AddUL();
            BP.Sys.FieldGroupXmls xmls = new FieldGroupXmls();
            xmls.RetrieveAll();
            foreach (FieldGroupXml en in xmls)
            {
                this.Pub1.AddLi("<a href='Do.aspx?DoType=AddFG&RefNo=" + en.No + "&FK_MapData=" + this.FK_MapData + "' >" + en.Name + "</a><br>" + en.Desc);
            }
            this.Pub1.AddULEnd();
            this.Pub1.AddFieldSetEnd();
        }
        int pageSize = 10;
        public void AddSFTable()
        {
            this.Title = "增加新字段向导";

            this.Pub1.AddTable();
            this.Pub1.AddCaption("<a href='Do.aspx?DoType=AddF&MyPK=" + this.MyPK + "&IDX=" + this.IDX + "'>增加新字段向导</a> - 增加外键字段 - <a href='SFTable.aspx?DoType=New&MyPK=" + this.MyPK + "&IDX=" + this.IDX + "' > 新建表</a>");
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("IDX");
            this.Pub1.AddTDTitle("编号(点击增加到表单)");
            this.Pub1.AddTDTitle("名称");
            this.Pub1.AddTDTitle("类别");
            this.Pub1.AddTDTitle("描述/编辑");
            this.Pub1.AddTDTitle("编辑数据");
            this.Pub1.AddTREnd();

            BP.Sys.SFTables ens = new SFTables();
            QueryObject qo = new QueryObject(ens);
            this.Pub2.BindPageIdx(qo.GetCount(), pageSize, this.PageIdx,
                "Do.aspx?DoType=AddSFTable&MyPK=" + this.MyPK + "&IDX=&GroupField");
            qo.DoQuery("No", pageSize, this.PageIdx);

            bool is1 = false;
            int idx = 0;
            foreach (BP.Sys.SFTable sem in ens)
            {
                idx++;
                //is1 = this.Pub1.AddTR(is1);
                is1 = this.Pub1.AddTR(is1);
                this.Pub1.AddTDIdx(idx);
                this.Pub1.AddTD("<a  href=\"javascript:AddSFTable('" + this.MyPK + "','" + this.IDX + "','" + sem.No + "')\" >" + sem.No + "</a>");
                this.Pub1.AddTD(sem.Name);

                if (sem.IsClass)
                    this.Pub1.AddTD("<a href=\"javascript:WinOpen('../Comm/Search.aspx?EnsName=" + sem.No + "','sg')\"  ><img src='../Img/Btn/Edit.gif' border=0/>" + sem.TableDesc + "</a>");
                else
                    this.Pub1.AddTD("<a href=\"javascript:WinOpen('SFTable.aspx?DoType=Edit&MyPK=" + this.MyPK + "&IDX=" + this.IDX + "&RefNo=" + sem.No + "','sg')\"  ><img src='../Img/Btn/Edit.gif' border=0/>" + sem.TableDesc + "</a>");

                if (sem.No.Contains("."))
                    this.Pub1.AddTD("&nbsp;");
                else
                    this.Pub1.AddTD("<a href=\"javascript:WinOpen('SFTableEditData.aspx?RefNo=" + sem.No + "');\" >编辑</a>");
                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTableEnd();
        }
    }
}