using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.DA;
using BP.Web.Controls;

namespace CCFlow.WF.MapDef
{
    public partial class ImpTableField : System.Web.UI.Page
    {
        #region 参数.
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
        public string FK_MapData
        {
            get
            {
                string str = this.Request.QueryString["FK_MapData"];
                if (string.IsNullOrEmpty(str))
                    return "abc";
                return str;
            }
        }
        public string FK_SFDBSrc
        {
            get
            {
                return this.Request.QueryString["FK_SFDBSrc"];
            }
        }

        public string STable
        {
            get
            {
                return this.Request.QueryString["STable"];
            }
        }

        public string SColumns
        {
            get { return this.Request.QueryString["SColumns"]; }
        }
        #endregion 参数.

        protected void Page_Load(object sender, EventArgs e)
        {
            #region 第1步.
            if (this.Step == 1)
            {
                BP.Sys.SFDBSrcs ens = new BP.Sys.SFDBSrcs();
                ens.RetrieveAll();


                Pub1.AddTable("class='Table' cellSpacing='0' cellPadding='0'  border='0' style='width:100%'");
                Pub1.AddTR();
                Pub1.AddTDGroupTitle("", "第1步：请选择数据源");
                Pub1.AddTREnd();

                Pub1.AddTR();
                Pub1.AddTDBegin();
                Pub1.AddUL("class='navlist'");

                foreach (BP.Sys.SFDBSrc item in ens)
                {

                    Pub1.AddLi("<div><a href='ImpTableField.aspx?Step=2&FK_MapData=" + this.FK_MapData + "&FK_SFDBSrc=" + item.No + "'><span class='nav'>" + item.No + "  -  " + item.Name + "</span></a></div>");
                }

                Pub1.AddLi("<div><a href=\"javascript:WinOpen('../RefFunc/UIEn.aspx?EnsName=BP.Sys.SFDBSrcs')\" ><img src='../Img/New.gif' align='middle' /><span class='nav'>新建数据源</span></a></div>");

                Pub1.AddULEnd();
                Pub1.AddTDEnd();
                Pub1.AddTREnd();
                Pub1.AddTableEnd();
            }
            #endregion 第1步.

            #region 第2步.
            if (this.Step == 2)
            {
                SFDBSrc src = new SFDBSrc(this.FK_SFDBSrc);

                Pub1.Add("<div class='easyui-layout' data-options=\"fit:true\">");
                Pub1.Add(string.Format("<div data-options=\"region:'west',split:true,title:'选择 {0} 数据表/视图'\" style='width:200px;'>",
                                       src.DBName));

                var lb = new LB();
                lb.ID = "LB_Table";
                lb.BindByTableNoName(src.GetTables());
                lb.Style.Add("width", "100%");
                lb.Style.Add("height", "100%");
                lb.AutoPostBack = false;
                lb.Attributes["onchange"] =
                    string.Format(
                        "javascript:self.location='{0}?Step={1}&FK_MapData={2}&FK_SFDBSrc={3}&STable=' + this.value",
                        Request.Url.AbsolutePath, this.Step, this.FK_MapData, this.FK_SFDBSrc);

                if (string.IsNullOrWhiteSpace(this.STable))
                {
                    lb.SelectedIndex = 0;
                }
                else
                {
                    lb.Items.FindByValue(this.STable).Selected = true;
                }

                Pub1.Add(lb);

                Pub1.AddDivEnd();

                Pub1.Add("<div data-options=\"region:'center',title:'第2步：请选择要导入的数据列'\" style='padding:5px;'>");

                //加载选中数据表/视图的列信息
                Pub1.AddTable("id='maintable' class='Table' cellSpacing='0' cellPadding='0'  border='0' style='width:100%'");
                Pub1.AddTR();


                var cb = new CheckBox();
                cb.ID = "CB_CheckAll";
                cb.Attributes["onclick"] = "CheckAll(this.checked)";

                Pub1.AddTDGroupTitle("style='width:40px;text-align:center'", cb);
                Pub1.AddTDGroupTitle("style='width:50px;text-align:center'", "序");
                Pub1.AddTDGroupTitle("字段名");
                Pub1.AddTDGroupTitle("中文描述");
                Pub1.AddTDGroupTitle("style='width:80px;text-align:center'", "类型");
                Pub1.AddTDGroupTitle("style='width:50px;text-align:center'", "最大长度");
                Pub1.AddTREnd();

                if (lb.SelectedItem != null)
                {
                    var tableColumns = src.GetColumns(lb.SelectedItem.Value);

                    foreach (DataRow dr in tableColumns.Rows)
                    {
                        cb = new CheckBox();
                        cb.ID = "CB_Col_" + dr["name"];
                        cb.Checked = this.SColumns == null ? false : this.SColumns.Contains(dr["name"] + ",");

                        Pub1.AddTR();
                        Pub1.AddTD(cb);
                        Pub1.AddTD(dr["colid"].ToString());
                        Pub1.AddTD(dr["name"].ToString());
                        Pub1.AddTD(dr["Desc"].ToString());
                        Pub1.AddTD(dr["type"].ToString());
                        Pub1.AddTD(Convert.ToInt32(dr["length"]));
                        Pub1.AddTREnd();
                    }
                }

                Pub1.AddTableEnd();
                Pub1.AddBR();
                Pub1.AddBR();
                Pub1.AddSpace(1);

                var btn = new LinkBtn(false, NamesOfBtn.Next, "下一步");
                btn.Click += new EventHandler(btn_Click);
                Pub1.Add(btn);
                Pub1.AddSpace(1);

                Pub1.Add(string.Format("<a href='{0}?Step=1&FK_MapData={1}' class='easyui-linkbutton'>上一步</a>", Request.Url.AbsolutePath, this.FK_MapData));
                Pub1.AddBR();
                Pub1.AddBR();
                Pub1.AddDivEnd();
                Pub1.AddDivEnd();
            }
            #endregion 第2步.

            #region 第3步.
            if (this.Step == 3)
            {
                SFDBSrc src = new SFDBSrc(this.FK_SFDBSrc);

                Pub1.AddTable("id='maintable' class='Table' cellSpacing='0' cellPadding='0' border='0' style='width:100%'");
                Pub1.AddTR();
                Pub1.AddTDGroupTitle("colspan='7'", "第3步：导入");
                Pub1.AddTREnd();

                Pub1.AddTDGroupTitle("style='width:50px;text-align:center'", "序");
                Pub1.AddTDGroupTitle("字段名");
                Pub1.AddTDGroupTitle("中文描述");
                Pub1.AddTDGroupTitle("style='width:80px;text-align:center'", "类型");
                Pub1.AddTDGroupTitle("style='width:80px;text-align:center'", "逻辑类型");
                Pub1.AddTDGroupTitle("style='width:50px;text-align:center'", "最大长度");
                Pub1.AddTDGroupTitle("style='width:140px;text-align:center'", "顺序操作");
                Pub1.AddTREnd();

                var tableColumns = src.GetColumns(this.STable);
                var i = 0;

                foreach (DataRow dr in tableColumns.Rows)
                {
                    if (!this.SColumns.Contains(dr["name"] + ","))
                        continue;

                    Pub1.AddTR();

                    Pub1.AddTDIdx((++i).ToString());
                    Pub1.AddTD(dr["name"].ToString());

                    var tb = new TB();
                    tb.ID = "TB_Desc_" + dr["name"];
                    tb.Style.Add("width", "99%");
                    tb.Text = dr["Desc"].ToString();
                    Pub1.AddTD(tb);

                    var lbl = new Label();
                    lbl.ID = "LBL_Type_" + dr["name"];
                    lbl.Text = dr["type"].ToString();
                    Pub1.AddTD("style='text-align:center'", lbl);

                    var ddl = new DDL();
                    //ddl.Attributes["class"] = "easyui-combobox";
                    //ddl.Style.Add("width", "60px");
                    ddl.ID = "DDL_LogicType_" + dr["name"];
                    ddl.SelfBindSysEnum(MapAttrAttr.LGType);
                    Pub1.AddTDBegin("style='text-align:center'");
                    Pub1.Add(ddl);
                    Pub1.AddTDEnd();

                    lbl = new Label();
                    lbl.ID = "LBL_Length_" + dr["name"];
                    lbl.Text = dr["length"].ToString();
                    Pub1.AddTD(lbl);

                    Pub1.AddTDBegin("style='text-align:center'");

                    var hiddenIdx = new HiddenField();
                    hiddenIdx.ID = "HID_Idx_" + dr["name"];
                    hiddenIdx.Value = i.ToString();
                    Pub1.Add(hiddenIdx);

                    Pub1.Add("<a href='javascript:void(0)' onclick='up(this, 6)' class='easyui-linkbutton' data-options=\"iconCls:'icon-up'\"></a>&nbsp;");
                    Pub1.Add("<a href='javascript:void(0)' onclick='down(this, 6)' class='easyui-linkbutton' data-options=\"iconCls:'icon-down'\"></a>");

                    Pub1.AddTDEnd();
                    Pub1.AddTREnd();
                }

                Pub1.AddTableEnd();
                Pub1.AddBR();
                Pub1.AddBR();
                Pub1.AddSpace(1);

                var btn = new LinkBtn(false, NamesOfBtn.Save, "导入字段，生成表单");
                btn.Click += new EventHandler(btn_Save_Click);
                Pub1.Add(btn);
                Pub1.AddSpace(1);

                Pub1.Add(string.Format("<a href='{0}' class='easyui-linkbutton'>上一步</a>", Request.Url.ToString().Replace("Step=3", "Step=2")));
                Pub1.AddBR();
                Pub1.AddBR();
            }
            #endregion 第3步.
        }

