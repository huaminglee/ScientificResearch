using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;
using BP.Sys;

namespace BP.WF
{
    /// <summary>
    /// 流程类别属性
    /// </summary>
    public class FlowSortAttr : EntityTreeAttr
    {
    }
    /// <summary>
    ///  流程类别
    /// </summary>
    public class FlowSort : EntityTree
    {
        #region 构造方法
        /// <summary>
        /// 流程类别
        /// </summary>
        public FlowSort()
        {
        }
        /// <summary>
        /// 流程类别
        /// </summary>
        /// <param name="_No"></param>
        public FlowSort(string _No) : base(_No) { }
        #endregion

        /// <summary>
        /// 流程类别Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_FlowSort");
                map.EnDesc = "流程类别";
                map.CodeStruct = "2";
                map.IsAllowRepeatName = false;

                map.DepositaryOfEntity = Depositary.Application;
                map.DepositaryOfMap = Depositary.Application;

                map.AddTBStringPK(FlowSortAttr.No, null, "编号", true, true, 1, 10, 20);
                map.AddTBString(FlowSortAttr.Name, null, "名称", true, false, 0, 100, 30);
                map.AddTBString(FlowSortAttr.ParentNo, null, "父节点No", false, false, 0, 100, 30);
                map.AddTBString(FlowSortAttr.TreeNo, null, "TreeNo", false, false, 0, 100, 30);

                map.AddTBInt(FlowSortAttr.Idx, 0, "Idx", false, false);
                map.AddTBInt(FlowSortAttr.IsDir, 0, "IsDir", false, false);

                this._enMap = map;
                return this._enMap;
            }
        }

        #region 重写方法
        public void WritToGPM()
        {
            return;

            if (BP.WF.Glo.OSModel == OSModel.WorkFlow)
                return;

            string pMenuNo = "";

            #region 检查系统是否存在，并返回系统菜单编号
            string sql = "SELECT * FROM GPM_App where No='" + SystemConfig.SysNo + "'";

            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (dt != null && dt.Rows.Count == 0)
            {
                //系统类别
                sql = "SELECT No FROM GPM_Menu WHERE ParentNo=0";
                string sysRootNo = DBAccess.RunSQLReturnStringIsNull(sql,"0");
                // 取得该功能菜单的主键编号.
                pMenuNo = DBAccess.GenerOID("BP.GPM.Menu").ToString();
                string url = Glo.HostURL;
                /*如果没有系统，就插入该系统菜单.*/
                sql = "INSERT INTO GPM_Menu(No,Name,ParentNo,IsDir,MenuType,FK_App,IsEnable,Flag)";
                sql += " VALUES('{0}','{1}','{2}',1,2,'{3}',1,'{4}')";
                sql = string.Format(sql, pMenuNo, SystemConfig.SysName, sysRootNo, SystemConfig.SysNo, "FlowSort" + SystemConfig.SysNo);
                DBAccess.RunSQL(sql);
                /*如果没有系统，就插入该系统.*/
                sql = "INSERT INTO GPM_App(No,Name,AppModel,FK_AppSort,Url,RefMenuNo,MyFileName)";
                sql += " VALUES('{0}','{1}',0,'01','{2}','{3}','admin.gif')";

                sql = string.Format(sql, SystemConfig.SysNo, SystemConfig.SysName, url, pMenuNo);
                sql = sql.Replace("//", "/");
                DBAccess.RunSQL(sql);
            }
            else
            {
                pMenuNo = dt.Rows[0]["RefMenuNo"].ToString();
            }
            #endregion

            try
            {
                sql = "SELECT * FROM GPM_Menu WHERE Flag='FlowSort" + this.No + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count >= 1)
                    DBAccess.RunSQL("DELETE FROM GPM_Menu WHERE Flag='FlowSort" + this.No + "' AND FK_App='" + SystemConfig.SysNo + "' ");
            }
            catch
            {
            }

            // 组织数据。
            // 获取他的ParentNo
            string parentNo = "";//this.ParentNo
            if (!string.IsNullOrEmpty(this.ParentNo))
            {
                sql = "SELECT * FROM GPM_Menu WHERE Flag='FlowSort" + this.ParentNo + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count >= 1)
                {
                    pMenuNo = dt.Rows[0]["No"].ToString();
                }
            }

            string menuNo = DBAccess.GenerOID("BP.GPM.Menu").ToString();
            // 执行插入.
            sql = "INSERT INTO GPM_Menu(No,Name,ParentNo,IsDir,MenuType,FK_App,IsEnable,Flag)";
            sql += " VALUES('{0}','{1}','{2}',{3},{4},'{5}',{6},'{7}')";
            sql = string.Format(sql, menuNo, this.Name, pMenuNo, 1, 3, SystemConfig.SysNo, 1, "FlowSort" + this.No);
            DBAccess.RunSQL(sql);
        }

        protected override bool beforeInsert()
        {
            this.WritToGPM();
            return base.beforeInsert();
        }

        protected override bool beforeDelete()
        {
            try
            {
                //删除权限管理
                if (BP.WF.Glo.OSModel == OSModel.BPM)
                    DBAccess.RunSQL("DELETE FROM GPM_Menu WHERE Flag='FlowSort" + this.No + "' AND FK_App='" + SystemConfig.SysNo + "'");
            }
            catch
            {
            }
            return base.beforeDelete();
        }

        protected override bool beforeUpdate()
        {
            //修改权限管理
            if (BP.WF.Glo.OSModel == OSModel.BPM)
            {
                DBAccess.RunSQL("UPDATE  GPM_Menu SET Name='" + this.Name + "' WHERE Flag='FlowSort" + this.No + "' AND FK_App='" + SystemConfig.SysNo + "'");
            }
            return base.beforeUpdate();
        }
        #endregion 重写方法
    }
    /// <summary>
    /// 流程类别
    /// </summary>
    public class FlowSorts : EntitiesTree
    {
        /// <summary>
        /// 流程类别s
        /// </summary>
        public FlowSorts() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FlowSort();
            }

        }
        /// <summary>
        /// 流程类别s
        /// </summary>
        /// <param name="no">ss</param>
        /// <param name="name">anme</param>
        public void AddByNoName(string no, string name)
        {
            FlowSort en = new FlowSort();
            en.No = no;
            en.Name = name;
            this.AddEntity(en);
        }
        public override int RetrieveAll()
        {
            int i = base.RetrieveAll();
            if (i == 0)
            {
                FlowSort fs = new FlowSort();
                fs.Name = "公文类";
                fs.No = "01";
                fs.Insert();

                fs = new FlowSort();
                fs.Name = "办公类";
                fs.No = "02";
                fs.Insert();
                i = base.RetrieveAll();
            }

            return i;
        }
    }
}
