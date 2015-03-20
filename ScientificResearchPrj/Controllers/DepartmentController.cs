using ScientificResearchPrj.BLL;
using ScientificResearchPrj.Controllers.Base;
using ScientificResearchPrj.IBLL;
using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScientificResearchPrj.Controllers
{
    public class DepartmentController : Controller
    {

        private IDepartmentService deptService = new DepartmentService();


        public ActionResult BuMenGuanLi()
        {
            return View();
        }
        
        public ActionResult GetDepartments()
        {
            List<MyPort_Dept> departments = deptService.GetDeptList();
            if (departments == null || departments.Count==0)
            {
                return Json(new
                {
                    state = "-1",
                    message = "当前部门数据为空"
                });
            }
            else
            {
                return Json(new
                {
                    state = "0",
                    message = "成功加载了" + departments.Count + "条数据",
                    _Json = EasyUIJson.GetEasyUIJson_Dept(departments),
                });
            }

        }
         
        [HttpPost]
        public ActionResult TianJiaBuMen(MyPort_Dept dept)
        {
            Dictionary<string, string> dictionary = deptService.AddDept(dept);
            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }
        
        [HttpPost]
        public ActionResult ShanChuBuMen(string deptNo)
        {
            Dictionary<string, string> dictionary = deptService.DeleteDept(deptNo);
            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        } 
        
        [HttpPost]
        public ActionResult XiuGaiBuMen(string oldNo, MyPort_Dept dept)
        {
            Dictionary<string, string> dictionary = deptService.ModifyDept(oldNo, dept);
            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }
    }
}
