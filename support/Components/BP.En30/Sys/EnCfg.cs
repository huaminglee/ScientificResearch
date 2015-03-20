using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP;
namespace BP.Sys
{
	/// <summary>
	///  ������Ϣ
	/// </summary>
    public class EnCfgAttr : EntityNoAttr
    {
        /// <summary>
        /// �����ǩ
        /// </summary>
        public const string GroupTitle = "GroupTitle";
        /// <summary>
        /// ����·��
        /// </summary>
        public const string FJSavePath = "FJSavePath";
        /// <summary>
        /// ����·��
        /// </summary>
        public const string FJWebPath = "FJWebPath";
        /// <summary>
        /// ���ݷ�����ʽ
        /// </summary>
        public const string Datan = "Datan";
    }
	/// <summary>
	/// EnCfgs
	/// </summary>
    public class EnCfg : EntityNo
    {
        #region ��������
        /// <summary>
        /// ���ݷ�����ʽ
        /// </summary>
        public string Datan
        {
            get
            {
                return this.GetValStringByKey(EnCfgAttr.Datan);
            }
            set
            {
                this.SetValByKey(EnCfgAttr.Datan, value);
            }
        }
        /// <summary>
        /// ����Դ
        /// </summary>
        public string GroupTitle
        {
            get
            {
                return this.GetValStringByKey(EnCfgAttr.GroupTitle);
            }
            set
            {
                this.SetValByKey(EnCfgAttr.GroupTitle, value);
            }
        }
        /// <summary>
        /// ����·��
        /// </summary>
        public string FJSavePath
        {
            get
            {
                string str = this.GetValStringByKey(EnCfgAttr.FJSavePath);
                if (str == "" || str == null || str==string.Empty)
                    return BP.Sys.SystemConfig.PathOfDataUser + this.No + "\\";
                return str;
            }
            set
            {
                this.SetValByKey(EnCfgAttr.FJSavePath, value);
            }
        }
        public string FJWebPath
        {
            get
            {
                string str = this.GetValStringByKey(EnCfgAttr.FJWebPath);
                if (str == "" || str == null)
                    return BP.Sys.Glo.Request.ApplicationPath +"/DataUser/" + this.No + "/";
                return str;
            }
            set
            {
                this.SetValByKey(EnCfgAttr.FJWebPath, value);
            }
        }
        #endregion

        #region ���췽��
        /// <summary>
        /// ϵͳʵ��
        /// </summary>
        public EnCfg()
        {
        }
        /// <summary>
        /// ϵͳʵ��
        /// </summary>
        /// <param name="no"></param>
        public EnCfg(string enName)
        {
            this.No = enName;
            try
            {
                this.Retrieve();
            }
            catch (Exception ex)
            {
            }
        }
        /// <summary>
        /// map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Sys_EnCfg");
                map.DepositaryOfEntity = Depositary.Application;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "ʵ������";
                map.EnType = EnType.Sys;

                map.AddTBStringPK(EnCfgAttr.No, null, "ʵ������", true, false, 1, 100, 60);
                map.AddTBString(EnCfgAttr.GroupTitle, null, "�����ǩ", true, false, 0, 2000, 60);
                map.AddTBString(EnCfgAttr.FJSavePath, null, "����·��", true, false, 0, 100, 60);
                map.AddTBString(EnCfgAttr.FJWebPath, null, "����Web·��", true, false, 0, 100, 60);
                map.AddTBString(EnCfgAttr.Datan, null, "�ֶ����ݷ�����ʽ", true, false, 0, 200, 60);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
	/// ʵ�弯��
	/// </summary>
    public class EnCfgs : EntitiesNoName
    {
        #region ����
        /// <summary>
        /// ������Ϣ
        /// </summary>
        public EnCfgs()
        {
        }
        /// <summary>
        /// �õ����� Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new EnCfg();
            }
        }
        #endregion
    }
}
