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
    public class SheJiSuanFaController : ProcessBase<ISheJiSuanFaService>
    {
        public override void SetCurrentService()
        {
            this.CurrentService = SrService.SheJiSuanFaService;
        }

        public ActionResult SheJiSuanFa()
        {
            SetViewData();
            return View();
        }

        public void SheJiSuanFaFaQi()
        {
            CCFlowArgs args = new CCFlowArgs();
            args.FK_Flow = this.FK_Flow;
            args.WorkID = WorkID;

            CurrentService.InsertOrUpdateTrack(args); 

            this.FaSong(args);
        }

        public ActionResult SheJiSuanFaShenHe()
        {
            SetViewData();
            return View();
        }

        public void SheJiSuanFaShenHeFaQi()
        {
            CCFlowArgs args = new CCFlowArgs();
            args.FK_Flow = this.FK_Flow;
            args.WorkID = WorkID;
             
            this.FaSong(args);
        }
        
        public ActionResult GetHistoryData(CCFlowArgs args)
        {
            List<Process_SuanFa> sfList = CurrentService.GetHistoryData(args);

            if (sfList == null || sfList.Count == 0)
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
                    message = "成功加载算法数据",
                    _Json = EasyUIJson.GetEasyUIJson_SuanFa(sfList)
                });
            }
        }

        public ActionResult TianJiaSuanFa(Process_SuanFa suanfa, Process_BasicData basicData)
        {
            suanfa.Process_BasicData = basicData;

            Dictionary<string, string> dictionary = CurrentService.AddSuanFa(suanfa);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }

        public ActionResult XiuGaiSuanFa(string oldNo, Process_SuanFa suanfa, Process_BasicData basicData)
        {
            suanfa.Process_BasicData = basicData;

            Dictionary<string, string> dictionary = CurrentService.ModifySuanFa(oldNo, suanfa);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }

        public ActionResult ShanChuSuanFa(string sfNo)
        {
            Dictionary<string, string> dictionary = CurrentService.DeleteSuanFa(sfNo);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }
    }
}
