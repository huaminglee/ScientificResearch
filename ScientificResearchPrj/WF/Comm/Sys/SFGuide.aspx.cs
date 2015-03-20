using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using System.Data;
using BP.Web.Controls;
using CCFlow.WF.Admin.UC;

namespace CCFlow.WF.Comm.Sys
{
    public partial class SFGuide : System.Web.UI.Page
    {
        public int Step
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["Step"]);
                }
                catch
                {
                    return 1;
                }
            }
        }
        public string FK_SFDBSrc
        {
            get
            {
                return this.Request.QueryString["FK_SFDBSrc"];
            }
        }

        /// <summary>
        /// 判断表符合规则的数据
        /// </summary>
        private Dictionary<int, string[]> regs = new Dictionary<int, string[]>
                                                     {
                                                         {0, new[] {"id", "no", "pk"}},
                                                         {1, new[] {"name", "title"}},
                                                         {2, new[] {"parentid", "parentno"}}
                                                     };

        protected void Page_Load(object sender, EventArgs e)
        {
            #region Step = 1

            if (this.Step == 1)
            {
                BP.Sys.SFDBSrcs ens = new BP.Sys.SFDBSrcs();
                ens.RetrieveAll();


                Pub1.AddTable("class='Table' cellSpacing='1' cellPadding='1'  border='1' style='width:100%'");
                Pub1.AddTR();
                Pub1.AddTDGroupTitle("", "第1步：请选择数据源");
                Pub1.AddTREnd();

                Pub1.AddTR();
                Pub1.AddTDBegin();
                Pub1.AddUL("class='navlist'");

                foreach (BP.Sys.SFDBSrc item in ens)
                {
                    Pub1.AddLi("<div><a href='SFGuide.aspx?Step=2&FK_SFDBSrc=" + item.No + "'><span class='nav'>" + item.No + "  -  " + item.Name + "</span></a></div>");
                }

                Pub1.AddLi("<div><a href=\"javascript:WinOpen('../RefFunc/UIEn.aspx?EnsName=BP.Sys.SFDBSrcs')\" ><img src='../../Img/New.gif' align='middle' /><span class='nav'>新建数据源</span></a></div>");
                Pub1.AddLi("<div><a href='SFGuide.aspx?Step=12&FK_SFDBsrc=local'><img src='../../Img/New.gif' align='middle' /><span>创建本地编码字典表</span></a></div>");
                Pub1.AddULEnd();
                Pub1.AddTDEnd();
                Pub1.AddTREnd();
                Pub1.AddTableEnd();
            }
            #endregion

            #region Step = 2

            if (this.Step == 2)
            {
                SFDBSrc src = new SFDBSrc(this.FK_SFDBSrc);

                Pub1.Add("<div class='easyui-layout' data-options=\"fit:true\">");
                Pub1.Add(string.Format("<div data-options=\"region:'west',split:true,title:'选择 {0} 表/视图'\" style='width:200px;'>",
                                       src.No));

                var lb = new LB();
                lb.ID = "LB_Table";
                lb.BindByTableNoName(src.GetTables());
                lb.Style.Add("width", "100%");
                lb.Style.Add("height", "100%");
                lb.AutoPostBack = true;
                lb.SelectedIndexChanged += new EventHandler(lb_SelectedIndexChanged);
                Pub1.Add(lb);

                Pub1.AddDivEnd();

                Pub1.Add("<div data-options=\"region:'center',title:'第2步：请填写基础信息'\" style='padding:5px;'>");
                Pub1.AddTable("class='Table' cellSpacing='1' cellPadding='1'  border='1' style='width:100%'");

                var islocal = (src.DBSrcType == DBSrcType.Localhost).ToString().ToLower();

                Pub1.AddTR();
                Pub1.AddTDGroupTitle("style='width:100px'", "值(编号)：");
                var ddl = new DDL();
                ddl.ID = "DDL_ColValue";
                ddl.Attributes.Add("onchange", string.Format("generateSQL('{0}','{1}',{2})", src.No, src.DBName, islocal));
                Pub1.AddTDBegin();
                Pub1.Add(ddl);
                Pub1.Add("&nbsp;编号列，比如：类别编号");
                Pub1.AddTDEnd();
                Pub1.AddTREnd();

                Pub1.AddTR();
                Pub1.AddTDGroupTitle("标签(名称)：");
                ddl = new DDL();
                ddl.ID = "DDL_ColText";
                ddl.Attributes.Add("onchange", string.Format("generateSQL('{0}','{1}',{2})", src.No, src.DBName, islocal));
                Pub1.AddTDBegin();
                Pub1.Add(ddl);
                Pub1.Add("&nbsp;显示的列，比如：类别名称");
                Pub1.AddTDEnd();
                Pub1.AddTREnd();

                Pub1.AddTR();
                Pub1.AddTDGroupTitle("父结点值(字段)：");
                ddl = new DDL();
                ddl.ID = "DDL_ColParentNo";
                ddl.Attributes.Add("onchange", string.Format("generateSQL('{0}','{1}',{2})", src.No, src.DBName, islocal));
                Pub1.AddTDBegin();
                Pub1.Add(ddl);
                Pub1.Add("&nbsp;如果是树类型实体，该列设置有效，比如：上级类别编号");
                Pub1.AddTDEnd();
                Pub1.AddTREnd();

                Pub1.AddTR();
                Pub1.AddTDGroupTitle("字典表类型：");

                ddl = new DDL();
                ddl.ID = "DDL_SFTableType";
                ddl.SelfBindSysEnum(SFTableAttr.SFTableType);
                ddl.Attributes.Add("onchange", string.Format("generateSQL('{0}','{1}',{2})", src.No, src.DBName, islocal));
                Pub1.AddTD(ddl);
                Pub1.AddTREnd();

                Pub1.AddTR();
                Pub1.AddTDGroupTitle("查询语句：");
                var tb = new TB();
                tb.ID = "TB_SelectStatement";
                tb.TextMode = TextBoxMode.MultiLine;
                tb.Columns = 60;
                tb.Rows = 10;
                tb.Style.Add("width", "99%");
                Pub1.AddTDBegin();
                Pub1.Add(tb);
                Pub1.Add("<br />&nbsp;说明：查询语句可以修改，但请保证查询语句的准确性及有效性！");
                Pub1.AddTDEnd();
                Pub1.AddTREnd();

                Pub1.AddTableEnd();
                Pub1.AddBR();
                Pub1.AddBR();
                Pub1.AddSpace(1);

                var btn = new LinkBtn(false, NamesOfBtn.Next, "下一步");
                btn.Click += new EventHandler(btn_Click);
                Pub1.Add(btn);
                Pub1.AddSpace(1);

                Pub1.Add("<a href='" + Request.UrlReferrer + "' class='easyui-linkbutton'>上一步</a>");

                Pub1.AddDivEnd();
                Pub1.AddDivEnd();

                if (!IsPostBack && lb.Items.Count > 0)
                {
                    lb.SelectedIndex = 0;
                    ShowSelectedTableColumns();
                }
            }
            #endregion

            #region Step = 12

            if (this.Step == 12)
            {
                Pub1.AddTable("class='Table' cellSpacing='1' cellPadding='1'  border='1' style='width:100%'");
                Pub1.AddTR();
                Pub1.AddTDGroupTitle("colspan='2'", "第2步：创建");
                Pub1.AddTREnd();

                TextBox tb = new TextBox();
                tb.ID = "TB_No";
                Pub1.AddTR();
                Pub1.AddTDGroupTitle("style='width:100px'", "表英文名称：");
                Pub1.AddTDBegin();
                Pub1.Add(tb);
                Pub1.Add("&nbsp;必须以字母或者下画线开头");
                Pub1.AddTDEnd();
                Pub1.AddTREnd();

                tb = new TextBox();
                tb.ID = "TB_" + SFTableAttr.Name;
                Pub1.AddTR();
                Pub1.AddTDGroupTitle("", "表中文名称：");
                Pub1.AddTDBegin();
                Pub1.Add(tb);
                Pub1.Add("&nbsp;显示的标签");
                Pub1.AddTDEnd();
                Pub1.AddTREnd();

                tb = new TextBox();
                tb.ID = "TB_" + SFTableAttr.TableDesc;
                Pub1.AddTR();
                Pub1.AddTDGroupTitle("", "描述：");
                Pub1.AddTDBegin();
                Pub1.Add(tb);
                Pub1.Add("&nbsp;表描述");
                Pub1.AddTDEnd();
                Pub1.AddTREnd();

                Pub1.AddTableEnd();
                Pub1.AddBR();
                Pub1.AddBR();
                Pub1.AddSpace(1);

                var btn = new LinkBtn(false, NamesOfBtn.Apply, "执行创建");
                btn.Click += new EventHandler(btn_Create_Local_Click);
                Pub1.Add(btn);
                Pub1.AddSpace(1);

                Pub1.Add("<a href='" + Request.UrlReferrer + "' class='easyui-linkbutton'>上一步</a>");
            }
            #endregion

            #region Step = 3

            if (this.Step == 3)
            {
                Pub1.AddTable("class='Table' cellSpacing='1' cellPadding='1'  border='1' style='width:100%'");
                Pub1.AddTR();
                Pub1.AddTDGroupTitle("colspan='2'", "第3步：创建");
                Pub1.AddTREnd();

                TextBox tb = new TextBox();
                tb.ID = "TB_No";
                Pub1.AddTR();
                Pub1.AddTDGroupTitle("style='width:100px'", "表英文名称：");
                Pub1.AddTDBegin();
                Pub1.Add(tb);
                Pub1.Add("&nbsp;必须以字母或者下画线开头");
                Pub1.AddTDEnd();
                Pub1.AddTREnd();

                tb = new TextBox();
                tb.ID = "TB_" + SFTableAttr.Name;
                Pub1.AddTR();
                Pub1.AddTDGroupTitle("", "表中文名称：");
                Pub1.AddTDBegin();
                Pub1.Add(tb);
                Pub1.Add("&nbsp;显示的标签");
                Pub1.AddTDEnd();
                Pub1.AddTREnd();

                tb = new TextBox();
                tb.ID = "TB_" + SFTableAttr.TableDesc;
                Pub1.AddTR();
                Pub1.AddTDGroupTitle("", "描述：");
                Pub1.AddTDBegin();
                Pub1.Add(tb);
                Pub1.Add("&nbsp;表描述");
                Pub1.AddTDEnd();
                Pub1.AddTREnd();

                Pub1.AddTableEnd();
                Pub1.AddBR();
                Pub1.AddBR();
                Pub1.AddSpace(1);

                var btn = new LinkBtn(false, NamesOfBtn.Apply, "执行创建");
                btn.Click += new EventHandler(btn_Create_Click);
                Pub1.Add(btn);
                Pub1.AddSpace(1);

                Pub1.Add("<a href='" + Request.UrlReferrer + "' class='easyui-linkbutton'>上一步</a>");
            }
            #endregion
        }

        void lb_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowSelectedTableColumns();
        }

        void btn_Create_Click(object sender, EventArgs e)
        {
            SFTable table = new SFTable();
            table = this.Pub1.Copy(table) as SFTable;
            table.FK_SFDBSrc = Request.QueryString["FK_SFDBSrc"];
            table.SrcTable = this.Request.QueryString["LB_Table"];
            table.FK_Val = "FK_" + table.SrcTable;
            table.ColumnText = this.Request.QueryString["DDL_ColText"];
            table.ColumnValue = this.Request.QueryString["DDL_ColValue"];
            table.ParentValue = this.Request.QueryString["DDL_ColParentNo"];
            table.SelectStatement = Uri.UnescapeDataString(Request.QueryString["TB_SelectStatement"]);
            table.SFTableType = int.Parse(Request.QueryString["DDL_SFTableType"]);

            if (table.SFTableType == 1)
            {
                table.IsTree = true;
            }
            else
            {
                table.IsTree = false;
                table.ParentValue = null;
            }

            if (BP.DA.DBAccess.IsExitsObject(table.No))
            {
                EasyUiHelper.AddEasyUiMessagerAndBack(this, "@对象（" + table.No + "）已经存在.", "错误", "error");
                return;
            }

            var sql = "CREATE VIEW " + table.No + ""
                      + " AS "
                      + table.SelectStatement;

            BP.DA.DBAccess.RunSQL(sql);

            table.Save();

            EasyUiHelper.AddEasyUiMessagerAndGo(this, "创建成功！查看数据……", "提示", "info", "../../MapDef/SFTableEditData.aspx?RefNo=" + table.No);
        }

        void btn_Create_Local_Click(object sender, EventArgs e)
        {
            SFTable table = new SFTable();
            table = this.Pub1.Copy(table) as SFTable;
            table.FK_SFDBSrc = Request.QueryString["FK_SFDBSrc"];
            table.SrcTable = table.No;
            table.ColumnText = "Name";
            table.ColumnValue = "No";
            table.IsTree = false;

            if (BP.DA.DBAccess.IsExitsObject(table.No))
            {
                //判断已经存在的表是否符合NoName规则，如果符合，则自动加入到SFTable中
                var src = new SFDBSrc(this.FK_SFDBSrc);
                var columns = src.GetColumns(table.No);
                var cols = new List<string>();

                foreach (DataRow dr in columns.Rows)
                    cols.Add(dr["name"].ToString());

                var regColValue = cols.FirstOrDefault(o => regs[0].Contains(o.ToLower()));
                var regColText = cols.FirstOrDefault(o => regs[1].Contains(o.ToLower()));
                var regColParentNo = cols.FirstOrDefault(o => regs[2].Contains(o.ToLower()));

                if(regColValue != null && regColText != null && regColParentNo != null)
                {
                    table.SFTableType = 1;
                    table.IsTree = true;
                    table.ColumnValue = regColValue;
                    table.ColumnText = regColText;
                    table.ParentValue = regColParentNo;
                    table.FK_SFDBSrc = "local";

                    table.Save(); 
                    EasyUiHelper.AddEasyUiMessagerAndGo(this, "您所创建的“" +table.No+ "” 名称的字典表，本地库已经存在，已成功注册！编辑数据……", "提示", "info", "../../MapDef/SFTableEditData.aspx?RefNo=" + table.No);
                }
                else if(regColValue != null && regColText != null)
                {
                    table.SFTableType = 0;
                    table.IsTree = false;
                    table.ColumnValue = regColValue;
                    table.ColumnText = regColText;
                    table.ParentValue = null;
                    table.FK_SFDBSrc = "local";

                    table.Save();
                    EasyUiHelper.AddEasyUiMessagerAndGo(this, "您所创建的“" + table.No + "” 名称的字典表，本地库已经存在，已成功注册！编辑数据……", "提示", "info", "../../MapDef/SFTableEditData.aspx?RefNo=" + table.No);
                }
                else
                {
                    EasyUiHelper.AddEasyUiMessagerAndBack(this, "@对象（" + table.No + "）已经存在.", "错误", "error");
                }

                return;
            }

            var sql = new StringBuilder();
            sql.AppendLine(string.Format("CREATE TABLE dbo.{0}", table.No));
            sql.AppendLine("(");
            sql.AppendLine("No    NVARCHAR(50) NOT NULL PRIMARY KEY,");
            sql.AppendLine("Name  NVARCHAR(100) NULL");
            sql.AppendLine(") ");

            BP.DA.DBAccess.RunSQL(sql.ToString());

            sql.Clear();
            sql.Append("INSERT INTO [dbo].[{0}] ([No], [Name]) VALUES ('0{1}', 'Item{1}')");

            for (var i = 1; i < 4; i++)
            {
                BP.DA.DBAccess.RunSQL(string.Format(sql.ToString(), table.No, i));
            }

            sql.Clear();
            sql.AppendFormat(
                "EXECUTE sp_addextendedproperty N'MS_Description', N'{0}', N'SCHEMA', N'dbo', N'TABLE', N'{1}', NULL, NULL",
                table.Name, table.No);

            BP.DA.DBAccess.RunSQL(sql.ToString());

            table.Save();

            EasyUiHelper.AddEasyUiMessagerAndGo(this, "创建成功！编辑数据……", "提示", "info", "../../MapDef/SFTableEditData.aspx?RefNo=" + table.No);
        }

        void btn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Pub1.GetTBByID("TB_SelectStatement").Text))
            {
                EasyUiHelper.AddEasyUiMessagerAndBack(this, "查询语句不能为空", "错误", "error");
                return;
            }

            string url = "SFGuide.aspx?Step=3&FK_SFDBSrc=" + this.FK_SFDBSrc + "&DDL_ColValue=" + this.Pub1.GetDDLByID("DDL_ColValue").SelectedItemStringVal;
            url += "&LB_Table=" + this.Pub1.GetLBByID("LB_Table").SelectedItemStringVal;
            url += "&DDL_ColText=" + this.Pub1.GetDDLByID("DDL_ColText").SelectedItemStringVal;
            url += "&DDL_ColParentNo=" + this.Pub1.GetDDLByID("DDL_ColParentNo").SelectedItemStringVal;
            url += "&TB_SelectStatement=" + Uri.EscapeDataString(Pub1.GetTBByID("TB_SelectStatement").Text);
            url += "&DDL_SFTableType=" + Pub1.GetDDLByID("DDL_SFTableType").SelectedItemStringVal;

            Response.Redirect(url, true);
        }

        /// <summary>
        /// 加载选中表的所有列信息
        /// </summary>
        private void ShowSelectedTableColumns()
        {
            var src = new SFDBSrc(this.FK_SFDBSrc);
            var colTables = src.GetColumns(this.Pub1.GetLBByID("LB_Table").SelectedItemStringVal);
            colTables.Columns.Add("text", typeof(string));

            var cols = new List<string>();
            string type;
            var length = 0;
            foreach (DataRow dr in colTables.Rows)
            {
                cols.Add(dr["name"].ToString());
                type = dr["type"].ToString().ToLower();
                length = int.Parse(dr["length"].ToString());

                dr["text"] = dr["name"] + " (" + (LengthTypes.Contains(type) ?
                    (string.Format("{0}{1}", type,
                    (length == -1 || length == 0) ?
                    (MaxTypes.Contains(type) ? "(max)" : "")
                      : string.Format("({0})", length))) : type) + ")";
            }

            //自动判断是否符合规则
            var regColValue = cols.FirstOrDefault(o => regs[0].Contains(o.ToLower()));
            var regColText = cols.FirstOrDefault(o => regs[1].Contains(o.ToLower()));
            var regColParentNo = cols.FirstOrDefault(o => regs[2].Contains(o.ToLower()));

            var ddl = this.Pub1.GetDDLByID("DDL_ColValue");
            ddl.Items.Clear();
            ddl.Bind(colTables, "name", "text");

            if (regColValue != null)
                ddl.SetSelectItem(regColValue);

            ddl = this.Pub1.GetDDLByID("DDL_ColText");
            ddl.Items.Clear();
            ddl.Bind(colTables, "name", "text");

            if (regColText != null)
                ddl.SetSelectItem(regColText);

            ddl = this.Pub1.GetDDLByID("DDL_ColParentNo");
            ddl.Items.Clear();
            ddl.Bind(colTables, "name", "text");

            if (regColParentNo != null)
                ddl.SetSelectItem(regColParentNo);

            Pub1.GetTBByID("TB_SelectStatement").Text = string.Empty;
            Pub1.GetDDLByID("DDL_SFTableType").SetSelectItem((regColValue != null && regColText != null &&
                                                              regColParentNo != null)
                                                                  ? "1"
                                                                  : "0");
        }

        private string[] LengthTypes = new[] { "char", "nchar", "varchar", "nvarchar", "varbinary" };
        private string[] MaxTypes = new[] { "nvarchar", "varbinary", "varchar" }; //如:nvarchar(max) 则maxLength为-1
    }
}