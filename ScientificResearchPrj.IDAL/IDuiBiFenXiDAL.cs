using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IDAL 
{
    public interface IDuiBiFenXiDAL : IBaseDAL<Process_DuiBiFenXi>
    {
        int SelectMaxOid();

        void UpdateTrackDBFXOID(int oldDBFXOID, int newDBFXOID);
    }
}