using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IDAL 
{
    public interface IProjectDAL : IBaseDAL<Process_Project>
    {
        int SelectMaxOid();

        void UpdateSubjectXmOID(int oldPrjOID, int newPrjOID);

        void UpdateTrackXmOID(int oldPrjOID, int newPrjOID);
    }
}