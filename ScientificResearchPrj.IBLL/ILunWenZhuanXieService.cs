using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IBLL 
{
    public interface ILunWenZhuanXieService : IBaseService<Process_LunWen>
    {
        List<Process_LunWen> GetHistoryData(CCFlowArgs args);

        void InsertOrUpdateTrack(CCFlowArgs args);

        Dictionary<string, string> AddLunWen(Process_LunWen lunwen);

        Dictionary<string, string> DeleteLunWen(string lwNo);

        Dictionary<string, string> ModifyLunWen(string oldNo, Process_LunWen lunwen);

        List<Process_LunWen> GetLunWenHistoryDataFromTrack(CCFlowArgs args);
    }
}