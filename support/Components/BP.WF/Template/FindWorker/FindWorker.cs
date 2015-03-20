﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BP.GPM;
using BP.En;
using BP.DA;
using BP.Web;
using BP.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// 找人规则
    /// </summary>
    public class FindWorker
    {
        #region 变量
        public WorkNode town = null;
        public WorkNode currWn = null;
        public Flow fl = null;
        string dbStr = BP.Sys.SystemConfig.AppCenterDBVarStr;
        public Paras ps = null;
        string JumpToEmp = null;
        int JumpToNode = 0;
        Int64 WorkID = 0;
        #endregion 变量

        /// <summary>
        /// 找人
        /// </summary>
        /// <param name="fl"></param>
        /// <param name="currWn"></param>
        /// <param name="toWn"></param>
        public FindWorker()
        {
        }
        private DataTable FindByWorkFlowModel()
        {
            this.town = town;

            DataTable dt = new DataTable();
            dt.Columns.Add("No", typeof(string));
            string sql;
            string FK_Emp;

            // 如果执行了两次发送，那前一次的轨迹就需要被删除,这里是为了避免错误。
            ps = new Paras();
            ps.Add("WorkID", this.WorkID);
            ps.Add("FK_Node", town.HisNode.NodeID);
            ps.SQL = "DELETE FROM WF_GenerWorkerlist WHERE WorkID=" + dbStr + "WorkID AND FK_Node =" + dbStr + "FK_Node";
            DBAccess.RunSQL(ps);

            // 如果指定特定的人员处理。
            if (string.IsNullOrEmpty(JumpToEmp) == false)
            {
                string[] emps = JumpToEmp.Split(',');
                foreach (string emp in emps)
                {
                    if (string.IsNullOrEmpty(emp))
                        continue;
                    DataRow dr = dt.NewRow();
                    dr[0] = emp;
                    dt.Rows.Add(dr);
                }
                return  dt;
            }

            // 按上一节点发送人处理。
            if (town.HisNode.HisDeliveryWay == DeliveryWay.ByPreviousNodeEmp)
            {
                DataRow dr = dt.NewRow();
                dr[0] = BP.Web.WebUser.No;
                dt.Rows.Add(dr);
                return dt;
            }
             

            //首先判断是否配置了获取下一步接受人员的sql.
            if (town.HisNode.HisDeliveryWay == DeliveryWay.BySQL
                || town.HisNode.HisDeliveryWay == DeliveryWay.BySQLAsSubThreadEmpsAndData)
            {
                if (town.HisNode.DeliveryParas.Length < 4)
                    throw new Exception("@您设置的当前节点按照SQL，决定下一步的接受人员，但是你没有设置sql.");
                
                sql = town.HisNode.DeliveryParas;
                sql = sql.Clone().ToString();

                sql = Glo.DealExp(sql, this.currWn.rptGe, null);
                if (sql.Contains("@"))
                {
                    if (Glo.SendHTOfTemp != null)
                    {
                        foreach (string key in Glo.SendHTOfTemp.Keys)
                        {
                            sql = sql.Replace("@" + key, Glo.SendHTOfTemp[key].ToString());
                        }
                    }
                }

                dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0 && town.HisNode.HisWhenNoWorker != WhenNoWorker.Skip)
                    throw new Exception("@没有找到可接受的工作人员。@技术信息：执行的sql没有发现人员:" + sql);
                return dt;
            }

            #region 按照明细表,作为子线程的接收人.
            if (town.HisNode.HisDeliveryWay == DeliveryWay.ByDtlAsSubThreadEmps)
            {
                if (this.town.HisNode.HisRunModel != RunModel.SubThread)
                    throw new Exception("@您设置的节点接收人方式为：以分流点表单的明细表数据源确定子线程的接收人，但是当前节点非子线程节点。");
                
                BP.Sys.MapDtls dtls = new BP.Sys.MapDtls(this.currWn.HisNode.NodeFrmID);
                string msg = null;
                foreach (BP.Sys.MapDtl dtl in dtls)
                {
                    try
                    {
                        ps = new Paras();
                        ps.SQL = "SELECT UserNo FROM " + dtl.PTable + " WHERE RefPK=" + dbStr + "OID ORDER BY OID";
                        ps.Add("OID", this.WorkID);
                        dt = DBAccess.RunSQLReturnTable(ps);
                        if (dt.Rows.Count == 0 && town.HisNode.HisWhenNoWorker != WhenNoWorker.Skip)
                            throw new Exception("@流程设计错误，到达的节点（" + town.HisNode.Name + "）在指定的节点中没有数据，无法找到子线程的工作人员。");
                        return dt;
                    }
                    catch (Exception ex)
                    {
                        msg += ex.Message;
                        //if (dtls.Count == 1)
                        //    throw new Exception("@估计是流程设计错误,没有在分流节点的明细表中设置");
                    }
                }
                throw new Exception("@没有找到分流节点的明细表作为子线程的发起的数据源，流程设计错误，请确认分流节点表单中的明细表是否有UserNo约定的系统字段。"+msg);
            }
            #endregion 按照明细表,作为子线程的接收人.

            #region 按节点绑定的人员处理.
            if (town.HisNode.HisDeliveryWay == DeliveryWay.ByBindEmp)
            {
                ps = new Paras();
                ps.Add("FK_Node", town.HisNode.NodeID);
                ps.SQL = "SELECT FK_Emp FROM WF_NodeEmp WHERE FK_Node=" + dbStr + "FK_Node ORDER BY FK_Emp";
                dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count == 0 )
                    throw new Exception("@流程设计错误:下一个节点(" + town.HisNode.Name + ")没有绑定工作人员 . ");
                return dt;
            }
            #endregion 按节点绑定的人员处理.

            #region 按照选择的人员处理。
            if (town.HisNode.HisDeliveryWay == DeliveryWay.BySelected)
            {
                ps = new Paras();
                ps.Add("FK_Node", this.town.HisNode.NodeID);
                ps.Add("WorkID", this.currWn.HisWork.OID);
                ps.SQL = "SELECT FK_Emp FROM WF_SelectAccper WHERE FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "WorkID AND AccType=0 ORDER BY IDX";
                dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count == 0)
                {
                    /*从上次发送设置的地方查询. */
                    SelectAccpers sas = new SelectAccpers();
                    int i = sas.QueryAccepterPriSetting(this.town.HisNode.NodeID);
                    if (i == 0)
                        throw new Exception("请选择下一步骤工作(" + town.HisNode.Name + ")接受人员。"); //

                    //插入里面.
                    foreach (SelectAccper item in sas)
                    {
                        DataRow dr = dt.NewRow();
                        dr[0] = item.FK_Emp;
                        dt.Rows.Add(dr);
                    }
                    return dt;
                }
                return dt;
            }
            #endregion 按照选择的人员处理。

            #region 按照指定节点的处理人计算。
            if (town.HisNode.HisDeliveryWay == DeliveryWay.BySpecNodeEmp
                || town.HisNode.HisDeliveryWay == DeliveryWay.ByStarter)
            {
                /* 按指定节点岗位上的人员计算 */
                string strs = "";
                if (town.HisNode.HisDeliveryWay == DeliveryWay.ByStarter)
                    strs = int.Parse(this.fl.No)+"01";
                else
                    strs=town.HisNode.DeliveryParas;

                // 首先从本流程里去找。
                strs = strs.Replace(";", ",");
                string[] nds = strs.Split(',');
                foreach (string nd in nds)
                {
                    if (string.IsNullOrEmpty(nd))
                        continue;

                    if (DataType.IsNumStr(nd) == false)
                        throw new Exception("流程设计错误:您设置的节点(" + town.HisNode.Name + ")的接收方式为按指定的节点岗位投递，但是您没有在访问规则设置中设置节点编号。");

                    ps = new Paras();
                    ps.SQL = "SELECT FK_Emp FROM WF_GenerWorkerList WHERE WorkID=" + dbStr + "OID AND FK_Node=" + dbStr + "FK_Node AND IsPass=1 AND IsEnable=1 ";
                    ps.Add("FK_Node", int.Parse(nd));
                    if (this.currWn.HisNode.HisRunModel == RunModel.SubThread)
                        ps.Add("OID", this.currWn.HisWork.FID);
                    else
                        ps.Add("OID", this.WorkID);

                    dt = DBAccess.RunSQLReturnTable(ps);
                    if (dt.Rows.Count == 1)
                        return dt;

                    //就要到轨迹表里查,因为有可能是跳过的节点.
                    ps = new Paras();
                    ps.SQL = "SELECT " + TrackAttr.EmpTo + " FROM ND" + int.Parse(fl.No) + "Track WHERE ActionType=" + dbStr + "ActionType AND NDTo=" + dbStr + "NDTo AND WorkID=" + dbStr + "WorkID";
                    ps.Add("ActionType", (int)ActionType.Skip);
                    ps.Add("NDTo", int.Parse(nd));
                    ps.Add("WorkID", this.WorkID);
                    dt = DBAccess.RunSQLReturnTable(ps);
                    if (dt.Rows.Count != 0)
                        return dt;
                }

                //本流程里没有有可能该节点是配置的父流程节点,也就是说子流程的一个节点与父流程指定的节点的工作人员一致.
                GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
                if (gwf.PWorkID != 0)
                {
                    foreach (string pnodeiD in nds)
                    {
                        if (string.IsNullOrEmpty(pnodeiD))
                            continue;

                        Node nd = new Node(int.Parse(pnodeiD));
                        if (nd.FK_Flow != gwf.PFlowNo)
                            continue; // 如果不是父流程的节点，就不执行.

                        ps = new Paras();
                        ps.SQL = "SELECT FK_Emp FROM WF_GenerWorkerList WHERE WorkID=" + dbStr + "OID AND FK_Node=" + dbStr + "FK_Node AND IsPass=1 AND IsEnable=1 ";
                        ps.Add("FK_Node", nd.NodeID);
                        if (this.currWn.HisNode.HisRunModel == RunModel.SubThread)
                            ps.Add("OID", gwf.PFID);
                        else
                            ps.Add("OID", gwf.PWorkID);

                        dt = DBAccess.RunSQLReturnTable(ps);
                        if (dt.Rows.Count == 1)
                            return dt;

                        //就要到轨迹表里查,因为有可能是跳过的节点.
                        ps = new Paras();
                        ps.SQL = "SELECT " + TrackAttr.EmpTo + " FROM ND" + int.Parse(fl.No) + "Track WHERE ActionType=" + dbStr + "ActionType AND NDTo=" + dbStr + "NDTo AND WorkID=" + dbStr + "WorkID";
                        ps.Add("ActionType", (int)ActionType.Skip);
                        ps.Add("NDTo", nd.NodeID);

                        if (this.currWn.HisNode.HisRunModel == RunModel.SubThread)
                            ps.Add("OID", gwf.PFID);
                        else
                            ps.Add("OID", gwf.PWorkID);

                        dt = DBAccess.RunSQLReturnTable(ps);
                        if (dt.Rows.Count != 0)
                            return dt;
                    }
                }

                throw new Exception("@流程设计错误，到达的节点（" + town.HisNode.Name + "）在指定的节点(" + strs + ")中没有数据，无法找到工作的人员。投递方式:BySpecNodeEmp sql=" + ps.SQL);
            }
            #endregion 按照节点绑定的人员处理。

            #region 按照上一个节点表单指定字段的人员处理。
            if (town.HisNode.HisDeliveryWay == DeliveryWay.ByPreviousNodeFormEmpsField)
            {
                // 检查接受人员规则,是否符合设计要求.
                string specEmpFields = town.HisNode.DeliveryParas;
                if (string.IsNullOrEmpty(specEmpFields))
                    specEmpFields = "SysSendEmps";

                if (this.currWn.HisWork.EnMap.Attrs.Contains(specEmpFields) == false)
                    throw new Exception("@您设置的当前节点按照指定的人员，决定下一步的接受人员，但是你没有在节点表单中设置该表单" + specEmpFields + "字段。");

                //获取接受人并格式化接受人, 
                string emps = this.currWn.HisWork.GetValStringByKey(specEmpFields);
                emps = emps.Replace(" ", "");
                if (emps.Contains(",") && emps.Contains(";"))
                {
                    /*如果包含,; 例如 zhangsan,张三;lisi,李四;*/
                    string[] myemps1 = emps.Split(';');
                    foreach (string str in    myemps1)
                    {
                        if (string.IsNullOrEmpty(str))
                            continue;

                        string[] ss = str.Split(',');
                        DataRow dr = dt.NewRow();
                        dr[0] = ss[0];
                        dt.Rows.Add(dr);
                    }
                    if (dt.Rows.Count == 0)
                        throw new Exception("@输入的接受人员信息错误;[" + emps + "]。");
                    else
                        return dt;
                }

                emps = emps.Replace(";", ",");
                emps = emps.Replace("；", ",");
                emps = emps.Replace("，", ",");
                emps = emps.Replace("、", ",");
                emps = emps.Replace("@", ",");

                if (string.IsNullOrEmpty(emps))
                    throw new Exception("@没有在字段[" + this.currWn.HisWork.EnMap.Attrs.GetAttrByKey(specEmpFields).Desc + "]中指定接受人，工作无法向下发送。");

                // 把它加入接受人员列表中.
                string[] myemps = emps.Split(',');
                foreach (string s in myemps)
                {
                    if (string.IsNullOrEmpty(s))
                        continue;

                    //if (BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(NO) AS NUM FROM Port_Emp WHERE NO='" + s + "' or name='"+s+"'", 0) == 0)
                    //    continue;

                    DataRow dr = dt.NewRow();
                    dr[0] = s;
                    dt.Rows.Add(dr);
                }
                return dt;
            }
            #endregion 按照上一个节点表单指定字段的人员处理。

            string prjNo = "";
            FlowAppType flowAppType = this.currWn.HisNode.HisFlow.HisFlowAppType;
            sql = "";
            if (this.currWn.HisNode.HisFlow.HisFlowAppType == FlowAppType.PRJ)
            {
                prjNo = "";
                try
                {
                    prjNo = this.currWn.HisWork.GetValStrByKey("PrjNo");
                }
                catch (Exception ex)
                {
                    throw new Exception("@当前流程是工程类流程，但是在节点表单中没有PrjNo字段(注意区分大小写)，请确认。@异常信息:" + ex.Message);
                }
            }

            #region 按部门与岗位的交集计算.
            if (town.HisNode.HisDeliveryWay == DeliveryWay.ByDeptAndStation)
            {
                sql = "SELECT No FROM Port_Emp WHERE No IN ";
                sql += "(SELECT FK_Emp FROM Port_EmpDept WHERE FK_Dept IN ";
                sql += "( SELECT FK_Dept FROM WF_NodeDept WHERE FK_Node=" + dbStr + "FK_Node1)";
                sql += ")";
                sql += "AND No IN ";
                sql += "(";
                sql += "SELECT FK_Emp FROM Port_EmpStation WHERE FK_Station IN ";
                sql += "( SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + dbStr + "FK_Node2 )";
                sql += ") ORDER BY No ";

                ps = new Paras();
                ps.Add("FK_Node1", town.HisNode.NodeID);
                ps.Add("FK_Node2", town.HisNode.NodeID);
                ps.SQL = sql;
                dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count > 0)
                    return dt;
                else
                    throw new Exception("@节点访问规则错误:节点(" + town.HisNode.NodeID + "," + town.HisNode.Name + "), 按照岗位与部门的交集确定接受人的范围错误，没有找到人员:SQL=" + sql);
            }
            #endregion 按部门与岗位的交集计算.

            #region 判断节点部门里面是否设置了部门，如果设置了，就按照它的部门处理。
            if (town.HisNode.HisDeliveryWay == DeliveryWay.ByDept)
            {
                if (flowAppType == FlowAppType.Normal)
                {
                    ps = new Paras();
                    ps.SQL = "SELECT No,Name FROM Port_Emp WHERE FK_Dept IN (SELECT FK_Dept FROM WF_NodeDept WHERE FK_Node=" + dbStr + "FK_Node1)";
                    ps.SQL += " OR ";
                    ps.SQL += " No IN (SELECT FK_Emp FROM Port_EmpDept WHERE FK_Dept IN ( SELECT FK_Dept FROM WF_NodeDept WHERE FK_Node=" + dbStr + "FK_Node2 ) )";
                    ps.SQL += " ORDER BY No";
                    ps.Add("FK_Node1", town.HisNode.NodeID);
                    ps.Add("FK_Node2", town.HisNode.NodeID);

                    dt = DBAccess.RunSQLReturnTable(ps);
                    if (dt.Rows.Count > 0  && town.HisNode.HisWhenNoWorker != WhenNoWorker.Skip)
                    {
                        return dt;
                    }
                    else
                    {
                        //IsFindWorker = false;
                        //  ps.SQL = "SELECT No,Name FROM Port_Emp WHERE FK_Dept IN ( SELECT FK_Dept FROM WF_NodeDept WHERE FK_Node=" + dbStr + "FK_Node )";
                        throw new Exception("@按部门确定接受人的范围,没有找到人员.");
                    }
                }

                if (flowAppType == FlowAppType.PRJ)
                {
                    sql = "SELECT No FROM Port_Emp WHERE No IN ";
                    sql += "(SELECT FK_Emp FROM Port_EmpDept WHERE FK_Dept IN ";
                    sql += "( SELECT FK_Dept FROM WF_NodeDept WHERE FK_Node=" + dbStr + "FK_Node1)";
                    sql += ")";
                    sql += "AND NO IN ";
                    sql += "(";
                    sql += "SELECT FK_Emp FROM Prj_EmpPrjStation WHERE FK_Station IN ";
                    sql += "( SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + dbStr + "FK_Node2) AND FK_Prj=" + dbStr + "FK_Prj ";
                    sql += ")";
                    sql += " ORDER BY No";

                    ps = new Paras();
                    ps.Add("FK_Node1", town.HisNode.NodeID);
                    ps.Add("FK_Node2", town.HisNode.NodeID);
                    ps.Add("FK_Prj", prjNo);
                    ps.SQL = sql;

                    dt = DBAccess.RunSQLReturnTable(ps);
                    if (dt.Rows.Count == 0)
                    {
                        /* 如果项目组里没有工作人员就提交到公共部门里去找。*/
                        sql = "SELECT NO FROM Port_Emp WHERE NO IN ";
                        sql += "(SELECT FK_Emp FROM Port_EmpDept WHERE FK_Dept IN ";
                        sql += "( SELECT FK_Dept FROM WF_NodeDept WHERE FK_Node=" + dbStr + "FK_Node1)";
                        sql += ")";
                        sql += "AND NO IN ";
                        sql += "(";
                        sql += "SELECT FK_Emp FROM Port_EmpStation WHERE FK_Station IN ";
                        sql += "( SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + dbStr + "FK_Node2)";
                        sql += ")";
                        sql += " ORDER BY No";

                        ps = new Paras();
                        ps.Add("FK_Node1", town.HisNode.NodeID);
                        ps.Add("FK_Node2", town.HisNode.NodeID);
                        ps.SQL = sql;
                    }
                    else
                    {
                        return  dt;
                    }

                    dt = DBAccess.RunSQLReturnTable(ps);
                    if (dt.Rows.Count > 0)
                        return dt;
                }
            }
            #endregion 判断节点部门里面是否设置了部门，如果设置了，就按照它的部门处理。

            #region 仅按岗位计算
            if (town.HisNode.HisDeliveryWay == DeliveryWay.ByStationOnly)
            {
                sql = "SELECT A.FK_Emp FROM Port_EmpStation A, WF_NodeStation B WHERE A.FK_Station=B.FK_Station AND B.FK_Node=" + dbStr + "FK_Node ORDER BY A.FK_Emp";
                ps = new Paras();
                ps.Add("FK_Node", town.HisNode.NodeID);
                ps.SQL = sql;
                dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count > 0)
                    return dt;
                else
                    throw new Exception("@节点访问规则错误:节点(" + town.HisNode.NodeID + "," + town.HisNode.Name + "), 按节点岗位与人员部门集合两个纬度计算，没有找到人员:SQL=" + sql);
            }
            #endregion

            #region 按岗位计算(以部门集合为纬度).
            if (town.HisNode.HisDeliveryWay == DeliveryWay.ByStationAndEmpDept)
            {
                sql = "SELECT No FROM Port_Emp WHERE NO IN "
                      + "(SELECT  FK_Emp  FROM Port_EmpStation WHERE FK_Station IN (SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + dbStr + "FK_Node) )"
                      + " AND  FK_Dept IN "
                      + "(SELECT  FK_Dept  FROM Port_EmpDept WHERE FK_Emp =" + dbStr + "FK_Emp)";

                sql += " ORDER BY No";

                ps = new Paras();
                ps.Add("FK_Node", town.HisNode.NodeID);
                ps.Add("FK_Emp", WebUser.No);
                ps.SQL = sql;
                //2012.7.16李健修改
                //+" AND  NO IN "
                //+ "(SELECT  FK_Emp  FROM Port_EmpDept WHERE FK_Emp = '" + WebUser.No + "')";
                dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count > 0)
                    return dt;
                else
                    throw new Exception("@节点访问规则错误:节点(" + town.HisNode.NodeID + "," + town.HisNode.Name + "), 按节点岗位与人员部门集合两个纬度计算，没有找到人员:SQL=" + sql);
            }
            #endregion

            string empNo = WebUser.No;
            string empDept = WebUser.FK_Dept;

            #region 按指定的节点的人员岗位，做为下一步骤的流程接受人。
            if (town.HisNode.HisDeliveryWay == DeliveryWay.BySpecNodeEmpStation)
            {
                /* 按指定的节点的人员岗位 */
                string fk_node = town.HisNode.DeliveryParas;
                if (DataType.IsNumStr(fk_node) == false)
                    throw new Exception("流程设计错误:您设置的节点(" + town.HisNode.Name + ")的接收方式为按指定的节点人员岗位投递，但是您没有在访问规则设置中设置节点编号。");

                ps = new Paras();
                ps.SQL = "SELECT Rec,FK_Dept FROM ND" + fk_node + " WHERE OID=" + dbStr + "OID";
                ps.Add("OID", this.WorkID);
                dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count != 1)
                    throw new Exception("@流程设计错误，到达的节点（" + town.HisNode.Name + "）在指定的节点中没有数据，无法找到工作的人员。");

                empNo = dt.Rows[0][0].ToString();
                empDept = dt.Rows[0][1].ToString();
            }
            #endregion 按指定的节点人员，做为下一步骤的流程接受人。

            #region 最后判断 - 按照岗位来执行。
            if (this.currWn.HisNode.IsStartNode == false)
            {
                ps = new Paras();
                if (flowAppType == FlowAppType.Normal || flowAppType == FlowAppType.DocFlow)
                {
                    // 如果当前的节点不是开始节点， 从轨迹里面查询。
                    sql = "SELECT DISTINCT FK_Emp  FROM Port_EmpStation WHERE FK_Station IN "
                       + "(SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + town.HisNode.NodeID + ") "
                       + "AND FK_Emp IN (SELECT FK_Emp FROM WF_GenerWorkerlist WHERE WorkID=" + dbStr + "WorkID AND FK_Node IN (" + DataType.PraseAtToInSql(town.HisNode.GroupStaNDs, true) + ") )";

                    sql += " ORDER BY FK_Emp ";

                    ps.SQL = sql;
                    ps.Add("WorkID", this.WorkID);
                }

                if (flowAppType == FlowAppType.PRJ)
                {
                    // 如果当前的节点不是开始节点， 从轨迹里面查询。
                    sql = "SELECT DISTINCT FK_Emp  FROM Prj_EmpPrjStation WHERE FK_Station IN "
                       + "(SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + dbStr + "FK_Node ) AND FK_Prj=" + dbStr + "FK_Prj "
                       + "AND FK_Emp IN (SELECT FK_Emp FROM WF_GenerWorkerlist WHERE WorkID=" + dbStr + "WorkID AND FK_Node IN (" + DataType.PraseAtToInSql(town.HisNode.GroupStaNDs, true) + ") )";
                    sql += " ORDER BY FK_Emp ";

                    ps = new Paras();
                    ps.SQL = sql;
                    ps.Add("FK_Node", town.HisNode.NodeID);
                    ps.Add("FK_Prj", prjNo);
                    ps.Add("WorkID", this.WorkID);

                    dt = DBAccess.RunSQLReturnTable(ps);
                    if (dt.Rows.Count == 0)
                    {
                        /* 如果项目组里没有工作人员就提交到公共部门里去找。*/
                        sql = "SELECT DISTINCT FK_Emp  FROM Port_EmpStation WHERE FK_Station IN "
                         + "(SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + dbStr + "FK_Node ) "
                         + "AND FK_Emp IN (SELECT FK_Emp FROM WF_GenerWorkerlist WHERE WorkID=" + dbStr + "WorkID AND FK_Node IN (" + DataType.PraseAtToInSql(town.HisNode.GroupStaNDs, true) + ") )";
                        sql += " ORDER BY FK_Emp ";

                        ps = new Paras();
                        ps.SQL = sql;
                        ps.Add("FK_Node", town.HisNode.NodeID);
                        ps.Add("WorkID", this.WorkID);
                    }
                    else
                    {
                        return  dt;
                    }
                }

                dt = DBAccess.RunSQLReturnTable(ps);
                // 如果能够找到.
                if (dt.Rows.Count >= 1)
                {
                    if (dt.Rows.Count == 1)
                    {
                        /*如果人员只有一个的情况，说明他可能要 */
                    }
                    return dt;
                }
            }

            /* 如果执行节点 与 接受节点岗位集合一致 */
            if (this.currWn.HisNode.GroupStaNDs == town.HisNode.GroupStaNDs)
            {
                /* 说明，就把当前人员做为下一个节点处理人。*/
                DataRow dr = dt.NewRow();
                dr[0] = WebUser.No;
                dt.Rows.Add(dr);
                return dt;
            }

            /* 如果执行节点 与 接受节点岗位集合不一致 */
            if (this.currWn.HisNode.GroupStaNDs != town.HisNode.GroupStaNDs)
            {
                /* 没有查询到的情况下, 先按照本部门计算。*/
                if (flowAppType == FlowAppType.Normal)
                {
                    switch (BP.Sys.SystemConfig.AppCenterDBType)
                    {
                        case DBType.MySQL:
                        case DBType.MSSQL:
                            sql = "select No from Port_Emp x inner join (select FK_Emp from Port_EmpStation a inner join WF_NodeStation b ";
                            sql += " on a.FK_Station=b.FK_Station where FK_Node=" + dbStr + "FK_Node) as y on x.No=y.FK_Emp inner join Port_EmpDept z on";
                            sql += " x.No=z.FK_Emp where z.FK_Dept =" + dbStr + "FK_Dept order by x.No";
                            break;
                        default:
                            sql = "SELECT No FROM Port_Emp WHERE NO IN "
                          + "(SELECT  FK_Emp  FROM Port_EmpStation WHERE FK_Station IN (SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + dbStr + "FK_Node) )"
                          + " AND  NO IN "
                          + "(SELECT  FK_Emp  FROM Port_EmpDept WHERE FK_Dept =" + dbStr + "FK_Dept)";
                            sql += " ORDER BY No ";
                            break;
                    }

                    ps = new Paras();
                    ps.SQL = sql;
                    ps.Add("FK_Node", town.HisNode.NodeID);
                    ps.Add("FK_Dept", empDept);
                }

                if (flowAppType == FlowAppType.PRJ)
                {
                    sql = "SELECT  FK_Emp  FROM Prj_EmpPrjStation WHERE FK_Prj=" + dbStr + "FK_Prj1 AND FK_Station IN (SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + dbStr + "FK_Node)"
                    + " AND  FK_Prj=" + dbStr + "FK_Prj2 ";
                    sql += " ORDER BY FK_Emp ";

                    ps = new Paras();
                    ps.SQL = sql;
                    ps.Add("FK_Prj1", prjNo);
                    ps.Add("FK_Node", town.HisNode.NodeID);
                    ps.Add("FK_Prj2", prjNo);
                    dt = DBAccess.RunSQLReturnTable(ps);
                    if (dt.Rows.Count == 0)
                    {
                        /* 如果项目组里没有工作人员就提交到公共部门里去找。 */
                        sql = "SELECT No FROM Port_Emp WHERE NO IN "
                      + "(SELECT  FK_Emp  FROM Port_EmpStation WHERE FK_Station IN (SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + dbStr + "FK_Node))"
                      + " AND  NO IN "
                      + "(SELECT FK_Emp FROM Port_EmpDept WHERE FK_Dept =" + dbStr + "FK_Dept)";

                        sql += " ORDER BY No ";

                        ps = new Paras();
                        ps.SQL = sql;
                        ps.Add("FK_Node", town.HisNode.NodeID);
                        ps.Add("FK_Dept", empDept);
                        //  dt = DBAccess.RunSQLReturnTable(ps);
                    }
                    else
                    {
                        return dt;
                    }
                }

                dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count == 0)
                {
                    NodeStations nextStations = town.HisNode.NodeStations;
                    if (nextStations.Count == 0)
                        throw new Exception("节点没有岗位:" + town.HisNode.NodeID + "  " + town.HisNode.Name);
                    //else
                    //    return dt;
                }
                else
                {
                    bool isInit = false;
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr[0].ToString() == BP.Web.WebUser.No)
                        {
                            /* 如果岗位分组不一样，并且结果集合里还有当前的人员，就说明了出现了当前操作员，拥有本节点上的岗位也拥有下一个节点的工作岗位
                             导致：节点的分组不同，传递到同一个人身上。 */
                            isInit = true;
                        }
                    }
