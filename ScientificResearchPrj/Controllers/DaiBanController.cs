using ScientificResearchPrj.BLL;
using ScientificResearchPrj.Controllers.Base;
using ScientificResearchPrj.IBLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScientificResearchPrj.Controllers
{
    public class DaiBanController : ProcessBase<IDaiBanService>
    {
        public override void SetCurrentService()
        {
            this.CurrentService = SrService.DaiBanService;
        }
 
        public ActionResult DaiBan()
        {
            SetLoginUserData();

            DataTable todoFlows = CurrentService.GetTodoFlows();
            ViewData["_Json"] = EasyUIJson.GetEasyUIJsonFromDataTable(todoFlows);
            return View();
        }
        
    }
}