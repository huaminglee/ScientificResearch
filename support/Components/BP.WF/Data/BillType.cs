using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.WF.Data
{
	/// <summary>
    ///  单据类型
	/// </summary>
    public class BillType : EntityNoName
    {
        #region 属性.
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStrByKey("FK_Flow");
            }
            set
            {
                this.SetValByKey("FK_Flow", value);
            }
        }
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }
        #endregion 属性.

        #region 构造方法
        /// <summary>
        /// 单据类型
        /// </summary>
        public BillType()
        {
        }
        /// <summary>
        /// 单据类型
        /// </summary>
        /// <param name="_No"></param>
        public BillType(string _No) : base(_No) { }
        /// <summary>
        /// 单据类型Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("WF_BillType");
                map.EnDesc = "单据类型";
                map.CodeStruct = "2";
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.IsAutoGenerNo = true;

                map.AddTBStringPK(SimpleNoNameAttr.No, null, "编号", true, true, 2, 2, 2);
                map.AddTBString(SimpleNoNameAttr.Name, null, "名称", true, false, 1, 50, 50);
                map.AddTBString("FK_Flow", null, "流程", true, false, 1, 50, 50);

                map.AddTBInt("IDX", 0, "IDX", false, false);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
    /// 单据类型
	/// </summary>
    public class BillTypes : SimpleNoNames
    {
        /// <summary>
        /// 单据类型s
        /// </summary>
        public BillTypes() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new BillType();
            }
        }
    }
}
