using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.GPM
{
    /// <summary>
    /// 职务
    /// </summary>
    public class DutyAttr : EntityNoNameAttr
    {
    }
	/// <summary>
    ///  职务
	/// </summary>
	public class Duty :EntityNoName
    {
        #region 属性
        #endregion
     
		#region 构造方法
		/// <summary>
		/// 职务
		/// </summary>
		public Duty()
        {
        }
        /// <summary>
        /// 职务
        /// </summary>
        /// <param name="_No"></param>
        public Duty(string _No) : base(_No) { }
		#endregion 

		/// <summary>
		/// 职务Map
		/// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Port_Duty");
                map.EnDesc = "职务";
                map.CodeStruct = "2";

                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;

                map.AddTBStringPK(DutyAttr.No, null, "编号", true, true, 2, 2, 2);
                map.AddTBString(DutyAttr.Name, null, "名称", true, false, 1, 50, 20);
                this._enMap = map;
                return this._enMap;
            }
        }
	}
	/// <summary>
    /// 职务
	/// </summary>
    public class Dutys : EntitiesNoName
	{
		/// <summary>
		/// 职务s
		/// </summary>
        public Dutys() { }
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
                return new Duty();
			}
		}
	}
}
