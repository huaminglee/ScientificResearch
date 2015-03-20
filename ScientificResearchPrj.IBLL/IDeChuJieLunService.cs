using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IBLL 
{
    public interface IDeChuJieLunService : IBaseService<Process_DeChuJieLun>
    {
        List<Process_DeChuJieLun> GetHistoryData(CCFlowArgs args);

        void InsertOrUpdateTrack(CCFlowArgs args);

        Dictionary<string, string> AddJieLun(Process_DeChuJieLun jielun);

        Dictionary<string, string> DeleteJieLun(string jlNo);

        Dictionary<string, string> ModifyJieLun(string oldNo, Process_DeChuJieLun jielun);

        List<Process_DeChuJieLun> GetJieLunHistoryDataFromTrack(CCFlowArgs args);
    }
}