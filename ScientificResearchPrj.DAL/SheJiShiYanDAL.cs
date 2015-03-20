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
    public class SheJiShiYanDAL : BaseDAL<Process_ShiYan>, ISheJiShiYanDAL
    {
        public int SelectMaxOid()
        {
            try
            {
                string sql = "Select Max(OID) From Process_ShiYan";
                return BP.DA.DBAccess.RunSQLReturnValInt(sql);
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public void UpdateTrackSJSYOID(int oldSYOID, int newSYOID)
        {
            string oldSYStr = GetAllTrackSYOIDByOneOID(oldSYOID);
            string[] OIDArray = oldSYStr.Split(',');

            for (int i = 0; i < OIDArray.Length; i++)
            {
                if (OIDArray[i] == oldSYOID.ToString())
                {
                    OIDArray[i] = newSYOID.ToString();
                }
            }
            string newSYStr = String.Join(",", Array.ConvertAll(OIDArray, (Converter<string, string>)Convert.ToString));

            string sql = "Update Process_Track Set SJSYOID='" + newSYStr + "' Where SJSYOID='" + oldSYStr + "'";
            BP.DA.DBAccess.RunSQL(sql);
        }

        private string GetAllTrackSYOIDByOneOID(int oldSYOID)
        {
            string sql = "Select SJSYOID From Process_Track  Where  SJSYOID Like '" + oldSYOID + ",%' or SJSYOID Like '%," + oldSYOID + "' or SJSYOID Like '%," + oldSYOID + ",%'";
            string syOIDStr = BP.DA.DBAccess.RunSQLReturnStringIsNull(sql, null);

            if (syOIDStr == null) return "";
            return syOIDStr;
        }

        public void UpdateDuiBiFenXiSYOID(int oldSYOID, int newSYOID)
        {
            string sql = "Update Process_DuiBiFenXi Set FK_SYOID='" + newSYOID + "' Where FK_SYOID='" + oldSYOID + "'";
            BP.DA.DBAccess.RunSQL(sql);
        }
    }
}