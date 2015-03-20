using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ScientificResearchPrj.IBLL
{
    public interface IChaoSongService
    {
        DataTable GetAllCClist();
        DataTable GetReadCClist();
        DataTable GetUnReadCClist();
        DataTable GetDeleteCClist();
        void CCSetRead(string myPK);
        string WriteToCCList(CCModel cc);
    }
}
