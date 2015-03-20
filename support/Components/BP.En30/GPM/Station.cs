using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.GPM
{	 
	/// <summary>
	/// 岗位属性
	/// </summary>
    public class StationAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 岗位类型
        /// </summary>
        public const string FK_StationType = "FK_StationType";
        /// <summary>
        /// 素质要求
        /// </summary>
        public const string Makings = "Makings";
        /// <summary>
        /// 职责要求
        /// </summary>
        public const string DutyReq = "DutyReq";
    }
	/// <summary>
	/// 岗位
	/// </summary>
    public class Station : EntityNoName
    {
        #region 属性
        public string FK_StationType
        {
            get
            {
                return this.GetValStrByKey(StationAttr.FK_StationType);
            }
            set
            {
                this.SetValByKey(StationAttr.FK_StationType, value);
            }
        }
        #endregion

        #region 实现基本的方方法
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 岗位
        /// </summary> 
        public Station()
        {
        }
        /// <summary>
        /// 岗位
        /// </summary>
        /// <param name="_No"></param>
        public Station(string _No) : base(_No) { }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Port_Station");
                map.EnDesc = "岗位"; // "岗位";
                map.EnType = EnType.Admin;
                map.DepositaryOfMap = Depositary.Application;
                map.DepositaryOfEntity = Depositary.Application;
                map.CodeStruct = "2222222"; // 最大级别是 7 .

                map.AddTBStringPK(EmpAttr.No, null, "编号", true, false, 1, 20, 100);
                map.AddTBString(EmpAttr.Name, null, "名称", true, false, 0, 100, 100);

                map.AddTBStringDoc(StationAttr.DutyReq, null, "职责要求", true, false);
                map.AddTBStringDoc(StationAttr.Makings, null, "素质要求", true, false);

                map.AddDDLEntities(StationAttr.FK_StationType, null, "类型", new StationTypes(), true);


                map.AddSearchAttr(StationAttr.FK_StationType);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	 /// <summary>
	 /// 岗位s
	 /// </summary>
	public class Stations : EntitiesNoName
	{
		/// <summary>
		/// 岗位
		/// </summary>
        public Stations() { }
		/// <summary>
		/// 得到它的 Entity
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new BP.GPM.Station();
			}
		}
	}
}
