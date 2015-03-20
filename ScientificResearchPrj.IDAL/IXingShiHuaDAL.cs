using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IDAL 
{
    public interface IXingShiHuaDAL : IBaseDAL<Process_XingShiHua>
    {
        int SelectMaxOid();

        void UpdateTrackXSHOID(int oldXSHOID, int newXSHOID);
        void UpdateSuanFaXSHOID(int oldXSHOID, int newXSHOID);
    }
}