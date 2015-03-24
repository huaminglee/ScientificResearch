using BP.DA;
using BP.WF;
using ScientificResearchPrj.IBLL;
using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ScientificResearchPrj.BLL
{
    public class DaiBanService : BaseService<Object>, IDaiBanService
    {
        public override void SetCurrentDAL()
        {
            CurrentDAL = this.DbSession.DaiBanDAL;
        }

        public DataTable GetTodoFlows()
        {
            DataTable todoFlows = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable();
            Dictionary<string, string> ccArg = new Dictionary<string, string>();
            ccArg["columnName"] = "AtPara";
            ccArg["columnValue"] = "@IsCC=1";
            todoFlows = DataTableClone.CloneDataFromDataTable(todoFlows, ccArg);
            //添加父节点字段           
            todoFlows.Columns.Add("_parentId");
            //添加tree的ID节点字段           
            todoFlows.Columns.Add("TreeID");
            //添加标题链接字段           
            todoFlows.Columns.Add("TitleLink");
            //添加状态字段           
            todoFlows.Columns.Add("State");
            //添加催办字段           
            todoFlows.Columns.Add("Press");

            DataTable parentClass = DataTableClone.CloneStructureFromDataTable(todoFlows);
            NoRepeatClass noRepeatClass = new NoRepeatClass();
            string nowTime = DateTime.Now.ToString("yyyyMMddHHmmss");

            //构建子流程的父分类
            foreach (DataRow row in todoFlows.Rows)
            {
                var flowName = row["FlowName"];
                //更新名称提示
                row["FlowName"] = row["FlowName"] + "<font color=red>(" + WFStateTrans.GetWFStateStr(row["WFState"].ToString()) + ")</font>";
                //更新子节点的TreeID
                row["TreeID"] = row["WorkID"];
                //更新当前父节点编号, 以流程编号作为分类依据！！！
                row["_parentId"] = row["FK_Flow"] + nowTime;
                //更新TitleLink字段
                string paras = row["AtPara"] as string;
                int isRead = Convert.ToInt32(row["IsRead"]);
                if (isRead == 0) {
                    row["TitleLink"] =
                        "/WF/MyFlow.aspx?FK_Flow=" + row["FK_Flow"] + "&FK_Node=" + row["FK_Node"] + "&FID=" + row["FID"] + "&WorkID=" + row["WorkID"] + "&Paras=" + paras + "&IsRead=0&T=" + nowTime;
                }
                else
                {
                    row["TitleLink"] =
                        "/WF/MyFlow.aspx?FK_Flow=" + row["FK_Flow"] + "&FK_Node=" + row["FK_Node"] + "&FID=" + row["FID"] + "&WorkID=" + row["WorkID"] + "&Paras=" + paras + "&T=" + nowTime;
                }
                //更新回执字段
                row["Press"] = GetPress(row);
                //更新状态
                DateTime mysdt = DataType.ParseSysDate2DateTime(row["SDTOfNode"] as string);
                DateTime cdt = DateTime.Now;
                if (DateTime.Compare(mysdt, cdt) < 0)
                {
                    row["State"] = "<font color=red>逾期</font>";
                }
                else
                {
                    row["State"] = "正常";
                }

                if (noRepeatClass.IfClassExist(row["FK_FlowSort"].ToString()))
                {
                    continue;
                }
                noRepeatClass.AddClass(row["FK_FlowSort"].ToString(), row["FK_FlowSort"].ToString());

                DataRow parentNewRow = parentClass.NewRow();
                //类别对应的编号,作为树的Id，不能为空，应该与子节点的_parentId一致
                parentNewRow["TreeID"] = row["FK_Flow"] + nowTime;
                //类别对应的名称
                FlowSort flowSort = new FlowSort(row["FK_FlowSort"].ToString());
                if (flowSort != null)
                {
                    parentNewRow["FlowName"] = flowSort.Name;
                }

                parentClass.Rows.Add(parentNewRow);
            }
            todoFlows.Merge(parentClass);
            return todoFlows;
        }

        private string GetPress(DataRow row)
        {
            string fk_Node = row["FK_Node"].ToString();
            string workID = row["WorkID"].ToString();
            string msgFlag = "Press" + workID + "_" + fk_Node;
            string pressSender = "";

            DataTable table = this.DbSession.CommonOperationDAL.SelectSMS(BP.Web.WebUser.No, msgFlag);
            if (table != null && table.Rows != null)
            {
                foreach (DataRow r in table.Rows)
                {
                    pressSender += "【发送者】:&nbsp;&nbsp;";
                    string sender = r["Sender"].ToString();
                    MyPort_Emp emp = DbSession.EmpDAL.LoadEntities(a => a.EmpNo == sender).FirstOrDefault();
                    if (emp != null)
                    {
                        pressSender += emp.Name + "<br>";
                    }
                    pressSender += "【标题】:&nbsp;&nbsp;" + r["EmailTitle"].ToString() + "<br>";
                    pressSender += "【内容】:&nbsp;&nbsp;" + r["EmailDoc"].ToString().Substring(0, r["EmailDoc"].ToString().IndexOf("<")) + "<br><br>";
                }
            }
            return pressSender;
        }
    }
}
