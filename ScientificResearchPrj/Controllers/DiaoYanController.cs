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
    public class DiaoYanController : ProcessBase<IDiaoYanService>
    {
        public override void SetCurrentService()
        {
            this.CurrentService = SrService.DiaoYanService;
        }

        public ActionResult DiaoYan()
        {
            SetViewData();
            return View();
        }

        public void DiaoYanFaQi()
        {
            CCFlowArgs args = new CCFlowArgs();
            args.FK_Flow = this.FK_Flow;
            args.WorkID = WorkID;

            CurrentService.InsertOrUpdateTrack(args); 

            this.FaSong(args);
        }

        public ActionResult DiaoYanShenHe()
        {
            SetViewData();
            return View();
        }

        public void DiaoYanShenHeFaQi()
        {
            CCFlowArgs args = new CCFlowArgs();
            args.FK_Flow = this.FK_Flow;
            args.WorkID = WorkID;
             
            this.FaSong(args);
        }
         
        public ActionResult GetHistoryData(CCFlowArgs args)
        {
            List<Process_DiaoYan> dyList = CurrentService.GetHistoryData(args);

            if (dyList==null || dyList.Count == 0)
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
                    message = "成功加载调研数据",
                    _Json = EasyUIJson.GetEasyUIJson_DiaoYan(dyList)
                });
            }
        }

        public ActionResult TianJiaDiaoYan(Process_DiaoYan diaoyan, Process_BasicData basicData)
        {
            diaoyan.Process_BasicData = basicData;

            Dictionary<string, string> dictionary = CurrentService.AddDiaoYan(diaoyan);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }

        public ActionResult XiuGaiDiaoYan(string oldNo, Process_DiaoYan diaoyan, Process_BasicData basicData)
        {
            diaoyan.Process_BasicData = basicData;

            Dictionary<string, string> dictionary = CurrentService.ModifyDiaoYan(oldNo, diaoyan);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }

        public ActionResult ShanChuDiaoYan(string dyNo)
        {
            Dictionary<string, string> dictionary = CurrentService.DeleteDiaoYan(dyNo);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }
    }
}
