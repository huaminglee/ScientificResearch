using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IDAL 
{
    public interface ITiChuWenTiDAL : IBaseDAL<Process_TiChuWenTi>
    {
        int SelectMaxOid();

        void UpdateTrackWTOID(int oldWTOID, int newWTOID);
        void UpdateSiLuWTOID(int oldWTOID, int newWTOID);
    }
}