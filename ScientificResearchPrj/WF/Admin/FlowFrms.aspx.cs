using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Web.Controls;
using BP.WF;
using BP.WF.Template;
using BP.En;
using BP.DA;
using BP.Sys;
using BP;
namespace CCFlow.WF.Admin
{
    public partial class WF_Admin_FlowFrms : BP.Web.WebPage
    {
        #region 属性.
        /// <summary>
        /// 显示类型
        /// </summary>
        public string ShowType
        {
            get
            {
                string s = this.Request.QueryString["ShowType"];
                if (s == null)
                    return "FrmLib";
                return s;
            }
        }
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        /// <summary>
        /// 节点
        /// </summary>
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
                     return int.Parse(this.Request.QueryString["FK_Flow"]);
                }
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            #region 执行功能.
            if (this.IsPostBack==false)
            {
                switch (this.DoType)
                {
                    case "Up":
                        FrmNode fnU = new FrmNode(this.MyPK);
                        fnU.DoUp();
                        break;
                    case "Down":
                        FrmNode fnD = new FrmNode(this.MyPK);
                        fnD.DoDown();
                        break;
                    case "DelFrm":
                        FrmNodes fnsR = new FrmNodes();
                        if (fnsR.Retrieve(FrmNodeAttr.FK_Frm, this.FK_MapData) != 0)
                        {
                            this.Alert("此表单已经被多个流程节点(" + fnsR.Count + ")绑定，所以您不能删除它。");
                        }
                        else
                        {
                            MapData md = new MapData();
                            md.No = this.FK_MapData;
                            md.Delete();
                        }
                        break;
                    case "Del":
                        FrmNodes fns = new FrmNodes(this.FK_Flow, this.FK_Node);
                        foreach (FrmNode fn in fns)
                        {
                            if (fn.FK_Frm == this.FK_MapData)
                            {
                                fn.Delete();
                                break;
                            }
                        }
                        break;
                    case "Add":
                        FrmNode fnN = new FrmNode();
                        fnN.FK_Frm = this.FK_MapData;
                        fnN.FK_Node = this.FK_Node;
                        fnN.FK_Flow = this.FK_Flow;
                        fnN.Save();
                        break;
                    default:
                        break;
                }
            }
            #endregion 执行功能.

            switch (this.ShowType)
            {
                case "Frm":
                    this.BindFrm();
                    this.Title = "表单";
                    break;
                case "FrmLib":
                case "FrmLab":
                    this.BindFrmLib();
                    this.Title = "表单库";
                    break;
                case "FlowFrms":
                    this.BindFlowFrms();
                    this.Title = "流程表单";
                    break;
                case "FrmSorts":
                    this.BindFrmSorts();
                    this.Title = "流程类别";
                    break;
                case "EditPowerOrder": //编辑权限与顺序.
                    this.BindEditPowerOrder();
                    break;
                default:
                    break;
            }

