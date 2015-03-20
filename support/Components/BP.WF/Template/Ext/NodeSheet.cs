using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Sys;
using System.Collections;
using BP.Port;

namespace BP.WF.Template.Ext
{
    /// <summary>
    /// ������ÿ���ڵ����Ϣ.
    /// </summary>
    public class NodeSheet : Entity
    {
        #region Index
        /// <summary>
        /// ��ȡ�ڵ�İ�����Ϣurl
        /// <para></para>
        /// <para>added by liuxc,2014-8-19</para> 
        /// </summary>
        /// <param name="sysNo">������վ������ϵͳNo</param>
        /// <param name="searchTitle">�����������</param>
        /// <returns></returns>
        private string this[string sysNo, string searchTitle]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(sysNo) || string.IsNullOrWhiteSpace(searchTitle))
                    return "javascript:alert('�˴���û�а�����Ϣ��')";

                return string.Format("http://online.ccflow.org/KM/Tree.aspx?no={0}&st={1}", sysNo, Uri.EscapeDataString(searchTitle));
            }
        }
        #endregion

        #region Const
        /// <summary>
        /// CCFlow��������
        /// </summary>
        private const string SYS_CCFLOW = "001";
        /// <summary>
        /// CCForm��������
        /// </summary>
        private const string SYS_CCFORM = "002";
        #endregion

        #region ����.
        ///// <summary>
        ///// �ڵ���
        ///// </summary>
        //public string NodeMark
        //{
        //    get
        //    {
        //        return this.GetValStrByKey(NodeAttr.NodeMark);
        //    }
        //}

        /// <summary>
        /// ��ʱ������ʽ
        /// </summary>
        public OutTimeDeal HisOutTimeDeal
        {
            get
            {
                return (OutTimeDeal)this.GetValIntByKey(NodeAttr.OutTimeDeal);
            }
            set
            {
                this.SetValByKey(NodeAttr.OutTimeDeal, (int)value);
            }
        }
        /// <summary>
        /// ���ʹ���
        /// </summary>
        public ReturnRole HisReturnRole
        {
            get
            {
                return (ReturnRole)this.GetValIntByKey(NodeAttr.ReturnRole);
            }
            set
            {
                this.SetValByKey(NodeAttr.ReturnRole, (int)value);
            }
        }

        /// <summary>
        /// ���ʹ���
        /// </summary>
        public DeliveryWay HisDeliveryWay
        {
            get
            {
                return (DeliveryWay)this.GetValIntByKey(NodeAttr.DeliveryWay);
            }
            set
            {
                this.SetValByKey(NodeAttr.DeliveryWay, (int)value);
            }
        }
        public int Step
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.Step);
            }
            set
            {
                this.SetValByKey(NodeAttr.Step, value);
            }
        }
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.NodeID);
            }
            set
            {
                this.SetValByKey(NodeAttr.NodeID, value);
            }
        }
        /// <summary>
        /// ��ʱ��������
        /// </summary>
        public string DoOutTime
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.DoOutTime);
            }
            set
            {
                this.SetValByKey(NodeAttr.DoOutTime, value);
            }
        }
        /// <summary>
        /// ��ʱ��������
        /// </summary>
        public string DoOutTimeCond
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.DoOutTimeCond);
            }
            set
            {
                this.SetValByKey(NodeAttr.DoOutTimeCond, value);
            }
        }
        public string Name
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.Name);
            }
            set
            {
                this.SetValByKey(NodeAttr.Name, value);
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(NodeAttr.FK_Flow, value);
            }
        }
        public string FlowName
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.FlowName);
            }
            set
            {
                this.SetValByKey(NodeAttr.FlowName, value);
            }
        }
        /// <summary>
        /// ������sql
        /// </summary>
        public string DeliveryParas
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.DeliveryParas);
            }
            set
            {
                this.SetValByKey(NodeAttr.DeliveryParas, value);
            }
        }
        /// <summary>
        /// �Ƿ�����˻�
        /// </summary>
        public bool ReturnEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.ReturnRole);
            }
        }

        public override string PK
        {
            get
            {
                return "NodeID";
            }
        }
        #endregion ����.

        #region ���Ի�ȫ�ֵ� Node
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                Flow fl = new Flow(this.FK_Flow);
                if (BP.Web.WebUser.No == "admin")
                    uac.IsUpdate = true;
                return uac;
            }
        }
        #endregion

        #region ���캯��
        /// <summary>
        /// �ڵ�
        /// </summary>
        public NodeSheet() { }
        /// <summary>
        /// ��д���෽��
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map();
                //map �Ļ�����Ϣ.
                map.PhysicsTable = "WF_Node";
                map.EnDesc = "�ڵ�";
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;

                #region  ��������
                map.AddTBIntPK(NodeAttr.NodeID, 0, "�ڵ�ID", true, true);
                map.AddTBInt(NodeAttr.Step, 0, "����(�޼�������)", true, false);
                map.SetHelperAlert(NodeAttr.Step, "�����ڽڵ��������ȷ�����ò���������������׶�д."); //ʹ��alert�ķ�ʽ��ʾ������Ϣ.
                map.AddTBString(NodeAttr.FK_Flow, null, "���̱��", false, false, 3, 3, 10, false);

                map.AddTBString(NodeAttr.Name, null, "����", true, true, 0, 100, 10, true);

                string str = "";
                str += "@0=01.����ǰ����Ա������֯�ṹ�𼶲��Ҹ�λ";
                str += "@1=02.���ڵ�󶨵Ĳ��ż���";
                str += "@2=03.�����õ�SQL��ȡ�����˼���";
                str += "@3=04.���ڵ�󶨵���Ա����";
                str += "@4=05.����һ�ڵ㷢����ͨ������Աѡ������ѡ�������";
                str += "@5=06.����һ�ڵ����ָ�����ֶ�ֵ��Ϊ������Ľ�����";
                str += "@6=07.����һ�ڵ㴦����Ա��ͬ";
                str += "@7=08.�뿪ʼ�ڵ㴦������ͬ";
                str += "@8=09.��ָ���ڵ㴦������ͬ";
                str += "@9=10.���󶨵ĸ�λ�벿�Ž�������";
                str += "@10=11.���󶨵ĸ�λ���㲢���԰󶨵Ĳ��ż���Ϊγ��";
                str += "@11=12.��ָ���ڵ����Ա��λ����";
                str += "@12=13.��SQLȷ�����߳̽�����������Դ";
                str += "@13=14.����һ�ڵ����ϸ�����������̵߳Ľ�����";
                str += "@14=15.�����󶨵ĸ�λ����";
                str += "@15=16.��FEE������";

                str += "@100=16.��ccflow��BPMģʽ����";
                map.AddDDLSysEnum(NodeAttr.DeliveryWay, 0, "�ڵ���ʹ���", true, true, NodeAttr.DeliveryWay,str);

                map.AddTBString(NodeAttr.DeliveryParas, null, "���ʹ�����������", true, false, 0, 500, 10, true);
                map.AddDDLSysEnum(NodeAttr.WhoExeIt, 0, "˭ִ����",true, true, NodeAttr.WhoExeIt, "@0=����Աִ��@1=����ִ��@2=���ִ��");
                map.AddDDLSysEnum(NodeAttr.TurnToDeal, 0, "���ͺ�ת��",
                 true, true, NodeAttr.TurnToDeal, "@0=��ʾccflowĬ����Ϣ@1=��ʾָ����Ϣ@2=ת��ָ����url@3=��������ת��");
                map.AddTBString(NodeAttr.TurnToDealDoc, null, "ת��������", true, false, 0, 1000, 10, true);
                map.AddDDLSysEnum(NodeAttr.ReadReceipts, 0, "�Ѷ���ִ", true, true, NodeAttr.ReadReceipts,
                    "@0=����ִ@1=�Զ���ִ@2=����һ�ڵ�����ֶξ���@3=��SDK�����߲�������");
                map.SetHelperUrl(NodeAttr.ReadReceipts, this[SYS_CCFLOW, "�Ѷ���ִ"]);

                map.AddDDLSysEnum(NodeAttr.CondModel, 0, "�����������ƹ���", true, true, NodeAttr.CondModel,
                 "@0=����������������@1=���û��ֹ�ѡ��");
                map.SetHelperUrl(NodeAttr.CondModel, this[SYS_CCFLOW, "�����������ƹ���"]); //���Ӱ���

                // ��������.
                map.AddDDLSysEnum(NodeAttr.CancelRole,(int)CancelRole.OnlyNextStep, "��������", true, true,
                    NodeAttr.CancelRole,"@0=��һ�����Գ���@1=���ܳ���@2=��һ���뿪ʼ�ڵ���Գ���@3=ָ���Ľڵ���Գ���");

                // �ڵ㹤��������. edit by peng, 2014-01-24.
                map.AddDDLSysEnum(NodeAttr.BatchRole, (int)BatchRole.None, "����������", true, true, NodeAttr.BatchRole, "@0=������������@1=�������@2=�����������");
                map.AddTBInt(NodeAttr.BatchListCount, 12, "����������", true, false);
                map.SetHelperUrl(NodeAttr.BatchRole, this[SYS_CCFLOW, "�ڵ㹤��������"]); //���Ӱ���
                map.AddTBString(NodeAttr.BatchParas, null, "����������", true, false, 0, 300, 10, true);


                map.AddBoolean(NodeAttr.IsTask, true, "�������乤����?", true, true, false);
                map.SetHelperBaidu(NodeAttr.IsTask); //���ӵ�baidu����.
                map.AddBoolean(NodeAttr.IsRM, true, "�Ƿ�����Ͷ��·���Զ����书��?", true, true, false);
                map.SetHelperBaidu(NodeAttr.IsRM); //���ӵ�baidu����.

                map.AddTBDateTime("DTFrom", "�������ڴ�", true, true);
                map.AddTBDateTime("DTTo", "�������ڵ�", true, true);
                #endregion  ��������

                #region ����.
                map.AddDDLSysEnum(NodeAttr.FormType, (int)NodeFormType.FreeForm, "�ڵ��������", true, true,
                 "NodeFormType", "@0=ɵ�ϱ���(ccflow6ȡ��֧��)@1=���ɱ���@2=�Զ������@3=SDK����@4=SL����(ccflow6ȡ��֧��)@5=������@6=��̬������@7=���ı���(WebOffice)@8=Excel����(������)@9=Word����(������)@100=����(�Զ����������Ч)");
                map.AddTBString(NodeAttr.FormUrl, null, "����URL", true, false, 0, 200, 10, true);

                map.AddTBString(NodeAttr.FocusField, null, "�����ֶ�", true, false, 0, 50, 10, true);
                map.SetHelperBaidu(NodeAttr.FocusField); //���ӵ�baidu����.

                map.AddTBString(NodeAttr.NodeFrmID, null, "�ڵ����ID", true, false, 0, 50, 10);
                map.AddDDLSysEnum(NodeAttr.SaveModel, 0, "���淽ʽ", true, true);
                #endregion ����.

                #region �ֺ������߳�����
                map.AddDDLSysEnum(NodeAttr.RunModel, 0, "����ģʽ",
                    true, true, NodeAttr.RunModel, "@0=��ͨ@1=����@2=����@3=�ֺ���@4=���߳�");
                map.SetHelperUrl(NodeAttr.RunModel, this[SYS_CCFLOW, "����ģʽ"]); //���Ӱ���.
        
                
                //���߳�����.
                map.AddDDLSysEnum(NodeAttr.SubThreadType, 0, "���߳�����", true, true, NodeAttr.SubThreadType, "@0=ͬ����@1=�����");
                map.SetHelperUrl(NodeAttr.SubThreadType, this[SYS_CCFLOW, "���߳�����"]); //���Ӱ���


                map.AddTBDecimal(NodeAttr.PassRate, 0, "���ͨ����", true, false);
                map.SetHelperUrl(NodeAttr.PassRate, this[SYS_CCFLOW, "���ͨ����"]); //���Ӱ���.

                // �������̲߳��� 2013-01-04
                map.AddDDLSysEnum(NodeAttr.SubFlowStartWay, (int)SubFlowStartWay.None, "���߳�������ʽ", true, true,
                    NodeAttr.SubFlowStartWay, "@0=������@1=ָ�����ֶ�����@2=����ϸ������");
                map.AddTBString(NodeAttr.SubFlowStartParas, null, "��������", true, false, 0, 100, 10, true);
                map.SetHelperUrl(NodeAttr.SubFlowStartWay, this[SYS_CCFLOW, "���߳�������ʽ"]); //���Ӱ���

                //���촦��ģʽ.
                map.AddDDLSysEnum(NodeAttr.TodolistModel, (int)TodolistModel.QiangBan, "���촦��ģʽ", true, true, NodeAttr.TodolistModel,
                    "@0=����ģʽ@1=Э��ģʽ@2=����ģʽ@3=����ģʽ");
                map.SetHelperUrl(NodeAttr.TodolistModel, this[SYS_CCFLOW, "���촦��ģʽ"]); //���Ӱ���.


                //��������ģʽ.
                map.AddDDLSysEnum(NodeAttr.BlockModel, (int)BlockModel.None, "��������ģʽ", true, true, NodeAttr.BlockModel,
                    "@0=������@1=��ǰ�ڵ������δ��ɵ�������@2=��Լ����ʽ����δ���������@3=����SQL����@4=���ձ���ʽ����");
                map.SetHelperUrl(NodeAttr.BlockModel, this[SYS_CCFLOW, "���촦��ģʽ"]); //���Ӱ���.

                map.AddTBString(NodeAttr.BlockExp, null, "��������ʽ", true, false, 0, 700, 10,true);
                map.SetHelperAlert(NodeAttr.BlockExp,"��������Ϸ�������ģʽ����������");

                map.AddTBString(NodeAttr.BlockAlert, null, "������ʱ��ʾ��Ϣ", true, false, 0, 700, 10, true);
                map.SetHelperAlert(NodeAttr.BlockAlert, "��������Ϸ�������ģʽ����������,��д֧��cc����ʽ.");

                //map.AddBoolean(NodeAttr.IsCheckSubFlowOver, false, "(��ǰ�ڵ�����������ʱ)�Ƿ������������̽�����,�ýڵ�������·���?",
               //true, true, true);

                ////  add 2013-09-14 
                //map.AddBoolean(NodeAttr.IsEnableTaskPool, true,
                //    "�Ƿ����ù��������(��web.config�е�IsEnableTaskPool�������ò���Ч,�����߳��޹�)��", true, true, true);
                //map.SetHelperBaidu(NodeAttr.IsEnableTaskPool); //���Ӱ���.
              

                map.AddBoolean(NodeAttr.IsAllowRepeatEmps, false, "�Ƿ��������߳̽�����Ա�ظ�(���������������̷߳���ʱ��Ч)?", true, true, true);
                map.AddBoolean(NodeAttr.IsGuestNode, false, "�Ƿ��ǿͻ�ִ�нڵ�(����֯�ṹ��Ա���봦�������Ľڵ�)?", true, true, true);
                #endregion �ֺ������߳�����

                #region �Զ���ת����
                map.AddBoolean(NodeAttr.AutoJumpRole0, false, "�����˾��Ƿ�����", true, true, false);
                map.SetHelperUrl(NodeAttr.AutoJumpRole0, this[SYS_CCFLOW, "�Զ���ת����"]); //���Ӱ���

                map.AddBoolean(NodeAttr.AutoJumpRole1, false, "�������Ѿ����ֹ�", true, true, false);
                map.AddBoolean(NodeAttr.AutoJumpRole2, false, "����������һ����ͬ", true, true, false);
                map.AddDDLSysEnum(NodeAttr.WhenNoWorker, 0, "�Ҳ��������˴�������",
       true, true, NodeAttr.WhenNoWorker, "@0=��ʾ����@1=�Զ�ת����һ��");
                #endregion

                #region  ���ܰ�ť״̬
                map.AddTBString(BtnAttr.SendLab, "����", "���Ͱ�ť��ǩ", true, false, 0, 50, 10);
                map.AddTBString(BtnAttr.SendJS, "", "��ťJS����", true, false, 0, 50, 10);
                //map.SetHelperBaidu(BtnAttr.SendJS, "ccflow ����ǰ�����������ж�"); //���Ӱ���.
                map.SetHelperUrl(BtnAttr.SendJS, this[SYS_CCFLOW, "��ťJS����"]);

                map.AddTBString(BtnAttr.SaveLab, "����", "���水ť��ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.SaveEnable, true, "�Ƿ�����", true, true);
                map.SetHelperUrl(BtnAttr.SaveLab, this[SYS_CCFLOW, "����"]); //���Ӱ���

                map.AddTBString(BtnAttr.ThreadLab, "���߳�", "���̰߳�ť��ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ThreadEnable, false, "�Ƿ�����", true, true);
                map.SetHelperUrl(BtnAttr.ThreadLab, this[SYS_CCFLOW, "���̰߳�ť��ǩ"]); //���Ӱ���


                map.AddDDLSysEnum(NodeAttr.ThreadKillRole, (int)ThreadKillRole.None, "���߳�ɾ����ʽ", true, true,
           NodeAttr.ThreadKillRole, "@0=����ɾ��@1=�ֹ�ɾ��@2=�Զ�ɾ��",true);
                map.SetHelperUrl(NodeAttr.ThreadKillRole, this[SYS_CCFLOW, "���߳�ɾ����ʽ"]); //���Ӱ���
               

                map.AddTBString(BtnAttr.SubFlowLab, "������", "�����̰�ť��ǩ", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.SubFlowCtrlRole, 0, "���ƹ���", true, true, BtnAttr.SubFlowCtrlRole, "@0=��@1=������ɾ��������@2=����ɾ��������");

                map.AddTBString(BtnAttr.JumpWayLab, "��ת", "��ת��ť��ǩ", true, false, 0, 50, 10);
                map.AddDDLSysEnum(NodeAttr.JumpWay, 0, "��ת����", true, true, NodeAttr.JumpWay);
                map.AddTBString(NodeAttr.JumpToNodes, null, "����ת�Ľڵ�", true, false, 0, 200, 10, true);
                map.SetHelperUrl(NodeAttr.JumpWay, this[SYS_CCFLOW, "��ת����"]); //���Ӱ���.

                map.AddTBString(BtnAttr.ReturnLab, "�˻�", "�˻ذ�ť��ǩ", true, false, 0, 50, 10);
                map.AddDDLSysEnum(NodeAttr.ReturnRole, 0,"�˻ع���",true, true, NodeAttr.ReturnRole);
              //  map.AddTBString(NodeAttr.ReturnToNodes, null, "���˻ؽڵ�", true, false, 0, 200, 10, true);
                map.SetHelperUrl(NodeAttr.ReturnRole, this[SYS_CCFLOW, "222"]); //���Ӱ���.

                map.AddBoolean(NodeAttr.IsBackTracking, false, "�Ƿ����ԭ·����(�����˻ع��ܲ���Ч)", true, true, false);
                map.AddTBString(BtnAttr.ReturnField, "", "�˻���Ϣ��д�ֶ�", true, false, 0, 50, 10);
                map.SetHelperUrl(NodeAttr.IsBackTracking, this[SYS_CCFLOW, "�Ƿ����ԭ·����"]); //���Ӱ���.

                map.AddTBString(BtnAttr.CCLab, "����", "���Ͱ�ť��ǩ", true, false, 0, 50, 10);
                map.AddDDLSysEnum(NodeAttr.CCRole, 0, "���͹���", true, true, NodeAttr.CCRole);
                map.SetHelperUrl(NodeAttr.CCRole, this[SYS_CCFLOW, "���͹���"]); //���Ӱ���.

                // add 2014-04-05.
                map.AddDDLSysEnum(NodeAttr.CCWriteTo, 0, "����д�����",
             true, true, NodeAttr.CCWriteTo, "@0=д�볭���б�@1=д�����@2=д������볭���б�", true);
                map.SetHelperUrl(NodeAttr.CCWriteTo, this[SYS_CCFLOW, "����д�����"]); //���Ӱ���

                map.AddTBString(BtnAttr.ShiftLab, "�ƽ�", "�ƽ���ť��ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ShiftEnable, false, "�Ƿ�����", true, true);
                map.SetHelperUrl(BtnAttr.ShiftLab, this[SYS_CCFLOW, "�ƽ�"]); //���Ӱ���.note:none

                map.AddTBString(BtnAttr.DelLab, "ɾ��", "ɾ����ť��ǩ", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.DelEnable, 0, "ɾ������", true, true, BtnAttr.DelEnable);
                map.SetHelperUrl(BtnAttr.DelLab, this[SYS_CCFLOW, "ɾ��"]); //���Ӱ���.

                map.AddTBString(BtnAttr.EndFlowLab, "��������", "�������̰�ť��ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.EndFlowEnable, false, "�Ƿ�����", true, true);
                map.SetHelperUrl(BtnAttr.EndFlowLab, this[SYS_CCFLOW, "��������"]); //���Ӱ���

                map.AddTBString(BtnAttr.PrintDocLab, "��ӡ����", "��ӡ���ݰ�ť��ǩ", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.PrintDocEnable, 0, "��ӡ��ʽ", true,
                    true, BtnAttr.PrintDocEnable, "@0=����ӡ@1=��ӡ��ҳ@2=��ӡRTFģ��@3=��ӡWordģ��");
                map.SetHelperUrl(BtnAttr.PrintDocEnable, this[SYS_CCFLOW, "������ӡ��ʽ"]); //���Ӱ���

                // map.AddBoolean(BtnAttr.PrintDocEnable, false, "�Ƿ�����", true, true);
                //map.AddTBString(BtnAttr.AthLab, "����", "������ť��ǩ", true, false, 0, 50, 10);
                //map.AddDDLSysEnum(NodeAttr.FJOpen, 0, this.ToE("FJOpen", "����Ȩ��"), true, true, 
                //    NodeAttr.FJOpen, "@0=�رո���@1=����Ա@2=����ID@3=����ID");

                map.AddTBString(BtnAttr.TrackLab, "�켣", "�켣��ť��ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.TrackEnable, true, "�Ƿ�����", true, true);
                map.SetHelperUrl(BtnAttr.TrackLab, this[SYS_CCFLOW, "�켣"]); //���Ӱ���


                map.AddTBString(BtnAttr.HungLab, "����", "����ť��ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.HungEnable, false, "�Ƿ�����", true, true);
                map.SetHelperUrl(BtnAttr.HungLab, this[SYS_CCFLOW, "����"]); //���Ӱ���.

                map.AddTBString(BtnAttr.SelectAccepterLab, "������", "�����˰�ť��ǩ", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.SelectAccepterEnable, 0, "������ʽ",
          true, true, BtnAttr.SelectAccepterEnable);
                map.SetHelperUrl(BtnAttr.SelectAccepterLab, this[SYS_CCFLOW, "������"]); //���Ӱ���


                map.AddTBString(BtnAttr.SearchLab, "��ѯ", "��ѯ��ť��ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.SearchEnable, false, "�Ƿ�����", true, true);
                map.SetHelperUrl(BtnAttr.SearchLab, this[SYS_CCFLOW, "��ѯ"]); //���Ӱ���


                map.AddTBString(BtnAttr.WorkCheckLab, "���", "��˰�ť��ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.WorkCheckEnable, false, "�Ƿ�����", true, true);

                map.AddTBString(BtnAttr.BatchLab, "������", "��������ť��ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.BatchEnable, false, "�Ƿ�����", true, true);
                map.SetHelperUrl(BtnAttr.BatchLab, this[SYS_CCFLOW, "������"]); //���Ӱ���

                map.AddTBString(BtnAttr.AskforLab, "��ǩ", "��ǩ��ť��ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.AskforEnable, false, "�Ƿ�����", true, true);

                // add by ���� 2014-11-21. ���û������Լ�������ת.
                map.AddTBString(BtnAttr.TCLab, "��ת�Զ���", "��ת�Զ���", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.TCEnable, false, "�Ƿ�����", true, true);



                //map.AddTBString(BtnAttr.AskforLabRe, "ִ��", "��ǩ��ť��ǩ", true, false, 0, 50, 10);
                //map.AddBoolean(BtnAttr.AskforEnable, false, "�Ƿ�����", true, true);

                map.SetHelperUrl(BtnAttr.AskforLab, this[SYS_CCFLOW, "��ǩ"]); //���Ӱ���
                map.AddTBString(BtnAttr.WebOfficeLab, "����", "�ĵ���ť��ǩ", true, false, 0, 50, 10);
                //  map.AddBoolean(BtnAttr.WebOfficeEnable, false, "�Ƿ�����", true, true);
                map.AddDDLSysEnum(BtnAttr.WebOfficeEnable, 0, "�ĵ����÷�ʽ", true, true, BtnAttr.WebOfficeEnable,
                  "@0=������@1=��ť��ʽ@2=��ǩҳ��ʽ");
                map.SetHelperUrl(BtnAttr.WebOfficeLab, this[SYS_CCFLOW, "����"]);

                //map.AddBoolean(BtnAttr.SelectAccepterEnable, false, "�Ƿ�����", true, true);
                #endregion  ���ܰ�ť״̬

                #region ��������
                // ��������
                map.AddTBFloat(NodeAttr.WarningDays, 0, "��������(0������)", true, false); // "��������(0������)"
                map.AddTBFloat(NodeAttr.DeductDays, 1, "����(��)", true, false); //"����(��)"
                map.AddTBFloat(NodeAttr.DeductCent, 2, "�۷�(ÿ����1���)", true, false); //"�۷�(ÿ����1���)"

                map.AddTBFloat(NodeAttr.MaxDeductCent, 0, "��߿۷�", true, false);   //"��߿۷�"
                map.AddTBFloat(NodeAttr.SwinkCent, float.Parse("0.1"), "�����÷�", true, false); //"�����÷�"
                map.AddDDLSysEnum(NodeAttr.OutTimeDeal, 0, "��ʱ����",
                true, true, NodeAttr.OutTimeDeal,
                "@0=������@1=�Զ������˶�(���˶���ָ���ڵ�)@2=�Զ���תָ���ĵ�@3=�Զ�ת��ָ������Ա@4=��ָ������Ա����Ϣ@5=ɾ������@6=ִ��SQL");

                map.AddTBString(NodeAttr.DoOutTime, null, "��������", true, false, 0, 300, 10, true);
                map.AddTBString(NodeAttr.DoOutTimeCond, null, "ִ�г�ʱ����", true, false, 0, 100, 10, true);

                //map.AddTBString(NodeAttr.FK_Flows, null, "flow", false, false, 0, 100, 10);

                map.AddDDLSysEnum(NodeAttr.CHWay, 0, "���˷�ʽ", true, true, NodeAttr.CHWay, "@0=������@1=��ʱЧ@2=��������");
                map.AddTBFloat(NodeAttr.Workload, 0, "������(��λ:����)", true, false);

                // �Ƿ��������˵㣿
                map.AddBoolean(NodeAttr.IsEval, false, "�Ƿ��������˵�", true, true, true);
                #endregion ��������

                #region ����������, �˴������BP.Sys.FrmWorkCheck ҲҪ���.
                // BP.Sys.FrmWorkCheck
                map.AddDDLSysEnum(FrmWorkCheckAttr.FWCSta, (int)FrmWorkCheckSta.Disable, "������״̬",
                    true, true, FrmWorkCheckAttr.FWCSta, "@0=����@1=����@2=ֻ��");

                map.AddDDLSysEnum(FrmWorkCheckAttr.FWCShowModel, (int)FrmWorkShowModel.Free, "��ʾ��ʽ",
                    true, true, FrmWorkCheckAttr.FWCShowModel, "@0=����ʽ@1=����ģʽ"); //��������ʱû����.

                map.AddDDLSysEnum(FrmWorkCheckAttr.FWCType, (int)FWCType.Check, "������������ʽ", true, true, FrmWorkCheckAttr.FWCType, "@0=������@1=��־���");

                map.AddBoolean(FrmWorkCheckAttr.FWCTrackEnable, true, "�켣ͼ�Ƿ���ʾ��", true, true, true);
                map.AddBoolean(FrmWorkCheckAttr.FWCListEnable, true, "��ʷ�����Ϣ�Ƿ���ʾ��(��,��ʷ��Ϣ�����������)", true, true, true);

                map.AddBoolean(FrmWorkCheckAttr.FWCIsShowAllStep, false, "�ڹ켣�����Ƿ���ʾ���еĲ��裿", true, true);

                map.AddTBString(FrmWorkCheckAttr.FWCOpLabel, "���", "��������(���/����/��ʾ)", true, false, 0, 50, 10);
                map.AddTBString(FrmWorkCheckAttr.FWCDefInfo, "ͬ��", "Ĭ�������Ϣ", true, false, 0, 50, 10);
                map.AddBoolean(FrmWorkCheckAttr.SigantureEnabel, false, "�������Ƿ���ʾΪͼƬǩ����", true, true);
                map.AddBoolean(FrmWorkCheckAttr.FWCIsFullInfo, true, "����û�δ����Ƿ���Ĭ�������䣿", true, true, true);

                //map.AddTBFloat(FrmWorkCheckAttr.FWC_X, 5, "λ��X", true, false);
                //map.AddTBFloat(FrmWorkCheckAttr.FWC_Y, 5, "λ��Y", true, false);

                
                // �߶������, ��������ɱ����Ͳ�Ҫ�仯������.
                map.AddTBFloat(FrmWorkCheckAttr.FWC_H, 300, "�߶�", true, false);
                map.SetHelperAlert(FrmWorkCheckAttr.FWC_H, "��������ɱ����Ͳ�Ҫ�仯������,Ϊ0�����ʶΪ100%,Ӧ�õ����ģʽ."); //���Ӱ���
                map.AddTBFloat(FrmWorkCheckAttr.FWC_W, 400, "����", true, false);
                map.SetHelperAlert(FrmWorkCheckAttr.FWC_W, "��������ɱ����Ͳ�Ҫ�仯������,Ϊ0�����ʶΪ100%,Ӧ�õ����ģʽ."); //���Ӱ���
                #endregion ����������.

                #region ���İ�ť
                map.AddTBString(BtnAttr.OfficeOpenLab, "�򿪱���", "�򿪱��ر�ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeOpenEnable, false, "�Ƿ�����", true, true);

                map.AddTBString(BtnAttr.OfficeOpenTemplateLab, "��ģ��", "��ģ���ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeOpenTemplateEnable, false, "�Ƿ�����", true, true);

                map.AddTBString(BtnAttr.OfficeSaveLab, "����", "�����ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeSaveEnable, true, "�Ƿ�����", true, true);

                map.AddTBString(BtnAttr.OfficeAcceptLab, "�����޶�", "�����޶���ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeAcceptEnable, false, "�Ƿ�����", true, true);

                map.AddTBString(BtnAttr.OfficeRefuseLab, "�ܾ��޶�", "�ܾ��޶���ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeRefuseEnable, false, "�Ƿ�����", true, true);

                map.AddTBString(BtnAttr.OfficeOverLab, "�׺�", "�׺찴ť��ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeOverEnable, false, "�Ƿ�����", true, true);

                map.AddBoolean(BtnAttr.OfficeMarksEnable, true, "�Ƿ�鿴�û�����", true, true,true);

                map.AddTBString(BtnAttr.OfficePrintEnable, "��ӡ", "��ӡ��ť��ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficePrintEnable, false, "�Ƿ�����", true, true);

                map.AddTBString(BtnAttr.OfficeSaveLab, "ǩ��", "ǩ�°�ť��ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeSealEnable, false, "�Ƿ�����", true, true);

                map.AddTBString(BtnAttr.OfficeInsertFlowLab, "��������", "�������̱�ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeInsertFlowEnable, false, "�Ƿ�����", true, true);

                map.AddBoolean(BtnAttr.OfficeNodeInfo, false, "�Ƿ��¼�ڵ���Ϣ", true, true);
                map.AddBoolean(BtnAttr.OfficeReSavePDF, false, "�Ƿ���Զ�����ΪPDF", true, true);

                map.AddTBString(BtnAttr.OfficeDownLab, "����", "���ذ�ť��ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeDownEnable, false, "�Ƿ�����", true, true);

                map.AddBoolean(BtnAttr.OfficeIsMarks, true, "�Ƿ��������ģʽ", true, true);
                map.AddTBString(BtnAttr.OfficeTemplate, "", "ָ���ĵ�ģ��", true, false, 0, 100, 10);

                map.AddBoolean(BtnAttr.OfficeIsParent, true, "�Ƿ�ʹ�ø����̵��ĵ�", true, true);

                map.AddBoolean(BtnAttr.OfficeTHEnable, false, "�Ƿ��Զ��׺�", true, true);
                map.AddTBString(BtnAttr.OfficeTHTemplate, "", "�Զ��׺�ģ��", true, false, 0, 200, 10);
                #endregion

                #region �ƶ�����.
                map.AddDDLSysEnum(NodeAttr.MPhone_WorkModel, 0, "�ֻ�����ģʽ", true, true, NodeAttr.MPhone_WorkModel, "@0=ԭ��̬@1=�����@2=����");
                map.AddDDLSysEnum(NodeAttr.MPhone_SrcModel, 0, "�ֻ���Ļģʽ", true, true, NodeAttr.MPhone_SrcModel, "@0=ǿ�ƺ���@1=ǿ������@2=��������Ӧ����");

                map.AddDDLSysEnum(NodeAttr.MPad_WorkModel, 0, "ƽ�幤��ģʽ", true, true, NodeAttr.MPad_WorkModel, "@0=ԭ��̬@1=�����@2=����");
                map.AddDDLSysEnum(NodeAttr.MPad_SrcModel, 0, "ƽ����Ļģʽ", true, true, NodeAttr.MPad_SrcModel, "@0=ǿ�ƺ���@1=ǿ������@2=��������Ӧ����");
                map.SetHelperUrl(NodeAttr.MPhone_WorkModel, "http://bbs.ccflow.org/showtopic-2866.aspx");
                #endregion �ƶ�����.

                //�ڵ㹤����
                map.AddDtl(new NodeToolbars(), NodeToolbarAttr.FK_Node);

                #region ��Ӧ��ϵ
                // ��ع��ܡ�
                if (Glo.OSModel == OSModel.WorkFlow)
                {
                    map.AttrsOfOneVSM.Add(new BP.WF.Template.NodeStations(), new BP.WF.Port.Stations(),
                        NodeStationAttr.FK_Node, NodeStationAttr.FK_Station,
                        DeptAttr.Name, DeptAttr.No, "�ڵ�󶨸�λ");

                    //�ж��Ƿ�Ϊ����ʹ�ã�����ʱ����ҳ��������չʾ
                    if (Glo.IsUnit == true)
                    {
                        RefMethod rmDept = new RefMethod();
                        rmDept.Title = "�ڵ�󶨲���";
                        rmDept.ClassMethodName = this.ToString() + ".DoDepts";
                        rmDept.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                        map.AddRefMethod(rmDept);
                    }
                    else
                    {
                        map.AttrsOfOneVSM.Add(new BP.WF.Template.NodeDepts(), new BP.WF.Port.Depts(), NodeDeptAttr.FK_Node, NodeDeptAttr.FK_Dept, DeptAttr.Name,
            DeptAttr.No, "�ڵ�󶨲���");
                    }
                }
                else
                {
                    //�ڵ��λ.
                    map.AttrsOfOneVSM.Add(new BP.WF.Template.NodeStations(),
                        new BP.GPM.Stations(),
                      NodeStationAttr.FK_Node, NodeStationAttr.FK_Station,
                      DeptAttr.Name, DeptAttr.No, "�ڵ�󶨸�λ");
                    //�ж��Ƿ�Ϊ����ʹ�ã�����ʱ����ҳ��������չʾ
                    if (Glo.IsUnit == true)
                    {
                        RefMethod rmDept = new RefMethod();
                        rmDept.Title = "�ڵ�󶨲���";
                        rmDept.ClassMethodName = this.ToString() + ".DoDepts";
                        rmDept.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                        map.AddRefMethod(rmDept);
                    }
                    else
                    {
                        //�ڵ㲿��.
                        map.AttrsOfOneVSM.Add(new BP.WF.Template.NodeDepts(), new BP.GPM.Depts(),
                            NodeDeptAttr.FK_Node, NodeDeptAttr.FK_Dept, DeptAttr.Name,
            DeptAttr.No, "�ڵ�󶨲���");
                    }
                }


                map.AttrsOfOneVSM.Add(new BP.WF.Template.NodeEmps(), new BP.WF.Port.Emps(), NodeEmpAttr.FK_Node, EmpDeptAttr.FK_Emp, DeptAttr.Name,
                    DeptAttr.No, "�ڵ�󶨽�����");

                // ɵ�ϱ������Ե��õ�������. 2014.10.19 ȥ��.
                //map.AttrsOfOneVSM.Add(new BP.WF.NodeFlows(), new Flows(), NodeFlowAttr.FK_Node, NodeFlowAttr.FK_Flow, DeptAttr.Name, DeptAttr.No,
                //    "ɵ�ϱ����ɵ��õ�������");
                #endregion

                RefMethod rm = new RefMethod();
                rm.Title = "���˻صĽڵ�(���˻ع������ÿ��˻�ָ���Ľڵ�ʱ,��������Ч.)"; // "��Ʊ���";
                rm.ClassMethodName = this.ToString() + ".DoCanReturnNodes";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkModel;
                //��������ֶ�.
                rm.RefAttrKey = NodeAttr.ReturnRole;
                rm.RefAttrLinkLabel = "���ÿ��˻صĽڵ�";
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "�ɳ������ͽڵ�(ֻ�г���������ָ���Ľڵ���Գ���ʱ,��������Ч.)"; // "�ɳ������͵Ľڵ�";
                rm.ClassMethodName = this.ToString() + ".DoCanCancelNodes";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                //��������ֶ�.
                rm.RefAttrKey = NodeAttr.CancelRole;
                rm.RefAttrLinkLabel = "";
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "�����Զ����͹���(���ڵ�Ϊ�Զ�����ʱ,��������Ч.)"; // "���͹���";
                rm.ClassMethodName = this.ToString() + ".DoCCRole";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                //��������ֶ�.
                rm.RefAttrKey = NodeAttr.CCRole;
                rm.RefAttrLinkLabel = "�Զ���������";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "���ɵ�ϱ���(���ڵ������������Ϊɵ�ϱ���ʱ,��������Ч.)"; // "��Ʊ���";
                rm.ClassMethodName = this.ToString() + ".DoFormCol4";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                //��������ֶ�.
                rm.RefAttrKey = NodeAttr.SaveModel;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "������ɱ���(���ڵ������������Ϊ���ɱ���ʱ,��������Ч.)"; // "��Ʊ���";
                rm.ClassMethodName = this.ToString() + ".DoFormFree";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                //��������ֶ�.
                rm.RefAttrKey = NodeAttr.SaveModel;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "�����̱���"; // "��Ʊ���"; (���ڵ������������Ϊ���α���ʱ,��������Ч.)
                rm.ClassMethodName = this.ToString() + ".DoFormTree";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
              //   rm.Title
                //��������ֶ�.
                rm.RefAttrKey = NodeAttr.SaveModel;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);
                

                rm = new RefMethod();
                rm.Title = "��rtf��ӡ��ʽģ��(����ӡ��ʽΪ��ӡRTF��ʽģ��ʱ,��������Ч)"; //"����&����";
                rm.ClassMethodName = this.ToString() + ".DoBill";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/FileType/doc.gif";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;

                //��������ֶ�.
                rm.RefAttrKey = NodeAttr.PrintDocEnable;
                rm.RefAttrLinkLabel = "";
                rm.Target = "_blank";
                map.AddRefMethod(rm);
                if (BP.Sys.SystemConfig.CustomerNo == "HCBD")
                {
                    /* Ϊ���ɰ�����õĸ��Ի�����. */
                    rm = new RefMethod();
                    rm.Title = "DXReport����";
                    rm.ClassMethodName = this.ToString() + ".DXReport";
                    rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/FileType/doc.gif";
                    map.AddRefMethod(rm);
                }

                rm = new RefMethod();
                rm.Title = "�����¼�"; // "�����¼��ӿ�";
                rm.ClassMethodName = this.ToString() + ".DoAction";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Event.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "������������Ϣ"; // "�����¼��ӿ�";
                rm.ClassMethodName = this.ToString() + ".DoPush2Current";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Message24.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
              //  map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "��ָ����������Ϣ"; // "�����¼��ӿ�";
                rm.ClassMethodName = this.ToString() + ".DoPush2Spec";
              //  rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Message32.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
              //  map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "��Ϣ����"; // "�����¼��ӿ�";
                rm.ClassMethodName = this.ToString() + ".DoListen";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "�����������"; // "�����������";
                rm.ClassMethodName = this.ToString() + ".DoCond";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);
             

                rm = new RefMethod();
                rm.Title = "���ͳɹ�ת������"; // "ת������";
                rm.ClassMethodName = this.ToString() + ".DoTurn";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                //��������ֶ�.
                rm.RefAttrKey = NodeAttr.TurnToDealDoc;
                rm.RefAttrLinkLabel = "";
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "���á�������ѡ����������Աѡ��Χ��"; // "���Ի������˴���"; //(���ʹ�������Ϊ��05����Ч)
                rm.ClassMethodName = this.ToString() + ".DoAccepter";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                //��������ֶ�.
                rm.RefAttrKey = NodeAttr.DeliveryWay;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.RefAttrLinkLabel = "";
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                if (Glo.OSModel==OSModel.BPM)
                {
                    rm = new RefMethod();
                    rm.Title = "BPMģʽ���������ù���";
                    rm.ClassMethodName = this.ToString() + ".DoAccepterRole";
                    rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";

                    //��������ֶ�.
                    //rm.RefAttrKey = NodeAttr.WhoExeIt;
                    rm.RefMethodType = RefMethodType.RightFrameOpen;
                    rm.RefAttrLinkLabel = "";
                    rm.Target = "_blank";
                    map.AddRefMethod(rm);
                }

                rm = new RefMethod();
                rm.Title = "�������̱�����Ȩ��";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoNodeFormTree";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                if (Glo.IsEnableZhiDu)
                {
                    rm = new RefMethod();
                    rm.Title = "��Ӧ�ƶ��½�"; // "���Ի������˴���";
                    rm.ClassMethodName = this.ToString() + ".DoZhiDu";
                    rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                    map.AddRefMethod(rm);

                    rm = new RefMethod();
                    rm.Title = "���յ�"; // "���Ի������˴���";
                    rm.ClassMethodName = this.ToString() + ".DoFengXianDian";
                    rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                    map.AddRefMethod(rm);

                    rm = new RefMethod();
                    rm.Title = "��λְ��"; // "���Ի������˴���";
                    rm.ClassMethodName = this.ToString() + ".DoGangWeiZhiZe";
                    rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                    map.AddRefMethod(rm);
                }

                this._enMap = map;
                return this._enMap;
            }
        }
        /// <summary>
        /// ���Ų�����
        /// </summary>
        /// <returns></returns>
        public string DoDepts()
        {
            PubClass.WinOpen(Glo.CCFlowAppPath + "WF/Comm/Port/DeptTree.aspx?s=d34&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID + "&RefNo=" + DataType.CurrentDataTime, 500, 550);
            return null;
        }
        /// <summary>
        /// �������̱�����Ȩ��
        /// </summary>
        /// <returns></returns>
        public string DoNodeFormTree()
        {
            return Glo.CCFlowAppPath + "WF/Admin/FlowFormTree.aspx?s=d34&FK_Flow=" + this.FK_Flow + "&FK_Node=" +
                   this.NodeID + "&RefNo=" + DataType.CurrentDataTime;
        }
        /// <summary>
        /// �ƶ�
        /// </summary>
        /// <returns></returns>
        public string DoZhiDu()
        {
            PubClass.WinOpen(Glo.CCFlowAppPath + "ZhiDu/NodeZhiDuDtl.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow, "�ƶ�", "Bill", 700, 400, 200, 300);
            return null;
        }
        /// <summary>
        /// ���յ�
        /// </summary>
        /// <returns></returns>
        public string DoFengXianDian()
        {
            // PubClass.WinOpen(Glo.CCFlowAppPath + "ZhiDu/NodeFengXianDian.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow, "�ƶ�", "Bill", 700, 400, 200, 300);
            return null;
        }
        /// <summary>
        /// ���˹���
        /// </summary>
        /// <returns></returns>
        public string DoAccepterRole()
        {
            BP.WF.Node nd = new BP.WF.Node(this.NodeID);

            if (nd.HisDeliveryWay != DeliveryWay.ByCCFlowBPM)
                return "�ڵ���ʹ�����û�����ð���bpmģʽ����������ִ�иò�����Ҫ��ִ�иò�����ѡ��ڵ������нڵ�������Ȼ��ѡ����bpmģʽ���㣬�㱣�水ť��";

            return Glo.CCFlowAppPath + "WF/Admin/FindWorker/List.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow; 
         //   return null;
        }
        public string DoTurn()
        {
            return Glo.CCFlowAppPath + "WF/Admin/TurnTo.aspx?FK_Node=" + this.NodeID;
            //, "�ڵ����ת����", "FrmTurn", 800, 500, 200, 300);
            //BP.WF.Node nd = new BP.WF.Node(this.NodeID);
            //return nd.DoTurn();
        }
        /// <summary>
        /// ���͹���
        /// </summary>
        /// <returns></returns>
        public string DoCCRole()
        {
            return Glo.CCFlowAppPath + "WF/Comm/RefFunc/UIEn.aspx?EnName=BP.WF.Template.CC&PK=" + this.NodeID; 
            //PubClass.WinOpen("./RefFunc/UIEn.aspx?EnName=BP.WF.CC&PK=" + this.NodeID, "���͹���", "Bill", 800, 500, 200, 300);
            //return null;
        }
        /// <summary>
        /// ���Ի������˴���
        /// </summary>
        /// <returns></returns>
        public string DoAccepter()
        {
            return Glo.CCFlowAppPath + "WF/Comm/RefFunc/UIEn.aspx?EnName=BP.WF.Template.Selector&PK=" + this.NodeID;
            //return null;
        }
        /// <summary>
        /// �˻ؽڵ�
        /// </summary>
        /// <returns></returns>
        public string DoCanReturnNodes()
        {
            return Glo.CCFlowAppPath + "WF/Admin/CanReturnNodes.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// �������͵Ľڵ�
        /// </summary>
        /// <returns></returns>
        public string DoCanCancelNodes()
        {
            return Glo.CCFlowAppPath + "WF/Admin/CanCancelNodes.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow; 
        }
        /// <summary>
        /// DXReport
        /// </summary>
        /// <returns></returns>
        public string DXReport()
        {
            return Glo.CCFlowAppPath + "WF/Admin/DXReport.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        public string DoPush2Current()
        {
            return Glo.CCFlowAppPath + "WF/Admin/Listen.aspx?CondType=0&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID + "&FK_Attr=&DirType=&ToNodeID=";
        }
        public string DoPush2Spec()
        {
            return Glo.CCFlowAppPath + "WF/Admin/Listen.aspx?CondType=0&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID + "&FK_Attr=&DirType=&ToNodeID=";
        }
        /// <summary>
        /// ִ����Ϣ����
        /// </summary>
        /// <returns></returns>
        public string DoListen()
        {
            return Glo.CCFlowAppPath + "WF/Admin/Listen.aspx?CondType=0&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID + "&FK_Attr=&DirType=&ToNodeID=";
        }
        public string DoFeatureSet()
        {
            return Glo.CCFlowAppPath + "WF/Admin/FeatureSetUI.aspx?CondType=0&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID + "&FK_Attr=&DirType=&ToNodeID=";
        }
        public string DoShowSheets()
        {
            return Glo.CCFlowAppPath + "WF/Admin/ShowSheets.aspx?CondType=0&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID + "&FK_Attr=&DirType=&ToNodeID=";
        }
        public string DoCond()
        {
            return Glo.CCFlowAppPath + "WF/Admin/Condition.aspx?CondType=" + (int)CondType.Flow + "&FK_Flow=" + this.FK_Flow + "&FK_MainNode=" + this.NodeID + "&FK_Node=" + this.NodeID + "&FK_Attr=&DirType=&ToNodeID=" + this.NodeID;
        }
        /// <summary>
        /// ���ɵ�ϱ���
        /// </summary>
        /// <returns></returns>
        public string DoFormCol4()
        {
            return Glo.CCFlowAppPath + "WF/MapDef/MapDef.aspx?PK=ND" + this.NodeID;
        }
        /// <summary>
        /// ������ɱ���
        /// </summary>
        /// <returns></returns>
        public string DoFormFree()
        {
            return Glo.CCFlowAppPath + "WF/MapDef/CCForm/Frm.aspx?FK_MapData=ND" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// �����̱���
        /// </summary>
        /// <returns></returns>
        public string DoFormTree()
        {
            return Glo.CCFlowAppPath + "WF/Admin/FlowFrms.aspx?ShowType=FlowFrms&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID + "&Lang=CH";
        }
        
        public string DoMapData()
        {
            int i = this.GetValIntByKey(NodeAttr.FormType);

            // ����.
            NodeFormType type = (NodeFormType)i;
            switch (type)
            {
                case NodeFormType.FreeForm:
                    PubClass.WinOpen(Glo.CCFlowAppPath + "WF/MapDef/CCForm/Frm.aspx?FK_MapData=ND" + this.NodeID + "&FK_Flow=" + this.FK_Flow, "��Ʊ���", "sheet", 1024, 768, 0, 0);
                    break;
                default:
                case NodeFormType.FixForm:
                    PubClass.WinOpen(Glo.CCFlowAppPath + "WF/MapDef/MapDef.aspx?PK=ND" + this.NodeID, "��Ʊ���", "sheet", 800, 500, 210, 300);
                    break;
            }
            return null;
        }
        public string DoAction()
        {
            return Glo.CCFlowAppPath + "WF/Admin/Action.aspx?NodeID=" + this.NodeID + "&FK_Flow=" + this.FK_Flow + "&tk=" + new Random().NextDouble();
        }
        /// <summary>
        /// ���ݴ�ӡ
        /// </summary>
        /// <returns></returns>
        public string DoBill()
        {
            return Glo.CCFlowAppPath + "WF/Admin/Bill.aspx?NodeID=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// ����
        /// </summary>
        /// <returns></returns>
        public string DoFAppSet()
        {
            return Glo.CCFlowAppPath + "WF/Admin/FAppSet.aspx?NodeID=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }

        protected override bool beforeUpdate()
        {
            //�������̰汾
            Flow.UpdateVer(this.FK_Flow);

            //�ѹ����������÷��� sys_mapdata��.
            ToolbarExcel te = new ToolbarExcel("ND" + this.NodeID);
            te.Copy(this);
            try
            {
                te.Update();
            }
            catch
            {

            }

           
            #region  //��� NEE ʵ��.
            //if (string.IsNullOrEmpty(this.NodeMark) == false)
            //{
            //    Flow fl = new Flow(this.FK_Flow);

            //    object obj = Glo.GetNodeEventEntityByNodeMark( fl.FlowMark, this.NodeMark);
            //    if (obj == null)
            //        throw new Exception("@�ڵ��Ǵ���û���ҵ��ýڵ���(" + this.NodeMark + ")�Ľڵ��¼�ʵ��.");
            //    this.NodeEventEntity = obj.ToString();
            //}
            //else
            //{
            //    this.NodeEventEntity = "";
            //}
            #endregion ͬ���¼�ʵ��

            #region �����ڵ�����.
            Node nd = new Node(this.NodeID);
            if (nd.IsStartNode == true)
            {
                /*������ť������*/
                //�����˻�, ��ǩ���ƽ����˻�, ���߳�.
                this.SetValByKey(BtnAttr.ReturnRole,(int)ReturnRole.CanNotReturn);
                this.SetValByKey(BtnAttr.HungEnable, false);
                this.SetValByKey(BtnAttr.ThreadEnable, false); //���߳�.
            }

            if (nd.HisRunModel == RunModel.HL || nd.HisRunModel == RunModel.FHL)
            {
                /*����Ǻ�����*/
            }
            else
            {
                this.SetValByKey(BtnAttr.ThreadEnable, false); //���߳�.
            }
            #endregion �����ڵ�����.

            //#region ������Ϣ�����ֶ�.
            //this.SetPara(NodeAttr.MsgCtrl, this.GetValIntByKey(NodeAttr.MsgCtrl));
            //this.SetPara(NodeAttr.MsgIsSend, this.GetValIntByKey(NodeAttr.MsgIsSend));
            //this.SetPara(NodeAttr.MsgIsReturn, this.GetValIntByKey(NodeAttr.MsgIsReturn));
            //this.SetPara(NodeAttr.MsgIsShift, this.GetValIntByKey(NodeAttr.MsgIsShift));
            //this.SetPara(NodeAttr.MsgIsCC, this.GetValIntByKey(NodeAttr.MsgIsCC));

            //this.SetPara(NodeAttr.MsgMailEnable, this.GetValIntByKey(NodeAttr.MsgMailEnable));
            //this.SetPara(NodeAttr.MsgMailTitle, this.GetValStrByKey(NodeAttr.MsgMailTitle));
            //this.SetPara(NodeAttr.MsgMailDoc, this.GetValStrByKey(NodeAttr.MsgMailDoc));

            //this.SetPara(NodeAttr.MsgSMSEnable, this.GetValIntByKey(NodeAttr.MsgSMSEnable));
            //this.SetPara(NodeAttr.MsgSMSDoc, this.GetValStrByKey(NodeAttr.MsgSMSDoc));
            //#endregion

            return base.beforeUpdate();
        }
        #endregion
    }
    /// <summary>
    /// �ڵ㼯��
    /// </summary>
    public class NodeSheets : Entities
    {
        #region ���췽��
        /// <summary>
        /// �ڵ㼯��
        /// </summary>
        public NodeSheets()
        {
        }
        #endregion

        public override Entity GetNewEntity
        {
            get { return new NodeSheet(); }
        }
    }
}