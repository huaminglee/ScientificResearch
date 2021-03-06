﻿using BP.DA;
using BP.WF;
using ScientificResearchPrj.IBLL;
using ScientificResearchPrj.IDAL;
using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ScientificResearchPrj.BLL
{
    public class JournalService : BaseService<Object>, IJournalService
    {
        public override void SetCurrentDAL()
        {
            CurrentDAL = this.DbSession.JournalDAL;
        }

        public string FlowSearchMethod()
        {
            FlowSorts fss = new FlowSorts();
            fss.RetrieveAll();
            Flows fls = new Flows();
            fls.RetrieveAll();
            StringBuilder appFlow = new StringBuilder();
            appFlow.Append("{");
            appFlow.Append("\"rows\":[");

            foreach (FlowSort fs in fss)
            {
                if (appFlow.Length == 9) { appFlow.Append("{"); } else { appFlow.Append(",{"); }
                if (fs.ParentNo + "" == "0")
                {
                    appFlow.Append(string.Format("\"No\":\"{0}\",\"Name\":\"{1}\",\"NumOfBill\":\"{2}\",\"_parentId\":null,\"state\":\"closed\",\"Element\":\"sort\"", fs.No, fs.Name, "0"));
                }
                else
                {
                    appFlow.Append(string.Format("\"No\":\"{0}\",\"Name\":\"{1}\",\"NumOfBill\":\"{2}\",\"_parentId\":\"{3}\",\"state\":\"closed\",\"Element\":\"sort\"", fs.No, fs.Name, "0", fs.ParentNo));
                }
                appFlow.Append("}");
            }

            foreach (FlowSort fs in fss)
            {
                foreach (Flow fl in fls)
                {
                    if (fl.FK_FlowSort != fs.No)
                        continue;

                    if (appFlow.Length == 9) { appFlow.Append("{"); } else { appFlow.Append(",{"); }

                    appFlow.Append(string.Format("\"No\":\"{0}\",\"Name\":\"{1}\",\"NumOfBill\":\"{2}\",\"_parentId\":\"{3}\",\"Element\":\"flow\"", fl.No, fl.Name, fl.NumOfBill, fl.FK_FlowSort));
                    appFlow.Append("}");
                }
            }
            appFlow.Append("]");
            appFlow.Append(",\"total\":" + fls.Count + fss.Count + "");
            appFlow.Append("}");
            return appFlow.ToString();
        }

        public PageModelForDataTable SearchRpt(string RptNo, int pageSize, int pageNow) {
            PageModelForDataTable pageModel = new PageModelForDataTable();
            int totalCount = (CurrentDAL as IJournalDAL).SelectRptTotalCount(RptNo);
            DataTable table = (CurrentDAL as IJournalDAL).SelectRpt(RptNo, pageSize, pageNow);
            
            if (table != null && table.Rows != null) {
                table.Columns.Add("FlowStarterName");
                table.Columns.Add("FlowEnderName");
                table.Columns.Add("DeptName");
                table.Columns.Add("FlowEndNodeName");
                table.Columns.Add("WFStateName");

                foreach (DataRow r in table.Rows) {
                    r["FlowStarterName"] = GetEmpName(r["FlowStarter"].ToString());
                    r["FlowEnderName"] = GetEmpName(r["FlowEnder"].ToString());
                    r["DeptName"] = GetDeptName(r["FK_Dept"].ToString());
                    r["FlowEndNodeName"] = GetNodeName(r["FlowEndNode"].ToString());
                    r["WFStateName"] = WFStateTrans.GetWFStateStr(r["WFState"].ToString());
                }
            }

            if (table != null) pageModel.SetTable(table);
            pageModel.SetPageNo(pageNow);
            pageModel.SetPageSize(pageSize);
            pageModel.SetTotalCount(totalCount);

            return pageModel;
        }

        private string GetEmpName(string empNo) { 
            MyPort_Emp emp=this.DbSession.EmpDAL.LoadEntities(a=>a.EmpNo==empNo).FirstOrDefault();
            if (emp != null) {
                return emp.Name;
            }
            return "";
        }

        private string GetDeptName(string deptNo)
        {
            MyPort_Dept dept = this.DbSession.DepartmentDAL.LoadEntities(a => a.DeptNo == deptNo).FirstOrDefault();
            if (dept != null)
            {
                return dept.Name;
            }
            return "";
        }

        private string GetNodeName(string nodeNo) 
        {
            Node node = new Node(nodeNo);
            if (node != null) {
                return node.Name;
            }
            return "";
        }

        public DataTable ReadTrack(CCFlowArgs args)
        {
            try
            {
                DataTable table = BP.WF.Dev2Interface.DB_GenerTrack(args.FK_Flow, args.WorkID, args.FID).Tables["Track"];
                if (table != null && table.Rows != null && table.Rows.Count != 0) {
                    table.Columns.Add("FormLink");
                    table.Columns.Add("ChartLink");

                    foreach (DataRow row in table.Rows) {
                        row["FormLink"] = "/WF/MyFlow.aspx?FK_Flow=" + args.FK_Flow + "&FK_Node=" + row["NDFrom"] + "&FID=" + row["FID"] + "&WorkID=" + row["WorkID"] + "&Paras=" + "&T=" + DateTime.Now.ToString("yyyyMMddHHmmss") + "&CanEdit=0";
                        row["ChartLink"] = "/WF/Chart.aspx?FK_Flow=" + args.FK_Flow + "&DoType=Chart&T=" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    }
                }
                return table;
            }
            catch (Exception e) {
                return new DataTable();
            }
        }
    }
}
