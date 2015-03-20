using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.Sys
{
	/// <summary>
	/// 用户自定义表
	/// </summary>
    public class SFTableAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 是否可以删除
        /// </summary>
        public const string IsDel = "IsDel";
        /// <summary>
        /// 字段
        /// </summary>
        public const string FK_Val = "FK_Val";
        /// <summary>
        /// 数据表描述
        /// </summary>
        public const string TableDesc = "TableDesc";
        /// <summary>
        /// 默认值
        /// </summary>
        public const string DefVal = "DefVal";
        /// <summary>
        /// 数据源
        /// </summary>
        public const string DBSrc = "DBSrc";
        /// <summary>
        /// 是否是树
        /// </summary>
        public const string IsTree = "IsTree";

        /// <summary>
        /// 字典表类型
        /// </summary>
	    public const string SFTableType = "SFTableType";

        #region 链接到其他系统获取数据的属性。
        /// <summary>
        /// 数据源
        /// </summary>
        public const string FK_SFDBSrc = "FK_SFDBSrc";
        /// <summary>
        /// 数据源表
        /// </summary>
        public const string SrcTable = "SrcTable";
        /// <summary>
        /// 显示的值
        /// </summary>
        public const string ColumnValue = "ColumnValue";
        /// <summary>
        /// 显示的文字
        /// </summary>
        public const string ColumnText = "ColumnText";
        /// <summary>
        /// 父结点值
        /// </summary>
        public const string ParentValue = "ParentValue";
        /// <summary>
        /// 查询语句
        /// </summary>
	    public const string SelectStatement = "SelectStatement";
        #endregion 链接到其他系统获取数据的属性。

    }

	/// <summary>
	/// 用户自定义表
	/// </summary>
    public class SFTable : EntityNoName
    {
        #region 链接到其他系统获取数据的属性
        /// <summary>
        /// 数据源
        /// </summary>
        public string FK_SFDBSrc
        {
            get
            {
                return this.GetValStringByKey(SFTableAttr.FK_SFDBSrc);
            }
            set
            {
                this.SetValByKey(SFTableAttr.FK_SFDBSrc, value);
            }
        }
        /// <summary>
        /// 物理表名称
        /// </summary>
        public string SrcTable
        {
            get
            {
                return this.GetValStringByKey(SFTableAttr.SrcTable);
            }
            set
            {
                this.SetValByKey(SFTableAttr.SrcTable, value);
            }
        }
        /// <summary>
        /// 值/主键字段名
        /// </summary>
        public string ColumnValue
        {
            get
            {
                return this.GetValStringByKey(SFTableAttr.ColumnValue);
            }
            set
            {
                this.SetValByKey(SFTableAttr.ColumnValue, value);
            }
        }
        /// <summary>
        /// 显示字段/显示字段名
        /// </summary>
        public string ColumnText
        {
            get
            {
                return this.GetValStringByKey(SFTableAttr.ColumnText);
            }
            set
            {
                this.SetValByKey(SFTableAttr.ColumnText, value);
            }
        }
        /// <summary>
        /// 父结点字段名
        /// </summary>
        public string ParentValue
        {
            get
            {
                return this.GetValStringByKey(SFTableAttr.ParentValue);
            }
            set
            {
                this.SetValByKey(SFTableAttr.ParentValue, value);
            }
        }

        /// <summary>
        /// 查询语句
        /// </summary>
        public string SelectStatement
        {
            get
            {
                return this.GetValStringByKey(SFTableAttr.SelectStatement);
            }
            set
            {
                this.SetValByKey(SFTableAttr.SelectStatement, value);
            }
        }
        #endregion

        #region 属性
        /// <summary>
        /// 是否是类
        /// </summary>
        public bool IsClass
        {
            get
            {
                if (this.No.Contains("."))
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// 是否是树形实体?
        /// </summary>
        public bool IsTree
        {
            get
            {
                return this.GetValBooleanByKey(SFTableAttr.IsTree);
            }
            set
            {
                this.SetValByKey(SFTableAttr.IsTree, value);
            }
        }

        /// <summary>
        /// 字典表类型
        /// <remarks>0：NoName类型</remarks>
        /// <remarks>1：NoNameTree类型</remarks>
        /// <remarks>2：NoName行政区划类型</remarks>
        /// </summary>
	    public int SFTableType
	    {
	        get
	        {
	            return this.GetValIntByKey(SFTableAttr.SFTableType);
	        }
            set
            {
                this.SetValByKey(SFTableAttr.SFTableType, value);
            }
	    }

        /// <summary>
        /// 值
        /// </summary>
        public string FK_Val
        {
            get
            {
                return this.GetValStringByKey(SFTableAttr.FK_Val);
            }
            set
            {
                this.SetValByKey(SFTableAttr.FK_Val, value);
            }
        }
        public string TableDesc
        {
            get
            {
                return this.GetValStringByKey(SFTableAttr.TableDesc);
            }
            set
            {
                this.SetValByKey(SFTableAttr.TableDesc, value);
            }
        }
        public string DefVal
        {
            get
            {
                return this.GetValStringByKey(SFTableAttr.DefVal);
            }
            set
            {
                this.SetValByKey(SFTableAttr.DefVal, value);
            }
        }
        public EntitiesNoName HisEns
        {
            get
            {
                if (this.IsClass)
                {
                    EntitiesNoName ens = (EntitiesNoName)BP.En.ClassFactory.GetEns(this.No);
                    ens.RetrieveAll();
                    return ens;
                }

                BP.En.GENoNames ges = new GENoNames(this.No, this.Name);
                ges.RetrieveAll();
                return ges;
            }
        }
        #endregion

        #region 构造方法
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                uac.IsInsert = false;
                return uac;
            }
        }
        /// <summary>
        /// 用户自定义表
        /// </summary>
        public SFTable()
        {
        }
        public SFTable(string mypk)
        {
            this.No = mypk;
            try
            {
                this.Retrieve();
            }
            catch (Exception ex)
            {
                switch (this.No)
                {
                    case "BP.Pub.NYs":
                        this.Name = "年月";
                      //  this.HisSFTableType = SFTableType.ClsLab;
                        this.FK_Val = "FK_NY";
                     //   this.IsEdit = true;
                        this.Insert();
                        break;
                    case "BP.Pub.YFs":
                        this.Name = "月";
                      //  this.HisSFTableType = SFTableType.ClsLab;
                        this.FK_Val = "FK_YF";
                       // this.IsEdit = true;
                        this.Insert();
                        break;
                    case "BP.Pub.Days":
                        this.Name = "天";
                     //   this.HisSFTableType = SFTableType.ClsLab;
                        this.FK_Val = "FK_Day";
                        //this.IsEdit = true;
                        this.Insert();
                        break;
                    case "BP.Pub.NDs":
                        this.Name = "年";
                     //   this.HisSFTableType = SFTableType.ClsLab;
                        this.FK_Val = "FK_ND";
                       // this.IsEdit = true;
                        this.Insert();
                        break;
                    default:
                        throw new Exception(ex.Message);
                }
            }
        }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Sys_SFTable");
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "字典表";
                map.EnType = EnType.Sys;

                map.AddTBStringPK(SFTableAttr.No, null, "表英文名称", true, false, 1, 20, 20);
                map.AddTBString(SFTableAttr.Name, null, "表中文名称", true, false, 0, 30, 20);
                map.AddTBString(SFTableAttr.FK_Val, null, "默认创建的字段名", true, false, 0, 50, 20);
                map.AddTBString(SFTableAttr.TableDesc, null, "表描述", true, false, 0, 50, 20);
                map.AddTBString(SFTableAttr.DefVal, null, "默认值", true, false, 0, 200, 20);
                map.AddBoolean(SFTableAttr.IsTree, false, "是否是树实体", true, true);
                map.AddDDLSysEnum(SFTableAttr.SFTableType, 0, "字典表类型", true, false, SFTableAttr.SFTableType, "@0=NoName类型@1=NoNameTree类型@2=NoName行政区划类型");

                //数据源.
                map.AddDDLEntities(SFTableAttr.FK_SFDBSrc, "local", "数据源", new BP.Sys.SFDBSrcs(), true);
                map.AddTBString(SFTableAttr.SrcTable, null, "数据源表", true, false, 0, 50, 20);
                map.AddTBString(SFTableAttr.ColumnValue, null, "显示的值(编号列)", true, false, 0, 50, 20);
                map.AddTBString(SFTableAttr.ColumnText, null, "显示的文字(名称列)", true, false, 0, 50, 20);
                map.AddTBString(SFTableAttr.ParentValue, null, "父级值(父级列)", true, false, 0, 50, 20);
                map.AddTBString(SFTableAttr.SelectStatement, null, "查询语句", true, false, 0, 1000, 600, true);

                //查找.
                map.AddSearchAttr(SFTableAttr.FK_SFDBSrc);

                RefMethod rm = new RefMethod();
                rm.Title = "编辑数据"; 
                rm.ClassMethodName = this.ToString() + ".DoEdit";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.IsForEns = false;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "创建Table向导";
                rm.ClassMethodName = this.ToString() + ".DoGuide";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.IsForEns = false;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "数据源管理";
                rm.ClassMethodName = this.ToString() + ".DoMangDBSrc";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.IsForEns = false;
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
        /// <summary>
        /// 数据源管理
        /// </summary>
        /// <returns></returns>
        public string DoMangDBSrc()
        {
            return "/WF/Comm/Search.aspx?EnsName=BP.Sys.SFDBSrcs";
        }
        /// <summary>
        /// 创建表向导
        /// </summary>
        /// <returns></returns>
        public string DoGuide()
        {
            return "/WF/Comm/Sys/SFGuide.aspx";
        }
        /// <summary>
        /// 编辑数据
        /// </summary>
        /// <returns></returns>
        public string DoEdit()
        {
            if (this.IsClass)
                return "/WF/Comm/Ens.aspx?EnsName=" + this.No;
            else
                return "/WF/MapDef/SFTableEditData.aspx?RefNo=" + this.No;
        }
        protected override bool beforeDelete()
        {
            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.UIBindKey, this.No);
            if (attrs.Count != 0)
            {
                string err = "";
                foreach (MapAttr item in attrs)
                    err += " @ " + item.MyPK + " " + item.Name ;
                throw new Exception("@如下实体字段在引用:"+err);
            }
            return base.beforeDelete();
        }
    }
	/// <summary>
	/// 用户自定义表s
	/// </summary>
    public class SFTables : EntitiesNoName
	{		
		#region 构造
        /// <summary>
        /// 用户自定义表s
        /// </summary>
		public SFTables()
		{
		}
		/// <summary>
		/// 得到它的 Entity
		/// </summary>
		public override Entity GetNewEntity 
		{
			get
			{
				return new SFTable();
			}
		}
		#endregion
	}
}
