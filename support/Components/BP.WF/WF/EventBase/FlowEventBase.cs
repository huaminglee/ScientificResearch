using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;
using BP.Web.Controls;
using BP.Web;
using BP.Sys;
using BP.WF.XML;

namespace BP.WF
{
    /// <summary>
    /// 流程事件基类
    /// </summary>
    abstract public class FlowEventBase
    {
        #region 属性.
        /// <summary>
        /// 发送对象
        /// </summary>
        public SendReturnObjs SendReturnObjs = null;
        /// <summary>
        /// 实体，一般是工作实体
        /// </summary>
        public Entity HisEn = null;
        /// <summary>
        /// 当前节点
        /// </summary>
        public Node HisNode = null;
        /// <summary>
        /// 参数对象.
        /// </summary>
        private Row _SysPara = null;
        /// <summary>
        /// 参数
        /// </summary>
        public Row SysPara
        {
            get
            {
                if (_SysPara == null)
                    _SysPara = new Row();
                return _SysPara;
            }
            set
            {
                _SysPara = value;
            }
        }
        /// <summary>
        /// 成功信息
        /// </summary>
        public string SucessInfo = null;
        #endregion 属性.

        #region 系统参数
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FK_Mapdata
        {
            get
            {
                return this.GetValStr("FK_MapData");
            }
        }
        #endregion

