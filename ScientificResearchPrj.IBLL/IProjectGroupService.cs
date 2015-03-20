using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IBLL 
{
    public interface IProjectGroupService : IBaseService<Process_ProjectGroup>
    {

        PageModel<Process_ProjectGroup> GetProjectGroups(int pageSize, int pageNow);
        List<Process_ProjectGroup> GetProjectGroups();

        Dictionary<string, string> AddProjectGroup(Process_ProjectGroup proGroup);

        Dictionary<string, string> DeleteProjectGroup(string proGroupNo);

        Dictionary<string, string> ModifyProjectGroup(string oldNo, Process_ProjectGroup proGroup);
    }
}