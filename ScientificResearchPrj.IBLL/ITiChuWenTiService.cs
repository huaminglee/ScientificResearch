using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IBLL 
{
    public interface ITiChuWenTiService : IBaseService<Process_TiChuWenTi>
    {
        List<Process_TiChuWenTi> GetHistoryData(CCFlowArgs args);

        void InsertOrUpdateTrack(CCFlowArgs args);

        Dictionary<string, string> AddWenTi(Process_TiChuWenTi wenti);

        Dictionary<string, string> DeleteWenTi(string wtNo);

        Dictionary<string, string> ModifyWenTi(string oldNo, Process_TiChuWenTi wenti);

        List<Process_TiChuWenTi> GetTiChuWenTiHistoryDataFromTrack(CCFlowArgs args);
    }
}