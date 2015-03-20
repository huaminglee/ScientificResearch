using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.Port
{
	/// <summary>
	/// 操作员属性
	/// </summary>
    public class EmpAttr : BP.En.EntityNoNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 部门
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// 密码
        /// </summary>
        public const string Pass = "Pass";
        #endregion
    }
	/// <summary>
	/// Emp 的摘要说明。
	/// </summary>
    public class Emp : EntityNoName
    {
        #region 扩展属性
        /// <summary>
        /// 主要的部门。
        /// </summary>
        public Dept HisDept
        {
            get
            {
                try
                {
                    return new Dept(this.FK_Dept);
                }
                catch (Exception ex)
                {
                    throw new Exception("@获取操作员" + this.No + "部门[" + this.FK_Dept + "]出现错误,可能是系统管理员没有给他维护部门.@" + ex.Message);
                }
            }
        }
        /// <summary>
        /// 工作岗位集合。
        /// </summary>
        public Stations HisStations
        {
            get
            {
                EmpStations sts = new EmpStations();
                Stations mysts = sts.GetHisStations(this.No);
                return mysts;
                //return new Station(this.FK_Station);
            }
        }
        /// <summary>
        /// 工作部门集合
        /// </summary>
        public Depts HisDepts
        {
            get
            {
                EmpDepts sts = new EmpDepts();
                Depts dpts = sts.GetHisDepts(this.No);
                if (dpts.Count == 0)
                {
                    string sql = "select FK_Dept from Port_Emp where No='" + this.No + "' and FK_Dept in(select No from Port_Dept)";
                    string fk_dept = BP.DA.DBAccess.RunSQLReturnVal(sql) as string;
                    if (fk_dept == null)
                        return dpts;

                    Dept dept = new Dept(fk_dept);
                    dpts.AddEntity(dept);
                }
                return dpts;
            }
        }
        /// <summary>
        /// 部门编号
        /// </summary>
        public string FK_Dept
        {
            get
            {
                return this.GetValStrByKey(EmpAttr.FK_Dept);
            }
            set
            {
                this.SetValByKey(EmpAttr.FK_Dept, value);
            }
        }
        /// <summary>
        /// 部门编号
        /// </summary>
        public string FK_DeptText
        {
            get
            {
                return this.GetValRefTextByKey(EmpAttr.FK_Dept);
            }
        }
        /// <summary>
        /// 密码
        /// </summary>
        public string Pass
        {
            get
            {
                return this.GetValStrByKey(EmpAttr.Pass);
            }
            set
            {
                this.SetValByKey(EmpAttr.Pass, value);
            }
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 检查密码(可以重写此方法)
        /// </summary>
        /// <param name="pass">密码</param>
        /// <returns>是否匹配成功</returns>
        public bool CheckPass(string pass)
        {
            if (SystemConfig.IsDebug)
                return true;

            if (this.Pass == pass )
                return true;
            return false;
        }
        #endregion 公共方法

        #region 构造函数
        /// <summary>
        /// 操作员
        /// </summary>
        public Emp()
        {
        }
        /// <summary>
        /// 操作员
        /// </summary>
        /// <param name="no">编号</param>
        public Emp(string no)
        {
            this.No = no.Trim();
            if (this.No.Length == 0)
                throw new Exception("@要查询的操作员编号为空。");
            try
            {
                this.Retrieve();
            }
            catch (Exception ex)
            {
                //登陆帐号查询不到用户，使用职工编号查询。
                QueryObject obj = new QueryObject(this);
                obj.AddWhere(EmpAttr.No, no);
                int i = obj.DoQuery();
                if (i == 0)
                    i = this.RetrieveFromDBSources();
                if (i == 0)
                    throw new Exception("@用户或者密码错误：[" + no + "]，或者帐号被停用。@技术信息(从内存中查询出现错误)：ex1=" + ex.Message);
            }
        }
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForAppAdmin();
                return uac;
            }
        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map();

                #region 基本属性
                map.EnDBUrl =
                    new DBUrl(DBUrlType.AppCenterDSN); //要连接的数据源（表示要连接到的那个系统数据库）。
                map.PhysicsTable = "Port_Emp"; // 要物理表。
                map.DepositaryOfMap = Depositary.Application;    //实体map的存放位置.
                map.DepositaryOfEntity = Depositary.Application; //实体存放位置
                map.EnDesc = "用户"; // "用户";
                map.EnType = EnType.App;   //实体类型。
                #endregion

                #region 字段
                /* 关于字段属性的增加 */
                map.AddTBStringPK(EmpAttr.No, null, "编号", true, false, 1, 20, 30); 
                map.AddTBString(EmpAttr.Name, null, "名称", true, false, 0, 200, 30);
                map.AddTBString(EmpAttr.Pass, "123", "密码", false, false, 0, 20, 10);
                map.AddDDLEntities(EmpAttr.FK_Dept, null, "部门", new Port.Depts(), true);

                //map.AddTBString("Tel", null, "Tel", false, false, 0, 20, 10);
                //map.AddTBString(EmpAttr.PID, null, this.ToE("PID", "UKEY的PID"), true, false, 0, 100, 30);
                //map.AddTBString(EmpAttr.PIN, null, this.ToE("PIN", "UKEY的PIN"), true, false, 0, 100, 30);
                //map.AddTBString(EmpAttr.KeyPass, null, this.ToE("KeyPass", "UKEY的KeyPass"), true, false, 0, 100, 30);
                //map.AddTBString(EmpAttr.IsUSBKEY, null, this.ToE("IsUSBKEY", "是否使用usbkey"), true, false, 0, 100, 30);
                //map.AddDDLSysEnum("Sex", 0, "性别", "@0=女@1=男");
                #endregion 字段

                map.AddSearchAttr(EmpAttr.FK_Dept);

                #region 增加多对多属性
                //他的部门权限
                map.AttrsOfOneVSM.Add(new EmpDepts(), new Depts(), EmpDeptAttr.FK_Emp, 
                    EmpDeptAttr.FK_Dept, DeptAttr.Name, DeptAttr.No, "部门权限");
                map.AttrsOfOneVSM.Add(new EmpStations(), new Stations(), EmpStationAttr.FK_Emp, 
                    EmpStationAttr.FK_Station,DeptAttr.Name, DeptAttr.No, "岗位权限");
                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }
        /// <summary>
        /// 获取集合
        /// </summary>
        public override Entities GetNewEntities
        {
            get { return new Emps(); }
        }
        #endregion 构造函数
    }
	/// <summary>
	/// 操作员
	// </summary>
    public class Emps : EntitiesNoName
    {
        #region 构造方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Emp();
            }
        }
        /// <summary>
        /// 操作员s
        /// </summary>
        public Emps()
        {
        }
        #endregion 构造方法
    }
}
 