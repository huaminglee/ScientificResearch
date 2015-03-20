using Newtonsoft.Json;
using ScientificResearchPrj.BLL;
using ScientificResearchPrj.Controllers.Base;
using ScientificResearchPrj.IBLL;
using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ScientificResearchPrj.Controllers
{
    public class StationController : Controller
    {
        private IStationService stationService = new StationService();

        public ActionResult GangWeiGuanLi()
        {
            return View();
        }

        public ActionResult GetStations(int pageSize,int pageNow)
        {
            PageModel<MyPort_Station> pageModel = stationService.GetStations(pageSize,pageNow);

            List<MyPort_Station> stations=pageModel.GetList();
            if (stations == null || stations.Count==0)
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
                    message = "成功加载了" + stations.Count + "条数据",
                    _Json = EasyUIJson.GetEasyUIJson_Station(stations),
                    totalCount = pageModel.GetTotalCount(),
                    pageSize = pageModel.GetPageSize(),
                    pageNumber = pageModel.GetPageNo()
                });
            }
           
        }
 
        [HttpPost]
        public ActionResult TianJiaGangWei(MyPort_Station station)
        {
            Dictionary<string, string> dictionary = stationService.AddStation(station);
            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }
     
        [HttpPost]
        public ActionResult ShanChuGangWei(string staNo)
        {
            Dictionary<string, string> dictionary = stationService.DeleteStation(staNo);
            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }
        
        [HttpPost]
        public ActionResult XiuGaiGangWei(string oldNo,MyPort_Station station)
        {
            Dictionary<string, string> dictionary = stationService.ModifyStation(oldNo,station);
            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }
       
      
    }
}
