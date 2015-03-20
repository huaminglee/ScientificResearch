using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.En;
using BP;
using BP.Sys;

public partial class WF_MapDef_UC_MExt : BP.Web.UC.UCBase3
{
    #region 属性。
    public string FK_MapData
    {
        get
        {
            return this.Request.QueryString["FK_MapData"];
        }
    }

    public string OperAttrKey
    {
        get
        {
            return this.Request.QueryString["OperAttrKey"];
        }
    }
    public string ExtType
    {
        get
        {
            string s = this.Request.QueryString["ExtType"];
            if (s == "")
                s = null;
            return s;
        }
    }
    public string Lab = null;
    #endregion 属性。

    /// <summary>
    /// BindLeft
    /// </summary>
    public void BindLeft()
    {
        if (this.ExtType == MapExtXmlList.StartFlow)
            return;

        MapExtXmls fss = new MapExtXmls();
        fss.RetrieveAll();
        this.Left.Add("<a href='http://ccflow.org' target=_blank  ><img src='../../DataUser/ICON/" + SystemConfig.CompanyID + "/LogBiger.png' style='width:180px;' /></a><hr>");
        this.Left.AddUL();
        foreach (MapExtXml fs in fss)
        {
            if (this.ExtType == fs.No)
            {
                this.Lab = fs.Name;
                this.Left.AddLiB(fs.URL + "&FK_MapData=" + this.FK_MapData + "&ExtType=" + fs.No + "&RefNo=" + this.RefNo, "<span>" + fs.Name + "</span>");
            }
            else
                this.Left.AddLi(fs.URL + "&FK_MapData=" + this.FK_MapData + "&ExtType=" + fs.No + "&RefNo=" + this.RefNo, "<span>" + fs.Name + "</span>");
        }


        this.Left.AddLi("<a href='MapExt.aspx?FK_MapData=" + this.FK_MapData + "&RefNo=" + this.RefNo + "'><span>帮助</span></a>");
        this.Left.AddULEnd();
    }
    public void BindLeftV1()
    {
        this.Left.Add("\t\n<div id='tabsJ'  align='center'>");
        MapExtXmls fss = new MapExtXmls();
        fss.RetrieveAll();

        this.Left.AddUL();
        foreach (MapExtXml fs in fss)
        {
            if (this.ExtType == fs.No)
            {
                this.Lab = fs.Name;
                this.Left.AddLiB(fs.URL + "&FK_MapData=" + this.FK_MapData + "&ExtType=" + fs.No + "&RefNo=" + this.RefNo, "<span>" + fs.Name + "</span>");
            }
            else
                this.Left.AddLi(fs.URL + "&FK_MapData=" + this.FK_MapData + "&ExtType=" + fs.No + "&RefNo=" + this.RefNo, "<span>" + fs.Name + "</span>");
        }
        this.Left.AddLi("<a href='MapExt.aspx?FK_MapData=" + this.FK_MapData + "&RefNo=" + this.RefNo + "'><span>帮助</span></a>");
        this.Left.AddULEnd();
        this.Left.AddDivEnd();
    }
    /// <summary>
    /// 新建文本框自动完成
    /// </summary>
    public void EditAutoFullM2M_TB()
    {
        MapExt myme = new MapExt(this.MyPK);
        MapM2Ms m2ms = new MapM2Ms(myme.FK_MapData);

        this.Pub2.AddH2("设置自动填充从表. <a href='?ExtType=" + this.ExtType + "&MyPK=" + this.MyPK + "&FK_MapData=" + this.FK_MapData + "&RefNo=" + this.RefNo + "'>返回</a>");
        if (m2ms.Count == 0)
        {
            this.Pub2.Clear();
            this.Pub2.AddFieldSet("设置自动填充从表. <a href='?ExtType=" + this.ExtType + "&MyPK=" + this.MyPK + "&FK_MapData=" + this.FK_MapData + "&RefNo=" + this.RefNo + "'>返回</a>");
            this.Pub2.Add("该表单下没有从表，所以您不能为从表设置自动填充。");
            this.Pub2.AddFieldSetEnd();
            return;
        }
        string[] strs = myme.Tag2.Split('$');
        bool is1 = false;
        bool isHaveM2M = false;
        bool isHaveM2MM = false;
        foreach (MapM2M m2m in m2ms)
        {
            if (m2m.HisM2MType == M2MType.M2M)
                isHaveM2M = true;
            if (m2m.HisM2MType == M2MType.M2MM)
                isHaveM2MM = true;

            TextBox tb = new TextBox();
            tb.ID = "TB_" + m2m.NoOfObj;
            tb.Columns = 70;
            tb.Rows = 5;
            tb.TextMode = TextBoxMode.MultiLine;
            foreach (string s in strs)
            {
                if (s == null)
                    continue;

                if (s.Contains(m2m.NoOfObj + ":") == false)
                    continue;

                string[] ss = s.Split(':');
                tb.Text = ss[1];
            }
            this.Pub2.AddFieldSet("编号:" + m2m.NoOfObj + ",名称:" + m2m.Name);
            this.Pub2.Add(tb);
            this.Pub2.AddFieldSetEnd();
        }
        this.Pub2.AddHR();
        Button mybtn = new Button();
        mybtn.ID = "Btn_Save";
        mybtn.CssClass = "Btn";
        mybtn.Text = "保存";
        mybtn.Click += new EventHandler(mybtn_SaveAutoFullM2M_Click);
        this.Pub2.Add(mybtn);

        mybtn = new Button();
        mybtn.CssClass = "Btn";
        mybtn.ID = "Btn_Cancel";
        mybtn.Text = "取消";
        mybtn.Click += new EventHandler(mybtn_SaveAutoFullM2M_Click);
        this.Pub2.Add(mybtn);
        this.Pub2.AddFieldSetEnd();

        if (isHaveM2M)
        {
            this.Pub2.AddFieldSet("帮助:一对多");
            this.Pub2.Add("在主表相关数据发生变化后，一对多数据要发生变化，变化的格式为：");
            this.Pub2.AddBR("实例：SELECT No,Name FROM WF_Emp WHERE FK_Dept='@Key' ");
            this.Pub2.AddBR("相关内容的值发生改变时而自动填充checkbox。");
            this.Pub2.AddBR("注意:");
            this.Pub2.AddBR("1，@Key 是主表字段传递过来的变量。");
            this.Pub2.AddBR("2，必须并且仅有No,Name两个列，顺序不要颠倒。");
            this.Pub2.AddFieldSetEnd();
        }

        if (isHaveM2MM)
        {
            this.Pub2.AddFieldSet("帮助:一对多多");
            this.Pub2.Add("在主表相关数据发生变化后，一对多多数据要发生变化，变化的格式为：");
            this.Pub2.AddBR("实例：SELECT a.FK_Emp M1ID, a.FK_Station as M2ID, b.Name as M2Name FROM Port_EmpStation a, Port_Station b WHERE  A.FK_Station=B.No and a.FK_Emp='@Key'");
            this.Pub2.AddBR("相关内容的值发生改变时而自动填充checkbox。");
            this.Pub2.AddBR("注意:");
            this.Pub2.AddBR("1，@Key 是主表字段传递过来的变量。");
            this.Pub2.AddBR("2，必须并且仅有3个列 M1ID,M2ID,M2Name，顺序不要颠倒。第1列的ID对应列表的ID，第2，3列对应的是列表数据源的ID与名称。");
            this.Pub2.AddFieldSetEnd();
        }
    }
    /// <summary>
    /// 新建文本框自动完成
    /// </summary>
    public void EditAutoFullDtl_TB()
    {
        MapExt myme = new MapExt(this.MyPK);
        MapDtls dtls = new MapDtls(myme.FK_MapData);

        this.Pub2.AddH2("设置自动填充从表. <a href='?ExtType=" + this.ExtType + "&MyPK=" + this.MyPK + "&FK_MapData=" + this.FK_MapData + "&RefNo=" + this.RefNo + "'>返回</a>");
        if (dtls.Count == 0)
        {
            this.Pub2.Clear();
            this.Pub2.AddFieldSet("设置自动填充从表. <a href='?ExtType=" + this.ExtType + "&MyPK=" + this.MyPK + "&FK_MapData=" + this.FK_MapData + "&RefNo=" + this.RefNo + "'>返回</a>");
            this.Pub2.Add("该表单下没有从表，所以您不能为从表设置自动填充。");
            this.Pub2.AddFieldSetEnd();
            return;
        }

        string[] strs = myme.Tag1.Split('$');
        bool is1 = false;
        foreach (MapDtl dtl in dtls)
        {
            TextBox tb = new TextBox();
            tb.ID = "TB_" + dtl.No;
            tb.Columns = 70;
            tb.Rows = 5;
            tb.TextMode = TextBoxMode.MultiLine;
            foreach (string s in strs)
            {
                if (s == null)
                    continue;

                if (s.Contains(dtl.No + ":") == false)
                    continue;

                string[] ss = s.Split(':');
                tb.Text = ss[1];
            }
            this.Pub2.AddFieldSet("编号:" + dtl.No + ",名称:" + dtl.Name);
            this.Pub2.Add(tb);

            string fs = "可填充的字段:";
            MapAttrs attrs = new MapAttrs(dtl.No);
            foreach (MapAttr item in attrs)
            {
                if (item.KeyOfEn == "OID" || item.KeyOfEn == "RefPKVal")
                    continue;
                fs += item.KeyOfEn + ",";
            }
            this.Pub2.Add("<BR>" + fs.Substring(0, fs.Length - 1));
            this.Pub2.AddFieldSetEnd();
        }

        this.Pub2.AddHR();
        Button mybtn = new Button();
        mybtn.ID = "Btn_Save";
        mybtn.CssClass = "Btn";
        mybtn.Text = "保存";
        mybtn.Click += new EventHandler(mybtn_SaveAutoFullDtl_Click);
        this.Pub2.Add(mybtn);

        mybtn = new Button();
        mybtn.ID = "Btn_Cancel";
        mybtn.Text = "取消";
        mybtn.Click += new EventHandler(mybtn_SaveAutoFullDtl_Click);
        this.Pub2.Add(mybtn);
        this.Pub2.AddFieldSetEnd();

        this.Pub2.AddFieldSet("帮助:");
        this.Pub2.Add("在这里您需要设置一个查询语句");
        this.Pub2.AddBR("例如：SELECT XLMC AS suozaixianlu, bustype as V_BusType FROM [V_XLVsBusType] WHERE jbxx_htid='@Key'");
        this.Pub2.AddBR("这个查询语句要与从表的列对应上就可以在文本框的值发生改变时而自动填充。");
        this.Pub2.AddBR("注意:");
        this.Pub2.AddBR("1，@Key 是主表字段传递过来的变量。");
        this.Pub2.AddBR("2，从表列字段字名，与填充sql列字段大小写匹配。");
        this.Pub2.AddFieldSetEnd();
    }
    /// <summary>
    /// 新建文本框自动完成
    /// </summary>
    public void EditAutoFullDtl_DDL()
    {
        this.Pub2.AddFieldSet("<a href='?ExtType=" + this.ExtType + "&MyPK=" + this.MyPK + "&FK_MapData=" + this.FK_MapData + "&RefNo=" + this.RefNo + "'>返回</a> -设置自动填充从表");
        MapExt myme = new MapExt(this.MyPK);
        MapDtls dtls = new MapDtls(myme.FK_MapData);
        string[] strs = myme.Tag1.Split('$');
        this.Pub2.AddTable("border=0  align=left ");
        bool is1 = false;
        foreach (MapDtl dtl in dtls)
        {
            is1 = this.AddTR(is1);
            TextBox tb = new TextBox();
            tb.ID = "TB_" + dtl.No;
            tb.Columns = 80;
            tb.Rows = 3;
            tb.TextMode = TextBoxMode.MultiLine;
            foreach (string s in strs)
            {
                if (s == null)
                    continue;

                if (s.Contains(dtl.No + ":") == false)
                    continue;
                string[] ss = s.Split(':');
                tb.Text = ss[1];
            }

            this.Pub2.AddTDBegin();
            this.Pub2.AddB("&nbsp;&nbsp;" + dtl.Name + "-从表");
            this.Pub2.AddBR();
            this.Pub2.Add(tb);
            this.Pub2.AddTDEnd();
            this.Pub2.AddTREnd();
        }
        this.Pub2.AddTableEndWithHR();
        Button mybtn = new Button();
        mybtn.ID = "Btn_Save";
        mybtn.CssClass = "Btn";
        mybtn.Text = "保存";
        mybtn.Click += new EventHandler(mybtn_SaveAutoFullDtl_Click);
        this.Pub2.Add(mybtn);

        mybtn = new Button();
        mybtn.ID = "Btn_Cancel";
        mybtn.Text = "取消";
        mybtn.Click += new EventHandler(mybtn_SaveAutoFullDtl_Click);
        this.Pub2.Add(mybtn);
        this.Pub2.AddFieldSetEnd();
    }

