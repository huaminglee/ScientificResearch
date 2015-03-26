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
    public class ZaiTuService : BaseService<Object>, IZaiTuService
    {
        public override void SetCurrentDAL()
        {
            CurrentDAL = this.DbSession.ZaiTuDAL;
        }

        public DataTable GetRunningFlows()
        {
            //获取在途工作
            DataTable runningFlows = BP.WF.Dev2Interface.DB_GenerRuningV2();
            //添加父节点字段           
            runningFlows.Columns.Add("_parentId");
            //添加tree的ID节点字段           
            runningFlows.Columns.Add("TreeID");
            //添加标题链接字段           
            runningFlows.Columns.Add("TitleLink");
            //添加状态字段           
            runningFlows.Columns.Add("State"); 
            //添加回执字段           
            runningFlows.Columns.Add("RP");
           
            DataTable parentClass = DataTableClone.CloneStructureFromDataTable(runningFlows);
            NoRepeatClass noRepeatClass = new NoRepeatClass();
            string nowTime = DateTime.Now.ToString("yyyyMMddHHmmss");

            //构建子流程的父分类
            foreach (DataRow row in runningFlows.Rows)
            {
                var flowName = row["FlowName"];
                //更新名称提示
                row["FlowName"] = row["FlowName"] + "<font color=red>(" + WFStateTrans.GetWFStateStr(row["WFState"].ToString()) + ")</font>";
                //更新子节点的TreeID
                row["TreeID"] = row["WorkID"];
                //更新当前父节点编号, 以流程编号作为分类依据！！！
                row["_parentId"] = row["FK_Flow"] + nowTime;
                //更新TitleLink字段
                row["TitleLink"] = "/Journal/ReadTrackView?FK_Flow=" + row["FK_Flow"] + "&WorkID=" + row["WorkID"] + "&FID=" + row["FID"];
                //更新回执字段
                row["RP"] = GetRP(row);
                 
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
            runningFlows.Merge(parentClass);
            return runningFlows;
        }
         
        private string GetRP(DataRow row)
        {
            string fk_Node = row["FK_Node"].ToString();
            string workID = row["WorkID"].ToString();
            string msgFlag = "RP" + workID + "_" + fk_Node;
            string rpSender = "";

            DataTable table = this.DbSession.CommonOperationDAL.SelectSMS(BP.Web.WebUser.No, msgFlag);
            if (table != null && table.Rows != null)
            {
                foreach (DataRow r in table.Rows)
                {
                    rpSender += "【发送者】:&nbsp;&nbsp;";
                    string sender = r["Sender"].ToString();
                    MyPort_Emp emp = DbSession.EmpDAL.LoadEntities(a => a.EmpNo == sender).FirstOrDefault();
                    if (emp != null)
                    {
                        rpSender += emp.Name + "<br>";
                    }
                    rpSender += "【标题】:&nbsp;&nbsp;" + r["EmailTitle"].ToString() + "<br>";
                    rpSender += "【内容】:&nbsp;&nbsp;" + r["EmailDoc"].ToString().Substring(0, r["EmailDoc"].ToString().IndexOf("<")) + "<br><br>";
                }
            }
            return rpSender;
        }
    }
}
