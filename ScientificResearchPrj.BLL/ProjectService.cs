using ScientificResearchPrj.DAL;
using ScientificResearchPrj.IBLL;
using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.BLL 
{
    public class ProjectService : BaseService<Process_Project>, IProjectService
    {
        public override void SetCurrentDAL()
        {
            CurrentDAL = this.DbSession.ProjectDAL;
        }

        public PageModel<Process_Project> GetProjects(int pageSize, int pageNow)
        {
            PageModel<Process_Project> pageModel = new PageModel<Process_Project>();
            int totalCount = 0;

            List<Process_Project> projects = this.LoadPageEntities(pageNow, pageSize, out totalCount, a => true, b => true, true).ToList();
            if (projects != null)
            {
                foreach (Process_Project prj in projects)
                {
                    SetProposerName(prj);
                    SetProjectGroupName(prj);
                }
            }

            if (projects != null) pageModel.SetList(projects);
            pageModel.SetPageNo(pageNow);
            pageModel.SetPageSize(pageSize);
            pageModel.SetTotalCount(totalCount);

            return pageModel;
        }

        private void SetProposerName(Process_Project prj) 
        {
            if (prj.Process_BasicData.FK_Proposer == null) return;

            MyPort_Emp proposer = this.DbSession.EmpDAL.LoadEntities(e => e.EmpNo.Equals(prj.Process_BasicData.FK_Proposer)).FirstOrDefault();
            if (proposer != null)
            {
                prj.Process_BasicData.ProposerName = proposer.Name; 
            }
        }

        private void SetProjectGroupName(Process_Project prj)
        {
            if (prj.FK_Xmz == null) return;

            Process_ProjectGroup prjGroup = this.DbSession.ProjectGroupDAL.LoadEntities(e => e.No.Equals(prj.FK_Xmz)).FirstOrDefault();
            if (prjGroup != null)
            {
                prj.FK_XMZName = prjGroup.Name;
            }
        }

        public Process_Project GetProjectByNo(string priNo)
        {
            Process_Project project = this.LoadEntities(a => a.FK_BDNo == priNo).FirstOrDefault();
            if (project != null) {

                SetProposerName(project);
                SetProjectGroupName(project);
                
                return project;
            }
            return new Process_Project();
        }

        public Process_Project GetProjectByOID(int priOID)
        {
            Process_Project project = this.LoadEntities(a => a.OID == priOID).FirstOrDefault();
            if (project != null)
            {

                SetProposerName(project);
                SetProjectGroupName(project);

                return project;
            }
            return new Process_Project();
        }

        public Dictionary<string, string> AddProject(Process_Project project)
        { 
            try
            {
                Process_Project ifIdxHasExist = this.LoadEntities(a => a.FK_BDNo == project.Process_BasicData.No).FirstOrDefault();
                if (ifIdxHasExist != null) throw new Exception("项目【" + project.Process_BasicData.No + "】编号已存在");

                project.OID = (CurrentDAL as ProjectDAL).SelectMaxOid() + 1;
                project.FK_BDNo = project.Process_BasicData.No;

                this.AddEntity(project);

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
 
        public Dictionary<string, string> DeleteProject(string prjNo)
        {
            try
            {
                DeleteProjectWithoutUpdateColumn(prjNo);

                //更新课题表的外键关系
                //ToDo---------------------------------------------------------------------
                //(CurrentDAL as ProjectDAL).UpdateSubjectXmOID(prjNo, "");

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

        private void DeleteProjectWithoutUpdateColumn(string prjNo)
        {
            Process_Project project = this.LoadEntities(a => a.FK_BDNo == prjNo).FirstOrDefault();
            if (project != null)
            {
                this.DeleteEntity(project);
            } 

            Process_BasicData basicData = this.DbSession.BasicDataDAL.LoadEntities(a => a.No == prjNo).FirstOrDefault();
            if (basicData != null) {
                this.DbSession.BasicDataDAL.DeleteEntity(basicData);
                this.DbSession.SaveChanges();
            }
        }

        public Dictionary<string, string> ModifyProject(string oldNo, Process_Project project)
        { 
            try
            {
                if (!oldNo.Equals(project.Process_BasicData.No))
                {
                    Process_Project ifIdxHasExist = this.LoadEntities(a => a.FK_BDNo == project.Process_BasicData.No).FirstOrDefault();
                    if (ifIdxHasExist != null) throw new Exception("项目【" + project.Process_BasicData.No + "】编号已存在");

                    this.AddProject(project);
                }
                else {
                    Process_Project prjDesc = this.LoadEntities(a => a.FK_BDNo == oldNo).FirstOrDefault();
                    if (prjDesc != null)
                    {
                        ModifyProject(prjDesc, project);
                        this.UpdateEntity(prjDesc);
                    }
                }

                //使用OID作为外键，没有修改编号的话，不改变OID，无需修改外键关系
                if (!oldNo.Equals(project.Process_BasicData.No))
                {
                    Process_Project oldProject = this.LoadEntities(a => a.FK_BDNo == oldNo).FirstOrDefault();
                    Process_Project newProject = this.LoadEntities(a => a.FK_BDNo == project.Process_BasicData.No).FirstOrDefault();

                    //更新课题表的外键关系
                    (CurrentDAL as ProjectDAL).UpdateSubjectXmOID(oldProject.OID, newProject.OID);
                    //更新轨迹表的关联关系
                    (CurrentDAL as ProjectDAL).UpdateTrackXmOID(oldProject.OID, newProject.OID);

                    this.DeleteProjectWithoutUpdateColumn(oldNo);
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

        private void ModifyProject(Process_Project projectDesc, Process_Project projectSource)
        {
            projectDesc.OID = projectDesc.OID;

            projectDesc.FK_BDNo = projectSource.Process_BasicData.No;
            projectDesc.FK_Xmz = projectSource.FK_Xmz;
            projectDesc.Columns = projectSource.Columns;
            projectDesc.Tasks = projectSource.Tasks;
            projectDesc.Questions = projectSource.Questions;

            projectDesc.Process_BasicData.No = projectSource.Process_BasicData.No;
            projectDesc.Process_BasicData.Name = projectSource.Process_BasicData.Name;
            projectDesc.Process_BasicData.FK_Proposer = projectSource.Process_BasicData.FK_Proposer;
            projectDesc.Process_BasicData.ProposeTime = projectSource.Process_BasicData.ProposeTime;
            projectDesc.Process_BasicData.Description = projectSource.Process_BasicData.Description;
            projectDesc.Process_BasicData.Keys = projectSource.Process_BasicData.Keys;
            projectDesc.Process_BasicData.Remarks = projectSource.Process_BasicData.Remarks;
            projectDesc.Process_BasicData.ModifyTime = projectSource.Process_BasicData.ModifyTime;
            projectDesc.Process_BasicData.LastSendTime = projectSource.Process_BasicData.LastSendTime;
            projectDesc.Process_BasicData.FK_Flow = projectSource.Process_BasicData.FK_Flow;
            projectDesc.Process_BasicData.WorkId = projectSource.Process_BasicData.WorkId;

            if (projectSource.Process_BasicData.FK_Flow != null)
            {
                projectDesc.Process_BasicData.FK_Flow = projectSource.Process_BasicData.FK_Flow;
            }
            else
            {
                projectDesc.Process_BasicData.FK_Flow = projectDesc.Process_BasicData.FK_Flow;
            }

            if (projectSource.Process_BasicData.FK_Node != null)
            {
                projectDesc.Process_BasicData.FK_Node = projectSource.Process_BasicData.FK_Node;
            }
            else
            {
                projectDesc.Process_BasicData.FK_Node = projectDesc.Process_BasicData.FK_Node;
            }

            if (projectSource.Process_BasicData.WorkId != null)
            {
                projectDesc.Process_BasicData.WorkId = projectSource.Process_BasicData.WorkId;
            }
            else
            {
                projectDesc.Process_BasicData.WorkId = projectDesc.Process_BasicData.WorkId;
            }
        }

        public Process_Project GetProjectHistoryDataFromTrack(CCFlowArgs args){
            try
            {
                string workid = args.WorkID.ToString();
                Process_Track track = this.DbSession.TrackDAL.LoadEntities(a => a.FK_Flow == args.FK_Flow
                    && a.WorkID == workid).FirstOrDefault();

                if (track != null)
                {
                    int xmOID = Convert.ToInt32(track.XMOID);
                    Process_Project project = this.LoadEntities(a => a.OID == xmOID).FirstOrDefault();

                    if (project != null)
                    {

                        SetProposerName(project);
                        SetProjectGroupName(project);

                        return project;
                    }
                }
            }catch(Exception e){
                return new Process_Project();
            }
            return new Process_Project();
        }
    }
}