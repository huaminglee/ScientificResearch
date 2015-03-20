using System;
using System.Collections;
using BP.DA;
using BP.Sys;
using BP.En;
using BP.WF.Port;
using BP.WF.Template;
using BP.WF;

namespace BP.Sys
{
	/// <summary>
	/// Frm属性
	/// </summary>
    public class SysFormAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 运行类型
        /// </summary>
        public const string FormRunType = "FormRunType";
        /// <summary>
        /// 表单树
        /// </summary>
        public const string FK_FormTree = "FK_FormTree";
        /// <summary>
        /// URL
        /// </summary>
        public const string URL = "URL";
        /// <summary>
        /// PTable
        /// </summary>
        public const string PTable = "PTable";
        /// <summary>
        /// DBURL
        /// </summary>
        public const string DBURL = "DBURL";
    }
	/// <summary>
	/// 系统表单
	/// </summary>
    public class SysForm : EntityNoName
    {
        #region 基本属性
        public string PTable
        {
            get
            {
                return this.GetValStringByKey(SysFormAttr.PTable);
            }
            set
            {
                this.SetValByKey(SysFormAttr.PTable, value);
            }
        }
        public string URL
        {
            get
            {
                return this.GetValStringByKey(SysFormAttr.URL);
            }
            set
            {
                this.SetValByKey(SysFormAttr.URL, value);
            }
        }
        public FormRunType HisFormRunType
        {
            get
            {
                return (FormRunType)this.GetValIntByKey(SysFormAttr.FormRunType);
            }
            set
            {
                this.SetValByKey(SysFormAttr.FormRunType, (int)value);
            }
        }
        public string FK_FormTree
        {
            get
            {
                return this.GetValStringByKey(SysFormAttr.FK_FormTree);
            }
            set
            {
                this.SetValByKey(SysFormAttr.FK_FormTree, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// Frm
        /// </summary>
        public SysForm()
        {
        }
        /// <summary>
        /// Frm
        /// </summary>
        /// <param name="no"></param>
        public SysForm(string no)
            : base(no)
        {

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

                Map map = new Map("Sys_MapData");

                map.EnDesc = "系统表单";
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.CodeStruct = "4";
                map.IsAutoGenerNo = false;

                map.AddTBStringPK(SysFormAttr.No, null, null, true, true, 1, 4, 4);
                map.AddTBString(SysFormAttr.Name, null, null, true, false, 0, 50, 10);

                //表单的运行类型.
                map.AddDDLSysEnum(SysFormAttr.FormRunType, (int)BP.WF.FormRunType.FreeForm, "运行类型",
                    true, false, SysFormAttr.FormRunType, "@0=傻瓜表单@1=自由表单@2=自定义表单@4=Silverlight表单");

                //该表单对应的物理表
                map.AddTBString(SysFormAttr.PTable, null, "物理表", true, false, 0, 50, 10);

                // FormRunType=自定义表单时, 该字段有效. 
                map.AddTBString(SysFormAttr.URL, null, "Url", true, false, 0, 50, 10);

                //系统表单类别.
                map.AddTBString(SysFormAttr.FK_FormTree, null, "表单树", true, false, 0, 10, 20);

                map.AddTBInt(Sys.MapDataAttr.FrmW, 900, "系统表单宽度", true, false);
                map.AddTBInt(Sys.MapDataAttr.FrmH, 1200, "系统表单高度", true, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        public int FrmW
        {
            get
            {
                return this.GetValIntByKey(Sys.MapDataAttr.FrmW);
            }
        }
        public int FrmH
        {
            get
            {
                return this.GetValIntByKey(Sys.MapDataAttr.FrmH);
            }
        }
        
        #endregion
    }
	/// <summary>
    /// 系统表单s
	/// </summary>
    public class SysForms : EntitiesNoName
    {
        /// <summary>
        /// Frm
        /// </summary>
        public SysForms()
        {

        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SysForm();
            }
        }
    }
}
