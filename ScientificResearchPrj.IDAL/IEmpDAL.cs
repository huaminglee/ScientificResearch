using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IDAL
{
    public interface IEmpDAL : IBaseDAL<MyPort_Emp>
    {
        
        void UpdateStudentTutorIdx(string oldEmpIdx, string newEmpIdx);

        void UpdateProjectGroupLeaderIdx(string oldEmpIdx, string newEmpIdx);

        void UpdateProjectGroupMemberIdx(string oldEmpIdx, string newEmpIdx);

        void UpdateShenHeRenIdx(string oldEmpIdx, string newEmpIdx);

        void UpdateDataBasicProposerIdx(string oldEmpIdx, string newEmpIdx);

    }
}