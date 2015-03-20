using ScientificResearchPrj.DAL;
using ScientificResearchPrj.IBLL;
using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScientificResearchPrj.BLL 
{
    public class DepartmentService :BaseService<MyPort_Dept>,IDepartmentService
    {

        public override void SetCurrentDAL()
        {
            CurrentDAL = this.DbSession.DepartmentDAL;
        }

        public List<MyPort_Dept> GetDeptList()
        {
            List<MyPort_Dept> list = this.LoadEntities(a => true).ToList();
            //添加父部门名称
            if(list!=null)
                foreach (MyPort_Dept d in list)
                {
                    if (d.ParentNo != null && !d.ParentNo.Equals("0"))
                        d.ParentDept = this.LoadEntities(a=>a.DeptNo==d.ParentNo).FirstOrDefault().Name;
                }
            return list;
        }

        public Dictionary<string, string> AddDept(MyPort_Dept dept)
        {
            try
            {
                this.AddEntity(dept);
                
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

        public Dictionary<string, string> DeleteDept(string deptNo)
        {
            try
            {
                MyPort_Dept dept = this.LoadEntities(a => a.DeptNo == deptNo).FirstOrDefault();
                if(dept!=null)
                {
                    foreach (MyPort_EmpDept empDept in dept.MyPort_EmpDept)
                    {
                        empDept.MyPort_Emp.FK_Dept = null;
                    }
                    //清空多对多关系
                    dept.MyPort_EmpDept.Clear();
                    this.DeleteEntity(dept);
                    
                }
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
         
        public Dictionary<string, string> ModifyDept(string oldNo, MyPort_Dept dept)
        {
            try
            {
                if (!oldNo.Equals(dept.DeptNo))
                {
                    MyPort_Dept ifIdxHasExist = this.LoadEntities(a => a.DeptNo == dept.DeptNo).FirstOrDefault();
                    if (ifIdxHasExist != null) throw new Exception("编号已存在");

                    //先添加
                    this.AddEntity(dept);
                }
                else {
                    MyPort_Dept deptDesc = this.LoadEntities(a => a.DeptNo == oldNo).FirstOrDefault();
                    ModifyDept(deptDesc, dept);

                    //先修改
                    this.UpdateEntity(deptDesc);
                }
                

                //再修复人员、部门表的外键关系
                (CurrentDAL as DepartmentDAL).UpdateEmpDeptIdx(oldNo, dept.DeptNo);

                //修复子部门依附的父部门编号
                (CurrentDAL as DepartmentDAL).UpdateParentIdx(oldNo, dept.DeptNo);
                //修复人员表的fk_dept字段
                (CurrentDAL as DepartmentDAL).UpdateFK_DeptInEmp(oldNo, dept.DeptNo);

                //最后删除
                if (!oldNo.Equals(dept.DeptNo))
                {
                    MyPort_Dept deptOld = this.LoadEntities(a => a.DeptNo == oldNo).FirstOrDefault();
                    this.DeleteEntity(deptOld);
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

        private void ModifyDept(MyPort_Dept deptDesc, MyPort_Dept deptSource) {
            deptDesc.DeptNo = deptSource.DeptNo;
            deptDesc.Name = deptSource.Name;
            deptDesc.ParentNo = deptSource.ParentNo;
            deptDesc.Description = deptSource.Description;
        }
    }
}