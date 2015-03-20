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
    public class LunWenZhuanXieDAL : BaseDAL<Process_LunWen>, ILunWenZhuanXieDAL
    {
        public int SelectMaxOid()
        {
            try
            {
                string sql = "Select Max(OID) From Process_LunWen";
                return BP.DA.DBAccess.RunSQLReturnValInt(sql);
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public void UpdateTrackLWZXOID(int oldLWZXOID, int newLWZXOID)
        {
            string oldLWZXStr = GetAllTrackLWZXOIDByOneOID(oldLWZXOID);
            string[] OIDArray = oldLWZXStr.Split(',');

            for (int i = 0; i < OIDArray.Length; i++)
            {
                if (OIDArray[i] == oldLWZXOID.ToString())
                {
                    OIDArray[i] = newLWZXOID.ToString();
                }
            }
            string newLWZXStr = String.Join(",", Array.ConvertAll(OIDArray, (Converter<string, string>)Convert.ToString));

            string sql = "Update Process_Track Set LWZXOID='" + newLWZXStr + "' Where LWZXOID='" + oldLWZXStr + "'";
            BP.DA.DBAccess.RunSQL(sql);
        }

        private string GetAllTrackLWZXOIDByOneOID(int oldLWZXOID)
        {
            string sql = "Select LWZXOID From Process_Track  Where  LWZXOID Like '" + oldLWZXOID + ",%' or LWZXOID Like '%," + oldLWZXOID + "' or LWZXOID Like '%," + oldLWZXOID + ",%'";
            string lwOIDStr = BP.DA.DBAccess.RunSQLReturnStringIsNull(sql, null);

            if (lwOIDStr == null) return "";
            return lwOIDStr;
        }
    }
}