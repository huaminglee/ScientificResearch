//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ScientificResearchPrj.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Process_BasicData
    {
        public Process_BasicData()
        {
            this.Process_DeChuJieLun = new HashSet<Process_DeChuJieLun>();
            this.Process_DiaoYan = new HashSet<Process_DiaoYan>();
            this.Process_DuiBiFenXi = new HashSet<Process_DuiBiFenXi>();
            this.Process_LunWen = new HashSet<Process_LunWen>();
            this.Process_Project = new HashSet<Process_Project>();
            this.Process_ShiYan = new HashSet<Process_ShiYan>();
            this.Process_SiLu = new HashSet<Process_SiLu>();
            this.Process_SuanFa = new HashSet<Process_SuanFa>();
            this.Process_Subject_XuQiuFenXi = new HashSet<Process_Subject_XuQiuFenXi>();
            this.Process_TiChuWenTi = new HashSet<Process_TiChuWenTi>();
            this.Process_XingShiHua = new HashSet<Process_XingShiHua>();
        }
    
        public int Id { get; set; }
        public string No { get; set; }
        public string Name { get; set; }
        public string FK_Proposer { get; set; }
        public string ProposeTime { get; set; }
        public string Description { get; set; }
        public string Keys { get; set; }
        public string Remarks { get; set; }
        public string ModifyTime { get; set; }
        public string LastSendTime { get; set; }
        public string FK_Flow { get; set; }
        public string WorkId { get; set; }
        public string FK_Node { get; set; }
    
        public virtual MyPort_Emp MyPort_Emp { get; set; }
        public virtual ICollection<Process_DeChuJieLun> Process_DeChuJieLun { get; set; }
        public virtual ICollection<Process_DiaoYan> Process_DiaoYan { get; set; }
        public virtual ICollection<Process_DuiBiFenXi> Process_DuiBiFenXi { get; set; }
        public virtual ICollection<Process_LunWen> Process_LunWen { get; set; }
        public virtual ICollection<Process_Project> Process_Project { get; set; }
        public virtual ICollection<Process_ShiYan> Process_ShiYan { get; set; }
        public virtual ICollection<Process_SiLu> Process_SiLu { get; set; }
        public virtual ICollection<Process_SuanFa> Process_SuanFa { get; set; }
        public virtual ICollection<Process_Subject_XuQiuFenXi> Process_Subject_XuQiuFenXi { get; set; }
        public virtual ICollection<Process_TiChuWenTi> Process_TiChuWenTi { get; set; }
        public virtual ICollection<Process_XingShiHua> Process_XingShiHua { get; set; }
        //
        public string ProposerName { get; set; }

    }
}