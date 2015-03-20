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
    public class ZaiTuController : ProcessBase<IZaiTuService>
    {
        public override void SetCurrentService()
        {
            this.CurrentService = SrService.ZaiTuService;
        }
         
        public ActionResult ZaiTu()
        {
            DataTable runningFlows = CurrentService.GetRunningFlows();
            ViewData["_Json"] = EasyUIJson.GetEasyUIJsonFromDataTable(runningFlows);
            return View();
        }
        
    }
}