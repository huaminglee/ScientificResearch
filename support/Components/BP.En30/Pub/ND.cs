using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.Pub
{
	/// <summary>
	/// 年度
	/// </summary>
	public class ND :SimpleNoNameFix
	{
		#region 实现基本的方方法
		/// <summary>
		/// 物理表
		/// </summary>
		public override string  PhysicsTable
		{
			get
			{
				return "Pub_ND";
			}
		}
		/// <summary>
		/// 描述
		/// </summary>
		public override string  Desc
		{
			get
			{
                return "年度";// "年度";
			}
		}
		#endregion 

		#region 构造方法
		public ND()
		{
		}
		public ND(string _No ):base(_No)
		{
		}
		#endregion 
	}
	/// <summary>
	/// NDs
	/// </summary>
	public class NDs :SimpleNoNameFixs
	{
        protected override void OnClear()
        {
            if (1 == 1)
            {

            }
            base.OnClear();
        }
		/// <summary>
		/// 年度集合
		/// </summary>
		public NDs()
		{
		}
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new ND();
			}
		}
        public override int RetrieveAll()
        {
            return base.RetrieveAll();
        }
	}
}
