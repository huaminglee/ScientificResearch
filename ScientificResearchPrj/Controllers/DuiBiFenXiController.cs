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
    public class DuiBiFenXiController : ProcessBase<IDuiBiFenXiService>
    {
        public override void SetCurrentService()
        {
            this.CurrentService = SrService.DuiBiFenXiService;
        }

        public ActionResult DuiBiFenXi()
        {
            SetViewData();
            return View();
        }

        public void DuiBiFenXiFaQi()
        {
            CCFlowArgs args = new CCFlowArgs();
            args.FK_Flow = this.FK_Flow;
            args.WorkID = WorkID;

            CurrentService.InsertOrUpdateTrack(args); 

            this.FaSong(args);
        }

        public ActionResult DuiBiFenXiShenHe()
        {
            SetViewData();
            return View();
        }

        public void DuiBiFenXiShenHeFaQi()
        {
            CCFlowArgs args = new CCFlowArgs();
            args.FK_Flow = this.FK_Flow;
            args.WorkID = WorkID;
             
            this.FaSong(args);
        }
        
        public ActionResult GetHistoryData(CCFlowArgs args)
        {
            List<Process_DuiBiFenXi> dbfxList = CurrentService.GetHistoryData(args);

            if (dbfxList == null || dbfxList.Count == 0)
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
                    message = "成功加载量化对比分析数据",
                    _Json = EasyUIJson.GetEasyUIJson_DuiBiFenXi(dbfxList)
                });
            }
        }

        public ActionResult TianJiaDuiBiFenXi(Process_DuiBiFenXi duibifenxi, Process_BasicData basicData)
        {
            duibifenxi.Process_BasicData = basicData;

            Dictionary<string, string> dictionary = CurrentService.AddDuiBiFenXi(duibifenxi);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }

        public ActionResult XiuGaiDuiBiFenXi(string oldNo, Process_DuiBiFenXi duibifenxi, Process_BasicData basicData)
        {
            duibifenxi.Process_BasicData = basicData;

            Dictionary<string, string> dictionary = CurrentService.ModifyDuiBiFenXi(oldNo, duibifenxi);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }

        public ActionResult ShanChuDuiBiFenXi(string dbfxNo)
        {
            Dictionary<string, string> dictionary = CurrentService.DeleteDuiBiFenXi(dbfxNo);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }
    }
}
