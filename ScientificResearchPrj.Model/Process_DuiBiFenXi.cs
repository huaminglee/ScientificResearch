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
    
    public partial class Process_DuiBiFenXi
    {
        public Process_DuiBiFenXi()
        {
            this.Process_DeChuJieLun = new HashSet<Process_DeChuJieLun>();
        }
    
        public int Id { get; set; }
        public int OID { get; set; }
        public string FK_BDNo { get; set; }
        public string Data { get; set; }
        public string Methods { get; set; }
        public string AnalysisResult { get; set; }
        public string InferType { get; set; }
        public string InferContent { get; set; }
        public Nullable<int> FK_SYOID { get; set; }
    
        public virtual Process_BasicData Process_BasicData { get; set; }
        public virtual ICollection<Process_DeChuJieLun> Process_DeChuJieLun { get; set; }
        public virtual Process_ShiYan Process_ShiYan { get; set; }
        //
        public string FK_SYName { get; set; }

    }
}