#warning edit by peng, 用来确定不同岗位集合的传递包含同一个人的处理方式。
                    //  if (isInit == false || isInit == true)
                    return dt;
                }
            }

            // 没有查询到的情况下, 执行查询隶属本部门的下级部门人员。
            if (flowAppType == FlowAppType.Normal)
            {
                switch (BP.Sys.SystemConfig.AppCenterDBType)
                {
                    case DBType.MSSQL:
                    case DBType.MySQL:
                        sql = "SELECT No FROM Port_Emp x inner join "
                   + " (select FK_Emp from Port_empStation a inner join WF_NodeStation b on a.FK_Station=b.FK_Station where b.FK_Node=" + town.HisNode.NodeID + ") as y on x.No=y.FK_Emp "
                   + "  inner join Port_EmpDept z on x.No= z.FK_Emp and z.FK_Dept LIKE '" + empDept + "%' "
                   + " where x.No!=" + dbStr + "FK_Emp";
                        sql += " ORDER BY x.No ";
                        break;
                    default:
                        sql = "SELECT No FROM Port_Emp WHERE NO IN "
                   + "(SELECT  FK_Emp  FROM Port_EmpStation WHERE FK_Station IN (SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + town.HisNode.NodeID + ") )"
                   + " AND  NO IN "
                   + "(SELECT  FK_Emp  FROM Port_EmpDept WHERE FK_Dept LIKE '" + empDept + "%')"
                   + " AND No!=" + dbStr + "FK_Emp";
                        sql += " ORDER BY No ";
                        break;
                }

                ps = new Paras();
                ps.SQL = sql;
                ps.Add("FK_Emp", empNo);

            }

            if (flowAppType == FlowAppType.PRJ)
            {
                sql = "SELECT  FK_Emp  FROM Prj_EmpPrjStation WHERE FK_Prj=" + dbStr + "FK_Prj1 AND FK_Station IN (SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + dbStr + "FK_Node )"
                    + " AND  FK_Prj=" + dbStr + "FK_Prj2 ";
                sql += " ORDER BY FK_Emp ";

                ps = new Paras();
                ps.SQL = sql;
                ps.Add("FK_Prj1", prjNo);
                ps.Add("FK_Node", town.HisNode.NodeID);
                ps.Add("FK_Prj2", prjNo);
                dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count == 0)
                {
                    /* 如果项目组里没有工作人员就提交到公共部门里去找。*/
                    switch (BP.Sys.SystemConfig.AppCenterDBType)
                    {
                        case DBType.MySQL:
                        case DBType.MSSQL:
                            sql = "SELECT No FROM Port_Emp x inner join "
                   + "(select FK_Emp from Port_empStation a inner join WF_NodeStation b on a.FK_Station=b.FK_Station where b.FK_Node=" + dbStr + "FK_Node) as y on x.No=y.FK_Emp "
                   + "  inner join Port_EmpDept z on x.No= z.FK_Emp and z.FK_Dept LIKE '" + empDept + "%' "
                   + "  where x.No!=" + dbStr + "FK_Emp";
                            sql += " ORDER BY x.No ";
                            break;
                        default:
                            sql = "SELECT No FROM Port_Emp WHERE No IN "
                   + "(SELECT  FK_Emp  FROM Port_EmpStation WHERE FK_Station IN (SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + dbStr + "FK_Node) )"
                   + " AND  NO IN "
                   + "(SELECT  FK_Emp  FROM Port_EmpDept WHERE FK_Dept LIKE '" + empDept + "%')"
                   + " AND No!=" + dbStr + "FK_Emp";
                            sql += " ORDER BY No ";
                            break;
                    }


                    ps = new Paras();
                    ps.SQL = sql;
                    ps.Add("FK_Node", town.HisNode.NodeID);
                    ps.Add("FK_Emp", empNo);
                }
                else
                {
                    return dt;
                }
            }

            dt = DBAccess.RunSQLReturnTable(ps);
            if (dt.Rows.Count == 0)
            {
                NodeStations nextStations = town.HisNode.NodeStations;
                if (nextStations.Count == 0)
                    throw new Exception("节点没有岗位:" + town.HisNode.NodeID + "  " + town.HisNode.Name);
            }
            else
            {
                return dt;
            }

            /* 没有查询到的情况下, 按照最大匹配数 提高一个级别计算，递归算法未完成。
             * 因为:以上已经做的岗位的判断，就没有必要在判断其它类型的节点处理了。
             * */
            string nowDeptID = empDept.Clone() as string;
            while (true)
            {
                BP.Port.Dept myDept = new BP.Port.Dept(nowDeptID);
                nowDeptID = myDept.ParentNo;
                if (nowDeptID == "-1" || nowDeptID.ToString() == "0")
                {
                    break; /*一直找到了最高级仍然没有发现，就跳出来循环从当前操作员人部门向下找。*/
                    throw new Exception("@按岗位计算没有找到(" + town.HisNode.Name + ")接受人.");
                }

                //检查指定的部门下面是否有该人员.
                DataTable mydtTemp = this.Func_GenerWorkerList_DiGui(nowDeptID, empNo);
                if (mydtTemp == null )
                {
                    /*如果父亲级没有，就找父级的平级. */
                    BP.Port.Depts myDepts = new BP.Port.Depts();
                    myDepts.Retrieve(BP.Port.DeptAttr.ParentNo, myDept.ParentNo);
                    foreach (BP.Port.Dept item in myDepts)
                    {
                        if (item.No == nowDeptID)
                            continue;
                       mydtTemp = this.Func_GenerWorkerList_DiGui(item.No, empNo);
                       if (mydtTemp == null)
                           continue;
                       else
                           return mydtTemp;
                    }

                    continue; /*如果平级也没有，就continue.*/
                }
                else
                    return mydtTemp;
            }

            /*如果向上找没有找到，就考虑从本级部门上向下找。 */
            nowDeptID = empDept.Clone() as string;
            BP.Port.Depts subDepts = new BP.Port.Depts(nowDeptID);

            //递归出来子部门下有该岗位的人员
            DataTable mydt = Func_GenerWorkerList_DiGui_ByDepts(subDepts, empNo);
            if (mydt == null)
                throw new Exception("@按岗位计算没有找到(" + town.HisNode.Name + ")接受人.");
            return mydt;
            #endregion  按照岗位来执行。
        }
        /// <summary>
        /// 递归出来子部门下有该岗位的人员
        /// </summary>
        /// <param name="subDepts"></param>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public DataTable Func_GenerWorkerList_DiGui_ByDepts(BP.Port.Depts subDepts, string empNo)
        {
            foreach (BP.Port.Dept item in subDepts)
            {
                DataTable dt = Func_GenerWorkerList_DiGui(item.No, empNo);
                if (dt != null)
                    return dt;

                dt = Func_GenerWorkerList_DiGui_ByDepts(item.HisSubDepts, empNo);
                if (dt != null)
                    return dt;
            }
            return null;
        }
        /// <summary>
        /// 根据部门获取下一步的操作员
        /// </summary>
        /// <param name="deptNo"></param>
        /// <param name="emp1"></param>
        /// <returns></returns>
        public DataTable Func_GenerWorkerList_DiGui(string deptNo, string empNo)
        {

            string sql = "SELECT a.FK_Emp FROM "
              + " Port_EmpStation a, WF_NodeStation b , Port_EmpDept c "
              + " WHERE a.FK_Station=b.FK_Station"
              + " AND a.FK_Emp=c.FK_Emp "
              + " AND b.FK_Node=" + dbStr + "FK_Node"
              + " AND c.FK_Dept=" + dbStr + "FK_Dept"
              + " AND a.FK_Emp !=" + dbStr + "FK_Emp ";

            ps = new Paras();
            ps.SQL = sql;
            ps.Add("FK_Node", town.HisNode.NodeID);
            ps.Add("FK_Dept", deptNo);
            ps.Add("FK_Emp", empNo);

            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            if (dt.Rows.Count == 0)
            {
                NodeStations nextStations = town.HisNode.NodeStations;
                if (nextStations.Count == 0)
                    throw new Exception("@节点没有岗位:" + town.HisNode.NodeID + "  " + town.HisNode.Name);

                sql = "SELECT No FROM Port_Emp WHERE No IN ";
                sql += "(SELECT  FK_Emp  FROM Port_EmpStation WHERE FK_Station IN (SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + dbStr + "FK_Node ) )";
                sql += " AND No IN ";

                if (deptNo == "1")
                {
                    sql += "(SELECT FK_Emp FROM Port_EmpDept WHERE FK_Emp!=" + dbStr + "FK_Emp ) ";
                }
                else
                {
                    BP.Port.Dept deptP = new BP.Port.Dept(deptNo);
                    sql += "(SELECT FK_Emp FROM Port_EmpDept WHERE FK_Emp!=" + dbStr + "FK_Emp AND FK_Dept = '" + deptP.ParentNo + "')";
                }

                ps = new Paras();
                ps.SQL = sql;
                ps.Add("FK_Node", town.HisNode.NodeID);
                ps.Add("FK_Emp", empNo);
                dt = DBAccess.RunSQLReturnTable(ps);
                //if (dt.Rows.Count == 0)
                //{
                //    sql = "SELECT No FROM Port_Emp WHERE No!=" + dbStr + "FK_Emp AND No IN ";
                //    sql += "(SELECT  FK_Emp  FROM Port_EmpStation WHERE FK_Station IN (SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + dbStr + "FK_Node ) )";
                //    ps = new Paras();
                //    ps.SQL = sql;
                //    ps.Add("FK_Emp", empNo);
                //    ps.Add("FK_Node", town.HisNode.NodeID);
                //    dt = DBAccess.RunSQLReturnTable(ps);
                //    if (dt.Rows.Count == 0)
                //        throw new Exception("@岗位(" + town.HisNode.HisStationsStr + ")下没有人员，对应节点:" + town.HisNode.Name);
                //}
                if (dt.Rows.Count == 0)
                    return null;
                return dt;
            }
            else
            {
                return dt;
            }
            return null;
        }
        /// <summary>
        /// 执行找人
        /// </summary>
        /// <returns></returns>
        public DataTable DoIt(Flow fl, WorkNode currWn, WorkNode toWn)
        {
            // 给变量赋值.
            this.fl = fl;
            this.currWn = currWn;
            this.town = toWn;
            this.WorkID = currWn.WorkID;

            //如果到达的节点是按照workflow的模式。
            if (toWn.HisNode.HisDeliveryWay != DeliveryWay.ByCCFlowBPM)
                return this.FindByWorkFlowModel();

            // 规则集合.
            FindWorkerRoles ens = new FindWorkerRoles(town.HisNode.NodeID);
            foreach (FindWorkerRole en in ens)
            {
                en.fl = this.fl;
                en.town = toWn;
                en.currWn = currWn;
                en.HisNode = currWn.HisNode;
                en.WorkID = this.WorkID;

                DataTable dt = en.GenerWorkerOfDataTable();
                if (dt==null || dt.Rows.Count == 0)
                    continue;
                return dt;
            }

            //没有找到人的情况，就返回空.
            return null;
        }
    }
}