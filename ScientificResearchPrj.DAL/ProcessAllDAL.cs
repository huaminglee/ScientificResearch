using ScientificResearchPrj.IDAL;
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
            string sql = "SELECT  A.*,B.Name as FlowSortName FROM WF_GenerWorkFlow as A, WF_FlowSort as B where A.FK_FlowSort=B.No";
            return BP.DA.DBAccess.RunSQLReturnTable(sql);
        }
    }
}
