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
    public class JieJueSiLuDAL : BaseDAL<Process_SiLu>, IJieJueSiLuDAL
    {
        public int SelectMaxOid() {
            try
            {
                string sql = "Select Max(OID) From Process_SiLu";
                return BP.DA.DBAccess.RunSQLReturnValInt(sql);
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public void UpdateTrackSLOID(int oldSLOID, int newSLOID) {
            string oldSLStr = GetAllTrackSLOIDByOneOID(oldSLOID);
            string[] OIDArray = oldSLStr.Split(',');

            for (int i = 0; i < OIDArray.Length; i++)
            {
                if (OIDArray[i] == oldSLOID.ToString())
                {
                    OIDArray[i] = newSLOID.ToString();
                }
            }
            string newSLStr = String.Join(",", Array.ConvertAll(OIDArray, (Converter<string, string>)Convert.ToString));

            string sql = "Update Process_Track Set JJSLOID='" + newSLStr + "' Where JJSLOID='" + oldSLStr + "'";
            BP.DA.DBAccess.RunSQL(sql);
        }

        private string GetAllTrackSLOIDByOneOID(int oldSLOID)
        {
            string sql = "Select JJSLOID From Process_Track  Where  JJSLOID Like '" + oldSLOID + ",%' or JJSLOID Like '%," + oldSLOID + "' or JJSLOID Like '%," + oldSLOID + ",%'";
            string slOIDStr = BP.DA.DBAccess.RunSQLReturnStringIsNull(sql, null);

            if (slOIDStr == null) return "";
            return slOIDStr;
        }

        public void UpdateXingShiHuaSLOID(int oldSLOID, int newSLOID) {
            string sql = "Update Process_XingShiHua Set FK_SLOID='" + newSLOID + "' Where FK_SLOID='" + oldSLOID + "'";
            BP.DA.DBAccess.RunSQL(sql);
        }
    }
}