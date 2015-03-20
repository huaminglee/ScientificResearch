using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.XML;
using BP.Sys;

namespace BP.WF.XML
{
    /// <summary>
    /// Ƥ��
    /// </summary>
	public class Skin:XmlEnNoName
	{
        public new string Name
        {
            get
            {
                return this.GetValStringByKey(BP.Web.WebUser.SysLang);
            }
        }
        public new string CSS
        {
            get
            {
                return this.GetValStringByKey("CSS");
            }
        }

		#region ����
		/// <summary>
		/// �ڵ���չ��Ϣ
		/// </summary>
		public Skin()
		{
		}
		/// <summary>
		/// ��ȡһ��ʵ��
		/// </summary>
		public override XmlEns GetNewEntities
		{
			get
			{
				return new Skins();
			}
		}
		#endregion
	}
	/// <summary>
    /// Ƥ��s
	/// </summary>
	public class Skins:XmlEns
	{
		#region ����
		/// <summary>
        /// Ƥ��s
		/// </summary>
        public Skins() { }
		#endregion

		#region ��д�������Ի򷽷���
		/// <summary>
		/// �õ����� Entity 
		/// </summary>
		public override XmlEn GetNewEntity
		{
			get
			{
				return new Skin();
			}
		}
		public override string File
		{
			get
			{
                return SystemConfig.PathOfWebApp + "\\WF\\Style\\Tools.xml";
			}
		}
		/// <summary>
		/// ��������
		/// </summary>
		public override string TableName
		{
			get
			{
				return "Skin";
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