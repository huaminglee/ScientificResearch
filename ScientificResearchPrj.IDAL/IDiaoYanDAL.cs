using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IDAL 
{
    public interface IDiaoYanDAL : IBaseDAL<Process_DiaoYan>
    {
        int SelectMaxOid();

        void UpdateTrackDYOID(int oldDYOID, int newDYOID);
    }
}