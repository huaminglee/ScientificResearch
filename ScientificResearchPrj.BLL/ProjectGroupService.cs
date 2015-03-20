using ScientificResearchPrj.DAL;
using ScientificResearchPrj.IBLL;
using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.BLL 
{
    public class ProjectGroupService : BaseService<Process_ProjectGroup>, IProjectGroupService
    {
        public override void SetCurrentDAL()
        {
            CurrentDAL = this.DbSession.ProjectGroupDAL;
        }

        public PageModel<Process_ProjectGroup> GetProjectGroups(int pageSize, int pageNow)
        {
            PageModel<Process_ProjectGroup> pageModel = new PageModel<Process_ProjectGroup>();
            int totalCount = 0;

            List<Process_ProjectGroup> prjGroups = this.LoadPageEntities(pageNow, pageSize, out totalCount, a => true, b => true, true).ToList();
            if (prjGroups != null)
            {
                foreach (Process_ProjectGroup prjGroup in prjGroups)
                {
                     SetGroupLeaderName(prjGroup);
                     SetMemberNoAndName(prjGroup);
                     SetProjectsName(prjGroup);
                }
            }

            if (prjGroups != null) pageModel.SetList(prjGroups);
            pageModel.SetPageNo(pageNow);
            pageModel.SetPageSize(pageSize);
            pageModel.SetTotalCount(totalCount);

            return pageModel;
        }

        private void SetGroupLeaderName(Process_ProjectGroup prjGroup) 
        {
            if (prjGroup.FK_GroupLeader == null) return;

            MyPort_Emp leader = this.DbSession.EmpDAL.LoadEntities(e => e.EmpNo.Equals(prjGroup.FK_GroupLeader)).FirstOrDefault();
            if (leader != null) {
                prjGroup.FK_GroupLeaderName = leader.Name; 
            }
        }

        private void SetMemberNoAndName(Process_ProjectGroup prjGroup)
        {
            string memberNo = "";
            string memberName = "";

            foreach (Process_GroupMember member in prjGroup.Process_GroupMember) {
                memberNo += member.MyPort_Emp.EmpNo + ",";
                memberName += member.MyPort_Emp.Name + ",";
            }

            ///消除最后的逗号
            if (memberNo != "")
                memberNo = memberNo.Substring(0, memberNo.Length - 1);
            if (memberName != "")
                memberName = memberName.Substring(0, memberName.Length - 1);

            prjGroup.FK_GroupMember = memberNo;
            prjGroup.FK_GroupMemberName = memberName;
        }

        private void SetProjectsName(Process_ProjectGroup prjGroup)
        {
            string projectsName = "";

            foreach (Process_Project prj in prjGroup.Process_Project)
            {
                projectsName += prj.Process_BasicData.Name + ",";
            }

            ///消除最后的逗号
            if (projectsName != "")
                projectsName = projectsName.Substring(0, projectsName.Length - 1);

            prjGroup.Projects = projectsName;
        }

        public List<Process_ProjectGroup> GetProjectGroups() {
            List<Process_ProjectGroup> prjGroups = this.LoadEntities(a => true).ToList();
            return prjGroups;
        }

        public Dictionary<string, string> AddProjectGroup(Process_ProjectGroup proGroup)
        {
            try
            {
                AddGroupMember(proGroup,proGroup.FK_GroupMember);
                this.AddEntity(proGroup);

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

        private void AddGroupMember(Process_ProjectGroup proGroup,string fk_member)
        {
            List<string> memberList = new List<string>();

            if (fk_member != null)
            {
                memberList = fk_member.Split(',').ToList();
            }

            if (memberList != null)
                foreach (string member in memberList)
                {
                    Process_GroupMember gm = new Process_GroupMember();
                    gm.FK_Xmz = proGroup.No;
                    gm.FK_Emp = member;
                    proGroup.Process_GroupMember.Add(gm);
                }
        }

        public Dictionary<string, string> DeleteProjectGroup(string proGroupNo)
        {
            try
            {
                DeleteProjectGroupWithoutUpdateColumn(proGroupNo);

                //更新项目表的外键关系
                //ToDo---------------------------------------------------------------------
               // (CurrentDAL as ProjectGroupDAL).UpdateProjectIdx(proGroupNo, "");

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

        private void DeleteProjectGroupWithoutUpdateColumn(string proGroupNo)
        {
            Process_ProjectGroup projectGroup = this.LoadEntities(a => a.No == proGroupNo).FirstOrDefault();
            if (projectGroup != null) {
                projectGroup.Process_GroupMember.Clear();
            }
            this.DeleteEntity(projectGroup);
        }

        public Dictionary<string, string> ModifyProjectGroup(string oldNo, Process_ProjectGroup proGroup)
        {
            try
            {
                if (!oldNo.Equals(proGroup.No))
                {
                    Process_ProjectGroup ifIdxHasExist = this.LoadEntities(a => a.No == proGroup.No).FirstOrDefault();
                    if (ifIdxHasExist != null) throw new Exception("编号已存在");

                    this.AddProjectGroup(proGroup);
                }
                else {
                    Process_ProjectGroup proGroupDesc = this.LoadEntities(a => a.No == oldNo).FirstOrDefault();
                    if (proGroupDesc != null)
                    {
                        proGroupDesc.Process_GroupMember.Clear();
                        //记得更新
                        this.UpdateEntity(proGroupDesc);

                        ModifyGroup(proGroupDesc, proGroup);
                    }
                    //对象存在，但没有被跟踪
                    if (proGroupDesc != null)
                        this.DetachEntity(proGroupDesc);

                    this.UpdateEntity(proGroupDesc);
                }
                
                //更新项目表的外键关系
                (CurrentDAL as ProjectGroupDAL).UpdateProjectIdx(oldNo, proGroup.No);

                if (!oldNo.Equals(proGroup.No))
                {
                    Process_ProjectGroup proGroupOld = this.LoadEntities(a => a.No == oldNo).FirstOrDefault();
                    this.DeleteProjectGroupWithoutUpdateColumn(proGroupOld.No);
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

        private void ModifyGroup(Process_ProjectGroup proGroupDesc, Process_ProjectGroup proGroupSource)
        {
            proGroupDesc.No = proGroupSource.No;
            proGroupDesc.Name = proGroupSource.Name;
            proGroupDesc.Description = proGroupSource.Description;
            proGroupDesc.FK_GroupLeader = proGroupSource.FK_GroupLeader;

            AddGroupMember(proGroupDesc,proGroupSource.FK_GroupMember);
        }
    }
}