using System;
using System.Text;
using System.Collections;
using BP.DA;
using BP.En;
using BP.En;
using BP.Port;
using BP.Web;

namespace BP.Sys
{
    /// <summary>
    /// 消息控制方式
    /// </summary>
    public enum MsgCtrl
    {
        /// <summary>
        /// bufasong 
        /// </summary>
        None,
        /// <summary>
        /// 按照设置计算
        /// </summary>
        BySet,
        /// <summary>
        /// 按照表单的是否发送字段计算，字段:IsSendMsg
        /// </summary>
        ByFrmIsSendMsg,
        /// <summary>
        /// 按照SDK参数计算.
        /// </summary>
        BySDK
    }
    public enum EventDoType
    {
        /// <summary>
        /// 禁用
        /// </summary>
        Disable=0,
        /// <summary>
        /// 执行存储过程
        /// </summary>
        SP=1,
        /// <summary>
        /// 运行SQL
        /// </summary>
        SQL=2,
        /// <summary>
        /// 自定义URL
        /// </summary>
        URLOfSelf=3,
        /// <summary>
        /// 自定义WS
        /// </summary>
        WSOfSelf=4,
        /// <summary>
        /// EXE
        /// </summary>
        EXE=5,
        /// <summary>
        /// 基类
        /// </summary>
        EventBase=6,
        /// <summary>
        /// JS
        /// </summary>
        Javascript=7
    }
    public class FrmEventList
    {
        /// <summary>
        /// 表单载入前
        /// </summary>
        public const string FrmLoadBefore = "FrmLoadBefore";
        /// <summary>
        /// 表单载入后
        /// </summary>
        public const string FrmLoadAfter = "FrmLoadAfter";
        /// <summary>
        /// 表单保存前
        /// </summary>
        public const string SaveBefore = "SaveBefore";
        /// <summary>
        /// 表单保存后
        /// </summary>
        public const string SaveAfter = "SaveAfter";
    }
    /// <summary>
    /// 事件标记列表
    /// </summary>
    public class EventListOfNode : FrmEventList
    {
        #region 节点事件
        /// <summary>
        /// 节点发送前
        /// </summary>
        public const string SendWhen = "SendWhen";
        /// <summary>
        /// 节点发送成功后
        /// </summary>
        public const string SendSuccess = "SendSuccess";
        /// <summary>
        /// 节点发送失败后
        /// </summary>
        public const string SendError = "SendError";
        /// <summary>
        /// 当节点退回前
        /// </summary>
        public const string ReturnBefore = "ReturnBefore";
        /// <summary>
        /// 当节点退后
        /// </summary>
        public const string ReturnAfter = "ReturnAfter";
        /// <summary>
        /// 当节点撤销发送前
        /// </summary>
        public const string UndoneBefore = "UndoneBefore";
        /// <summary>
        /// 当节点撤销发送后
        /// </summary>
        public const string UndoneAfter = "UndoneAfter";
        /// <summary>
        /// 当前节点移交后
        /// </summary>
        public const string ShitAfter = "ShitAfter";
        /// <summary>
        /// 当节点加签后
        /// </summary>
        public const string AskerAfter = "AskerAfter";
       /// <summary>
       /// 当节点加签答复后
       /// </summary>
        public const string AskerReAfter = "AskerReAfter";
        #endregion 节点事件

        #region 流程事件
        /// <summary>
        /// 流程完成时.
        /// </summary>
        public const string FlowOverBefore = "FlowOverBefore";
        /// <summary>
        /// 结束后.
        /// </summary>
        public const string FlowOverAfter = "FlowOverAfter";
        /// <summary>
        /// 流程删除前
        /// </summary>
        public const string BeforeFlowDel = "BeforeFlowDel";
        /// <summary>
        /// 流程删除后
        /// </summary>
        public const string AfterFlowDel = "AfterFlowDel";
        #endregion 流程事件
    }
	/// <summary>
	/// 事件属性
	/// </summary>
    public class FrmEventAttr
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public const string FK_Event = "FK_Event";
        /// <summary>
        /// 节点ID
        /// </summary>
        public const string FK_MapData = "FK_MapData";
        /// <summary>
        /// 执行类型
        /// </summary>
        public const string DoType = "DoType";
        /// <summary>
        /// 执行内容
        /// </summary>
        public const string DoDoc = "DoDoc";
        /// <summary>
        /// 标签
        /// </summary>
        public const string MsgOK = "MsgOK";
        /// <summary>
        /// 执行错误提示
        /// </summary>
        public const string MsgError = "MsgError";


