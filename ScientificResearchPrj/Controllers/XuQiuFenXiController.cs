using ScientificResearchPrj.Controllers.Base;
using ScientificResearchPrj.IBLL;
using ScientificResearchPrj.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScientificResearchPrj.Controllers
{
    public class XuQiuFenXiController : ProcessBase<IXuQiuFenXiService>
    {
        public override void SetCurrentService()
        {
            this.CurrentService = SrService.XuQiuFenXiService;
        }

        public ActionResult XuQiuFenXi()
        {
            SetViewData();
            return View();
        }
       
        public void XuQiuFenXiFaQi()
        {
            CCFlowArgs args = new CCFlowArgs();
            args.FK_Flow = FK_Flow;
            args.WorkID = WorkID;

            CurrentService.InsertOrUpdateTrack(args); 

            this.FaSong(args);
        }
         
        public ActionResult XuQiuFenXiShenHe()
        {
            SetViewData();
            return View();
        }
        
        public void XuQiuFenXiShenHeFaQi()
        {
            CCFlowArgs args = new CCFlowArgs();
            args.FK_Flow = this.FK_Flow;
            args.WorkID = WorkID;
              
            this.FaSong(args);
        }

        public ActionResult GetHistoryData(CCFlowArgs args) 
        {
            List<Process_Subject_XuQiuFenXi> subList = new List<Process_Subject_XuQiuFenXi>();
            Process_Subject_XuQiuFenXi subject = CurrentService.GetHistoryData(args);

            subList.Add(subject);

            if (subject.FK_BDNo == null)
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
                    message = "成功加载需求分析课题数据",
                    _Json = EasyUIJson.GetEasyUIJson_Subject(subList)
                });
            }
        }

        

        
    }
}