using ScientificResearchPrj.DAL;
using ScientificResearchPrj.IBLL;
using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.BLL 
{
    public class SubjectService : BaseService<Process_Subject_XuQiuFenXi>, ISubjectService
    {
        public override void SetCurrentDAL()
        {
            CurrentDAL = this.DbSession.SubjectDAL;
        }

        public PageModel<Process_Subject_XuQiuFenXi> GetSubjects(int pageSize, int pageNow)
        {
            PageModel<Process_Subject_XuQiuFenXi> pageModel = new PageModel<Process_Subject_XuQiuFenXi>();
            int totalCount = 0;

            List<Process_Subject_XuQiuFenXi> subjects = this.LoadPageEntities(pageNow, pageSize, out totalCount, a => true, b => true, true).ToList();
            if (subjects != null)
            {
                foreach (Process_Subject_XuQiuFenXi sub in subjects)
                {
                    SetProposerName(sub);
                    SetProjectName(sub);
                }
            }

            if (subjects != null) pageModel.SetList(subjects);
            pageModel.SetPageNo(pageNow);
            pageModel.SetPageSize(pageSize);
            pageModel.SetTotalCount(totalCount);

            return pageModel;
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
       
        public Process_Subject_XuQiuFenXi GetSubjectByNo(string subNo)
        {
            Process_Subject_XuQiuFenXi subject = this.LoadEntities(a => a.FK_BDNo == subNo).FirstOrDefault();
            if (subject != null)
            {
                SetProposerName(subject);
                SetProjectName(subject);
                return subject;
            }
            return new Process_Subject_XuQiuFenXi();
        }

 
        public  Dictionary<string, string> AddSubject(Process_Subject_XuQiuFenXi subject)
        { 
            try
            {
                Process_Subject_XuQiuFenXi ifIdxHasExist = this.LoadEntities(a => a.FK_BDNo == subject.Process_BasicData.No).FirstOrDefault();
                if (ifIdxHasExist != null) throw new Exception("课题【" + subject.Process_BasicData.No + "】编号已存在");

                subject.OID = (CurrentDAL as SubjectDAL).SelectMaxOid() + 1;
                subject.FK_BDNo = subject.Process_BasicData.No;

                this.AddEntity(subject);

                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add("state", "0");
                dictionary.Add("message", "添加成功");
                return dictionary;
            }
            catch (Exception ex)
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add("state", "-1");
                dictionary.Add("message", "添加失败~~~" + ex.Message);
                return dictionary;
            } 
        }
 
        public Dictionary<string, string> DeleteSubject(string subNo)
        { 
            try
            {
                DeleteSubjectWithoutUpdateColumn(subNo);

                ///更新调研表的外键关系
                //ToDo---------------------------------------------------------------------
                ///更新提出问题表的外键关系
                ///更新轨迹表的关联关系
                
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add("state", "0");
                dictionary.Add("message", "删除成功");
                return dictionary;
            }
            catch (Exception ex)
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add("state", "-1");
                dictionary.Add("message", "删除失败~~~" + ex.Message);
                return dictionary;
            }  
        }

        private void DeleteSubjectWithoutUpdateColumn(string subNo)
        {
            Process_Subject_XuQiuFenXi subject = this.LoadEntities(a => a.FK_BDNo == subNo).FirstOrDefault();
            if (subject != null)
            {
                this.DeleteEntity(subject);
            }

            Process_BasicData basicData = this.DbSession.BasicDataDAL.LoadEntities(a => a.No == subNo).FirstOrDefault();
            if (basicData != null) {
                this.DbSession.BasicDataDAL.DeleteEntity(basicData);
                this.DbSession.SaveChanges();
            } 
        }

        public Dictionary<string, string> ModifySubject(string oldNo, Process_Subject_XuQiuFenXi subject)
        {  
            try
            {
                if (!oldNo.Equals(subject.Process_BasicData.No))
                {
                    Process_Subject_XuQiuFenXi ifIdxHasExist = this.LoadEntities(a => a.FK_BDNo == subject.Process_BasicData.No).FirstOrDefault();
                    if (ifIdxHasExist != null) throw new Exception("课题【" + subject.Process_BasicData.No + "】编号已存在");

                    this.AddSubject(subject);
                }
                else {
                    Process_Subject_XuQiuFenXi subDesc = this.LoadEntities(a => a.FK_BDNo == oldNo).FirstOrDefault();
                    if (subDesc != null)
                    {
                        ModifySubject(subDesc, subject);
                        this.UpdateEntity(subDesc);
                    }
                }

               //使用OID作为外键，没有修改编号的话，不改变OID，无需修改外键关系
                if (!oldNo.Equals(subject.Process_BasicData.No))
                {
                    Process_Subject_XuQiuFenXi oldSubject = this.LoadEntities(a => a.FK_BDNo == oldNo).FirstOrDefault();
                    Process_Subject_XuQiuFenXi newSubject = this.LoadEntities(a => a.FK_BDNo == subject.Process_BasicData.No).FirstOrDefault();
                
                    //更新调研表的外键关系
                    (CurrentDAL as SubjectDAL).UpdateDiaoYanKTOID(oldSubject.OID, newSubject.OID);
                    //更新提出问题表的外键关系
                    (CurrentDAL as SubjectDAL).UpdateTiChuWenTiKTOID(oldSubject.OID, newSubject.OID);
                    //更新轨迹表的关联关系
                    (CurrentDAL as SubjectDAL).UpdateTrackKTOID(oldSubject.OID, newSubject.OID);


                    this.DeleteSubjectWithoutUpdateColumn(oldNo);
                }
                
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add("state", "0");
                dictionary.Add("message", "修改成功");
                return dictionary;
            }
            catch (Exception ex)
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add("state", "-1");
                dictionary.Add("message", "修改失败~~~" + ex.Message);
                return dictionary;
            } 
        }

        private void ModifySubject(Process_Subject_XuQiuFenXi subjectDesc, Process_Subject_XuQiuFenXi subjectSource)
        {
            subjectDesc.OID = subjectDesc.OID;

            subjectDesc.FK_BDNo = subjectSource.Process_BasicData.No;
            subjectDesc.FK_XmOID = subjectSource.FK_XmOID;
            subjectDesc.SourceDesc = subjectSource.SourceDesc;
            subjectDesc.AnalysisResult = subjectSource.AnalysisResult;
            subjectDesc.TargetTask = subjectSource.TargetTask;
            subjectDesc.Innovation = subjectSource.Innovation;
              
            subjectDesc.Process_BasicData.No = subjectSource.Process_BasicData.No;
            subjectDesc.Process_BasicData.Name = subjectSource.Process_BasicData.Name;
            subjectDesc.Process_BasicData.FK_Proposer = subjectSource.Process_BasicData.FK_Proposer;
            subjectDesc.Process_BasicData.ProposeTime = subjectSource.Process_BasicData.ProposeTime;
            subjectDesc.Process_BasicData.Description = subjectSource.Process_BasicData.Description;
            subjectDesc.Process_BasicData.Keys = subjectSource.Process_BasicData.Keys;
            subjectDesc.Process_BasicData.Remarks = subjectSource.Process_BasicData.Remarks;
            subjectDesc.Process_BasicData.ModifyTime = subjectSource.Process_BasicData.ModifyTime;

            if (subjectSource.Process_BasicData.FK_Flow != null)
            {
                subjectDesc.Process_BasicData.FK_Flow = subjectSource.Process_BasicData.FK_Flow;
            }
            else {
                subjectDesc.Process_BasicData.FK_Flow = subjectDesc.Process_BasicData.FK_Flow;
            }

            if (subjectSource.Process_BasicData.FK_Node != null)
            {
                subjectDesc.Process_BasicData.FK_Node = subjectSource.Process_BasicData.FK_Node;
            }
            else
            {
                subjectDesc.Process_BasicData.FK_Node = subjectDesc.Process_BasicData.FK_Node;
            }

            if (subjectSource.Process_BasicData.WorkId != null)
            {
                subjectDesc.Process_BasicData.WorkId = subjectSource.Process_BasicData.WorkId;
            }
            else
            {
                subjectDesc.Process_BasicData.WorkId = subjectDesc.Process_BasicData.WorkId;
            }
        }

        public Process_Subject_XuQiuFenXi GetSubjectHistoryDataFromTrack(CCFlowArgs args)
        {
            try
            {
                string workid = args.WorkID.ToString();
                Process_Track track = this.DbSession.TrackDAL.LoadEntities(a => a.FK_Flow == args.FK_Flow
                    && a.WorkID == workid).FirstOrDefault();

                if (track != null)
                {
                    int xqfxOID = Convert.ToInt32(track.XQFXOID);
                    Process_Subject_XuQiuFenXi subject = this.LoadEntities(a => a.OID == xqfxOID).FirstOrDefault();

                    if (subject != null)
                    {

                        SetProposerName(subject);
                        SetProjectName(subject);

                        return subject;
                    }
                }
            }
            catch (Exception e)
            {
                return new Process_Subject_XuQiuFenXi();
            }
            return new Process_Subject_XuQiuFenXi();
        }
    }
}