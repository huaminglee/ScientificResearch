using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IDAL 
{
    public interface ISheJiSuanFaDAL : IBaseDAL<Process_SuanFa>
    {
        int SelectMaxOid();

        void UpdateTrackSJSFOID(int oldSFOID, int newSFOID);
        void UpdateShiYanSFOID(int oldSFOID, int newSFOID);
    }
}