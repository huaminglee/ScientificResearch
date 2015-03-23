using ScientificResearchPrj.DAL;
using ScientificResearchPrj.IBLL;
using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.BLL 
{
    public class EmpService  :BaseService<MyPort_Emp>,IEmpService
    {
        public override void SetCurrentDAL()
        {
            CurrentDAL = this.DbSession.EmpDAL;
        }

        public PageModel<MyPort_Emp> GetEmps(int pageSize, int pageNow, string type)
        {
            PageModel<MyPort_Emp> pageModel = new PageModel<MyPort_Emp>();
            int totalCount = 0;

            List<MyPort_Emp> emps = GetEmpByType(pageSize, pageNow,out totalCount, type);

            if (emps != null && type.Equals(EmpType.DAOSHI))
            {
                foreach (MyPort_Emp emp in emps)
                {
                    SetDeptNoAndName(emp);
                    SetStationNoAndName(emp);
                }
            }
            else 
            {
                foreach (MyPort_Emp emp in emps)
                {
                    SetDeptNoAndName(emp);
                    SetTutorName(emp);
                }
            }

            if (emps != null) pageModel.SetList(emps);
            pageModel.SetPageNo(pageNow);
            pageModel.SetPageSize(pageSize);
            pageModel.SetTotalCount(totalCount);

            return pageModel;
        }

        private List<MyPort_Emp> GetEmpByType(int pageSize, int pageNow, out int totalCount, string type)
        {
            List<MyPort_Emp> emps = new List<MyPort_Emp>();
            if (type.Equals(EmpType.DAOSHI))
            {
                emps = this.LoadPageEntities(pageNow, pageSize, out totalCount, emp => emp.Type.Equals(EmpType.DAOSHI), b => true, true).ToList();
            }
            else if (type.Equals(EmpType.XUESHENG))
            {
                emps = this.LoadPageEntities(pageNow, pageSize, out totalCount, emp =>
                    emp.Type.Equals(EmpType.BENKESHENG) ||
                    emp.Type.Equals(EmpType.YANJIUSHENG) ||
                    emp.Type.Equals(EmpType.BOSHISHENG), b => true, true).ToList();
            }
            else
            {
                emps = this.LoadPageEntities(pageNow, pageSize, out totalCount, emp => emp.Type.Equals(type), b => true, true).ToList();
            }
            return emps;
        }

        /*--说明：部门本来设置为可选择多个部门，并将字段放在MyPort_Tutor类中，现在设置为单个部门，且字段放在emp基类中-*/
        private void SetDeptNoAndName(MyPort_Emp emp)
        {
            string deptNo = "";
            string deptName = "";
            foreach (MyPort_EmpDept d in emp.MyPort_EmpDept)
            {
                deptNo += d.MyPort_Dept.DeptNo + ",";
                deptName += d.MyPort_Dept.Name + ",";
            }
            ///消除最后的逗号
            if (deptNo != "")
                deptNo = deptNo.Substring(0, deptNo.Length - 1);
            if (deptName != "")
                deptName = deptName.Substring(0, deptName.Length - 1);
            /*
            if (emp.MyPort_Tutor != null)
            {
                emp.MyPort_Tutor.FK_Dept = deptNo;
                emp.MyPort_Tutor.FK_DeptName = deptName;
            }
             * */
            emp.FK_Dept = deptNo;
            emp.FK_DeptName = deptName;
        }

        private void SetStationNoAndName(MyPort_Emp emp)
        {
            ///添加岗位编号和岗位名称
            ///编号和名称都要额外操作
            string stationNo = "", stationName = "";
            foreach (MyPort_EmpStation sta in emp.MyPort_EmpStation)
            {
                stationNo += sta.MyPort_Station.StaNo + ",";
                stationName += sta.MyPort_Station.Name + ",";
            }
            ///消除最后的逗号
            if (stationNo != "")
                stationNo = stationNo.Substring(0, stationNo.Length - 1);
            if (stationName != "")
                stationName = stationName.Substring(0, stationName.Length - 1);
            if (emp.MyPort_Tutor != null)
            {
                emp.MyPort_Tutor.FK_Station = stationNo;
                emp.MyPort_Tutor.FK_StationName = stationName;
            }
        }

        private void SetTutorName(MyPort_Emp student)
        {
            if (student.MyPort_Student.FK_Tutor == null) return;

            MyPort_Emp tutor = this.LoadEntities(e => e.EmpNo.Equals(student.MyPort_Student.FK_Tutor)).FirstOrDefault();
            if (tutor != null && student.MyPort_Student!=null)
            {
                student.MyPort_Student.FK_TutorName = tutor.Name; 
            }
        }

        public Dictionary<string, string> AddEmp(EmpForJson emp)
        {
            try
            {
                if (emp.Type.Equals(EmpType.DAOSHI))
                {
                    AddTutor(emp);
                }
                else 
                {
                    AddStudent(emp);
                }
               
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add("state","0");
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

        private void AddTutor(EmpForJson empForJson) 
        {
            MyPort_Emp emp = new MyPort_Emp();
            
            emp.EmpNo = empForJson.EmpNo;
            emp.Name = empForJson.Name;
            emp.Tel = empForJson.Tel;
            emp.Email = empForJson.Email;
            emp.Type = empForJson.Type;

            //说明，改为单部门以后此处实际上只有一个部门
            string firstDept = AddMyPortEmpDept(emp, empForJson.FK_Dept);
            AddMyPortEmpStation(emp, empForJson.FK_Station);

            emp.MyPort_Tutor = new MyPort_Tutor();
            emp.MyPort_Tutor.FK_EmpNo = empForJson.EmpNo;
            emp.MyPort_Tutor.ChargeWork = empForJson.ChargeWork;
            emp.MyPort_Tutor.OfficeAddr = empForJson.OfficeAddr;
            emp.MyPort_Tutor.OfficeTel = empForJson.OfficeTel;

            emp.FK_Dept = firstDept;
            emp.SID = "";
            emp.Pass = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(emp.EmpNo, "MD5").ToUpper(); 

            this.AddEntity(emp);
        }

        private void AddStudent(EmpForJson empForJson)
        {
            MyPort_Emp emp = new MyPort_Emp();

            emp.EmpNo = empForJson.EmpNo;
            emp.Name = empForJson.Name;
            emp.Tel = empForJson.Tel;
            emp.Email = empForJson.Email;
            emp.Type = empForJson.Type;
            //说明，改为单部门以后此处实际上只有一个部门
            string firstDept = AddMyPortEmpDept(emp, empForJson.FK_Dept);

            emp.MyPort_Student = new MyPort_Student();
            emp.MyPort_Student.FK_EmpNo = empForJson.EmpNo;
            emp.MyPort_Student.AdmissionYear = empForJson.AdmissionYear;
            emp.MyPort_Student.SchoolingLength = empForJson.SchoolingLength;
            emp.MyPort_Student.FK_Tutor = empForJson.FK_Tutor;
            emp.MyPort_Student.LabAddr = empForJson.LabAddr;

            emp.FK_Dept = firstDept;
            emp.SID = "";
            emp.Pass = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(emp.EmpNo, "MD5").ToUpper(); 
            this.AddEntity(emp);
        }

        private string AddMyPortEmpDept( MyPort_Emp emp ,string fk_dept) 
        {
            List<string> deptList = new List<string>() ;
            string firstDept = "";

            if (fk_dept != null)
            {
                deptList = fk_dept.Split(',').ToList();
                firstDept = deptList[0];
            }

            if (deptList != null)
                foreach (string dept in deptList)
                {
                    MyPort_EmpDept ed = new MyPort_EmpDept();
                    ed.FK_Dept = dept;
                    ed.FK_Emp = emp.EmpNo;
                    emp.MyPort_EmpDept.Add(ed);
                }

            return firstDept;
        }

        private void AddMyPortEmpStation(MyPort_Emp emp, string fk_station)
        {
            List<string> stationList = new List<string>();
            if (fk_station != null)
                stationList = fk_station.Split(',').ToList();

            if (stationList != null)
                foreach (string sta in stationList)
                {
                    MyPort_EmpStation es = new MyPort_EmpStation();
                    es.FK_Emp = emp.EmpNo;
                    es.FK_Station = sta;
                    emp.MyPort_EmpStation.Add(es);
                }
        }

        public Dictionary<string, string> DeleteEmp(string empNo)
        {
            try
            {
                DeleteEmpWithoutUpdateColumn(empNo);

                //再更新外键关系或者依赖关系
                //学生表的导师字段
                (CurrentDAL as EmpDAL).UpdateStudentTutorIdx(empNo, "");
                //项目组组长字段
                (CurrentDAL as EmpDAL).UpdateProjectGroupLeaderIdx(empNo, "");
                //审核表审核人字段
                (CurrentDAL as EmpDAL).UpdateShenHeRenIdx(empNo, "");

                //Todo
                //基础信息表发起人外键关系  如何删除？
                //(CurrentDAL as EmpDAL).UpdateDataBasicProposerIdx(oldNo, empForJson.EmpNo);

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

        private void DeleteEmpWithoutUpdateColumn(string empNo)
        {
            //1.要删除的对象
            MyPort_Emp emp = this.LoadEntities(a => a.EmpNo == empNo).FirstOrDefault();

            if (emp != null)
            {
                emp.MyPort_EmpDept.Clear();
                emp.Process_GroupMember.Clear();

                if (emp.Type.Equals(EmpType.DAOSHI))
                {
                    emp.MyPort_EmpStation.Clear();

                    MyPort_Tutor tutor = this.DbSession.TutorDAL.LoadEntities(t => t.FK_EmpNo == emp.EmpNo).FirstOrDefault();
                    if (tutor != null)
                    {
                        this.DbSession.TutorDAL.DeleteEntity(tutor);
                        this.DbSession.SaveChanges();
                    }
                }
                else
                {
                    MyPort_Student student = this.DbSession.StudentDAL.LoadEntities(t => t.FK_EmpNo == emp.EmpNo).FirstOrDefault();
                    if (student != null)
                    {
                        this.DbSession.StudentDAL.DeleteEntity(student);
                        this.DbSession.SaveChanges();
                    }
                }

                this.DeleteEntity(emp);
            }
        }

        public Dictionary<string, string> ModifyEmp(string oldNo, EmpForJson empForJson)
        {
            try
            {
                ///只有从db中查询的数据才有关联到外键数据，直接使用emp的话MyPort_Dept与MyPort_Station数据是空的
                ///数据必须加到db中才能更新，使用db.MyPort_Emp.AsNoTracking()得到数据（使用AsNoTracking()数据不加载到db中，
                ///可避免与下面更改entry.State时db加载数据时产生键冲突），但是无法更新
                if (!oldNo.Equals(empForJson.EmpNo))
                {
                    MyPort_Emp ifIdxHasExist = this.LoadEntities(a => a.EmpNo == empForJson.EmpNo).FirstOrDefault();
                    if (ifIdxHasExist != null) throw new Exception("编号已存在");

                    this.AddEmp(empForJson);
                }

                else {
                    MyPort_Emp empDesc = this.LoadEntities(a => a.EmpNo == oldNo).FirstOrDefault();
                    if (empDesc != null)
                    {
                        empDesc.MyPort_EmpDept.Clear();
                        empDesc.Process_GroupMember.Clear();

                        if (empForJson.Type.Equals(EmpType.DAOSHI))
                        {
                           // empDesc.MyPort_EmpDept.Clear();
                            empDesc.MyPort_EmpStation.Clear();
                            //记得更新
                            this.UpdateEntity(empDesc);

                            ModifyTutor(empDesc, empForJson);
                        }
                        else
                        {
                            //记得更新
                            this.UpdateEntity(empDesc);
                            
                            ModifyStudent(empDesc, empForJson);
                        }
                    }
                    //对象存在，但没有被跟踪
                    if (empDesc != null)
                        this.DetachEntity(empDesc);

                    this.UpdateEntity(empDesc);
                }

                //再更新外键关系或者依赖关系
                //学生表的导师字段
                (CurrentDAL as EmpDAL).UpdateStudentTutorIdx(oldNo, empForJson.EmpNo);
                //项目组组长字段
                (CurrentDAL as EmpDAL).UpdateProjectGroupLeaderIdx(oldNo, empForJson.EmpNo);
                //项目组组员外键关系
                (CurrentDAL as EmpDAL).UpdateProjectGroupMemberIdx(oldNo, empForJson.EmpNo);
                //审核表审核人字段
                (CurrentDAL as EmpDAL).UpdateShenHeRenIdx(oldNo, empForJson.EmpNo);
                //基础信息表发起人外键关系
                (CurrentDAL as EmpDAL).UpdateDataBasicProposerIdx(oldNo, empForJson.EmpNo);

                //最后删除
                if (!oldNo.Equals(empForJson.EmpNo))
                {
                    this.DeleteEmpWithoutUpdateColumn(oldNo);
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

        private void ModifyTutor(MyPort_Emp emp,EmpForJson empForJson)
        {
            emp.EmpNo = empForJson.EmpNo;
            emp.Name = empForJson.Name;
            emp.Tel = empForJson.Tel;
            emp.Email = empForJson.Email;
            emp.Type = empForJson.Type;

            string firstDept = AddMyPortEmpDept(emp, empForJson.FK_Dept);
            AddMyPortEmpStation(emp, empForJson.FK_Station);

            emp.MyPort_Tutor.FK_EmpNo = empForJson.EmpNo;
            emp.MyPort_Tutor.ChargeWork = empForJson.ChargeWork;
            emp.MyPort_Tutor.OfficeAddr = empForJson.OfficeAddr;
            emp.MyPort_Tutor.OfficeTel = empForJson.OfficeTel;

            emp.FK_Dept = firstDept;
            emp.SID = emp.SID;
            emp.Pass = emp.Pass;
        }

        private void ModifyStudent(MyPort_Emp emp, EmpForJson empForJson)
        {
            emp.EmpNo = empForJson.EmpNo;
            emp.Name = empForJson.Name;
            emp.Tel = empForJson.Tel;
            emp.Email = empForJson.Email;
            emp.Type = empForJson.Type;
            string firstDept = AddMyPortEmpDept(emp, empForJson.FK_Dept);

            emp.MyPort_Student.FK_EmpNo = empForJson.EmpNo;
            emp.MyPort_Student.AdmissionYear = empForJson.AdmissionYear;
            emp.MyPort_Student.SchoolingLength = empForJson.SchoolingLength;
            emp.MyPort_Student.FK_Tutor = empForJson.FK_Tutor;
            emp.MyPort_Student.LabAddr = empForJson.LabAddr;

            emp.FK_Dept = firstDept;
            emp.SID = emp.SID;
            emp.Pass = emp.Pass;
        }

        public MyPort_Emp GetCurrentLoginUserInfo() {
            MyPort_Emp emp = CurrentDAL.LoadEntities(a => a.EmpNo == BP.Web.WebUser.No).FirstOrDefault();
            if (emp != null) return emp;
            else return new MyPort_Emp();
        }

        public Dictionary<string, string> ModifyEmpInfo(MyPort_Emp emp, string NewPass)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            try
            {
                MyPort_Emp empDesc = this.CurrentDAL.LoadEntities(a => a.EmpNo == emp.EmpNo).FirstOrDefault(); 
                if (empDesc != null)
                {
                    //需要更新密码
                    if (string.IsNullOrEmpty(NewPass) == false)
                    {
                        //作为密码方式加密   
                        string oldMd5 = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(emp.Pass, "MD5").ToUpper();
                        string newMd5 = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(NewPass, "MD5").ToUpper();


                        //与原来密码不一致
                        if ((string.IsNullOrEmpty(empDesc.Pass) == true && string.IsNullOrEmpty(emp.Pass) == false) ||
                            (string.IsNullOrEmpty(empDesc.Pass) == false && string.IsNullOrEmpty(emp.Pass) == true) ||
                            (string.IsNullOrEmpty(empDesc.Pass) == false && string.IsNullOrEmpty(emp.Pass) == false && empDesc.Pass.ToUpper().Equals(oldMd5) == false))
                        {
                            dictionary.Add("state", "-1");
                            dictionary.Add("message", "与原来密码不一致");
                            return dictionary;
                        }

                        //与原来密码一致,更新
                        empDesc.Pass = newMd5;
                    }

                    empDesc.Name = emp.Name;
                    empDesc.Email = emp.Email;
                    empDesc.Tel = emp.Tel;
                    this.UpdateEntity(empDesc);

                    dictionary.Add("state", "0");
                    dictionary.Add("message", "修改成功");
                    return dictionary;
                }
            }catch(Exception e){
                dictionary.Add("state", "-1");
                dictionary.Add("message", "修改失败~~" + e.Message);
                return dictionary;
            }

            dictionary.Add("state", "-1");
            dictionary.Add("message", "服务器未知错误");
            return dictionary;
        }
    }
}