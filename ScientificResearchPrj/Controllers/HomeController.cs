using ScientificResearchPrj.BLL;
using ScientificResearchPrj.IBLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScientificResearchPrj.Controllers
{
    public class HomeController : Controller
    {

        private IAccountService AccountService = new AccountService();
        
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
           
            return View();
        }

        [HttpGet]
        public ActionResult ResetPassword()
        {
            return View();
        } 

        [HttpPost]
        public ActionResult ResetPassword(Model.ResetPasswordModels model)
        {
            if (ModelState.IsValid)
            {
                Dictionary<string, string> dictionary = AccountService.ResetPassword(model);
                return Json(new
                {
                    state = dictionary["state"],
                    message = dictionary["message"]
                });
            }
            return Json(new
            {
                state = "-1",
                message = "信息验证失败"
            });
        } 

         
        [HttpGet]
        public ActionResult SignIn()
        {
            return View();
        } 
 
         
        [HttpPost]
        public ActionResult SignIn(Model.AccountModel model)
        {
            if (ModelState.IsValid)
            {
                Dictionary<string, string> dictionary = AccountService.SignIn(model);
                return Json(new
                {
                    state = dictionary["state"],
                    message = dictionary["message"]
                });
            }
            return Json(new 
            {
                state="-1",
                message="信息验证失败"
            });
        }
         
          
    }
}
