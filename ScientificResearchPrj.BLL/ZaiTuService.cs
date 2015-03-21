using BP.DA;
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
            //添加权限字段           
            runningFlows.Columns.Add("OpenRight");

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
                row["TitleLink"] = GetTitleLink(row);
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
            runningFlows.Merge(parentClass);
            return runningFlows;
        }

        private string GetTitleLink(DataRow row)
        {
            string nowTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            //若当前登陆者是处理人员，可以编辑；若是发起者，可以查看不能编辑；若是历史参与者，可以查看不能编辑
            //特别注意！！！队列模式下，若a先完成，工作转到b，此时a不算在历史处理人里面，因为a的工作状态不算为通过（即WF_GenerWorkerlist的IsPass不为1，而是-99），只有当前工作节点通过以后才算在历史处理人里面
                if (CurrentLoginUserIsDoEmp(row) == true)
                {
                    row["OpenRight"] = 3;//3表示能查看能编辑
                    string paras = row["AtPara"] as string;
                    return "/WF/MyFlow.aspx?FK_Flow=" + row["FK_Flow"] + "&FK_Node=" + row["FK_Node"] + "&FID=" + row["FID"] + "&WorkID=" + row["WorkID"] + "&Paras=" + paras + "&T=" + nowTime + "&CanEdit=1";
                }
                else if (CurrentLoginUserIsStarter(row) == true)
                {
                    row["OpenRight"] = 2;//2表示发起者能查看不能编辑
                    string paras = row["AtPara"] as string;
                    return "/WF/MyFlow.aspx?FK_Flow=" + row["FK_Flow"] + "&FK_Node=" + row["FK_Node"] + "&FID=" + row["FID"] + "&WorkID=" + row["WorkID"] + "&Paras=" + paras + "&T=" + nowTime + "&CanEdit=0";
                }
                else 
                {
                    row["OpenRight"] = 1;//1表示历史参与者能查看不能编辑
                    string paras = row["AtPara"] as string;
                    return "/WF/MyFlow.aspx?FK_Flow=" + row["FK_Flow"] + "&FK_Node=" + row["FK_Node"] + "&FID=" + row["FID"] + "&WorkID=" + row["WorkID"] + "&Paras=" + paras + "&T=" + nowTime + "&CanEdit=0";
                }
                
        }

        private bool CurrentLoginUserIsDoEmp(DataRow row)
        {
            CCFlowArgs args = new CCFlowArgs();
            args.FK_Flow = row["FK_Flow"].ToString();
            args.FK_Node = Convert.ToInt32(row["FK_Node"].ToString());
            args.WorkID = Convert.ToInt32(row["WorkID"].ToString());
            args.FID = Convert.ToInt32(row["FID"].ToString());

            DataTable table = DbSession.CommonOperationDAL.SelectUnPassedFlowWithFK_Node(args);

            if (table != null && table.Rows != null)
            {
                foreach (DataRow r in table.Rows)
                {
                    if (r["FK_Emp"].ToString().Equals(BP.Web.WebUser.No))
                        return true;
                }
            }
            return false;
        }

        private bool CurrentLoginUserIsStarter(DataRow row)
        {
            if (row != null)
            {
                if (BP.Web.WebUser.No.Equals(row["Starter"].ToString()))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
