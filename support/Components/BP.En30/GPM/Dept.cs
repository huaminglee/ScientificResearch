using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Web;

namespace BP.GPM
{
    /// <summary>
    /// 部门属性
    /// </summary>
    public class DeptAttr : EntityTreeAttr
    {
        /// <summary>
        /// 部门类型
        /// </summary>
        public const string FK_DeptType = "FK_DeptType";
        /// <summary>
        /// 部门负责人
        /// </summary>
        public const string Leader = "Leader";
        /// <summary>
        /// 联系电话
        /// </summary>
        public const string Tel = "Tel";
        /// <summary>
        /// 单位全名
        /// </summary>
        public const string NameOfPath = "NameOfPath";
    }
    /// <summary>
    /// 部门
    /// </summary>
    public class Dept : EntityTree
    {
        #region 属性
        /// <summary>
        /// 全名
        /// </summary>
        public  string NameOfPath
        {
            get
            {
                return this.GetValStrByKey(DeptAttr.NameOfPath);
            }
            set
            {
                this.SetValByKey(DeptAttr.NameOfPath, value);
            }
        }
        /// <summary>
        /// 父节点的ID
        /// </summary>
        public new string ParentNo
        {
            get
            {
                return this.GetValStrByKey(DeptAttr.ParentNo);
            }
            set
            {
                this.SetValByKey(DeptAttr.ParentNo, value);
            }
        }
        /// <summary>
        /// 部门类型
        /// </summary>
        public string FK_DeptType
        {
            get
            {
                return this.GetValStrByKey(DeptAttr.FK_DeptType);
            }
            set
            {
                this.SetValByKey(DeptAttr.FK_DeptType, value);
            }
        }
        /// <summary>
        /// 部门类型名称
        /// </summary>
        public string FK_DeptTypeText
        {
            get
            {
                return this.GetValRefTextByKey(DeptAttr.FK_DeptType);
            }
        }
        private Depts _HisSubDepts = null;
        /// <summary>
        /// 它的子节点
        /// </summary>
        public Depts HisSubDepts
        {
            get
            {
                if (_HisSubDepts == null)
                    _HisSubDepts = new Depts(this.No);
                return _HisSubDepts;
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 部门
        /// </summary>
        public Dept() { }
        /// <summary>
        /// 部门
        /// </summary>
        /// <param name="no">编号</param>
        public Dept(string no) : base(no) { }
        #endregion

        #region 重写方法
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }
        /// <summary>
        /// Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map();
                map.EnDBUrl = new DBUrl(DBUrlType.AppCenterDSN); //连接到的那个数据库上. (默认的是: AppCenterDSN )
                map.PhysicsTable = "Port_Dept";
                map.EnType = EnType.Admin;

                map.EnDesc = "部门"; //  实体的描述.
                map.DepositaryOfEntity = Depositary.Application; //实体map的存放位置.
                map.DepositaryOfMap = Depositary.Application;    // Map 的存放位置.

                map.AddTBStringPK(DeptAttr.No, null, "编号", true, true, 1, 50, 20);

                //比如xx分公司财务部
                map.AddTBString(DeptAttr.Name, null, "名称", true, false, 0, 100, 30);

                //比如:\\驰骋集团\\南方分公司\\财务部
                map.AddTBString(DeptAttr.NameOfPath, null, "部门路径", false, false, 0, 300, 30);

                map.AddTBString(DeptAttr.ParentNo, null, "父节点编号", false, false, 0, 100, 30);
                map.AddTBString(DeptAttr.TreeNo, null, "树编号", false, false, 0, 100, 30);
                map.AddTBString(DeptAttr.Leader, null, "领导", false, false, 0, 100, 30);

                //比如: 财务部，生产部，人力资源部.
                map.AddTBString(DeptAttr.Tel, null, "联系电话", false, false, 0, 100, 30);

                map.AddTBInt(DeptAttr.Idx, 0, "Idx", false, false);
                map.AddTBInt(DeptAttr.IsDir, 0, "是否是目录", false, false);

                map.AddDDLEntities(DeptAttr.FK_DeptType, null, "部门类型", new DeptTypes(), true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 生成部门全名称.
        /// </summary>
        public void GenerNameOfPath()
        {
            string name = this.Name;
            Dept dept = new Dept(this.ParentNo);
            while (true)
            {
                if (dept.IsRoot)
                    break;
                name = dept.Name + "\\\\" + name;
            }
            this.NameOfPath = name;
            this.DirectUpdate();
        }
    }
    /// <summary>
    ///得到集合
    /// </summary>
    public class Depts : EntitiesNoName
    {
        /// <summary>
        /// 得到一个新实体
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Dept();
            }
        }
        /// <summary>
        /// 部门集合
        /// </summary>
        public Depts()
        {

        }
        /// <summary>
        /// 部门集合
        /// </summary>
        /// <param name="parentNo">父部门No</param>
        public Depts(string parentNo)
        {
            this.Retrieve(DeptAttr.ParentNo, parentNo);
        }
    }
}
