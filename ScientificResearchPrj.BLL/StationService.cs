using ScientificResearchPrj.DAL;
using ScientificResearchPrj.IBLL;
using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.BLL 
{
    public class StationService : BaseService<MyPort_Station>, IStationService
    {
        public override void SetCurrentDAL()
        {
            CurrentDAL = this.DbSession.StationDAL;
        }

        public PageModel<MyPort_Station> GetStations(int pageSize, int pageNow)
        {
            PageModel<MyPort_Station> pageModel = new PageModel<MyPort_Station>();
            int totalCount = 0;

            List<MyPort_Station> list = this.LoadPageEntities(pageNow, pageSize, out totalCount, a => true, b => b.Name ,true).ToList();

            if(list != null)  pageModel.SetList(list);
            pageModel.SetPageNo(pageNow);
            pageModel.SetPageSize(pageSize);
            pageModel.SetTotalCount(totalCount);

            return pageModel;
        }
 
        public Dictionary<string, string> AddStation(MyPort_Station station)
        {
            try
            {
                this.AddEntity(station);
              
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add("state", "0");
                dictionary.Add("message", "添加成功");
                return dictionary;
            }
            catch (Exception ex)
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add("state", "-1");
                dictionary.Add("message", "添加失败~~~" + ex.Message);
                return dictionary;
            }
        }

        public Dictionary<string, string> DeleteStation(string staNo)
        {　
            try
            {
                MyPort_Station station = this.LoadEntities(a => a.StaNo == staNo).FirstOrDefault();
                //清除多对多关系
                if (station != null)
                {
                    station.MyPort_EmpStation.Clear();
                    this.DeleteEntity(station);
                }
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add("state", "0");
                dictionary.Add("message", "删除成功");
                return dictionary;
            }
            catch (Exception ex)
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add("state", "-1");
                dictionary.Add("message", "删除失败~~~" + ex.Message);
                return dictionary;
            }
        }
 
        public Dictionary<string, string> ModifyStation(string oldNo, MyPort_Station station)
        {
            try
            {
                if (!oldNo.Equals(station.StaNo))
                {
                    MyPort_Station ifIdxHasExist = this.LoadEntities(a => a.StaNo == station.StaNo).FirstOrDefault();
                    if (ifIdxHasExist != null) throw new Exception("编号已存在");
 
                    //先添加
                    this.AddEntity(station);
                }
                else {
                    MyPort_Station stationDesc = this.LoadEntities(a => a.StaNo == oldNo).FirstOrDefault();
                    ModifyStation(stationDesc, station);

                    //先修改
                    this.UpdateEntity(stationDesc);
                }
                

                //再更新人员、岗位表的外键关系
                (CurrentDAL as StationDAL).UpdateEmpStationIdx(oldNo, station.StaNo);

                //最后删除
                if (!oldNo.Equals(station.StaNo)) {
                    MyPort_Station stationOld = this.LoadEntities(a => a.StaNo == oldNo).FirstOrDefault();
                    this.DeleteEntity(stationOld);
                }

                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add("state", "0");
                dictionary.Add("message", "修改成功");
                return dictionary;
            }
            catch (Exception ex)
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add("state", "-1");
                dictionary.Add("message", "修改失败~~~" + ex.Message);
                return dictionary;
            }
        }

        private void ModifyStation(MyPort_Station stationDesc, MyPort_Station stationSource)
        {
            stationDesc.StaNo = stationSource.StaNo;
            stationDesc.Name = stationSource.Name;
            stationDesc.Description = stationSource.Description;
            stationDesc.StaGrade = stationSource.StaGrade;
        }
    }
}