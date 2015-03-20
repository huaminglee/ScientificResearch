using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using BP.Port;
using BP.Sys;
using BP.Web.Controls;
using BP.DA;
using BP.En;
using BP.Web;
using BP.WF.Template;
using BP.WF;
namespace CCFlow.WF.Admin.UC
{
    #region EasyUi帮助类

    /// <summary>
    /// EasyUi帮助类
    /// <para>added by liuxc. 2014-10-22</para>
    /// </summary>
    public class EasyUiHelper
    {
        /// <summary>
        /// 增加一个弹窗显示信息
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="msg"></param>
        /// <param name="title"></param>
        /// <param name="autoCloseMillionSeconds"></param>
        public static void AddEasyUiMessager(Control ctrl, string msg, string title = "提示", int autoCloseMillionSeconds = 2000)
        {
            ScriptManager.RegisterClientScriptBlock(ctrl, typeof(string), "msg", "showInfo('" + title + "','" + msg + "'," + autoCloseMillionSeconds + ");", true);
        }

        /// <summary>
        /// 增加一个弹窗信息，然后转到一个网址上
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="msg"></param>
        /// <param name="title"></param>
        /// <param name="type">"info","error","warning","question"</param>
        /// <param name="url"></param>
        public static void AddEasyUiMessagerAndGo(Control ctrl, string msg, string title = "提示", string type = "info", string url = null)
        {
            ScriptManager.RegisterClientScriptBlock(ctrl, typeof(string), "msg", "showInfoAndGo('" + title + "','" + msg + "','" + type + "', '" + (url ?? string.Empty) + "');", true);
        }

        /// <summary>
        /// 增加一个弹窗消息，然后当前窗口返回上一页
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="msg"></param>
        /// <param name="title"></param>
        /// <param name="type">"info","error","warning","question"</param>
        public static void AddEasyUiMessagerAndBack(Control ctrl, string msg, string title = "提示", string type = "info")
        {
            ScriptManager.RegisterClientScriptBlock(ctrl, typeof(string), "msg", "showInfoAndBack('" + title + "','" + msg + "','" + type + "');", true);
        }
    }
    #endregion

