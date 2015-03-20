using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IDAL 
{
    public interface ILunWenZhuanXieDAL : IBaseDAL<Process_LunWen>
    {
        int SelectMaxOid();

        void UpdateTrackLWZXOID(int oldLWZXOID, int newLWZXOID);
    }
}