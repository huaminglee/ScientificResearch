using ScientificResearchPrj.Model;
using ScientificResearchPrj.IDAL;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.DAL
{
    public class DepartmentDAL : BaseDAL<MyPort_Dept>, IDepartmentDAL
    {
        public void UpdateEmpDeptIdx(string oldDeptIdx, string newDeptIdx)
        {
            string sql = "Update MyPort_EmpDept Set FK_Dept='" + newDeptIdx + "' Where FK_Dept='" + oldDeptIdx + "'";
            BP.DA.DBAccess.RunSQL(sql);
        }

        public void UpdateParentIdx(string oldDeptIdx, string newDeptIdx)
        {
            string sql = "Update MyPort_Dept Set ParentNo='" + newDeptIdx + "' Where ParentNo='" + oldDeptIdx + "'";
            BP.DA.DBAccess.RunSQL(sql);
        }

        public void UpdateFK_DeptInEmp(string oldDeptIdx, string newDeptIdx) {
            string sql = "Update MyPort_Emp Set FK_Dept='" + newDeptIdx + "' Where FK_Dept='" + oldDeptIdx + "'";
            BP.DA.DBAccess.RunSQL(sql);
        }
    }
}