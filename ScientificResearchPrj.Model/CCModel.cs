using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScientificResearchPrj.Model
{
    //抄送的model
    public class CCModel : CCFlowArgs
    {
        //抄送对象
        public string ChaoSongRenTo { get; set; }

        //抄送的标题
        public string ChaoSongBiaoTi { get; set; }

        //抄送的内容
        public string ChaoSongNeiRong { get; set; }
    }
}
