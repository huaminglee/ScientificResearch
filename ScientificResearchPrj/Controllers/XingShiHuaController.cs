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
    public class XingShiHuaController : ProcessBase<IXingShiHuaService>
    {
        public override void SetCurrentService()
        {
            this.CurrentService = SrService.XingShiHuaService;
        }

        public ActionResult XingShiHua()
        {
            SetViewData();
            return View();
        }

        public void XingShiHuaFaQi()
        {
            CCFlowArgs args = new CCFlowArgs();
            args.FK_Flow = this.FK_Flow;
            args.WorkID = WorkID;

            CurrentService.InsertOrUpdateTrack(args); 

            this.FaSong(args);
        }

        public ActionResult XingShiHuaShenHe()
        {
            SetViewData();
            return View();
        }

        public void XingShiHuaShenHeFaQi()
        {
            CCFlowArgs args = new CCFlowArgs();
            args.FK_Flow = this.FK_Flow;
            args.WorkID = WorkID;
             
            this.FaSong(args);
        }
         
        public ActionResult GetHistoryData(CCFlowArgs args)
        {
            List<Process_XingShiHua> xshList = CurrentService.GetHistoryData(args);

            if (xshList == null || xshList.Count == 0)
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
                    message = "成功加载形式化数据",
                    _Json = EasyUIJson.GetEasyUIJson_XingShiHua(xshList)
                });
            }
        }

        public ActionResult TianJiaXingShiHua(Process_XingShiHua xingshihua, Process_BasicData basicData)
        {
            xingshihua.Process_BasicData = basicData;

            Dictionary<string, string> dictionary = CurrentService.AddXingShiHua(xingshihua);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }

        public ActionResult XiuGaiXingShiHua(string oldNo, Process_XingShiHua xingshihua, Process_BasicData basicData)
        {
            xingshihua.Process_BasicData = basicData;

            Dictionary<string, string> dictionary = CurrentService.ModifyXingShiHua(oldNo, xingshihua);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }

        public ActionResult ShanChuXingShiHua(string xshNo)
        {
            Dictionary<string, string> dictionary = CurrentService.DeleteXingShiHua(xshNo);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        } 
    }
}
