using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.Sys
{
    /// <summary>
    /// 属性
    /// </summary>
    public class DefValAttr : EntityTreeAttr
    {
        /// <summary>
        /// 父节点编号
        /// </summary>
        public const string ParentNo = "ParentNo";
        /// <summary>
        /// 是否父节点
        /// </summary>
        public const string IsParent = "IsParent";
        /// <summary>
        /// 类别
        /// </summary>
        public const string WordsSort = "WordsSort";
        /// <summary>
        /// 节点表编号
        /// </summary>
        public const string NodeName = "NodeName";
        /// <summary>
        /// 节点对应字段
        /// </summary>
        public const string NodeAttrKey = "NodeAttrKey";
        /// <summary>
        /// 历史词汇
        /// </summary>
        public const string IsHisWords = "IsHisWords";
        /// <summary>
        /// 人员编号
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 节点文本
        /// </summary>
        public const string CurValue = "CurValue";
    }
    /// <summary>
    /// 默认值
    /// </summary>
    public class DefVal : EntityTree
    {
        #region 基本属性
        /// <summary>
        /// 父节点编号
        /// </summary>
        public string ParentNo
        {
            get
            {
                return this.GetValStringByKey(DefValAttr.ParentNo);
            }
            set
            {
                this.SetValByKey(DefValAttr.ParentNo, value);
            }
        }
        /// <summary>
        /// 是否父节点
        /// </summary>
        public string IsParent
        {
            get
            {
                return this.GetValStringByKey(DefValAttr.IsParent);
            }
            set
            {
                this.SetValByKey(DefValAttr.IsParent, value);
            }
        }
        /// <summary>
        /// 词汇类别
        /// </summary>
        public string WordsSort
        {
            get
            {
                return this.GetValStringByKey(DefValAttr.WordsSort);
            }
            set
            {
                this.SetValByKey(DefValAttr.WordsSort, value);
            }
        }
        /// <summary>
        /// 节点编号
        /// </summary>
        public string NodeName
        {
            get
            {
                return this.GetValStringByKey(DefValAttr.NodeName);
            }
            set
            {
                this.SetValByKey(DefValAttr.NodeName, value);
            }
        }
        /// <summary>
        /// 节点对应字段
        /// </summary>
        public string NodeAttrKey
        {
            get
            {
                return this.GetValStringByKey(DefValAttr.NodeAttrKey);
            }
            set
            {
                this.SetValByKey(DefValAttr.NodeAttrKey, value);
            }
        }
        /// <summary>
        /// 是否历史词汇
        /// </summary>
        public string IsHisWords
        {
            get
            {
                return this.GetValStringByKey(DefValAttr.IsHisWords);
            }
            set
            {
                this.SetValByKey(DefValAttr.IsHisWords, value);
            }
        }
        /// <summary>
        /// 人员编号
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(DefValAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(DefValAttr.FK_Emp, value);
            }
        }
        /// <summary>
        /// 节点文本
        /// </summary>
        public string CurValue
        {
            get
            {
                return this.GetValStringByKey(DefValAttr.CurValue);
            }
            set
            {
                this.SetValByKey(DefValAttr.CurValue, value);
            }
        }
        #endregion

        #region 构造方法

        /// <summary>
        /// 默认值
        /// </summary>
        public DefVal()
        {
        }
        /// <summary>
        /// map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null) return this._enMap;
                Map map = new Map("Sys_DefVal");
                map.EnType = EnType.Sys;
                map.EnDesc = "默认值";
                map.DepositaryOfEntity = Depositary.Application;
                map.DepositaryOfMap = Depositary.Application;
                //秦2015-1-10   根据公司需求改动   以下是源码
                //map.AddTBStringPK(DefValAttr.No, null, "编号", true, true, 1, 50, 20);
                //map.AddTBString(DefValAttr.EnsName, null, "类名称", false, true, 0, 100, 10);
                //map.AddTBString(DefValAttr.EnsDesc, null, "类描述", false, true, 0, 100, 10);
                //map.AddTBString(DefValAttr.AttrKey, null, "属性", false, true, 0, 100, 10);
                //map.AddTBString(DefValAttr.AttrDesc, null, "属性描述", false, false, 0, 100, 10);
                //map.AddTBString(DefValAttr.FK_Emp, null, "人员", false, true, 0, 100, 10);
                //map.AddTBString(DefValAttr.Val, null, "值", true, false, 0, 1000, 10);
                //map.AddTBString(DefValAttr.ParentNo, null, "父节点编号", false, false, 0, 50, 20);
                //map.AddTBInt(DefValAttr.IsParent, 0, "是否父节点", false, false);
                //map.AddTBString(DefValAttr.HistoryWords, null, "历史词汇", false, false, 0, 2000, 20);

                map.AddTBStringPK(DefValAttr.No, null, "编号", true, true, 1, 50, 20);
                map.AddTBString(DefValAttr.ParentNo, null, "父节点编号", false, false, 0, 50, 20);
                map.AddTBInt(DefValAttr.IsParent, 0, "是否父节点", false, false);//1,0  Y/N
                map.AddTBInt(DefValAttr.WordsSort, 0, "词汇类别", false, false);//0,1,2...   表单-退回-移交...(暂时)
                map.AddTBString(DefValAttr.NodeName, null, "节点编号", false, false, 0, 50, 20);
                map.AddTBString(DefValAttr.NodeAttrKey, null, "节点对应字段", false, false, 0, 50, 20);
                map.AddTBInt(DefValAttr.IsHisWords, 0, "是否历史词汇", false, false);//1,0 Y/N
                map.AddTBString(DefValAttr.FK_Emp, null, "人员", false, true, 0, 100, 10);
                map.AddTBString(DefValAttr.CurValue, null, "节点文本", false, true, 0, 100, 10);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 默认值s
    /// </summary>
    public class DefVals : EntitiesNoName
    {
        /// <summary>
        /// 默认值
        /// </summary>
        public DefVals()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new DefVal();
            }
        }
    }
}
