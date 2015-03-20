using BP.DA;
using ScientificResearchPrj.IDAL;
using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.DAL 
{
    public class CommonOperationDAL : BaseDAL<Object>, ICommonOperationDAL
    {
        public DataTable SelectReturnInfo(ReturnNodeModel retNode)
        {
            Paras ps = new Paras();
            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            ps.SQL = "SELECT Top(1) * FROM  ND" + int.Parse(retNode.FK_Flow) + "Track  " +
                "Where ActionType=2 And FID=" + dbstr + "FID And WorkID=" + dbstr + "WorkID And NDTo=" + dbstr + "NDTo And EmpTo=" + dbstr + "EmpTo order By RDT desc;";
            ps.Add("FID", retNode.FID);
            ps.Add("WorkID", retNode.WorkID);
            ps.Add("NDTo", retNode.ReturnToNode);
            ps.Add("EmpTo", retNode.ReturnToEmps);
            return BP.DA.DBAccess.RunSQLReturnTable(ps);
        }

        public DataTable SelectCurrentFlowInfoFromEmpWorks(CCFlowArgs args)
        {
            Paras ps = new Paras();
            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            ps.SQL = "SELECT * FROM  WF_EmpWorks  " +
                "Where FK_Flow=" + dbstr + "FK_Flow And WorkID=" + dbstr + "WorkID ";
            ps.Add("FK_Flow", args.FK_Flow);
            ps.Add("WorkID", args.WorkID);
            return BP.DA.DBAccess.RunSQLReturnTable(ps);
        }

        public DataTable SelectPreviousNodeInfo(CCFlowArgs args)
        {
            Paras ps = new Paras();
            ps.SQL = "SELECT  b.FK_Emp , a.FK_NodeText FROM WF_GenerWorkerlist as a, WF_NodeEmp as b where a.FK_Node = b.FK_Node "+
                " And a.FK_Flow =" + args.FK_Flow + " And a.WorkID =" + args.WorkID +
                " And a.FID =" + args.FID + " And a.FK_Node !=" + args.FK_Node;
             
            return BP.DA.DBAccess.RunSQLReturnTable(ps);
        }
    }
}