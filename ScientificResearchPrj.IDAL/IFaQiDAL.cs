using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScientificResearchPrj.IDAL
{
    public interface IFaQiDAL : IBaseDAL<Object>
    {
        DataTable SelectHistoryStartFlows(string fk_flow);
    }
}