            this.BindLeft();
        }
        /// <summary>
        /// 编辑权限与顺序
        /// </summary>
        public void BindEditPowerOrder()
        {
            this.Pub1.AddH2("表单权限与显示顺序");
            this.Pub1.AddHR();
            this.Pub1.AddTable("align=left");
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("IDX");
            this.Pub1.AddTDTitle("编号");
            this.Pub1.AddTDTitle("名称");
            this.Pub1.AddTDTitle("显示方式");
            this.Pub1.AddTDTitle("是否可编辑？");
            this.Pub1.AddTDTitle("是否可打印");
           // this.Pub1.AddTDTitle("表单元素权限控制方案");
            this.Pub1.AddTDTitle("权限控制方案");

            this.Pub1.AddTDTitle("自定义方案设置");
            this.Pub1.AddTDTitle("谁是主键？");
            this.Pub1.AddTDTitle("顺序");
            this.Pub1.AddTDTitle("");
            this.Pub1.AddTDTitle("");
            this.Pub1.AddTREnd();

            FrmNodes fns = new FrmNodes(this.FK_Flow, this.FK_Node);
            int idx = 1;
            foreach (FrmNode fn in fns)
            {
                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(idx++);
                this.Pub1.AddTD(fn.FK_Frm);

                MapData md = new MapData(fn.FK_Frm);
                md = new MapData(fn.FK_Frm);
                // this.Pub1.AddTD(md.Name);
                this.Pub1.AddTDA("FlowFrms.aspx?ShowType=Frm&FK_MapData=" + md.No + "&FK_Node=" + this.FK_Node, md.Name);

                DDL ddl = new DDL();
                ddl.ID = "DDL_FrmType_" + fn.FK_Frm;
                ddl.BindSysEnum("FrmType", (int)fn.HisFrmType);
              //  ddl.Enabled = false;
                this.Pub1.AddTD(ddl);


                //if (md.HisFrmType == FrmType.ExcelFrm || md.HisFrmType == FrmType.WordFrm)
                //{
                //    /* 如果是 office 表单，就让其选择 toolbar 的控制方案.
                //     */
                //    ddl = new DDL();
                //    ddl.ID = "DDL_ToolbarSln_" + md.No;
                //    ddl.Items.Add(new ListItem("默认方案", "0"));
                //    ddl.Items.Add(new ListItem("自定义", this.FK_Node.ToString()));
                //    ddl.SetSelectItem(fn.FrmSln); //设置权限控制方案.
                //    this.Pub1.AddTD(ddl);
                //    this.Pub1.AddTD("<a href=\"javascript:WinField('" + md.No + "','" + this.FK_Node + "','" + this.FK_Flow + "')\" >字段</a>|<a href=\"javascript:WinFJ('" + md.No + "','" + this.FK_Node + "','" + this.FK_Flow + "')\" >附件</a>");

                //}
                //else
                //{
                    CheckBox cb = new CheckBox();
                    cb.ID = "CB_IsEdit_" + md.No;
                    cb.Text = "是否可编辑？";
                    cb.Checked = fn.IsEdit;
                    this.Pub1.AddTD(cb);

                    cb = new CheckBox();
                    cb.ID = "CB_IsPrint_" + md.No;
                    cb.Text = "是否可打印";
                    cb.Checked = fn.IsPrint;
                    this.Pub1.AddTD(cb);
              //  }


                ddl = new DDL();
                ddl.ID = "DDL_Sln_" + md.No;
                //   ddl.BindAtParas(md.Slns);
                ddl.Items.Add(new ListItem("默认方案", "0"));
                ddl.Items.Add(new ListItem("自定义", this.FK_Node.ToString()));
                ddl.SetSelectItem(fn.FrmSln); //设置权限控制方案.
                this.Pub1.AddTD(ddl);

                this.Pub1.AddTDBegin();
                this.Pub1.Add("<a href=\"javascript:WinField('" + md.No + "','" + this.FK_Node + "','" + this.FK_Flow + "')\" >字段</a>");
                this.Pub1.Add("-<a href=\"javascript:WinFJ('" + md.No + "','" + this.FK_Node + "','" + this.FK_Flow + "')\" >附件</a>");

                if (md.HisFrmType == FrmType.ExcelFrm )
                    this.Pub1.Add("-<a href=\"javascript:ToolbarExcel('" + md.No + "','" + this.FK_Node + "','" + this.FK_Flow + "')\" >ToolbarExcel</a>");

                if (md.HisFrmType == FrmType.WordFrm)
                    this.Pub1.Add("-<a href=\"javascript:ToolbarWord('" + md.No + "','" + this.FK_Node + "','" + this.FK_Flow + "')\" >ToolbarWord</a>");

                this.Pub1.AddTDEnd();

                ddl = new DDL();
                ddl.ID = "DDL_WhoIsPK_" + md.No;
                ddl.BindSysEnum("WhoIsPK");
                ddl.SetSelectItem( (int)fn.WhoIsPK ); //谁是主键？.
                this.Pub1.AddTD(ddl);

                TextBox tb = new TextBox();
                tb.ID = "TB_Idx_" + md.No;
                tb.Text = fn.Idx.ToString();
                tb.Columns = 5;
                this.Pub1.AddTD(tb);

                this.Pub1.AddTDA("FlowFrms.aspx?ShowType=EditPowerOrder&FK_Node=" + this.FK_Node + "&FK_Flow="+this.FK_Flow+"&MyPK=" + fn.MyPK + "&DoType=Up", "上移");
                this.Pub1.AddTDA("FlowFrms.aspx?ShowType=EditPowerOrder&FK_Node=" + this.FK_Node + "&FK_Flow=" + this.FK_Flow + "&MyPK=" + fn.MyPK + "&DoType=Down", "下移");

                this.Pub1.AddTREnd();
            }

            this.Pub1.AddTR();
            Button btn = new Button();
            btn.ID = "Save";
            btn.Text = "  Save  ";
            btn.CssClass = "Btn";
            btn.Click += new EventHandler(btn_SavePowerOrders_Click);
            this.Pub1.AddTD("colspan=12", btn);
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();
        }

        void btn_SavePowerOrders_Click(object sender, EventArgs e)
        {
            FrmNodes fns = new FrmNodes(this.FK_Flow, this.FK_Node);
            foreach (FrmNode fn in fns)
            {
                fn.IsEdit = this.Pub1.GetCBByID("CB_IsEdit_" + fn.FK_Frm).Checked;
                fn.IsPrint = this.Pub1.GetCBByID("CB_IsPrint_" + fn.FK_Frm).Checked;
                fn.Idx = int.Parse(this.Pub1.GetTextBoxByID("TB_Idx_" + fn.FK_Frm).Text);
                fn.HisFrmType = (BP.Sys.FrmType)this.Pub1.GetDDLByID("DDL_FrmType_" + fn.FK_Frm).SelectedItemIntVal;

                //权限控制方案.
                fn.FrmSln = this.Pub1.GetDDLByID("DDL_Sln_" + fn.FK_Frm).SelectedItemIntVal;
                fn.WhoIsPK = (WhoIsPK)this.Pub1.GetDDLByID("DDL_WhoIsPK_" + fn.FK_Frm).SelectedItemIntVal;

                fn.FK_Flow = this.FK_Flow;
                fn.FK_Node = this.FK_Node;
                //fn.FK_Frm = 

                fn.MyPK = fn.FK_Frm + "_" + fn.FK_Node + "_" + fn.FK_Flow;

                fn.Update();
            }
            this.Response.Redirect("FlowFrms.aspx?ShowType=EditPowerOrder&FK_Node=" + this.FK_Node+"&FK_Flow="+this.FK_Flow, true);
        }
        public void BindFrmSorts()
        {
            SysFormTrees fss = new SysFormTrees();
            fss.RetrieveAll();
            this.Pub1.AddH2("表单类别维护");
            this.Pub1.AddHR();

            this.Pub1.AddTable("align=left");
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("序号");
            this.Pub1.AddTDTitle("类别编号");
            this.Pub1.AddTDTitle("类别名称");
            this.Pub1.AddTREnd();

            for (int i = 1; i <= 15; i++)
            {
                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(i);


                TextBox tb = new TextBox();
                tb.Text = i.ToString().PadLeft(2, '0');
                SysFormTree fs = fss.GetEntityByKey(SysFormTreeAttr.No, tb.Text) as SysFormTree;

                tb.ID = "TB_No_" + i;
                tb.Columns = 5;
                tb.ReadOnly = true;
                this.Pub1.AddTD(tb.Text);

                tb = new TextBox();
                tb.ID = "TB_Name_" + i;
                tb.Columns = 40;
                if (fs != null)
                    tb.Text = fs.Name;

                this.Pub1.AddTD(tb);
                this.Pub1.AddTREnd();
            }

            Button btn = new Button();
            btn.Text = "Save";
            btn.CssClass = "Btn";
            btn.Click += new EventHandler(btn_SaveFrmSort_Click);
            this.Pub1.Add(btn);

            this.Pub1.AddTR();
            this.Pub1.AddTD("colspan=2", btn);
            this.Pub1.AddTD("要删除类别，请把文本框数据清空保存即可。");
            this.Pub1.AddTREnd();

            this.Pub1.AddTableEndWithHR();

        }

        void btn_SaveFrmSort_Click(object sender, EventArgs e)
        {
            for (int i = 1; i <= 15; i++)
            {
                TextBox tbName = this.Pub1.GetTextBoxByID("TB_Name_" + i);
                SysFormTree fs = new SysFormTree();
                fs.No = i.ToString().PadLeft(2, '0');
                fs.Name = tbName.Text.ToString();
                if (fs.Name.Length > 1)
                    fs.Save();
                else
                    fs.Delete();
            }
            this.Alert("保存成功");
        }
        public void BindFlowFrms()
        {
            FrmNodes fns = new FrmNodes(this.FK_Flow, this.FK_Node);
            this.Pub1.AddH2("流程表单绑定");
            this.Pub1.AddTable("align=left");
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("Idx");
            this.Pub1.AddTDTitle("表单编号");
            this.Pub1.AddTDTitle("名称");
            this.Pub1.AddTDTitle("物理表");
             this.Pub1.AddTDTitle("权限");
            this.Pub1.AddTREnd();

            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);

            MapDatas mds = new MapDatas();
            QueryObject obj_mds = new QueryObject(mds);
            obj_mds.AddWhere(MapDataAttr.AppType, (int)AppType.Application);
            obj_mds.addOrderBy(MapDataAttr.Name);
            obj_mds.DoQuery();
            //FrmSorts fss = new FrmSorts();
            //fss.RetrieveAll();
            SysFormTrees formTrees = new SysFormTrees();
            QueryObject objInfo = new QueryObject(formTrees);
            objInfo.AddWhere(SysFormTreeAttr.ParentNo,"0");
            objInfo.addOrderBy(SysFormTreeAttr.Name);
            objInfo.DoQuery();

            int idx = 0;
            foreach (SysFormTree fs in formTrees)
            {
                idx++;
                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(idx);
                this.Pub1.AddTDB("colspan=4", fs.Name);
                this.Pub1.AddTREnd();
                foreach (MapData md in mds)
                {
                    if (md.FK_FormTree != fs.No)
                        continue;
                    idx++;
                    this.Pub1.AddTR();
                    this.Pub1.AddTDIdx(idx);

                    CheckBox cb = new CheckBox();
                    cb.ID = "CB_" + md.No;
                    cb.Text = md.No;
                    cb.Checked = fns.Contains(FrmNodeAttr.FK_Frm, md.No);

                    this.Pub1.AddTD(cb);
                    this.Pub1.AddTD("<a href='../MapDef/CCForm/Frm.aspx?FK_MapData=" + md.No + "&FK_Flow=" + this.FK_Flow + "'  target=_blank>" + md.Name + "</a>");
                    this.Pub1.AddTD(md.PTable);

                    if (cb.Checked)
                        this.Pub1.AddTD("<a href=\"javascript:WinField('" + md.No + "','" + this.FK_Node + "','" + this.FK_Flow + "')\">字段</a>|<a href=\"javascript:WinFJ('" + md.No + "','" + this.FK_Node + "','" + this.FK_Flow + "')\">附件</a>");
                    else
                        this.Pub1.AddTD();
                    //this.Pub1.AddTD(md.Designer);
                    //this.Pub1.AddTD(md.DesignerUnit);
                    //this.Pub1.AddTD(md.DesignerContact);
                    this.Pub1.AddTREnd();
                }
                AddChildNode(fs.No, mds, fns);
            }
            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.Text = "Save";
            btn.CssClass = "Btn";
            btn.Click += new EventHandler(btn_SaveFlowFrms_Click);
            this.Pub1.AddTR();
            this.Pub1.AddTD("colspan=5", btn);
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();
        }

        private void AddChildNode(string parentNo, MapDatas mds, FrmNodes fns)
        {
            SysFormTrees formTrees = new SysFormTrees();
            QueryObject objInfo = new QueryObject(formTrees);
            objInfo.AddWhere(SysFormTreeAttr.ParentNo, parentNo);
            objInfo.addOrderBy(SysFormTreeAttr.Name);
            objInfo.DoQuery();

            int idx = 0;
            foreach (SysFormTree fs in formTrees)
            {
                idx++;
                foreach (MapData md in mds)
                {
                    if (md.FK_FormTree != fs.No)
                        continue;
                    idx++;
                    this.Pub1.AddTR();
                    this.Pub1.AddTDIdx(idx);

                    CheckBox cb = new CheckBox();
                    cb.ID = "CB_" + md.No;
                    cb.Text = md.No;
                    cb.Checked = fns.Contains(FrmNodeAttr.FK_Frm, md.No);

                    this.Pub1.AddTD(cb);
                    this.Pub1.AddTD(md.Name);
                    this.Pub1.AddTD(md.PTable);
                    this.Pub1.AddTREnd();
                }
                AddChildNode(fs.No, mds, fns);
            }
        }
        void btn_SaveFlowFrms_Click(object sender, EventArgs e)
        {
            FrmNodes fns = new FrmNodes(this.FK_Flow, this.FK_Node);
            MapDatas mds = new MapDatas();
            mds.Retrieve(MapDataAttr.AppType, (int)AppType.Application);

            //BP.WF.Node nd = new BP.WF.Node(this.FK_Node);

            string ids = ",";
            foreach (MapData md in mds)
            {
                CheckBox cb = this.Pub1.GetCBByID("CB_" + md.No);
                if (cb == null || cb.Checked == false)
                    continue;
                ids += md.No + ",";
            }

            //删除已经删除的。
            foreach (FrmNode fn in fns)
            {
                if (ids.Contains("," + fn.FK_Frm + ",") == false)
                {
                    fn.Delete();
                    continue;
                }
            }

            // 增加集合中没有的。
            string[] strs = ids.Split(',');
            foreach (string s in strs)
            {
                if (string.IsNullOrEmpty(s))
                    continue;
                if (fns.Contains(FrmNodeAttr.FK_Frm, s))
                    continue;

                FrmNode fn = new FrmNode();
                fn.FK_Frm = s;
                fn.FK_Flow = this.FK_Flow;
                fn.FK_Node = this.FK_Node;
                fn.Save();
            }
            this.Response.Redirect("FlowFrms.aspx?ShowType=EditPowerOrder&FK_Node=" + this.FK_Node+"&FK_Flow="+this.FK_Flow, true);
        }
        public void BindFrmLib()
        {
            this.Pub1.AddH2("表单库");

            this.Pub1.AddTable("width=100% align=left");
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("Idx");
            this.Pub1.AddTDTitle("表单编号");
            this.Pub1.AddTDTitle("名称");
            this.Pub1.AddTDTitle("物理表");

            //this.Pub1.AddTDTitle("设计者");
            //this.Pub1.AddTDTitle("设计单位");
            //this.Pub1.AddTDTitle("联系方式");
            this.Pub1.AddTREnd();

            MapDatas mds = new MapDatas();
            mds.Retrieve(MapDataAttr.AppType, (int)AppType.Application);

            SysFormTrees fss = new SysFormTrees();
            fss.RetrieveAll();
            int idx = 0;
            foreach (SysFormTree fs in fss)
            {
                idx++;
                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(idx);
                this.Pub1.AddTD("colspan=6", "<b>" + fs.Name + "</b>");
                this.Pub1.AddTREnd();
                foreach (MapData md in mds)
                {
                    if (md.FK_FrmSort != fs.No)
                        continue;
                    idx++;
                    this.Pub1.AddTR();
                    this.Pub1.AddTDIdx(idx);
                    this.Pub1.AddTD(md.No);
                    this.Pub1.AddTDA("FlowFrms.aspx?ShowType=Frm&FK_MapData=" + md.No + "&FK_Node=" + this.FK_Node+"&FK_Flow="+this.FK_Flow, md.Name);
                    this.Pub1.AddTD(md.PTable);
                    //this.Pub1.AddTD(md.Designer);
                    //this.Pub1.AddTD(md.DesignerUnit);
                    //this.Pub1.AddTD(md.DesignerContact);
                    this.Pub1.AddTREnd();

                    //this.Pub1.AddTR();
                    //this.Pub1.AddTD();
                    //this.Pub1.AddTD();
                    //this.Pub1.AddTDBegin("colspan=5");
                    //this.Pub1.AddTDEnd();
                    //this.Pub1.AddTREnd();
                }
            }
            this.Pub1.AddTableEnd();
        }
        public void BindFrm()
        {
            MapData md = new MapData();
            if (string.IsNullOrEmpty(this.FK_MapData) == false)
            {
                md.No = this.FK_MapData;
                md.RetrieveFromDBSources();
                this.Pub1.AddH2("表单属性" + md.Name);
                this.Pub1.AddHR();
            }
            else
            {
                this.Pub1.AddH2("新建表单");
                this.Pub1.AddHR();
            }

            this.Pub1.AddTable("align=left");
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("属性");
            this.Pub1.AddTDTitle("采集");
            this.Pub1.AddTDTitle("描述");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("表单名称");
            TextBox tb = new TextBox();
            tb.ID = "TB_Name";
            tb.Text = md.Name;
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("描述");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("表单编号");
            tb = new TextBox();
            tb.ID = "TB_No";
            tb.Text = md.No;
            if (string.IsNullOrEmpty(md.No) == false)
                tb.Attributes["readonly"] = "true";

            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("也是表单ID.");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("表单类型");
            DDL ddl = new DDL();
            ddl.ID = "DDL_FrmType";
            ddl.BindSysEnum(MapDataAttr.FrmType, (int)md.HisFrmType);
            this.Pub1.AddTD(ddl);
            this.Pub1.AddTD("");
            this.Pub1.AddTREnd();


            this.Pub1.AddTR();
            this.Pub1.AddTD("物理表/视图");
            tb = new TextBox();
            tb.ID = "TB_PTable";
            tb.Text = md.No;
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("多个表单可以对应同一个表或视图<br>如果表不存在,ccflow会自动创建.");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("类别");
            ddl = new DDL();
            ddl.ID = "DDL_FK_FrmSort";
            SysFormTrees fss = new SysFormTrees();
            fss.RetrieveAll();

            ddl.Bind(fss, md.FK_FrmSort);

            this.Pub1.AddTD(ddl);
            this.Pub1.AddTD("");
            this.Pub1.AddTREnd();

            //this.Pub1.AddTR();
            //this.Pub1.AddTD("设计单位");
            //tb = new TextBox();
            //tb.ID = "TB_" + MapDataAttr.DesignerUnit;
            //tb.Text = md.DesignerUnit;
            //if (string.IsNullOrEmpty(tb.Text))
            //    tb.Text = BP.Sys.SystemConfig.DeveloperName;

            //tb.Columns = 60;
            //this.Pub1.AddTD("colspan=2", tb);
            //this.Pub1.AddTREnd();

            //this.Pub1.AddTR();
            //this.Pub1.AddTD("联系方式");
            //tb = new TextBox();
            //tb.ID = "TB_" + MapDataAttr.DesignerContact;
            //tb.Text = md.DesignerContact;
            //if (string.IsNullOrEmpty(tb.Text))
            //    tb.Text = BP.Sys.SystemConfig.ServiceTel + "," + BP.Sys.SystemConfig.ServiceMail;
            //tb.Columns = 60;
            //this.Pub1.AddTD("colspan=2", tb);
            //this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("");
            this.Pub1.AddTDBegin();
            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.Text = "Save";
            btn.CssClass = "Btn";
            btn.Click += new EventHandler(btn_SaveFrm_Click);
            this.Pub1.Add(btn);

            if (string.IsNullOrEmpty(md.No) == false)
            {
                btn = new Button();
                btn.ID = "Btn_Delete";
                btn.Text = "Delete";
                btn.CssClass = "Btn";
                btn.Attributes["onclick"] = "return window.confirm('您确定要删除吗？')";
                btn.Click += new EventHandler(btn_SaveFrm_Click);
                this.Pub1.Add(btn);
            }

            this.Pub1.AddTDEnd();
            this.Pub1.AddTD("");
            this.Pub1.AddTREnd();

            if (string.IsNullOrEmpty(md.No) == false)
            {
                this.Pub1.AddTR();
                this.Pub1.AddTDBegin("colspan=3");
                //// this.Pub1.Add("<a href='FlowFrms.aspx?ShowType=FrmLib&DoType=DelFrm&FK_Node=" + FK_Node + "&FK_MapData=" + md.No + "'  ><img src='./Img/Btn/Delete.gif' border=0 />删除</a>");
                //this.Pub1.Add("<a href='../MapDef/ViewFrm.aspx?DoType=Column4Frm&FK_MapData=" + md.No + "&FK_Flow=" + this.FK_Flow + "' target=_blank  ><img src='../Img/Btn/View.gif' border=0 />傻瓜表单预览</a>");
                //this.Pub1.Add("<a href='../CCForm/Frm.aspx?FK_MapData=" + md.No + "&FK_Flow=" + this.FK_Flow + "&IsTest=1&WorkID=0' target=_blank  ><img src='../Img/Btn/View.gif' border=0 />自由表单预览</a>");
                //this.Pub1.Add("<a href='../MapDef/ViewFrm.aspx?DoType=dd&FK_MapData=" + md.No + "&FK_Flow=" + this.FK_Flow + "' target=_blank  ><img src='../Img/Btn/View.gif' border=0 />手机表单预览</a>");
                //this.Pub1.Add("<a href='../CCForm/Frm.aspx?FK_MapData=" + md.No + "&FK_Flow=" + this.FK_Flow + "' target=_blank  ><img src='../Img/Btn/View.gif' border=0 />启动自由表单设计器</a>");
                //this.Pub1.Add("<a href='../MapDef/MapDef.aspx?PK=" + md.No + "&FK_Flow=" + this.FK_Flow + "' target=_blank  ><img src='../Img/Btn/View.gif' border=0 />启动傻瓜表单设计器</a>");
                this.Pub1.Add("<a href='../CCForm/Frm.aspx?FK_MapData=" + md.No + "&FK_Flow=" + this.FK_Flow + "' target=_blank  ><img src='../Img/Btn/View.gif' border=0 />启动自由表单设计器</a>");
                this.Pub1.AddTDEnd();
                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTableEnd();
        }

        void btn_SaveFrm_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn.ID == "Btn_Delete")
            {
                //MapData mdDel = new MapData();
                //mdDel.No = this.FK_MapData;
                //mdDel.Delete();
                this.Response.Redirect("FlowFrms.aspx?ShowType=Frm&DoType=DelFrm&FK_Node=" + this.FK_Node + "&FK_MapData=" + this.FK_MapData + "&FK_Flow=" + this.FK_Flow, true);
                return;
            }
            MapData md = new MapData();
            if (string.IsNullOrEmpty(this.FK_MapData) == false)
            {
                md.No = this.FK_MapData;
                md.RetrieveFromDBSources();
            }
            md = (MapData)this.Pub1.Copy(md);

            md.HisFrmTypeInt = this.Pub1.GetDDLByID("DDL_" + MapDataAttr.FrmType).SelectedItemIntVal;
            md.FK_FrmSort = this.Pub1.GetDDLByID("DDL_" + MapDataAttr.FK_FrmSort).SelectedItemStringVal;
            md.HisAppType = AppType.Application;
            if (string.IsNullOrEmpty(this.FK_MapData) == true)
            {
                if (md.IsExits == true)
                {
                    this.Alert("表单编号(" + md.No + ")已存在");
                    return;
                }
                else
                {
                    md.Insert();
                    this.Response.Redirect("FlowFrms.aspx?ShowType=Frm&FK_Node=" + this.FK_Node + "&FK_MapData=" + md.No + "&FK_Flow=" + this.FK_Flow, true);
                }
            }
            else
            {
                md.Update();
                this.Alert("更新成功。");
            }
        }
        public void BindLeft()
        {
            this.Left.Add("<a href='http://ccflow.org' target=_blank ><img src='../../DataUser/ICON/" + SystemConfig.CompanyID + "/LogBiger.png' border=0/></a>");
            this.Left.AddHR();

            this.Left.AddUL();
            ////if (this.FK_Node == 0)
            ////{
            //    this.Left.AddLi("<a href=\"FlowFrms.aspx?ShowType=FrmLib&FK_Node=" + this.FK_Node + "&FK_MapData=" + this.FK_MapData + "&FK_Flow=" + this.FK_Flow + "\"><b>表单库</b></a>");
            //    this.Left.Add("查看，修改，设计，表单。<br><br>");

            //    this.Left.AddLi("<a href=\"FlowFrms.aspx?ShowType=FrmSorts&FK_Node=" + this.FK_Node + "&FK_MapData=" + this.FK_MapData + "&FK_Flow=" + this.FK_Flow + "\"><b>类别维护</b></a>");
            //    this.Left.Add("维护表单类别。<br><br>");

            //    this.Left.AddLi("<a href=\"FlowFrms.aspx?ShowType=Frm&FK_Node=" + this.FK_Node + "&FK_Flow=" + this.FK_Flow + "\"><b>新建表单</b></a>");
            //    this.Left.Add("新建表单。<br><br>");
            ////}
            //else
            //{
            this.Left.AddLi("<a href=\"FlowFrms.aspx?ShowType=FrmLib&FK_Node=" + this.FK_Node + "&FK_MapData=" + this.FK_MapData + "&FK_Flow=" + this.FK_Flow + "\"><b>表单库</b></a>-<a href=\"FlowFrms.aspx?ShowType=FrmSorts&FK_Node=" + this.FK_Node + "&FK_MapData=" + this.FK_MapData + "\"><b>类别维护</b></a>-<a href=\"FlowFrms.aspx?ShowType=Frm&FK_Node=" + this.FK_Node + "\"><b>新建表单</b></a>");
            this.Left.Add("表单库维护，类别维护，新建表单<br><br>");

            this.Left.AddLi("<a href=\"FlowFrms.aspx?ShowType=FlowFrms&FK_Node=" + this.FK_Node + "&FK_MapData=" + this.FK_MapData + "&FK_Flow=" + this.FK_Flow + "\"><b>增加移除流程表单绑定</b></a>");
            this.Left.Add("增加或移除查询结果集合中的列内容。<br><br>");

            this.Left.AddLi("<a href=\"FlowFrms.aspx?ShowType=EditPowerOrder&FK_Node=" + this.FK_Node + "&FK_MapData=" + this.FK_MapData + "&FK_Flow=" + this.FK_Flow + "\"><b>表单权限与显示顺序</b></a>");
            this.Left.Add("表单在该节点的权限与显示顺序控制。<br><br>");
            //}
            this.Left.AddULEnd();
        }
    }
}