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
    public class DuiBiFenXiDAL : BaseDAL<Process_DuiBiFenXi>, IDuiBiFenXiDAL
    {
        public int SelectMaxOid()
        {
            try
            {
                string sql = "Select Max(OID) From Process_DuiBiFenXi";
                return BP.DA.DBAccess.RunSQLReturnValInt(sql);
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public void UpdateTrackDBFXOID(int oldDBFXOID, int newDBFXOID)
        {
            string oldDBFXStr = GetAllTrackDBFXOIDByOneOID(oldDBFXOID);
            string[] OIDArray = oldDBFXStr.Split(',');

            for (int i = 0; i < OIDArray.Length; i++)
            {
                if (OIDArray[i] == oldDBFXOID.ToString())
                {
                    OIDArray[i] = newDBFXOID.ToString();
                }
            }
            string newDBFXStr = String.Join(",", Array.ConvertAll(OIDArray, (Converter<string, string>)Convert.ToString));

            string sql = "Update Process_Track Set DBFXOID='" + newDBFXStr + "' Where DBFXOID='" + oldDBFXStr + "'";
            BP.DA.DBAccess.RunSQL(sql);
        }

        private string GetAllTrackDBFXOIDByOneOID(int oldDBFXOID)
        {
            string sql = "Select DBFXOID From Process_Track  Where  DBFXOID Like '" + oldDBFXOID + ",%' or DBFXOID Like '%," + oldDBFXOID + "' or DBFXOID Like '%," + oldDBFXOID + ",%'";
            string dbfxOIDStr = BP.DA.DBAccess.RunSQLReturnStringIsNull(sql, null);

            if (dbfxOIDStr == null) return "";
            return dbfxOIDStr;
        }

    }
}