    public void EditAutoJL()
    {
        MapExt myme = new MapExt(this.MyPK);
        MapAttrs attrs = new MapAttrs(myme.FK_MapData);
        string[] strs = myme.Tag.Split('$');

        this.Pub2.AddTable("border=0 width='70%' align=left");
        this.Pub2.AddCaptionLeft("<a href='?ExtType=" + this.ExtType + "&MyPK=" + this.MyPK + "&FK_MapData=" + this.FK_MapData + "&RefNo=" + this.RefNo + "'>返回</a> -设置级连菜单");
        bool is1 = false;
        foreach (MapAttr attr in attrs)
        {
            if (attr.LGType == FieldTypeS.Normal)
                continue;
            if (attr.UIIsEnable == false)
                continue;

            TextBox tb = new TextBox();
            tb.ID = "TB_" + attr.KeyOfEn;
            tb.Attributes["width"] = "100%";
            tb.Columns = 90;
            tb.Rows = 4;
            tb.TextMode = TextBoxMode.MultiLine;

            foreach (string s in strs)
            {
                if (s == null)
                    continue;

                if (s.Contains(attr.KeyOfEn + ":") == false)
                    continue;

                string[] ss = s.Split(':');
                tb.Text = ss[1];
            }

            this.Pub2.AddTR();
            this.Pub2.AddTD("" + attr.Name + "  " + attr.KeyOfEn + " 字段<br>");
            this.Pub2.AddTREnd();

            this.Pub2.AddTR();
            this.Pub2.AddTD(tb);
            this.Pub2.AddTREnd();
        }
        this.Pub2.AddTR();
        this.Pub2.AddTDBegin();

        Button mybtn = new Button();
        mybtn.ID = "Btn_Save";
        mybtn.CssClass = "Btn";
        mybtn.Text = "保存";
        mybtn.Click += new EventHandler(mybtn_SaveAutoFullJilian_Click);
        this.Pub2.Add(mybtn);

        mybtn = new Button();
        mybtn.CssClass = "Btn";
        mybtn.ID = "Btn_Cancel";
        mybtn.Text = "取消";
        mybtn.Click += new EventHandler(mybtn_SaveAutoFullJilian_Click);
        this.Pub2.Add(mybtn);

        this.Pub2.AddTDEnd();
        this.Pub2.AddTREnd();

        this.Pub2.AddTableEnd();
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        this.BindLeft();
        this.Page.Title = "表单扩展设置";
        switch (this.DoType)
        {
            case "Del":
                MapExt mm = new MapExt();
                mm.MyPK = this.MyPK;
                if (this.ExtType == MapExtXmlList.InputCheck)
                    mm.Retrieve();

                mm.Delete();
                this.Response.Redirect("MapExt.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&RefNo=" + this.RefNo, true);
                return;
            case "EditAutoJL":
                this.EditAutoJL();
                return;
            default:
                break;
        }

        if (this.ExtType == null)
        {
            this.Pub2.AddFieldSet("Help");
            this.Pub2.AddH3("所有技术资料都整理在，《驰骋工作流程引擎-流程开发说明书.doc》与《驰骋工作流程引擎-表单设计器操作说明书.doc》两个文件中。");
            this.Pub2.AddH3("<br>这两个文件位于:D:\\ccflow\\Documents下面.");
            this.Pub2.AddH3("<a href='http://ccflow.org/Help.aspx' target=_blank>官网帮助..</a>");
            this.Pub2.AddFieldSetEnd();
            return;
        }

        MapExts mes = new MapExts();
        switch (this.ExtType)
        {
            case MapExtXmlList.WordFrm: //word模版。
                this.FrmWord();
                break;
            case MapExtXmlList.ExcelFrm: //ExcelFrm。
                this.FrmExcel();
                break;
            case MapExtXmlList.Link: //字段连接。
                if (this.MyPK != null || this.DoType == "New")
                {
                    this.BindLinkEdit();
                    return;
                }
                this.BindLinkList();
                break;
            case MapExtXmlList.RegularExpression: //正则表达式。
                if (this.DoType == "templete")//选择模版
                {
                    this.BindReTemplete();
                    return;
                }
                if (this.MyPK != null || this.DoType == "New")
                {
                    this.BindRegularExpressionEdit();
                    return;
                }
                this.BindRegularExpressionList();
                break;
            case MapExtXmlList.PageLoadFull: //表单装载填充。
            case MapExtXmlList.StartFlow: //表单装载填充。
                this.BindPageLoadFull();
                break;
            case MapExtXmlList.AutoFullDLL: //动态的填充下拉框。
                this.BindAutoFullDDL();
                break;
            case MapExtXmlList.ActiveDDL: //联动菜单.
                if (this.MyPK != null || this.OperAttrKey != null || this.DoType == "New")
                {
                    Edit_ActiveDDL();
                    return;
                }
                mes.Retrieve(MapExtAttr.ExtType, this.ExtType,
                    MapExtAttr.FK_MapData, this.FK_MapData);
                this.MapExtList(mes);
                break;
            case MapExtXmlList.TBFullCtrl:  //自动完成.
                if (this.DoType == "EditAutoFullDtl")
                {
                    this.EditAutoFullDtl_TB();
                    return;
                }
                if (this.DoType == "EditAutoFullM2M")
                {
                    this.EditAutoFullM2M_TB();
                    return;
                }

                if (this.MyPK != null || this.DoType == "New")
                {
                    this.EditAutoFull_TB();
                    return;
                }
                mes.Retrieve(MapExtAttr.ExtType, this.ExtType,
                    MapExtAttr.FK_MapData, this.FK_MapData);
                this.MapExtList(mes);
                break;
            case MapExtXmlList.DDLFullCtrl:  //DDL自动完成.
                if (this.DoType == "EditAutoFullDtl")
                {
                    this.EditAutoFullDtl_DDL();
                    return;
                }
                if (this.MyPK != null || this.DoType == "New")
                {
                    this.EditAutoFull_DDL();
                    return;
                }
                mes.Retrieve(MapExtAttr.ExtType, this.ExtType,
                    MapExtAttr.FK_MapData, this.FK_MapData);
                this.MapExtList(mes);
                break;
            case MapExtXmlList.InputCheck: //输入检查.
                if (this.MyPK != null || this.DoType == "New")
                {
                    Edit_InputCheck();
                    return;
                }
                mes.Retrieve(MapExtAttr.ExtType, this.ExtType,
                    MapExtAttr.FK_MapData, this.FK_MapData);
                this.MapJS(mes);
                break;
            case MapExtXmlList.PopVal: //联动菜单.
                if (this.MyPK != null || this.DoType == "New")
                {
                    Edit_PopVal();
                    return;
                }
                mes.Retrieve(MapExtAttr.ExtType, this.ExtType,
                    MapExtAttr.FK_MapData, this.FK_MapData);
                this.MapExtList(mes);
                break;
            case MapExtXmlList.Func: //联动菜单.
                this.BindExpFunc();
                break;
            default:
                break;
        }
    }

    #region 正则表达式.
    public void BindReTemplete()
    {
        this.Pub2.AddFieldSet("使用事件模版：" + this.OperAttrKey);
        this.Pub2.AddTable("align=left width=100%");

        this.Pub2.AddTR();
        this.Pub2.AddTD("colspan=2", "<b color='blue'>使用事件模版,能够帮助您快速的定义表单字段事件</b>");
        this.Pub2.AddTREnd();

        this.Pub2.AddTR();
        this.Pub2.AddTDTitle("colspan=2", "事件模版-点击名称选用它");
        this.Pub2.AddTREnd();

        this.Pub2.AddTR();
        ListBox lb = new ListBox();
        lb.Style["width"] = "100%";
        lb.AutoPostBack = false;
        lb.ID = "LBReTemplete";
        
        BP.XML.RegularExpressions res = new BP.XML.RegularExpressions();
        res.RetrieveAll();
        foreach (BP.XML.RegularExpression item in res)
        {
            ListItem li = new ListItem(item.Name + "->" + item.Note, item.No);
            lb.Items.Add(li);
        }
        this.Pub2.AddTD("colspan=2", lb);
        this.Pub2.AddTREnd();

        this.Pub2.AddTRSum();
        Button btn = new Button();
        btn.ID = "BtnSave";
        btn.CssClass = "Btn";
        btn.Text = "Save";
        btn.Click += new EventHandler(btn_SaveReTemplete_Click);

        this.Pub2.AddTD("colspan=1 width='60'", btn);
        this.Pub2.AddTD("<a href='MapExt.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&OperAttrKey=" + this.OperAttrKey + "&DoType=New'>返回</a>");
        this.Pub2.AddTREnd();

        this.Pub2.AddTableEnd();

        this.Pub2.AddFieldSetEnd();
    }
    public void btn_SaveReTemplete_Click(object sender, EventArgs e)
    {
        ListBox lb = this.Pub2.FindControl("LBReTemplete") as ListBox;
        if (lb == null && lb.SelectedItem.Value == null) return;

        string newMyPk = "";
        BP.XML.RegularExpressionDtls reDtls = new BP.XML.RegularExpressionDtls();
        reDtls.RetrieveAll();

        //删除现有的逻辑.
        BP.Sys.MapExts exts = new BP.Sys.MapExts();
        exts.Delete(MapExtAttr.AttrOfOper, this.OperAttrKey,
            MapExtAttr.ExtType, BP.Sys.MapExtXmlList.RegularExpression);

        // 开始装载.
        foreach (BP.XML.RegularExpressionDtl dtl in reDtls)
        {
            if (dtl.ItemNo != lb.SelectedItem.Value)
                continue;

            BP.Sys.MapExt ext = new BP.Sys.MapExt();
            ext.MyPK = this.FK_MapData + "_" + this.OperAttrKey + "_" + MapExtXmlList.RegularExpression + "_" + dtl.ForEvent;
            ext.FK_MapData = this.FK_MapData;
            ext.AttrOfOper = this.OperAttrKey;
            ext.Doc = dtl.Exp; //表达公式.
            ext.Tag = dtl.ForEvent; //时间.
            ext.Tag1 = dtl.Msg;  //消息
            ext.ExtType = MapExtXmlList.RegularExpression; // 表达公式 .
            ext.Insert();
            newMyPk = ext.MyPK;
        }
        this.Response.Redirect("MapExt.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&MyPK=" + newMyPk + "&OperAttrKey=" + this.OperAttrKey + "&DoType=New", true);
    }
    public void BindRegularExpressionList()
    {
        MapExts mes = new MapExts();
        mes.Retrieve(MapExtAttr.ExtType, this.ExtType,
                   MapExtAttr.FK_MapData, this.FK_MapData);
        this.Pub2.AddTable("align=left width=100%");
        this.Pub2.AddCaptionLeftTX("正则表达式");
        this.Pub2.AddTR();
        this.Pub2.AddTDTitle("IDX");
        this.Pub2.AddTDTitle("字段");
        //this.Pub2.AddTDTitle("表达公式");
        //this.Pub2.AddTDTitle("提示信息");
        this.Pub2.AddTDTitle("操作");
        this.Pub2.AddTREnd();

        string tKey = DateTime.Now.ToString("yyMMddhhmmss");

        MapAttrs attrs = new MapAttrs(this.FK_MapData);
        int idx = 0;
        foreach (MapAttr attr in attrs)
        {
            if (attr.UIVisible == false)
                continue;

            #region  songhonggang (2014-06-15) 修改显示全部的正则
            //if (attr.UIIsEnable == false)
            //    continue;
            #endregion

            this.Pub2.AddTR();
            this.Pub2.AddTDIdx(idx++);
            this.Pub2.AddTD(attr.KeyOfEn + "-" + attr.Name);
            MapExt me = mes.GetEntityByKey(MapExtAttr.AttrOfOper, attr.KeyOfEn) as MapExt;
            if (me == null)
                this.Pub2.AddTD("<a href='MapExt.aspx?s=" + tKey + "&FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&OperAttrKey=" + attr.KeyOfEn + "&DoType=New'>设置</a>");
            else
                this.Pub2.AddTD("<a href='MapExt.aspx?s=" + tKey + "&FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&MyPK=" + me.MyPK + "&OperAttrKey=" + attr.KeyOfEn + "'>修改</a>");
            this.Pub2.AddTREnd();
        }
        this.Pub2.AddTableEnd();
    }
   
