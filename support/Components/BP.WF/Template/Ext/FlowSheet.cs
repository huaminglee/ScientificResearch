using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Port;
using BP.En;
using BP.Web;
using BP.Sys;
using BP.WF.Data;
using BP.WF.Data;

namespace BP.WF.Template.Ext
{
    /// <summary>
    /// ����
    /// </summary>
    public class FlowSheet : EntityNoName
    {
        #region ����.
        /// <summary>
        /// �����¼�ʵ��
        /// </summary>
        public string FlowEventEntity
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.FlowEventEntity);
            }
            set
            {
                this.SetValByKey(FlowAttr.FlowEventEntity, value);
            }
        }
        /// <summary>
        /// ���̱��
        /// </summary>
        public string FlowMark
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.FlowMark);
            }
            set
            {
                this.SetValByKey(FlowAttr.FlowMark, value);
            }
        }
        /// <summary>
        /// ����߱��
        /// </summary>
        public string DesignerNo
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.DesignerNo);
            }
            set
            {
                this.SetValByKey(FlowAttr.DesignerNo, value);
            }
        }
        /// <summary>
        /// ���������
        /// </summary>
        public string DesignerName
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.DesignerName);
            }
            set
            {
                this.SetValByKey(FlowAttr.DesignerName, value);
            }
        }
        /// <summary>
        /// ������ɸ�ʽ
        /// </summary>
        public string BillNoFormat
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.BillNoFormat);
            }
            set
            {
                this.SetValByKey(FlowAttr.BillNoFormat, value);
            }
        }
        #endregion ����.

        #region ���췽��
        /// <summary>
        /// UI�����ϵķ��ʿ���
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (BP.Web.WebUser.No == "admin" || this.DesignerNo == WebUser.No)
                {
                    uac.IsUpdate = true;
                }
                return uac;
            }
        }
        /// <summary>
        /// ����
        /// </summary>
        public FlowSheet()
        {
        }
        /// <summary>
        /// ����
        /// </summary>
        /// <param name="_No">���</param>
        public FlowSheet(string _No)
        {
            this.No = _No;
            if (SystemConfig.IsDebug)
            {
                int i = this.RetrieveFromDBSources();
                if (i == 0)
                    throw new Exception("���̱�Ų�����");
            }
            else
            {
                this.Retrieve();
            }
        }
        /// <summary>
        /// ��д���෽��
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_Flow");

                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "����";
                map.CodeStruct = "3";

                #region �������ԡ�
                map.AddTBStringPK(FlowAttr.No, null, "���", true, true, 1, 10, 3);
                map.SetHelperAlert(FlowAttr.No, "���̱�Ŵ�001��ʼ,��string����,�ڵ�����int����,�����̱�ż���λ�������. \t\n���磺���̱����001,�ڵ��ž���:101,102....."); //ʹ��alert�ķ�ʽ��ʾ������Ϣ.

                map.AddDDLEntities(FlowAttr.FK_FlowSort, "01", "�������", new FlowSorts(), true);
                map.AddTBString(FlowAttr.Name, null, "����", true, false, 0, 50, 10, true);

                // add 2013-02-14 Ψһȷ�������̵ı��
                map.AddTBString(FlowAttr.FlowMark, null, "���̱��", true, false, 0, 150, 10);
                map.AddTBString(FlowAttr.FlowEventEntity, null, "�����¼�ʵ��", true, true, 0, 150, 10);
                map.SetHelperBaidu(FlowAttr.FlowMark, "ccflow ���̱��");

                // add 2013-02-05.
                map.AddTBString(FlowAttr.TitleRole, null, "�������ɹ���", true, false, 0, 150, 10, true);
                map.SetHelperBaidu(FlowAttr.TitleRole, "ccflow �������ɹ���");

                //add  2013-08-30.
                map.AddTBString(FlowAttr.BillNoFormat, null, "���ݱ�Ÿ�ʽ", true, false, 0, 50, 10, false);
                map.SetHelperBaidu(FlowAttr.BillNoFormat, "ccflow ���ݱ�Ÿ�ʽ");

                // add 2014-10-19.
                map.AddDDLSysEnum(FlowAttr.ChartType, (int)FlowChartType.Icon, "�ڵ�ͼ������", true, true,
                    "ChartType", "@0=����ͼ��@1=Ф��ͼƬ");

                map.AddBoolean(FlowAttr.IsCanStart, true, "���Զ���������(�������������̿�����ʾ�ڷ��������б���)", true, true, true);
                map.AddBoolean(FlowAttr.IsMD5, false, "�Ƿ������ݼ�������(MD5���ݼ��ܷ��۸�)", true, true, true);
                map.SetHelperBaidu(FlowAttr.IsMD5, "ccflow MD5");
                map.AddBoolean(FlowAttr.IsFullSA, false, "�Ƿ��Զ�����δ���Ĵ����ˣ�", true, true, true);

                map.AddBoolean(FlowAttr.IsAutoSendSubFlowOver, false,
                    "(Ϊ������ʱ)�����̽���ʱ���Ƿ���������������ɺ��ø������Զ����͵���һ����", true, true, true);
                map.SetHelperBaidu(FlowAttr.IsAutoSendSubFlowOver, "ccflow �Ƿ���������������ɺ������Զ����͵���һ��");
                map.AddBoolean(FlowAttr.IsGuestFlow, false, "�Ƿ��ⲿ�û���������(����֯�ṹ��Ա���������)", true, true, false);

                //�������� add 2013-12-27. 
                map.AddBoolean(FlowAttr.IsBatchStart, false, "�Ƿ���������������̣�(����Ǿ�Ҫ���÷������Ҫ��д���ֶ�,����ö��ŷֿ�)", true, true, true);
                map.AddTBString(FlowAttr.BatchStartFields, null, "�����ֶ�s", true, false, 0, 500, 10, true);
                map.SetHelperBaidu(FlowAttr.IsBatchStart, "ccflow �Ƿ����������������");
                map.AddDDLSysEnum(FlowAttr.FlowAppType, (int)FlowAppType.Normal, "����Ӧ������",
                  true, true, "FlowAppType", "@0=ҵ������@1=������(��Ŀ������)@2=��������(VSTO)");
                map.AddDDLSysEnum(FlowAttr.TimelineRole, (int)TimelineRole.ByNodeSet, "ʱЧ�Թ���",
                 true, true, FlowAttr.TimelineRole, "@0=���ڵ�(�ɽڵ�����������)@1=��������(��ʼ�ڵ�SysSDTOfFlow�ֶμ���)");

                // ���ݴ洢.
                map.AddDDLSysEnum(FlowAttr.DataStoreModel, (int)DataStoreModel.ByCCFlow,
                    "�������ݴ洢ģʽ", true, true, FlowAttr.DataStoreModel,
                   "@0=���ݹ켣ģʽ@1=���ݺϲ�ģʽ");
                map.AddTBString(FlowAttr.PTable, null, "�洢����", true, false, 0, 30, 10);

                //add 2013-05-22.
                map.AddTBString(FlowAttr.HistoryFields, null, "��ʷ�鿴�ֶ�", true, false, 0, 500, 10, true);
                map.SetHelperBaidu(FlowAttr.HistoryFields, "ccflow ��ʷ�鿴�ֶ�");
                map.AddTBString(FlowAttr.FlowNoteExp, null, "��ע�ı���ʽ", true, false, 0, 500, 10, true);
                map.SetHelperBaidu(FlowAttr.FlowNoteExp, "ccflow ��ע�ı���ʽ");
                map.AddTBString(FlowAttr.Note, null, "��������", true, false, 0, 100, 10, true);

                map.AddDDLSysEnum(FlowAttr.FlowAppType, (int)FlowAppType.Normal, "����Ӧ������", true, true, "FlowAppType", "@0=ҵ������@1=������(��Ŀ������)@2=��������(VSTO)");
                #endregion �������ԡ�

                #region ������ʽ
                map.AddDDLSysEnum(FlowAttr.FlowRunWay, (int)FlowRunWay.HandWork, "������ʽ",
                    true, true, FlowAttr.FlowRunWay, "@0=�ֹ�����@1=ָ����Ա��ʱ����@2=���ݼ���ʱ����@3=����ʽ����");

                map.SetHelperBaidu(FlowAttr.FlowRunWay, "ccflow ���з�ʽ");
                // map.AddTBString(FlowAttr.RunObj, null, "��������", true, false, 0, 100, 10, true);
                map.AddTBStringDoc(FlowAttr.RunObj, null, "��������", true, false, true);
                #endregion ������ʽ

                #region ������������
                string role = "@0=������";
                role += "@1=ÿ��ÿ��һ��";
                role += "@2=ÿ��ÿ��һ��";
                role += "@3=ÿ��ÿ��һ��";
                role += "@4=ÿ��ÿ��һ��";
                role += "@5=ÿ��ÿ��һ��";
                role += "@6=������в����ظ�,(����п����ö��ŷֿ�)";
                role += "@7=���õ�SQL����ԴΪ��,���߷��ؽ��Ϊ��ʱ��������.";
                role += "@8=���õ�SQL����ԴΪ��,���߷��ؽ��Ϊ��ʱ����������.";
                map.AddDDLSysEnum(FlowAttr.StartLimitRole, (int)StartLimitRole.None, "�������ƹ���", true, true, FlowAttr.StartLimitRole, role, true);
                map.AddTBString(FlowAttr.StartLimitPara, null, "�������", true, false, 0, 500, 10, true);
                map.AddTBStringDoc(FlowAttr.StartLimitAlert, null, "������ʾ", true, false, true);

                //   map.AddTBString(FlowAttr.StartLimitAlert, null, "������ʾ", true, false, 0, 500, 10, true);
                //    map.AddDDLSysEnum(FlowAttr.StartLimitWhen, (int)StartLimitWhen.StartFlow, "��ʾʱ��", true, true, FlowAttr.StartLimitWhen, "@0=��������ʱ@1=����ǰ��ʾ", false);
                #endregion ������������

                #region ����ǰ������
                //map.AddDDLSysEnum(FlowAttr.DataStoreModel, (int)DataStoreModel.ByCCFlow,
                //    "�������ݴ洢ģʽ", true, true, FlowAttr.DataStoreModel,
                //   "@0=���ݹ켣ģʽ@1=���ݺϲ�ģʽ");

                //����ǰ���ù���.
                map.AddDDLSysEnum(FlowAttr.StartGuideWay, (int)StartGuideWay.None, "ǰ�õ�����ʽ", true, true,
                    FlowAttr.StartGuideWay,
                    "@0=��@1=��ϵͳ��URL-(��������)����ģʽ@2=��ϵͳ��URL-(�Ӹ�����)����ģʽ@3=��ϵͳ��URL-(ʵ���¼,δ���)����ģʽ@4=��ϵͳ��URL-(ʵ���¼,δ���)����ģʽ@5=�ӿ�ʼ�ڵ�Copy����@10=���Զ����Url@11=���û��������", true);
                map.SetHelperBaidu(FlowAttr.StartGuideWay, "ccflow ǰ�õ�����ʽ");

                map.AddTBStringDoc(FlowAttr.StartGuidePara1, null, "����1", true, false, true);
                map.AddTBStringDoc(FlowAttr.StartGuidePara2, null, "����2", true, false, true);
                map.AddTBStringDoc(FlowAttr.StartGuidePara3, null, "����3", true, false, true);

                map.AddBoolean(FlowAttr.IsResetData, false, "�Ƿ����ÿ�ʼ�ڵ��������ð�ť��", true, true, true);
                //     map.AddBoolean(FlowAttr.IsImpHistory, false, "�Ƿ����õ�����ʷ���ݰ�ť��", true, true, true);
                map.AddBoolean(FlowAttr.IsLoadPriData, false, "�Ƿ��Զ�װ����һ�����ݣ�", true, true, true);

                #endregion ����ǰ������

                #region �������̡�
                //��������.
                map.AddDDLSysEnum(FlowAttr.CFlowWay, (int)CFlowWay.None, "��������", true, true,
                    FlowAttr.CFlowWay, "@0=��:������������@1=���ղ���@2=�����ֶ�����");
                map.AddTBStringDoc(FlowAttr.CFlowPara, null, "�������̲���", true, false, true);

                // add 2013-03-24.
                map.AddTBString(FlowAttr.DesignerNo, null, "����߱��", false, false, 0, 32, 10);
                map.AddTBString(FlowAttr.DesignerName, null, "���������", false, false, 0, 100, 10);
                #endregion �������̡�


                map.AddSearchAttr(FlowAttr.FK_FlowSort);

                //map.AddRefMethod(rm);
                RefMethod rm = new RefMethod();
                rm = new RefMethod();
                rm.Title = "��������"; // "��Ƽ�鱨��";
                //rm.ToolTip = "���������Ƶ����⡣";
                rm.Icon = Glo.CCFlowAppPath + "WF/Img/EntityFunc/Flow/Run.png";
                rm.ClassMethodName = this.ToString() + ".DoRunIt";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "��鱨��"; // "��Ƽ�鱨��";
                rm.Icon = Glo.CCFlowAppPath + "WF/Img/EntityFunc/Flow/CheckRpt.png";
                rm.ClassMethodName = this.ToString() + ".DoCheck";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "��Ʊ���"; // "��������";
                rm.Icon = Glo.CCFlowAppPath + "WF/Img/Btn/Rpt.gif";
                rm.ClassMethodName = this.ToString() + ".DoOpenRpt()";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Icon = Glo.CCFlowAppPath + "WF/Img/Btn/Delete.gif";
                rm.Title = "ɾ��ȫ����������"; // this.ToE("DelFlowData", "ɾ������"); // "ɾ������";
                rm.Warning = "��ȷ��Ҫִ��ɾ������������? \t\n�����̵�����ɾ���󣬾Ͳ��ָܻ�����ע��ɾ�������ݡ�";// "��ȷ��Ҫִ��ɾ������������";
                rm.ClassMethodName = this.ToString() + ".DoDelData";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Icon = Glo.CCFlowAppPath + "WF/Img/Btn/Delete.gif";
                rm.Title = "���չ���IDɾ����������"; // this.ToE("DelFlowData", "ɾ������"); // "ɾ������";
                rm.ClassMethodName = this.ToString() + ".DoDelDataOne";
                rm.HisAttrs.AddTBInt("WorkID", 0, "���빤��ID", true, false);
                rm.HisAttrs.AddTBString("beizhu", null, "ɾ����ע", true, false, 0, 100, 100);
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Icon = Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.Title = "�������ɱ�������"; // "ɾ������";
                rm.Warning = "��ȷ��Ҫִ����? ע��:�˷����ķ���Դ��";// "��ȷ��Ҫִ��ɾ������������";
                rm.ClassMethodName = this.ToString() + ".DoReloadRptData";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "�����Զ���������Դ";
                rm.Icon = Glo.CCFlowAppPath + "WF/Img/EntityFunc/Flow/Run.png";
                rm.ClassMethodName = this.ToString() + ".DoSetStartFlowDataSources()";
                //��������ֶ�.
                rm.RefAttrKey = FlowAttr.FlowRunWay;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "�ֹ�������ʱ����";
                rm.Icon = Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.Warning = "��ȷ��Ҫִ����? ע��:���������������������Ϊweb��ִ��ʱ�����ʱ���⣬�����ִ��ʧ�ܡ�";// "��ȷ��Ҫִ��ɾ������������";
                rm.ClassMethodName = this.ToString() + ".DoAutoStartIt()";
                //��������ֶ�.
                rm.RefAttrKey = FlowAttr.FlowRunWay;
                rm.Target = "_blank";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "���̼��";
                rm.Icon = Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoDataManger()";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "�����޸Ľڵ�����";
                rm.Icon = Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoFeatureSetUI()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "�����������̱���";
                rm.Icon = Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoGenerTitle()";
                rm.Warning = "��ȷ��Ҫ�����µĹ������²���������";

                //��������ֶ�.
                rm.RefAttrKey = FlowAttr.TitleRole;

                //rm.RefAttrKey = FlowAttr.TitleRole;
                //rm.RefMethodType = RefMethodType.LinkModel;
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "�ع�����";
                rm.Icon = Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoRebackFlowData()";
                // rm.Warning = "��ȷ��Ҫ�ع�����";
                rm.HisAttrs.AddTBInt("workid", 0, "������Ҫ���WorkID", true, false);
                rm.HisAttrs.AddTBInt("nodeid", 0, "������Ľڵ�ID", true, false);
                rm.HisAttrs.AddTBString("note", null, "�ع�ԭ��", true, false, 0, 600, 200);
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "���̱�����";
                rm.Icon = Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoFlowFormTree()";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "���������ֶ�";
                rm.Icon = Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoBatchStartFields()";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "�����̱���";
                rm.Icon = Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoBindFlowSheet()";
                map.AddRefMethod(rm);


                //rm = new RefMethod();
                //rm.Title = "�����Զ�����"; // "��������";
                //rm.Icon = "/WF/Img/Btn/View.gif";
                //rm.ClassMethodName = this.ToString() + ".DoOpenRpt()";
                ////rm.Icon = "/WF/Img/Btn/Table.gif"; 
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = this.ToE("Event", "�¼�"); // "��������";
                //rm.Icon = "/WF/Img/Btn/View.gif";
                //rm.ClassMethodName = this.ToString() + ".DoOpenRpt()";
                ////rm.Icon = "/WF/Img/Btn/Table.gif";
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = this.ToE("FlowSheetDataOut", "����ת������");  //"����ת������";
                ////  rm.Icon = "/WF/Img/Btn/Table.gif";
                //rm.ToolTip = "���������ʱ�䣬��������ת���浽����ϵͳ��Ӧ�á�";
                //rm.ClassMethodName = this.ToString() + ".DoExp";
                //map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region  ��������
        /// <summary>
        /// �����޸Ľڵ�����.
        /// </summary>
        /// <returns></returns>
        public string DoFeatureSetUI()
        {
            return Glo.CCFlowAppPath + "WF/Admin/FeatureSetUI.aspx?s=d34&ShowType=FlowFrms&FK_Node=" + int.Parse(this.No) + "01&FK_Flow=" + this.No + "&ExtType=StartFlow&RefNo=" + DataType.CurrentDataTime;
        }
        public string DoBindFlowSheet()
        {
            PubClass.WinOpen(Glo.CCFlowAppPath + "WF/Admin/FlowFrms.aspx?s=d34&ShowType=FlowFrms&FK_Node=0&FK_Flow=" + this.No + "&ExtType=StartFlow&RefNo=" + DataType.CurrentDataTime, 700, 500);
            return null;
        }
        /// <summary>
        /// ���������ֶ�
        /// </summary>
        /// <returns></returns>
        public string DoBatchStartFields()
        {
            PubClass.WinOpen(Glo.CCFlowAppPath + "WF/Admin/BatchStartFields.aspx?s=d34&FK_Flow=" + this.No + "&ExtType=StartFlow&RefNo=" + DataType.CurrentDataTime, 700, 500);
            return null;
        }
        /// <summary>
        /// �ָ�����ɵ��������ݵ�ָ���Ľڵ㣬����ڵ�Ϊ0�ͻָ������һ����ɵĽڵ���ȥ.
        /// </summary>
        /// <param name="workid">Ҫ�ָ���workid</param>
        /// <param name="backToNodeID">�ָ����Ľڵ��ţ������0����ʾ�ظ����������һ���ڵ���ȥ.</param>
        /// <param name="note"></param>
        /// <returns></returns>
        public string DoRebackFlowData(Int64 workid, int backToNodeID, string note)
        {
            if (note.Length <= 2)
                return "����д�ָ�����ɵ�����ԭ��.";

            Flow fl = new Flow(this.No);
            GERpt rpt = new GERpt("ND" + int.Parse(this.No) + "Rpt");
            rpt.OID = workid;
            int i = rpt.RetrieveFromDBSources();
            if (i == 0)
                throw new Exception("@�����������ݶ�ʧ��");
            if (backToNodeID == 0)
                backToNodeID = rpt.FlowEndNode;

            Emp empStarter = new Emp(rpt.FlowStarter);

            // ���һ���ڵ�.
            Node endN = new Node(backToNodeID);
            GenerWorkFlow gwf = null;
            bool isHaveGener = false;
            try
            {
                #region ��������������������.
                gwf = new GenerWorkFlow();
                gwf.WorkID = workid;
                if (gwf.RetrieveFromDBSources() == 1)
                {
                    isHaveGener = true;
                    //�ж�״̬
                    if (gwf.WFState != WFState.Complete)
                        throw new Exception("@��ǰ����IDΪ:" + workid + "������û�н���,���ܲ��ô˷����ָ���");
                }

                gwf.FK_Flow = this.No;
                gwf.FlowName = this.Name;
                gwf.WorkID = workid;
                gwf.PWorkID = rpt.PWorkID;
                gwf.PFlowNo = rpt.PFlowNo;
                gwf.PNodeID = rpt.PNodeID;
                gwf.PEmp = rpt.PEmp;


                gwf.FK_Node = backToNodeID;
                gwf.NodeName = endN.Name;

                gwf.Starter = rpt.FlowStarter;
                gwf.StarterName = empStarter.Name;
                gwf.FK_FlowSort = fl.FK_FlowSort;
                gwf.Title = rpt.Title;
                gwf.WFState = WFState.ReturnSta; /*����Ϊ�˻ص�״̬*/
                gwf.FK_Dept = rpt.FK_Dept;

                Dept dept = new Dept(empStarter.FK_Dept);

                gwf.DeptName = dept.Name;
                gwf.PRI = 1;

                DateTime dttime = DateTime.Now;
                dttime = dttime.AddDays(3);

                gwf.SDTOfNode = dttime.ToString("yyyy-MM-dd HH:mm:ss");
                gwf.SDTOfFlow = dttime.ToString("yyyy-MM-dd HH:mm:ss");
                if (isHaveGener)
                    gwf.Update();
                else
                    gwf.Insert(); /*����������������.*/

                #endregion ��������������������
                string ndTrack = "ND" + int.Parse(this.No) + "Track";
                string actionType = (int)ActionType.Forward + "," + (int)ActionType.FlowOver + "," + (int)ActionType.ForwardFL + "," + (int)ActionType.ForwardHL;
                string sql = "SELECT  * FROM " + ndTrack + " WHERE   ActionType IN (" + actionType + ")  and WorkID=" + workid + " ORDER BY RDT DESC, NDFrom ";
                System.Data.DataTable dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0)
                    throw new Exception("@����IDΪ:" + workid + "�����ݲ�����.");

                string starter = "";
                bool isMeetSpecNode = false;
                GenerWorkerList currWl = new GenerWorkerList();
                foreach (DataRow dr in dt.Rows)
                {
                    int ndFrom = int.Parse(dr["NDFrom"].ToString());
                    Node nd = new Node(ndFrom);

                    string ndFromT = dr["NDFromT"].ToString();
                    string EmpFrom = dr[TrackAttr.EmpFrom].ToString();
                    string EmpFromT = dr[TrackAttr.EmpFromT].ToString();

                    // ������ ������Ա����Ϣ.
                    GenerWorkerList gwl = new GenerWorkerList();
                    gwl.WorkID = workid;
                    gwl.FK_Flow = this.No;

                    gwl.FK_Node = ndFrom;
                    gwl.FK_NodeText = ndFromT;

                    if (gwl.FK_Node == backToNodeID)
                    {
                        gwl.IsPass = false;
                        currWl = gwl;
                    }

                    gwl.FK_Emp = EmpFrom;
                    gwl.FK_EmpText = EmpFromT;
                    if (gwl.IsExits)
                        continue; /*�п����Ƿ����˻ص����.*/

                    Emp emp = new Emp(gwl.FK_Emp);
                    gwl.FK_Dept = emp.FK_Dept;

                    gwl.RDT = dr["RDT"].ToString();
                    gwl.SDT = dr["RDT"].ToString();
                    gwl.DTOfWarning = gwf.SDTOfNode;
                    gwl.WarningDays = nd.WarningDays;
                    gwl.IsEnable = true;
                    gwl.WhoExeIt = nd.WhoExeIt;
                    gwl.Insert();
                }

                #region �����˻���Ϣ, �ý������ܹ������˻�ԭ��.
                ReturnWork rw = new ReturnWork();
                rw.WorkID = workid;
                rw.ReturnNode = backToNodeID;
                rw.ReturnNodeName = endN.Name;
                rw.Returner = WebUser.No;
                rw.ReturnerName = WebUser.Name;

                rw.ReturnToNode = currWl.FK_Node;
                rw.ReturnToEmp = currWl.FK_Emp;
                rw.Note = note;
                rw.RDT = DataType.CurrentDataTime;
                rw.IsBackTracking = false;
                rw.MyPK = BP.DA.DBAccess.GenerGUID();
                #endregion   �����˻���Ϣ, �ý������ܹ������˻�ԭ��.

                //�������̱���״̬.
                rpt.FlowEnder = currWl.FK_Emp;
                rpt.WFState = WFState.ReturnSta; /*����Ϊ�˻ص�״̬*/
                rpt.FlowEndNode = currWl.FK_Node;
                rpt.Update();

                // ������˷���һ����Ϣ.
                BP.WF.Dev2Interface.Port_SendMsg(currWl.FK_Emp, "�����ָ�:" + gwf.Title, "������:" + WebUser.No + " �ָ�." + note, "ReBack" + workid, BP.WF.SMSMsgType.ToDo, this.No, int.Parse(this.No + "01"), workid, 0);

                //д�����־.
                WorkNode wn = new WorkNode(workid, currWl.FK_Node);
                wn.AddToTrack(ActionType.RebackOverFlow, currWl.FK_Emp, currWl.FK_EmpText, currWl.FK_Node, currWl.FK_NodeText, note);

                return "@�Ѿ���ԭ�ɹ�,���ڵ������Ѿ���ԭ��(" + currWl.FK_NodeText + "). @��ǰ����������Ϊ(" + currWl.FK_Emp + " , " + currWl.FK_EmpText + ")  @��֪ͨ����������.";
            }
            catch (Exception ex)
            {
                //�˱��ļ�¼ɾ����ȡ��
                //gwf.Delete();
                GenerWorkerList wl = new GenerWorkerList();
                wl.Delete(GenerWorkerListAttr.WorkID, workid);

                string sqls = "";
                sqls += "@UPDATE " + fl.PTable + " SET WFState=" + (int)WFState.Complete + " WHERE OID=" + workid;
                DBAccess.RunSQLs(sqls);
                return "<font color=red>����ڼ���ִ���</font><hr>" + ex.Message;
            }
        }
        /// <summary>
        /// ���²������⣬�����µĹ���.
        /// </summary>
        public string DoGenerTitle()
        {
            if (WebUser.No != "admin")
                return "��admin�û�����ִ�С�";
            Flow fl = new Flow(this.No);
            Node nd = fl.HisStartNode;
            Works wks = nd.HisWorks;
            wks.RetrieveAllFromDBSource(WorkAttr.Rec);
            string table = nd.HisWork.EnMap.PhysicsTable;
            string tableRpt = "ND" + int.Parse(this.No) + "Rpt";
            Sys.MapData md = new Sys.MapData(tableRpt);
            foreach (Work wk in wks)
            {

                if (wk.Rec != WebUser.No)
                {
                    BP.Web.WebUser.Exit();
                    try
                    {
                        Emp emp = new Emp(wk.Rec);
                        BP.Web.WebUser.SignInOfGener(emp);
                    }
                    catch
                    {
                        continue;
                    }
                }
                string sql = "";
                string title = WorkNode.GenerTitle(fl, wk);
                Paras ps = new Paras();
                ps.Add("Title", title);
                ps.Add("OID", wk.OID);
                ps.SQL = "UPDATE " + table + " SET Title=" + SystemConfig.AppCenterDBVarStr + "Title WHERE OID=" + SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQL(ps);

                ps.SQL = "UPDATE " + md.PTable + " SET Title=" + SystemConfig.AppCenterDBVarStr + "Title WHERE OID=" + SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQL(ps);

                ps.SQL = "UPDATE WF_GenerWorkFlow SET Title=" + SystemConfig.AppCenterDBVarStr + "Title WHERE WorkID=" + SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQL(ps);

                ps.SQL = "UPDATE WF_GenerFH SET Title=" + SystemConfig.AppCenterDBVarStr + "Title WHERE FID=" + SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQLs(sql);
            }
            Emp emp1 = new Emp("admin");
            BP.Web.WebUser.SignInOfGener(emp1);

            return "ȫ�����ɳɹ�,Ӱ������(" + wks.Count + ")��";
        }
        /// <summary>
        /// ���̼��
        /// </summary>
        /// <returns></returns>
        public string DoDataManger()
        {
            PubClass.WinOpen(Glo.CCFlowAppPath + "WF/Rpt/OneFlow.aspx?FK_Flow=" + this.No + "&ExtType=StartFlow&RefNo=", 700, 500);
            return null;
        }
        /// <summary>
        /// �����̱���
        /// </summary>
        /// <returns></returns>
        public string DoFlowFormTree()
        {
            PubClass.WinOpen(Glo.CCFlowAppPath + "WF/Admin/FlowFormTree.aspx?s=d34&FK_Flow=" + this.No + "&ExtType=StartFlow&RefNo=" + DataType.CurrentDataTime, 700, 500);
            return null;
        }
        /// <summary>
        /// ���屨��
        /// </summary>
        /// <returns></returns>
        public string DoAutoStartIt()
        {
            Flow fl = new Flow();
            fl.No = this.No;
            fl.RetrieveFromDBSources();
            return fl.DoAutoStartIt();
        }
        /// <summary>
        /// ɾ������
        /// </summary>
        /// <param name="workid"></param>
        /// <param name="sd"></param>
        /// <returns></returns>
        public string DoDelDataOne(int workid, string note)
        {
            BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(this.No, workid, true);
            return "ɾ���ɹ� workid=" + workid + "  ����:" + note;
        }
        /// <summary>
        /// ���÷�������Դ
        /// </summary>
        /// <returns></returns>
        public string DoSetStartFlowDataSources()
        {
            string flowID = int.Parse(this.No).ToString() + "01";
            return Glo.CCFlowAppPath + "WF/MapDef/MapExt.aspx?s=d34&FK_MapData=ND" + flowID + "&ExtType=StartFlow&RefNo=";
        }
        public string DoCCNode()
        {
            PubClass.WinOpen(Glo.CCFlowAppPath + "WF/Admin/CCNode.aspx?FK_Flow=" + this.No, 400, 500);
            return null;
        }
        /// <summary>
        /// ִ������
        /// </summary>
        /// <returns></returns>
        public string DoRunIt()
        {
            return "/WF/Admin/TestFlow.aspx?FK_Flow=" + this.No + "&Lang=CH";
        }
        /// <summary>
        /// ִ�м��
        /// </summary>
        /// <returns></returns>
        public string DoCheck()
        {
            Flow fl = new Flow();
            fl.No = this.No;
            fl.RetrieveFromDBSources();
            return fl.DoCheck();
        }
        /// <summary>
        /// ִ������װ������
        /// </summary>
        /// <returns></returns>
        public string DoReloadRptData()
        {
            Flow fl = new Flow();
            fl.No = this.No;
            fl.RetrieveFromDBSources();
            return fl.DoReloadRptData();
        }
        /// <summary>
        /// ɾ������.
        /// </summary>
        /// <returns></returns>
        public string DoDelData()
        {
            Flow fl = new Flow();
            fl.No = this.No;
            fl.RetrieveFromDBSources();
            return fl.DoDelData();
        }
        /// <summary>
        /// �������ת��
        /// </summary>
        /// <returns></returns>
        public string DoExp()
        {
            Flow fl = new Flow();
            fl.No = this.No;
            fl.RetrieveFromDBSources();
            return fl.DoExp();
        }
        /// <summary>
        /// ���屨��
        /// </summary>
        /// <returns></returns>
        public string DoDRpt()
        {
            Flow fl = new Flow();
            fl.No = this.No;
            fl.RetrieveFromDBSources();
            return fl.DoDRpt();
        }
        /// <summary>
        /// ���б���
        /// </summary>
        /// <returns></returns>
        public string DoOpenRpt()
        {
            return Glo.CCFlowAppPath + "WF/Rpt/OneFlow.aspx?FK_Flow=" + this.No + "&DoType=Edit&FK_MapData=ND" +
                   int.Parse(this.No) + "Rpt";
        }
        /// <summary>
        /// ����֮������飬ҲҪ���»��档
        /// </summary>
        protected override void afterUpdate()
        {
            // Flow fl = new Flow();
            // fl.No = this.No;
            // fl.RetrieveFromDBSources();
            //fl.Update();

            if (Glo.OSModel == OSModel.BPM)
            {
                DBAccess.RunSQL("UPDATE  GPM_Menu SET Name='" + this.Name + "' WHERE Flag='Flow" + this.No + "' AND FK_App='" + SystemConfig.SysNo + "'");
            }
        }
        protected override bool beforeUpdate()
        {
            //�������̰汾
            Flow.UpdateVer(this.No);

            #region ͬ���¼�ʵ��.
            try
            {
                if (string.IsNullOrEmpty(this.FlowMark) == false)
                    this.FlowEventEntity = BP.WF.Glo.GetFlowEventEntityByFlowMark(this.FlowMark).ToString();
                else
                    this.FlowEventEntity = "";
            }
            catch
            {
                this.FlowEventEntity = "";
            }
            #endregion ͬ���¼�ʵ��.

            return base.beforeUpdate();
        }
        protected override void afterInsertUpdateAction()
        {
            //ͬ���������ݱ�.
            string ndxxRpt = "ND" + int.Parse(this.No) + "Rpt";
            Flow fl = new Flow(this.No);
            if (fl.PTable != "ND" + int.Parse(this.No) + "Rpt")
            {
                BP.Sys.MapData md = new Sys.MapData(ndxxRpt);
                if (md.PTable != fl.PTable)
                    md.Update();
            }
            base.afterInsertUpdateAction();
        }
                #endregion
    }
    /// <summary>
    /// ���̼���
    /// </summary>
    public class FlowSheets : EntitiesNoName
    {
        #region ��ѯ
        /// <summary>
        /// ��ѯ����ȫ�����������ڼ��ڵ�����
        /// </summary>
        /// <param name="FlowSort">�������</param>
        /// <param name="IsCountInLifeCycle">�ǲ��Ǽ����������ڼ��� true ��ѯ����ȫ���� </param>
        public void Retrieve(string FlowSort)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(BP.WF.Template.FlowAttr.FK_FlowSort, FlowSort);
            qo.addOrderBy(BP.WF.Template.FlowAttr.No);
            qo.DoQuery();
        }
        #endregion

        #region ���췽��
        /// <summary>
        /// ��������
        /// </summary>
        public FlowSheets() { }
        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="fk_sort"></param>
        public FlowSheets(string fk_sort)
        {
            this.Retrieve(BP.WF.Template.FlowAttr.FK_FlowSort, fk_sort);
        }
        #endregion

        #region �õ�ʵ��
        /// <summary>
        /// �õ����� Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FlowSheet();
            }
        }
        #endregion
    }
}
