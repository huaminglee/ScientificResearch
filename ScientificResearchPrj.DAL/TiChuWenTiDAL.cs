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
    public class TiChuWenTiDAL : BaseDAL<Process_TiChuWenTi>, ITiChuWenTiDAL
    {
        public int SelectMaxOid()
        {
            try
            {
                string sql = "Select Max(OID) From Process_TiChuWenTi";
                return BP.DA.DBAccess.RunSQLReturnValInt(sql);
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public void UpdateTrackWTOID(int oldWTOID, int newWTOID)
        {
            string oldWTStr = GetAllTrackWTOIDByOneOID(oldWTOID);
            string[] OIDArray = oldWTStr.Split(',');

            for (int i = 0; i < OIDArray.Length; i++)
            {
                if (OIDArray[i] == oldWTOID.ToString())
                {
                    OIDArray[i] = newWTOID.ToString();
                }
            }
            string newWTStr = String.Join(",", Array.ConvertAll(OIDArray, (Converter<string, string>)Convert.ToString));

            string sql = "Update Process_Track Set TCWTOID='" + newWTStr + "' Where TCWTOID='" + oldWTStr + "'";
            BP.DA.DBAccess.RunSQL(sql);
        }

        private string GetAllTrackWTOIDByOneOID(int oldWTOID)
        {
            string sql = "Select TCWTOID From Process_Track  Where  TCWTOID Like '" + oldWTOID + ",%' or TCWTOID Like '%," + oldWTOID + "' or TCWTOID Like '%," + oldWTOID + ",%'";
            string wtOIDStr = BP.DA.DBAccess.RunSQLReturnStringIsNull(sql, null);

            if (wtOIDStr == null) return "";
            return wtOIDStr;
        }

        public void UpdateSiLuWTOID(int oldWTOID, int newWTOID)
        {
            string sql = "Update Process_SiLu Set FK_WTOID='" + newWTOID + "' Where FK_WTOID='" + oldWTOID + "'";
            BP.DA.DBAccess.RunSQL(sql);
        }
    }
}