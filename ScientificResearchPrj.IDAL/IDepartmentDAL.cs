using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IDAL
{
    public interface IDepartmentDAL : IBaseDAL<MyPort_Dept>
    {
        void UpdateEmpDeptIdx(string oldDeptIdx, string newDeptIdx);

        void UpdateParentIdx(string oldDeptIdx, string newDeptIdx);

        void UpdateFK_DeptInEmp(string oldDeptIdx, string newDeptIdx);
        
    }
}