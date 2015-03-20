using ScientificResearchPrj.Controllers.Base;
using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScientificResearchPrj.Controllers
{
    public class ShouYueController : ProcessBase<Object>
    {
        public override void SetCurrentService()
        {
            this.CurrentService = new Object();
        }

        public ActionResult FaQiRenShouYue()
        {
            SetViewData();
            return View();
        }
        
        public void FaQiRenShouYueFaQi()
        {
            CCFlowArgs args = new CCFlowArgs();
            args.FK_Flow = this.FK_Flow;
            args.WorkID = WorkID;

            this.FaSong(args);
        }

    }
}
