using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IDAL 
{
    public interface IStationDAL : IBaseDAL<MyPort_Station>
    {
        void UpdateEmpStationIdx(string oldStaIdx, string newStaIdx);
    }
}