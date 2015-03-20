using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.WF.Rpt
{
    /// <summary>
    /// 报表人员
    /// </summary>
    public class RptEmpAttr
    {
        #region 基本属性
        /// <summary>
        /// 报表ID
        /// </summary>
        public const string FK_Rpt = "FK_Rpt";
        /// <summary>
        /// 人员
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        #endregion
    }
    /// <summary>
    /// RptEmp 的摘要说明。
    /// </summary>
    public class RptEmp : Entity
    {

        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (BP.Web.WebUser.No == "admin")
                {
                    uac.IsView = true;
                    uac.IsDelete = true;
                    uac.IsInsert = true;
                    uac.IsUpdate = true;
                    uac.IsAdjunct = true;
                }
                return uac;
            }
        }

        #region 基本属性
        /// <summary>
        /// 报表ID
        /// </summary>
        public string FK_Rpt
        {
            get
            {
                return this.GetValStringByKey(RptEmpAttr.FK_Rpt);
            }
            set
            {
                SetValByKey(RptEmpAttr.FK_Rpt, value);
            }
        }
        public string FK_EmpT
        {
            get
            {
                return this.GetValRefTextByKey(RptEmpAttr.FK_Emp);
            }
        }
        /// <summary>
        ///人员
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(RptEmpAttr.FK_Emp);
            }
            set
            {
                SetValByKey(RptEmpAttr.FK_Emp, value);
            }
        }
        #endregion

        #region 扩展属性

        #endregion

        #region 构造函数
        /// <summary>
        /// 报表人员
        /// </summary> 
        public RptEmp() { }
        /// <summary>
        /// 报表人员对应
        /// </summary>
        /// <param name="_empoid">报表ID</param>
        /// <param name="wsNo">人员编号</param> 	
        public RptEmp(string _empoid, string wsNo)
        {
            this.FK_Rpt = _empoid;
            this.FK_Emp = wsNo;
            if (this.Retrieve() == 0)
                this.Insert();
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

                Map map = new Map("Sys_RptEmp");
                map.EnDesc = "报表人员对应信息";
                map.EnType = EnType.Dot2Dot;

                map.AddTBStringPK(RptEmpAttr.FK_Rpt, null, "报表", false, false, 1, 15, 1);
                map.AddDDLEntitiesPK(RptEmpAttr.FK_Emp, null, "人员", new Emps(), true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 报表人员 
    /// </summary>
    public class RptEmps : Entities
    {
        #region 构造
        /// <summary>
        /// 报表与人员集合
        /// </summary>
        public RptEmps() { }
        #endregion

        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new RptEmp();
            }
        }
        #endregion
    }
}
