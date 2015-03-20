using ScientificResearchPrj.DAL;
using ScientificResearchPrj.IBLL;
using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.BLL 
{
    public class XuQiuFenXiService : BaseService<Process_Subject_XuQiuFenXi>, IXuQiuFenXiService
    {
        public override void SetCurrentDAL()
        {
            CurrentDAL = this.DbSession.XuQiuFenXiDAL;
        }

        public Process_Subject_XuQiuFenXi GetHistoryData(CCFlowArgs args)
        {
            string fk_node = args.FK_Node.ToString();
            string workid = args.WorkID.ToString();

            Process_Subject_XuQiuFenXi subject = this.LoadEntities(
                a => a.Process_BasicData.FK_Flow == args.FK_Flow &&
                a.Process_BasicData.FK_Node == fk_node &&
                a.Process_BasicData.WorkId == workid).FirstOrDefault();
            if (subject != null)
            {
                SetProposerName(subject);
                SetProjectName(subject);
                return subject;
            }
            return new Process_Subject_XuQiuFenXi();
        }

        private void SetProposerName(Process_Subject_XuQiuFenXi subject)
        {
            if (subject.Process_BasicData.FK_Proposer == null) return;

            MyPort_Emp proposer = this.DbSession.EmpDAL.LoadEntities(e => e.EmpNo.Equals(subject.Process_BasicData.FK_Proposer)).FirstOrDefault();
            if (proposer != null)
            {
                subject.Process_BasicData.ProposerName = proposer.Name;
            }
        }

        private void SetProjectName(Process_Subject_XuQiuFenXi subject)
        {
            if (subject.FK_XmOID == null) return;

            Process_Project prj = this.DbSession.ProjectDAL.LoadEntities(e => e.OID == subject.FK_XmOID).FirstOrDefault();
            if (prj != null)
            {
                subject.FK_XmName = prj.Process_BasicData.Name;
            }
        }

        public void InsertOrUpdateTrack(CCFlowArgs args) {
            string workid = args.WorkID.ToString();
            Process_Track ifExist = this.DbSession.TrackDAL.LoadEntities(a => a.FK_Flow == args.FK_Flow
                && a.WorkID == workid).FirstOrDefault();

            Process_Project project = this.DbSession.ProjectDAL.LoadEntities(a => a.Process_BasicData.FK_Flow == args.FK_Flow
                && a.Process_BasicData.WorkId == workid).FirstOrDefault();
            Process_Subject_XuQiuFenXi subject = this.DbSession.SubjectDAL.LoadEntities(a => a.Process_BasicData.FK_Flow == args.FK_Flow
                && a.Process_BasicData.WorkId == workid).FirstOrDefault();

            if (ifExist != null)
            {
                if(project!=null)
                    ifExist.XMOID = project.OID.ToString();
                if (subject != null)
                    ifExist.XQFXOID = subject.OID.ToString();

                this.DbSession.TrackDAL.UpdateEntity(ifExist);
                this.DbSession.SaveChanges();
            }
            else {
                ifExist = new Process_Track();
                if (project != null)
                    ifExist.XMOID = project.OID.ToString();
                if (subject != null)
                    ifExist.XQFXOID = subject.OID.ToString();
                ifExist.FK_Flow = args.FK_Flow;
                ifExist.WorkID = workid;

                this.DbSession.TrackDAL.AddEntity(ifExist);
                this.DbSession.SaveChanges();
            }
        }
    }
}