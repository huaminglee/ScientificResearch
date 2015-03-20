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
    public class LinkController : ProcessBase<ILinkService>
    {
        public override void SetCurrentService()
        {
            this.CurrentService = SrService.LinkService;
        }

        public ActionResult GetLinkHistoryData(string No_OID, string isShenHe)
        {
            List<Process_Link> linkList = CurrentService.GetHistoryData(No_OID, isShenHe);

            if (linkList.Count == 0)
            {
                return Json(new
                {
                    state = "-1",
                    message = "链接数据为空"
                });
            }
            else
            {
                return Json(new
                {
                    state = "0",
                    message = "成功加载链接数据",
                    _Json = EasyUIJson.GetEasyUIJson_Link(linkList)
                });
            }
        }

        public ActionResult TianJiaLink(Process_Link link)
        {
            Dictionary<string, string> dictionary = CurrentService.AddLink(link);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"],
                OID = dictionary["OID"]
            });
        }

        public ActionResult ShanChuLink(int OID)
        {
            Dictionary<string, string> dictionary = CurrentService.DeleteLink(OID);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }
    }
}