    public void BindRegularExpressionEdit()
    {
        this.Pub2.AddTable();
        this.Pub2.AddCaption("正则表达式 -<a href=\"MapExt.aspx?s=3&FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&OperAttrKey=" + this.OperAttrKey + "&DoType=templete\">加载模版</a>- <a href='MapExt.aspx?s=3&FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "' >返回</a>");

        this.Pub2.AddTR();
        this.Pub2.AddTDTitle("序");
        this.Pub2.AddTDTitle("事件");
        this.Pub2.AddTDTitle("事件内容");
        this.Pub2.AddTDTitle("提示信息");
        this.Pub2.AddTREnd();
           
        #region 绑定事件
        int idx = 1;
        idx = BindRegularExpressionEditExt(idx, "onblur");
        idx = BindRegularExpressionEditExt(idx, "onchange");

        idx = BindRegularExpressionEditExt(idx, "onclick");
        idx = BindRegularExpressionEditExt(idx, "ondblclick");

        idx = BindRegularExpressionEditExt(idx, "onkeypress");
        idx = BindRegularExpressionEditExt(idx, "onkeyup");
        idx = BindRegularExpressionEditExt(idx, "onsubmit");
        #endregion
        this.Pub2.AddTableEnd();

        Button btn = new Button();
        btn.ID = "Btn_Save";
        btn.Text = "Save";
        btn.Click += new EventHandler(BindRegularExpressionEdit_Click);
        this.Pub2.Add(btn);
       
    }
    public int BindRegularExpressionEditExt(int idx, string myEvent)
    {
        // 查询.
        MapExt me = new MapExt();
        me.FK_MapData = this.FK_MapData;
        me.Tag = myEvent;
        me.AttrOfOper = this.OperAttrKey;
        me.MyPK = me.FK_MapData + "_" + this.OperAttrKey + "_" + this.ExtType + "_" + myEvent;
        me.RetrieveFromDBSources();

        this.Pub2.AddTR();
        this.Pub2.AddTDIdx(idx);
        this.Pub2.AddTD(myEvent);

        TextBox tb = new TextBox();
        tb.TextMode = TextBoxMode.MultiLine;
        tb.ID = "TB_Doc_" + myEvent;
        tb.Text = me.Doc;
        tb.Columns = 50;
        tb.Rows = 3;
        this.Pub2.AddTD(tb);

        tb = new TextBox();
        tb.ID = "TB_Tag1_" + myEvent;
        tb.Text = me.Tag1;
        tb.Columns = 50;
        tb.Rows = 3;
        this.Pub2.AddTD(tb);
        this.Pub2.AddTREnd();
        idx = idx + 1;
        return idx;
    }
    public void BindRegularExpressionEdit_ClickSave(string myEvent)
    {
        MapExt me = new MapExt();
        me.FK_MapData = this.FK_MapData;
        me.ExtType = this.ExtType;
        me.Tag = myEvent;
        me.AttrOfOper = this.OperAttrKey;
        me.MyPK = this.FK_MapData + "_" + this.OperAttrKey + "_" + me.ExtType + "_" + me.Tag;
        me.Delete();

        me.Doc = this.Pub2.GetTextBoxByID("TB_Doc_" + myEvent).Text;
        me.Tag1 = this.Pub2.GetTextBoxByID("TB_Tag1_" + myEvent).Text;
        if (me.Doc.Trim().Length == 0)
            return;

        me.Insert();
    }
    void BindRegularExpressionEdit_Click(object sender, EventArgs e)
    {
        #region 保存
        BindRegularExpressionEdit_ClickSave("onblur");
        BindRegularExpressionEdit_ClickSave("onchange");

        BindRegularExpressionEdit_ClickSave("onclick");

        BindRegularExpressionEdit_ClickSave("ondblclick");

        BindRegularExpressionEdit_ClickSave("onkeypress");
        BindRegularExpressionEdit_ClickSave("onkeyup");
        BindRegularExpressionEdit_ClickSave("onsubmit");
        #endregion

        this.Response.Redirect("MapExt.aspx?s=3&FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType, true);
    }
    #endregion RegularExpression


    #region link.
    public void BindLinkList()
    {
        MapExts mes = new MapExts();
        mes.Retrieve(MapExtAttr.ExtType, this.ExtType,
                   MapExtAttr.FK_MapData, this.FK_MapData);
        this.Pub2.AddTable("align=left width=100%");
        this.Pub2.AddCaptionLeft("字段超连接");
        this.Pub2.AddTR();
        this.Pub2.AddTDTitle("IDX");
        this.Pub2.AddTDTitle("字段");
        this.Pub2.AddTDTitle("连接");
        this.Pub2.AddTDTitle("窗口");
        this.Pub2.AddTDTitle("操作");
        this.Pub2.AddTREnd();

        MapAttrs attrs = new MapAttrs(this.FK_MapData);
        int idx = 0;
        foreach (MapAttr attr in attrs)
        {
            if (attr.UIVisible == false)
                continue;

            if (attr.UIIsEnable == true)
                continue;

            this.Pub2.AddTR();
            this.Pub2.AddTDIdx(idx++);
            this.Pub2.AddTD(attr.KeyOfEn + "-" + attr.Name);
            MapExt me = mes.GetEntityByKey(MapExtAttr.AttrOfOper, attr.KeyOfEn) as MapExt;
            if (me == null)
            {
                this.Pub2.AddTD("-");
                this.Pub2.AddTD("-");
                this.Pub2.AddTD("<a href='MapExt.aspx?s=3&FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&OperAttrKey=" + attr.KeyOfEn + "&DoType=New'>设置</a>");
            }
            else
            {
                this.Pub2.AddTD(me.Tag);
                this.Pub2.AddTD(me.Tag1);
                this.Pub2.AddTD("<a href='MapExt.aspx?s=3&FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&MyPK=" + me.MyPK + "&OperAttrKey=" + attr.KeyOfEn + "'>修改</a>");
            }
            this.Pub2.AddTREnd();
        }
        this.Pub2.AddTableEnd();
    }
    public void BindLinkEdit()
    {
        MapExt me = new MapExt();
        if (this.MyPK != null)
        {
            me.MyPK = this.MyPK;
            me.RetrieveFromDBSources();
        }
        else
        {
            me.FK_MapData = this.FK_MapData;
            me.AttrOfOper = this.OperAttrKey;
            me.Tag = "http://ccflow.org";
            me.Tag1 = "_" + this.OperAttrKey;
        }

        this.Pub2.AddTable();
        this.Pub2.AddCaptionLeft("字段超连接 - <a href='MapExt.aspx?s=3&FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "' >返回</a>");
        this.Pub2.AddTR();
        this.Pub2.AddTD("字段英文名");
        this.Pub2.AddTD(this.OperAttrKey);
        this.Pub2.AddTREnd();

        this.Pub2.AddTR();
        this.Pub2.AddTD("字段中文名");
        MapAttr ma = new MapAttr(this.FK_MapData, this.OperAttrKey);
        this.Pub2.AddTD(ma.Name);
        this.Pub2.AddTREnd();
        TextBox tb = new TextBox();
        tb.ID = "TB_Tag";
        tb.Text = me.Tag;
        tb.Columns = 50;
        this.Pub2.AddTR();
        this.Pub2.AddTD("Url");
        this.Pub2.AddTD(tb);
        this.Pub2.AddTREnd();

        tb = new TextBox();
        tb.ID = "TB_Tag1";
        tb.Text = me.Tag1;
        tb.Columns = 50;
        this.Pub2.AddTR();
        this.Pub2.AddTD("窗口");
        this.Pub2.AddTD(tb);
        this.Pub2.AddTREnd();

        Button btn = new Button();
        btn.ID = "Btn_Save";
        btn.CssClass = "Btn";
        btn.Text = "Save";
        btn.Click += new EventHandler(BindLinkEdit_Click);
        this.Pub2.AddTR();
        this.Pub2.AddTD(btn);
        if (this.MyPK != null)
        {
            btn = new Button();
            btn.ID = "Btn_Del";
            btn.CssClass = "Btn";
            btn.Text = "Delete";
            btn.Click += new EventHandler(BindLinkEdit_Click);
            btn.Attributes["onclick"] = "return window.confirm('您确定要删除吗？');";
            this.Pub2.AddTD(btn);
        }
        else
        {
            this.Pub2.AddTD();
        }
        this.Pub2.AddTREnd();
        this.Pub2.AddTableEnd();
    }
    void BindLinkEdit_Click(object sender, EventArgs e)
    {
        MapExt me = new MapExt();
        Button btn = sender as Button;
        if (btn.ID == "Btn_Del")
        {
            me.MyPK = this.MyPK;
            me.Delete();
            this.Response.Redirect("MapExt.aspx?s=3&FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType, true);
            return;
        }

        me = (MapExt)this.Pub2.Copy(me);
        me.FK_MapData = this.FK_MapData;
        me.AttrOfOper = this.OperAttrKey;
        //me.Tag = this.Pub2.GetTextBoxByID("TB_Tag").Text;
        //me.Tag1 = this.Pub2.GetTextBoxByID("TB_Tag1").Text;
        me.ExtType = this.ExtType;
        if (this.MyPK == null)
            me.MyPK = me.FK_MapData + "_" + me.AttrOfOper + "_" + this.ExtType;
        else
            me.MyPK = this.MyPK;
        me.Save();

        this.Response.Redirect("MapExt.aspx?s=3&FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType, true);
    }
    #endregion
    /// <summary>
    /// BindPageLoadFull
    /// </summary>
    public void BindPageLoadFull()
    {
        MapExt me = new MapExt();
        me.MyPK = this.FK_MapData + "_" + this.ExtType;
        me.RetrieveFromDBSources();

        this.Pub2.AddTable("align=left");
        this.Pub2.AddCaptionLeft("填充主表SQL");
        this.Pub2.AddTR();
        this.Pub2.AddTDTitle("主表设置");
        this.Pub2.AddTREnd();

        TextBox tb = new TextBox();
        tb.ID = "TB_" + MapExtAttr.Tag;
        tb.Text = me.Tag;
        tb.TextMode = TextBoxMode.MultiLine;
        tb.Rows = 10;
        tb.Columns = 70;
        this.Pub2.AddTR();
        this.Pub2.AddTD(tb);
        this.Pub2.AddTREnd();

        this.Pub2.AddTR();
        this.Pub2.AddTD("说明:填充主表的sql,表达式里支持@变量与约定的公用变量。 <br>比如: SELECT No,Name,Tel FROM Port_Emp WHERE No='@WebUser.No' , 如果列名与开始表单字段名相同，就会自动给值。");
        this.Pub2.AddTREnd();

        MapDtls dtls = new MapDtls(this.FK_MapData);
        if (dtls.Count != 0)
        {
            this.Pub2.AddTR();
            this.Pub2.AddTDTitle("从表的自动填充.");
            this.Pub2.AddTREnd();
            string[] sqls = me.Tag1.Split('*');
            foreach (MapDtl dtl in dtls)
            {
                this.Pub2.AddTR();
                this.Pub2.AddTD("从表:(" + dtl.No + ")" + dtl.Name);
                this.Pub2.AddTREnd();
                tb = new TextBox();
                tb.ID = "TB_" + dtl.No;
                foreach (string sql in sqls)
                {
                    if (string.IsNullOrEmpty(sql))
                        continue;
                    string key = sql.Substring(0, sql.IndexOf('='));
                    if (key == dtl.No)
                    {
                        tb.Text = sql.Substring(sql.IndexOf('=') + 1);
                        break;
                    }
                }
                tb.TextMode = TextBoxMode.MultiLine;
                tb.Rows = 10;
                tb.Columns = 70;
                this.Pub2.AddTR();
                this.Pub2.AddTD(tb);
                this.Pub2.AddTREnd();
            }

            this.Pub2.AddTR();
            this.Pub2.AddTD("说明:结果集合填充从表");
            this.Pub2.AddTREnd();
        }

        Button btn = new Button();
        btn.CssClass = "Btn";
        btn.ID = "Btn_Save";
        btn.Text = " Save ";
        btn.Click += new EventHandler(btn_SavePageLoadFull_Click);
        this.Pub2.AddTR();
        this.Pub2.AddTD(btn);
        this.Pub2.AddTREnd();
        this.Pub2.AddTableEnd();
        return;
    }
    /// <summary>
    /// 保存它
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void btn_SavePageLoadFull_Click(object sender, EventArgs e)
    {
        MapExt me = new MapExt();
        me.MyPK = this.FK_MapData + "_" + this.ExtType;
        me.FK_MapData = this.FK_MapData;
        me.ExtType = this.ExtType;
        me.RetrieveFromDBSources();

        me.Tag = this.Pub2.GetTextBoxByID("TB_" + MapExtAttr.Tag).Text;
        string sql = "";
        MapDtls dtls = new MapDtls(this.FK_MapData);
        foreach (MapDtl dtl in dtls)
        {
            sql += "*" + dtl.No + "=" + this.Pub2.GetTextBoxByID("TB_" + dtl.No).Text;
        }
        me.Tag1 = sql;

        me.MyPK = this.FK_MapData + "_" + this.ExtType;

        string info = me.Tag1 + me.Tag;
        if (string.IsNullOrEmpty(info))
            me.Delete();
        else
            me.Save();
    }

    #region 保存word模版属性.
    /// <summary>
    /// Word属性.
    /// </summary>
    public void FrmWord()
    {
        MapData ath = new MapData(this.FK_MapData);

        #region WebOffice控制方式.
        this.Pub2.AddTable();

        this.Pub2.AddTR1();
        this.Pub2.AddTDTitle("colspan=3", "WebOffice控制方式.");
        this.Pub2.AddTREnd();

        this.Pub2.AddTR();
        CheckBox cb = new CheckBox();
        cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableWF;
        cb.Text = "是否启用weboffice？";
        cb.Checked = ath.IsWoEnableWF;
        this.Pub2.AddTD(cb);

        cb = new CheckBox();
        cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableSave;
        cb.Text = "是否启用保存？";
        cb.Checked = ath.IsWoEnableSave;
        this.Pub2.AddTD(cb);

        cb = new CheckBox();
        cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableReadonly;
        cb.Text = "是否只读？";
        cb.Checked = ath.IsWoEnableReadonly;
        this.Pub2.AddTD(cb);
        this.Pub2.AddTREnd();



        this.Pub2.AddTR();
        cb = new CheckBox();
        cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableRevise;
        cb.Text = "是否启用修订？";
        cb.Checked = ath.IsWoEnableRevise;
        this.Pub2.AddTD(cb);

        cb = new CheckBox();
        cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableViewKeepMark;
        cb.Text = "是否查看用户留痕？";
        cb.Checked = ath.IsWoEnableViewKeepMark;
        this.Pub2.AddTD(cb);

        cb = new CheckBox();
        cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnablePrint;
        cb.Text = "是否打印？";
        cb.Checked = ath.IsWoEnablePrint;
        this.Pub2.AddTD(cb);
        this.Pub2.AddTREnd();


        this.Pub2.AddTR();
        cb = new CheckBox();
        cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableOver;
        cb.Text = "是否启用套红？";
        cb.Checked = ath.IsWoEnableOver;
        this.Pub2.AddTD(cb);

        cb = new CheckBox();
        cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableSeal;
        cb.Text = "是否启用签章？";
        cb.Checked = ath.IsWoEnableSeal;
        this.Pub2.AddTD(cb);

        cb = new CheckBox();
        cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableTemplete;
        cb.Text = "是否启用模板文件？";
        cb.Checked = ath.IsWoEnableTemplete;
        this.Pub2.AddTD(cb);

        this.Pub2.AddTREnd();
        this.Pub2.AddTR();
        cb = new CheckBox();
        cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableCheck;
        cb.Text = "是否记录节点信息？";
        cb.Checked = ath.IsWoEnableCheck;
        this.Pub2.AddTD(cb);
        cb = new CheckBox();
        cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableInsertFlow;
        cb.Text = "是否启用插入流程？";
        cb.Checked = ath.IsWoEnableInsertFlow;
        this.Pub2.AddTD(cb);
        cb = new CheckBox();
        cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableInsertFengXian;
        cb.Text = "是否启用插入风险点？";
        cb.Checked = ath.IsWoEnableInsertFengXian;
        this.Pub2.AddTD(cb);
        this.Pub2.AddTR();
        cb = new CheckBox();
        cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableMarks;
        cb.Text = "是否进入留痕模式？";
        cb.Checked = ath.IsWoEnableMarks;
        this.Pub2.AddTD(cb);
        cb = new CheckBox();
        cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableDown;
        cb.Text = "是否启用下载？";
        cb.Checked = ath.IsWoEnableDown;
        this.Pub2.AddTD(cb);
        this.Pub2.AddTD("");
        this.Pub2.AddTREnd();
        this.Pub2.AddTREnd();
        this.Pub2.AddTableEnd();
        #endregion WebOffice控制方式.

        this.Pub2.Add("模版文件(必须是*.doc文件):");
        FileUpload fu = new FileUpload();
        fu.ID = "FU";
        this.Pub2.Add(fu);
        this.Pub2.Add("<a href='/DataUser/FrmOfficeTemplate/"+this.FK_MapData+".doc' target=_blank>下载或打开模版</a>");

        this.Pub2.AddBR(); 
        Button btn = new Button();
        btn.ID = "Save";
        btn.Text = "保存";
        btn.Click += new EventHandler(btn_SaveWordFrm_Click);
        this.Pub2.Add(btn);
    }

    void btn_SaveWordFrm_Click(object sender, EventArgs e)
    {
        MapData ath = new MapData(this.FK_MapData);
        //ath.IsWoEnableWF = this.Pub2.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableWF).Checked;
        //ath.IsWoEnableSave = this.Pub2.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableSave).Checked;
        //ath.IsWoEnableReadonly = this.Pub2.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableReadonly).Checked;
        //ath.IsWoEnableRevise = this.Pub2.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableRevise).Checked;
        //ath.IsWoEnableViewKeepMark = this.Pub2.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableViewKeepMark).Checked;
        //ath.IsWoEnablePrint = this.Pub2.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnablePrint).Checked;
        //ath.IsWoEnableSeal = this.Pub2.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableSeal).Checked;
        //ath.IsWoEnableOver = this.Pub2.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableOver).Checked;
        //ath.IsWoEnableTemplete = this.Pub2.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableTemplete).Checked;
        //ath.IsWoEnableCheck = this.Pub2.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableCheck).Checked;
        //ath.IsWoEnableInsertFengXian = this.Pub2.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableInsertFengXian).Checked;
        //ath.IsWoEnableInsertFlow = this.Pub2.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableInsertFlow).Checked;
        //ath.IsWoEnableMarks = this.Pub2.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableMarks).Checked;
        //ath.IsWoEnableDown = this.Pub2.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableDown).Checked;
        //ath.Update();

        FileUpload fu = this.Pub2.FindControl("FU") as FileUpload;
        if (fu.FileName != null)
            fu.SaveAs(SystemConfig.PathOfDataUser + "\\FrmOfficeTemplate\\" + this.FK_MapData + ".doc");
    }
    #endregion 保存word模版属性.

    #region 保存Excel模版属性.
    /// <summary>
    /// Word属性.
    /// </summary>
    public void FrmExcel()
    {
        BP.Sys.ToolbarExcel en =new ToolbarExcel(this.FK_MapData);

        this.Pub2.AddH2("编辑Excel表单属性.");
        this.Pub2.Add("<a href=\"javascript:WinOpen('/WF/Comm/RefFunc/UIEn.aspx?EnName=BP.Sys.ToolbarExcel&No=" + this.FK_MapData + "')\" >Excel配置项</a>");
        this.Pub2.Add("<br>模版文件(必须是*.xls文件):");
        FileUpload fu = new FileUpload();
        fu.ID = "FU";
        this.Pub2.Add(fu);
        this.Pub2.Add("<a href='/DataUser/FrmOfficeTemplate/" + this.FK_MapData + ".xls' target=_blank>下载或打开模版</a>");
        
        this.Pub2.AddBR();
        Button btn = new Button();
        btn.ID = "Save";
        btn.Text = "保存";
        btn.Click += new EventHandler(btn_SaveExcelFrm_Click);
        this.Pub2.Add(btn);
    }

    void btn_SaveExcelFrm_Click(object sender, EventArgs e)
    {
        //ath.IsWoEnableWF = this.Pub2.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableWF).Checked;
        //ath.IsWoEnableSave = this.Pub2.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableSave).Checked;
        //ath.IsWoEnableReadonly = this.Pub2.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableReadonly).Checked;
        //ath.IsWoEnableRevise = this.Pub2.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableRevise).Checked;
        //ath.IsWoEnableViewKeepMark = this.Pub2.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableViewKeepMark).Checked;
        //ath.IsWoEnablePrint = this.Pub2.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnablePrint).Checked;
        //ath.IsWoEnableSeal = this.Pub2.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableSeal).Checked;
        //ath.IsWoEnableOver = this.Pub2.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableOver).Checked;
        //ath.IsWoEnableTemplete = this.Pub2.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableTemplete).Checked;
        //ath.IsWoEnableCheck = this.Pub2.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableCheck).Checked;
        //ath.IsWoEnableInsertFengXian = this.Pub2.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableInsertFengXian).Checked;
        //ath.IsWoEnableInsertFlow = this.Pub2.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableInsertFlow).Checked;
        //ath.IsWoEnableMarks = this.Pub2.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableMarks).Checked;
        //ath.IsWoEnableDown = this.Pub2.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableDown).Checked;
        //ath.Update();

        FileUpload fu = this.Pub2.FindControl("FU") as FileUpload;
        if (fu.FileName != null)
            fu.SaveAs(SystemConfig.PathOfDataUser + "\\FrmOfficeTemplate\\" + this.FK_MapData + ".xls");
    }
    #endregion 保存word模版属性.

    public void BindAutoFullDDL()
    {
        if (this.DoType == "Del")
        {
            BP.Sys.MapExt me = new MapExt();
            me.MyPK = this.Request.QueryString["FK_MapExt"];
            me.Delete();
        }

        MapAttrs attrs = new MapAttrs();
        attrs.Retrieve(MapAttrAttr.FK_MapData, this.FK_MapData,
            MapAttrAttr.UIContralType, (int)BP.En.UIContralType.DDL,
            MapAttrAttr.UIVisible, 1, MapAttrAttr.UIIsEnable, 1);

        if (attrs.Count == 0)
        {
            this.Pub2.AddMsgOfWarning("提示",
                "此表单中没有可设置的自动填充内容。<br>只有满足，可见，被启用，是外键字段，才可以设置。");
            return;
        }

        MapExts mes = new MapExts();
        mes.Retrieve(MapExtAttr.FK_MapData, this.FK_MapData, MapExtAttr.ExtType, MapExtXmlList.AutoFullDLL);
        this.Pub2.AddTable("align=left width='60%'");
        this.Pub2.AddCaptionLeft(this.Lab);
        this.Pub2.AddTR();
        this.Pub2.AddTDTitle("序号");
        this.Pub2.AddTDTitle("字段");
        this.Pub2.AddTDTitle("中文名");
        this.Pub2.AddTDTitle("绑定源");
        this.Pub2.AddTDTitle("操作");
        this.Pub2.AddTREnd();
        string fk_attr = this.Request.QueryString["FK_Attr"];
        int idx = 0;
        MapAttr attrOper = null;
        foreach (MapAttr attr in attrs)
        {
            if (attr.KeyOfEn == fk_attr)
                attrOper = attr;

            this.Pub2.AddTR();
            this.Pub2.AddTDIdx(idx++);
            this.Pub2.AddTD(attr.KeyOfEn);
            this.Pub2.AddTD(attr.Name);
            this.Pub2.AddTD(attr.UIBindKey);
            MapExt me = mes.GetEntityByKey(MapExtAttr.AttrOfOper, attr.KeyOfEn) as MapExt;
            if (me == null)
                this.Pub2.AddTD("<a href='?FK_MapData=" + this.FK_MapData + "&FK_Attr=" + attr.KeyOfEn + "&ExtType=" + MapExtXmlList.AutoFullDLL + "' >设置</a>");
            else
                this.Pub2.AddTD("<a href='?FK_MapData=" + this.FK_MapData + "&FK_Attr=" + attr.KeyOfEn + "&ExtType=" + MapExtXmlList.AutoFullDLL + "' >编辑</a> - <a href=\"javascript:DoDel('" + me.MyPK + "','" + this.FK_MapData + "','" + MapExtXmlList.AutoFullDLL + "')\" >删除</a>");
            this.Pub2.AddTREnd();
        }

        if (fk_attr != null)
        {
            MapExt me = new MapExt();
            me.MyPK = MapExtXmlList.AutoFullDLL + "_" + this.FK_MapData + "_" + fk_attr;
            me.RetrieveFromDBSources();
            this.Pub2.AddTR();
            this.Pub2.AddTDBegin("colspan=5");
            this.Pub2.AddFieldSet("设置:(" + attrOper.KeyOfEn + " - " + attrOper.Name + ")运行时自动填充数据");
            TextBox tb = new TextBox();
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Columns = 80;
            tb.ID = "TB_Doc";
            tb.Rows = 4;
            tb.Text = me.Doc.Replace("~", "'");
            this.Pub2.Add(tb);
            this.Pub2.AddBR();
            Button btn = new Button();
            btn.ID = "Btn_Save_AutoFullDLL";
            btn.CssClass = "Btn";
            btn.Text = " 保 存 ";
            btn.Click += new EventHandler(btn_Save_AutoFullDLL_Click);
            this.Pub2.Add(btn);
            this.Pub2.Add("<br>事例:SELECT No,Name FROM Port_Emp WHERE FK_Dept LIKE '@WebUser.FK_Dept%' <br>您可以用@符号取本表单中的字段变量，或者全局变量，更多的信息请参考说明书。");
            this.Pub2.Add("<br>数据源必须具有No,Name两个列。");

            this.Pub2.AddFieldSetEnd();
            this.Pub2.AddTDEnd();
            this.Pub2.AddTREnd();
            this.Pub2.AddTableEnd();
        }
        else
        {
            this.Pub2.AddTableEnd();
        }
    }
    void btn_Save_AutoFullDLL_Click(object sender, EventArgs e)
    {
        string attr = this.Request.QueryString["FK_Attr"];
        MapExt me = new MapExt();
        me.MyPK = MapExtXmlList.AutoFullDLL + "_" + this.FK_MapData + "_" + attr;
        me.RetrieveFromDBSources();
        me.FK_MapData = this.FK_MapData;
        me.AttrOfOper = attr;
        me.ExtType = MapExtXmlList.AutoFullDLL;
        me.Doc = this.Pub2.GetTextBoxByID("TB_Doc").Text.Replace("'", "~");

        try
        {
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(me.Doc);
        }
        catch
        {
            this.Alert("SQL不能被正确的执行，拼写有问题，请检查。");
            return;
        }

        me.Save();
        this.Response.Redirect("MapExt.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + MapExtXmlList.AutoFullDLL, true);
    }
    /// <summary>
    /// 功能执行
    /// </summary>
    public void BindExpFunc()
    {
        BP.Sys.ExpFucnXmls xmls = new ExpFucnXmls();
        xmls.RetrieveAll();

        this.Pub2.AddFieldSet("导出");
        this.Pub2.AddUL();
        foreach (ExpFucnXml item in xmls)
        {
            this.Pub2.AddLi("MapExt.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&MyPK=" + this.MyPK + "&DoType=" + item.No + "&RefNo=" + this.RefNo,
           item.Name);
        }
        this.Pub2.AddULEnd();
        this.Pub2.AddFieldSetEnd();
    }
    void mybtn_SaveAutoFullDtl_Click(object sender, EventArgs e)
    {
        Button btn = sender as Button;
        if (btn.ID.Contains("Cancel"))
        {
            this.Response.Redirect("MapExt.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&MyPK=" + this.MyPK + "&RefNo=" + this.RefNo, true);
            return;
        }

        MapExt myme = new MapExt(this.MyPK);
        MapDtls dtls = new MapDtls(myme.FK_MapData);
        string info = "";
        string error = "";
        foreach (MapDtl dtl in dtls)
        {
            TextBox tb = this.Pub2.GetTextBoxByID("TB_" + dtl.No);
            if (tb.Text.Trim() == "")
                continue;
            try
            {
                //DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(tb.Text);
                //MapAttrs attrs = new MapAttrs(dtl.No);
                //string err = "";
                //foreach (DataColumn dc in dt.Columns)
                //{
                //    if (attrs.IsExits(MapAttrAttr.KeyOfEn, dc.ColumnName) == false)
                //    {
                //        err += "<br>列" + dc.ColumnName + "不能与从表 属性匹配.";
                //    }
                //}
                //if (err != "")
                //{
                //    error += "在为("+dtl.Name+")检查sql设置时出现错误:"+err;
                //}
            }
            catch (Exception ex)
            {
                this.Alert("SQL ERROR: " + ex.Message);
                return;
            }
            info += "$" + dtl.No + ":" + tb.Text;
        }

        if (error != "")
        {
            this.Pub2.AddMsgOfWarning("设置错误,请更正:", error);
            return;
        }
        myme.Tag1 = info;
        myme.Update();
        this.Response.Redirect("MapExt.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&MyPK=" + this.MyPK + "&RefNo=" + this.RefNo, true);
    }
    void mybtn_SaveAutoFullM2M_Click(object sender, EventArgs e)
    {
        Button btn = sender as Button;
        if (btn.ID.Contains("Cancel"))
        {
            this.Response.Redirect("MapExt.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&MyPK=" + this.MyPK + "&RefNo=" + this.RefNo, true);
            return;
        }

        MapExt myme = new MapExt(this.MyPK);
        MapM2Ms m2ms = new MapM2Ms(myme.FK_MapData);
        string info = "";
        string error = "";
        foreach (MapM2M m2m in m2ms)
        {
            TextBox tb = this.Pub2.GetTextBoxByID("TB_" + m2m.NoOfObj);
            if (tb.Text.Trim() == "")
                continue;
            try
            {
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(tb.Text);
                string err = "";
                if (dt.Columns[0].ColumnName != "No")
                    err += "第1列不是No.";
                if (dt.Columns[1].ColumnName != "Name")
                    err += "第2列不是Name.";

                if (err != "")
                {
                    error += "在为(" + m2m.Name + ")检查sql设置时出现错误：请确认列的顺序是否正确为大小写是否匹配。" + err;
                }
            }
            catch (Exception ex)
            {
                this.Alert("SQL ERROR: " + ex.Message);
                return;
            }
            info += "$" + m2m.NoOfObj + ":" + tb.Text;
        }

        if (error != "")
        {
            this.Pub2.AddMsgOfWarning("设置错误,请更正:", error);
            return;
        }
        myme.Tag2 = info;
        myme.Update();
        this.Response.Redirect("MapExt.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&MyPK=" + this.MyPK + "&RefNo=" + this.RefNo, true);
    }
    void mybtn_SaveAutoFullJilian_Click(object sender, EventArgs e)
    {
        Button btn = sender as Button;
        if (btn.ID.Contains("Cancel"))
        {
            this.Response.Redirect("MapExt.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&MyPK=" + this.MyPK + "&RefNo=" + this.RefNo, true);
            return;
        }

        MapExt myme = new MapExt(this.MyPK);
        MapAttrs attrs = new MapAttrs(myme.FK_MapData);
        string info = "";
        foreach (MapAttr attr in attrs)
        {
            if (attr.LGType == FieldTypeS.Normal)
                continue;

            if (attr.UIIsEnable == false)
                continue;

            TextBox tb = this.Pub2.GetTextBoxByID("TB_" + attr.KeyOfEn);
            if (tb.Text.Trim() == "")
                continue;

            try
            {
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(tb.Text);
                if (tb.Text.Contains("@Key") == false)
                    throw new Exception("缺少@Key参数。");

                if (dt.Columns.Contains("No") == false || dt.Columns.Contains("Name") == false)
                    throw new Exception("在您的sql表单公式中必须有No,Name两个列，来绑定下拉框。");
            }
            catch (Exception ex)
            {
                this.Alert("SQL ERROR: " + ex.Message);
                return;
            }
            info += "$" + attr.KeyOfEn + ":" + tb.Text;
        }
        myme.Tag = info;
        myme.Update();
        this.Alert("保存成功.");
        //   this.Response.Redirect("MapExt.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&MyPK=" + this.MyPK + "&RefNo=" + this.RefNo, true);
    }
    public void Edit_PopVal()
    {
        this.Pub2.AddTable("border=0");
        MapExt me = null;
        if (this.MyPK == null)
        {
            me = new MapExt();
            this.Pub2.AddCaptionLeft("新建:" + this.Lab + "-帮助请详见驰骋表单设计器说明书");
        }
        else
        {
            me = new MapExt(this.MyPK);
            this.Pub2.AddCaptionLeft("编辑:" + this.Lab + "-帮助请详见驰骋表单设计器说明书");
        }

        me.FK_MapData = this.FK_MapData;
        this.Pub2.AddTR();
        this.Pub2.AddTDTitle("项目");
        this.Pub2.AddTDTitle("采集");
        this.Pub2.AddTDTitle("说明");
        this.Pub2.AddTREnd();

        this.Pub2.AddTR();
        this.Pub2.AddTD("作用字段");
        BP.Web.Controls.DDL ddl = new BP.Web.Controls.DDL();
        ddl.ID = "DDL_Oper";
        MapAttrs attrs = new MapAttrs(this.FK_MapData);
        foreach (MapAttr attr in attrs)
        {
            if (attr.UIVisible == false)
                continue;

            if (attr.UIIsEnable == false)
                continue;

            if (attr.UIContralType == UIContralType.TB)
            {
                ddl.Items.Add(new ListItem(attr.KeyOfEn + " - " + attr.Name, attr.KeyOfEn));
                continue;
            }
        }
        ddl.SetSelectItem(me.AttrOfOper);
        this.Pub2.AddTD(ddl);
        this.Pub2.AddTD("处理pop窗体的字段.");
        this.Pub2.AddTREnd();

        this.Pub2.AddTR();
        this.Pub2.AddTD("设置类型");
        this.Pub2.AddTDBegin();

        RadioButton rb = new RadioButton();
        rb.Text = "自定义URL";
        rb.ID = "RB_Tag_0";
        rb.GroupName = "sd";
        if (me.PopValWorkModel == 0)
            rb.Checked = true;
        else
            rb.Checked = false;
        this.Pub2.Add(rb);
        rb = new RadioButton();
        rb.ID = "RB_Tag_1";
        rb.Text = "ccform内置";
        rb.GroupName = "sd";
        if (me.PopValWorkModel == 1)
            rb.Checked = true;
        else
            rb.Checked = false;
        this.Pub2.Add(rb);
        this.Pub2.AddTDEnd();
        this.Pub2.AddTD("如果是自定义URL,仅填写URL字段.");
        this.Pub2.AddTREnd();


        this.Pub2.AddTR();
        this.Pub2.AddTD("URL");
        TextBox tb = new TextBox();
        tb.ID = "TB_" + MapExtAttr.Doc;
        tb.Text = me.Doc;
        tb.Columns = 50;
        this.Pub2.AddTD("colspan=2", tb);
        this.Pub2.AddTREnd();

        this.Pub2.AddTR();
        this.Pub2.AddTD("colspan=3", "URL填写说明:请输入一个弹出窗口的url,当操作员关闭后返回值就会被设置在当前控件中<br>Test URL:http://localhost/Flow/SDKFlowDemo/PopSelectVal.aspx.");
        this.Pub2.AddTREnd();


        this.Pub2.AddTR();
        this.Pub2.AddTD("数据分组SQL");
        tb = new TextBox();
        tb.ID = "TB_" + MapExtAttr.Tag1;
        tb.Text = me.Tag1;
        tb.Columns = 50;
        this.Pub2.AddTD("colspan=2", tb);
        this.Pub2.AddTREnd();

        this.Pub2.AddTR();
        this.Pub2.AddTD("数据源SQL");
        tb = new TextBox();
        tb.ID = "TB_" + MapExtAttr.Tag2;
        tb.Text = me.Tag2;
        tb.Columns = 50;
        this.Pub2.AddTD("colspan=2", tb);
        this.Pub2.AddTREnd();
        this.Pub2.AddTREnd();

        #region 选择方式
        this.Pub2.AddTR();
        this.Pub2.AddTD("选择方式");
        this.Pub2.AddTDBegin();

        rb = new RadioButton();
        rb.Text = "多项选择";
        rb.ID = "RB_Tag3_0";
        rb.GroupName = "dd";
        if (me.PopValSelectModel == 0)
            rb.Checked = true;
        else
            rb.Checked = false;
        this.Pub2.Add(rb);

        rb = new RadioButton();
        rb.ID = "RB_Tag3_1";
        rb.Text = "单项选择";
        rb.GroupName = "dd";
        if (me.PopValSelectModel == 1)
            rb.Checked = true;
        else
            rb.Checked = false;
        this.Pub2.Add(rb);
        this.Pub2.AddTDEnd();
        this.Pub2.AddTD("");
        this.Pub2.AddTREnd();
        #endregion 选择方式


        #region 呈现方式
        this.Pub2.AddTR();
        this.Pub2.AddTD("数据源呈现方式");
        this.Pub2.AddTDBegin();

        rb = new RadioButton();
        rb.Text = "表格方式";
        rb.ID = "RB_Tag4_0";
        rb.GroupName = "dsd";
        if (me.PopValShowModel == 0)
            rb.Checked = true;
        else
            rb.Checked = false;
        this.Pub2.Add(rb);

        rb = new RadioButton();
        rb.ID = "RB_Tag4_1";
        rb.Text = "目录方式";
        rb.GroupName = "dsd";
        if (me.PopValShowModel == 1)
            rb.Checked = true;
        else
            rb.Checked = false;
        this.Pub2.Add(rb);
        this.Pub2.AddTDEnd();
        this.Pub2.AddTD("");
        this.Pub2.AddTREnd();
        #endregion 呈现方式

        this.Pub2.AddTR();
        this.Pub2.AddTD("返回值格式");
        ddl = new BP.Web.Controls.DDL();
        ddl.ID = "DDL_PopValFormat";
        ddl.BindSysEnum("PopValFormat");

        ddl.SetSelectItem(me.PopValFormat);

        this.Pub2.AddTD("colspan=2", ddl);
        this.Pub2.AddTREnd();
        this.Pub2.AddTREnd();

        this.Pub2.AddTRSum();
        Button btn = new Button();
        btn.ID = "BtnSave";
        btn.CssClass = "Btn";
        btn.Text = "Save";
        btn.Click += new EventHandler(btn_SavePopVal_Click);
        this.Pub2.AddTD("colspan=3", btn);
        this.Pub2.AddTREnd();
        this.Pub2.AddTableEnd();
    }
    public string EventName
    {
        get
        {
            string s = this.Request.QueryString["EventName"];
            return s;
        }
    }
    string temFile = "s@xa";
    public void Edit_InputCheck()
    {
        MapExt me = null;
        if (this.MyPK == null)
        {
            me = new MapExt();
            this.Pub2.AddFieldSet("新建:" + this.Lab);
        }
        else
        {
            me = new MapExt(this.MyPK);
            this.Pub2.AddFieldSet("编辑:" + this.Lab);
        }
        me.FK_MapData = this.FK_MapData;
        temFile = me.Tag;

        this.Pub2.AddTable("border=0  width='70%' align=left ");
        MapAttr attr = new MapAttr(this.RefNo);
        this.Pub2.AddCaptionLeft(attr.KeyOfEn + " - " + attr.Name);
        this.Pub2.AddTR();
        this.Pub2.AddTD("函数库来源:");
        this.Pub2.AddTDBegin();

        System.Web.UI.WebControls.RadioButton rb = new System.Web.UI.WebControls.RadioButton();
        rb.Text = "ccflow系统js函数库.";
        rb.ID = "RB_0";
        rb.AutoPostBack = true;
        if (me.DoWay == 0)
            rb.Checked = true;
        else
            rb.Checked = false;
        rb.GroupName = "s";
        rb.CheckedChanged += new EventHandler(rb_CheckedChanged);
        this.Pub2.Add(rb);

        rb = new System.Web.UI.WebControls.RadioButton();
        rb.AutoPostBack = true;
        rb.Text = "我自定义的函数库.";
        rb.CheckedChanged += new EventHandler(rb_CheckedChanged);
        rb.GroupName = "s";
        rb.ID = "RB_1";
        rb.AutoPostBack = true;
        if (me.DoWay == 1)
            rb.Checked = true;
        else
            rb.Checked = false;
        this.Pub2.Add(rb);
        this.Pub2.AddTDEnd();
        this.Pub2.AddTREnd();

        this.Pub2.AddTR();
        this.Pub2.AddTDTitle("colspan=2", "函数列表");
        this.Pub2.AddTREnd();
        this.Pub2.AddTR();

        ListBox lb = new ListBox();
        lb.Attributes["width"] = "100%";
        lb.AutoPostBack = false;
        lb.ID = "LB1";
        this.Pub2.AddTD("colspan=2", lb);
        this.Pub2.AddTREnd();

        this.Pub2.AddTRSum();
        Button btn = new Button();
        btn.ID = "BtnSave";
        btn.CssClass = "Btn";
        btn.Text = "Save";
        btn.Click += new EventHandler(btn_SaveInputCheck_Click);

        this.Pub2.AddTD("colspan=1", btn);
        this.Pub2.AddTD("<a href='MapExt.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "'>返回</a>");
        this.Pub2.AddTREnd();
        this.Pub2.AddTableEnd();
        this.Pub2.AddFieldSetEnd();
        rb_CheckedChanged(null, null);
    }
    void rb_CheckedChanged(object sender, EventArgs e)
    {
        string path = BP.Sys.SystemConfig.PathOfData + "\\JSLib\\";
        System.Web.UI.WebControls.RadioButton rb = this.Pub2.GetRadioButtonByID("RB_0"); // sender as System.Web.UI.WebControls.RadioButton;
        if (rb.Checked == false)
            path = BP.Sys.SystemConfig.PathOfDataUser + "\\JSLib\\";

        ListBox lb = this.Pub2.FindControl("LB1") as ListBox;
        lb.Items.Clear();
        lb.AutoPostBack = false;
        lb.SelectionMode = ListSelectionMode.Multiple;
        lb.Rows = 10;
        //lb.SelectedIndexChanged += new EventHandler(lb_SelectedIndexChanged);
        string file = temFile;
        if (string.IsNullOrEmpty(temFile) == false)
        {
            file = file.Substring(file.LastIndexOf('\\') + 4);
            file = file.Replace(".js", "");
        }
        else
        {
            file = "!!!";
        }

        MapExts mes = new MapExts();
        mes.Retrieve(MapExtAttr.FK_MapData, this.FK_MapData,
            MapExtAttr.AttrOfOper, this.OperAttrKey,
            MapExtAttr.ExtType, this.ExtType);

        string[] dirs = System.IO.Directory.GetDirectories(path);
        foreach (string dir in dirs)
        {
            string[] strs = Directory.GetFiles(dir);
            foreach (string s in strs)
            {
                if (s.Contains(".js") == false)
                    continue;

                ListItem li = new ListItem(s.Replace(path, "").Replace(".js", ""), s);
                if (s.Contains(file))
                    li.Selected = true;

                lb.Items.Add(li);
            }
        }
    }
    public void EditAutoFull_TB()
    {
        MapExt me = null;
        if (this.MyPK == null)
            me = new MapExt();
        else
            me = new MapExt(this.MyPK);

        me.FK_MapData = this.FK_MapData;

        this.Pub2.AddTable("border=0");
        this.Pub2.AddCaptionLeft("新建:" + this.Lab);
        this.Pub2.AddTR();
        this.Pub2.AddTDTitle("项目");
        this.Pub2.AddTDTitle("采集");
        this.Pub2.AddTREnd();

        this.Pub2.AddTR();
        this.Pub2.AddTD("下拉框");
        BP.Web.Controls.DDL ddl = new BP.Web.Controls.DDL();
        ddl.ID = "DDL_Oper";
        MapAttrs attrs = new MapAttrs(this.FK_MapData);
        foreach (MapAttr attr in attrs)
        {
            if (attr.UIVisible == false)
                continue;

            if (attr.UIIsEnable == false)
                continue;

            if (attr.UIContralType == UIContralType.TB)
            {
                ddl.Items.Add(new ListItem(attr.KeyOfEn + " - " + attr.Name, attr.KeyOfEn));
                continue;
            }
        }
        ddl.SetSelectItem(me.AttrOfOper);
        this.Pub2.AddTD(ddl);
        this.Pub2.AddTREnd();

        this.Pub2.AddTR();
        this.Pub2.AddTDTitle("colspan=2", "自动填充SQL:");
        this.Pub2.AddTREnd();

        this.Pub2.AddTR();
        TextBox tb = new TextBox();
        tb.ID = "TB_Doc";
        tb.Text = me.Doc;
        tb.TextMode = TextBoxMode.MultiLine;
        tb.Rows = 5;
        tb.Columns = 80;
        this.Pub2.AddTD("colspan=2", tb);
        this.Pub2.AddTREnd();

        this.Pub2.AddTR();
        this.Pub2.AddTDTitle("colspan=2", "关键字查询的SQL:");
        this.Pub2.AddTREnd();

        this.Pub2.AddTR();
        tb = new TextBox();
        tb.ID = "TB_Tag";
        tb.Text = me.Tag;
        tb.TextMode = TextBoxMode.MultiLine;
        tb.Rows = 5;
        tb.Columns = 80;
        this.Pub2.AddTD("colspan=2", tb);
        this.Pub2.AddTREnd();

        this.Pub2.AddTRSum();
        this.Pub2.AddTDBegin("colspan=2");

        Button btn = new Button();
        btn.CssClass = "Btn";
        btn.ID = "BtnSave";
        btn.Text = "保存";
        btn.Click += new EventHandler(btn_SaveAutoFull_Click);
        this.Pub2.Add(btn);

        if (this.MyPK == null)
        {
        }
        else
        {
            this.Pub2.Add("<a href=\"MapExt.aspx?MyPK=" + this.MyPK + "&FK_MapData=" + this.FK_MapData + "&RefNo = " + this.RefNo + "&ExtType=" + this.ExtType + "&DoType=EditAutoJL\" >级连下拉框</a>");
            this.Pub2.Add("-<a href=\"MapExt.aspx?MyPK=" + this.MyPK + "&FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&RefNo=" + this.RefNo + "&DoType=EditAutoFullDtl\" >填充从表</a>");
            this.Pub2.Add("-<a href=\"MapExt.aspx?MyPK=" + this.MyPK + "&FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&RefNo=" + this.RefNo + "&DoType=EditAutoFullM2M\" >填充一对多</a>");

        }
        this.Pub2.AddTDEnd();
        this.Pub2.AddTREnd();
        this.Pub2.AddTableEnd();
        #region 输出事例

        this.Pub2.AddFieldSet("帮助");
        this.Pub2.AddB("For oracle:");
        string sql = "自动填充SQL:<br>SELECT No as ~No~ , Name as ~Name~, Name as ~mingcheng~ FROM WF_Emp WHERE No LIKE '@Key%' AND ROWNUM<=15";
        sql += "<br>关键字查询SQL:<br>SELECT No as ~No~ , Name as ~Name~, Name as ~mingcheng~ FROM WF_Emp WHERE No LIKE '@Key%'  ";
        this.Pub2.AddBR(sql.Replace("~", "\""));

        this.Pub2.AddB("<br>For sqlserver:");
        sql = "自动填充SQL:<br>SELECT TOP 15 No, Name , Name as mingcheng FROM WF_Emp WHERE No LIKE '@Key%'";
        sql += "<br>关键字查询SQL:<br>SELECT  No, Name , Name as mingcheng FROM WF_Emp WHERE No LIKE '@Key%'";
        this.Pub2.AddBR(sql.Replace("~", "\""));

        this.Pub2.AddB("<br>注意:");
        this.Pub2.AddBR("1,文本框自动完成填充事例: 必须有No,Name两列，它用于显示下列出的提示列表。");
        this.Pub2.AddBR("2,设置合适的记录数量，能够改善系统执行效率。");
        this.Pub2.AddBR("3,@Key 是系统约定的关键字，就是当用户输入一个字符后ccform就会传递此关键字到数据库查询把结果返回给用户。");
        this.Pub2.AddBR("4,其它的列与本表单的字段名相同则可自动填充，要注意大小写匹配。");
        this.Pub2.AddBR("5,关键字查询sql是用来，双点文本框时弹出的查询语句，如果为空就按自动填充的sql计算。");

        this.Pub2.AddFieldSetEnd();
        #endregion 输出事例
    }
    public void EditAutoFull_DDL()
    {
        MapExt me = null;
        if (this.MyPK == null)
            me = new MapExt();
        else
            me = new MapExt(this.MyPK);

        me.FK_MapData = this.FK_MapData;

        this.Pub2.AddTable("align=left");
        this.Pub2.AddCaptionLeft("新建:" + this.Lab);
        this.Pub2.AddTR();
        this.Pub2.AddTDTitle("项目");
        this.Pub2.AddTDTitle("采集");
        this.Pub2.AddTDTitle("说明");
        this.Pub2.AddTREnd();

        this.Pub2.AddTR();
        this.Pub2.AddTD("下拉框");
        BP.Web.Controls.DDL ddl = new BP.Web.Controls.DDL();
        ddl.ID = "DDL_Oper";
        MapAttrs attrs = new MapAttrs(this.FK_MapData);
        foreach (MapAttr attr in attrs)
        {
            if (attr.UIVisible == false)
                continue;

            if (attr.UIIsEnable == false)
                continue;

            if (attr.UIContralType == UIContralType.DDL)
            {
                ddl.Items.Add(new ListItem(attr.KeyOfEn + " - " + attr.Name, attr.KeyOfEn));
                continue;
            }
        }
        ddl.SetSelectItem(me.AttrOfOper);

        this.Pub2.AddTD(ddl);
        this.Pub2.AddTD("输入项");
        this.Pub2.AddTREnd();

        this.Pub2.AddTR();
        this.Pub2.AddTDTitle("colspan=3", "自动填充SQL:");
        this.Pub2.AddTREnd();

        this.Pub2.AddTR();
        TextBox tb = new TextBox();
        tb.ID = "TB_Doc";
        tb.Text = me.Doc;
        tb.TextMode = TextBoxMode.MultiLine;
        tb.Rows = 5;
        tb.Columns = 80;
        this.Pub2.AddTD("colspan=3", tb);
        this.Pub2.AddTREnd();

        this.Pub2.AddTRSum();
        Button btn = new Button();
        btn.CssClass = "Btn";
        btn.ID = "BtnSave";
        btn.Text = "保存";
        btn.Click += new EventHandler(btn_SaveAutoFull_Click);
        this.Pub2.AddTD("colspan=2", btn);
        if (this.MyPK == null)
            this.Pub2.AddTD();
        else
            this.Pub2.AddTD("<a href=\"MapExt.aspx?MyPK=" + this.MyPK + "&FK_MapData=" + this.FK_MapData + "&RefNo = " + this.RefNo + "&ExtType=" + this.ExtType + "&DoType=EditAutoJL\" >级连下拉框</a>-<a href=\"MapExt.aspx?MyPK=" + this.MyPK + "&FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&RefNo=" + this.RefNo + "&DoType=EditAutoFullDtl\" >填充从表</a>");
        this.Pub2.AddTREnd();

        #region 输出事例
        this.Pub2.AddTRSum();
        this.Pub2.Add("\n<TD class='BigDoc' valign=top colspan=3>");

        this.Pub2.AddFieldSet("填充事例:");
        string sql = "SELECT dizhi as Addr, fuzeren as Manager FROM Prj_Main WHERE No = '@Key'";
        this.Pub2.Add(sql.Replace("~", "\""));
        this.Pub2.AddBR("<hr><b>说明：</b>根据用户当前选择下拉框的实例（比如:选择一个工程）把相关此实例的其它属性放在控件中");
        this.Pub2.Add("（比如：工程的地址，负责人。）");
        this.Pub2.AddBR("<b>备注：</b><br>1.只有列名与本表单中字段名称匹配才能自动填充上去。<br>2.sql查询出来的是一行数据，@Key 是当前选择的值。");
        this.Pub2.AddFieldSetEnd();

        this.Pub2.AddTDEnd();
        this.Pub2.AddTREnd();
        this.Pub2.AddTableEnd();
        #endregion 输出事例
    }
    public void Edit_ActiveDDL()
    {
        MapExt me = null;
        if (this.MyPK == null)
        {
            me = new MapExt();
            this.Pub2.AddFieldSet("新建:" + this.Lab);
        }
        else
        {
            me = new MapExt(this.MyPK);
            this.Pub2.AddFieldSet("编辑:" + this.Lab);
        }
        me.FK_MapData = this.FK_MapData;

        this.Pub2.AddTable("border=0  width='300px' align=left ");
        this.Pub2.AddCaptionLeft(this.Lab);
        this.Pub2.AddTR();
        this.Pub2.AddTDTitle("项目");
        this.Pub2.AddTDTitle("采集");
        this.Pub2.AddTDTitle("说明");
        this.Pub2.AddTREnd();

        this.Pub2.AddTR();
        this.Pub2.AddTD("主菜单");
        BP.Web.Controls.DDL ddl = new BP.Web.Controls.DDL();
        ddl.ID = "DDL_Oper";
        MapAttrs attrs = new MapAttrs(this.FK_MapData);
        int num = 0;
        foreach (MapAttr attr in attrs)
        {
            if (attr.UIVisible == false)
                continue;

            if (attr.UIIsEnable == false)
                continue;

            if (attr.UIContralType == UIContralType.DDL)
            {
                num++;
                ddl.Items.Add(new ListItem(attr.KeyOfEn + " - " + attr.Name, attr.KeyOfEn));
                continue;
            }
        }
        ddl.SetSelectItem(me.AttrOfOper);

        this.Pub2.AddTD(ddl);
        this.Pub2.AddTD("输入项");
        this.Pub2.AddTREnd();

        this.Pub2.AddTR();
        this.Pub2.AddTD("联动项");
        ddl = new BP.Web.Controls.DDL();
        ddl.ID = "DDL_Attr";
        foreach (MapAttr attr in attrs)
        {
            if (attr.UIVisible == false)
                continue;

            if (attr.UIIsEnable == false)
                continue;

            if (attr.UIContralType != UIContralType.DDL)
                continue;

            ddl.Items.Add(new ListItem(attr.KeyOfEn + " - " + attr.Name, attr.KeyOfEn));
        }
        ddl.SetSelectItem(me.AttrsOfActive);
        this.Pub2.AddTD(ddl);
        this.Pub2.AddTD("要实现联动效果的菜单");
        this.Pub2.AddTREnd();

        this.Pub2.AddTR();
        this.Pub2.AddTDTitle("colspan=3", "联动方式");
        this.Pub2.AddTREnd();

        this.Pub2.AddTR();
        this.Pub2.Add("<TD class=BigDoc width='100%' colspan=3>");
        RadioButton rb = new RadioButton();
        rb.Text = "通过sql获取联动";
        rb.GroupName = "sdr";
        rb.ID = "RB_0";
        if (me.DoWay == 0)
            rb.Checked = true;

        this.Pub2.AddFieldSet(rb);
        this.Pub2.Add("在下面文本框中输入一个SQL,具有编号，标签列，用来绑定下从动下拉框。");
        this.Pub2.Add("比如:SELECT No, Name FROM CN_City WHERE FK_SF = '@Key' ");
        this.Pub2.AddBR();
        TextBox tb = new TextBox();
        tb.ID = "TB_Doc";
        tb.Text = me.Doc;
        tb.Columns = 80;
        tb.CssClass = "TH";
        tb.TextMode = TextBoxMode.MultiLine;
        tb.Rows = 7;
        this.Pub2.Add(tb);
        this.Pub2.Add("说明:@Key是ccflow约定的关键字，是主下拉框传递过来的值");
        this.Pub2.AddFieldSetEnd();

        rb = new RadioButton();
        rb.Text = "通过编码标识获取";
        rb.GroupName = "sdr";
        rb.Enabled = false;
        rb.ID = "RB_1";
        if (me.DoWay == 1)
            rb.Checked = true;

        this.Pub2.AddFieldSet(rb);
        this.Pub2.Add("主菜单是编号的是从动菜单编号的前几位，不必联动内容。");
        this.Pub2.Add("比如: 主下拉框是省份，联动菜单是城市。");
        this.Pub2.AddFieldSetEnd();

        this.Pub2.Add("</TD>");
        this.Pub2.AddTREnd();



        this.Pub2.AddTRSum();
        Button btn = new Button();
        btn.CssClass = "Btn";
        btn.ID = "BtnSave";
        btn.Text = "Save";
        btn.Click += new EventHandler(btn_SaveJiLian_Click);
        this.Pub2.AddTD("colspan=3", btn);
        this.Pub2.AddTREnd();
        this.Pub2.AddTableEnd();

        this.Pub2.AddFieldSetEnd();
    }
    void btn_SaveJiLian_Click(object sender, EventArgs e)
    {
        MapExt me = new MapExt();
        me.MyPK = this.MyPK;
        if (me.MyPK.Length > 2)
            me.RetrieveFromDBSources();
        me = (MapExt)this.Pub2.Copy(me);
        me.ExtType = this.ExtType;
        me.Doc = this.Pub2.GetTextBoxByID("TB_Doc").Text;
        me.AttrOfOper = this.Pub2.GetDDLByID("DDL_Oper").SelectedItemStringVal;
        me.AttrsOfActive = this.Pub2.GetDDLByID("DDL_Attr").SelectedItemStringVal;
        if (me.AttrsOfActive == me.AttrOfOper)
        {
            this.Alert("两个项目不能相同.");
            return;
        }
        if (this.Pub2.GetRadioButtonByID("RB_1").Checked)
            me.DoWay = 1;
        else
            me.DoWay = 0;

        me.FK_MapData = this.FK_MapData;
        try
        {
            me.MyPK = this.FK_MapData + "_" + me.ExtType + "_" + me.AttrOfOper + "_" + me.AttrsOfActive;

            if (me.Doc.Contains("No") == false || me.Doc.Contains("Name") == false)
                throw new Exception("在您的sql表达式里，必须有No,Name 还两个列。");
            //DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(me.Doc);
            //if (dt.Columns.Contains("Name") == false || dt.Columns.Contains("No") == false)
            //    throw new Exception("在您的sql表达式里，必须有No,Name 还两个列。");
            me.Save();
        }
        catch (Exception ex)
        {
            this.Alert(ex.Message);
            return;
        }
        this.Response.Redirect("MapExt.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&RefNo = " + this.RefNo, true);
    }
    void btn_SaveInputCheck_Click(object sender, EventArgs e)
    {
        ListBox lb = this.Pub2.FindControl("LB1") as ListBox;

        // 检查路径. 没有就创建它。
        string pathDir = BP.Sys.SystemConfig.PathOfDataUser + "\\JSLibData\\";
        if (Directory.Exists(pathDir) == false)
            Directory.CreateDirectory(pathDir);

        // 删除已经存在的数据.
        MapExt me = new MapExt();
        me.Retrieve(MapExtAttr.FK_MapData, this.FK_MapData,
            MapExtAttr.ExtType, this.ExtType,
            MapExtAttr.AttrOfOper, this.OperAttrKey);

        foreach (ListItem li in lb.Items)
        {
            if (li.Selected == false)
                continue;

            me = (MapExt)this.Pub2.Copy(me);
            me.ExtType = this.ExtType;

            // 操作的属性.
            me.AttrOfOper = this.OperAttrKey;
            //this.Pub2.GetDDLByID("DDL_Oper").SelectedItemStringVal;

            int doWay = 0;
            if (this.Pub2.GetRadioButtonByID("RB_0").Checked == false)
                doWay = 1;

            me.DoWay = doWay;
            me.Doc = BP.DA.DataType.ReadTextFile(li.Value);
            FileInfo info = new FileInfo(li.Value);
            me.Tag2 = info.Directory.Name;

            //获取函数的名称.
            string func = me.Doc;
            func = me.Doc.Substring(func.IndexOf("function") + 8);
            func = func.Substring(0, func.IndexOf("("));
            me.Tag1 = func.Trim();

            // 检查路径,没有就创建它.
            FileInfo fi = new FileInfo(li.Value);
            me.Tag = li.Value;
            me.FK_MapData = this.FK_MapData;
            me.ExtType = this.ExtType;
            me.MyPK = this.FK_MapData + "_" + me.ExtType + "_" + me.AttrOfOper + "_" + me.Tag1;
            try
            {
                me.Insert();
            }
            catch
            {
                me.Update();
            }
        }

        #region 把所有的js 文件放在一个文件里面。
        MapExts mes = new MapExts();
        mes.Retrieve(MapExtAttr.FK_MapData, this.FK_MapData,
            MapExtAttr.ExtType, this.ExtType);

        string js = "";
        foreach (MapExt me1 in mes)
        {
            js += "\r\n" + BP.DA.DataType.ReadTextFile(me1.Tag);
        }

        if (File.Exists(pathDir + "\\" + this.FK_MapData + ".js"))
            File.Delete(pathDir + "\\" + this.FK_MapData + ".js");

        BP.DA.DataType.WriteFile(pathDir + "\\" + this.FK_MapData + ".js", js);
        #endregion 把所有的js 文件放在一个文件里面。


        this.Response.Redirect("MapExt.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&RefNo = " + this.RefNo, true);
    }
    void btn_SavePopVal_Click(object sender, EventArgs e)
    {
        MapExt me = new MapExt();
        me.MyPK = this.MyPK;
        if (me.MyPK.Length > 2)
            me.RetrieveFromDBSources();
        me = (MapExt)this.Pub2.Copy(me);
        me.ExtType = this.ExtType;
        me.Doc = this.Pub2.GetTextBoxByID("TB_Doc").Text;
        me.AttrOfOper = this.Pub2.GetDDLByID("DDL_Oper").SelectedItemStringVal;
        me.SetPara("PopValFormat", this.Pub2.GetDDLByID("DDL_PopValFormat").SelectedItemStringVal);

        RadioButton rb = this.Pub2.GetRadioButtonByID("RB_Tag_0");
        if (rb.Checked)
            me.PopValWorkModel = 0;
        else
            me.PopValWorkModel = 1;

        rb = this.Pub2.GetRadioButtonByID("RB_Tag3_0");
        if (rb.Checked)
            me.PopValSelectModel = 0;
        else
            me.PopValSelectModel = 1;

           rb = this.Pub2.GetRadioButtonByID("RB_Tag4_0");
        if (rb.Checked)
            me.PopValShowModel = 0;
        else
            me.PopValShowModel = 1;
        

        me.FK_MapData = this.FK_MapData;
        me.MyPK = this.FK_MapData + "_" + me.ExtType + "_" + me.AttrOfOper;
        me.Save();
        this.Response.Redirect("MapExt.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&RefNo = " + this.RefNo, true);
    }
    void btn_SaveAutoFull_Click(object sender, EventArgs e)
    {
        MapExt me = new MapExt();
        me.MyPK = this.MyPK;
        if (me.MyPK.Length > 2)
            me.RetrieveFromDBSources();

        me = (MapExt)this.Pub2.Copy(me);
        me.ExtType = this.ExtType;
        me.Doc = this.Pub2.GetTextBoxByID("TB_Doc").Text;
        me.AttrOfOper = this.Pub2.GetDDLByID("DDL_Oper").SelectedItemStringVal;
        me.FK_MapData = this.FK_MapData;
        me.MyPK = this.FK_MapData + "_" + me.ExtType + "_" + me.AttrOfOper;

        try
        {
            //DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(me.Doc);
            //if (string.IsNullOrEmpty(me.Tag) == false)
            //{
            //    dt = BP.DA.DBAccess.RunSQLReturnTable(me.Tag);
            //    if (dt.Columns.Contains("Name") == false || dt.Columns.Contains("No") == false)
            //        throw new Exception("在您的sql表达式里，必须有No,Name 还两个列。");
            //}

            //if (this.ExtType == MapExtXmlList.TBFullCtrl)
            //{
            //    if (dt.Columns.Contains("Name") == false || dt.Columns.Contains("No") == false)
            //        throw new Exception("在您的sql表达式里，必须有No,Name 还两个列。");
            //}

            //MapAttrs attrs = new MapAttrs(this.FK_MapData);
            //foreach (DataColumn dc in dt.Columns)
            //{
            //    if (dc.ColumnName.ToLower() == "no" || dc.ColumnName.ToLower() == "name")
            //        continue;

            //    if (attrs.Contains(MapAttrAttr.KeyOfEn, dc.ColumnName) == false)
            //        throw new Exception("@系统没有找到您要匹配的列(" + dc.ColumnName + ")，注意:您要指定的列名区分大小写。");
            //}
            me.Save();
        }
        catch (Exception ex)
        {
            //this.Alert(ex.Message);
            this.AlertMsg_Warning("SQL错误", ex.Message);
            return;
        }
        this.Response.Redirect("MapExt.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&RefNo=" + this.RefNo, true);
    }
    public void MapExtList(MapExts ens)
    {
        this.Pub2.AddTable("border=0 width='80%' align=left");
        this.Pub2.AddCaptionLeft("<a href='MapExt.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&DoType=New&RefNo=" + this.RefNo + "' ><img src='/WF/Img/Btn/New.gif' border=0/>新建:" + this.Lab + "</a>");

        this.Pub2.AddTR();
        this.Pub2.AddTDTitle("类型");
        this.Pub2.AddTDTitle("描述");
        this.Pub2.AddTDTitle("字段");
        this.Pub2.AddTDTitle("删除");
        this.Pub2.AddTREnd();
        foreach (MapExt en in ens)
        {
            MapAttr ma = new MapAttr();
            ma.MyPK = this.FK_MapData + "_" + en.AttrOfOper;
            if (ma.RetrieveFromDBSources() == 0)
            {
                ma.Delete();
                continue;
            }

            this.Pub2.AddTR();
            this.Pub2.AddTD(en.ExtType);
            this.Pub2.AddTDA("MapExt.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&MyPK=" + en.MyPK + "&RefNo=" + this.RefNo, en.ExtDesc);

            this.Pub2.AddTD(en.AttrOfOper + " " + ma.Name);

            this.Pub2.AddTD("<a href=\"javascript:DoDel('" + en.MyPK + "','" + this.FK_MapData + "','" + this.ExtType + "');\" >删除</a>");
            this.Pub2.AddTREnd();
        }
        this.Pub2.AddTableEndWithBR();
    }
    public void MapJS(MapExts ens)
    {
        this.Pub2.AddTable("border=0 width=90% align=left");
        this.Pub2.AddCaptionLeft("脚本验证");
        this.Pub2.AddTR();
        this.Pub2.AddTDTitle("字段");
        this.Pub2.AddTDTitle("类型");
        this.Pub2.AddTDTitle("验证函数中文名");
        this.Pub2.AddTDTitle("显示");
        this.Pub2.AddTDTitle("操作");
        this.Pub2.AddTREnd();

        MapAttrs attrs = new MapAttrs(this.FK_MapData);
        foreach (MapAttr attr in attrs)
        {
            if (attr.UIVisible == false)
                continue;

            MapExt myEn = null;
            foreach (MapExt en in ens)
            {
                if (en.AttrOfOper == attr.KeyOfEn)
                {
                    myEn = en;
                    break;
                }
            }

            if (myEn == null)
            {
                this.Pub2.AddTRTX();
                this.Pub2.AddTD(attr.KeyOfEn + "-" + attr.Name);
                this.Pub2.AddTD("无");
                this.Pub2.AddTD("无");
                this.Pub2.AddTD("无");
                this.Pub2.AddTDA("MapExt.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&RefNo=" + attr.MyPK + "&OperAttrKey=" + attr.KeyOfEn + "&DoType=New", "<img src='/WF/Img/Btn/Edit.gif' border=0/>编辑");
                this.Pub2.AddTREnd();
            }
            else
            {
                this.Pub2.AddTRTX();
                this.Pub2.AddTD(attr.KeyOfEn + "-" + attr.Name);

                if (myEn.DoWay == 0)
                    this.Pub2.AddTD("系统函数");
                else
                    this.Pub2.AddTD("自定义函数");

                string file = myEn.Tag;
                file = file.Substring(file.LastIndexOf('\\') + 4);
                file = file.Replace(".js", "");

                this.Pub2.AddTDA("MapExt.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&MyPK=" + myEn.MyPK + "&RefNo=" + attr.MyPK + "&OperAttrKey=" + attr.KeyOfEn, file);

                this.Pub2.AddTD(myEn.Tag2 + "=" + myEn.Tag1 + "(this);");

                this.Pub2.AddTD("<a href=\"javascript:DoDel('" + myEn.MyPK + "','" + this.FK_MapData + "','" + this.ExtType + "');\" ><img src='/WF/Img/Btn/Delete.gif' border=0/>删除</a>");
                this.Pub2.AddTREnd();
            }
        }
        this.Pub2.AddTableEnd();
    }


}