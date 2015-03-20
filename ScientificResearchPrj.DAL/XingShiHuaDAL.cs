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
    public class XingShiHuaDAL : BaseDAL<Process_XingShiHua>, IXingShiHuaDAL
    {
        public int SelectMaxOid()
        {
            try
            {
                string sql = "Select Max(OID) From Process_XingShiHua";
                return BP.DA.DBAccess.RunSQLReturnValInt(sql);
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public void UpdateTrackXSHOID(int oldXSHOID, int newXSHOID)
        {
            string oldXSHStr = GetAllTrackXSHOIDByOneOID(oldXSHOID);
            string[] OIDArray = oldXSHStr.Split(',');

            for (int i = 0; i < OIDArray.Length; i++)
            {
                if (OIDArray[i] == oldXSHOID.ToString())
                {
                    OIDArray[i] = newXSHOID.ToString();
                }
            }
            string newXSHStr = String.Join(",", Array.ConvertAll(OIDArray, (Converter<string, string>)Convert.ToString));

            string sql = "Update Process_Track Set XSHOID='" + newXSHStr + "' Where XSHOID='" + oldXSHStr + "'";
            BP.DA.DBAccess.RunSQL(sql);
        }

        private string GetAllTrackXSHOIDByOneOID(int oldXSHOID)
        {
            string sql = "Select XSHOID From Process_Track  Where  XSHOID Like '" + oldXSHOID + ",%' or XSHOID Like '%," + oldXSHOID + "' or XSHOID Like '%," + oldXSHOID + ",%'";
            string xshOIDStr = BP.DA.DBAccess.RunSQLReturnStringIsNull(sql, null);

            if (xshOIDStr == null) return "";
            return xshOIDStr;
        }

        public void UpdateSuanFaXSHOID(int oldXSHOID, int newXSHOID)
        {
            string sql = "Update Process_SuanFa Set FK_XSHOID='" + newXSHOID + "' Where FK_XSHOID='" + oldXSHOID + "'";
            BP.DA.DBAccess.RunSQL(sql);
        }
    }
}