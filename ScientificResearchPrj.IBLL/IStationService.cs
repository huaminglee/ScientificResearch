using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IBLL 
{
    public interface IStationService : IBaseService<MyPort_Station>
    {
        
        PageModel<MyPort_Station> GetStations(int pageSize, int pageNow);
 
        Dictionary<string, string> AddStation(MyPort_Station station);
 
        Dictionary<string, string> DeleteStation(string staNo);

        Dictionary<string, string> ModifyStation(string oldNo, MyPort_Station station);
    }
}