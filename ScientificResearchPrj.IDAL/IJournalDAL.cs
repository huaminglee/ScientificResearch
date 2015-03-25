using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IDAL 
{
    public interface IJournalDAL : IBaseDAL<Object>
    {
        DataTable SelectRpt(string RptNo, int pageSize, int pageNow);

        int SelectRptTotalCount(string RptNo);
    }
}