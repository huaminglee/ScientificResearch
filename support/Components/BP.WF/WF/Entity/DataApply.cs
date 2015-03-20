using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Port; 
using BP.Port; 
using BP.En;

namespace BP.WF
{
	/// <summary>
	/// 追加时间申请
	/// </summary>
	public class DataApplyAttr 
	{
		#region 基本属性
        /// <summary>
        /// MyPK
        /// </summary>
        public const string MyPK = "MyPK";
		/// <summary>
		/// 工作ID
		/// </summary>
		public const  string WorkID="WorkID";
		/// <summary>
		/// 节点
		/// </summary>
		public const  string NodeId="NodeId";

        /// <summary>
        /// 申请人
        /// </summary>
        public const string Applyer = "Applyer";
        public const string ApplyData = "ApplyData";
        public const string ApplyDays = "ApplyDays";
        public const string ApplyNote1 = "ApplyNote1";
        public const string ApplyNote2 = "ApplyNote2";
		/// <summary>
		/// 审核人
		/// </summary>
        public const string Checker = "Checker";
        public const string CheckerData = "CheckerData";

        public const string CheckerDays = "CheckerDays";
        public const string CheckerNote1 = "CheckerNote1";
        public const string CheckerNote2 = "CheckerNote2";


        public const string RunState = "RunState";

		#endregion
	}
	/// <summary>
	/// 追加时间申请
	/// </summary>
	public class DataApply : Entity
	{		
		#region 基本属性
        /// <summary>
        /// MyPK
        /// </summary>
        public string MyPK
        {
            get
            {
                return this.GetValStrByKey(DataApplyAttr.MyPK);
            }
            set
            {
                this.SetValByKey(DataApplyAttr.MyPK, value);
            }
        }
		/// <summary>
		/// 工作ID
		/// </summary>
        public Int64 WorkID
		{
			get
			{
				return this.GetValInt64ByKey(DataApplyAttr.WorkID);
			}
			set
			{
				SetValByKey(DataApplyAttr.WorkID,value);
			}
		}
		/// <summary>
		/// NodeId
		/// </summary>
		public int  NodeId
		{
			get
			{
				return this.GetValIntByKey(DataApplyAttr.NodeId);
			}
			set
			{
				SetValByKey(DataApplyAttr.NodeId,value);
			}
		}
        public int ApplyDays
        {
            get
            {
                return this.GetValIntByKey(DataApplyAttr.ApplyDays);
            }
            set
            {
                SetValByKey(DataApplyAttr.ApplyDays, value);
            }
        }
        public int CheckerDays
        {
            get
            {
                return this.GetValIntByKey(DataApplyAttr.CheckerDays);
            }
            set
            {
                SetValByKey(DataApplyAttr.CheckerDays, value);
            }
        }
        public string Applyer
		{
			get
			{
				return this.GetValStringByKey(DataApplyAttr.Applyer);
			}
			set
			{
                SetValByKey(DataApplyAttr.Applyer, value);
			}
		}
        public string ApplyNote1
        {
            get
            {
                return this.GetValStringByKey(DataApplyAttr.ApplyNote1);
            }
            set
            {
                SetValByKey(DataApplyAttr.ApplyNote1, value);
            }
        }
        public string ApplyNote2
        {
            get
            {
                return this.GetValStringByKey(DataApplyAttr.ApplyNote2);
            }
            set
            {
                SetValByKey(DataApplyAttr.ApplyNote2, value);
            }
        }
        public string ApplyData
        {
            get
            {
                return this.GetValStringByKey(DataApplyAttr.ApplyData);
            }
            set
            {
                SetValByKey(DataApplyAttr.ApplyData, value);
            }
        }
        public string Checker
        {
            get
            {
                return this.GetValStringByKey(DataApplyAttr.Checker);
            }
            set
            {
                SetValByKey(DataApplyAttr.Checker, value);
            }
        }
        public int RunState
        {
            get
            {
                return this.GetValIntByKey(DataApplyAttr.RunState);
            }
            set
            {
                SetValByKey(DataApplyAttr.RunState, value);
            }
        }
		#endregion 

		#region 构造函数
		/// <summary>
		/// 追加时间申请
		/// </summary>
		public DataApply(){}

