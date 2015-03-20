using BP.WF;
using ScientificResearchPrj.DAL;
using ScientificResearchPrj.IBLL;
using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.BLL 
{
    public class CommonOperationService :BaseService<Object>, ICommonOperationService
    {
        public override void SetCurrentDAL()
        {
            CurrentDAL = this.DbSession.CommonOperationDAL;
        }

        public DataTable GetReturnInfo(ReturnNodeModel retNode)
        {
            return (CurrentDAL as CommonOperationDAL).SelectReturnInfo(retNode);
        }

        public string SendWorks(CCFlowArgs args)
        {
            //发起不使用Node_StartWork的原因：当工作是退回的时候，且退回到第一个节点的话，
            //不加上“退回”判断的话会新生成workid,即生成新的流程。
            BP.WF.SendReturnObjs objs = null;
            if (args.ExpendArgs != null)
            {
                //调用发送api, 返回发送对象.
                objs = BP.WF.Dev2Interface.Node_SendWork(args.FK_Flow, args.WorkID, args.ExpendArgs, 0, null);
            }
            else
            {
                objs = BP.WF.Dev2Interface.Node_SendWork(args.FK_Flow, args.WorkID, 0, null);
            }
            return objs.ToMsgOfText();
        }

        public string GetTrackURL(string fk_flow)
        {
            string nowTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            string url = "/WF/Chart.aspx?FK_Flow=" + fk_flow + "&DoType=Chart&T=" + nowTime;
            return url;
        }
         
        public DataTable GetCanReturnNodes(CCFlowArgs args)
        {
            return BP.WF.Dev2Interface.DB_GenerWillReturnNodes(args.FK_Node, args.WorkID, args.FID);
        }

        public WorkNode GetPreviousWorkNode(CCFlowArgs args)
        {
            //上次节点
            WorkNode wn = new WorkNode(args.WorkID, args.FK_Node);
            WorkNode pwn = wn.GetPreviousWorkNode();
            return pwn;
        }

        public string ReturnWork(ReturnNodeModel retNode)
        {
            ///ToDo------人员分离
            ///
            int returnToNodeID = Convert.ToInt32(retNode.ReturnNodeInfo.Split('&')[0]);
            string returnToNodeEmp = retNode.ReturnNodeInfo.Split('&')[1];
            //执行退回api.
            string rInfo = BP.WF.Dev2Interface.Node_ReturnWork(retNode.FK_Flow, retNode.WorkID, retNode.FID,
                retNode.FK_Node, returnToNodeID, returnToNodeEmp, retNode.TuiHuiLiYou, retNode.IsBackTracking);
            return rInfo;
        }

        public string DoOverFlow(FlowOver args)
        {
            return BP.WF.Dev2Interface.Flow_DoFlowOver(args.FK_Flow, args.WorkID, args.OverMsg);
        }

        public DataTable GetCurrentFlowInfoFromEmpWorks(CCFlowArgs args)
        {
            DataTable flowInfoTable = (CurrentDAL as CommonOperationDAL).SelectCurrentFlowInfoFromEmpWorks(args);
            
            foreach (DataRow row in flowInfoTable.Rows)
            {
                if (row["Starter"] != null) {
                    string empNo = row["Starter"] as string;
                    MyPort_Emp starter = this.DbSession.EmpDAL.LoadEntities(a => a.EmpNo == empNo).FirstOrDefault();
                    if (starter != null)
                    {
                        row["StarterName"] = starter.Name;
                    }
                }
            }

            return flowInfoTable;
        }

        public DataTable GetPreviousNodeInfo(CCFlowArgs args) 
        {
            DataTable preNodesInfoTable = (CurrentDAL as CommonOperationDAL).SelectPreviousNodeInfo(args);
            preNodesInfoTable.Columns.Add("EmpName");

            foreach (DataRow row in preNodesInfoTable.Rows)
            {
                if (row["FK_Emp"] != null)
                {
                    string empNo = row["FK_Emp"] as string;
                    MyPort_Emp emp = this.DbSession.EmpDAL.LoadEntities(a => a.EmpNo == empNo).FirstOrDefault();
                    if (emp != null)
                    {
                        row["EmpName"] = emp.Name;
                    }
                }
            }

            return preNodesInfoTable;
        }
    }
}