using BP.WF;
using ScientificResearchPrj.DALFactory;
using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ScientificResearchPrj.IBLL
{
   public interface ICommonOperationService
   {
       DataTable GetReturnInfo(ReturnNodeModel retNode);

       string SendWorks(CCFlowArgs args);

       string GetTrackURL(string fk_flow);

       DataTable GetCanReturnNodes(CCFlowArgs args);
       
       WorkNode GetPreviousWorkNode(CCFlowArgs args);

       string ReturnWork(ReturnNodeModel retNode);

       string DoOverFlow(FlowOver args);

       DataTable GetCurrentFlowInfoFromEmpWorks(CCFlowArgs args);

       DataTable GetPreviousNodeInfo(CCFlowArgs args);

       string Press(int workID, string msg);
   }
}
