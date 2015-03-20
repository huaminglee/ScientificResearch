using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP;
namespace BP.Sys
{
	/// <summary>
	/// 用户日志
	/// </summary>
    public class UserLogAttr
    {
        /// <summary>
        /// 名称
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 日志标记
        /// </summary>
        public const string LogFlag = "LogFlag";
        /// <summary>
        /// 处理内容
        /// </summary>
        public const string Docs = "Docs";
        /// <summary>
        /// 记录日期
        /// </summary>
        public const string RDT = "RDT";
        public const string IP = "IP";

    }
	/// <summary>
	/// 用户日志
	/// </summary>
	public class UserLog: EntityMyPK
	{

		#region 用户日志信息键值列表
		#endregion

		#region 基本属性
        public string IP
        {
            get
            {
                return this.GetValStringByKey(UserLogAttr.IP);
            }
            set
            {
                this.SetValByKey(UserLogAttr.IP, value);
            }
        }
        /// <summary>
        /// 日志标记键
        /// </summary>
        public string LogFlag
        {
            get
            {
                return this.GetValStringByKey(UserLogAttr.LogFlag);
            }
            set
            {
                this.SetValByKey(UserLogAttr.LogFlag, value);
            }
        }
		/// <summary>
		/// FK_Emp
		/// </summary>
		public string FK_Emp
		{
			get
			{
				return this.GetValStringByKey(UserLogAttr.FK_Emp) ; 
			}
			set
			{
				this.SetValByKey(UserLogAttr.FK_Emp,value) ; 
			}
		}
        public string RDT
        {
            get
            {
                return this.GetValStringByKey(UserLogAttr.RDT);
            }
            set
            {
                this.SetValByKey(UserLogAttr.RDT, value);
            }
        }
      
        public string Docs
        {
            get
            {
                return this.GetValStringByKey(UserLogAttr.Docs);
            }
            set
            {
                this.SetValByKey(UserLogAttr.Docs, value);
            }
        }
      
		#endregion

		#region 构造方法
		/// <summary>
		/// 用户日志
		/// </summary>
		public UserLog()
		{
		}
	 
		/// <summary>
		/// EnMap
		/// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Sys_UserLogT");
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;

                map.EnDesc = "用户日志";
                map.EnType = EnType.Sys;
                map.AddMyPK();
                map.AddTBString(UserLogAttr.FK_Emp, null, "用户", false, false, 0, 30, 20);
                map.AddTBString(UserLogAttr.IP, null, "IP", true, false, 0, 200, 20);
                map.AddTBString(UserLogAttr.LogFlag, null, "Flag", true, false, 0, 300, 20);
                map.AddTBString(UserLogAttr.Docs, null, "Docs", true, false, 0, 300, 20);
                map.AddTBString(UserLogAttr.RDT, null, "记录日期", true, false, 0, 20, 20);
                this._enMap = map;
                return this._enMap;
            }
        }
		#endregion 

        #region 重写
        public override Entities GetNewEntities
        {
            get { return new UserLogs(); }
        }
        #endregion 重写
    }
	/// <summary>
	/// 用户日志s
	/// </summary>
    public class UserLogs : EntitiesMyPK
    {
        #region 构造
        public UserLogs()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="emp"></param>
        public UserLogs(string emp)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(UserLogAttr.FK_Emp, emp);
            qo.DoQuery();
        }
        #endregion

        #region 重写
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new UserLog();
            }
        }
        #endregion

    }
}
