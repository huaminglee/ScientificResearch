using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IBLL 
{
    public interface IEmpService : IBaseService<MyPort_Emp>
    {
        PageModel<MyPort_Emp> GetEmps(int pageSize, int pageNow, string type);

        Dictionary<string, string> AddEmp(EmpForJson emp);

        Dictionary<string, string> DeleteEmp(string empNo);
  
        Dictionary<string, string> ModifyEmp(string oldNo, EmpForJson emp);

        MyPort_Emp GetCurrentLoginUserInfo();

        Dictionary<string, string> ModifyEmpInfo(MyPort_Emp emp, string NewPass);
    }
}