using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IBLL 
{
    public interface IDepartmentService : IBaseService<MyPort_Dept>
    {
        
        List<MyPort_Dept> GetDeptList();
         
        Dictionary<string, string> AddDept(MyPort_Dept dept);

        Dictionary<string, string> DeleteDept(string deptNo);
 
        Dictionary<string, string> ModifyDept(string oldNo, MyPort_Dept dept);
    }
}