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
    public class ProjectDAL : BaseDAL<Process_Project>, IProjectDAL
    {
        public int SelectMaxOid()
        {
            try
            {
                string sql = "Select Max(OID) From Process_Project";
                return BP.DA.DBAccess.RunSQLReturnValInt(sql);
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public void UpdateSubjectXmOID(int oldPrjOID, int newPrjOID) {
            string sql = "Update Process_Subject_XuQiuFenXi Set FK_XmOID='" + newPrjOID + "' Where FK_XmOID='" + oldPrjOID + "'";
            BP.DA.DBAccess.RunSQL(sql);
        }

        public void UpdateTrackXmOID(int oldPrjOID, int newPrjOID)
        {
            string sql = "Update Process_Track Set XMOID='" + newPrjOID + "' Where XMOID='" + oldPrjOID + "'";
            BP.DA.DBAccess.RunSQL(sql);
        }
    }
}