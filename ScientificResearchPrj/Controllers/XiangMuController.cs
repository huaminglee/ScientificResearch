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
    public class XiangMuController : ProcessBase<IProjectService>
    {
        public override void SetCurrentService()
        {
            this.CurrentService = this.SrService.ProjectService;
        }
         
        public ActionResult XiangMuGuanLi()
        {
            ViewData["LoginUser"] = BP.Web.WebUser.No;
            ViewData["LoginUserName"] = BP.Web.WebUser.Name;

            return View();
        }

        public ActionResult GetXiangMu(int pageSize, int pageNow)
        {
            PageModel<Process_Project> pageModel = this.CurrentService.GetProjects(pageSize, pageNow);

            List<Process_Project> projects = pageModel.GetList();
            if (projects == null || projects.Count == 0)
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
                    message = "成功加载了" + projects.Count + "条数据",
                    _Json = EasyUIJson.GetEasyUIJson_Project(projects),
                    totalCount = pageModel.GetTotalCount(),
                    pageSize = pageModel.GetPageSize(),
                    pageNumber = pageModel.GetPageNo()
                });
            }
        }

        public ActionResult GetXiangMuByNo(string priNo)
        {
            List<Process_Project> priList = new List<Process_Project>();
            Process_Project prj = this.CurrentService.GetProjectByNo(priNo);

            priList.Add(prj);

            if (prj.FK_BDNo == null)
            {
                return Json(new
                {
                    state = "-1",
                    message = "当前项目数据为空"
                });
            }
            else
            {
                return Json(new
                {
                    state = "0",
                    message = "成功加载项目数据",
                    _Json = EasyUIJson.GetEasyUIJson_Project(priList) 
                });
            }
        }

        public ActionResult GetXiangMuByOID(int priOID)
        {
            List<Process_Project> priList = new List<Process_Project>();
            Process_Project prj = this.CurrentService.GetProjectByOID(priOID);

            priList.Add(prj);

            if (prj.FK_BDNo == null)
            {
                return Json(new
                {
                    state = "-1",
                    message = "当前项目数据为空"
                });
            }
            else
            {
                return Json(new
                {
                    state = "0",
                    message = "成功加载项目数据",
                    _Json = EasyUIJson.GetEasyUIJson_Project(priList)
                });
            }
        }

        [HttpPost]
        public ActionResult TianJiaXiangMu(Process_Project prj, Process_BasicData basicData)
        {
            prj.Process_BasicData = basicData;
            Dictionary<string, string> dictionary = CurrentService.AddProject(prj);
           
            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }

        [HttpPost]
        public ActionResult ShanChuXiangMu(string prjNo)
        {
            Dictionary<string, string> dictionary = CurrentService.DeleteProject(prjNo);
            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }

        [HttpPost]
        public ActionResult XiuGaiXiangMu(string oldNo, Process_Project prj, Process_BasicData basicData)
        {
            prj.Process_BasicData = basicData;
            Dictionary<string, string> dictionary = CurrentService.ModifyProject(oldNo, prj);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }
    }
}