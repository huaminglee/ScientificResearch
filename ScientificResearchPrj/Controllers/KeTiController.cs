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
    public class KeTiController : ProcessBase<ISubjectService>
    {
        public override void SetCurrentService()
        {
            this.CurrentService = this.SrService.SubjectService;
        }
        
        public ActionResult KeTiGuanLi()
        {
            ViewData["LoginUser"] = BP.Web.WebUser.No;
            ViewData["LoginUserName"] = BP.Web.WebUser.Name;

            return View();
        }

        public ActionResult GetKeTi(int pageSize, int pageNow)
        {
            PageModel<Process_Subject_XuQiuFenXi> pageModel = this.CurrentService.GetSubjects(pageSize, pageNow);

            List<Process_Subject_XuQiuFenXi> subjects = pageModel.GetList();
            if (subjects == null || subjects.Count == 0)
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
                    message = "成功加载了" + subjects.Count + "条数据",
                    _Json = EasyUIJson.GetEasyUIJson_Subject(subjects),
                    totalCount = pageModel.GetTotalCount(),
                    pageSize = pageModel.GetPageSize(),
                    pageNumber = pageModel.GetPageNo()
                });
            }
        }

        public ActionResult GetKeTiByNo(string subNo)
        {
            List<Process_Subject_XuQiuFenXi> subList = new List<Process_Subject_XuQiuFenXi>();
            Process_Subject_XuQiuFenXi sub = this.CurrentService.GetSubjectByNo(subNo);

            subList.Add(sub);

            if (sub.FK_BDNo == null)
            {
                return Json(new
                {
                    state = "-1",
                    message = "当前课题数据为空"
                });
            }
            else
            {
                return Json(new
                {
                    state = "0",
                    message = "成功加载课题数据",
                    _Json = EasyUIJson.GetEasyUIJson_Subject(subList)
                });
            }
        }

        [HttpPost]
        public ActionResult TianJiaKeTi(Process_Subject_XuQiuFenXi sub, Process_BasicData basicData)
        {
            sub.Process_BasicData = basicData;
            Dictionary<string, string> dictionary = CurrentService.AddSubject(sub);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }

        [HttpPost]
        public ActionResult ShanChuKeTi(string subNo)
        {
            Dictionary<string, string> dictionary = CurrentService.DeleteSubject(subNo);
            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }

        [HttpPost]
        public ActionResult XiuGaiKeTi(string oldNo, Process_Subject_XuQiuFenXi sub, Process_BasicData basicData)
        {
            sub.Process_BasicData = basicData;
            Dictionary<string, string> dictionary = CurrentService.ModifySubject(oldNo, sub);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }
    }
}