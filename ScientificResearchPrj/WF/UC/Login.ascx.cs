﻿using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using BP.Web;
using BP.En;
using BP.DA;
using BP.WF;
using BP.Sys;
using BP.Port;
using BP;

namespace CCFlow.WF.UC
{
    public partial class Login : BP.Web.UC.UCBase3
    {
        public string Lang
        {
            get
            {
                string s = this.Request.QueryString["Lang"];
                if (s == null)
                    return WebUser.SysLang;
                return s;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string userNo = this.Request.QueryString["UserNo"];
            if (userNo != null && userNo.Length > 1)
            {
                string sid = this.Request.QueryString["SID"];
                if (WebUser.CheckSID(userNo,sid) == true)
                {
                    Emp emp = new Emp(userNo);
                    BP.Web.WebUser.SignInOfGener(emp);
                    BP.Web.WebUser.SID = sid;
                    Response.Redirect(this.ToWhere, false);
                    return;
                }
            }

            if (this.Request.Browser.Cookies == false)
            {
                //this.Alert("您的浏览器不支持cookies功能，无法使用该系统。");
                //return;
            }

            // if (this.DoType == "Logout")
            if (this.DoType != null)
            {
                BP.Web.WebUser.Exit();
                //this.Response.Redirect(this.PageID + ".aspx?DoType=del", true);
                //return;
            }

            WebUser.SysLang = this.Lang;
            Response.AddHeader("P3P", "CP=CAO PSA OUR");
            int colspan = 1;

            this.AddTable("border=0");

            this.AddTR();
            this.Add("<TD class=C align=left colspan=" + colspan + "><img src='/WF/Img/Login.gif' > <b>系统登陆</b></TD>");
            this.AddTREnd();
            this.AddTR();
            this.Add("<TD align=center >");

            this.AddTable("border=0px align=center ");
            this.AddTR();
            this.AddTD(  "用户名：");

            TextBox tb = new TextBox();
            tb.ID = "TB_User";
            tb.Text = BP.Web.WebUser.No;
            tb.Columns = 20;

            this.AddTD(tb);
            this.AddTREnd();

            this.AddTR();
            this.AddTD( "密&nbsp;&nbsp;码：" );
            tb = new TextBox();
            tb.ID = "TB_Pass";
            tb.TextMode = TextBoxMode.Password;
            tb.Columns = 22;
            this.AddTD(tb);
            this.AddTREnd();

            this.AddTRSum();
            this.AddTDBegin("colspan=3 align=center");
            Button btn = new Button();
            btn.CssClass = "Btn";
            btn.Text = "登 陆";

            btn.Click += new EventHandler(btn_Click);
            this.Add(btn);
            if (WebUser.No != null)
            {
                string home = "";
                if (WebUser.IsWap)
                    home = "-<a href='Home.aspx'>Home</a>";

                if (WebUser.IsAuthorize)
                    this.Add(" - <a href=\"javascript:ExitAuth('" + WebUser.Auth + "')\" >退出授权模式[" + WebUser.Auth + "]</a>" + home);
                else
                    this.Add(" - <a href='Tools.aspx?RefNo=AutoLog' >授权方式登陆</a>" + home);

                this.Add(" - <a href='" + this.PageID + ".aspx?DoType=Logout' ><font color=green><b>安全退出</b></a>");
            }
            this.AddTDEnd();
            this.AddTREnd();
            this.AddTableEnd();

            this.AddBR();
            this.AddBR();

            this.AddTDEnd();
            this.AddTREnd();
            this.AddTableEnd();
        }
        public string ToWhere
        {
            get
            {
                if (this.Request.QueryString["ToWhere"] == null)
                {
                    if (this.Request.RawUrl.ToLower().Contains("small"))
                        return "EmpWorks.aspx";
                    else
                        return "EmpWorks.aspx";
                }
                else
                {
                    return this.Request.QueryString["ToWhere"];
                }
            }
        }
        void btn_Click(object sender, EventArgs e)
        {
            string user = this.GetTextBoxByID("TB_User").Text.Trim();
            string pass = this.GetTextBoxByID("TB_Pass").Text;
            try
            {

                Emp em = new Emp();
                em.No = user;
                if (em.RetrieveFromDBSources() == 0)
                {
                    this.Alert("用户名或密码错误，注意两者区分大小写，请检查是否按下了CapsLock。");
                    return;
                }
                if (em.CheckPass(pass))
                {
                    // 执行登陆.
                    WebUser.SignInOfGenerLang(em, this.Lang);
                    if (this.Request.RawUrl.ToLower().Contains("wap"))
                        WebUser.IsWap = true;
                    else
                        WebUser.IsWap = false;

                    WebUser.Token = this.Session.SessionID;
                    if (WebUser.IsWap)
                    {
                        Response.Redirect("Home.aspx", true);
                        return;
                    }
                    Response.Redirect(this.ToWhere, false);
                    return;
                }
                this.Alert("用户名或密码错误，注意两者区分大小写，请检查是否按下了CapsLock。");
            }
            catch (System.Exception ex)
            {
                this.Response.Write("<font color=red ><b>@用户名密码错误!@检查是否按下了CapsLock.@更详细的信息:" + ex.Message + "</b></font>");
            }
        }
    }
}