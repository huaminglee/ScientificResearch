﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ProcessEntities : DbContext
    {
        public ProcessEntities()
            : base("name=ProcessEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<MyPort_Dept> MyPort_Dept { get; set; }
        public DbSet<MyPort_Emp> MyPort_Emp { get; set; }
        public DbSet<MyPort_EmpDept> MyPort_EmpDept { get; set; }
        public DbSet<MyPort_EmpStation> MyPort_EmpStation { get; set; }
        public DbSet<MyPort_Station> MyPort_Station { get; set; }
        public DbSet<MyPort_Student> MyPort_Student { get; set; }
        public DbSet<MyPort_Tutor> MyPort_Tutor { get; set; }
        public DbSet<Process_Attach> Process_Attach { get; set; }
        public DbSet<Process_BasicData> Process_BasicData { get; set; }
        public DbSet<Process_DeChuJieLun> Process_DeChuJieLun { get; set; }
        public DbSet<Process_DiaoYan> Process_DiaoYan { get; set; }
        public DbSet<Process_DuiBiFenXi> Process_DuiBiFenXi { get; set; }
        public DbSet<Process_GroupMember> Process_GroupMember { get; set; }
        public DbSet<Process_Link> Process_Link { get; set; }
        public DbSet<Process_LunWen> Process_LunWen { get; set; }
        public DbSet<Process_Project> Process_Project { get; set; }
        public DbSet<Process_ProjectGroup> Process_ProjectGroup { get; set; }
        public DbSet<Process_ShenHe> Process_ShenHe { get; set; }
        public DbSet<Process_ShiYan> Process_ShiYan { get; set; }
        public DbSet<Process_SiLu> Process_SiLu { get; set; }
        public DbSet<Process_SuanFa> Process_SuanFa { get; set; }
        public DbSet<Process_Subject_XuQiuFenXi> Process_Subject_XuQiuFenXi { get; set; }
        public DbSet<Process_TiChuWenTi> Process_TiChuWenTi { get; set; }
        public DbSet<Process_Track> Process_Track { get; set; }
        public DbSet<Process_XingShiHua> Process_XingShiHua { get; set; }
    }
}
