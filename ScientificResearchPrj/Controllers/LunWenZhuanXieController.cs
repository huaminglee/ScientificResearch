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
    public class LunWenZhuanXieController : ProcessBase<ILunWenZhuanXieService>
    {
        public override void SetCurrentService()
        {
            this.CurrentService = SrService.LunWenZhuanXieService;
        }

        public ActionResult LunWenZhuanXie()
        {
            SetViewData();
            return View();
        }

        public void LunWenZhuanXieFaQi()
        {
            CCFlowArgs args = new CCFlowArgs();
            args.FK_Flow = this.FK_Flow;
            args.WorkID = WorkID;

            CurrentService.InsertOrUpdateTrack(args); 

            this.FaSong(args);
        }

        public ActionResult LunWenZhuanXieShenHe()
        {
            SetViewData();
            return View();
        }

        public void LunWenZhuanXieShenHeFaQi()
        {
            CCFlowArgs args = new CCFlowArgs();
            args.FK_Flow = this.FK_Flow;
            args.WorkID = WorkID;
             
            this.FaSong(args);
        }
        
        public ActionResult GetHistoryData(CCFlowArgs args)
        {
            List<Process_LunWen> lwList = CurrentService.GetHistoryData(args);

            if (lwList == null || lwList.Count == 0)
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
                    message = "成功加载论文数据",
                    _Json = EasyUIJson.GetEasyUIJson_LunWen(lwList)
                });
            }
        }

        public ActionResult TianJiaLunWen(Process_LunWen lunwen, Process_BasicData basicData)
        {
            lunwen.Process_BasicData = basicData;

            Dictionary<string, string> dictionary = CurrentService.AddLunWen(lunwen);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }

        public ActionResult XiuGaiLunWen(string oldNo, Process_LunWen lunwen, Process_BasicData basicData)
        {
            lunwen.Process_BasicData = basicData;

            Dictionary<string, string> dictionary = CurrentService.ModifyLunWen(oldNo, lunwen);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }

        public ActionResult ShanChuLunWen(string lwNo)
        {
            Dictionary<string, string> dictionary = CurrentService.DeleteLunWen(lwNo);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }
    }
}
