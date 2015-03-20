using ScientificResearchPrj.BLL;
using ScientificResearchPrj.Controllers.Base;
using ScientificResearchPrj.IBLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScientificResearchPrj.Controllers
{
    public class LiuChengSheJiController : ProcessBase<ILiuChengSheJiService>
    {
        public override void SetCurrentService()
        {
            this.CurrentService = SrService.LiuChengSheJiService;
        }

        public ActionResult LiuChengSheJi()
        {
            ViewData["url"] = CurrentService.GetLiuChengSheJiURL();
            return View();
        }
         
    }
}