using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ScientificResearchPrj.IDAL
{
    public interface ICommonOperationDAL : IBaseDAL<Object>
    {
        DataTable SelectReturnInfo(ReturnNodeModel retNode);
        
        DataTable SelectCurrentFlowInfoFromEmpWorks(CCFlowArgs args);

        DataTable SelectPreviousNodeInfo(CCFlowArgs args);

        DataTable SelectUnPassedFlowWithFK_Node(CCFlowArgs args);

        DataTable SelectPassedFlow(CCFlowArgs args);
    }
}
