using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IDAL 
{
    public interface ISubjectDAL : IBaseDAL<Process_Subject_XuQiuFenXi>
    {
        int SelectMaxOid();

        void UpdateDiaoYanKTOID(int oldKTOID, int newKTOID);
        void UpdateTiChuWenTiKTOID(int oldKTOID, int newKTOID);
        void UpdateTrackKTOID(int oldKTOID, int newKTOID);
    }
}