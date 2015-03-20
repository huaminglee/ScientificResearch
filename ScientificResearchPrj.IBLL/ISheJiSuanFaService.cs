using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IBLL 
{
    public interface ISheJiSuanFaService : IBaseService<Process_SuanFa>
    {
        List<Process_SuanFa> GetHistoryData(CCFlowArgs args);

        void InsertOrUpdateTrack(CCFlowArgs args);

        Dictionary<string, string> AddSuanFa(Process_SuanFa suanfa);

        Dictionary<string, string> DeleteSuanFa(string sfNo);

        Dictionary<string, string> ModifySuanFa(string oldNo, Process_SuanFa suanfa);

        List<Process_SuanFa> GetSuanFaHistoryDataFromTrack(CCFlowArgs args);
    }
}