using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IBLL 
{
    public interface IDuiBiFenXiService : IBaseService<Process_DuiBiFenXi>
    {
        List<Process_DuiBiFenXi> GetHistoryData(CCFlowArgs args);

        void InsertOrUpdateTrack(CCFlowArgs args);

        Dictionary<string, string> AddDuiBiFenXi(Process_DuiBiFenXi duibifenxi);

        Dictionary<string, string> DeleteDuiBiFenXi(string dbfxNo);

        Dictionary<string, string> ModifyDuiBiFenXi(string oldNo, Process_DuiBiFenXi duibifenxi);

        List<Process_DuiBiFenXi> GetDuiBiFenXiHistoryDataFromTrack(CCFlowArgs args);
    }
}