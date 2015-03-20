using ScientificResearchPrj.IDAL;
using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.DAL
{
    public class EmpDAL:BaseDAL<MyPort_Emp>,IEmpDAL
    {
         
        public void UpdateStudentTutorIdx(string oldEmpIdx, string newEmpIdx) {
            string sql = "Update MyPort_Student Set FK_Tutor='" + newEmpIdx + "' Where FK_Tutor='" + oldEmpIdx + "'";
            BP.DA.DBAccess.RunSQL(sql);
        }

        public void UpdateProjectGroupLeaderIdx(string oldEmpIdx, string newEmpIdx) {
            string sql = "Update Process_ProjectGroup Set FK_GroupLeader='" + newEmpIdx + "' Where FK_GroupLeader='" + oldEmpIdx + "'";
            BP.DA.DBAccess.RunSQL(sql);
        }

        public void UpdateProjectGroupMemberIdx(string oldEmpIdx, string newEmpIdx) {
            string sql = "Update Process_GroupMember Set FK_Emp='" + newEmpIdx + "' Where FK_Emp='" + oldEmpIdx + "'";
            BP.DA.DBAccess.RunSQL(sql);
        }

        public void UpdateShenHeRenIdx(string oldEmpIdx, string newEmpIdx) {
            string sql = "Update Process_ShenHe Set ShenHeRen='" + newEmpIdx + "' Where ShenHeRen='" + oldEmpIdx + "'";
            BP.DA.DBAccess.RunSQL(sql);
        }

        public void UpdateDataBasicProposerIdx(string oldEmpIdx, string newEmpIdx) {
            string sql = "Update Process_BasicData Set FK_Proposer='" + newEmpIdx + "' Where FK_Proposer='" + oldEmpIdx + "'";
            BP.DA.DBAccess.RunSQL(sql);
        }
    }
}