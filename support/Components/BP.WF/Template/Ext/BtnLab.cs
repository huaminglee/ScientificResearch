using System;
using System.Collections;
using BP.DA;
using BP.Sys;
using BP.En;
using BP.WF.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// Btn
    /// </summary>
    public class BtnLab : Entity
    {
        /// <summary>
        /// but
        /// </summary>
        public static string Btns
        {
            get
            {
                return "Send,Save,Thread,Return,CC,Shift,Del,Rpt,Ath,Track,Opt,EndFlow,SubFlow";
            }
        }
        /// <summary>
        /// PK
        /// </summary>
        public override string PK
        {
            get
            {
                return NodeAttr.NodeID;
            }
        }

        #region 基本属性
        /// <summary>
        /// 节点ID
        /// </summary>
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(BtnAttr.NodeID);
            }
            set
            {
                this.SetValByKey(BtnAttr.NodeID, value);
            }
        }
        /// <summary>
        /// 查询标签
        /// </summary>
        public string SearchLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.SearchLab);
            }
            set
            {
                this.SetValByKey(BtnAttr.SearchLab, value);
            }
        }
        /// <summary>
        /// 查询是否可用
        /// </summary>
        public bool SearchEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.SearchEnable);
            }
            set
            {
                this.SetValByKey(BtnAttr.SearchEnable, value);
            }
        }
        /// <summary>
        /// 移交
        /// </summary>
        public string ShiftLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.ShiftLab);
            }
            set
            {
                this.SetValByKey(BtnAttr.ShiftLab, value);
            }
        }
        /// <summary>
        /// 是否启用移交
        /// </summary>
        public bool ShiftEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.ShiftEnable);
            }
            set
            {
                this.SetValByKey(BtnAttr.ShiftEnable, value);
            }
        }
        /// <summary>
        /// 选择接受人
        /// </summary>
        public string SelectAccepterLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.SelectAccepterLab);
            }
        }
        /// <summary>
        /// 选择接受人类型
        /// </summary>
        public int SelectAccepterEnable
        {
            get
            {
                return this.GetValIntByKey(BtnAttr.SelectAccepterEnable);
            }
        }
        /// <summary>
        /// 保存
        /// </summary>
        public string SaveLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.SaveLab);
            }
        }
        /// <summary>
        /// 是否启用保存
        /// </summary>
        public bool SaveEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.SaveEnable);
            }
        }
        /// <summary>
        /// 子线程按钮标签
        /// </summary>
        public string ThreadLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.ThreadLab);
            }
        }
        /// <summary>
        /// 子线程按钮是否启用
        /// </summary>
        public bool ThreadEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.ThreadEnable);
            }
        }

        /// <summary>
        /// 子流程按钮标签
        /// </summary>
        public string SubFlowLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.SubFlowLab);
            }
        }
        /// <summary>
        /// 子流程按钮是否启用
        /// </summary>
        public SubFlowCtrlRole SubFlowCtrlRole
        {
            get
            {
                return (SubFlowCtrlRole)this.GetValIntByKey(BtnAttr.SubFlowCtrlRole);
            }
        }
        /// <summary>
        /// 跳转标签
        /// </summary>
        public string JumpWayLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.JumpWayLab);
            }
        }
        public JumpWay JumpWayEnum
        {
            get
            {
                return (JumpWay)this.GetValIntByKey(NodeAttr.JumpWay);
            }
        }
        /// <summary>
        /// 是否启用跳转
        /// </summary>
        public bool JumpWayEnable
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.JumpWay);
            }
        }
        /// <summary>
        /// 退回标签
        /// </summary>
        public string ReturnLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.ReturnLab);
            }
        }
        /// <summary>
        /// 退回字段
        /// </summary>
        public string ReturnField
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.ReturnField);
            }
        }
        /// <summary>
        /// 跳转是否启用
        /// </summary>
        public bool ReturnEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.ReturnRole);
            }
        }
        /// <summary>
        /// 挂起标签
        /// </summary>
        public string HungLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.HungLab);
            }
        }
        /// <summary>
        /// 是否启用挂起
        /// </summary>
        public bool HungEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.HungEnable);
            }
        }
        /// <summary>
        /// 打印标签
        /// </summary>
        public string PrintDocLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.PrintDocLab);
            }
        }
        /// <summary>
        /// 是否启用打印
        /// </summary>
        public bool PrintDocEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.PrintDocEnable);
            }
        }
        /// <summary>
        /// 发送标签
        /// </summary>
        public string SendLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.SendLab);
            }
        }
        /// <summary>
        /// 是否启用发送?
        /// </summary>
        public bool SendEnable
        {
            get
            {
                return true;
            }
        }
        /// <summary>
        /// 发送的Js代码
        /// </summary>
        public string SendJS
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.SendJS).Replace("~", "'");
            }
        }
        /// <summary>
        /// 轨迹标签
        /// </summary>
        public string TrackLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.TrackLab);
            }
        }
        /// <summary>
        /// 是否启用轨迹
        /// </summary>
        public bool TrackEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.TrackEnable);
            }
        }
        /// <summary>
        /// 抄送标签
        /// </summary>
        public string CCLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.CCLab);
            }
        }
        /// <summary>
        /// 抄送规则
        /// </summary>
        public CCRole CCRole
        {
            get
            {
                return (CCRole)this.GetValIntByKey(BtnAttr.CCRole);
            }
        }
        /// <summary>
        /// 删除标签
        /// </summary>
        public string DeleteLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.DelLab);
            }
        }
        /// <summary>
        /// 删除类型
        /// </summary>
        public int DeleteEnable
        {
            get
            {
                return this.GetValIntByKey(BtnAttr.DelEnable);
            }
        }
        /// <summary>
        /// 结束流程
        /// </summary>
        public string EndFlowLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.EndFlowLab);
            }
        }
        /// <summary>
        /// 是否启用结束流程
        /// </summary>
        public bool EndFlowEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.EndFlowEnable);
            }
        }
          /// <summary>
        /// 是否启用流转自定义
        /// </summary>
        public string TCLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.TCLab);
            }
        }
        /// <summary>
        /// 是否启用流转自定义
        /// </summary>
        public bool TCEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.TCEnable);
            }
            set
            {
                this.SetValByKey(BtnAttr.TCEnable, value);
            }
        }
        
        /// <summary>
        /// 审核标签
        /// </summary>
        public string WorkCheckLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.WorkCheckLab);
            }
        }
        /// <summary>
        /// 审核是否可用
        /// </summary>
        public bool WorkCheckEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.WorkCheckEnable);
            }
            set
            {
                this.SetValByKey(BtnAttr.WorkCheckEnable, value);
            }
        }
        /// <summary>
        /// 批量处理是否可用
        /// </summary>
        public bool BatchEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.BatchEnable);
            }
        }
        /// <summary>
        /// 批处理标签
        /// </summary>
        public string BatchLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.BatchLab);
            }
        }
        /// <summary>
        /// 加签
        /// </summary>
        public bool AskforEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.AskforEnable);
            }
        }
        /// <summary>
        /// 加签
        /// </summary>
        public string AskforLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.AskforLab);
            }
        }
        /// <summary>
        ///是否启用文档
        /// </summary>
        public int WebOfficeEnable
        {
            get
            {
                return this.GetValIntByKey(BtnAttr.WebOfficeEnable);
            }
        }
        /// <summary>
        /// 文档按钮标签
        /// </summary>
        public string WebOfficeLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.WebOfficeLab);
            }
        }
        /// <summary>
        /// 打开本地文件
        /// </summary>
        public bool OfficeOpenEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeOpenEnable); }
        }
        /// <summary>
        /// 打开本地标签      
        /// </summary>
        public string OfficeOpenLab
        {
            get { return this.GetValStrByKey(BtnAttr.OfficeOpenLab); }
        }
        /// <summary>
        /// 打开模板
        /// </summary>
        public bool OfficeOpenTemplateEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeOpenTemplateEnable); }
        }
        /// <summary>
        /// 打开模板标签
        /// </summary>
        public string OfficeOpenTemplateLab
        {
            get { return this.GetValStringByKey(BtnAttr.OfficeOpenTemplateLab); }
        }
        /// <summary>
        /// 保存按钮
        /// </summary>
        public bool OfficeSaveEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeSaveEnable); }
        }
        /// <summary>
        /// 保存标签
        /// </summary>
        public string OfficeSaveLab
        {
            get { return this.GetValStringByKey(BtnAttr.OfficeSaveLab); }
        }
        /// <summary>
        /// 接受修订
        /// </summary>
        public bool OfficeAcceptEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeAcceptEnable); }
        }
        /// <summary>
        /// 接受修订标签
        /// </summary>
        public string OfficeAcceptLab
        {
            get { return this.GetValStringByKey(BtnAttr.OfficeAcceptLab); }
        }
        /// <summary>
        /// 拒绝修订
        /// </summary>
        public bool OfficeRefuseEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeRefuseEnable); }
        }
        /// <summary>
        /// 拒绝修订标签
        /// </summary>
        public string OfficeRefuseLab
        {
            get { return this.GetValStringByKey(BtnAttr.OfficeRefuseLab); }
        }
        /// <summary>
        /// 是否套红
        /// </summary>
        public bool OfficeOverEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeOverEnable); }
        }
        /// <summary>
        /// 套红按钮标签
        /// </summary>
        public string OfficeOVerLab
        {
            get { return this.GetValStringByKey(BtnAttr.OfficeOverLab); }
        }
        /// <summary>
        /// 是否打印
        /// </summary>
        public bool OfficePrintEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficePrintEnable); }
        }
        /// <summary>
        /// 是否查看用户留痕
        /// </summary>
        public bool OfficeMarksEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeMarksEnable); }
        }
        /// <summary>
        /// 打印按钮标签
        /// </summary>
        public string OfficePrintLab
        {
            get { return this.GetValStringByKey(BtnAttr.OfficePrintLab); }
        }
        /// <summary>
        /// 签章按钮
        /// </summary>
        public bool OfficeSealEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeSealEnable); }
        }
        /// <summary>
        /// 签章标签
        /// </summary>
        public string OfficeSealLab
        {
            get { return this.GetValStringByKey(BtnAttr.OfficeSealLab); }
        }

        /// <summary>
        ///插入流程
        /// </summary>
        public bool OfficeInsertFlowEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeInsertFlowEnable); }
        }
        /// <summary>
        /// 流程标签
        /// </summary>
        public string OfficeInsertFlowLab
        {
            get { return this.GetValStringByKey(BtnAttr.OfficeInsertFlowLab); }
        }

         
        /// <summary>
        /// 是否自动记录节点信息
        /// </summary>
        public bool OfficeNodeInfo
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeNodeInfo); }
        }

        /// <summary>
        /// 是否自动记录节点信息
        /// </summary>
        public bool OfficeReSavePDF
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeReSavePDF); }
        }

        /// <summary>
        /// 是否进入留痕模式
        /// </summary>
        public bool OfficeIsMarks
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeIsMarks); }
        }

        /// <summary>
        /// 下载按钮标签
        /// </summary>
        public String OfficeDownLab
        {
            get { return this.GetValStringByKey(BtnAttr.OfficeDownLab); }
        }

        /// <summary>
        /// 是否启用下载
        /// </summary>
        public bool OfficeDownEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeDownEnable); }
        }

        /// <summary>
        /// 指定文档模板
        /// </summary>
        public String OfficeTemplate
        {
            get { return this.GetValStringByKey(BtnAttr.OfficeTemplate); }
        }

 
        /// <summary>
        /// 是否使用父流程的文档
        /// </summary>
        public bool OfficeIsParent
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeIsParent); }
        }

        /// <summary>
        /// 是否自动套红
        /// </summary>
        public bool OfficeTHEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeTHEnable); }
        }
        /// <summary>
        /// 自动套红模板
        /// </summary>
        public string OfficeTHTemplate
        {
            get { return this.GetValStringByKey(BtnAttr.OfficeTHTemplate); }
        }

        #endregion

        #region 构造方法
        /// <summary>
        /// Btn
        /// </summary>
        public BtnLab() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeid"></param>
        public BtnLab(int nodeid)
        {
            this.NodeID = nodeid;
            this.RetrieveFromDBSources();
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

                Map map = new Map("WF_Node");
                map.EnDesc = "节点标签";

                map.DepositaryOfEntity = Depositary.Application;
                map.DepositaryOfMap = Depositary.Application;

                map.AddTBIntPK(BtnAttr.NodeID, 0, "NodeID", true, false);

                map.AddTBString(BtnAttr.SendLab, "发送", "发送按钮标签", true, false, 0, 50, 10);
                map.AddTBString(BtnAttr.SendJS, "", "按钮JS函数", true, false, 0, 50, 10);

                //map.AddBoolean(BtnAttr.SendEnable, true, "是否启用", true, true);

                map.AddTBString(BtnAttr.JumpWayLab, "跳转", "跳转按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(NodeAttr.JumpWay, false, "是否启用", true, true);


                map.AddTBString(BtnAttr.SaveLab, "保存", "保存按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.SaveEnable, true, "是否启用", true, true);


                map.AddTBString(BtnAttr.ThreadLab, "子线程", "子线程按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ThreadEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.SubFlowLab, "子流程", "子流程按钮标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.SubFlowCtrlRole, 0, "控制规则", true, true, BtnAttr.SubFlowCtrlRole, "@0=无@1=不可以删除子流程@2=可以删除子流程");


                map.AddTBString(BtnAttr.ReturnLab, "退回", "退回按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ReturnRole, true, "是否启用", true, true);
                map.AddTBString(BtnAttr.ReturnField, "", "退回信息填写字段", true, false, 0, 50, 10, true);


                map.AddTBString(BtnAttr.CCLab, "抄送", "抄送按钮标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.CCRole, 0, "抄送规则", true, true, BtnAttr.CCRole);

                //  map.AddBoolean(BtnAttr, true, "是否启用", true, true);

                map.AddTBString(BtnAttr.ShiftLab, "移交", "移交按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ShiftEnable, true, "是否启用", true, true);

                map.AddTBString(BtnAttr.DelLab, "删除流程", "删除流程按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.DelEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.EndFlowLab, "结束流程", "结束流程按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.EndFlowEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.HungLab, "挂起", "挂起按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.HungEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.PrintDocLab, "打印单据", "打印单据按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.PrintDocEnable, false, "是否启用", true, true);

                //map.AddTBString(BtnAttr.AthLab, "附件", "附件按钮标签", true, false, 0, 50, 10);
                //map.AddBoolean(BtnAttr.FJOpen, true, "是否启用", true, true);

                map.AddTBString(BtnAttr.TrackLab, "轨迹", "轨迹按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.TrackEnable, true, "是否启用", true, true);

                map.AddTBString(BtnAttr.SelectAccepterLab, "接受人", "接受人按钮标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.SelectAccepterEnable, 0, "方式",
          true, true, BtnAttr.SelectAccepterEnable);

                // map.AddBoolean(BtnAttr.SelectAccepterEnable, false, "是否启用", true, true);
                //map.AddTBString(BtnAttr.OptLab, "选项", "选项按钮标签", true, false, 0, 50, 10);
                //map.AddBoolean(BtnAttr.OptEnable, true, "是否启用", true, true);

                map.AddTBString(BtnAttr.SearchLab, "查询", "查询按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.SearchEnable, true, "是否启用", true, true);

                // 
                map.AddTBString(BtnAttr.WorkCheckLab, "审核", "审核按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.WorkCheckEnable, false, "是否启用", true, true);

                // 
                map.AddTBString(BtnAttr.BatchLab, "批量审核", "批量审核标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.BatchEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.AskforLab, "加签", "加签标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.AskforEnable, false, "是否启用", true, true);

                // add by stone 2014-11-21. 让用户可以自己定义流转.
                map.AddTBString(BtnAttr.TCLab, "流转自定义", "流转自定义", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.TCEnable, false, "是否启用", true, true);


                map.AddTBString(BtnAttr.WebOfficeLab, "公文", "公文标签", true, false, 0, 50, 10);
                //map.AddBoolean(BtnAttr.WebOfficeEnable, false, "是否启用", true, true);
                map.AddDDLSysEnum(BtnAttr.WebOfficeEnable, 0, "文档启用方式", true, true, BtnAttr.WebOfficeEnable,
                 "@0=不启用@1=按钮方式@2=标签页方式");

                #region 公文按钮
                map.AddTBString(BtnAttr.OfficeOpenLab, "打开本地", "打开本地标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeOpenEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.OfficeOpenTemplateLab, "打开模板", "打开模板标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeOpenTemplateEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.OfficeSaveLab, "保存", "保存标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeSaveEnable, true, "是否启用", true, true);

                map.AddTBString(BtnAttr.OfficeAcceptLab, "接受修订", "接受修订标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeAcceptEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.OfficeRefuseLab, "拒绝修订", "拒绝修订标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeRefuseEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.OfficeTHTemplate, "套红", "套红标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeOverEnable, false, "是否启用", true, true);

                map.AddBoolean(BtnAttr.OfficeMarksEnable, true, "是否查看用户留痕", true, true,true);

                map.AddTBString(BtnAttr.OfficePrintLab, "打印", "打印标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficePrintEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.OfficeSaveLab, "签章", "签章标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeSealEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.OfficeInsertFlowLab, "插入流程", "插入流程标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeInsertFlowEnable, false, "是否启用", true, true);

                map.AddBoolean(BtnAttr.OfficeNodeInfo, false, "是否记录节点信息", true, true);
                map.AddBoolean(BtnAttr.OfficeReSavePDF, false, "是否该自动保存为PDF", true, true);


                map.AddTBString(BtnAttr.OfficeDownLab, "下载", "下载按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeDownEnable, false, "是否启用", true, true);

                map.AddBoolean(BtnAttr.OfficeIsMarks, true, "是否进入留痕模式", true, true);
                map.AddTBString(BtnAttr.OfficeTemplate, "", "指定文档模板", true, false, 0, 100, 10);
                map.AddBoolean(BtnAttr.OfficeIsParent, true, "是否使用父流程的文档", true, true);
 
                map.AddBoolean(BtnAttr.OfficeTHEnable, false, "是否自动套红", true, true);
                map.AddTBString(BtnAttr.OfficeTHTemplate, "", "自动套红模板", true, false, 0, 200, 10);

                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// Btn
    /// </summary>
    public class BtnLabs : Entities
    {
        /// <summary>
        /// Btn
        /// </summary>
        public BtnLabs()
        {
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new BtnLab();
            }
        }
    }
}
