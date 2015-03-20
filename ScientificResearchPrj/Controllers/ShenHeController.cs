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
    public class ShenHeController : ProcessBase<IShenHeService>
    {
        public override void SetCurrentService()
        {
            this.CurrentService = SrService.ShenHeService;
        }
         
        public ActionResult GetShenHeHistoryData(CCFlowArgs args, string shenHeRen)
        {
            List<Process_ShenHe> shenheList = new List<Process_ShenHe>();
            Process_ShenHe shenhe = CurrentService.GetShenHeHistoryData(args, shenHeRen);

            shenheList.Add(shenhe);

            if (shenhe.OID == 0)
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
                    message = "成功加载审核数据",
                    _Json = EasyUIJson.GetEasyUIJson_ShenHeJieGuo(shenheList)
                });
            }
        }

        public ActionResult GetShenHeHistoryDataWithoutCurrentLoginUser(CCFlowArgs args, string shenHeRen)
        {
            List<Process_ShenHe> shenheList = CurrentService.GetShenHeHistoryDataWithoutCurrentShenHeRen(args, shenHeRen);

            if (shenheList.Count == 0)
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
                    message = "成功加载审核数据",
                    _Json = EasyUIJson.GetEasyUIJson_ShenHeJieGuo(shenheList)
                });
            }
        }

        public ActionResult GetShenHeHistoryDataByStep(CCFlowArgs args, string stepType)
        {
            List<Process_ShenHe> shenheList = CurrentService.GetShenHeHistoryDataByStep(args, stepType);

            if (shenheList.Count == 0)
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
                    message = "成功加载审核数据",
                    _Json = EasyUIJson.GetEasyUIJson_ShenHeJieGuo(shenheList)
                });
            }
        }

        public ActionResult TianJiaShenHe(Process_ShenHe shenhe) {
            Dictionary<string, string> dictionary = CurrentService.AddShenHe(shenhe);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }

        public ActionResult XiuGaiShenHe(int oldOID, Process_ShenHe shenhe)
        {
            Dictionary<string, string> dictionary = CurrentService.ModifyShenHe(oldOID,shenhe);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }
    }
}
