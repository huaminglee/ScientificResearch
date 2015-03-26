using BP.DA;
using ScientificResearchPrj.DAL;
using ScientificResearchPrj.IBLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ScientificResearchPrj.BLL
{
    public class FaQiService : BaseService<Object>, IFaQiService
    {
        public override void SetCurrentDAL()
        {
            CurrentDAL = this.DbSession.FaQiDAL;
        }

        public DataTable GetCanStartFlows()
        {
            DataTable canStartFlows = BP.WF.Dev2Interface.DB_GenerCanStartFlowsOfDataTable(BP.Web.WebUser.No);
            //添加父节点字段           
            canStartFlows.Columns.Add("_parentId");
            //添加发起链接字段
            canStartFlows.Columns.Add("StartLink");
            //添加历史发起链接字段
            canStartFlows.Columns.Add("HistoryFK_Flow");
            //添加轨迹图查看字段
            canStartFlows.Columns.Add("TrackLink");
            //添加tree的ID节点字段           
            canStartFlows.Columns.Add("TreeID");

            DataTable parentClass = DataTableClone.CloneStructureFromDataTable(canStartFlows);
            NoRepeatClass noRepeatClass = new NoRepeatClass();
            string nowTime = DateTime.Now.ToString("yyyyMMddHHmmss");

            //构建子流程的父分类
            foreach (DataRow row in canStartFlows.Rows)
            {
                //更新子节点的TreeID
                row["TreeID"] = row["No"];
                //更新当前父节点编号,以流程所属类别作为分类依据！！！
                //加上时间是为了防止FK_FlowSort与子节点的No一样，父节点No赋值为FK_FlowSort导致tree的id重复
                row["_parentId"] = row["FK_FlowSort"] + nowTime;
                //更新发起链接字段
                row["StartLink"] = "/WF/MyFlow.aspx?FK_Flow=" + row["No"] + "&FK_Node=" + int.Parse(row["No"].ToString()) + "01&CanEdit=1";
                //更新历史发起链接字段
                row["HistoryFK_Flow"] = row["No"];
                //更新查看流程图链接字段
                row["TrackLink"] = "/WF/Chart.aspx?FK_Flow=" + row["No"] + "&DoType=Chart&T=" + nowTime;

                if (noRepeatClass.IfClassExist(row["FK_FlowSort"].ToString()))
                {
                    continue;
                }
                noRepeatClass.AddClass(row["FK_FlowSort"].ToString(), row["FK_FlowSort"].ToString());

                DataRow parentNewRow = parentClass.NewRow();
                //作为树的Id，不能为空，应该与子节点的_parentId一致
                parentNewRow["TreeID"] = row["FK_FlowSort"] + nowTime;
                parentNewRow["Name"] = row["FK_FlowSortText"];
                parentNewRow["FK_FlowSort"] = "9999";
                parentClass.Rows.Add(parentNewRow);
            }

            canStartFlows.Merge(parentClass);
            return canStartFlows;
        }

        public DataTable GetHisToryStartFlows(string fk_flow)
        {
            return (CurrentDAL as FaQiDAL).SelectHistoryStartFlows(fk_flow);
        }
        
    }
}