        public DataApply(int workid, int nodeid)
        {
            this.WorkID = workid;
            this.NodeId = nodeid;
            this.Retrieve();
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

                Map map = new Map("WF_DataApply");
                map.EnDesc = "追加时间申请";
                map.EnType = EnType.App;

                map.AddMyPK();

                map.AddTBInt(DataApplyAttr.WorkID, 0, "工作ID", true, true);
                map.AddTBInt(DataApplyAttr.NodeId, 0, "NodeId", true, true);
                map.AddTBInt(DataApplyAttr.RunState, 0, "运行状态0,没有提交，1，提交申请执行审批中，2，审核完毕。", true, true);


                map.AddTBInt(DataApplyAttr.ApplyDays, 0, "申请天数", true, true);
                map.AddTBDate(DataApplyAttr.ApplyData, "申请日期", true, true);
                map.AddDDLEntities(DataApplyAttr.Applyer, null, "申请人", new Emps(), false);
                map.AddTBStringDoc(DataApplyAttr.ApplyNote1, "", "申请原因", true, true);
                map.AddTBStringDoc(DataApplyAttr.ApplyNote2, "", "申请备注", true, true);



                map.AddDDLEntities(DataApplyAttr.Checker, null, "审批人", new Emps(), false);
                map.AddTBDate(DataApplyAttr.CheckerData, "审批日期", true, true);
                map.AddTBInt(DataApplyAttr.CheckerDays, 0, "批准天数", true, true);
                map.AddTBStringDoc(DataApplyAttr.CheckerNote1, "", "审批意见", true, true);
                map.AddTBStringDoc(DataApplyAttr.CheckerNote2, "", "审批备注", true, true);


                RefMethod rm = new RefMethod();
                rm.Title = "提出追加时限申请";
                rm.Warning = "您确定要向您的领导提出追加时限申请吗？";
                rm.ClassMethodName = this.ToString() + ".DoApply";
               // rm.ClassMethodName =   "BP.WF.DataApply.DoApply";


                Attrs attrs = new Attrs();
                attrs.AddTBInt(DataApplyAttr.ApplyDays, 0, "追加天数", true, false);
                attrs.AddDDLEntities(DataApplyAttr.Checker, null, "审批人", new Emps(), false);
                attrs.AddTBStringDoc(DataApplyAttr.ApplyNote1, "", "申请原因", true, false);
                attrs.AddTBStringDoc(DataApplyAttr.ApplyNote2, "", "申请备注", true, false);
                map.AddRefMethod(rm);


                //rm = new RefMethod();
                //rm.Title = "审批时限";

                //Attrs attrs = new Attrs();
                //attrs.AddTBInt(DataApplyAttr.CheckerDays, 0, "批准天数", true, false);
                //map.AddTBDate(DataApplyAttr.CheckerData, "审批日期", true, true);

                //attrs.AddDDLEntities(DataApplyAttr.Checker, null, "审批人", new Emps(), false);
                //attrs.AddTBStringDoc(DataApplyAttr.ApplyNote1, "", "申请原因", true, false);
                //attrs.AddTBStringDoc(DataApplyAttr.ApplyNote2, "", "申请备注", true, false);
                //map.AddRefMethod(rm);



                this._enMap = map;
                return this._enMap;
            }
        }
		#endregion	 

        #region 方法
        public string DoApply(int days, string checker, string note1, string note2)
        {
            this.ApplyDays = days;

            this.Checker = checker;
            this.ApplyNote1 = note1;
            this.ApplyNote2 = note2;
            this.ApplyData = DataType.CurrentData;


            this.RunState = 1; /*进入提交审核状态*/
            this.Update();

            return "执行成功，已经交给" + checker+"审核。";
             
        }
        public string DoCheck(int days, string checker, string note1, string note2)
        {
            this.ApplyDays = days;

            this.Checker = checker;
            this.ApplyNote1 = note1;
            this.ApplyNote2 = note2;

            // 调整当前预警与
            GenerWorkerLists wls = new GenerWorkerLists(this.WorkID, this.NodeId);
            wls.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FK_Node, this.NodeId);
            foreach (GenerWorkerList wl in wls)
            {
               // wl.DTOfWarning = DataType.AddDays(wl.DTOfWarning, days);
               // wl.SDT = DataType.AddDays(wl.SDT, days);
                wl.DirectUpdate();
            }

            this.RunState = 2; /*进入提交审核状态*/
            this.Update();

            return "执行成功，已经交给" + checker + "审核。";
        }
        #endregion
    }
	/// <summary>
	/// 追加时间申请s 
	/// </summary>
	public class DataApplys : Entities
	{	 
		#region 构造
		/// <summary>
		/// 追加时间申请s
		/// </summary>
		public DataApplys()
		{

		}
		/// <summary>
		/// 得到它的 Entity
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new DataApply();
			}
		}
		#endregion
	}
	
}
