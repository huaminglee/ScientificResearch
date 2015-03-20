using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IBLL 
{
    public interface IDiaoYanService : IBaseService<Process_DiaoYan>
    {
        List<Process_DiaoYan> GetHistoryData(CCFlowArgs args);

        void InsertOrUpdateTrack(CCFlowArgs args);

        Dictionary<string, string> AddDiaoYan(Process_DiaoYan diaoyan);

        Dictionary<string, string> DeleteDiaoYan(string No);

        Dictionary<string, string> ModifyDiaoYan(string oldNo, Process_DiaoYan diaoyan);

        List<Process_DiaoYan> GetDiaoYanHistoryDataFromTrack(CCFlowArgs args);
    }
}