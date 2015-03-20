using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IBLL 
{
    public interface IShenHeService : IBaseService<Process_ShenHe>
    {
        Process_ShenHe GetShenHeHistoryData(CCFlowArgs args, string shenHeRen);
        
        List<Process_ShenHe> GetShenHeHistoryDataWithoutCurrentShenHeRen(CCFlowArgs args, string shenHeRen);

        Dictionary<string, string> AddShenHe(Process_ShenHe shenhe);

        Dictionary<string, string> DeleteShenHe(int OID);

        Dictionary<string, string> ModifyShenHe(int oldOID, Process_ShenHe shenhe);

        List<Process_ShenHe> GetShenHeHistoryDataByStep(CCFlowArgs args, string stepType);
    }
}