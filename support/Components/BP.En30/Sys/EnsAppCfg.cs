using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.En;
using BP;
namespace BP.Sys
{
	/// <summary>
	///  应用配置
	/// </summary>
    public class EnsAppCfgAttr : EntityNoAttr
    {
        /// <summary>
        /// 分组标签
        /// </summary>
        public const string EnsName = "EnsName";
        /// <summary>
        /// 配置键值
        /// </summary>
        public const string CfgKey = "CfgKey";
        /// <summary>
        /// 值
        /// </summary>
        public const string CfgVal = "CfgVal";
    }
	/// <summary>
    /// 应用配置
	/// </summary>
    public class EnsAppCfg : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// 配置标签
        /// </summary>
        public string CfgVal
        {
            get
            {
                string val= this.GetValStrByKey(EnsAppCfgAttr.CfgVal);
                if (val == null || val == "")
                    return null;
                return val;
            }
            set
            {
                this.SetValByKey(EnsAppCfgAttr.CfgVal, value);
            }
        }
        /// <summary>
        /// Int 值
        /// </summary>
        public int CfgValOfInt
        {
            get
            {
                return this.GetValIntByKey(EnsAppCfgAttr.CfgVal);
            }
        }
        /// <summary>
        /// Boolen 值
        /// </summary>
        public bool CfgValOfBoolen
        {
            get
            {
                return this.GetValBooleanByKey(EnsAppCfgAttr.CfgVal);
            }
        }
        /// <summary>
        /// 数据源
        /// </summary>
        public string EnsName
        {
            get
            {
                return this.GetValStringByKey(EnsAppCfgAttr.EnsName);
            }
            set
            {
                this.SetValByKey(EnsAppCfgAttr.EnsName, value);
            }
        }
        /// <summary>
        /// 附件路径
        /// </summary>
        public string CfgKey
        {
            get
            {
                return this.GetValStringByKey(EnsAppCfgAttr.CfgKey);
            }
            set
            {
                this.SetValByKey(EnsAppCfgAttr.CfgKey, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 系统实体
        /// </summary>
        public EnsAppCfg()
        {
        }
        /// <summary>
        /// 系统实体
        /// </summary>
        /// <param name="no"></param>
        public EnsAppCfg(string pk)
        {
            this.MyPK = pk;
            int i = this.RetrieveFromDBSources();
            if (i == 0)
            {
                //BP.Sys.Xml.EnsAppXml xml = new BP.Sys.Xml.EnsAppXml();
            }
        }
        public EnsAppCfg(string ensName, string cfgkey)
        {
            this.MyPK = ensName + "@" + cfgkey;
            try
            {
                this.Retrieve();
            }
            catch
            {
                BP.Sys.Xml.EnsAppXmls xmls = new BP.Sys.Xml.EnsAppXmls();
                int i = xmls.Retrieve(BP.Sys.Xml.EnsAppXmlEnsName.EnsName, ensName,
                      "No", cfgkey);
                if (i == 0)
                {
                    Attrs attrs = this.EnMap.HisCfgAttrs;
                    foreach (Attr attr in attrs)
                    {
                        if (attr.Key == cfgkey)
                        {
                            this.EnsName = ensName;
                            this.CfgKey = cfgkey;
                            if (attr.Key == "FocusField")
                            {
                                Entity en = BP.En.ClassFactory.GetEns(ensName).GetNewEntity;
                                if (en.EnMap.Attrs.Contains("Name"))
                                    this.CfgVal = "Name";
                                if (en.EnMap.Attrs.Contains("Title"))
                                    this.CfgVal = "Title";
                            }
                            else
                            {
                                this.CfgVal = attr.DefaultVal.ToString();
                            }
                            this.Insert();
                            return;
                        }
                    }
                }
                BP.Sys.Xml.EnsAppXml xml = null;
                if (xmls.Count == 0)
                    xml = new Xml.EnsAppXml();
                else
                    xml = xmls[0] as BP.Sys.Xml.EnsAppXml;

                this.EnsName = ensName;
                this.CfgKey = cfgkey;
                this.CfgVal = xml.DefVal;
                this.Insert();
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
                Map map = new Map("Sys_EnsAppCfg");
                map.DepositaryOfEntity = Depositary.Application;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "实体集合配置";
                map.EnType = EnType.Sys;

                map.AddMyPK();
                map.AddTBString(EnsAppCfgAttr.EnsName, null, "实体集合", true, false, 0, 100, 60);
                map.AddTBString(EnsAppCfgAttr.CfgKey, null, "键", true, false, 0, 100, 60);
                map.AddTBString(EnsAppCfgAttr.CfgVal, null, "值", true, false, 0, 200, 60);


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
    /// 应用配置
	/// </summary>
    public class EnsAppCfgs : Entities
    {

        #region 获取数据
        public static string GetValString(string ensName, string cfgKey)
        {
            EnsAppCfg cfg = new EnsAppCfg(ensName,cfgKey);
            return  cfg.CfgVal;
        }
        public static int GetValInt(string ensName, string cfgKey)
        {
            try
            {
                EnsAppCfg cfg = new EnsAppCfg(ensName, cfgKey);
                return cfg.CfgValOfInt;
            }
            catch
            {
                return 400;
            }
        }
        public static bool GetValBoolen(string ensName, string cfgKey)
        {
            EnsAppCfg cfg = new EnsAppCfg(ensName, cfgKey);
            return cfg.CfgValOfBoolen;
        }
        #endregion 获取数据

        #region 构造
        /// <summary>
        /// 应用配置
        /// </summary>
        public EnsAppCfgs()
        {

        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new EnsAppCfg();
            }
        }
        #endregion
    }
}
