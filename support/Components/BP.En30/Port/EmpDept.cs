using System;
using System.Data;
using BP.DA;
using BP.En;


namespace BP.Port
{
	/// <summary>
	/// 人员部门对应 -属性
	/// </summary>
	public class EmpDeptAttr  
	{
		#region 基本属性
		/// <summary>
		/// 工作人员ID
		/// </summary>
		public const  string FK_Emp="FK_Emp";
		/// <summary>
		/// 部门
		/// </summary>
		public const  string FK_Dept="FK_Dept";		 
		#endregion	
	}
	/// <summary>
    /// 人员部门对应
	/// </summary>
	public class EmpDept :EntityMM
	{

		public override UAC HisUAC
		{
			get
			{
				UAC uac = new UAC();
				if (BP.Web.WebUser.No== "admin"   )
				{
					uac.IsView=true;
					uac.IsDelete=true;
					uac.IsInsert=true;
					uac.IsUpdate=true;
					uac.IsAdjunct=true;
				}
				return uac;
			}
		}
		 

		#region 基本属性
		/// <summary>
		/// 工作人员ID
		/// </summary>
		public string FK_Emp
		{
			get
			{
				return this.GetValStringByKey(EmpDeptAttr.FK_Emp);
			}
			set
			{
				SetValByKey(EmpDeptAttr.FK_Emp,value);
			}
		}
        public string FK_DeptT
        {
            get
            {
                return this.GetValRefTextByKey(EmpDeptAttr.FK_Dept);
            }
        }
		/// <summary>
		///部门
		/// </summary>
		public string FK_Dept
		{
			get
			{
				return this.GetValStringByKey(EmpDeptAttr.FK_Dept);
			}
			set
			{
				SetValByKey(EmpDeptAttr.FK_Dept,value);
			}
		}		  
		#endregion

		#region 扩展属性
		 
		#endregion		

		#region 构造函数
		/// <summary>
		/// 工作人员岗位
		/// </summary> 
		public EmpDept(){}
		/// <summary>
		/// 工作人员部门对应
		/// </summary>
		/// <param name="_empoid">工作人员ID</param>
		/// <param name="wsNo">部门编号</param> 	
		public EmpDept(string _empoid,string wsNo)
		{
			this.FK_Emp  = _empoid;
			this.FK_Dept = wsNo ;
			if (this.Retrieve()==0)
				this.Insert();
		}		
		/// <summary>
		/// 重写基类方法
		/// </summary>
		public override Map EnMap
		{
			get
			{
				if (this._enMap!=null) 
					return this._enMap;
				
				Map map = new Map("Port_EmpDept");
				map.EnDesc="工作人员部门对应信息";	
				map.EnType=EnType.Dot2Dot; //实体类型，admin 系统管理员表，PowerAble 权限管理表,也是用户表,你要想把它加入权限管理里面请在这里设置。。

             //   map.AddTBStringPK(EmpDeptAttr.FK_Emp, null, "Emp", false, false, 1, 15,1);
                //map.AddTBStringPK(EmpDeptAttr.FK_Dept, null, "Dept", false, false, 1, 15,1);
                //map.AddDDLEntitiesPK(EmpDeptAttr.FK_Emp,null,"操作员",new Emps(),true);

                map.AddTBStringPK(EmpDeptAttr.FK_Emp, null, "操作员", false, false, 1, 20, 1);
				map.AddDDLEntitiesPK(EmpDeptAttr.FK_Dept,null,"部门",new Depts(),true);


                //map.AddDDLEntitiesPK(EmpDeptAttr.FK_Emp,0, DataType.AppInt,"操作员",new 县局(),"OID","Name",true);
				//map.AddSearchAttr(EmpDeptAttr.FK_Emp);
				//map.AddSearchAttr(EmpDeptAttr.FK_Dept);

				this._enMap=map;
				return this._enMap;
			}
		}
		#endregion

		#region 重载基类方法
		/// <summary>
		/// 插入前所做的工作
		/// </summary>
		/// <returns>true/false</returns>
		protected override bool beforeInsert()
		{
			return base.beforeInsert();			
		}
		/// <summary>
		/// 更新前所做的工作
		/// </summary>
		/// <returns>true/false</returns>
		protected override bool beforeUpdate()
		{
			return base.beforeUpdate(); 
		}
		/// <summary>
		/// 删除前所做的工作
		/// </summary>
		/// <returns>true/false</returns>
		protected override bool beforeDelete()
		{
			return base.beforeDelete(); 
		}
		#endregion 
	
	}
	/// <summary>
    /// 人员部门对应s -集合 
	/// </summary>
	public class EmpDepts : EntitiesMM
	{
		#region 构造
		/// <summary>
		/// 工作人员与部门集合
		/// </summary>
		public EmpDepts(){}
		/// <summary>
		/// 工作人员与部门集合
		/// </summary>
		/// <param name="FK_Emp">FK_Emp</param>
		public EmpDepts(string  FK_Emp)
		{
			QueryObject qo = new QueryObject(this);
			qo.AddWhere(EmpDeptAttr.FK_Emp, FK_Emp);
			qo.DoQuery();
		}
		#endregion

		#region 重写方法
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new EmpDept();
			}
		}	
		#endregion 

		#region 查询方法
		/// <summary>
		/// 部门对应的节点
		/// </summary>
		/// <param name="stationNo">部门编号</param>
		/// <returns>节点s</returns>
		public Emps GetHisEmps(string stationNo)
		{
			QueryObject qo = new QueryObject(this);
			qo.AddWhere(EmpDeptAttr.FK_Dept, stationNo );
			qo.DoQuery();

			Emps ens = new Emps();
			foreach(EmpDept en in this)
			{
				ens.AddEntity( new Emp(en.FK_Emp )) ;
			}
			return ens;
		}
		/// <summary>
		/// 得到他的部门权限
		/// </summary>
		/// <param name="empid">empid </param>
		/// <returns>Depts</returns> 
		public Depts GetHisDepts(string empid)
		{
			QueryObject qo = new QueryObject(this);
			qo.AddWhere(EmpDeptAttr.FK_Emp,empid);
			qo.DoQuery();
			Depts ens = new Depts();
			foreach(EmpDept en in this)
			{				 
				ens.AddEntity( new Dept(en.FK_Dept) ) ;				 
			}			 
			return ens;
		}
		#endregion
				
	}
	
}
