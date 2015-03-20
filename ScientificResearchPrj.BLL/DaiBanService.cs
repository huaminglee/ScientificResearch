using BP.DA;
using ScientificResearchPrj.IBLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ScientificResearchPrj.BLL
{
    public class DaiBanService : IDaiBanService
    {

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
                row["TitleLink"] =
                    "/WF/MyFlow.aspx?FK_Flow=" + row["FK_Flow"] + "&FK_Node=" + row["FK_Node"] + "&FID=" + row["FID"] + "&WorkID=" + row["WorkID"] + "&Paras=" + paras + "&T=" + nowTime;
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
                parentNewRow["FlowName"] = flowName;
                parentClass.Rows.Add(parentNewRow);
            }
            todoFlows.Merge(parentClass);
            return todoFlows;
        }
       
    }
}
