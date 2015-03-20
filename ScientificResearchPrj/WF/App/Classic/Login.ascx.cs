using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Web;
using BP.En;
using BP.DA;
using BP.WF;
using BP.Sys;
using BP.Port;
using BP;

public partial class App_control_Login1 : BP.Web.UC.UCBase3
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.Page.RegisterStartupScript("event_handler", "<script>document.body.onkeypress = keyPressed;</script>");
        this.Page.RegisterClientScriptBlock("default_button", "<script> function keyPressed() { if(window.event.keyCode == 13) { document.getElementById(\""
            + this.lbtnSubmit.ClientID + "\").click(); } } </script>");
        if (WebUser.No != null)
        {
            txtUserName.Text = WebUser.No;
        }
    }
    protected void lbtnSubmit_Click(object sender, EventArgs e)
    {
       
    }
    public void Login(string strUser, string strPass)
    {
        txtUserName.Text = strUser;
        txtPassword.Text= strPass;
        Login();
    }
    private void Login()
    {
        string user = txtUserName.Text.Trim();
        string pass = txtPassword.Text.Trim();
        try
        {
            //关闭已登录用户
            if (WebUser.No != null) WebUser.Exit();

            Emp em = new Emp(user);
            if (em.CheckPass(pass))
            {
                WebUser.SignInOfGenerLang(em, WebUser.SysLang);

                if (this.Request.RawUrl.ToLower().Contains("wap"))
                    WebUser.IsWap = true;
                else
                    WebUser.IsWap = false;

                WebUser.Token = this.Session.SessionID;

                Response.Redirect("Default.aspx", false);
                return;
            }
            this.Alert("用户名密码错误，注意密码区分大小写，请检查是否按下了CapsLock.。");
        }
        catch (System.Exception ex)
        {
            this.Alert("@用户名密码错误!@检查是否按下了CapsLock.@更详细的信息:" + ex.Message);
        }
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        Login();
    }
}