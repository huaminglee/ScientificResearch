using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScientificResearchPrj.Model
{
    public class CCFlowArgs
    {
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow{ get; set; }
        /// <summary>
        /// 当前节点ID
        /// </summary>
        public int FK_Node { get; set; }
        /// <summary>
        /// 工作ID
        /// </summary>
        public Int64 WorkID { get; set; }
        /// <summary>
        /// 流程ID
        /// </summary>
        public Int64 FID { get; set; }

        public Hashtable ExpendArgs { get; set; }

    }
}
