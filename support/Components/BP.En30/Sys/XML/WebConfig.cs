using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;
using BP.XML;

namespace BP.Sys.Xml
{
    /// <summary>
    /// 属性
    /// </summary>
    public class WebConfigDescAttr
    {
        /// <summary>
        /// 过错行为
        /// </summary>
        public const string No = "No";
        /// <summary>
        /// Name
        /// </summary>
        public const string Name = "Name";
        /// <summary>
        /// 表达式
        /// </summary>
        public const string URL = "URL";
        /// <summary>
        /// 描述
        /// </summary>
        public const string Note = "Note";
        /// <summary>
        /// 类型
        /// </summary>
        public const string DBType = "DBType";
        /// <summary>
        /// IsEnable
        /// </summary>
        public const string IsEnable = "IsEnable";
        public const string IsShow = "IsShow";
        /// <summary>
        /// Vals
        /// </summary>
        public const string Vals = "Vals";
    }
    /// <summary>
    /// 配置文件信息
    /// </summary>
    public class WebConfigDesc : XmlEn
    {
        #region 属性
        private string _No = "";
        public string No
        {
            get
            {
                if (_No == "")
                    return this.GetValStringByKey(WebConfigDescAttr.No);
                else
                    return _No;
            }
            set
            {
                _No = value;
            }
        }
        public string Name
        {
            get
            {
                return this.GetValStringByKey(WebConfigDescAttr.Name);
            }
        }
        public bool IsEnable
        {
            get
            {
                if (this.GetValStringByKey(WebConfigDescAttr.IsEnable) == "0")
                    return false;
                return true;
            }
        }
        public bool IsShow
        {
            get
            {
                if (this.GetValStringByKey(WebConfigDescAttr.IsShow) == "0")
                    return false;
                return true;
            }
        }
        /// <summary>
        /// 描述
        /// </summary>
        public string Note
        {
            get
            {
                return this.GetValStringByKey(WebConfigDescAttr.Note);
            }
        }
        public string Vals
        {
            get
            {
                return this.GetValStringByKey(WebConfigDescAttr.Vals);
            }
        }
        /// <summary>
        /// 类型
        /// </summary>
        public string DBType
        {
            get
            {
                return this.GetValStringByKey(WebConfigDescAttr.DBType);
            }
        }
        #endregion

        #region 构造
        public WebConfigDesc()
        {
        }
        /// <summary>
        /// 获取一个实例
        /// </summary>
        public override XmlEns GetNewEntities
        {
            get
            {
                return new WebConfigDescs();
            }
        }
        #endregion
    }
    /// <summary>
    /// 配置文件信息
    /// </summary>
    public class WebConfigDescs : XmlEns
    {
        #region 构造
        /// <summary>
        /// 配置文件信息
        /// </summary>
        public WebConfigDescs() { }
        #endregion

        #region 重写基类属性或方法。
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override XmlEn GetNewEntity
        {
            get
            {
                return new WebConfigDesc();
            }
        }
        /// <summary>
        /// 文件
        /// </summary>
        public override string File
        {
            get
            {
                return SystemConfig.PathOfXML + "\\WebConfigDesc.xml";
            }
        }
        /// <summary>
        /// 物理表名
        /// </summary>
        public override string TableName
        {
            get
            {
                return "Item";
            }
        }
        public override Entities RefEns
        {
            get
            {
                return null;
            }
        }
        #endregion
    }
}
