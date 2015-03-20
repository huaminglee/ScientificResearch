using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IDAL 
{
    public interface IProjectGroupDAL : IBaseDAL<Process_ProjectGroup>
    {
        void UpdateProjectIdx(string oldXMZIdx, string newXMZIdx);
    }
}