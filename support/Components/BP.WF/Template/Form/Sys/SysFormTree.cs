using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Sys
{
    /// <summary>
    /// 属性
    /// </summary>
    public class SysFormTreeAttr : EntityTreeAttr
    {
    }
    /// <summary>
    ///  流程表单树
    /// </summary>
    public class SysFormTree : EntityTree
    {
        #region 构造方法
        /// <summary>
        /// 流程表单树
        /// </summary>
        public SysFormTree()
        {
        }
        /// <summary>
        /// 流程表单树
        /// </summary>
        /// <param name="_No"></param>
        public SysFormTree(string _No) : base(_No) { }
        #endregion

        #region 系统方法.
        /// <summary>
        /// 流程表单树Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Sys_FormTree");
                map.EnDesc = "表单树";
                map.CodeStruct = "2";

                map.DepositaryOfEntity = Depositary.Application;
                map.DepositaryOfMap = Depositary.Application;

                map.AddTBStringPK(SysFormTreeAttr.No, null, "编号", true, true, 1, 10, 20);
                map.AddTBString(SysFormTreeAttr.Name, null, "名称", true, false, 0, 100, 30);
                map.AddTBString(SysFormTreeAttr.ParentNo, null, "父节点No", false, false, 0, 100, 30);
                map.AddTBString(SysFormTreeAttr.TreeNo, null, "TreeNo", false, false, 0, 100, 30);

                map.AddTBInt(SysFormTreeAttr.IsDir, 0, "是否是目录?", false, false);
                map.AddTBInt(SysFormTreeAttr.Idx, 0, "Idx", false, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion 系统方法.

        protected override bool beforeDelete()
        {
            if (!string.IsNullOrEmpty(this.No))
                DeleteChild(this.No);
            return base.beforeDelete();
        }
        /// <summary>
        /// 删除子项
        /// </summary>
        /// <param name="parentNo"></param>
        private void DeleteChild(string parentNo)
        {
            SysFormTrees formTrees = new SysFormTrees();
            formTrees.RetrieveByAttr(SysFormTreeAttr.ParentNo, parentNo);
            foreach (SysFormTree item in formTrees)
            {
                MapData md = new MapData();
                md.FK_FormTree = item.No;
                md.Delete();
                DeleteChild(item.No);
            }
        }
    }
    /// <summary>
    /// 流程表单树
    /// </summary>
    public class SysFormTrees : EntitiesTree
    {
        /// <summary>
        /// 流程表单树s
        /// </summary>
        public SysFormTrees() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SysFormTree();
            }

        }
        public override int RetrieveAll()
        {
            int i = base.RetrieveAll();
            if (i == 0)
            {
                SysFormTree fs = new SysFormTree();
                fs.Name = "公文类";
                fs.No = "01";
                fs.Insert();

                fs = new SysFormTree();
                fs.Name = "办公类";
                fs.No = "02";
                fs.Insert();
                i = base.RetrieveAll();
            }
            return i;
        }
    }
}
