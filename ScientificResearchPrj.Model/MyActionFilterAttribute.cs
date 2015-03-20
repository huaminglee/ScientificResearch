using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScientificResearchPrj.Model
{
    //AllowMultiple = true:允许多个标签同时都起作用
    [AttributeUsage(AttributeTargets.All,AllowMultiple = true)]
    public class MyActionFilterAttribute : ActionFilterAttribute
    {

        //Action 执行之前先执行此方法
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ///当前访问首页，直接执行
            if (HttpContext.Current.Request.RawUrl.IndexOf("/Home") != -1)
            {
                base.OnActionExecuting(filterContext); 
            }
            ///当前访问的不是首页
            else
            {
                ///当前账户不为空，直接执行
                if (BP.Web.WebUser.No != null)
                {
                    base.OnActionExecuting(filterContext); 
                }
                ///当前账户为空，重新登录
                else
                {
                    HttpContext.Current.Response.Redirect("/Home/Index");
                }
            }

           
        }

        //Action执行之后
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
           
        }

        //ActionResult执行之前先执行此方法
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
        }
    }
}