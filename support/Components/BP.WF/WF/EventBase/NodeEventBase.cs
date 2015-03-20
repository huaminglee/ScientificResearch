using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.DTS;
using BP.En;
using BP.Web.Controls;
using BP.Web;
using BP.Sys;

namespace BP.WF
{
    /// <summary>
    /// 节点事件基类
    /// </summary>
    abstract public class NodeEventBase
    {
        #region 属性.
        public Node HisNode = null;
        public Entity HisEn = null;
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
            string str = this.GetValStr(key).ToString();
            return DataType.ParseSysDateTime2DateTime(str);
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
        public decimal GetValDecimal(string key)
        {
            return decimal.Parse(this.GetValStr(key));
        }
        #endregion 获取参数方法

        #region 构造方法
        /// <summary>
        /// 事件基类
        /// </summary>
        public NodeEventBase()
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
        /// <summary>
        /// 节点编码s, 可以有多个节点编号.
        /// 多个节点编码需要用逗号分开.
        /// </summary>
        abstract public string NodeMarks
        {
            get;
        }
        #endregion 要求子类重写的属性.

        #region 要求子类重写的方法(表单事件).
        /// <summary>
        /// 表单载入前
        /// </summary>
        abstract public string FrmLoadBefore();
        /// <summary>
        /// 表单载入后
        /// </summary>
        abstract public string FrmLoadAfter();
        /// <summary>
        /// 表单保存前
        /// </summary>
        abstract public string SaveBefore();
        /// <summary>
        /// 表单保存后
        /// </summary>
        abstract public string SaveAfter();
        #endregion 要求子类重写的方法(表单事件).

        #region 要求子类重写的方法(节点事件).
        /// <summary>
        /// 节点发送前
        /// </summary>
        abstract public string SendWhen();
        /// <summary>
        /// 节点发送成功后
        /// </summary>
        abstract public string SendSuccess();
        /// <summary>
        /// 节点发送失败后
        /// </summary>
        abstract public string SendError();
        /// <summary>
        /// 当节点退回前
        /// </summary>
        abstract public string ReturnBefore();
        /// <summary>
        /// 当节点退后
        /// </summary>
        abstract public string ReturnAfter();
        /// <summary>
        /// 当节点撤销发送前
        /// </summary>
        abstract public string UnSendBefore();
        /// <summary>
        /// 当节点撤销发送后
        /// </summary>
        abstract public string UnSendAfter();
        #endregion 要求子类重写的方法(节点事件).

        #region 基类方法.
        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="en">实体参数</param>
        public string DoIt(string eventType, Node currNode, Entity en, string atPara)
        {
            //他的节点.
            this.HisNode = currNode;
            this.HisEn = en;

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
                foreach (string key in System.Web.HttpContext.Current.Request.QueryString)
                {
                    string val = System.Web.HttpContext.Current.Request.QueryString[key];
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
                case EventListOfNode.SendWhen:
                    return this.SendWhen();
                case EventListOfNode.SendSuccess:
                    return this.SendSuccess();
                case EventListOfNode.SendError:
                    return this.SendError();
                case EventListOfNode.ReturnBefore:
                    return this.ReturnBefore();
                case EventListOfNode.ReturnAfter:
                    return this.ReturnAfter();
                case EventListOfNode.UndoneBefore:
                    return this.UnSendBefore();
                case EventListOfNode.UndoneAfter:
                    return this.UnSendAfter();
                case EventListOfNode.SaveBefore:
                    return this.SaveBefore();
                case EventListOfNode.SaveAfter:
                    return this.SaveAfter();
                case EventListOfNode.FrmLoadBefore:
                    return this.FrmLoadBefore();
                case EventListOfNode.FrmLoadAfter:
                    return this.FrmLoadAfter();
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
