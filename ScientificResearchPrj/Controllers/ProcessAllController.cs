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
    public class ProcessAllController : ProcessBase<IProcessAllService>
    {
        public override void SetCurrentService()
        {
            this.CurrentService = SrService.ProcessAllService;
        }

        public ActionResult AllProcess()
        {
            SetLoginUserData();

            DataTable allFlows = CurrentService.GetAllProcess();
            ViewData["_Json"] = EasyUIJson.GetEasyUIJsonFromDataTable(allFlows);
            return View();
        }
    }
}