using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScientificResearchPrj.Model
{
    //用于退回节点
    public class ReturnNodeModel : CCFlowArgs
    {
        /*如果允许原路退回*/
        public bool IsBackTracking { get; set; }
        //退回理由
        public string TuiHuiLiYou { get; set; }
        //退回节点信息
        public string ReturnNodeInfo { get; set; }
        public int ReturnToNode { get; set; }
        public string ReturnToEmps { get; set; }
    }
}
