using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IBLL 
{
    public interface IProjectService : IBaseService<Process_Project>
    {

        PageModel<Process_Project> GetProjects(int pageSize, int pageNow);

        Process_Project GetProjectByNo(string priNo);

        Process_Project GetProjectByOID(int priOID);

        Dictionary<string, string> AddProject(Process_Project project);

        Dictionary<string, string> DeleteProject(string prjNo);

        Dictionary<string, string> ModifyProject(string oldNo, Process_Project project);

        Process_Project GetProjectHistoryDataFromTrack(CCFlowArgs args);
    }
}