using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IDAL 
{
    public interface IDeChuJieLunDAL : IBaseDAL<Process_DeChuJieLun>
    {
        int SelectMaxOid();

        void UpdateTrackDCJLOID(int oldDCJLOID, int newDCJLOID);
    }
}