        #region 常用属性.
        /// <summary>
        /// 工作ID
        /// </summary>
        public int OID
        {
            get
            {
                return this.GetValInt("OID");
            }
        }
        /// <summary>
        /// 工作ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                if (this.OID == 0)
                    return this.GetValInt64("WorkID"); /*有可能开始节点的WorkID=0*/
                return this.OID;
            }
        }
        /// <summary>
        /// FID
        /// </summary>
        public Int64 FID
        {
            get
            {
                return this.GetValInt64("FID");
            }
        }
        /// <summary>
        /// 传过来的WorkIDs集合，子流程.
        /// </summary>
        public string WorkIDs
        {
            get
            {
                return this.GetValStr("WorkIDs");
            }
        }
        /// <summary>
        /// 编号集合s
        /// </summary>
        public string Nos
        {
            get
            {
                return this.GetValStr("Nos");
            }
        }
        #endregion 常用属性.

        #region 获取参数方法
        public DateTime GetValDateTime(string key)
        {
            try
            {
                string str = this.SysPara.GetValByKey(key).ToString();
                return DataType.ParseSysDateTime2DateTime(str);
            }
            catch (Exception ex)
            {
                throw new Exception("@流程事件实体在获取参数期间出现错误，请确认字段(" + key + ")是否拼写正确,技术信息:" + ex.Message);
            }
        }
        /// <summary>
        /// 获取字符串参数
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>如果为Nul,或者不存在就抛出异常</returns>
        public string GetValStr(string key)
        {
            try
            {
                return this.SysPara.GetValByKey(key).ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("@流程事件实体在获取参数期间出现错误，请确认字段(" + key + ")是否拼写正确,技术信息:" + ex.Message);
            }
        }
        /// <summary>
        /// 获取Int64的数值
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns>如果为Nul,或者不存在就抛出异常</returns>
        public Int64 GetValInt64(string key)
        {
            return Int64.Parse(this.GetValStr(key));
        }
        /// <summary>
        /// 获取int的数值
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns>如果为Nul,或者不存在就抛出异常</returns>
        public int GetValInt(string key)
        {
            return int.Parse(this.GetValStr(key));
        }
        public bool GetValBoolen(string key)
        {
            if (int.Parse(this.GetValStr(key)) == 0)
                return false;
            return true;
        }
        /// <summary>
        /// 获取decimal的数值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public decimal GetValDecimal(string key)
        {
            return decimal.Parse(this.GetValStr(key));
        }
        #endregion 获取参数方法

        #region 构造方法
        /// <summary>
        /// 流程事件基类
        /// </summary>
        public FlowEventBase()
        {
        }
        #endregion 构造方法

        #region 要求子类强制重写的属性.
        /// <summary>
        /// 流程编号
        /// </summary>
        abstract public string FlowMark
        {
            get;
        }
        #endregion 要求子类重写的属性.

        #region 节点表单事件
        public virtual string FrmLoadAfter()
        {
            return null;
        }
        public virtual string FrmLoadBefore()
        {
            return null;
        }
        #endregion

        #region 要求子类重写的方法(流程事件).
        /// <summary>
        /// 流程完成前
        /// </summary>
        public virtual string FlowOverBefore()
        {
            return null;
        }
        /// <summary>
        /// 结束后
        /// </summary>
        public virtual string FlowOverAfter()
        {
            return null;
        }
        /// <summary>
        ///流程删除前
        /// </summary>
        public virtual string BeforeFlowDel()
        {
            return null;
        }
        /// <summary>
        /// 流程删除后
        /// </summary>
        public virtual string AfterFlowDel()
        {
            return null;
        }
        #endregion 要求子类重写的方法(流程事件).


        #region 要求子类重写的方法(节点事件).
        /// <summary>
        /// 保存后
        /// </summary>
        public virtual string SaveAfter()
        {
            return null;
        }
        /// <summary>
        /// 保存前
        /// </summary>
        public virtual string SaveBefore()
        {
            return null;
        }
        /// <summary>
        ///发送前
        /// </summary>
        public virtual string SendWhen()
        {
            return null;
        }
        /// <summary>
        /// 发送成功时
        /// </summary>
        public virtual string SendSuccess()
        {
            return null;
        }
        /// <summary>
        /// 发送失败
        /// </summary>
        /// <returns></returns>
        public virtual string SendError() { return null; }
        public virtual string ReturnBefore() { return null; }
        public virtual string ReturnAfter() { return null; }
        public virtual string UndoneBefore() { return null; }
        public virtual string UndoneAfter() { return null; }
        /// <summary>
        /// 移交后
        /// </summary>
        /// <returns></returns>
        public virtual string ShiftAfter()
        {
            return null;
        }
        /// <summary>
        /// 加签后
        /// </summary>
        /// <returns></returns>
        public virtual string AskerAfter()
        {
            return null;
        }
        /// <summary>
        /// 加签答复后
        /// </summary>
        /// <returns></returns>
        public virtual string AskerReAfter()
        {
            return null;
        }
        #endregion 要求子类重写的方法(节点事件).


        #region 基类方法.
        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="en">实体参数</param>
        public string DoIt(string eventType, Node currNode, Entity en, string atPara)
        {
            this.HisEn = en;
            this.HisNode = currNode;

            #region 处理参数.
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
            this.SysPara = r;
            #endregion 处理参数.

            #region 执行事件.
            switch (eventType)
            {
                case EventListOfNode.FrmLoadAfter: // 节点表单事件。
                    return this.FrmLoadAfter();
                case EventListOfNode.FrmLoadBefore: // 节点表单事件。
                    return this.FrmLoadBefore();
                case EventListOfNode.SaveAfter: // 节点事件 保存后。
                    return this.SaveAfter();
                case EventListOfNode.SaveBefore: // 节点事件 - 保存前.。
                    return this.SaveBefore();
                case EventListOfNode.SendWhen: // 节点事件 - 发送前。
                    return this.SendWhen();
                case EventListOfNode.SendSuccess: // 节点事件 - 发送成功时。
                    return this.SendSuccess();
                case EventListOfNode.SendError: // 节点事件 - 发送失败。
                    return this.SendError();
                case EventListOfNode.ReturnBefore: // 节点事件 - 退回前。
                    return this.ReturnBefore();
                case EventListOfNode.ReturnAfter: // 节点事件 - 退回后。
                    return this.ReturnAfter();
                case EventListOfNode.UndoneBefore: // 节点事件 - 撤销前。
                    return this.UndoneBefore();
                case EventListOfNode.UndoneAfter: // 节点事件 - 撤销后。
                    return this.UndoneAfter();
                case EventListOfNode.ShitAfter:// 节点事件-移交后
                    return this.ShiftAfter();
                case EventListOfNode.AskerAfter://节点事件 加签后
                    return this.AskerAfter();
                case EventListOfNode.AskerReAfter://节点事件加签回复后
                    return this.AskerReAfter();
                case EventListOfNode.FlowOverBefore: // 流程事件 -------------------------------------------。
                    return this.FlowOverBefore();
                case EventListOfNode.FlowOverAfter: // 流程事件。
                    return this.FlowOverAfter();
                case EventListOfNode.BeforeFlowDel: // 流程事件。
                    return this.BeforeFlowDel();
                case EventListOfNode.AfterFlowDel: // 流程事件。
                    return this.AfterFlowDel();
                default:
                    throw new Exception("@没有判断的事件类型:" + eventType);
                    break;
            }
            #endregion 执行事件.
            return null;
        }
        #endregion 基类方法.
    }
}
