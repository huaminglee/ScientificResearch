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
    public class EmpController : Controller
    {
        private IEmpService empService = new EmpService();
        
        public ActionResult RenYuanGuanLi()
        {
            return View();
        }
         
        public ActionResult GetEmps(int pageSize, int pageNow, string type)
        {
            PageModel<MyPort_Emp> pageModel = new PageModel<MyPort_Emp>();

            pageModel = empService.GetEmps(pageSize, pageNow,type);

            List<MyPort_Emp> emps = pageModel.GetList();
            if (emps == null || emps.Count==0)
            {
                return Json(new
                {
                    state = "-1",
                    message = "当前页数据为空",
                    totalCount = pageModel.GetTotalCount(),
                    pageSize = pageModel.GetPageSize(),
                    pageNumber = pageModel.GetPageNo()
                });
            }
            else
            {
                return Json(new
                {
                    state = "0",
                    message = "成功加载了" + emps.Count + "条数据",
                    _Json = EasyUIJson.GetEasyUIJson_Emp(emps,type),
                    totalCount = pageModel.GetTotalCount(),
                    pageSize = pageModel.GetPageSize(),
                    pageNumber = pageModel.GetPageNo()
                });
            }
        }
         
        [HttpPost]
        public ActionResult TianJiaRenYuan(EmpForJson emp)
        {
            Dictionary<string, string> dictionary = empService.AddEmp(emp);
            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            }); 
        }  
        
        [HttpPost]
        public ActionResult ShanChuRenYuan(string empNo)
        {
            Dictionary<string, string> dictionary = empService.DeleteEmp(empNo);
            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        } 
         
        [HttpPost]
        public ActionResult XiuGaiRenYuan(string oldNo,EmpForJson emp)
        {
            Dictionary<string, string> dictionary = empService.ModifyEmp(oldNo, emp);
            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
         }

        public ActionResult HuoQuGeRenXinXi() {
            List<MyPort_Emp> emps = new List<MyPort_Emp>();
            MyPort_Emp emp = empService.GetCurrentLoginUserInfo();
            if (emp.EmpNo != null)
            {
                emps.Add(emp);
                ViewData["Json_UserInfo"] = EasyUIJson.GetEasyUIJson_Emp(emps,emp.Type);
            }

            return View();
        }

        public ActionResult XiuGaiGeRenXinXi(MyPort_Emp emp, string NewPass) {
            Dictionary<string, string> dictionary = empService.ModifyEmpInfo(emp, NewPass);
            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }
    }
}
