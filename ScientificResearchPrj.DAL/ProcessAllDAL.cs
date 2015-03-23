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
            string sql = "SELECT  a.*,b.IsRead,c.Name as FlowSortName FROM  WF_GenerWorkFlow a, WF_GenerWorkerlist  b,WF_FlowSort c where a.WorkID = b.WorkID and a.FK_Node = b.FK_Node  and b.IsPass =0 and a.FK_FlowSort = c.No";
            return BP.DA.DBAccess.RunSQLReturnTable(sql);
        }

        public DataTable SelectSMS(string sentTo, string msgFlag) {
            Paras ps = new Paras();
            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;

            ps.SQL = "SELECT * FROM  Sys_SMS where SendTo=" + dbstr + "SendTo and MsgFlag=" + dbstr + "MsgFlag";

            ps.Add("SendTo", sentTo);
            ps.Add("MsgFlag", msgFlag);
            return BP.DA.DBAccess.RunSQLReturnTable(ps);
        }
    }
}
