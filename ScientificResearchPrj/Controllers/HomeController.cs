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
        #region 系统首页 Index()
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        } 
        #endregion

       
       
        #region 找回密码 FindPassword()
        public ActionResult FindPassword()
        {
            ViewBag.Message = "Your FindPassword page.";

            return View();
        } 
        #endregion

        #region 登录 SignIn()
        [HttpGet]
        public ActionResult SignIn()
        {
            return View();
        } 
        #endregion

        #region 登录 SignIn()
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
        #endregion

        #region 注册 SignUp()
        public ActionResult SignUp()
        {
            ViewBag.Message = "Your SignUp page.";

            return View();
        } 
        #endregion
    }
}
