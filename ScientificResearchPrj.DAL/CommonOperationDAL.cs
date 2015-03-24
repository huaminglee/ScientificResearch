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

        public DataTable SelectUnPassedFlowWithFK_Node(CCFlowArgs args)
        {
            string sql = "SELECT A.*, B.FK_Emp FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.WorkID=" + args.WorkID + " And A.FK_Flow=" + args.FK_Flow + " And A.FK_Node=" + args.FK_Node + " And A.FID=" + args.FID + " AND A.WorkID=B.WorkID  AND B.IsEnable=1 AND B.IsPass=0 ";

            return BP.DA.DBAccess.RunSQLReturnTable(sql);
        }

        public DataTable SelectPassedFlow(CCFlowArgs args)
        {
            string sql = "SELECT A.*, B.FK_Emp  FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.WorkID=" + args.WorkID + " And A.FK_Flow=" + args.FK_Flow + " And A.FID=" + args.FID + " AND A.WorkID=B.WorkID  AND B.IsEnable=1 AND B.IsPass=1 ";

            return BP.DA.DBAccess.RunSQLReturnTable(sql);
        }

        public bool IsPressExist(string msgFlag) {
            Paras ps = new Paras();
            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            ps.SQL = "SELECT * From Sys_SMS Where MsgFlag=" + dbstr + "MsgFlag";

            ps.Add("MsgFlag", msgFlag);

            DataTable table = BP.DA.DBAccess.RunSQLReturnTable(ps);
            if (table != null && table.Rows != null && table.Rows.Count != 0)
            {
                return true;
            }
            else {
                return false;
            }
        }

        public DataTable SelectSMS(string sentTo, string msgFlag)
        {
            Paras ps = new Paras();
            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;

            ps.SQL = "SELECT * FROM  Sys_SMS where SendTo=" + dbstr + "SendTo and MsgFlag=" + dbstr + "MsgFlag";

            ps.Add("SendTo", sentTo);
            ps.Add("MsgFlag", msgFlag);
            return BP.DA.DBAccess.RunSQLReturnTable(ps);
        }
    }
}