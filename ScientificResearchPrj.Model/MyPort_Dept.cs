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
    
    public partial class MyPort_Dept
    {
        public MyPort_Dept()
        {
            this.MyPort_EmpDept = new HashSet<MyPort_EmpDept>();
        }
    
        public int Id { get; set; }
        public string DeptNo { get; set; }
        public string Name { get; set; }
        public string ParentNo { get; set; }
        public string Description { get; set; }
    
        public virtual ICollection<MyPort_EmpDept> MyPort_EmpDept { get; set; }
        //
        public string ParentDept { get; set; }

    }
}