using ScientificResearchPrj.BLL;
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
    public class XiangMuZuController : Controller
    {

        private IProjectGroupService projectGroupService = new ProjectGroupService();

        public ActionResult XiangMuZuGuanLi()
        {
            return View();
        }

        public ActionResult GetXiangMuZu(int pageSize, int pageNow)
        {
            PageModel<Process_ProjectGroup> pageModel = projectGroupService.GetProjectGroups(pageSize, pageNow);

            List<Process_ProjectGroup> projectGroups = pageModel.GetList();
            if (projectGroups == null || projectGroups.Count == 0)
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
                    message = "成功加载了" + projectGroups.Count + "条数据",
                    _Json = EasyUIJson.GetEasyUIJson_ProjectGroup(projectGroups),
                    totalCount = pageModel.GetTotalCount(),
                    pageSize = pageModel.GetPageSize(),
                    pageNumber = pageModel.GetPageNo()
                });
            }
        }

        public ActionResult GetXiangMuZuForCombobox()
        {
            List<Process_ProjectGroup> projectGroups = projectGroupService.GetProjectGroups();

            if (projectGroups == null || projectGroups.Count == 0)
            {
                return Json(new
                {
                    state = "-1",
                    message = "当前项目组数据为空"
                });
            }
            else
            {
                return Json(new
                {
                    state = "0",
                    message = "成功加载了" + projectGroups.Count + "条数据",
                    _Json = EasyUIJson.GetEasyUIJson_ProjectGroupForCombobox(projectGroups),
                });
            }
        }

        [HttpPost]
        public ActionResult TianJiaXiangMuZu(Process_ProjectGroup pg)
        {
            Dictionary<string, string> dictionary = projectGroupService.AddProjectGroup(pg);
            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }

        [HttpPost]
        public ActionResult ShanChuXiangMuZu(string prjGroupNo)
        {
            Dictionary<string, string> dictionary = projectGroupService.DeleteProjectGroup(prjGroupNo);
            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        } 

        [HttpPost]
        public ActionResult XiuGaiXiangMuZu(string oldNo, Process_ProjectGroup pg)
        {
            Dictionary<string, string> dictionary = projectGroupService.ModifyProjectGroup(oldNo, pg);
            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }


    }
}