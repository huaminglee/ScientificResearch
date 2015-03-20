using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IDAL 
{
    public interface IJieJueSiLuDAL : IBaseDAL<Process_SiLu>
    {
        int SelectMaxOid();

        void UpdateTrackSLOID(int oldSLOID, int newSLOID);
        void UpdateXingShiHuaSLOID(int oldSLOID, int newSLOID);
    }
}