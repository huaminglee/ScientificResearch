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
    public class SheJiShiYanController : ProcessBase<ISheJiShiYanService>
    {
        public override void SetCurrentService()
        {
            this.CurrentService = SrService.SheJiShiYanService;
        }

        public ActionResult SheJiShiYan()
        {
            SetViewData();
            return View();
        }

        public void SheJiShiYanFaQi()
        {
            CCFlowArgs args = new CCFlowArgs();
            args.FK_Flow = this.FK_Flow;
            args.WorkID = WorkID;

            CurrentService.InsertOrUpdateTrack(args); 

            this.FaSong(args);
        }

        public ActionResult SheJiShiYanShenHe()
        {
            SetViewData();
            return View();
        }

        public void SheJiShiYanShenHeFaQi()
        {
            CCFlowArgs args = new CCFlowArgs();
            args.FK_Flow = this.FK_Flow;
            args.WorkID = WorkID;
             
            this.FaSong(args);
        }
        
        public ActionResult GetHistoryData(CCFlowArgs args)
        {
            List<Process_ShiYan> syList = CurrentService.GetHistoryData(args);

            if (syList == null || syList.Count == 0)
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
                    message = "成功加载实验数据",
                    _Json = EasyUIJson.GetEasyUIJson_ShiYan(syList)
                });
            }
        }

        public ActionResult TianJiaShiYan(Process_ShiYan shiyan, Process_BasicData basicData)
        {
            shiyan.Process_BasicData = basicData;

            Dictionary<string, string> dictionary = CurrentService.AddShiYan(shiyan);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }

        public ActionResult XiuGaiShiYan(string oldNo, Process_ShiYan shiyan, Process_BasicData basicData)
        {
            shiyan.Process_BasicData = basicData;

            Dictionary<string, string> dictionary = CurrentService.ModifyShiYan(oldNo, shiyan);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }

        public ActionResult ShanChuShiYan(string syNo)
        {
            Dictionary<string, string> dictionary = CurrentService.DeleteShiYan(syNo);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }
    }
}
