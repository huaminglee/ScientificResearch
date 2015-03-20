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
    public class DeChuJieLunController : ProcessBase<IDeChuJieLunService>
    {
        public override void SetCurrentService()
        {
            this.CurrentService = SrService.DeChuJieLunService;
        }

        public ActionResult DeChuJieLun()
        {
            SetViewData();
            return View();
        }

        public void DeChuJieLunFaQi()
        {
            CCFlowArgs args = new CCFlowArgs();
            args.FK_Flow = this.FK_Flow;
            args.WorkID = WorkID;

            CurrentService.InsertOrUpdateTrack(args); 

            this.FaSong(args);
        }

        public ActionResult DeChuJieLunShenHe()
        {
            SetViewData();
            return View();
        }

        public void DeChuJieLunShenHeFaQi()
        {
            CCFlowArgs args = new CCFlowArgs();
            args.FK_Flow = this.FK_Flow;
            args.WorkID = WorkID;
             
            this.FaSong(args);
        }
        
        public ActionResult GetHistoryData(CCFlowArgs args)
        {
            List<Process_DeChuJieLun> jlList = CurrentService.GetHistoryData(args);

            if (jlList == null || jlList.Count == 0)
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
                    message = "成功加载结论数据",
                    _Json = EasyUIJson.GetEasyUIJson_DeChuJieLun(jlList)
                });
            }
        }

        public ActionResult TianJiaJieLun(Process_DeChuJieLun jielun, Process_BasicData basicData)
        {
            jielun.Process_BasicData = basicData;

            Dictionary<string, string> dictionary = CurrentService.AddJieLun(jielun);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }

        public ActionResult XiuGaiJieLun(string oldNo, Process_DeChuJieLun jielun, Process_BasicData basicData)
        {
            jielun.Process_BasicData = basicData;

            Dictionary<string, string> dictionary = CurrentService.ModifyJieLun(oldNo, jielun);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }

        public ActionResult ShanChuJieLun(string jlNo)
        {
            Dictionary<string, string> dictionary = CurrentService.DeleteJieLun(jlNo);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }
    }
}
