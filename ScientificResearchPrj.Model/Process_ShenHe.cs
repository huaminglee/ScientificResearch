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
    
    public partial class Process_ShenHe
    {
        public int Id { get; set; }
        public int OID { get; set; }
        public string ShenHeRen { get; set; }
        public string ShenHeShiJian { get; set; }
        public string ShenHeJieGuo { get; set; }
        public string ShenHeYiJian { get; set; }
        public string ModifyTime { get; set; }
        public string FK_Flow { get; set; }
        public string FK_Node { get; set; }
        public string WorkID { get; set; }
        public string StepType { get; set; }
        //
        public string ShenHeRenName { get; set; }

    }
}