        #region 消息设置.
        /// <summary>
        /// 控制方式
        /// </summary>
        public const string MsgCtrl = "MsgCtrl";
        /// <summary>
        /// 是否启用发送邮件
        /// </summary>
        public const string MsgMailEnable = "MsgMailEnable";
        /// <summary>
        /// 消息标题
        /// </summary>
        public const string MailTitle = "MailTitle";
        /// <summary>
        /// 消息内容模版
        /// </summary>
        public const string MailDoc = "MailDoc";
        /// <summary>
        /// 是否启用短信
        /// </summary>
        public const string SMSEnable = "SMSEnable";
        /// <summary>
        /// 短信内容模版
        /// </summary>
        public const string SMSDoc = "SMSDoc";
        /// <summary>
        /// 是否推送？
        /// </summary>
        public const string MobilePushEnable = "MobilePushEnable";
        #endregion 消息设置.
    }
	/// <summary>
	/// 事件
	/// 节点的节点保存事件有两部分组成.	 
	/// 记录了从一个节点到其他的多个节点.
	/// 也记录了到这个节点的其他的节点.
	/// </summary>
    public class FrmEvent : EntityMyPK
    {
        #region 基本属性
        public override En.UAC HisUAC
        {
            get
            {
                UAC uac = new En.UAC();
                uac.IsAdjunct = false;
                uac.IsDelete = false;
                uac.IsInsert = false;
                uac.IsUpdate = true;
                return uac;
            }
        }
        /// <summary>
        /// 节点
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.GetValStringByKey(FrmEventAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(FrmEventAttr.FK_MapData, value);
            }
        }
        public string DoDoc
        {
            get
            {
                return this.GetValStringByKey(FrmEventAttr.DoDoc).Replace("~", "'");
            }
            set
            {
                string doc = value.Replace("'", "~");
                this.SetValByKey(FrmEventAttr.DoDoc, doc);
            }
        }
        /// <summary>
        /// 执行成功提示
        /// </summary>
        public string MsgOK(Entity en)
        {
            string val = this.GetValStringByKey(FrmEventAttr.MsgOK);
            if (val.Trim() == "")
                return "";

            if (val.IndexOf('@') == -1)
                return val;

            foreach (Attr attr in en.EnMap.Attrs)
            {
                val = val.Replace("@" + attr.Key, en.GetValStringByKey(attr.Key));
            }
            return val;
        }
        public string MsgOKString
        {
            get
            {
                return this.GetValStringByKey(FrmEventAttr.MsgOK);
            }
            set
            {
                this.SetValByKey(FrmEventAttr.MsgOK, value);
            }
        }
        public string MsgErrorString
        {
            get
            {
                return this.GetValStringByKey(FrmEventAttr.MsgError);
            }
            set
            {
                this.SetValByKey(FrmEventAttr.MsgError, value);
            }
        }
        /// <summary>
        /// 错误或异常提示
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public string MsgError(Entity en)
        {
            string val = this.GetValStringByKey(FrmEventAttr.MsgError);
            if (val.Trim() == "")
                return null;

            if (val.IndexOf('@') == -1)
                return val;

            foreach (Attr attr in en.EnMap.Attrs)
            {
                val = val.Replace("@" + attr.Key, en.GetValStringByKey(attr.Key));
            }
            return val;
        }

        public string FK_Event
        {
            get
            {
                return this.GetValStringByKey(FrmEventAttr.FK_Event);
            }
            set
            {
                this.SetValByKey(FrmEventAttr.FK_Event, value);
            }
        }
        /// <summary>
        /// 执行类型
        /// </summary>
        public EventDoType HisDoType
        {
            get
            {
                return (EventDoType)this.GetValIntByKey(FrmEventAttr.DoType);
            }
            set
            {
                this.SetValByKey(FrmEventAttr.DoType, (int)value);
            }
        }
        #endregion

        #region 事件消息.
        /// <summary>
        /// 消息控制类型.
        /// </summary>
        public MsgCtrl MsgCtrl
        {
            get
            {
                return (MsgCtrl)this.GetValIntByKey(FrmEventAttr.MsgCtrl);
            }
            set
            {
                this.SetValByKey(FrmEventAttr.MsgCtrl, (int)value);
            }
        }
        public bool MobilePushEnable
        {
            get
            {
                return this.GetValBooleanByKey(FrmEventAttr.MobilePushEnable);
            }
            set
            {
                this.SetValByKey(FrmEventAttr.MobilePushEnable, value);
            }
        }
        /// <summary>
        /// 是否启用邮件发送?
        /// </summary>
        public bool MsgMailEnable
        {
            get
            {
                return this.GetValBooleanByKey(FrmEventAttr.MsgMailEnable);
            }
            set
            {
                this.SetValByKey(FrmEventAttr.MsgMailEnable, value);
            }
        }
        public string MailTitle
        {
            get
            {
                string str = this.GetValStrByKey(FrmEventAttr.MailTitle);
                if (string.IsNullOrEmpty(str) == false)
                    return str;
                switch (this.FK_Event)
                {
                    case EventListOfNode.SendSuccess:
                        return "新工作{{Title}},发送人@WebUser.No,@WebUser.Name";
                    case EventListOfNode.ShitAfter:
                        return "移交来的新工作{{Title}},移交人@WebUser.No,@WebUser.Name";
                    case EventListOfNode.ReturnAfter:
                        return "被退回来{{Title}},退回人@WebUser.No,@WebUser.Name";
                    case EventListOfNode.UndoneAfter:
                        return "工作被撤销{{Title}},发送人@WebUser.No,@WebUser.Name";
                    case EventListOfNode.AskerReAfter:
                        return "加签新工作{{Title}},发送人@WebUser.No,@WebUser.Name";
                        break;
                    default:
                        throw new Exception("@该事件类型没有定义默认的消息模版:" + this.FK_Event);
                        break;
                }
                return str;
            }
        }
        /// <summary>
        /// 邮件标题
        /// </summary>
        public string MailTitle_Real
        {
            get
            {
                string str = this.GetValStrByKey(FrmEventAttr.MailTitle);
                return str;
            }
            set
            {
                this.SetValByKey(FrmEventAttr.MailTitle, value);
            }
        }
        /// <summary>
        /// 邮件内容
        /// </summary>
        public string MailDoc_Real
        {
            get
            {
                return this.GetValStrByKey(FrmEventAttr.MailDoc);
            }
            set
            {
                this.SetValByKey(FrmEventAttr.MailDoc, value);
            }
        }
        public string MailDoc
        {
            get
            {
                string str = this.GetValStrByKey(FrmEventAttr.MailDoc);
                if (string.IsNullOrEmpty(str) == false)
                    return str;
                switch (this.FK_Event)
                {
                    case EventListOfNode.SendSuccess:
                        str += "\t\n您好:";
                        str += "\t\n    有新工作{{Title}}需要您处理, 点击这里打开工作{Url} .";
                        str += "\t\n致! ";
                        str += "\t\n    @WebUser.No, @WebUser.Name";
                        str += "\t\n    @RDT";
                        break;
                    case EventListOfNode.ReturnAfter:
                        str += "\t\n您好:";
                        str += "\t\n    工作{{Title}}被退回来了, 点击这里打开工作{Url} .";
                        str += "\t\n 致! ";
                        str += "\t\n    @WebUser.No,@WebUser.Name";
                        str += "\t\n    @RDT";
                        break;
                    case EventListOfNode.ShitAfter:
                        str += "\t\n您好:";
                        str += "\t\n    移交给您的工作{{Title}}, 点击这里打开工作{Url} .";
                        str += "\t\n 致! ";
                        str += "\t\n    @WebUser.No,@WebUser.Name";
                        str += "\t\n    @RDT";
                        break;
                    case EventListOfNode.UndoneAfter:
                         str += "\t\n您好:";
                         str += "\t\n    移交给您的工作{{Title}}, 点击这里打开工作{Url} .";
                        str += "\t\n 致! ";
                        str += "\t\n    @WebUser.No,@WebUser.Name";
                        str += "\t\n    @RDT";
                        break;
                    case EventListOfNode.AskerReAfter: //加签.
                        str += "\t\n您好:";
                        str += "\t\n    移交给您的工作{{Title}}, 点击这里打开工作{Url} .";
                        str += "\t\n 致! ";
                        str += "\t\n    @WebUser.No,@WebUser.Name";
                        str += "\t\n    @RDT";
                        break;
                    default:
                        throw new Exception("@该事件类型没有定义默认的消息模版:" + this.FK_Event);
                        break;
                }
                return str;
            }
        }
        /// <summary>
        /// 是否启用短信发送
        /// </summary>
        public bool SMSEnable
        {
            get
            {
                return this.GetValBooleanByKey(FrmEventAttr.SMSEnable);
            }
            set
            {
                this.SetValByKey(FrmEventAttr.SMSEnable, value);
            }
        }
        /// <summary>
        /// 短信模版内容
        /// </summary>
        public string SMSDoc_Real
        {
            get
            {
                string str = this.GetValStrByKey(FrmEventAttr.SMSDoc);
                return str;
            }
            set
            {
                this.SetValByKey(FrmEventAttr.SMSDoc, value);
            }
        }
        /// <summary>
        /// 短信模版内容
        /// </summary>
        public string SMSDoc
        {
            get
            {
                string str = this.GetValStrByKey(FrmEventAttr.SMSDoc);
                if (string.IsNullOrEmpty(str) == false)
                    return str;

                switch (this.FK_Event)
                {
                    case EventListOfNode.SendSuccess:
                        str = "有新工作{{Title}}需要您处理, 发送人:@WebUser.No, @WebUser.Name,打开{Url} .";
                        break;
                    case EventListOfNode.ReturnAfter:
                        str = "工作{{Title}}被退回,退回人:@WebUser.No, @WebUser.Name,打开{Url} .";
                        break;
                    case EventListOfNode.ShitAfter:
                        str = "移交工作{{Title}},移交人:@WebUser.No, @WebUser.Name,打开{Url} .";
                        break;
                    case EventListOfNode.UndoneAfter:
                        str = "工作撤销{{Title}},撤销人:@WebUser.No, @WebUser.Name,打开{Url}.";
                        break;
                    case EventListOfNode.AskerReAfter: //加签.
                        str = "工作加签{{Title}},加签人:@WebUser.No, @WebUser.Name,打开{Url}.";
                        break;
                    default:
                        throw new Exception("@该事件类型没有定义默认的消息模版:" + this.FK_Event);
                        break;
                }
                return str;
            }
            set
            {
                this.SetValByKey(FrmEventAttr.SMSDoc, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 事件
        /// </summary>
        public FrmEvent()
        {
        }
        public FrmEvent(string mypk)
        {
            this.MyPK = mypk;
            this.RetrieveFromDBSources();
        }
        public FrmEvent(string fk_mapdata, string fk_Event)
        {
            this.FK_Event = fk_Event;
            this.FK_MapData = fk_mapdata;
            this.MyPK = this.FK_MapData + "_" + this.FK_Event;
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

                Map map = new Map("Sys_FrmEvent");
                map.EnDesc = "事件";

                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.AddMyPK();

                map.AddTBString(FrmEventAttr.FK_Event, null, "事件名称", true, true, 0, 400, 10);
                map.AddTBString(FrmEventAttr.FK_MapData, null, "FK_MapData", true, true, 0, 400, 10);

                map.AddTBInt(FrmEventAttr.DoType, 0, "事件类型", true, true);
                map.AddTBString(FrmEventAttr.DoDoc, null, "执行内容", true, true, 0, 400, 10);
                map.AddTBString(FrmEventAttr.MsgOK, null, "成功执行提示", true, true, 0, 400, 10);
                map.AddTBString(FrmEventAttr.MsgError, null, "异常信息提示", true, true, 0, 400, 10);

                #region 消息设置.
                map.AddDDLSysEnum(FrmEventAttr.MsgCtrl, 0, "消息发送控制", true, true, FrmEventAttr.MsgCtrl,
                    "@0=不发送@1=按设置的发送范围自动发送@2=由本节点表单系统字段(IsSendEmail,IsSendSMS)来决定@3=由SDK开发者参数(IsSendEmail,IsSendSMS)来决定", true);

                map.AddBoolean(FrmEventAttr.MsgMailEnable, true, "是否启用邮件发送？(如果启用就要设置邮件模版，支持ccflow表达式。)", true, true, true);
                map.AddTBString(FrmEventAttr.MailTitle, null, "邮件标题模版", true, false, 0, 200, 20, true);
                map.AddTBStringDoc(FrmEventAttr.MailDoc, null, "邮件内容模版", true, false, true);

                //是否启用手机短信？
                map.AddBoolean(FrmEventAttr.SMSEnable, false, "是否启用短信发送？(如果启用就要设置短信模版，支持ccflow表达式。)", true, true, true);
                map.AddTBStringDoc(FrmEventAttr.SMSDoc, null, "短信内容模版", true, false, true);

                map.AddBoolean(FrmEventAttr.MobilePushEnable, true, "是否推送到手机、pad端。", true, true, true);

                #endregion 消息设置.


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeUpdateInsertAction()
        {
            this.MyPK = this.FK_MapData + "_" + this.FK_Event;
            return base.beforeUpdateInsertAction();
        }
    }
	/// <summary>
	/// 事件
	/// </summary>
    public class FrmEvents : EntitiesOID
    {
        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="dotype">执行类型</param>
        /// <param name="en">数据实体</param>
        /// <returns>null 没有事件，其他为执行了事件。</returns>
        public string DoEventNode(string dotype, Entity en)
        {
           return DoEventNode(dotype,en,null);
        }
        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="dotype">执行类型</param>
        /// <param name="en">数据实体</param>
        /// <param name="atPara">参数</param>
        /// <returns>null 没有事件，其他为执行了事件。</returns>
        public string DoEventNode(string dotype, Entity en, string atPara)
        {
            if (this.Count == 0)
                return null;
            string val= _DoEventNode(dotype, en, atPara);
            if (val != null)
                val = val.Trim();

            if (string.IsNullOrEmpty(val))
                return ""; // 说明有事件，执行成功了。
            else
                return val; // 没有事件. 
        }
        
        /// <summary>
        /// 执行事件，事件标记是 EventList.
        /// </summary>
        /// <param name="dotype">执行类型</param>
        /// <param name="en">数据实体</param>
        /// <param name="atPara">特殊的参数格式@key=value 方式.</param>
        /// <returns></returns>
        private string _DoEventNode(string dotype, Entity en, string atPara)
        {
            if (this.Count == 0)
                return null;

            FrmEvent nev = this.GetEntityByKey(FrmEventAttr.FK_Event, dotype) as FrmEvent;
           
            if (nev == null || nev.HisDoType == EventDoType.Disable)
                return null;

            string doc = nev.DoDoc.Trim();
            if (doc == null || doc == "")
                return null;

            #region 处理执行内容
            Attrs attrs = en.EnMap.Attrs;
            string MsgOK = "";
            string MsgErr = "";
            foreach (Attr attr in attrs)
            {
                if (doc.Contains("@" + attr.Key) == false)
                    continue;
                if (attr.MyDataType == DataType.AppString
                    || attr.MyDataType == DataType.AppDateTime
                    || attr.MyDataType == DataType.AppDate)
                    doc = doc.Replace("@" + attr.Key, "'" + en.GetValStrByKey(attr.Key) + "'");
                else
                    doc = doc.Replace("@" + attr.Key, en.GetValStrByKey(attr.Key));
            }

            doc = doc.Replace("~", "'");
            doc = doc.Replace("@WebUser.No", BP.Web.WebUser.No);
            doc = doc.Replace("@WebUser.Name", BP.Web.WebUser.Name);
            doc = doc.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
            doc = doc.Replace("@FK_Node", nev.FK_MapData.Replace("ND", ""));
            doc = doc.Replace("@FK_MapData", nev.FK_MapData);
            doc = doc.Replace("@WorkID", en.GetValStrByKey("OID","@WorkID"));

            //SDK表单上服务器地址,应用到使用ccflow的时候使用的是sdk表单,该表单会存储在其他的服务器上. 
            doc = doc.Replace("@SDKFromServHost", SystemConfig.AppSettings["SDKFromServHost"]);

            

            if (System.Web.HttpContext.Current != null)
            {
                /*如果是 bs 系统, 有可能参数来自于url ,就用url的参数替换它们 .*/
                string url = BP.Sys.Glo.Request.RawUrl;
                if (url.IndexOf('?') != -1)
                    url = url.Substring(url.IndexOf('?'));

                string[] paras = url.Split('&');
                foreach (string s in paras)
                {
                    if (doc.Contains("@" + s) == false)
                        continue;

                    string[] mys = s.Split('=');
                    if (doc.Contains("@" + mys[0]) == false)
                        continue;
                    doc = doc.Replace("@" + mys[0], mys[1]);
                }
            }

            if (nev.HisDoType == EventDoType.URLOfSelf)
            {
                if (doc.Contains("?") == false)
                    doc += "?1=2";

                doc += "&UserNo=" + WebUser.No;
                doc += "&SID=" + WebUser.SID;
                doc += "&FK_Dept=" + WebUser.FK_Dept;
                // doc += "&FK_Unit=" + WebUser.FK_Unit;
                doc += "&OID=" + en.PKVal;

                if (SystemConfig.IsBSsystem)
                {
                    /*是bs系统，并且是url参数执行类型.*/
                    string url = BP.Sys.Glo.Request.RawUrl;
                    if (url.IndexOf('?') != -1)
                        url = url.Substring(url.IndexOf('?'));
                    string[] paras = url.Split('&');
                    foreach (string s in paras)
                    {
                        if (doc.Contains(s))
                            continue;
                        doc += "&" + s;
                    }
                    doc = doc.Replace("&?", "&");
                }

                if (SystemConfig.IsBSsystem == false)
                {
                    /*非bs模式下调用,比如在cs模式下调用它,它就取不到参数. */
                }

                if (doc.StartsWith("http") == false)
                {
                    /*如果没有绝对路径 */
                    if (SystemConfig.IsBSsystem)
                    {
                        /*在cs模式下自动获取*/
                        string host = BP.Sys.Glo.Request.Url.Host;
                        if (doc.Contains("@AppPath"))
                            doc = doc.Replace("@AppPath", "http://" + host + BP.Sys.Glo.Request.ApplicationPath);
                        else
                            doc = "http://" + BP.Sys.Glo.Request.Url.Authority + doc;
                    }

                    if (SystemConfig.IsBSsystem == false)
                    {
                        /*在cs模式下它的baseurl 从web.config中获取.*/
                        string cfgBaseUrl = SystemConfig.AppSettings["BaseUrl"];
                        if (string.IsNullOrEmpty(cfgBaseUrl))
                        {
                            string err = "调用url失败:没有在web.config中配置BaseUrl,导致url事件不能被执行.";
                            Log.DefaultLogWriteLineError(err);
                            throw new Exception(err);
                        }
                        doc = cfgBaseUrl + doc;
                    }
                }

                //增加上系统约定的参数.
                doc += "&EntityName=" + en.ToString() + "&EntityPK=" + en.PK + "&EntityPKVal=" + en.PKVal + "&FK_Event=" + nev.MyPK;
            }
            #endregion 处理执行内容

            if (atPara != null && doc.Contains("@")==true)
            {
                AtPara ap = new AtPara(atPara);
                foreach (string s in ap.HisHT.Keys)
                    doc = doc.Replace("@" + s, ap.GetValStrByKey(s));
            }

            if (dotype == FrmEventList.FrmLoadBefore)
                en.Retrieve(); /*如果不执行，就会造成实体的数据与查询的数据不一致.*/

            switch (nev.HisDoType)
            {
                //case EventDoType.SP:
                //    try
                //    {
                //        Paras ps = new Paras();
                //        DBAccess.RunSP(doc, ps);
                //        return nev.MsgOK(en);
                //    }
                //    catch (Exception ex)
                //    {
                //        throw new Exception(nev.MsgError(en) + " Error:" + ex.Message);
                //    }
                //    break;
                case EventDoType.SP:
                case EventDoType.SQL:
                    try
                    {
                        // 允许执行带有GO的sql.
                        DBAccess.RunSQLs(doc);
                        return nev.MsgOK(en);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(nev.MsgError(en) + " Error:" + ex.Message);
                    }
                    break;
                case EventDoType.URLOfSelf:
                    string myURL = doc.Clone() as string;
                    if (myURL.Contains("http") == false)
                    {
                        if (SystemConfig.IsBSsystem)
                        {
                            string host = BP.Sys.Glo.Request.Url.Host;
                            if (myURL.Contains("@AppPath"))
                                myURL = myURL.Replace("@AppPath", "http://" + host + BP.Sys.Glo.Request.ApplicationPath);
                            else
                                myURL = "http://" + BP.Sys.Glo.Request.Url.Authority + myURL;
                        }
                        else
                        {
                            string cfgBaseUrl = SystemConfig.AppSettings["BaseUrl"];
                            if (string.IsNullOrEmpty(cfgBaseUrl))
                            {
                                string err = "调用url失败:没有在web.config中配置BaseUrl,导致url事件不能被执行.";
                                Log.DefaultLogWriteLineError(err);
                                throw new Exception(err);
                            }
                            myURL = cfgBaseUrl + myURL;
                        }
                    }
                    myURL = myURL.Replace("@SDKFromServHost", SystemConfig.AppSettings["SDKFromServHost"]);

                    try
                    {
                        Encoding encode = System.Text.Encoding.GetEncoding("gb2312");
                        string text = DataType.ReadURLContext(myURL, 600000, encode);
                        if (text == null)
                            throw new Exception("@流程设计错误，执行的url错误:" + myURL + ", 返回为null, 请检查url设置是否正确。提示：您可以copy出来此url放到浏览器里看看是否被正确执行。");

                        if (text != null
                            && text.Length > 7
                            && text.Substring(0, 7).ToLower().Contains("err"))
                            throw new Exception(text);

                        if (text == null || text.Trim() == "")
                            return null;
                        return text;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("@" + nev.MsgError(en) + " Error:" + ex.Message);
                    }
                    break;
                //case EventDoType.URLOfSystem:
                //    string hos1t = BP.Sys.Glo.Request.Url.Host;
                //    string url = "http://" + hos1t + BP.Sys.Glo.Request.ApplicationPath + "/DataUser/AppCoder/FrmEventHandle.aspx";
                //    url += "?FK_MapData=" + en.ClassID + "&WebUseNo=" + WebUser.No + "&EventType=" + nev.FK_Event;
                //    foreach (Attr attr in attrs)
                //    {
                //        if (attr.UIIsDoc || attr.IsRefAttr || attr.UIIsReadonly)
                //            continue;
                //        url += "&" + attr.Key + "=" + en.GetValStrByKey(attr.Key);
                //    }

                //    try
                //    {
                //        string text = DataType.ReadURLContext(url, 800, System.Text.Encoding.UTF8);
                //        if (text != null && text.Substring(0, 7).Contains("Err"))
                //            throw new Exception(text);

                //        if (text == null || text.Trim() == "")
                //            return null; // 如果是Null 没有事件配置。
                //        return text;
                //    }
                //    catch (Exception ex)
                //    {
                //        throw new Exception("@" + nev.MsgError(en) + " Error:" + ex.Message);
                //    }
                //    break;
                case EventDoType.EventBase: //执行事件类.

                    // 获取事件类.
                    string evName = doc.Clone() as string;
                    BP.Sys.EventBase ev = null;
                    try
                    {
                        ev = BP.En.ClassFactory.GetEventBase(evName);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("@事件名称:" + evName + "拼写错误，或者系统不存在。说明：事件所在的类库必须是以BP.开头，并且类库的BP.xxx.dll。");
                    }

                    //开始执行.
                    try
                    {
                        #region 处理整理参数.
                        Row r = en.Row;
                        try
                        {
                            //系统参数.
                            r.Add("FK_MapData", en.ClassID);
                        }
                        catch
                        {
                            r["FK_MapData"] = en.ClassID;
                        }

                        try
                        {
                            r.Add("EventType", nev.FK_Event);
                        }
                        catch
                        {
                            r["EventType"] = nev.FK_Event;
                        }

                        if (atPara != null)
                        {
                            AtPara ap = new AtPara(atPara);
                            foreach (string s in ap.HisHT.Keys)
                            {
                                try
                                {
                                    r.Add(s, ap.GetValStrByKey(s));
                                }
                                catch
                                {
                                    r[s] = ap.GetValStrByKey(s);
                                }
                            }
                        }

                        if (SystemConfig.IsBSsystem == true)
                        {
                            /*如果是bs系统, 就加入外部url的变量.*/
                            foreach (string key in BP.Sys.Glo.Request.QueryString)
                            {
                                string val = BP.Sys.Glo.Request.QueryString[key];
                                try
                                {
                                    r.Add(key, val);
                                }
                                catch
                                {
                                    r[key] = val;
                                }
                            }
                        }
                        #endregion 处理整理参数.

                        ev.SysPara = r;
                        ev.HisEn = en;
                        ev.Do();
                        return ev.SucessInfo;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("@执行事件(" + ev.Title + ")期间出现错误:" + ex.Message);
                    }
                    break;
                default:
                    throw new Exception("@no such way." + nev.HisDoType.ToString());
            }
        }
        /// <summary>
        /// 事件
        /// </summary>
        public FrmEvents() { }
        /// <summary>
        /// 事件
        /// </summary>
        /// <param name="FK_MapData">FK_MapData</param>
        public FrmEvents(string fk_MapData)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FrmEventAttr.FK_MapData, fk_MapData);
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmEvent();
            }
        }
    }
}
