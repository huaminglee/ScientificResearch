using ScientificResearchPrj.BLL;
using ScientificResearchPrj.Controllers.Base;
using ScientificResearchPrj.IBLL;
using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScientificResearchPrj.Controllers
{
    public class JournalController : ProcessBase<IJournalService>
    {
        public override void SetCurrentService()
        {
            this.CurrentService = SrService.JournalService;
        }

        public string RptNo
        {
            get
            {
                return "ND" + this.Request.QueryString["RptNo"] + "Rpt";
            }
        }

        public ActionResult RiZhi()
        {
            string flowTree = CurrentService.FlowSearchMethod();
            ViewData["FlowTree"] = flowTree;
            return View();
        }

        public ActionResult Search()
        {
            SetLoginUserData();

            ViewData["RptNo"] = RptNo;
            return View();
        }

        public ActionResult SearchRpt(string RptNo, int pageSize, int pageNow)
        {
            PageModelForDataTable pageModel = CurrentService.SearchRpt(RptNo, pageSize, pageNow);

            DataTable rpts = pageModel.GetTable();
            if (rpts == null || rpts.Rows == null || rpts.Rows.Count == 0)
            {
                return Json(new
                {
                    state = "-1",
                    message = "当前页数据为空"
                });
            }
            else
            {
                return Json(new
                {
                    state = "0",
                    message = "成功加载了" + rpts.Rows.Count + "条数据",
                    _Json = EasyUIJson.GetEasyUIJsonFromDataTable(rpts),
                    totalCount = pageModel.GetTotalCount(),
                    pageSize = pageModel.GetPageSize(),
                    pageNumber = pageModel.GetPageNo()
                });
            }
        }
    }
}