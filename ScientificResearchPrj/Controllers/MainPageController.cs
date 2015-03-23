using BP.En;
using ScientificResearchPrj.BLL;
using ScientificResearchPrj.Controllers.Base;
using ScientificResearchPrj.IBLL;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScientificResearchPrj.Controllers
{
    public class MainPageController : Controller
    {
        private IAccountService AccountService = new AccountService();
        
        public ActionResult Index()
        { 
            //当前登陆者
            ViewData["LoginUser"] = BP.Web.WebUser.No;
            ViewData["LoginUserName"] = BP.Web.WebUser.Name;
            ViewData["FK_DeptName"] = BP.Web.WebUser.FK_DeptName;
            return View();
        }

        public ActionResult SignOut()
        {
            AccountService.SignOut();
            return  RedirectToAction("SignIn","Home");
        }

      



      
    
    
    
    
    
    
    
    
    }
}
