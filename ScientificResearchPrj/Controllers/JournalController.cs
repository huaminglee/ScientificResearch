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

            ViewData["FK_Flow"] = FK_Flow;
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

        public ActionResult ReadTrackView() {
            ViewData["FK_Flow"] = FK_Flow;
            ViewData["WorkID"] = WorkID;
            ViewData["FID"] = FID;
            return View();
        }

        public ActionResult ReadTrack()
        {
            string fk_flow = Request.Form["FK_Flow"].ToString();
            long workid = Convert.ToInt32(Request.Form["WorkID"]);
            long fid = Convert.ToInt32(Request.Form["FID"]);

            CCFlowArgs args = new CCFlowArgs();
            args.FK_Flow = fk_flow;
            args.WorkID = workid;
            args.FID = fid;

            DataTable table = CurrentService.ReadTrack(args);

            if (table == null || table.Rows == null || table.Rows.Count == 0)
            {
                return Json(new
                {
                    state = "-1",
                    message = "当前的节点数据已经被删除！原因如下：<br />1、当前节点数据被非法删除；<br />"
                                   + "2、节点数据是退回人与被退回人中间的节点，这部分节点数据查看不支持"
                });
            }
            else
            {
                return Json(new
                {
                    state = "0",
                    message = "成功加载了" + table.Rows.Count + "条数据",
                    _Json = EasyUIJson.GetEasyUIJsonFromDataTable(table) 
                });
            }
        }
    }
}