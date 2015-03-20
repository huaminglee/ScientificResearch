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
    public class SheJiSuanFaDAL : BaseDAL<Process_SuanFa>, ISheJiSuanFaDAL
    {
        public int SelectMaxOid()
        {
            try
            {
                string sql = "Select Max(OID) From Process_SuanFa";
                return BP.DA.DBAccess.RunSQLReturnValInt(sql);
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public void UpdateTrackSJSFOID(int oldSFOID, int newSFOID)
        {
            string oldSFStr = GetAllTrackSFOIDByOneOID(oldSFOID);
            string[] OIDArray = oldSFStr.Split(',');

            for (int i = 0; i < OIDArray.Length; i++)
            {
                if (OIDArray[i] == oldSFOID.ToString())
                {
                    OIDArray[i] = newSFOID.ToString();
                }
            }
            string newSFStr = String.Join(",", Array.ConvertAll(OIDArray, (Converter<string, string>)Convert.ToString));

            string sql = "Update Process_Track Set SJSFOID='" + newSFStr + "' Where SJSFOID='" + oldSFStr + "'";
            BP.DA.DBAccess.RunSQL(sql);
        }

        private string GetAllTrackSFOIDByOneOID(int oldSFOID)
        {
            string sql = "Select SJSFOID From Process_Track  Where  SJSFOID Like '" + oldSFOID + ",%' or SJSFOID Like '%," + oldSFOID + "' or SJSFOID Like '%," + oldSFOID + ",%'";
            string sfOIDStr = BP.DA.DBAccess.RunSQLReturnStringIsNull(sql, null);

            if (sfOIDStr == null) return "";
            return sfOIDStr;
        }

        public void UpdateShiYanSFOID(int oldSFOID, int newSFOID)
        {
            string sql = "Update Process_ShiYan Set FK_SFOID='" + newSFOID + "' Where FK_SFOID='" + oldSFOID + "'";
            BP.DA.DBAccess.RunSQL(sql);
        }
    }
}