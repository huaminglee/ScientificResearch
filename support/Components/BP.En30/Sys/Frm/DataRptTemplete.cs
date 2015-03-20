using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.Sys
{
    /// <summary>
    /// 报表数据存储模版
    /// </summary>
    public class DataRptAttr : EntityMyPKAttr
    {
        public const string RefOID = "RefOID";
        public const string ColCount = "ColCount";
        public const string RowCount = "RowCount";
        public const string Val = "Val";
    }
	/// <summary>
    ///  报表数据存储模版  
	/// </summary>
    public class DataRpt : EntityMyPK
    {
        #region 构造方法
        /// <summary>
        /// 报表数据存储模版
        /// </summary>
        public DataRpt()
        {
        }
        #endregion

        /// <summary>
        /// 报表数据存储模版
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Sys_DataRpt");
                map.EnDesc = "报表数据存储模版";
                map.DepositaryOfMap = Depositary.Application;

                map.AddMyPK();
                map.AddTBString(DataRptAttr.ColCount, null, "列", true, true, 0, 50, 20);
                map.AddTBString(DataRptAttr.RowCount, null, "行", true, true, 0, 50, 20);
                map.AddTBDecimal(DataRptAttr.Val, 0, "值", true, false);
                map.AddTBDecimal(DataRptAttr.RefOID, 0, "关联的值", true, false);

                this._enMap = map;
                return this._enMap;
            }
        }
    }
	/// <summary>
    /// DataRpt数据存储
	/// </summary>
    public class DataRpts : SimpleNoNames
    {
        /// <summary>
        /// DataRpt数据存储s
        /// </summary>
        public DataRpts() 
        {
        }
        /// <summary>
        /// DataRpt数据存储 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new DataRpt();
            }
        }
    }
}
