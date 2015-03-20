using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IBLL 
{
    public interface IXuQiuFenXiService : IBaseService<Process_Subject_XuQiuFenXi>
    {
        Process_Subject_XuQiuFenXi GetHistoryData(CCFlowArgs args);

        void InsertOrUpdateTrack(CCFlowArgs args);
    }
}