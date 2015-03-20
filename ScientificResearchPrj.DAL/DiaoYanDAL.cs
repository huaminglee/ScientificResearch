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
    public class DiaoYanDAL : BaseDAL<Process_DiaoYan>, IDiaoYanDAL
    {
        public int SelectMaxOid()
        {
            try
            {
                string sql = "Select Max(OID) From Process_DiaoYan";
                return BP.DA.DBAccess.RunSQLReturnValInt(sql);
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public void UpdateTrackDYOID(int oldDYOID, int newDYOID)
        {
            string oldDyStr = GetAllTrackDYOIDByOneOID(oldDYOID);
            string[] OIDArray = oldDyStr.Split(',');

            for (int i = 0; i < OIDArray.Length; i++) {
                if (OIDArray[i] == oldDYOID.ToString()) {
                    OIDArray[i] = newDYOID.ToString();
                }
            }
            string newDYStr = String.Join(",", Array.ConvertAll(OIDArray, (Converter<string, string>)Convert.ToString));

            string sql = "Update Process_Track Set DYOID='" + newDYStr + "' Where DYOID='" + oldDyStr + "'";
            BP.DA.DBAccess.RunSQL(sql);
        }

        private string GetAllTrackDYOIDByOneOID(int oldDYOID) {
            string sql = "Select DYOID From Process_Track  Where  DYOID Like '" + oldDYOID + ",%' or DYOID Like '%," + oldDYOID + "' or DYOID Like '%," + oldDYOID + ",%'";
            string dyOIDStr = BP.DA.DBAccess.RunSQLReturnStringIsNull(sql, null);

            if (dyOIDStr == null) return "";
            return dyOIDStr;
        }
    }
}