    public partial class WF_Admin_UC_CondSta : BP.Web.UC.UCBase3
    {
        #region 属性
        /// <summary>
        /// 主键
        /// </summary>
        public new string MyPK
        {
            get
            {
                return this.Request.QueryString["MyPK"];
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public int FK_Attr
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["FK_Attr"]);
                }
                catch
                {
                    try
                    {
                        return this.DDL_Attr.SelectedItemIntVal;
                    }
                    catch
                    {
                        return 0;
                    }
                }
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
                    return 0;
                }
            }
        }
        public int FK_MainNode
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_MainNode"]);
            }
        }
        public int ToNodeID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["ToNodeID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 执行类型
        /// </summary>
        public CondType HisCondType
        {
            get
            {
                return (CondType)int.Parse(this.Request.QueryString["CondType"]);
            }
        }
        public string GetOperValText
        {
            get
            {
                if (this.Pub1.IsExit("TB_Val"))
                    return this.Pub1.GetTBByID("TB_Val").Text;
                return this.Pub1.GetDDLByID("DDL_Val").SelectedItem.Text;
            }
        }
        #endregion 属性

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Request.QueryString["DoType"] == "Del")
            {
                Cond nd = new Cond(this.MyPK);
                nd.Delete();
                this.Response.Redirect("CondStation.aspx?CondType=" + (int)this.HisCondType + "&FK_Flow=" + this.FK_Flow + "&FK_MainNode=" + nd.NodeID + "&FK_Node=" + this.FK_MainNode + "&ToNodeID=" + nd.ToNodeID, true);
                return;
            }

            this.BindCond();
        }
        public void BindCond()
        {
            string msg = "";
            string note = "";

            Cond cond = new Cond();
            cond.MyPK = this.GenerMyPK;
            cond.RetrieveFromDBSources();

            if (BP.WF.Glo.OSModel == OSModel.WorkFlow)
            {
                SysEnums ses = new SysEnums("StaGrade");
                Stations sts = new Stations();
                sts.RetrieveAll();

                string sql = "SELECT No,Name FROM Port_Station WHERE StaGrade  NOT IN (SELECT IntKey FROM Sys_Enum WHERE EnumKey='StaGrade')";
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count != 0)
                {
                    if(ses.Count == 0)
                    {
                        SysEnum se = new SysEnum();
                        se.EnumKey = "StaGrade";
                        se.Lab = "普通岗";
                        se.IntKey = 0;
                        se.Insert();

                        ses.AddEntity(se);
                    }

                    foreach(Station st in sts)
                    {
                        st.StaGrade = 0;
                        st.Save();
                    }

                    //Pub1.AddEasyUiPanelInfo("错误",
                    //                        "在ccflow的集成工作模式下,岗位表集成或者维护错误,有" + dt.Rows.Count +
                    //                        "个岗位枚举值对应不上:<br />{技术信息:排查的sql:" + sql + "}", "icon-no");
                    //return;
                }

                this.Pub1.AddTable("class='Table' cellSpacing='0' cellPadding='0'  border='0' style='width:100%'");
                
                foreach (SysEnum se in ses)
                {
                    this.Pub1.AddTR();
                    CheckBox mycb = new CheckBox();
                    mycb.Text = se.Lab;
                    mycb.ID = "CB_s_d" + se.IntKey;
                    this.Pub1.AddTD("colspan=3 class='GroupTitle'", mycb);
                    this.Pub1.AddTREnd();

                    int i = 0;
                    string ctlIDs = "";

                    foreach (Station st in sts)
                    {
                        if (st.StaGrade != se.IntKey)
                            continue;

                        i++;

                        if (i == 4)
                            i = 1;

                        if (i == 1)
                            Pub1.AddTR();

                        CheckBox cb = new CheckBox();
                        cb.ID = "CB_" + st.No;
                        ctlIDs += cb.ID + ",";
                        cb.Text = st.Name;
                        if (cond.OperatorValue.ToString().Contains("@" + st.No + "@"))
                            cb.Checked = true;

                        Pub1.AddTD(cb);

                        if (i == 3)
                            Pub1.AddTREnd();
                    }

                    mycb.Attributes["onclick"] = "SetSelected(this,'" + ctlIDs + "')";

                    switch (i)
                    {
                        case 1:
                            Pub1.AddTD();
                            Pub1.AddTD();
                            Pub1.AddTREnd();
                            break;
                        case 2:
                            Pub1.AddTD();
                            Pub1.AddTREnd();
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                /*BPM 模式*/
                BP.GPM.StationTypes tps = new BP.GPM.StationTypes();
                tps.RetrieveAll();

                BP.GPM.Stations sts = new BP.GPM.Stations();
                sts.RetrieveAll();

                string sql = "SELECT No,Name FROM Port_Station WHERE FK_StationType NOT IN (SELECT No FROM Port_StationType)";
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count != 0)
                {
                    if(tps.Count == 0)
                    {
                        var stp = new BP.GPM.StationType { No = "01", Name = "普通岗" };
                        stp.Save();

                        tps.AddEntity(stp);
                    }

                    //更新所有对不上岗位类型的岗位，岗位类型为01或第一个
                    foreach(BP.GPM.Station st in sts)
                    {
                        st.FK_StationType = tps[0].No;
                        st.Update();
                    }

                    //Pub1.AddEasyUiPanelInfo("错误",
                    //                        "在ccflow的集成工作模式下,岗位表集成或者维护错误,有" + dt.Rows.Count +
                    //                        "个岗位外键对应不上:<br />{技术信息:排查的sql:" + sql + "}", "icon-no");
                    //return;
                }

                foreach (BP.GPM.StationType tp in tps)
                {
                    this.Pub1.AddTR();
                    CheckBox mycb = new CheckBox();
                    mycb.Text = tp.Name;
                    mycb.ID = "CB_s_d" + tp.No;
                    this.Pub1.AddTD("colspan=3 class='GroupTitle'", mycb);
                    this.Pub1.AddTREnd();

                    int i = 0;
                    string ctlIDs = "";

                    foreach (BP.GPM.Station st in sts)
                    {
                        if (st.FK_StationType != tp.No)
                            continue;

                        i++;

                        if (i == 4)
                            i = 1;

                        if (i == 1)
                        {
                            Pub1.AddTR();
                        }

                        CheckBox cb = new CheckBox();
                        cb.ID = "CB_" + st.No;
                        ctlIDs += cb.ID + ",";
                        cb.Text = st.Name;
                        if (cond.OperatorValue.ToString().Contains("@" + st.No + "@"))
                            cb.Checked = true;

                        this.Pub1.AddTD(cb);

                        if (i == 3)
                            Pub1.AddTREnd();
                    }

                    mycb.Attributes["onclick"] = "SetSelected(this,'" + ctlIDs + "')";

                    switch (i)
                    {
                        case 1:
                            Pub1.AddTD();
                            Pub1.AddTD();
                            Pub1.AddTREnd();
                            break;
                        case 2:
                            Pub1.AddTD();
                            Pub1.AddTREnd();
                            break;
                        default:
                            break;
                    }
                }
            }

            this.Pub1.AddTableEnd();
            Pub1.AddBR();
            Pub1.AddSpace(1);

            var btn = new LinkBtn(false, NamesOfBtn.Save, "保存");
            btn.Click += new EventHandler(btn_Save_Click);
            this.Pub1.Add(btn);
            Pub1.AddSpace(1);

            btn = new LinkBtn(false, NamesOfBtn.Delete, "删除");
            btn.Attributes["onclick"] = " return confirm('您确定要删除吗？');";
            btn.Click += new EventHandler(btn_Save_Click);
            this.Pub1.Add(btn);
        }

        public Label Lab_Msg
        {
            get
            {
                return this.Pub1.GetLabelByID("Lab_Msg");
            }
        }
        public Label Lab_Note
        {
            get
            {
                return this.Pub1.GetLabelByID("Lab_Note");
            }
        }
        /// <summary>
        /// 属性
        /// </summary>
        public DDL DDL_Attr
        {
            get
            {
                return this.Pub1.GetDDLByID("DDL_Attr");
            }
        }
        public DDL DDL_Oper
        {
            get
            {
                return this.Pub1.GetDDLByID("DDL_Oper");
            }
        }
        public DDL DDL_ConnJudgeWay
        {
            get
            {
                return this.Pub1.GetDDLByID("DDL_ConnJudgeWay");
            }
        }
        public string GenerMyPK
        {
            get
            {
                return this.FK_MainNode + "_" + this.ToNodeID + "_" + this.HisCondType.ToString() + "_" + ConnDataFrom.Stas.ToString();
            }
        }
        void btn_Save_Click(object sender, EventArgs e)
        {
            Cond cond = new Cond();
            cond.Delete(CondAttr.NodeID, this.FK_MainNode,
              CondAttr.ToNodeID, this.ToNodeID,
              CondAttr.CondType, (int)this.HisCondType);

            var btn = sender as LinkBtn;

            if (btn.ID == NamesOfBtn.Delete)
            {
                this.Response.Redirect(this.Request.RawUrl, true);
                return;
            }

            // 删除岗位条件.
            cond.MyPK = this.GenerMyPK;

            if (cond.RetrieveFromDBSources() == 0)
            {
                cond.HisDataFrom = ConnDataFrom.Stas;
                cond.NodeID = this.FK_MainNode;
                cond.FK_Flow = this.FK_Flow;
                cond.ToNodeID = this.ToNodeID;
                cond.Insert();
            }

            string val = "";
            Stations sts = new Stations();
            sts.RetrieveAllFromDBSource();

            foreach (Station st in sts)
            {
                if (this.Pub1.IsExit("CB_" + st.No) == false)
                    continue;
                if (this.Pub1.GetCBByID("CB_" + st.No).Checked)
                    val += "@" + st.No;
            }

            if (val == "")
            {
                cond.Delete();
                return;
            }

            val += "@";
            cond.OperatorValue = val;
            cond.HisDataFrom = ConnDataFrom.Stas;
            cond.FK_Flow = this.FK_Flow;
            cond.HisCondType = this.HisCondType;
            cond.FK_Node = this.FK_MainNode;

            switch (this.HisCondType)
            {
                case CondType.Flow:
                case CondType.Node:
                    cond.Update();
                    this.Response.Redirect("CondStation.aspx?MyPK=" + cond.MyPK + "&FK_Flow=" + cond.FK_Flow + "&FK_Node=" + cond.FK_Node + "&FK_MainNode=" + cond.NodeID + "&CondType=" + (int)cond.HisCondType + "&FK_Attr=" + cond.FK_Attr + "&ToNodeID=" + this.Request.QueryString["ToNodeID"], true);
                    return;
                case CondType.Dir:
                    cond.ToNodeID = this.ToNodeID;
                    cond.Update();
                    this.Response.Redirect("CondStation.aspx?MyPK=" + cond.MyPK + "&FK_Flow=" + cond.FK_Flow + "&FK_Node=" + cond.FK_Node + "&FK_MainNode=" + cond.NodeID + "&CondType=" + (int)cond.HisCondType + "&FK_Attr=" + cond.FK_Attr + "&ToNodeID=" + this.Request.QueryString["ToNodeID"], true);
                    return;
                default:
                    throw new Exception("未设计的情况。");
            }

            EasyUiHelper.AddEasyUiMessager(this, "保存成功！");
        }
    }
}