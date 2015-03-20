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
    /// 这里存放每个节点的信息.
    /// </summary>
    public class NodeSheet : Entity
    {
        #region Index
        /// <summary>
        /// 获取节点的帮助信息url
        /// <para></para>
        /// <para>added by liuxc,2014-8-19</para> 
        /// </summary>
        /// <param name="sysNo">帮助网站中所属系统No</param>
        /// <param name="searchTitle">帮助主题标题</param>
        /// <returns></returns>
        private string this[string sysNo, string searchTitle]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(sysNo) || string.IsNullOrWhiteSpace(searchTitle))
                    return "javascript:alert('此处还没有帮助信息！')";

                return string.Format("http://online.ccflow.org/KM/Tree.aspx?no={0}&st={1}", sysNo, Uri.EscapeDataString(searchTitle));
            }
        }
        #endregion

        #region Const
        /// <summary>
        /// CCFlow流程引擎
        /// </summary>
        private const string SYS_CCFLOW = "001";
        /// <summary>
        /// CCForm表单引擎
        /// </summary>
        private const string SYS_CCFORM = "002";
        #endregion

        #region 属性.
        ///// <summary>
        ///// 节点标记
        ///// </summary>
        //public string NodeMark
        //{
        //    get
        //    {
        //        return this.GetValStrByKey(NodeAttr.NodeMark);
        //    }
        //}

        /// <summary>
        /// 超时处理方式
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
        /// 访问规则
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
        /// 访问规则
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
        /// 超时处理内容
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
        /// 超时处理条件
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
        /// 接受人sql
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
        /// 是否可以退回
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
        #endregion 属性.

        #region 初试化全局的 Node
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

        #region 构造函数
        /// <summary>
        /// 节点
        /// </summary>
        public NodeSheet() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map();
                //map 的基础信息.
                map.PhysicsTable = "WF_Node";
                map.EnDesc = "节点";
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;

                #region  基础属性
                map.AddTBIntPK(NodeAttr.NodeID, 0, "节点ID", true, true);
                map.AddTBInt(NodeAttr.Step, 0, "步骤(无计算意义)", true, false);
                map.SetHelperAlert(NodeAttr.Step, "它用于节点的排序，正确的设置步骤可以让流程容易读写."); //使用alert的方式显示帮助信息.
                map.AddTBString(NodeAttr.FK_Flow, null, "流程编号", false, false, 3, 3, 10, false);

                map.AddTBString(NodeAttr.Name, null, "名称", true, true, 0, 100, 10, true);

                string str = "";
                str += "@0=01.按当前操作员所属组织结构逐级查找岗位";
                str += "@1=02.按节点绑定的部门计算";
                str += "@2=03.按设置的SQL获取接受人计算";
                str += "@3=04.按节点绑定的人员计算";
                str += "@4=05.由上一节点发送人通过“人员选择器”选择接受人";
                str += "@5=06.按上一节点表单指定的字段值作为本步骤的接受人";
                str += "@6=07.与上一节点处理人员相同";
                str += "@7=08.与开始节点处理人相同";
                str += "@8=09.与指定节点处理人相同";
                str += "@9=10.按绑定的岗位与部门交集计算";
                str += "@10=11.按绑定的岗位计算并且以绑定的部门集合为纬度";
                str += "@11=12.按指定节点的人员岗位计算";
                str += "@12=13.按SQL确定子线程接受人与数据源";
                str += "@13=14.由上一节点的明细表来决定子线程的接受人";
                str += "@14=15.仅按绑定的岗位计算";
                str += "@15=16.由FEE来决定";

                str += "@100=16.按ccflow的BPM模式处理";
                map.AddDDLSysEnum(NodeAttr.DeliveryWay, 0, "节点访问规则", true, true, NodeAttr.DeliveryWay,str);

                map.AddTBString(NodeAttr.DeliveryParas, null, "访问规则设置内容", true, false, 0, 500, 10, true);
                map.AddDDLSysEnum(NodeAttr.WhoExeIt, 0, "谁执行它",true, true, NodeAttr.WhoExeIt, "@0=操作员执行@1=机器执行@2=混合执行");
                map.AddDDLSysEnum(NodeAttr.TurnToDeal, 0, "发送后转向",
                 true, true, NodeAttr.TurnToDeal, "@0=提示ccflow默认信息@1=提示指定信息@2=转向指定的url@3=按照条件转向");
                map.AddTBString(NodeAttr.TurnToDealDoc, null, "转向处理内容", true, false, 0, 1000, 10, true);
                map.AddDDLSysEnum(NodeAttr.ReadReceipts, 0, "已读回执", true, true, NodeAttr.ReadReceipts,
                    "@0=不回执@1=自动回执@2=由上一节点表单字段决定@3=由SDK开发者参数决定");
                map.SetHelperUrl(NodeAttr.ReadReceipts, this[SYS_CCFLOW, "已读回执"]);

                map.AddDDLSysEnum(NodeAttr.CondModel, 0, "方向条件控制规则", true, true, NodeAttr.CondModel,
                 "@0=由连接线条件控制@1=让用户手工选择");
                map.SetHelperUrl(NodeAttr.CondModel, this[SYS_CCFLOW, "方向条件控制规则"]); //增加帮助

                // 撤销规则.
                map.AddDDLSysEnum(NodeAttr.CancelRole,(int)CancelRole.OnlyNextStep, "撤销规则", true, true,
                    NodeAttr.CancelRole,"@0=上一步可以撤销@1=不能撤销@2=上一步与开始节点可以撤销@3=指定的节点可以撤销");

                // 节点工作批处理. edit by peng, 2014-01-24.
                map.AddDDLSysEnum(NodeAttr.BatchRole, (int)BatchRole.None, "工作批处理", true, true, NodeAttr.BatchRole, "@0=不可以批处理@1=批量审核@2=分组批量审核");
                map.AddTBInt(NodeAttr.BatchListCount, 12, "批处理数量", true, false);
                map.SetHelperUrl(NodeAttr.BatchRole, this[SYS_CCFLOW, "节点工作批处理"]); //增加帮助
                map.AddTBString(NodeAttr.BatchParas, null, "批处理参数", true, false, 0, 300, 10, true);


                map.AddBoolean(NodeAttr.IsTask, true, "允许分配工作否?", true, true, false);
                map.SetHelperBaidu(NodeAttr.IsTask); //链接到baidu搜索.
                map.AddBoolean(NodeAttr.IsRM, true, "是否启用投递路径自动记忆功能?", true, true, false);
                map.SetHelperBaidu(NodeAttr.IsRM); //链接到baidu搜索.

                map.AddTBDateTime("DTFrom", "生命周期从", true, true);
                map.AddTBDateTime("DTTo", "生命周期到", true, true);
                #endregion  基础属性

                #region 表单.
                map.AddDDLSysEnum(NodeAttr.FormType, (int)NodeFormType.FreeForm, "节点表单方案", true, true,
                 "NodeFormType", "@0=傻瓜表单(ccflow6取消支持)@1=自由表单@2=自定义表单@3=SDK表单@4=SL表单(ccflow6取消支持)@5=表单树@6=动态表单树@7=公文表单(WebOffice)@8=Excel表单(测试中)@9=Word表单(测试中)@100=禁用(对多表单流程有效)");
                map.AddTBString(NodeAttr.FormUrl, null, "表单URL", true, false, 0, 200, 10, true);

                map.AddTBString(NodeAttr.FocusField, null, "焦点字段", true, false, 0, 50, 10, true);
                map.SetHelperBaidu(NodeAttr.FocusField); //链接到baidu搜索.

                map.AddTBString(NodeAttr.NodeFrmID, null, "节点表单ID", true, false, 0, 50, 10);
                map.AddDDLSysEnum(NodeAttr.SaveModel, 0, "保存方式", true, true);
                #endregion 表单.

                #region 分合流子线程属性
                map.AddDDLSysEnum(NodeAttr.RunModel, 0, "运行模式",
                    true, true, NodeAttr.RunModel, "@0=普通@1=合流@2=分流@3=分合流@4=子线程");
                map.SetHelperUrl(NodeAttr.RunModel, this[SYS_CCFLOW, "运行模式"]); //增加帮助.
        
                
                //子线程类型.
                map.AddDDLSysEnum(NodeAttr.SubThreadType, 0, "子线程类型", true, true, NodeAttr.SubThreadType, "@0=同表单@1=异表单");
                map.SetHelperUrl(NodeAttr.SubThreadType, this[SYS_CCFLOW, "子线程类型"]); //增加帮助


                map.AddTBDecimal(NodeAttr.PassRate, 0, "完成通过率", true, false);
                map.SetHelperUrl(NodeAttr.PassRate, this[SYS_CCFLOW, "完成通过率"]); //增加帮助.

                // 启动子线程参数 2013-01-04
                map.AddDDLSysEnum(NodeAttr.SubFlowStartWay, (int)SubFlowStartWay.None, "子线程启动方式", true, true,
                    NodeAttr.SubFlowStartWay, "@0=不启动@1=指定的字段启动@2=按明细表启动");
                map.AddTBString(NodeAttr.SubFlowStartParas, null, "启动参数", true, false, 0, 100, 10, true);
                map.SetHelperUrl(NodeAttr.SubFlowStartWay, this[SYS_CCFLOW, "子线程启动方式"]); //增加帮助

                //待办处理模式.
                map.AddDDLSysEnum(NodeAttr.TodolistModel, (int)TodolistModel.QiangBan, "待办处理模式", true, true, NodeAttr.TodolistModel,
                    "@0=抢办模式@1=协作模式@2=队列模式@3=共享模式");
                map.SetHelperUrl(NodeAttr.TodolistModel, this[SYS_CCFLOW, "待办处理模式"]); //增加帮助.


                //发送阻塞模式.
                map.AddDDLSysEnum(NodeAttr.BlockModel, (int)BlockModel.None, "发送阻塞模式", true, true, NodeAttr.BlockModel,
                    "@0=不阻塞@1=当前节点的所有未完成的子流程@2=按约定格式阻塞未完成子流程@3=按照SQL阻塞@4=按照表达式阻塞");
                map.SetHelperUrl(NodeAttr.BlockModel, this[SYS_CCFLOW, "待办处理模式"]); //增加帮助.

                map.AddTBString(NodeAttr.BlockExp, null, "阻塞表达式", true, false, 0, 700, 10,true);
                map.SetHelperAlert(NodeAttr.BlockExp,"该属性配合发送阻塞模式属性起作用");

                map.AddTBString(NodeAttr.BlockAlert, null, "被阻塞时提示信息", true, false, 0, 700, 10, true);
                map.SetHelperAlert(NodeAttr.BlockAlert, "该属性配合发送阻塞模式属性起作用,编写支持cc表达式.");

                //map.AddBoolean(NodeAttr.IsCheckSubFlowOver, false, "(当前节点启动子流程时)是否检查所有子流程结束后,该节点才能向下发送?",
               //true, true, true);

                ////  add 2013-09-14 
                //map.AddBoolean(NodeAttr.IsEnableTaskPool, true,
                //    "是否启用共享任务池(与web.config中的IsEnableTaskPool配置启用才有效,与子线程无关)？", true, true, true);
                //map.SetHelperBaidu(NodeAttr.IsEnableTaskPool); //增加帮助.
              

                map.AddBoolean(NodeAttr.IsAllowRepeatEmps, false, "是否允许子线程接受人员重复(仅当分流点向子线程发送时有效)?", true, true, true);
                map.AddBoolean(NodeAttr.IsGuestNode, false, "是否是客户执行节点(非组织结构人员参与处理工作的节点)?", true, true, true);
                #endregion 分合流子线程属性

                #region 自动跳转规则
                map.AddBoolean(NodeAttr.AutoJumpRole0, false, "处理人就是发起人", true, true, false);
                map.SetHelperUrl(NodeAttr.AutoJumpRole0, this[SYS_CCFLOW, "自动跳转规则"]); //增加帮助

                map.AddBoolean(NodeAttr.AutoJumpRole1, false, "处理人已经出现过", true, true, false);
                map.AddBoolean(NodeAttr.AutoJumpRole2, false, "处理人与上一步相同", true, true, false);
                map.AddDDLSysEnum(NodeAttr.WhenNoWorker, 0, "找不到处理人处理规则",
       true, true, NodeAttr.WhenNoWorker, "@0=提示错误@1=自动转到下一步");
                #endregion

                #region  功能按钮状态
                map.AddTBString(BtnAttr.SendLab, "发送", "发送按钮标签", true, false, 0, 50, 10);
                map.AddTBString(BtnAttr.SendJS, "", "按钮JS函数", true, false, 0, 50, 10);
                //map.SetHelperBaidu(BtnAttr.SendJS, "ccflow 发送前数据完整性判断"); //增加帮助.
                map.SetHelperUrl(BtnAttr.SendJS, this[SYS_CCFLOW, "按钮JS函数"]);

                map.AddTBString(BtnAttr.SaveLab, "保存", "保存按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.SaveEnable, true, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.SaveLab, this[SYS_CCFLOW, "保存"]); //增加帮助

                map.AddTBString(BtnAttr.ThreadLab, "子线程", "子线程按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ThreadEnable, false, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.ThreadLab, this[SYS_CCFLOW, "子线程按钮标签"]); //增加帮助


                map.AddDDLSysEnum(NodeAttr.ThreadKillRole, (int)ThreadKillRole.None, "子线程删除方式", true, true,
           NodeAttr.ThreadKillRole, "@0=不能删除@1=手工删除@2=自动删除",true);
                map.SetHelperUrl(NodeAttr.ThreadKillRole, this[SYS_CCFLOW, "子线程删除方式"]); //增加帮助
               

                map.AddTBString(BtnAttr.SubFlowLab, "子流程", "子流程按钮标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.SubFlowCtrlRole, 0, "控制规则", true, true, BtnAttr.SubFlowCtrlRole, "@0=无@1=不可以删除子流程@2=可以删除子流程");

                map.AddTBString(BtnAttr.JumpWayLab, "跳转", "跳转按钮标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(NodeAttr.JumpWay, 0, "跳转规则", true, true, NodeAttr.JumpWay);
                map.AddTBString(NodeAttr.JumpToNodes, null, "可跳转的节点", true, false, 0, 200, 10, true);
                map.SetHelperUrl(NodeAttr.JumpWay, this[SYS_CCFLOW, "跳转规则"]); //增加帮助.

                map.AddTBString(BtnAttr.ReturnLab, "退回", "退回按钮标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(NodeAttr.ReturnRole, 0,"退回规则",true, true, NodeAttr.ReturnRole);
              //  map.AddTBString(NodeAttr.ReturnToNodes, null, "可退回节点", true, false, 0, 200, 10, true);
                map.SetHelperUrl(NodeAttr.ReturnRole, this[SYS_CCFLOW, "222"]); //增加帮助.

                map.AddBoolean(NodeAttr.IsBackTracking, false, "是否可以原路返回(启用退回功能才有效)", true, true, false);
                map.AddTBString(BtnAttr.ReturnField, "", "退回信息填写字段", true, false, 0, 50, 10);
                map.SetHelperUrl(NodeAttr.IsBackTracking, this[SYS_CCFLOW, "是否可以原路返回"]); //增加帮助.

                map.AddTBString(BtnAttr.CCLab, "抄送", "抄送按钮标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(NodeAttr.CCRole, 0, "抄送规则", true, true, NodeAttr.CCRole);
                map.SetHelperUrl(NodeAttr.CCRole, this[SYS_CCFLOW, "抄送规则"]); //增加帮助.

                // add 2014-04-05.
                map.AddDDLSysEnum(NodeAttr.CCWriteTo, 0, "抄送写入规则",
             true, true, NodeAttr.CCWriteTo, "@0=写入抄送列表@1=写入待办@2=写入待办与抄送列表", true);
                map.SetHelperUrl(NodeAttr.CCWriteTo, this[SYS_CCFLOW, "抄送写入规则"]); //增加帮助

                map.AddTBString(BtnAttr.ShiftLab, "移交", "移交按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ShiftEnable, false, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.ShiftLab, this[SYS_CCFLOW, "移交"]); //增加帮助.note:none

                map.AddTBString(BtnAttr.DelLab, "删除", "删除按钮标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.DelEnable, 0, "删除规则", true, true, BtnAttr.DelEnable);
                map.SetHelperUrl(BtnAttr.DelLab, this[SYS_CCFLOW, "删除"]); //增加帮助.

                map.AddTBString(BtnAttr.EndFlowLab, "结束流程", "结束流程按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.EndFlowEnable, false, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.EndFlowLab, this[SYS_CCFLOW, "结束流程"]); //增加帮助

                map.AddTBString(BtnAttr.PrintDocLab, "打印单据", "打印单据按钮标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.PrintDocEnable, 0, "打印方式", true,
                    true, BtnAttr.PrintDocEnable, "@0=不打印@1=打印网页@2=打印RTF模板@3=打印Word模版");
                map.SetHelperUrl(BtnAttr.PrintDocEnable, this[SYS_CCFLOW, "表单打印方式"]); //增加帮助

                // map.AddBoolean(BtnAttr.PrintDocEnable, false, "是否启用", true, true);
                //map.AddTBString(BtnAttr.AthLab, "附件", "附件按钮标签", true, false, 0, 50, 10);
                //map.AddDDLSysEnum(NodeAttr.FJOpen, 0, this.ToE("FJOpen", "附件权限"), true, true, 
                //    NodeAttr.FJOpen, "@0=关闭附件@1=操作员@2=工作ID@3=流程ID");

                map.AddTBString(BtnAttr.TrackLab, "轨迹", "轨迹按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.TrackEnable, true, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.TrackLab, this[SYS_CCFLOW, "轨迹"]); //增加帮助


                map.AddTBString(BtnAttr.HungLab, "挂起", "挂起按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.HungEnable, false, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.HungLab, this[SYS_CCFLOW, "挂起"]); //增加帮助.

                map.AddTBString(BtnAttr.SelectAccepterLab, "接受人", "接受人按钮标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.SelectAccepterEnable, 0, "工作方式",
          true, true, BtnAttr.SelectAccepterEnable);
                map.SetHelperUrl(BtnAttr.SelectAccepterLab, this[SYS_CCFLOW, "接受人"]); //增加帮助


                map.AddTBString(BtnAttr.SearchLab, "查询", "查询按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.SearchEnable, false, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.SearchLab, this[SYS_CCFLOW, "查询"]); //增加帮助


                map.AddTBString(BtnAttr.WorkCheckLab, "审核", "审核按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.WorkCheckEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.BatchLab, "批处理", "批处理按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.BatchEnable, false, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.BatchLab, this[SYS_CCFLOW, "批处理"]); //增加帮助

                map.AddTBString(BtnAttr.AskforLab, "加签", "加签按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.AskforEnable, false, "是否启用", true, true);

                // add by 周朋 2014-11-21. 让用户可以自己定义流转.
                map.AddTBString(BtnAttr.TCLab, "流转自定义", "流转自定义", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.TCEnable, false, "是否启用", true, true);



                //map.AddTBString(BtnAttr.AskforLabRe, "执行", "加签按钮标签", true, false, 0, 50, 10);
                //map.AddBoolean(BtnAttr.AskforEnable, false, "是否启用", true, true);

                map.SetHelperUrl(BtnAttr.AskforLab, this[SYS_CCFLOW, "加签"]); //增加帮助
                map.AddTBString(BtnAttr.WebOfficeLab, "公文", "文档按钮标签", true, false, 0, 50, 10);
                //  map.AddBoolean(BtnAttr.WebOfficeEnable, false, "是否启用", true, true);
                map.AddDDLSysEnum(BtnAttr.WebOfficeEnable, 0, "文档启用方式", true, true, BtnAttr.WebOfficeEnable,
                  "@0=不启用@1=按钮方式@2=标签页方式");
                map.SetHelperUrl(BtnAttr.WebOfficeLab, this[SYS_CCFLOW, "公文"]);

                //map.AddBoolean(BtnAttr.SelectAccepterEnable, false, "是否启用", true, true);
                #endregion  功能按钮状态

                #region 考核属性
                // 考核属性
                map.AddTBFloat(NodeAttr.WarningDays, 0, "警告期限(0不警告)", true, false); // "警告期限(0不警告)"
                map.AddTBFloat(NodeAttr.DeductDays, 1, "限期(天)", true, false); //"限期(天)"
                map.AddTBFloat(NodeAttr.DeductCent, 2, "扣分(每延期1天扣)", true, false); //"扣分(每延期1天扣)"

                map.AddTBFloat(NodeAttr.MaxDeductCent, 0, "最高扣分", true, false);   //"最高扣分"
                map.AddTBFloat(NodeAttr.SwinkCent, float.Parse("0.1"), "工作得分", true, false); //"工作得分"
                map.AddDDLSysEnum(NodeAttr.OutTimeDeal, 0, "超时处理",
                true, true, NodeAttr.OutTimeDeal,
                "@0=不处理@1=自动向下运动(或运动到指定节点)@2=自动跳转指定的点@3=自动转到指定的人员@4=向指定的人员发消息@5=删除流程@6=执行SQL");

                map.AddTBString(NodeAttr.DoOutTime, null, "处理内容", true, false, 0, 300, 10, true);
                map.AddTBString(NodeAttr.DoOutTimeCond, null, "执行超时条件", true, false, 0, 100, 10, true);

                //map.AddTBString(NodeAttr.FK_Flows, null, "flow", false, false, 0, 100, 10);

                map.AddDDLSysEnum(NodeAttr.CHWay, 0, "考核方式", true, true, NodeAttr.CHWay, "@0=不考核@1=按时效@2=按工作量");
                map.AddTBFloat(NodeAttr.Workload, 0, "工作量(单位:分钟)", true, false);

                // 是否质量考核点？
                map.AddBoolean(NodeAttr.IsEval, false, "是否质量考核点", true, true, true);
                #endregion 考核属性

                #region 审核组件属性, 此处变更了BP.Sys.FrmWorkCheck 也要变更.
                // BP.Sys.FrmWorkCheck
                map.AddDDLSysEnum(FrmWorkCheckAttr.FWCSta, (int)FrmWorkCheckSta.Disable, "审核组件状态",
                    true, true, FrmWorkCheckAttr.FWCSta, "@0=禁用@1=启用@2=只读");

                map.AddDDLSysEnum(FrmWorkCheckAttr.FWCShowModel, (int)FrmWorkShowModel.Free, "显示方式",
                    true, true, FrmWorkCheckAttr.FWCShowModel, "@0=表格方式@1=自由模式"); //此属性暂时没有用.

                map.AddDDLSysEnum(FrmWorkCheckAttr.FWCType, (int)FWCType.Check, "审核组件工作方式", true, true, FrmWorkCheckAttr.FWCType, "@0=审核组件@1=日志组件");

                map.AddBoolean(FrmWorkCheckAttr.FWCTrackEnable, true, "轨迹图是否显示？", true, true, true);
                map.AddBoolean(FrmWorkCheckAttr.FWCListEnable, true, "历史审核信息是否显示？(否,历史信息仅出现意见框)", true, true, true);

                map.AddBoolean(FrmWorkCheckAttr.FWCIsShowAllStep, false, "在轨迹表里是否显示所有的步骤？", true, true);

                map.AddTBString(FrmWorkCheckAttr.FWCOpLabel, "审核", "操作名词(审核/审阅/批示)", true, false, 0, 50, 10);
                map.AddTBString(FrmWorkCheckAttr.FWCDefInfo, "同意", "默认审核信息", true, false, 0, 50, 10);
                map.AddBoolean(FrmWorkCheckAttr.SigantureEnabel, false, "操作人是否显示为图片签名？", true, true);
                map.AddBoolean(FrmWorkCheckAttr.FWCIsFullInfo, true, "如果用户未审核是否按照默认意见填充？", true, true, true);

                //map.AddTBFloat(FrmWorkCheckAttr.FWC_X, 5, "位置X", true, false);
                //map.AddTBFloat(FrmWorkCheckAttr.FWC_Y, 5, "位置Y", true, false);

                
                // 高度与宽度, 如果是自由表单就不要变化该属性.
                map.AddTBFloat(FrmWorkCheckAttr.FWC_H, 300, "高度", true, false);
                map.SetHelperAlert(FrmWorkCheckAttr.FWC_H, "如果是自由表单就不要变化该属性,为0，则标识为100%,应用的组件模式."); //增加帮助
                map.AddTBFloat(FrmWorkCheckAttr.FWC_W, 400, "宽度", true, false);
                map.SetHelperAlert(FrmWorkCheckAttr.FWC_W, "如果是自由表单就不要变化该属性,为0，则标识为100%,应用的组件模式."); //增加帮助
                #endregion 审核组件属性.

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

                map.AddTBString(BtnAttr.OfficeOverLab, "套红", "套红按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeOverEnable, false, "是否启用", true, true);

                map.AddBoolean(BtnAttr.OfficeMarksEnable, true, "是否查看用户留痕", true, true,true);

                map.AddTBString(BtnAttr.OfficePrintEnable, "打印", "打印按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficePrintEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.OfficeSaveLab, "签章", "签章按钮标签", true, false, 0, 50, 10);
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

                #region 移动设置.
                map.AddDDLSysEnum(NodeAttr.MPhone_WorkModel, 0, "手机工作模式", true, true, NodeAttr.MPhone_WorkModel, "@0=原生态@1=浏览器@2=禁用");
                map.AddDDLSysEnum(NodeAttr.MPhone_SrcModel, 0, "手机屏幕模式", true, true, NodeAttr.MPhone_SrcModel, "@0=强制横屏@1=强制竖屏@2=由重力感应决定");

                map.AddDDLSysEnum(NodeAttr.MPad_WorkModel, 0, "平板工作模式", true, true, NodeAttr.MPad_WorkModel, "@0=原生态@1=浏览器@2=禁用");
                map.AddDDLSysEnum(NodeAttr.MPad_SrcModel, 0, "平板屏幕模式", true, true, NodeAttr.MPad_SrcModel, "@0=强制横屏@1=强制竖屏@2=由重力感应决定");
                map.SetHelperUrl(NodeAttr.MPhone_WorkModel, "http://bbs.ccflow.org/showtopic-2866.aspx");
                #endregion 移动设置.

                //节点工具栏
                map.AddDtl(new NodeToolbars(), NodeToolbarAttr.FK_Node);

                #region 对应关系
                // 相关功能。
                if (Glo.OSModel == OSModel.WorkFlow)
                {
                    map.AttrsOfOneVSM.Add(new BP.WF.Template.NodeStations(), new BP.WF.Port.Stations(),
                        NodeStationAttr.FK_Node, NodeStationAttr.FK_Station,
                        DeptAttr.Name, DeptAttr.No, "节点绑定岗位");

                    //判断是否为集团使用，集团时打开新页面以树形展示
                    if (Glo.IsUnit == true)
                    {
                        RefMethod rmDept = new RefMethod();
                        rmDept.Title = "节点绑定部门";
                        rmDept.ClassMethodName = this.ToString() + ".DoDepts";
                        rmDept.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                        map.AddRefMethod(rmDept);
                    }
                    else
                    {
                        map.AttrsOfOneVSM.Add(new BP.WF.Template.NodeDepts(), new BP.WF.Port.Depts(), NodeDeptAttr.FK_Node, NodeDeptAttr.FK_Dept, DeptAttr.Name,
            DeptAttr.No, "节点绑定部门");
                    }
                }
                else
                {
                    //节点岗位.
                    map.AttrsOfOneVSM.Add(new BP.WF.Template.NodeStations(),
                        new BP.GPM.Stations(),
                      NodeStationAttr.FK_Node, NodeStationAttr.FK_Station,
                      DeptAttr.Name, DeptAttr.No, "节点绑定岗位");
                    //判断是否为集团使用，集团时打开新页面以树形展示
                    if (Glo.IsUnit == true)
                    {
                        RefMethod rmDept = new RefMethod();
                        rmDept.Title = "节点绑定部门";
                        rmDept.ClassMethodName = this.ToString() + ".DoDepts";
                        rmDept.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                        map.AddRefMethod(rmDept);
                    }
                    else
                    {
                        //节点部门.
                        map.AttrsOfOneVSM.Add(new BP.WF.Template.NodeDepts(), new BP.GPM.Depts(),
                            NodeDeptAttr.FK_Node, NodeDeptAttr.FK_Dept, DeptAttr.Name,
            DeptAttr.No, "节点绑定部门");
                    }
                }


                map.AttrsOfOneVSM.Add(new BP.WF.Template.NodeEmps(), new BP.WF.Port.Emps(), NodeEmpAttr.FK_Node, EmpDeptAttr.FK_Emp, DeptAttr.Name,
                    DeptAttr.No, "节点绑定接受人");

                // 傻瓜表单可以调用的子流程. 2014.10.19 去掉.
                //map.AttrsOfOneVSM.Add(new BP.WF.NodeFlows(), new Flows(), NodeFlowAttr.FK_Node, NodeFlowAttr.FK_Flow, DeptAttr.Name, DeptAttr.No,
                //    "傻瓜表单可调用的子流程");
                #endregion

                RefMethod rm = new RefMethod();
                rm.Title = "可退回的节点(当退回规则设置可退回指定的节点时,该设置有效.)"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoCanReturnNodes";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkModel;
                //设置相关字段.
                rm.RefAttrKey = NodeAttr.ReturnRole;
                rm.RefAttrLinkLabel = "设置可退回的节点";
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "可撤销发送节点(只有撤销规则是指定的节点可以撤销时,该设置有效.)"; // "可撤销发送的节点";
                rm.ClassMethodName = this.ToString() + ".DoCanCancelNodes";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                //设置相关字段.
                rm.RefAttrKey = NodeAttr.CancelRole;
                rm.RefAttrLinkLabel = "";
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "设置自动抄送规则(当节点为自动抄送时,该设置有效.)"; // "抄送规则";
                rm.ClassMethodName = this.ToString() + ".DoCCRole";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                //设置相关字段.
                rm.RefAttrKey = NodeAttr.CCRole;
                rm.RefAttrLinkLabel = "自动抄送设置";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "设计傻瓜表单(当节点表单类型设置为傻瓜表单时,该设置有效.)"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoFormCol4";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                //设置相关字段.
                rm.RefAttrKey = NodeAttr.SaveModel;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "设计自由表单(当节点表单类型设置为自由表单时,该设置有效.)"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoFormFree";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                //设置相关字段.
                rm.RefAttrKey = NodeAttr.SaveModel;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "绑定流程表单"; // "设计表单"; (当节点表单类型设置为树形表单时,该设置有效.)
                rm.ClassMethodName = this.ToString() + ".DoFormTree";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
              //   rm.Title
                //设置相关字段.
                rm.RefAttrKey = NodeAttr.SaveModel;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);
                

                rm = new RefMethod();
                rm.Title = "绑定rtf打印格式模版(当打印方式为打印RTF格式模版时,该设置有效)"; //"单据&单据";
                rm.ClassMethodName = this.ToString() + ".DoBill";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/FileType/doc.gif";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;

                //设置相关字段.
                rm.RefAttrKey = NodeAttr.PrintDocEnable;
                rm.RefAttrLinkLabel = "";
                rm.Target = "_blank";
                map.AddRefMethod(rm);
                if (BP.Sys.SystemConfig.CustomerNo == "HCBD")
                {
                    /* 为海成邦达设置的个性化需求. */
                    rm = new RefMethod();
                    rm.Title = "DXReport设置";
                    rm.ClassMethodName = this.ToString() + ".DXReport";
                    rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/FileType/doc.gif";
                    map.AddRefMethod(rm);
                }

                rm = new RefMethod();
                rm.Title = "设置事件"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoAction";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Event.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "向当事人推送消息"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoPush2Current";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Message24.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
              //  map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "向指定人推送消息"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoPush2Spec";
              //  rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Message32.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
              //  map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "消息收听"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoListen";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "流程完成条件"; // "流程完成条件";
                rm.ClassMethodName = this.ToString() + ".DoCond";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);
             

                rm = new RefMethod();
                rm.Title = "发送成功转向条件"; // "转向条件";
                rm.ClassMethodName = this.ToString() + ".DoTurn";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                //设置相关字段.
                rm.RefAttrKey = NodeAttr.TurnToDealDoc;
                rm.RefAttrLinkLabel = "";
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "设置“接受人选择器”的人员选择范围。"; // "个性化接受人窗口"; //(访问规则设置为第05项有效)
                rm.ClassMethodName = this.ToString() + ".DoAccepter";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                //设置相关字段.
                rm.RefAttrKey = NodeAttr.DeliveryWay;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.RefAttrLinkLabel = "";
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                if (Glo.OSModel==OSModel.BPM)
                {
                    rm = new RefMethod();
                    rm.Title = "BPM模式接受人设置规则";
                    rm.ClassMethodName = this.ToString() + ".DoAccepterRole";
                    rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";

                    //设置相关字段.
                    //rm.RefAttrKey = NodeAttr.WhoExeIt;
                    rm.RefMethodType = RefMethodType.RightFrameOpen;
                    rm.RefAttrLinkLabel = "";
                    rm.Target = "_blank";
                    map.AddRefMethod(rm);
                }

                rm = new RefMethod();
                rm.Title = "设置流程表单树权限";
                rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoNodeFormTree";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                if (Glo.IsEnableZhiDu)
                {
                    rm = new RefMethod();
                    rm.Title = "对应制度章节"; // "个性化接受人窗口";
                    rm.ClassMethodName = this.ToString() + ".DoZhiDu";
                    rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                    map.AddRefMethod(rm);

                    rm = new RefMethod();
                    rm.Title = "风险点"; // "个性化接受人窗口";
                    rm.ClassMethodName = this.ToString() + ".DoFengXianDian";
                    rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                    map.AddRefMethod(rm);

                    rm = new RefMethod();
                    rm.Title = "岗位职责"; // "个性化接受人窗口";
                    rm.ClassMethodName = this.ToString() + ".DoGangWeiZhiZe";
                    rm.Icon = BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/DTS.gif";
                    map.AddRefMethod(rm);
                }

                this._enMap = map;
                return this._enMap;
            }
        }
        /// <summary>
        /// 集团部门树
        /// </summary>
        /// <returns></returns>
        public string DoDepts()
        {
            PubClass.WinOpen(Glo.CCFlowAppPath + "WF/Comm/Port/DeptTree.aspx?s=d34&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID + "&RefNo=" + DataType.CurrentDataTime, 500, 550);
            return null;
        }
        /// <summary>
        /// 设置流程表单树权限
        /// </summary>
        /// <returns></returns>
        public string DoNodeFormTree()
        {
            return Glo.CCFlowAppPath + "WF/Admin/FlowFormTree.aspx?s=d34&FK_Flow=" + this.FK_Flow + "&FK_Node=" +
                   this.NodeID + "&RefNo=" + DataType.CurrentDataTime;
        }
        /// <summary>
        /// 制度
        /// </summary>
        /// <returns></returns>
        public string DoZhiDu()
        {
            PubClass.WinOpen(Glo.CCFlowAppPath + "ZhiDu/NodeZhiDuDtl.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow, "制度", "Bill", 700, 400, 200, 300);
            return null;
        }
        /// <summary>
        /// 风险点
        /// </summary>
        /// <returns></returns>
        public string DoFengXianDian()
        {
            // PubClass.WinOpen(Glo.CCFlowAppPath + "ZhiDu/NodeFengXianDian.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow, "制度", "Bill", 700, 400, 200, 300);
            return null;
        }
        /// <summary>
        /// 找人规则
        /// </summary>
        /// <returns></returns>
        public string DoAccepterRole()
        {
            BP.WF.Node nd = new BP.WF.Node(this.NodeID);

            if (nd.HisDeliveryWay != DeliveryWay.ByCCFlowBPM)
                return "节点访问规则您没有设置按照bpm模式，所以您能执行该操作。要想执行该操作请选择节点属性中节点规则访问然后选择按照bpm模式计算，点保存按钮。";

            return Glo.CCFlowAppPath + "WF/Admin/FindWorker/List.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow; 
         //   return null;
        }
        public string DoTurn()
        {
            return Glo.CCFlowAppPath + "WF/Admin/TurnTo.aspx?FK_Node=" + this.NodeID;
            //, "节点完成转向处理", "FrmTurn", 800, 500, 200, 300);
            //BP.WF.Node nd = new BP.WF.Node(this.NodeID);
            //return nd.DoTurn();
        }
        /// <summary>
        /// 抄送规则
        /// </summary>
        /// <returns></returns>
        public string DoCCRole()
        {
            return Glo.CCFlowAppPath + "WF/Comm/RefFunc/UIEn.aspx?EnName=BP.WF.Template.CC&PK=" + this.NodeID; 
            //PubClass.WinOpen("./RefFunc/UIEn.aspx?EnName=BP.WF.CC&PK=" + this.NodeID, "抄送规则", "Bill", 800, 500, 200, 300);
            //return null;
        }
        /// <summary>
        /// 个性化接受人窗口
        /// </summary>
        /// <returns></returns>
        public string DoAccepter()
        {
            return Glo.CCFlowAppPath + "WF/Comm/RefFunc/UIEn.aspx?EnName=BP.WF.Template.Selector&PK=" + this.NodeID;
            //return null;
        }
        /// <summary>
        /// 退回节点
        /// </summary>
        /// <returns></returns>
        public string DoCanReturnNodes()
        {
            return Glo.CCFlowAppPath + "WF/Admin/CanReturnNodes.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// 撤销发送的节点
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
        /// 执行消息收听
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
        /// 设计傻瓜表单
        /// </summary>
        /// <returns></returns>
        public string DoFormCol4()
        {
            return Glo.CCFlowAppPath + "WF/MapDef/MapDef.aspx?PK=ND" + this.NodeID;
        }
        /// <summary>
        /// 设计自由表单
        /// </summary>
        /// <returns></returns>
        public string DoFormFree()
        {
            return Glo.CCFlowAppPath + "WF/MapDef/CCForm/Frm.aspx?FK_MapData=ND" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// 绑定流程表单
        /// </summary>
        /// <returns></returns>
        public string DoFormTree()
        {
            return Glo.CCFlowAppPath + "WF/Admin/FlowFrms.aspx?ShowType=FlowFrms&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID + "&Lang=CH";
        }
        
        public string DoMapData()
        {
            int i = this.GetValIntByKey(NodeAttr.FormType);

            // 类型.
            NodeFormType type = (NodeFormType)i;
            switch (type)
            {
                case NodeFormType.FreeForm:
                    PubClass.WinOpen(Glo.CCFlowAppPath + "WF/MapDef/CCForm/Frm.aspx?FK_MapData=ND" + this.NodeID + "&FK_Flow=" + this.FK_Flow, "设计表单", "sheet", 1024, 768, 0, 0);
                    break;
                default:
                case NodeFormType.FixForm:
                    PubClass.WinOpen(Glo.CCFlowAppPath + "WF/MapDef/MapDef.aspx?PK=ND" + this.NodeID, "设计表单", "sheet", 800, 500, 210, 300);
                    break;
            }
            return null;
        }
        public string DoAction()
        {
            return Glo.CCFlowAppPath + "WF/Admin/Action.aspx?NodeID=" + this.NodeID + "&FK_Flow=" + this.FK_Flow + "&tk=" + new Random().NextDouble();
        }
        /// <summary>
        /// 单据打印
        /// </summary>
        /// <returns></returns>
        public string DoBill()
        {
            return Glo.CCFlowAppPath + "WF/Admin/Bill.aspx?NodeID=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// 设置
        /// </summary>
        /// <returns></returns>
        public string DoFAppSet()
        {
            return Glo.CCFlowAppPath + "WF/Admin/FAppSet.aspx?NodeID=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }

        protected override bool beforeUpdate()
        {
            //更新流程版本
            Flow.UpdateVer(this.FK_Flow);

            //把工具栏的配置放入 sys_mapdata里.
            ToolbarExcel te = new ToolbarExcel("ND" + this.NodeID);
            te.Copy(this);
            try
            {
                te.Update();
            }
            catch
            {

            }

           
            #region  //获得 NEE 实体.
            //if (string.IsNullOrEmpty(this.NodeMark) == false)
            //{
            //    Flow fl = new Flow(this.FK_Flow);

            //    object obj = Glo.GetNodeEventEntityByNodeMark( fl.FlowMark, this.NodeMark);
            //    if (obj == null)
            //        throw new Exception("@节点标记错误：没有找到该节点标记(" + this.NodeMark + ")的节点事件实体.");
            //    this.NodeEventEntity = obj.ToString();
            //}
            //else
            //{
            //    this.NodeEventEntity = "";
            //}
            #endregion 同步事件实体

            #region 处理节点数据.
            Node nd = new Node(this.NodeID);
            if (nd.IsStartNode == true)
            {
                /*处理按钮的问题*/
                //不能退回, 加签，移交，退回, 子线程.
                this.SetValByKey(BtnAttr.ReturnRole,(int)ReturnRole.CanNotReturn);
                this.SetValByKey(BtnAttr.HungEnable, false);
                this.SetValByKey(BtnAttr.ThreadEnable, false); //子线程.
            }

            if (nd.HisRunModel == RunModel.HL || nd.HisRunModel == RunModel.FHL)
            {
                /*如果是合流点*/
            }
            else
            {
                this.SetValByKey(BtnAttr.ThreadEnable, false); //子线程.
            }
            #endregion 处理节点数据.

            //#region 处理消息参数字段.
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
    /// 节点集合
    /// </summary>
    public class NodeSheets : Entities
    {
        #region 构造方法
        /// <summary>
        /// 节点集合
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
