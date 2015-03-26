using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ScientificResearchPrj.IBLL
{
    public interface IJournalService
    {
        string FlowSearchMethod();

        PageModelForDataTable SearchRpt(string RptNo, int pageSize, int pageNow);

        DataTable ReadTrack(CCFlowArgs args);
    }
}
