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
    public class ProjectGroupDAL : BaseDAL<Process_ProjectGroup>, IProjectGroupDAL
    {
        public void UpdateProjectIdx(string oldXMZIdx, string newXMZIdx) {
            string sql = "Update Process_Project Set FK_Xmz='" + newXMZIdx + "' Where FK_Xmz='" + oldXMZIdx + "'";
            BP.DA.DBAccess.RunSQL(sql);
        }
    }
}