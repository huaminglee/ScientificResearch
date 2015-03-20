using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using System.Data;
using BP.DA;
using BP.Web;
using BP.En;
using System.Text;
using BP.Port;
using BP.Sys;
using BP.WF.Template;
using BP.WF.Data;
namespace CCFlow.WF
{
    /// <summary>
    /// 接受人
    /// </summary>
    public partial class WF_Accepter : BP.Web.WebPage
    {
        #region 属性.
        /// <summary>
        /// 打开
        /// </summary>
        public int IsWinOpen
        {
            get
            {
                string str = this.Request.QueryString["IsWinOpen"];
                if (str == "1" || str == null || str == "")
                    return 1;
                return 0;
            }
        }
        /// <summary>
        /// 到达的节点
        /// </summary>
        public int ToNode
        {
            get
            {

                if (this.Request.QueryString["ToNode"] == null)
                    return 0;
                return int.Parse(this.Request["ToNode"].ToString());
            }
        }
        public int FK_Node
        {
            get
            {
                return int.Parse(this.Request["FK_Node"].ToString());
            }
        }
        public Int64 WorkID
        {
            get
            {
                return Int64.Parse(this.Request["WorkID"].ToString());
            }
        }
        public Int64 FID
        {
            get
            {
                if (this.Request["FID"] != null)
                    return Int64.Parse(this.Request["FID"].ToString());

                return 0;
            }
        }
        public string FK_Dept
        {
            get
            {
                string s = this.Request.QueryString["FK_Dept"];
                if (s == null)
                    s = WebUser.FK_Dept;
                return s;
            }
        }
        public string FK_Station
        {
            get
            {
                return this.Request.QueryString["FK_Station"];
            }
        }
        public string WorkIDs
        {
            get
            {
                return this.Request.QueryString["WorkIDs"];
            }
        }
        public string DoFunc
        {
            get
            {
                return this.Request.QueryString["DoFunc"];
            }
        }
        public string CFlowNo
        {
            get
            {
                return this.Request.QueryString["CFlowNo"];
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }

        private bool IsMultiple = false;
        /// <summary>
        /// 获取传入参数
        /// </summary>
        /// <param name="param">参数名</param>
        /// <returns></returns>
        public string getUTF8ToString(string param)
        {
            return HttpUtility.UrlDecode(this.Request[param], System.Text.Encoding.UTF8);
        }
        #endregion 属性.

        //public DataTable GetTable()
        //{
        //    if (this.ToNode == 0)
        //        throw new Exception("@流程设计错误，没有转向的节点。举例说明: 当前是A节点。如果您在A点的属性里启用了[接受人]按钮，那么他的转向节点集合中(就是A可以转到的节点集合比如:A到B，A到C, 那么B,C节点就是转向节点集合)，必须有一个节点是的节点属性的[访问规则]设置为[由上一步发送人员选择]");

        //    NodeStations stas = new NodeStations(this.ToNode);
        //    if (stas.Count == 0)
        //    {
        //        BP.WF.Node toNd = new BP.WF.Node(this.ToNode);
        //        throw new Exception("@流程设计错误：设计员没有设计节点[" + toNd.Name + "]，接受人的岗位范围。");
        //    }

        //    string sql = "";
        //    if (this.Request.QueryString["IsNextDept"] != null)
        //    {
        //        int len = this.FK_Dept.Length + 2;
        //        string sqlDept = "SELECT No FROM Port_Dept WHERE " + SystemConfig.AppCenterDBLengthStr + "(No)=" + len + " AND No LIKE '" + this.FK_Dept + "%'";
        //        sql = "SELECT A.No,A.Name, A.FK_Dept, B.Name as DeptName FROM Port_Emp A,Port_Dept B WHERE A.FK_Dept=B.No AND a.NO IN ( ";
        //        sql += "SELECT FK_EMP FROM Port_EmpSTATION WHERE FK_STATION ";
        //        sql += "IN (SELECT FK_STATION FROM WF_NodeStation WHERE FK_Node=" + this.ToNode + ") ";
        //        sql += ") AND A.No IN( SELECT No FROM Port_Emp WHERE  " + SystemConfig.AppCenterDBLengthStr + "(FK_Dept)=" + len + " AND FK_Dept LIKE '" + this.FK_Dept + "%')";
        //        sql += " ORDER BY FK_DEPT ";
        //        return BP.DA.DBAccess.RunSQLReturnTable(sql);
        //    }


        //    // 优先解决本部门的问题。
        //    if (this.FK_Dept == WebUser.FK_Dept)
        //    {
        //        if (BP.WF.Glo.OSModel == OSModel.BPM)
        //        {
        //            sql = "SELECT A.No,A.Name, A.FK_Dept, B.Name as DeptName FROM Port_Emp A,Port_Dept B WHERE A.FK_Dept=B.No AND a.NO IN ( ";
        //            sql += "SELECT FK_EMP FROM Port_DeptEmpStation WHERE FK_STATION ";
        //            sql += "IN (SELECT FK_STATION FROM WF_NodeStation WHERE FK_Node=" + ToNode + ") ";
        //            sql += ") AND a.No IN (SELECT FK_Emp FROM Port_EmpDept WHERE FK_Dept ='" + WebUser.FK_Dept + "')";
        //            sql += " ORDER BY FK_DEPT ";
        //        }
        //        else
        //        {
        //            sql = "SELECT A.No,A.Name, A.FK_Dept, B.Name as DeptName FROM Port_Emp A,Port_Dept B WHERE A.FK_Dept=B.No AND a.NO IN ( ";
        //            sql += "SELECT FK_EMP FROM Port_EmpSTATION WHERE FK_STATION ";
        //            sql += "IN (SELECT FK_STATION FROM WF_NodeStation WHERE FK_Node=" + ToNode + ") ";
        //            sql += ") AND a.No IN (SELECT FK_Emp FROM Port_EmpDept WHERE FK_Dept ='" + WebUser.FK_Dept + "')";
        //            sql += " ORDER BY FK_DEPT ";
        //        }

        //        DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
        //        if (dt.Rows.Count != 0)
        //            return dt;
        //    }

        //    sql = "SELECT A.No,A.Name, A.FK_Dept, B.Name as DeptName FROM Port_Emp A,Port_Dept B WHERE A.FK_Dept=B.No AND a.NO IN ( ";
        //    sql += "SELECT FK_EMP FROM Port_EmpSTATION WHERE FK_STATION ";
        //    sql += "IN (SELECT FK_STATION FROM WF_NodeStation WHERE FK_Node=" + ToNode + ") ";
        //    sql += ") ORDER BY FK_DEPT ";
        //    return BP.DA.DBAccess.RunSQLReturnTable(sql);
        //}
        public DataTable GetTable()
        {
            if (this.ToNode == 0)
                throw new Exception("@流程设计错误，没有转向的节点。举例说明: 当前是A节点。如果您在A点的属性里启用了[接受人]按钮，那么他的转向节点集合中(就是A可以转到的节点集合比如:A到B，A到C, 那么B,C节点就是转向节点集合)，必须有一个节点是的节点属性的[访问规则]设置为[由上一步发送人员选择]");

            NodeStations stas = new NodeStations(this.ToNode);
            if (stas.Count == 0)
            {
                BP.WF.Node toNd = new BP.WF.Node(this.ToNode);
                throw new Exception("@流程设计错误：设计员没有设计节点[" + toNd.Name + "]，接受人的岗位范围。");
            }

            string BindByStationSql = "";
            if (this.Request.QueryString["IsNextDept"] != null)
            {
                int len = this.FK_Dept.Length + 2;
                string sqlDept = "SELECT No FROM Port_Dept WHERE " + SystemConfig.AppCenterDBLengthStr + "(No)=" + len + " AND No LIKE '" + this.FK_Dept + "%'";
                BindByStationSql = "SELECT A.No,A.Name, A.FK_Dept, B.Name as DeptName FROM Port_Emp A,Port_Dept B WHERE A.FK_Dept=B.No AND a.NO IN ( ";
                BindByStationSql += "SELECT FK_EMP FROM Port_EmpSTATION WHERE FK_STATION ";
                BindByStationSql += "IN (SELECT FK_STATION FROM WF_NodeStation WHERE FK_Node=" + this.ToNode + ") ";
                BindByStationSql += ") AND A.No IN( SELECT No FROM Port_Emp WHERE  " + SystemConfig.AppCenterDBLengthStr + "(FK_Dept)=" + len + " AND FK_Dept LIKE '" + this.FK_Dept + "%')";
                BindByStationSql += " ORDER BY FK_DEPT ";
                return BP.DA.DBAccess.RunSQLReturnTable(BindByStationSql);
            }

            string ParSql = "select No from Port_Dept where ParentNo='0'";
            DataTable ParDt = DBAccess.RunSQLReturnTable(ParSql);
            //if (ParDt.Rows.Count == 0)//错误的组织结构
            //{
            //}

            // 优先解决本部门的问题。
            BindByStationSql = string.Format("select No,Name,ParentNo,'1' IsParent from Port_Dept where ParentNo='0' union" +
                                                  " select No,Name,b.FK_Station as ParentNo,'0' IsParent  from Port_Emp a inner" +
                                                  " join Port_DeptEmpStation b on a.No=b.FK_Emp and b.FK_Station in" +
                                                  " (SELECT FK_STATION FROM WF_NodeStation WHERE FK_Node='{0}')  WHERE No in" +
                                                  "  (SELECT FK_EMP FROM Port_DeptEmpStation " +
                                                  " WHERE FK_STATION IN (SELECT FK_STATION FROM WF_NodeStation WHERE FK_Node='{0}'))" +
                                                  " AND No IN (SELECT FK_Emp FROM Port_EmpDept WHERE FK_Dept ='{1}') " +
                                                  " union select No,Name,'{2}' ParentNo,'1' IsParent  from Port_Station where no " +
                                                  "in(SELECT FK_STATION FROM WF_NodeStation WHERE FK_Node='{0}')", ToNode, WebUser.FK_Dept, ParDt.Rows[0][0].ToString());
            DdlEmpSql = string.Format("select No,Name from Port_Emp a inner" +
                                                " join Port_DeptEmpStation b on a.No=b.FK_Emp and b.FK_Station in" +
                                                " (SELECT FK_STATION FROM WF_NodeStation WHERE FK_Node='{0}')  WHERE No in" +
                                                "  (SELECT FK_EMP FROM Port_DeptEmpStation " +
                                                " WHERE FK_STATION IN (SELECT FK_STATION FROM WF_NodeStation WHERE FK_Node='{0}'))" +
                                                " AND No IN (SELECT FK_Emp FROM Port_EmpDept WHERE FK_Dept ='{1}')", ToNode, WebUser.FK_Dept);

            if (this.FK_Dept == WebUser.FK_Dept)
            {

                if (BP.WF.Glo.OSModel == OSModel.BPM)
                {

                }
                else
                {
                    BindByStationSql.Replace("Port_DeptEmpStation", "Port_EmpSTATION");
                    DdlEmpSql.Replace("Port_DeptEmpStation", "Port_EmpSTATION");
                }
                return DBAccess.RunSQLReturnTable(BindByStationSql);
            }

            BindByStationSql = string.Format("select No,Name,ParentNo,'1' IsParent from Port_Dept where ParentNo='0' union" +
                                               " select No,Name,b.FK_Station as ParentNo,'0' IsParent  from Port_Emp a inner" +
                                               " join Port_EmpSTATION b on a.No=b.FK_Emp and b.FK_Station in" +
                                               " (SELECT FK_STATION FROM WF_NodeStation WHERE FK_Node='{0}')  WHERE No in" +
                                               "  (SELECT FK_EMP FROM Port_EmpSTATION " +
                                               " WHERE FK_STATION IN (SELECT FK_STATION FROM WF_NodeStation WHERE FK_Node='{0}'))" +
                                               " AND No IN (SELECT FK_Emp FROM Port_EmpDept) " +
                                               " union select No,Name,'{2}' ParentNo,'1' IsParent  from Port_Station where no " +
                                               "in(SELECT FK_STATION FROM WF_NodeStation WHERE FK_Node='{0}')", ToNode, WebUser.FK_Dept, ParDt.Rows[0][0].ToString());

            DdlEmpSql = string.Format("select No,Name,b.FK_Station as ParentNo,'0' IsParent  from Port_Emp a inner" +
                                               " join Port_EmpSTATION b on a.No=b.FK_Emp and b.FK_Station in" +
                                               " (SELECT FK_STATION FROM WF_NodeStation WHERE FK_Node='{0}')  WHERE No in" +
                                               "  (SELECT FK_EMP FROM Port_EmpSTATION " +
                                               " WHERE FK_STATION IN (SELECT FK_STATION FROM WF_NodeStation WHERE FK_Node='{0}'))" +
                                               " AND No IN (SELECT FK_Emp FROM Port_EmpDept) ", ToNode, WebUser.FK_Dept, ParDt.Rows[0][0].ToString());

            return DBAccess.RunSQLReturnTable(BindByStationSql);
        }
        private BP.WF.Node _HisNode = null;
        /// <summary>
        /// 它的节点
        /// </summary>
        public BP.WF.Node HisNode
        {
            get
            {
                if (_HisNode == null)
                    _HisNode = new BP.WF.Node(this.FK_Node);
                return _HisNode;
            }
        }
        /// <summary>
        /// 是否多分支
        /// </summary>
        public bool IsMFZ
        {
            get
            {
                Nodes nds = this.HisNode.HisToNodes;
                int num = 0;
                foreach (BP.WF.Node mynd in nds)
                {
                    #region 过滤不能到达的节点.
                    Cond cond = new Cond();
                    int i = cond.Retrieve(CondAttr.FK_Node, this.HisNode.NodeID, CondAttr.ToNodeID, mynd.NodeID);
                    if (i == 0)
                        continue; // 没有设置方向条件，就让它跳过去。
                    cond.WorkID = this.WorkID;
                    cond.en = wk;

                    if (cond.IsPassed == false)
                        continue;
                    #endregion 过滤不能到达的节点.

                    if (mynd.HisDeliveryWay == DeliveryWay.BySelected)
                    {
                        num++;
                    }
                }
                if (num == 0)
                    return false;
                if (num == 1)
                    return false;
                return true;
            }
        }
        /// <summary>
        /// 绑定多分支
        /// </summary>
        public void BindMStations()
        {

            this.BindByStation();

            Nodes mynds = this.HisNode.HisToNodes;
            this.Left.Add("<fieldset><legend>&nbsp;选择方向:列出所选方向设置的人员&nbsp;</legend>");
            string str = "<p>";
            foreach (BP.WF.Node mynd in mynds)
            {
                if (mynd.HisDeliveryWay != DeliveryWay.BySelected)
                    continue;

                #region 过滤不能到达的节点.
                Cond cond = new Cond();
                int i = cond.Retrieve(CondAttr.FK_Node, this.HisNode.NodeID, CondAttr.ToNodeID, mynd.NodeID);
                if (i == 0)
                    continue; // 没有设置方向条件，就让它跳过去。

                cond.WorkID = this.WorkID;
                cond.en = wk;
                if (cond.IsPassed == false)
                    continue;
                #endregion 过滤不能到达的节点.

                if (this.ToNode == mynd.NodeID)
                    str += "&nbsp;&nbsp;<b class='l-link'><font color='red' >" + mynd.Name + "</font></b>";
                else
                    str += "&nbsp;&nbsp;<b><a class='l-link' href='Accepter.aspx?FK_Node=" + this.FK_Node + "&type=1&ToNode=" + mynd.NodeID + "&WorkID=" + this.WorkID + "' >" + mynd.Name + "</a></b>";
            }
            this.Left.Add(str + "</p>");
            this.Left.AddFieldSetEnd();
        }

        public Selector MySelector = null;
        public GERpt _wk = null;
        public GERpt wk
        {
            get
            {
                if (_wk == null)
                {
                    _wk = this.HisNode.HisFlow.HisGERpt;
                    _wk.OID = this.WorkID;
                    _wk.Retrieve();
                    _wk.ResetDefaultVal();
                }
                return _wk;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Pub1.Clear();
            this.Title = "选择下一步骤接受的人员";

            //判断是否需要转向。
            if (this.ToNode == 0)
            {
                int num = 0;
                int tempToNodeID = 0;
                /*如果到达的点为空 */
                /*首先判断当前节点的ID，是否配置到了其他节点里面，
                 * * 如果有则需要转向高级的选择框中去，当前界面不能满足公文类的选择人需求。*/
                string sql = "SELECT COUNT(*) FROM WF_Node WHERE FK_Flow='" + this.HisNode.FK_Flow + "' AND " + NodeAttr.DeliveryWay + "=" + (int)DeliveryWay.BySelected + " AND " + NodeAttr.DeliveryParas + " LIKE '%" + this.HisNode.NodeID + "%' ";

                if (DBAccess.RunSQLReturnValInt(sql, 0) > 0)
                {
                    /*说明以后的几个节点人员处理的选择 */
                    string url = "AccepterAdv.aspx?1=3" + this.RequestParas;
                    this.Response.Redirect(url, true);
                    return;
                }

                Nodes nds = this.HisNode.HisToNodes;
                if (nds.Count == 0)
                {
                    this.Pub1.AddFieldSetRed("提示", "当前点是最后的一个节点，不能使用此功能。");
                    return;
                }
                else if (nds.Count == 1)
                {
                    BP.WF.Node toND = nds[0] as BP.WF.Node;
                    tempToNodeID = toND.NodeID;
                }
                else
                {
                    BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
                    foreach (BP.WF.Node mynd in nds)
                    {
                        if (mynd.HisDeliveryWay != DeliveryWay.BySelected)
                            continue;

                        #region 过滤不能到达的节点.
                        if (nd.CondModel == CondModel.ByLineCond)
                        {
                            Cond cond = new Cond();
                            int i = cond.Retrieve(CondAttr.FK_Node, this.HisNode.NodeID, CondAttr.ToNodeID, mynd.NodeID);
                            if (i == 0)
                                continue; // 没有设置方向条件，就让它跳过去。
                            cond.WorkID = this.WorkID;
                            cond.en = wk;
                            if (cond.IsPassed == false)
                                continue;
                        }
                        #endregion 过滤不能到达的节点.
                        tempToNodeID = mynd.NodeID;
                        num++;
                    }
                }

                if (tempToNodeID == 0)
                {
                    this.WinCloseWithMsg("@流程设计错误：\n\n 当前节点的所有分支节点没有一个接受人员规则为按照选择接受。");
                    return;
                }


                this.Response.Redirect("Accepter.aspx?FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&ToNode=" + tempToNodeID + "&FID=" + this.FID + "&type=1&WorkID=" + this.WorkID + "&IsWinOpen=" + this.IsWinOpen, true);
                return;
            }


            try
            {
                /* 首先判断是否有多个分支的情况。*/
                if (this.IsMFZ && ToNode == 0)
                {
                    IsMultiple = true;
                    //this.BindMStations();
                    return;
                }
                MySelector = new Selector(this.ToNode);
                switch (MySelector.SelectorModel)
                {
                    case SelectorModel.Station:
                        //this.BindByStation();
                        returnValue("BindByStation");
                        break;
                    case SelectorModel.SQL:
                        //this.BindBySQL();
                        returnValue("BindBySQL");
                        break;
                    case SelectorModel.Dept:
                        //this.BindByDept();
                        returnValue("BindByDept");
                        break;
                    case SelectorModel.Emp:
                        //this.BindByEmp();
                        returnValue("BindByEmp");
                        break;
                    case SelectorModel.Url:
                        if (MySelector.SelectorP1.Contains("?"))
                            this.Response.Redirect(MySelector.SelectorP1 + "&WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node, true);
                        else
                            this.Response.Redirect(MySelector.SelectorP1 + "?WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node, true);
                        return;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                this.Pub1.Clear();
                this.Pub1.AddMsgOfWarning("错误", ex.Message);
            }
        }
        /// <summary>
        /// 按sql方式
        /// </summary>
        public string BindBySQL()
        {
            string sqlGroup = MySelector.SelectorP1;
            sqlGroup = sqlGroup.Replace("@WebUser.No", WebUser.No);
            sqlGroup = sqlGroup.Replace("@WebUser.Name", WebUser.Name);
            sqlGroup = sqlGroup.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);

            string sqlDB = MySelector.SelectorP2;
            sqlDB = sqlDB.Replace("@WebUser.No", WebUser.No);
            sqlDB = sqlDB.Replace("@WebUser.Name", WebUser.Name);
            sqlDB = sqlDB.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);

            //DataTable dtGroup = DBAccess.RunSQLReturnTable(sqlGroup);
            //DataTable dtDB = DBAccess.RunSQLReturnTable(sqlDB);

            //if (this.MySelector.SelectorDBShowWay == SelectorDBShowWay.Table)
            //    this.BindBySQL_Table(dtGroup, dtDB);
            //else
            //    this.BindBySQL_Tree(dtGroup, dtDB);
            DataTable ParDt = DBAccess.RunSQLReturnTable("select No from Port_Dept where ParentNo='0'");
            string BindBySQL = string.Format("select No,Name,FK_Dept as ParentNo,'0' IsParent from ({0}) emp" +
                                             " union  select No,Name,'{2}' ParentNo,'1' IsParent from ({1}) dept" +
                                             " union  select No,Name,'0' ParentNo,'1' IsParent from Port_Dept where ParentNo='0'", sqlDB, sqlGroup, ParDt.Rows[0][0].ToString());
            //????????????
            //DataTable empDt = DBAccess.RunSQLReturnTable(sqlDB);
            //string empsStr = "";
            //string setChar = ",";
            //for (int i = 0; i < empDt.Rows.Count; i++)
            //{
            //    if (i == empDt.Rows.Count - 1)
            //    {
            //        setChar = "";
            //    }
            //    empsStr += empDt.Rows[i]["No"].ToString() + setChar;
            //}

            //DdlEmpSql = string.Format("select No,Name from Port_Emp  WHERE No in ({0})", empsStr);


            DdlEmpSql = sqlDB;//No,Name没有的情况会报错
            DataTable BindBySQLDt = DBAccess.RunSQLReturnTable(BindBySQL);

            return GetTreeJsonByTable(BindBySQLDt, "NO", "NAME", "ParentNo", "0", "IsParent", "");

        }
        /// <summary>
        /// 按BindByEmp 方式
        /// </summary>
        public string BindByEmp()
        {
            //string sqlGroup = "SELECT No,Name FROM Port_Dept WHERE No IN (SELECT FK_Dept FROM Port_Emp WHERE No in(SELECT FK_EMP FROM WF_NodeEmp WHERE FK_Node='" + MySelector.NodeID + "'))";
            //string sqlDB = "SELECT No,Name,FK_Dept FROM Port_Emp WHERE No in (SELECT FK_EMP FROM WF_NodeEmp WHERE FK_Node='" + MySelector.NodeID + "')";

            //DataTable dtGroup = DBAccess.RunSQLReturnTable(sqlGroup);
            //DataTable dtDB = DBAccess.RunSQLReturnTable(sqlDB);

            //if (this.MySelector.SelectorDBShowWay == SelectorDBShowWay.Table)
            //    this.BindBySQL_Table(dtGroup, dtDB);
            //else
            //    this.BindBySQL_Tree(dtGroup, dtDB);

            string BindByEmpSql = string.Format("select No,Name,ParentNo,'1' IsParent  from Port_Dept   WHERE No IN (SELECT FK_Dept FROM " +
                                              "Port_Emp WHERE No in(SELECT FK_EMP FROM WF_NodeEmp WHERE FK_Node={0})) or ParentNo=0 union " +
                                              "select No,Name,FK_Dept as ParentNo,'0' IsParent  from Port_Emp  WHERE No in (SELECT FK_EMP " +
                                              "FROM WF_NodeEmp WHERE FK_Node={0})", MySelector.NodeID);
            DdlEmpSql = string.Format("select No,Name from Port_Emp  WHERE No in (SELECT FK_EMP " +
                                              "FROM WF_NodeEmp WHERE FK_Node={0})", MySelector.NodeID);
            DataTable BindByEmpDt = DBAccess.RunSQLReturnTable(BindByEmpSql);
            DataTable ParDt = DBAccess.RunSQLReturnTable("select No from Port_Dept where ParentNo='0'");
            foreach (DataRow r in BindByEmpDt.Rows)
            {
                if (r["IsParent"].ToString() == "1" && r["ParentNo"].ToString() != "0")
                {
                    r["ParentNo"] = ParDt.Rows[0][0].ToString();
                }
            }
            return GetTreeJsonByTable(BindByEmpDt, "NO", "NAME", "ParentNo", "0", "IsParent", "");
        }
        public string DdlEmpSql = "";
        /// <summary>
        /// 返回值
        /// </summary>
        private void returnValue(string whichMet)
        {
            string method = string.Empty;
            //返回值
            string s_responsetext = string.Empty;

            if (string.IsNullOrEmpty(Request["method"]))
                return;

            method = Request["method"].ToString();
            switch (method)
            {
                case "getTreeDateMet"://获取数据
                    s_responsetext = getTreeDateMet(whichMet);
                    break;
                case "saveMet":
                    saveMet();
                    break;
            }

            if (string.IsNullOrEmpty(s_responsetext))
                s_responsetext = "";
            s_responsetext = AppendJson(s_responsetext);
            s_responsetext = DdlValue(s_responsetext, DdlEmpSql);
            //组装ajax字符串格式,返回调用客户端 树型
            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.ContentType = "text/html";
            Response.Expires = 0;
            Response.Write(s_responsetext);
            Response.End();
        }
        public string AppendJson(string json)
        {
            StringBuilder AppendJson = new StringBuilder();
            AppendJson.Append(json);
            AppendJson.Append(",CheId:");
            string alreadyHadEmps = string.Format("select No, Name from Port_Emp where No in( select FK_Emp from WF_SelectAccper " +
                                                "where FK_Node={0} and WorkID={1})", this.ToNode, this.WorkID);
            DataTable dt = DBAccess.RunSQLReturnTable(alreadyHadEmps);
            AppendJson.Append("[{\"id\":\"CheId\",\"iconCls\":\"icon-save\",\"text\":\"已选人员\",\"children\":[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                AppendJson.Append("{\"id\":\"" + dt.Rows[i][0].ToString() + "\",iconCls:\"icon-user\"" + ",\"text\":\"" + dt.Rows[i][1].ToString() + "\"");
                if (i == dt.Rows.Count - 1)
                {
                    AppendJson.Append("}");
                    break;
                }
                AppendJson.Append("},");
            }
            //AppendJson.Append("]}]}");

            AppendJson.Append("]}]");

            AppendJson.Insert(0, "{tt:");
            return AppendJson.ToString();
        }
        public string DdlValue(string StrJson, string Str)
        {
            StringBuilder SBuilder = new StringBuilder();
            SBuilder.Append(StrJson);
            DataTable dt = DBAccess.RunSQLReturnTable(Str);

            SBuilder.Append(",ddl:[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i == 0)
                {
                    SBuilder.Append("{\"id\":\"" + dt.Rows[i]["No"].ToString() + "\",\"text\":\"" + dt.Rows[i]["Name"].ToString() + "\",\"selected\":\"selected\"}");
                }
                else
                {
                    SBuilder.Append("{\"id\":\"" + dt.Rows[i]["No"].ToString() + "\",\"text\":\"" + dt.Rows[i]["Name"].ToString() + "\"}");
                }
                if (i == dt.Rows.Count - 1)
                {
                    SBuilder.Append("");
                    continue;
                }
                SBuilder.Append(",");
            }
            SBuilder.Append("]}");
            return SBuilder.ToString();
        }
        public string getTreeDateMet(string Met)
        {
            switch (Met)
            {
                case "BindByEmp":
                    return BindByEmp();
                case "BindByDept":
                    return BindByDept();
                case "BindByStation":
                    return BindByStation();
                case "BindBySQL":
                    return BindBySQL();
                default:
                    return "";
            }
        }
        public string BindByDept()
        {
            //string sqlGroup = "SELECT No,Name FROM Port_Dept WHERE No IN (SELECT FK_Dept FROM WF_NodeDept WHERE FK_Node='" + MySelector.NodeID + "')";
            //string sqlDB = "SELECT No,Name, FK_Dept FROM Port_Emp WHERE FK_Dept IN (SELECT FK_Dept FROM WF_NodeDept WHERE FK_Node='" + MySelector.NodeID + "')";

            //DataTable dtGroup = DBAccess.RunSQLReturnTable(sqlGroup);
            //DataTable dtDB = DBAccess.RunSQLReturnTable(sqlDB);

            //if (this.MySelector.SelectorDBShowWay == SelectorDBShowWay.Table)
            //this.BindBySQL_Table(dtGroup, dtDB);
            //else
            //    this.BindBySQL_Tree(dtGroup, dtDB);


            string BindByDeptSql = string.Format("SELECT  No,Name,ParentNo,'1' IsParent  FROM Port_Dept WHERE No IN (SELECT " +
                                                 "FK_Dept FROM WF_NodeDept WHERE FK_Node={0}) or ParentNo=0 union SELECT No,Name,FK_Dept " +
                                                 "as ParentNo,'0' IsParent FROM Port_Emp WHERE FK_Dept IN (SELECT FK_Dept FROM WF_NodeDept WHERE FK_Node={0})", MySelector.NodeID);

            DdlEmpSql = string.Format("SELECT No,Name FROM Port_Emp WHERE FK_Dept IN (SELECT FK_Dept FROM WF_NodeDept WHERE FK_Node={0})", MySelector.NodeID);


            DataTable BindByDeptDt = DBAccess.RunSQLReturnTable(BindByDeptSql);
            DataTable ParDt = DBAccess.RunSQLReturnTable("select No from Port_Dept where ParentNo='0'");
            foreach (DataRow r in BindByDeptDt.Rows)
            {
                if (r["IsParent"].ToString() == "1" && r["ParentNo"].ToString() != "0")
                {
                    r["ParentNo"] = ParDt.Rows[0][0].ToString();
                }
            }
            return GetTreeJsonByTable(BindByDeptDt, "NO", "NAME", "ParentNo", "0", "IsParent", "");
        }
        /// <summary>
        /// 按table方式.
        /// </summary>
        public void BindBySQL_Table(DataTable dtGroup, DataTable dtObj)
        {
            int col = 4;
            this.Pub1.AddTable("style='border:0px;width:100%'");
            foreach (DataRow drGroup in dtGroup.Rows)
            {
                string ctlIDs = "";
                string groupNo = drGroup[0].ToString();

                //增加全部选择.
                this.Pub1.AddTR();
                CheckBox cbx = new CheckBox();
                cbx.ID = "CBs_" + drGroup[0].ToString();
                cbx.Text = drGroup[1].ToString();
                this.Pub1.AddTDTitle("align=left", cbx);
                this.Pub1.AddTREnd();

                this.Pub1.AddTR();
                this.Pub1.AddTDBegin("nowarp=false");

                this.Pub1.AddTable("style='border:0px;width:100%'");
                int colIdx = -1;
                foreach (DataRow drObj in dtObj.Rows)
                {
                    string no = drObj[0].ToString();
                    string name = drObj[1].ToString();
                    string group = drObj[2].ToString();
                    if (group.Trim() != groupNo.Trim())
                        continue;

                    colIdx++;
                    if (colIdx == 0)
                        this.Pub1.AddTR();

                    CheckBox cb = new CheckBox();
                    cb.ID = "CB_" + no;
                    ctlIDs += cb.ID + ",";
                    cb.Attributes["onclick"] = "isChange=true;";
                    cb.Text = name;
                    cb.Checked = false;
                    if (cb.Checked)
                        cb.Text = "<font color=green>" + cb.Text + "</font>";
                    this.Pub1.AddTD(cb);
                    if (col - 1 == colIdx)
                    {
                        this.Pub1.AddTREnd();
                        colIdx = -1;
                    }
                }
                cbx.Attributes["onclick"] = "SetSelected(this,'" + ctlIDs + "')";

                if (colIdx != -1)
                {
                    while (colIdx != col - 1)
                    {
                        colIdx++;
                        this.Pub1.AddTD();
                    }
                    this.Pub1.AddTREnd();
                }
                this.Pub1.AddTableEnd();
                this.Pub1.AddTDEnd();
                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTableEnd();

            this.BindEnd();
        }

        public void BindBySQL_Tree(DataTable dtGroup, DataTable dtDB)
        {
        }

        public string BindByStation()
        {
            return GetTreeJsonByTable(this.GetTable(), "No", "Name", "ParentNo", "0", "IsParent", "");


            //DataTable dt = this.GetTable(); //获取人员列表。
            //SelectAccpers accps = new SelectAccpers();
            //accps.QueryAccepter(this.FK_Node, WebUser.No, this.WorkID);

            //Dept dept = new Dept();
            //string fk_dept = "";
            //string info = "";

            //if (IsMultiple)
            //    this.Pub1.AddTable("width=400px");
            //else
            //    this.Pub1.AddTable("width=100%");

            //if (WebUser.FK_Dept.Length > 2)
            //{
            //    if (this.FK_Dept == WebUser.FK_Dept)
            //        info = "<b><a href='Accepter.aspx?ToNode=" + this.ToNode + "&WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&type=1&FK_Dept=" + WebUser.FK_Dept.Substring(0, WebUser.FK_Dept.Length - 2) + "'>上一级部门人员</b></a>|<b><a href='Accepter.aspx?ToNode=" + this.ToNode + "&WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&type=1&FK_Dept=" + this.FK_Dept + "&IsNextDept=1' >下一级部门人员</b></a>";
            //    else
            //        info = "<b><a href='Accepter.aspx?ToNode=" + this.ToNode + "&WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&type=1&FK_Dept=" + WebUser.FK_Dept + "'>本部门人员</a></b>";
            //}
            //else
            //{
            //    info = "<b><a href='Accepter.aspx?ToNode=" + this.ToNode + "&WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&type=1&FK_Dept=" + WebUser.FK_Dept + "'>本部门人员</a> | <a href='Accepter.aspx?ToNode=" + this.ToNode + "&WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&type=1&FK_Dept=" + this.FK_Dept + "&IsNextDept=1' >下一级部门人员</b></a>";
            //}


            //BP.WF.Node toNode = new BP.WF.Node(this.ToNode);
            //this.Pub1.AddCaptionLeft("<span style='color:red'>到达节点:[" + toNode.Name + "]</span>");
            //this.Pub1.AddCaptionLeft("可选择范围:" + dt.Rows.Count + " 位." + info);

            //if (dt.Rows.Count > 50)
            //{
            //    /*多于一定的数，就显示导航。*/
            //    this.Pub1.AddTRSum();
            //    this.Pub1.Add("<TD class=BigDoc colspan=5>");
            //    foreach (DataRow dr in dt.Rows)
            //    {
            //        if (fk_dept != dr["FK_Dept"].ToString())
            //        {
            //            fk_dept = dr["FK_Dept"].ToString();
            //            dept = new Dept(fk_dept);
            //            dr["DeptName"] = dept.Name;
            //            this.Pub1.Add("<a href='#d" + dept.No + "' >" + dept.Name + "</a>&nbsp;");
            //        }
            //    }
            //    this.Pub1.AddTDEnd();
            //    this.Pub1.AddTREnd();
            //}

            //int idx = -1;
            //bool is1 = false;
            //foreach (DataRow dr in dt.Rows)
            //{
            //    idx++;
            //    if (fk_dept != dr["FK_Dept"].ToString())
            //    {
            //        switch (idx)
            //        {
            //            case 0:
            //                break;
            //            case 1:
            //                this.Pub1.AddTD();
            //                this.Pub1.AddTD();
            //                this.Pub1.AddTD();
            //                this.Pub1.AddTD();
            //                this.Pub1.AddTREnd();
            //                break;
            //            case 2:
            //                this.Pub1.AddTD();
            //                this.Pub1.AddTD();
            //                this.Pub1.AddTD();
            //                this.Pub1.AddTREnd();
            //                break;
            //            case 3:
            //                this.Pub1.AddTD();
            //                this.Pub1.AddTD();
            //                this.Pub1.AddTREnd();
            //                break;
            //            case 4:
            //                this.Pub1.AddTD();
            //                this.Pub1.AddTREnd();
            //                break;
            //            default:
            //                throw new Exception("error");
            //        }

            //        this.Pub1.AddTRSum();
            //        fk_dept = dr["FK_Dept"].ToString();
            //        string deptName = dr["DeptName"].ToString();
            //        this.Pub1.AddTD("colspan=5 aligen=left class=FDesc ", "<a name='d" + dept.No + "'>" + deptName + "</a>");
            //        this.Pub1.AddTREnd();
            //        is1 = false;
            //        idx = 0;
            //    }

            //    string no = dr["No"].ToString();
            //    string name = dr["Name"].ToString();

            //    CheckBox cb = new CheckBox();
            //    cb.Text = BP.WF.Glo.DealUserInfoShowModel(no, name);

            //    cb.ID = "CB_" + no;
            //    if (accps.Contains("FK_Emp", no))
            //        cb.Checked = true;

            //    switch (idx)
            //    {
            //        case 0:
            //            is1 = this.Pub1.AddTR(is1);
            //            this.Pub1.AddTD(cb);
            //            break;
            //        case 1:
            //        case 2:
            //        case 3:
            //            this.Pub1.AddTD(cb);
            //            break;
            //        case 4:
            //            this.Pub1.AddTD(cb);
            //            this.Pub1.AddTREnd();
            //            idx = -1;
            //            break;
            //        default:
            //            throw new Exception("error");
            //    }
            //    this.Pub1.AddTREnd();
            //}
            //this.Pub1.AddTableEnd();

            //this.BindEnd();
        }
        /// <summary>
        /// 处理绑定结束
        /// </summary>
        public void BindEnd()
        {
            Button btn = new Button();
            if (this.IsWinOpen == 1)
            {
                btn.Text = "确定并关闭";
                btn.ID = "Btn_Save";
                btn.CssClass = "Btn";
                btn.Click += new EventHandler(btn_Save_Click);
                this.Pub1.Add(btn);
            }
            else
            {
                btn = new Button();
                btn.Text = "确定并发送";
                btn.ID = "Btn_Save";
                btn.CssClass = "Btn";
                btn.Click += new EventHandler(btn_Save_Click);
                this.Pub1.Add(btn);

                btn = new Button();
                btn.Text = "取消并返回";
                btn.ID = "Btn_Cancel";
                btn.CssClass = "Btn";
                btn.Click += new EventHandler(btn_Save_Click);
                this.Pub1.Add(btn);
            }

            CheckBox mycb = new CheckBox();
            mycb.ID = "CB_IsSetNextTime";
            mycb.Text = "以后发送都按照本次设置计算";
            this.Pub1.Add(mycb);

            //CheckBox mycb = new CheckBox();
            //mycb.ID = "CB_IsSetNextTime";
            //mycb.Text = "以后发送都按照本次设置计算";
            //mycb.Checked = accps.IsSetNextTime;
            //this.Pub1.Add(mycb);

        }
        //保存
        public void saveMet()
        {
            string getSaveNo = getUTF8ToString("getSaveNo");

            //此处做判断,删除checked的部门数据
            string[] getSaveNoArray = getSaveNo.Split(',');
            List<string> getSaveNoList = new List<string>();

            for (int i = 0; i < getSaveNoArray.Length; i++)
            {
                getSaveNoList.Add(getSaveNoArray[i]);
            }

            getSaveNo = null;
            string ziFu = ",";
            for (int i = 0; i < getSaveNoList.Count; i++)
            {
                if (i == getSaveNoList.Count - 1)
                {
                    ziFu = null;
                }
                getSaveNo += (getSaveNoList[i] + ziFu);
            }

            //设置人员.
            BP.WF.Dev2Interface.WorkOpt_SetAccepter(this.ToNode, this.WorkID, this.FID, getSaveNo, false);





            if (this.IsWinOpen == 0)
            {
                /*如果是 MyFlow.aspx 调用的, 就要调用发送逻辑. */
                //this.DoSend();
                return;
            }


            if (this.Request.QueryString["IsEUI"] == null)
            {
                this.WinClose();
            }
            else
            {
                PubClass.ResponseWriteScript("window.parent.$('windowIfrem').window('close');");

            }

            //#warning 刘文辉 保存收件人后调用发送按钮

            //            BtnLab nd = new BtnLab(this.FK_Node);
            //            if (nd.SelectAccepterEnable == 1)
            //            {
            //                if (this.Request.QueryString["IsEUI"] == null)
            //                {

            //                    /*如果是1不说明直接关闭它.*/
            //                    this.WinClose();
            //                    //ScriptManager.RegisterStartupScript(this.Page, typeof(Page), "", "this.close();", true);
            //                }
            //                else
            //                {
            //                    PubClass.ResponseWriteScript("window.parent.$('windowIfrem').window('close');");

            //                }
            //            }
            //            else
            //            {
            //                ScriptManager.RegisterStartupScript(this.Page, typeof(Page), "", "send();", true);
            //            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btn_Save_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.ID == "Btn_Cancel")
            {
                string url = "../MyFlow.aspx?FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + this.FID;
                this.Response.Redirect(url, true);
                return;
            }

            //DataTable dt = this.GetTable();
            string emps = "";
            foreach (Control ctl in this.Pub1.Controls)
            {
                CheckBox cb = ctl as CheckBox;
                if (cb == null || cb.ID == null || cb.ID.Contains("CBs_") || cb.ID == "CB_IsSetNextTime")
                    continue;

                if (cb.Checked == false)
                    continue;
                emps += cb.ID.Replace("CB_", "") + ",";
            }

            if (emps.Length < 2)
            {
                this.Alert("您没有选择人员。");
                return;
            }

            //获取是否下次自动设置.
            bool isNextTime = this.Pub1.GetCBByID("CB_IsSetNextTime").Checked;

            //设置人员.
            BP.WF.Dev2Interface.WorkOpt_SetAccepter(this.ToNode, this.WorkID, this.FID, emps, isNextTime);

            if (this.IsWinOpen == 0)
            {
                /*如果是 MyFlow.aspx 调用的, 就要调用发送逻辑. */
                this.DoSend();
                return;
            }


            if (this.Request.QueryString["IsEUI"] == null)
            {
                this.WinClose();
            }
            else
            {
                PubClass.ResponseWriteScript("window.parent.$('windowIfrem').window('close');");
            }

#warning 刘文辉 保存收件人后调用发送按钮

            BtnLab nd = new BtnLab(this.FK_Node);
            if (nd.SelectAccepterEnable == 1)
            {
                if (this.Request.QueryString["IsEUI"] == null)
                {

                    /*如果是1不说明直接关闭它.*/
                    this.WinClose();
                    //ScriptManager.RegisterStartupScript(this.Page, typeof(Page), "", "this.close();", true);
                }
                else
                {
                    PubClass.ResponseWriteScript("window.parent.$('windowIfrem').window('close');");

                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, typeof(Page), "", "send();", true);
            }
        }

        public void DoSend()
        {
            // 以下代码是从 MyFlow.aspx Send 方法copy 过来的，需要保持业务逻辑的一致性，所以代码需要保持一致.

            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            Work wk = nd.HisWork;
            wk.OID = this.WorkID;
            wk.Retrieve();

            WorkNode firstwn = new WorkNode(wk, nd);
            string msg = "";
            try
            {
                msg = firstwn.NodeSend().ToMsgOfHtml();
            }
            catch (Exception exSend)
            {
                this.Pub1.AddFieldSetGreen("错误");
                this.Pub1.Add(exSend.Message.Replace("@@", "@").Replace("@", "<BR>@"));
                this.Pub1.AddFieldSetEnd();
                return;
            }

            #region 处理通用的发送成功后的业务逻辑方法，此方法可能会抛出异常.
            try
            {
                //处理通用的发送成功后的业务逻辑方法，此方法可能会抛出异常.
                BP.WF.Glo.DealBuinessAfterSendWork(this.FK_Flow, this.WorkID, this.DoFunc, WorkIDs, this.CFlowNo, 0, null);
            }
            catch (Exception ex)
            {
                this.ToMsg(msg, ex.Message);
                return;
            }
            #endregion 处理通用的发送成功后的业务逻辑方法，此方法可能会抛出异常.


            /*处理转向问题.*/
            switch (firstwn.HisNode.HisTurnToDeal)
            {
                case TurnToDeal.SpecUrl:
                    string myurl = firstwn.HisNode.TurnToDealDoc.Clone().ToString();
                    if (myurl.Contains("&") == false)
                        myurl += "?1=1";
                    Attrs myattrs = firstwn.HisWork.EnMap.Attrs;
                    Work hisWK = firstwn.HisWork;
                    foreach (Attr attr in myattrs)
                    {
                        if (myurl.Contains("@") == false)
                            break;
                        myurl = myurl.Replace("@" + attr.Key, hisWK.GetValStrByKey(attr.Key));
                    }
                    if (myurl.Contains("@"))
                        throw new Exception("流程设计错误，在节点转向url中参数没有被替换下来。Url:" + myurl);

                    myurl += "&FromFlow=" + this.FK_Flow + "&FromNode=" + this.FK_Node + "&PWorkID=" + this.WorkID + "&UserNo=" + WebUser.No + "&SID=" + WebUser.SID;
                    this.Response.Redirect(myurl, true);
                    return;
                case TurnToDeal.TurnToByCond:
                    TurnTos tts = new TurnTos(this.FK_Flow);
                    if (tts.Count == 0)
                        throw new Exception("@您没有设置节点完成后的转向条件。");
                    foreach (TurnTo tt in tts)
                    {
                        tt.HisWork = firstwn.HisWork;
                        if (tt.IsPassed == true)
                        {
                            string url = tt.TurnToURL.Clone().ToString();
                            if (url.Contains("&") == false)
                                url += "?1=1";
                            Attrs attrs = firstwn.HisWork.EnMap.Attrs;
                            Work hisWK1 = firstwn.HisWork;
                            foreach (Attr attr in attrs)
                            {
                                if (url.Contains("@") == false)
                                    break;
                                url = url.Replace("@" + attr.Key, hisWK1.GetValStrByKey(attr.Key));
                            }
                            if (url.Contains("@"))
                                throw new Exception("流程设计错误，在节点转向url中参数没有被替换下来。Url:" + url);

                            url += "&PFlowNo=" + this.FK_Flow + "&FromNode=" + this.FK_Node + "&PWorkID=" + this.WorkID + "&UserNo=" + WebUser.No + "&SID=" + WebUser.SID;
                            this.Response.Redirect(url, true);
                            return;
                        }
                    }
#warning 为上海修改了如果找不到路径就让它按系统的信息提示。
                    this.ToMsg(msg, "info");
                    //throw new Exception("您定义的转向条件不成立，没有出口。");
                    break;
                default:
                    this.ToMsg(msg, "info");
                    break;
            }
            return;
        }

        public void ToMsg(string msg, string type)
        {
            this.Session["info"] = msg;
            this.Application["info" + WebUser.No] = msg;

            BP.WF.Glo.SessionMsg = msg;
            this.Response.Redirect("./../MyFlowInfo.aspx?FK_Flow=" + this.FK_Flow + "&FK_Type=" + type + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID, false);
        }
        /// <summary>
        /// 根据DataTable生成Json树结构
        /// </summary>
        /// <param name="tabel">数据源</param>
        /// <param name="idCol">ID列</param>
        /// <param name="txtCol">Text列</param>
        /// <param name="rela">关系字段</param>
        /// <param name="pId">父ID</param>
        ///<returns>easyui tree json格式</returns>
        StringBuilder treeResult = new StringBuilder();
        StringBuilder treesb = new StringBuilder();
        public string GetTreeJsonByTable(DataTable tabel, string idCol, string txtCol, string rela, object pId, string IsParent, string CheckedString)
        {
            string treeJson = string.Empty;
            treeResult.Append(treesb.ToString());

            treesb.Clear();
            if (tabel.Rows.Count > 0)
            {
                treesb.Append("[");
                string filer = string.Empty;
                if (pId.ToString() == "")
                {
                    filer = string.Format("{0} is null", rela);
                }
                else
                {
                    filer = string.Format("{0}='{1}'", rela, pId);
                }
                DataRow[] rows = tabel.Select(filer);
                if (rows.Length > 0)//修改
                {
                    foreach (DataRow row in rows)
                    {
                        string deptNo = row[idCol].ToString();

                        if (treeResult.Length == 0)
                        {
                            treesb.Append("{\"id\":\"" + row[idCol]
                                + "\",\"text\":\"" + row[txtCol]
                                    + "\",\"attributes\":{\"IsParent\":\"" + row[IsParent] + "\"}"
                                    + ",\"checked\":" + CheckedString.Contains("," + row[idCol] + ",").ToString().ToLower() + ",\"state\":\"open\"");
                        }
                        else if (tabel.Select(string.Format("{0}='{1}'", rela, row[idCol])).Length > 0)
                        {
                            treesb.Append("{\"id\":\"" + row[idCol]
                                + "\",\"text\":\"" + row[txtCol]
                                    + "\",\"attributes\":{\"IsParent\":\"" + row[IsParent] + "\"}"
                                + ",\"checked\":" + CheckedString.Contains("," + row[idCol] + ",").ToString().ToLower() + ",\"state\":\"open\"");
                        }
                        else
                        {
                            treesb.Append("{\"id\":\"" + row[idCol]
                                + "\",\"text\":\"" + row[txtCol]
                                    + "\",\"attributes\":{\"IsParent\":\"" + row[IsParent] + "\"}"
                              + ",\"checked\":" + CheckedString.Contains("," + row[idCol] + ",").ToString().ToLower());
                        }


                        if (tabel.Select(string.Format("{0}='{1}'", rela, row[idCol])).Length > 0)
                        {
                            treesb.Append(",\"children\":");
                            GetTreeJsonByTable(tabel, idCol, txtCol, rela, row[idCol], IsParent, CheckedString);
                            treeResult.Append(treesb.ToString());
                            treesb.Clear();
                        }
                        treeResult.Append(treesb.ToString());
                        treesb.Clear();
                        treesb.Append("},");
                    }
                    treesb = treesb.Remove(treesb.Length - 1, 1);
                }
                treesb.Append("]");
                treeResult.Append(treesb.ToString());
                treeJson = treeResult.ToString();
                treesb.Clear();
            }
            return treeJson;
        }
    }
}