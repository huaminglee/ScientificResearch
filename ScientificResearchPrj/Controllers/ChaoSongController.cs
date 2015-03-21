using ScientificResearchPrj.Controllers.Base;
using ScientificResearchPrj.BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ScientificResearchPrj.IBLL;
using ScientificResearchPrj.Model;

namespace ScientificResearchPrj.Controllers
{
    public class ChaoSongController : ProcessBase<IChaoSongService>
    {
        public override void SetCurrentService()
        {
            this.CurrentService = SrService.ChaoSongService;
        }
         
        public ActionResult ChaoSong()
        {
            string tabIndex = Request.QueryString["tabIndex"];
            if (tabIndex != null)
            {
                ViewData["tabIndex"] = tabIndex;
            }
            DataTable ccList_All = CurrentService.GetAllCClist();
            DataTable ccList_Read = CurrentService.GetReadCClist();
            DataTable ccList_UnRead = CurrentService.GetUnReadCClist();
            DataTable ccList_Delete = CurrentService.GetDeleteCClist();
            ViewData["ccList_All"] = EasyUIJson.GetEasyUIJsonFromDataTable(ccList_All);
            ViewData["ccList_Read"] = EasyUIJson.GetEasyUIJsonFromDataTable(ccList_Read);
            ViewData["ccList_UnRead"] = EasyUIJson.GetEasyUIJsonFromDataTable(ccList_UnRead);
            ViewData["ccList_Delete"] = EasyUIJson.GetEasyUIJsonFromDataTable(ccList_Delete);
            return View();
        }
         
        public void CCYiYue()
        {
            string myPK = Request.Form["MyPK"];
            if (string.IsNullOrEmpty(myPK) == false)
            {
                CurrentService.CCSetRead(myPK);
            }
        }

        public void CCShanChu()
        {
            string myPK = Request.Form["MyPK"];
            if (string.IsNullOrEmpty(myPK) == false)
            {
                CurrentService.CCLogicalDelete(myPK);
            }
        }

        public void CCCheDiShanChu()
        {
            string myPK = Request.Form["MyPK"];
            if (string.IsNullOrEmpty(myPK) == false)
            {
                CurrentService.CCPhysicalDelete(myPK);
            }
        }

        public ActionResult ChaoSongZhunBei()
        {
            CCFlowArgs args = new CCFlowArgs();
            args.FK_Flow = this.FK_Flow;
            args.WorkID = this.WorkID;
            args.FID = this.FID;
            args.FK_Node = this.FK_Node;

            DataTable previousNodesInfo = this.SrService.CommonOperationService.GetPreviousNodeInfo(args);

            if (previousNodesInfo != null && previousNodesInfo.Rows.Count == 0)
            {
                return Json(new
                {
                    state = "-1",
                    message = "获取以往节点信息失败."
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                state = "0",
                message = "加载成功",
                _Json = EasyUIJson.GetEasyUIJsonFromDataTable(previousNodesInfo)
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChaoSongTo(CCModel cc)
        {
            try
            {
                string returnStr = CurrentService.WriteToCCList(cc);
                return Json(new
                {
                    state = "0",
                    message = returnStr,
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    state = "-1",
                    message = ex.Message,
                });
                //throw (ex);
            }

        }
         
    }
}