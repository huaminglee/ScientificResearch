using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.SDKFlowDemo.SDK
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack == false)
            {
                string userNo = this.Request.QueryString["UserNo"];
                if (string.IsNullOrEmpty(userNo))
                    this.TB_No.Text = userNo;
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            #region 此部分可以省略.
            BP.WF.Port.Emp emp = new BP.WF.Port.Emp();
            emp.No = this.TB_No.Text;
            if (emp.IsExits == false)
            {
                this.ToMsg("用户名("+emp.No+")不存在....");
                return;
            }
            #endregion 此部分可以省略.

            //取到用户编号.
            string userNo = this.TB_No.Text;
            // 调用登录接口,写入登录信息。
            BP.WF.Dev2Interface.Port_Login(userNo);

            //转到待办.
            this.Response.Redirect("ToDoList.aspx", true);
        }

        #region 公用方法
        public void ToMsg(string msg)
        {
            System.Web.HttpContext.Current.Session["info"] = msg;
            System.Web.HttpContext.Current.Application["info" + BP.Web.WebUser.No] = msg;
            BP.WF.Glo.SessionMsg = msg;
            System.Web.HttpContext.Current.Response.Redirect(
                "/SDKFlowDemo/SDK/Info.aspx?FK_Flow=2&FK_Type=2&FK_Node=2&WorkID=22" + DateTime.Now.ToString(), false);
        }
        public void ToErrorPage(string msg)
        {
            System.Web.HttpContext.Current.Session["info"] = msg;
            System.Web.HttpContext.Current.Application["info" + BP.Web.WebUser.No] = msg;
            BP.WF.Glo.SessionMsg = msg;
            System.Web.HttpContext.Current.Response.Redirect("/SDKFlowDemo/SDK/ErrorPage.aspx", false);
        }
        #endregion 公用方法
    }
}