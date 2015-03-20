using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IDAL 
{
    public interface ISheJiShiYanDAL : IBaseDAL<Process_ShiYan>
    {
        int SelectMaxOid();

        void UpdateTrackSJSYOID(int oldSYOID, int newSYOID);
        void UpdateDuiBiFenXiSYOID(int oldSYOID, int newSYOID);
    }
}