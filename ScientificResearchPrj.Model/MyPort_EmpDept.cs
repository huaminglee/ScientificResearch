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
    
    public partial class MyPort_EmpDept
    {
        public int Id { get; set; }
        public string FK_Emp { get; set; }
        public string FK_Dept { get; set; }
    
        public virtual MyPort_Dept MyPort_Dept { get; set; }
        public virtual MyPort_Emp MyPort_Emp { get; set; }
    }
}
