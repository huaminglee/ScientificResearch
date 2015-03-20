using System;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.En;
using BP.Sys;
namespace BP.WF
{
    /// <summary>
    /// 重新生成标题 的摘要说明
    /// </summary>
    public class GenerTitle : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public GenerTitle()
        {
            this.Title = "重新生成标题（为所有的流程，根据新的规则生成流程标题）";
            this.Help = "您也可以打开流程属性一个个的单独执行。";
        }
        /// <summary>
        /// 设置执行变量
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
            //this.Warning = "您确定要执行吗？";
            //HisAttrs.AddTBString("P1", null, "原密码", true, false, 0, 10, 10);
            //HisAttrs.AddTBString("P2", null, "新密码", true, false, 0, 10, 10);
            //HisAttrs.AddTBString("P3", null, "确认", true, false, 0, 10, 10);
        }
        /// <summary>
        /// 当前的操纵员是否可以执行这个方法
        /// </summary>
        public override bool IsCanDo
        {
            get
            {
                return true;
            }
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>返回执行结果</returns>
        public override object Do()
        {
            BP.WF.Ext.FlowSheets ens = new Ext.FlowSheets();
            foreach (BP.WF.Ext.FlowSheet en in ens)
            {
                en.DoGenerTitle();
            }
            return "执行成功...";
        }
    }
}
