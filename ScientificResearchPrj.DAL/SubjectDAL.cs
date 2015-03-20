using BP.DA;
using ScientificResearchPrj.IDAL;
using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.DAL 
{
    public class SubjectDAL : BaseDAL<Process_Subject_XuQiuFenXi>, ISubjectDAL
    {
        public int SelectMaxOid()
        {
            try
            {
                string sql = "Select Max(OID) From Process_Subject_XuQiuFenXi";
                return BP.DA.DBAccess.RunSQLReturnValInt(sql);
            }
            catch (Exception e) {
                return 0;
            }
        }

        public void UpdateDiaoYanKTOID(int oldKTOID, int newKTOID) 
        {
            string sql = "Update Process_DiaoYan Set FK_KTOID='" + newKTOID + "' Where FK_KTOID='" + oldKTOID + "'";
            BP.DA.DBAccess.RunSQL(sql);
        }
        
        public void UpdateTiChuWenTiKTOID(int oldKTOID, int newKTOID)
        {
            string sql = "Update Process_TiChuWenTi Set FK_KTOID='" + newKTOID + "' Where FK_KTOID='" + oldKTOID + "'";
            BP.DA.DBAccess.RunSQL(sql);
        }

        public void UpdateTrackKTOID(int oldKTOID, int newKTOID)
        {
            string sql = "Update Process_Track Set XQFXOID='" + newKTOID + "' Where XQFXOID='" + oldKTOID + "'";
            BP.DA.DBAccess.RunSQL(sql);
        }
    }
}