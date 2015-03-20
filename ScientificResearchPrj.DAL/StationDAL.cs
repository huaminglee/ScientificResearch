using BP.DA;
using ScientificResearchPrj.IDAL;
using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.DAL 
{
    public class StationDAL:BaseDAL<MyPort_Station>,IStationDAL
    {
        public void UpdateEmpStationIdx(string oldStaIdx, string newStaIdx)
        {
            string sql = "Update MyPort_EmpStation Set FK_Station='" + newStaIdx + "' Where FK_Station='" + oldStaIdx + "'";
            BP.DA.DBAccess.RunSQL(sql);
        }
    }
}