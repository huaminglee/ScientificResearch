using System;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.En;
using BP.Sys;
namespace BP.WF.DTS
{
    /// <summary>
    /// �������ɱ���
    /// </summary>
    public class GenerTitle : Method
    {
        /// <summary>
        /// �������ɱ���
        /// </summary>
        public GenerTitle()
        {
            this.Title = "�������ɱ��⣨Ϊ���е����̣������µĹ����������̱��⣩";
            this.Help = "��Ҳ���Դ���������һ�����ĵ���ִ�С�";
        }
        /// <summary>
        /// ����ִ�б���
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
            //this.Warning = "��ȷ��Ҫִ����";
            //HisAttrs.AddTBString("P1", null, "ԭ����", true, false, 0, 10, 10);
            //HisAttrs.AddTBString("P2", null, "������", true, false, 0, 10, 10);
            //HisAttrs.AddTBString("P3", null, "ȷ��", true, false, 0, 10, 10);
        }
        /// <summary>
        /// ��ǰ�Ĳ���Ա�Ƿ����ִ���������
        /// </summary>
        public override bool IsCanDo
        {
            get
            {
                if (BP.Web.WebUser.No == "admin")
                    return true;
                return false;
            }
        }
        /// <summary>
        /// ִ��
        /// </summary>
        /// <returns>����ִ�н��</returns>
        public override object Do()
        {
            BP.WF.Template.Ext.FlowSheets ens = new BP.WF.Template.Ext.FlowSheets();
            foreach (BP.WF.Template.Ext.FlowSheet en in ens)
            {
                en.DoGenerTitle();
            }
            return "ִ�гɹ�...";
        }
    }
}
