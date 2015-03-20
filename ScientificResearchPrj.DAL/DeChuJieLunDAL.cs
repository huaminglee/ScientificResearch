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
    public class DeChuJieLunDAL : BaseDAL<Process_DeChuJieLun>, IDeChuJieLunDAL
    {
        public int SelectMaxOid()
        {
            try
            {
                string sql = "Select Max(OID) From Process_DeChuJieLun";
                return BP.DA.DBAccess.RunSQLReturnValInt(sql);
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public void UpdateTrackDCJLOID(int oldDCJLOID, int newDCJLOID)
        {
            string oldDCJLStr = GetAllTrackDCJLOIDByOneOID(oldDCJLOID);
            string[] OIDArray = oldDCJLStr.Split(',');

            for (int i = 0; i < OIDArray.Length; i++)
            {
                if (OIDArray[i] == oldDCJLOID.ToString())
                {
                    OIDArray[i] = newDCJLOID.ToString();
                }
            }
            string newDCJLStr = String.Join(",", Array.ConvertAll(OIDArray, (Converter<string, string>)Convert.ToString));

            string sql = "Update Process_Track Set DCJLOID='" + newDCJLStr + "' Where DCJLOID='" + oldDCJLStr + "'";
            BP.DA.DBAccess.RunSQL(sql);
        }

        private string GetAllTrackDCJLOIDByOneOID(int oldDCJLOID)
        {
            string sql = "Select DCJLOID From Process_Track  Where  DCJLOID Like '" + oldDCJLOID + ",%' or DCJLOID Like '%," + oldDCJLOID + "' or DCJLOID Like '%," + oldDCJLOID + ",%'";
            string dcjlOIDStr = BP.DA.DBAccess.RunSQLReturnStringIsNull(sql, null);

            if (dcjlOIDStr == null) return "";
            return dcjlOIDStr;
        }

    }
}