        void btn_Save_Click(object sender, EventArgs e)
        {
            var ts = new List<Tuple<string, string, int, int, int, int>>();
            var colname = string.Empty;

            Tuple<string, string, int, int, int, int> t = null;
            HiddenField hid = null;
            
            foreach (Control ctrl in Pub1.Controls)
            {
                if (ctrl.ID == null || !ctrl.ID.StartsWith("HID_Idx_")) continue;

                hid = ctrl as HiddenField;
                colname = hid.ID.Substring("HID_Idx_".Length);
                
                ts.Add(new Tuple<string, string, int, int, int, int>(
                           colname,
                           Pub1.GetTBByID("TB_Desc_" + colname).Text,
                           GetMyDataType(Pub1.GetLabelByID("LBL_Type_" + colname).Text),
                           int.Parse(Pub1.GetLabelByID("LBL_Length_" + colname).Text),
                           Pub1.GetDDLByID("DDL_LogicType_" + colname).SelectedItemIntVal,
                           int.Parse(hid.Value)
                           ));
            }

            ts.Sort((a,b)=>a.Item6.CompareTo(b.Item6));

            InitMapAttr(this.STable,
                        ts.Aggregate(string.Empty,
                                     (curr, next) =>
                                     curr + next.Item1 + "~" + next.Item2 + "~" + next.Item3 + "~" + next.Item4 + "~" +
                                     next.Item5 + "~" + next.Item6 + "^"));
        }

