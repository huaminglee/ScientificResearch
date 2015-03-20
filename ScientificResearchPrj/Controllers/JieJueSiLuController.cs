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
    public class JieJueSiLuController : ProcessBase<IJieJueSiLuService>
    {
        public override void SetCurrentService()
        {
            this.CurrentService = SrService.JieJueSiLuService;
        }

        public ActionResult JieJueSiLu()
        {
            SetViewData();
            return View();
        }

        public void JieJueSiLuFaQi()
        {
            CCFlowArgs args = new CCFlowArgs();
            args.FK_Flow = this.FK_Flow;
            args.WorkID = WorkID;

            CurrentService.InsertOrUpdateTrack(args); 

            this.FaSong(args);
        }

        public ActionResult JieJueSiLuShenHe()
        {
            SetViewData();
            return View();
        }

        public void JieJueSiLuShenHeFaQi()
        {
            CCFlowArgs args = new CCFlowArgs();
            args.FK_Flow = this.FK_Flow;
            args.WorkID = WorkID;
             
            this.FaSong(args);
        }

        public ActionResult GetHistoryData(CCFlowArgs args)
        {
            List<Process_SiLu> slList = CurrentService.GetHistoryData(args);

            if (slList == null || slList.Count == 0)
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
                    message = "成功加载解决思路数据",
                    _Json = EasyUIJson.GetEasyUIJson_JieJueSiLu(slList)
                });
            }
        }

        public ActionResult TianJiaSiLu(Process_SiLu silu, Process_BasicData basicData)
        {
            silu.Process_BasicData = basicData;

            Dictionary<string, string> dictionary = CurrentService.AddSiLu(silu);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }

        public ActionResult XiuGaiSiLu(string oldNo, Process_SiLu silu, Process_BasicData basicData)
        {
            silu.Process_BasicData = basicData;

            Dictionary<string, string> dictionary = CurrentService.ModifySiLu(oldNo, silu);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }

        public ActionResult ShanChuSiLu(string slNo)
        {
            Dictionary<string, string> dictionary = CurrentService.DeleteSiLu(slNo);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }
    }
}
