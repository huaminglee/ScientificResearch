using BP.DA;
using ScientificResearchPrj.IDAL;
using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScientificResearchPrj.DAL
{
    public class ProcessAllDAL : BaseDAL<Object>, IProcessAllDAL
    {
        public DataTable SelectAllProcess()
        {
            //当前处理人的IsPass状态永远是0,要判断当前流程是否已经IsRead，则应该取当前处理者的流程数据
            string sql = "SELECT  a.* ,c.Name as FlowSortName FROM  WF_GenerWorkFlow a, WF_FlowSort c where a.FK_FlowSort = c.No";
            return BP.DA.DBAccess.RunSQLReturnTable(sql);
        }

        public DataTable SelectCurrentGenerWorkerlistIsRead(long WorkID, string FK_Node)
        {
            Paras ps = new Paras();
            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            ps.SQL = "SELECT IsRead From WF_GenerWorkerlist Where WorkID=" + dbstr + "WorkID and FK_Node=" + dbstr + "FK_Node and IsPass =0";

            ps.Add("WorkID", WorkID);
            ps.Add("FK_Node", FK_Node);
            return BP.DA.DBAccess.RunSQLReturnTable(ps);
        }
    }
}