        /// <summary>
        /// 将SQL数据库字段类型转为系统类型
        /// </summary>
        /// <param name="sqlDataType">SQL数据库字段类型</param>
        /// <returns></returns>
        private int GetMyDataType(string sqlDataType)
        {
            switch (sqlDataType.ToLower())
            {
                case "tinyint":
                case "smallint":
                case "int":
                    return DataType.AppInt;
                case "money":
                case "smallmoney":
                    return DataType.AppMoney;
                case "float":
                case "decimal":
                case "bigint":
                case "real":
                    return DataType.AppDouble;
                case "bit":
                    return DataType.AppBoolean;
                case "datetime":
                case "smalldatetime":
                    return DataType.AppDateTime;
                case "date":
                    return DataType.AppDate;
                case "char":
                case "nchar":
                case "varchar":
                case "nvarchar":
                case "text":
                case "ntext":
                case "xml":
                    return DataType.AppString;
                //case "binary":
                //case "image":
                //case "timestamp":
                //case "uniqueidentifier":
                //case "varbinary":
                //case "variant":
                default:
                    return 0;   //系统中没有设置
            }
        }

        void btn_Click(object sender, EventArgs e)
        {
            var selectedColumns = string.Empty;

            foreach (Control ctrl in Pub1.Controls)
            {
                if (ctrl.GetType().Name != "CheckBox" || ctrl.ID == "CB_CheckAll" || !(ctrl as CheckBox).Checked)
                    continue;

                selectedColumns += ctrl.ID.Substring("CB_Col_".Length) + ",";
            }

            Response.Redirect(string.Format(
                        "{0}?Step=3&FK_MapData={1}&FK_SFDBSrc={2}&STable={3}&SColumns={4}",
                        Request.Url.AbsolutePath, this.FK_MapData, this.FK_SFDBSrc, this.STable ?? (Pub1.GetLBByID("LB_Table").SelectedItem.Value), selectedColumns));
        }

        /// <summary>
        /// 数据源
        /// </summary>
        /// <param name="attrs">数据字符串，规则如下:
        /// <para>1.每个字段间用 ^ 来分隔</para>
        /// <para>2.字段的信息间用 ~ 来分隔</para>
        /// <para>3.字段的信息分别为：英文名称，中文名称，数据类型，最大长度，逻辑类型，序号</para>
        /// </param>
        /// <param name="tableName">数据表名称</param>
        public void InitMapAttr(string tableName, string attrs)
        {
            Pub1.AddEasyUiPanelInfo("发送信息", attrs);
            return;
            //删除有可能存在的临时数据.
            string tempStr = tableName + "Tmp";

            MapAttr ma = new MapAttr();
            ma.Delete(MapAttrAttr.FK_MapData, tempStr);

            string[] strs = attrs.Split('^');
            foreach (string str in strs)
            {
                if (string.IsNullOrEmpty(str))
                    continue;

                string[] mystrs = str.Split('~');
                ma = new MapAttr();
                ma.KeyOfEn = mystrs[0];
                ma.Name = mystrs[1];
                ma.FK_MapData = tempStr;
                ma.MyDataType = int.Parse(mystrs[2]);
                ma.MaxLen = int.Parse(mystrs[3]);
                ma.LGType = (BP.En.FieldTypeS)int.Parse(mystrs[4]);
                ma.IDX = int.Parse(mystrs[5]);

                ma.MyPK = tempStr + "_" + ma.KeyOfEn;
                ma.Insert();
            }
        }
        /// <summary>
        /// 绑定集合.
        /// </summary>
        public void BindAttrs()
        {

        }
    }
}