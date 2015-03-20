using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP;
namespace BP.Sys
{
	/// <summary>
	///  配置信息
	/// </summary>
    public class EnCfgAttr : EntityNoAttr
    {
        /// <summary>
        /// 分组标签
        /// </summary>
        public const string GroupTitle = "GroupTitle";
        /// <summary>
        /// 附件路径
        /// </summary>
        public const string FJSavePath = "FJSavePath";
        /// <summary>
        /// 附件路径
        /// </summary>
        public const string FJWebPath = "FJWebPath";
        /// <summary>
        /// 数据分析方式
        /// </summary>
        public const string Datan = "Datan";
    }
	/// <summary>
	/// EnCfgs
	/// </summary>
    public class EnCfg : EntityNo
    {
        #region 基本属性
        /// <summary>
        /// 数据分析方式
        /// </summary>
        public string Datan
        {
            get
            {
                return this.GetValStringByKey(EnCfgAttr.Datan);
            }
            set
            {
                this.SetValByKey(EnCfgAttr.Datan, value);
            }
        }
        /// <summary>
        /// 数据源
        /// </summary>
        public string GroupTitle
        {
            get
            {
                return this.GetValStringByKey(EnCfgAttr.GroupTitle);
            }
            set
            {
                this.SetValByKey(EnCfgAttr.GroupTitle, value);
            }
        }
        /// <summary>
        /// 附件路径
        /// </summary>
        public string FJSavePath
        {
            get
            {
                string str = this.GetValStringByKey(EnCfgAttr.FJSavePath);
                if (str == "" || str == null || str==string.Empty)
                    return BP.Sys.SystemConfig.PathOfDataUser + this.No + "\\";
                return str;
            }
            set
            {
                this.SetValByKey(EnCfgAttr.FJSavePath, value);
            }
        }
        public string FJWebPath
        {
            get
            {
                string str = this.GetValStringByKey(EnCfgAttr.FJWebPath);
                if (str == "" || str == null)
                    return BP.Sys.Glo.Request.ApplicationPath +"/DataUser/" + this.No + "/";
                return str;
            }
            set
            {
                this.SetValByKey(EnCfgAttr.FJWebPath, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 系统实体
        /// </summary>
        public EnCfg()
        {
        }
        /// <summary>
        /// 系统实体
        /// </summary>
        /// <param name="no"></param>
        public EnCfg(string enName)
        {
            this.No = enName;
            try
            {
                this.Retrieve();
            }
            catch (Exception ex)
            {
            }
        }
        /// <summary>
        /// map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Sys_EnCfg");
                map.DepositaryOfEntity = Depositary.Application;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "实体配置";
                map.EnType = EnType.Sys;

                map.AddTBStringPK(EnCfgAttr.No, null, "实体名称", true, false, 1, 100, 60);
                map.AddTBString(EnCfgAttr.GroupTitle, null, "分组标签", true, false, 0, 2000, 60);
                map.AddTBString(EnCfgAttr.FJSavePath, null, "保存路径", true, false, 0, 100, 60);
                map.AddTBString(EnCfgAttr.FJWebPath, null, "附件Web路径", true, false, 0, 100, 60);
                map.AddTBString(EnCfgAttr.Datan, null, "字段数据分析方式", true, false, 0, 200, 60);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
	/// 实体集合
	/// </summary>
    public class EnCfgs : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 配置信息
        /// </summary>
        public EnCfgs()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new EnCfg();
            }
        }
        #endregion
    }
}
