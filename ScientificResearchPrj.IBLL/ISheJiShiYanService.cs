using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IBLL 
{
    public interface ISheJiShiYanService : IBaseService<Process_ShiYan>
    {
        List<Process_ShiYan> GetHistoryData(CCFlowArgs args);

        void InsertOrUpdateTrack(CCFlowArgs args);

        Dictionary<string, string> AddShiYan(Process_ShiYan shiyan);

        Dictionary<string, string> DeleteShiYan(string syNo);

        Dictionary<string, string> ModifyShiYan(string oldNo, Process_ShiYan shiyan);

        List<Process_ShiYan> GetShiYanHistoryDataFromTrack(CCFlowArgs args);
    }
}