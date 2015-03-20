using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP;
namespace BP.Sys
{
	/// <summary>
	/// 域
	/// </summary>
    public class DomainAttr:EntityNoNameAttr
    {
        /// <summary>
        /// DBLink
        /// </summary>
        public const string DBLink = "DBLink";
    }
	/// <summary>
	/// 域
	/// </summary>
	public class Domain: EntityNoName
	{
		#region 基本属性
        public string Docs
        {
            get
            {
                return this.GetValStringByKey(DomainAttr.DBLink);
            }
            set
            {
                this.SetValByKey(DomainAttr.DBLink, value);
            }
        }
		#endregion

		#region 构造方法
		/// <summary>
		/// 域
		/// </summary>
		public Domain()
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
                Map map = new Map("Sys_Domain");
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;

                map.EnDesc = "域";
                map.EnType = EnType.Sys;
                map.AddTBStringPK(DomainAttr.No, null, "编号", false, false, 0, 30, 20);
                map.AddTBString(DomainAttr.Name, null, "Name", false, false, 0, 30, 20);
                map.AddTBString(DomainAttr.DBLink, null, "DBLink", false, false, 0, 130, 20);
                 this._enMap = map;
                return this._enMap;
            }
        }
		#endregion 

        #region 重写
        public override Entities GetNewEntities
        {
            get { return new Domains(); }
        }
        #endregion 重写
    }
	/// <summary>
	/// 域s
	/// </summary>
    public class Domains : EntitiesNoName
    {
        #region 构造
        public Domains()
        {
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
                return new Domain();
            }
        }
        #endregion

    }
}
