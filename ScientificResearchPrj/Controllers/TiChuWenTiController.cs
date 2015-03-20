using ScientificResearchPrj.Controllers.Base;
using ScientificResearchPrj.IBLL;
using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScientificResearchPrj.Controllers
{
    public class TiChuWenTiController : ProcessBase<ITiChuWenTiService>
    {
        public override void SetCurrentService()
        {
            this.CurrentService = SrService.TiChuWenTiService;
        }
 
        public ActionResult TiChuWenTi()
        {
            SetViewData();
            return View();
        }
         
        public void TiChuWenTiFaQi()
        {
            CCFlowArgs args = new CCFlowArgs();
            args.FK_Flow = this.FK_Flow;
            args.WorkID = WorkID;

            CurrentService.InsertOrUpdateTrack(args); 

            this.FaSong(args);
        }
         
        public ActionResult TiChuWenTiShenHe()
        {
            SetViewData();
            return View();
        }
         
        public void TiChuWenTiShenHeFaQi()
        {
            CCFlowArgs args = new CCFlowArgs();
            args.FK_Flow = this.FK_Flow;
            args.WorkID = WorkID;
             
            this.FaSong(args);
        }

        public ActionResult GetHistoryData(CCFlowArgs args)
        {
            List<Process_TiChuWenTi> wtList = CurrentService.GetHistoryData(args);

            if (wtList == null || wtList.Count == 0)
            {
                return Json(new
                {
                    state = "-1",
                    message = "历史数据为空"
                });
            }
            else
            {
                return Json(new
                {
                    state = "0",
                    message = "成功加载提出问题数据",
                    _Json = EasyUIJson.GetEasyUIJson_TiChuWenTi(wtList)
                });
            }
        }

        public ActionResult TianJiaWenTi(Process_TiChuWenTi wenti, Process_BasicData basicData)
        {
            wenti.Process_BasicData = basicData;

            Dictionary<string, string> dictionary = CurrentService.AddWenTi(wenti);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }

        public ActionResult XiuGaiWenTi(string oldNo, Process_TiChuWenTi wenti, Process_BasicData basicData)
        {
            wenti.Process_BasicData = basicData;

            Dictionary<string, string> dictionary = CurrentService.ModifyWenTi(oldNo, wenti);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }

        public ActionResult ShanChuWenTi(string wtNo)
        {
            Dictionary<string, string> dictionary = CurrentService.DeleteWenTi(wtNo);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }
    }
}
