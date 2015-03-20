using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using BP.Web.Controls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using BP.Sys;
using BP.En;
using BP.Web;
using BP.Web.UC;
namespace CCFlow.WF.MapDef
{
    public partial class Comm_MapDef_EditTable : BP.Web.WebPage
    {
        /// <summary>
        /// GroupField
        /// </summary>
        public int GroupField
        {
            get
            {
                string s = this.Request.QueryString["GroupField"];
                if (s == "" || s == null)
                    return 0;
                return int.Parse(s);
            }
        }
        /// <summary>
        /// 执行类型
        /// </summary>
        public new string DoType
        {
            get
            {
                return this.Request.QueryString["DoType"];
            }
        }
        public string FType
        {
            get
            {
                return this.Request.QueryString["FType"];
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
            this.Title = "编辑外键类型";
            MapAttr attr = null;
            if (this.RefNo == null)
            {
                attr = new MapAttr();
                string sfKey = this.Request.QueryString["SFKey"];
                SFTable sf = new SFTable(sfKey);
                attr.KeyOfEn = sf.FK_Val;
                attr.UIBindKey = sfKey;
                attr.Name = sf.Name;
            }
            else
            {
                attr = new MapAttr(this.RefNo);
            }
            BindTable(attr);
        }
        int idx = 1;
        public void BindTable(MapAttr mapAttr)
        {
            this.Pub1.AddTable();
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("ID");
            this.Pub1.AddTDTitle("项目");
            this.Pub1.AddTDTitle("值");
            this.Pub1.AddTDTitle("描述");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR1();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("字段中文名"); // 字段中文名称
            TB tb = new TB();
            tb.ID = "TB_Name";
            tb.Text = mapAttr.Name;
            tb.Attributes["width"] = "100%";
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("字段英文名"); // "字段英文名称"
            tb = new TB();
            if (this.RefNo != null)
            {
                this.Pub1.AddTD(mapAttr.KeyOfEn);
            }
            else
            {
                tb = new TB();
                tb.ID = "TB_KeyOfEn";
                tb.Text = mapAttr.KeyOfEn;
                this.Pub1.AddTD(tb);
            }

            if (string.IsNullOrEmpty(mapAttr.KeyOfEn))
                this.Pub1.AddTD("字母/数字/下划线组合");
            else
                this.Pub1.AddTD("<a href=\"javascript:clipboardData.setData('Text','" + mapAttr.KeyOfEn + "');alert('已经copy到粘帖版上');\" ><img src='../Img/Btn/Copy.gif' class='ICON' />复制字段名</a></TD>");

            this.Pub1.AddTREnd();


            this.Pub1.AddTR1();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("默认值"); // "默认值"

            tb = new TB();
            tb.ID = "TB_DefVal";
            tb.Text = mapAttr.Name;
            tb.Attributes["width"] = "100%";
            tb.Text = mapAttr.DefValReal;
            this.Pub1.AddTD(tb);

            //DDL ddl = new DDL();
            //ddl.ID = "DDL";
            //ddl.BindEntities(mapAttr.HisEntitiesNoName);
            //ddl.SetSelectItem(mapAttr.DefVal);
            //   this.Pub1.AddTD(ddl);
            // this.Pub1.AddTD(ddl);

            if (mapAttr.UIBindKey.Contains("."))
                this.Pub1.AddTD("<a href=\"javascript:WinOpen('../Comm/Search.aspx?EnsName=" + mapAttr.UIBindKey + "','df');\" >打开</a>");
            else
                this.Pub1.AddTD("<a href=\"javascript:WinOpen('../MapDef/SFTableEditData.aspx?RefNo=" + mapAttr.UIBindKey + "','df');\" >打开</a>");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("是否可编辑");
            this.Pub1.Add("<TD>");
            RadioButton rb = new RadioButton();
            rb.ID = "RB_UIIsEnable_0";
            rb.Text = "不可编辑";  //"不可编辑";
            rb.GroupName = "s";
            if (mapAttr.UIIsEnable)
                rb.Checked = false;
            else
                rb.Checked = true;

            this.Pub1.Add(rb);
            rb = new RadioButton();
            rb.ID = "RB_UIIsEnable_1";
            rb.Text = "可编辑"; //"可编辑";
            rb.GroupName = "s";

            if (mapAttr.UIIsEnable)
                rb.Checked = true;
            else
                rb.Checked = false;

            this.Pub1.Add(rb);
            this.Pub1.AddTDEnd();
            this.Pub1.AddTD("");
            this.Pub1.AddTREnd();

            #region 是否可界面可见
            this.Pub1.AddTR1();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("是否界面可见"); //是否界面可见
            this.Pub1.AddTDBegin();
            rb = new RadioButton();
            rb.ID = "RB_UIVisible_0";
            rb.Text = "不可见"; // 界面不可见
            rb.GroupName = "sa3";
            if (mapAttr.UIVisible)
                rb.Checked = false;
            else
                rb.Checked = true;

            this.Pub1.Add(rb);
            if (mapAttr.IsTableAttr)
                rb.Enabled = false;

            rb = new RadioButton();
            rb.ID = "RB_UIVisible_1";
            rb.Text = "界面可见"; // 界面可见;
            rb.GroupName = "sa3";

            if (mapAttr.UIVisible)
                rb.Checked = true;
            else
                rb.Checked = false;

            if (mapAttr.IsTableAttr)
                rb.Enabled = false;

            this.Pub1.Add(rb);
            this.Pub1.AddTDEnd();

            this.Pub1.AddTD("不可见则为隐藏字段.");
            this.Pub1.AddTREnd();
            #endregion 是否可界面可见

            #region 是否可单独行显示
            this.Pub1.AddTR1();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("呈现方式"); //呈现方式;
            this.Pub1.AddTDBegin();
            rb = new RadioButton();
            rb.ID = "RB_UIIsLine_0";
            rb.Text = "两列显示"; // 两行
            rb.GroupName = "sa";
            if (mapAttr.UIIsLine)
                rb.Checked = false;
            else
                rb.Checked = true;

            this.Pub1.Add(rb);
            rb = new RadioButton();
            rb.ID = "RB_UIIsLine_1";
            rb.Text = "整行显示"; // "一行";
            rb.GroupName = "sa";

            if (mapAttr.UIIsLine)
                rb.Checked = true;
            else
                rb.Checked = false;

            this.Pub1.Add(rb);
            this.Pub1.AddTDEnd();
            this.Pub1.AddTD("对傻瓜表单有效");

            //this.Pub1.AddTD("控制该它在表单的显示方式");
            this.Pub1.AddTREnd();
            #endregion 是否可编辑

            #region 字段分组
            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("字段分组");
            DDL ddlGroup = new DDL();
            ddlGroup.ID = "DDL_GroupID";
            GroupFields gfs = new GroupFields(mapAttr.FK_MapData);
            ddlGroup.Bind(gfs, GroupFieldAttr.OID, GroupFieldAttr.Lab);
            if (mapAttr.GroupID == 0)
                mapAttr.GroupID = this.GroupField;

            ddlGroup.SetSelectItem(mapAttr.GroupID);

            this.Pub1.AddTD("colspan=2", ddlGroup);
            this.Pub1.AddTD();  //( this.to "隶属分组");
            this.Pub1.AddTREnd();
            #endregion 字段分组

            #region 字段按钮
            this.Pub1.AddTRSum();
            this.Pub1.Add("<TD colspan=4>");
            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.Text = "保存";
            btn.Click += new EventHandler(btn_Save_Click);
            btn.CssClass = "Btn";
            this.Pub1.Add(btn);

            btn = new Button();
            btn.ID = "Btn_SaveAndClose1";
            btn.CssClass = "Btn";
            btn.Text = "关闭";
            btn.Attributes["onclick"] = " window.close(); return false;";
            this.Pub1.Add(btn);

            btn = new Button();
            btn.ID = "Btn_SaveAndClose";
            btn.CssClass = "Btn";
            btn.Text = "保存并关闭"; //"保存并关闭";
            btn.Click += new EventHandler(btn_Save_Click);
            this.Pub1.Add(btn);

            btn = new Button();
            btn.ID = "Btn_SaveAndNew";
            btn.CssClass = "Btn";
            btn.Text = "保存并新建";
            btn.Click += new EventHandler(btn_Save_Click);
            this.Pub1.Add(btn);

            if (this.RefNo != null)
            {
                btn = new Button();
                btn.ID = "Btn_AutoFull";
                btn.CssClass = "Btn";
                btn.Text = "扩展设置";
                //  btn.Click += new EventHandler(btn_Save_Click);
                btn.Attributes["onclick"] = "javascript:WinOpen('AutoFull.aspx?RefNo=" + this.RefNo + "&FK_MapData=" + mapAttr.FK_MapData + "',''); return false;";
                this.Pub1.Add(btn);

                if (mapAttr.HisEditType == EditType.Edit)
                {
                    btn = new Button();
                    btn.ID = "Btn_Del";
                    btn.CssClass = "Btn";
                    btn.Text = "删除";
                    btn.Click += new EventHandler(btn_Save_Click);
                    btn.Attributes["onclick"] = " return confirm('您确认吗？');";
                    this.Pub1.Add(btn);
                }
                string myUrl = "EleBatch.aspx?KeyOfEn=" + mapAttr.KeyOfEn + "&FK_MapData=" + mapAttr.FK_MapData + "&EleType=MapAttr";
                this.Pub1.Add("<a href='" + myUrl + "' target='M" + mapAttr.KeyOfEn + "' ><img src='../Img/Btn/Apply.gif' border=0>批处理</a>");
            }

            string url = "Do.aspx?DoType=AddF&MyPK=" + mapAttr.FK_MapData + "&IDX=" + mapAttr.IDX;
            this.Pub1.Add("<a href='" + url + "'><img src='../Img/Btn/New.gif' border=0>新建</a></TD>");
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();
            #endregion 字段按钮
        }
        void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                switch (btn.ID)
                {
                    case "Btn_Del":
                        this.Response.Redirect("Do.aspx?DoType=Del&MyPK=" + this.MyPK + "&RefNo=" + this.RefNo, true);
                        return;
                    default:
                        break;
                }

                MapAttr attr = new MapAttr();
                if (this.RefNo == null || this.RefNo == "")
                {
                    attr.MyPK = this.MyPK + "_" + this.Pub1.GetTBByID("TB_KeyOfEn").Text;
                    attr.UIContralType = UIContralType.DDL;
                    attr.MyDataType = BP.DA.DataType.AppString;
                    attr.LGType = FieldTypeS.FK;
                    attr.DefVal = "";
                    attr.UIBindKey = this.Request.QueryString["SFKey"];
                    attr.UIIsEnable = true;
                }
                else
                {
                    attr.MyPK = this.RefNo;
                    attr.Retrieve();
                }
                attr = (MapAttr)this.Pub1.Copy(attr);
                attr.FK_MapData = this.MyPK;
                attr.GroupID = this.Pub1.GetDDLByID("DDL_GroupID").SelectedItemIntVal;
                attr.DefVal = this.Pub1.GetTBByID("TB_DefVal").Text;

                //if (this.Pub1.IsExit("CB_IsDefValNull"))
                //{
                //    if (this.Pub1.GetCBByID("CB_IsDefValNull").Checked == false)
                //        attr.DefVal = this.Pub1.GetDDLByID("DDL").SelectedItemStringVal;
                //    else
                //        attr.DefVal = "";
                //}
                //else
                //{
                //    string s = this.Pub1.GetDDLByID("DDL_DefVal").SelectedItemStringVal;
                //    if (s == "@Select")
                //    {
                //        attr.DefVal = this.Pub1.GetDDLByID("DDL").SelectedItemStringVal;
                //    }
                //    else
                //    {
                //        attr.DefVal = s;
                //    }
                //}

                if (this.RefNo == null || this.RefNo == "")
                    attr.Insert();
                else
                    attr.Update();

                switch (btn.ID)
                {
                    case "Btn_SaveAndClose":
                        this.WinClose();
                        return;
                    case "Btn_SaveAndNew":
                        this.Response.Redirect("Do.aspx?DoType=AddF&MyPK=" + this.MyPK + "&IDX=" + attr.IDX + "&GroupField=" + this.GroupField, true);
                        return;
                    default:
                        break;
                }
                this.Response.Redirect("EditTable.aspx?DoType=Edit&MyPK=" + this.MyPK + "&RefNo=" + attr.MyPK + "&GroupField=" + this.GroupField, true);
            }
            catch (Exception ex)
            {
                this.Alert(ex.Message);
            }
        }
        public string GetCaption
        {
            get
            {
                if (this.DoType == "Add")
                    return "增加新字段向导  - <a href='Do.aspx?DoType=ChoseFType&GroupField=" + this.GroupField + "'>选择类型</a> -" + "编辑字段";
                else
                    return "编辑字段"; // "编辑字段";
            }
        }
    }

}