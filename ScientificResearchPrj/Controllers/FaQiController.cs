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
    public class FaQiController : ProcessBase<IFaQiService>
    {
        public override void SetCurrentService()
        {
            this.CurrentService = SrService.FaQiService;
        }
         
        public ActionResult FaQi()
        {
            DataTable canStartFlows = CurrentService.GetCanStartFlows();
            ViewData["_Json"] = EasyUIJson.GetEasyUIJsonFromDataTable(canStartFlows);
            return View();
        }
        
        // id为FK_Flow
        public ActionResult LiShiFaQi(string id)
        {
            DataTable historyStartFlows = CurrentService.GetHisToryStartFlows(id);
            ViewData["_Json"] = EasyUIJson.GetEasyUIJsonFromDataTable(historyStartFlows);
            return View();
        }
       
    }
}