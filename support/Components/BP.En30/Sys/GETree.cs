using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.En
{
	/// <summary>
	/// 属性
	/// </summary>
	public class GETreeAttr : EntityNoNameAttr
	{
    }
	/// <summary>
	/// 树结构实体
	/// </summary>
    public class GETree : EntityNoName
    {
        #region 构造
        public override string ToString()
        {
            return this.PhysicsTable;
        }
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }
        public GETree()
        {

        }
        /// <summary>
        /// 编号
        /// </summary>
        /// <param name="no">编号</param>
        public GETree(string no)  :base(no)
        {

        }
        public GETree(string sftable, string tableDesc)
        {
            this.PhysicsTable = sftable;
            this.Desc = tableDesc;
        }
        public override Map EnMap
        {
            get
            {
               // if (this._enMap != null) return this._enMap;
                Map map = new Map(this.PhysicsTable);
                map.EnDesc = this.Desc;
                map.IsAutoGenerNo = true;

                map.DepositaryOfEntity = Depositary.Application;
                map.DepositaryOfMap = Depositary.Application;
                map.EnType = EnType.App;
                map.IsAutoGenerNo = true;

                map.AddTBStringPK(GETreeAttr.No, null, "编号", true, true, 1, 30, 3);
                map.AddTBString(GETreeAttr.Name, null, "名称", true, false, 1, 60, 500);
                return map;
            //    this._enMap = map;
                //return this._enMap;
            }
        }
        public string PhysicsTable = null;
        public string Desc = null;

        #endregion
    }
	/// <summary>
    /// 树结构实体s
	/// </summary>
	public class GETrees : EntitiesNoName
	{
        /// <summary>
        /// 物理表
        /// </summary>
        public string SFTable = null;
        public string Desc = null;

		/// <summary>
		/// GETrees
		/// </summary>
        public GETrees()
		{
		}
        public GETrees(string sftable, string tableDesc)
        {
            this.SFTable = sftable;
            this.Desc = tableDesc;
        }
        public override Entity GetNewEntity
        {
            get 
            {
                return new GETree(this.SFTable, this.Desc);
            }
        }
        public override int RetrieveAll()
        {
            return this.RetrieveAllFromDBSource();
        }
	}
}
