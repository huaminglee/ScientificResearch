using BP.DA;
using ScientificResearchPrj.IDAL;
using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.DAL 
{
    public class JournalDAL : BaseDAL<Object>,   IJournalDAL
    {
        public DataTable SelectRpt(string RptNo, int pageSize, int pageNow)
        {
            string sql = "SELECT TOP " + pageSize + " * FROM " + RptNo + " where OID not in( select top " + (pageSize * (pageNow - 1)) + " OID from " + RptNo + " order by OID desc) order by OID desc";

            return BP.DA.DBAccess.RunSQLReturnTable(sql);
        }

        public int SelectRptTotalCount(string RptNo) {
            string sql = "SELECT count(OID) FROM " + RptNo;
            return BP.DA.DBAccess.RunSQLReturnValInt(sql);
        }
    }
}