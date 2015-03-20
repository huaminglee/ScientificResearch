using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IBLL 
{
    public interface IXingShiHuaService : IBaseService<Process_XingShiHua>
    {
        List<Process_XingShiHua> GetHistoryData(CCFlowArgs args);

        void InsertOrUpdateTrack(CCFlowArgs args);

        Dictionary<string, string> AddXingShiHua(Process_XingShiHua xingshihua);

        Dictionary<string, string> DeleteXingShiHua(string xshNo);

        Dictionary<string, string> ModifyXingShiHua(string oldNo, Process_XingShiHua xingshihua);

        List<Process_XingShiHua> GetXingShiHuaHistoryDataFromTrack(CCFlowArgs args);
    }
}