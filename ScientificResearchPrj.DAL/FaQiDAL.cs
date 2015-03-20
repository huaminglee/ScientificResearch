using BP.DA;
using BP.WF;
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
    public class FaQiDAL : BaseDAL<Object>, IFaQiDAL
    {
        public DataTable SelectHistoryStartFlows(string fk_flow)
        {
            Flow startFlow = new Flow(fk_flow);
            string sql = "SELECT * FROM " + startFlow.PTable + " WHERE FlowStarter='" + BP.Web.WebUser.No + "'  and WFState not in (" + (int)WFState.Blank + "," + (int)WFState.Draft + ")";
            DataTable historyStartFlows = startFlow.RunSQLReturnTable(sql);
            return historyStartFlows;
        }
         
    }
}
