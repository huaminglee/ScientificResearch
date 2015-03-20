using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IBLL 
{
    public interface IJieJueSiLuService : IBaseService<Process_SiLu>
    {
        List<Process_SiLu> GetHistoryData(CCFlowArgs args);

        void InsertOrUpdateTrack(CCFlowArgs args);

        Dictionary<string, string> AddSiLu(Process_SiLu silu);

        Dictionary<string, string> DeleteSiLu(string slNo);

        Dictionary<string, string> ModifySiLu(string oldNo, Process_SiLu silu);

        List<Process_SiLu> GetJieJueSiLuHistoryDataFromTrack(CCFlowArgs args);
    }
}