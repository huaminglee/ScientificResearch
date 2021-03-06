﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.En;
using BP.DA;

public partial class Comm_Sys_EnumList : BP.Web.WebPageAdmin
{
    public void BindRefNo()
    {
        SysEnumMain sem = new SysEnumMain(this.RefNo);
        this.UCSys1.AddTable();
        this.UCSys1.AddCaptionLeft("<a href=EnumList.aspx ><img src='./../../Images/Btn/Home.gif' border=0>枚举值列表</a> -<a href='EnumList.aspx?DoType=New' ><img src='./../../Images/Btn/New.gif' border=0/>新建</a>- <img src='./../../Images/Btn/Edit.gif' border />编辑:" + sem.No + " " + sem.Name);

        this.UCSys1.AddTR();
        Button btn = new Button();
        btn.ID = "Btn_Save";
        btn.CssClass = "Btn";
        btn.Text = "  Save  ";
        btn.Click += new EventHandler(btn_Click);
        this.UCSys1.AddTDTitle("colspan=3", btn);
        this.UCSys1.AddTREnd();

        this.UCSys1.AddTR();
        this.UCSys1.AddTDTitle("项目");
        this.UCSys1.AddTDTitle("采集");
        this.UCSys1.AddTDTitle("说明");
        this.UCSys1.AddTREnd();

        SysEnums ses = new SysEnums();
        ses.Retrieve(SysEnumAttr.EnumKey, this.RefNo);

        this.UCSys1.AddTRSum();
        this.UCSys1.AddTD("编号");
        TextBox tb = new TextBox();
        tb.ID = "TB_No";
        tb.Text = this.RefNo;
        tb.Enabled = false;
        this.UCSys1.AddTD(tb);
        this.UCSys1.AddTD("不可修改");
        this.UCSys1.AddTREnd();

        this.UCSys1.AddTRSum();
        this.UCSys1.AddTD("名称");
        tb = new TextBox();
        tb.ID = "TB_Name";
        tb.Text = sem.Name;
        this.UCSys1.AddTD(tb);
        this.UCSys1.AddTD("");
        this.UCSys1.AddTREnd();

        int myNum = 0;
        foreach (SysEnum se in ses)
        {
            this.UCSys1.AddTR();
            this.UCSys1.AddTD(se.IntKey);
            tb = new TextBox();
            tb.ID = "TB_" + se.IntKey;
            tb.Text = se.Lab;
            tb.Columns = 50;
            this.UCSys1.AddTD(tb);
            this.UCSys1.AddTD("");
            this.UCSys1.AddTREnd();
            myNum = se.IntKey;
        }

        myNum++;

        for (int i = myNum; i < 20; i++)
        {
            this.UCSys1.AddTR();
            this.UCSys1.AddTD(i);
            tb = new TextBox();
            tb.ID = "TB_" + i;
            tb.Columns = 50;
            this.UCSys1.AddTD(tb);
            this.UCSys1.AddTD("");
            this.UCSys1.AddTREnd();
        }
        this.UCSys1.AddTableEnd();
    }
    public void BindNew()
    {
        this.UCSys1.AddTable();
        this.UCSys1.AddCaptionLeftTX("<a href=EnumList.aspx ><img src='./../../Images/Btn/Home.gif' border=0 />枚举值列表</a> - <img src='./../../Images/Btn/New.gif' />新建枚举值");

        this.UCSys1.AddTR();
        Button btn = new Button();
        btn.ID = "Btn_Save";
        btn.CssClass = "Btn";
        btn.Text = "  Save  ";
        btn.Click += new EventHandler(btn_New_Click);
        this.UCSys1.AddTDTitle("colspan=3", btn);
        this.UCSys1.AddTREnd();

        this.UCSys1.AddTR();
        this.UCSys1.AddTDTitle("项目");
        this.UCSys1.AddTDTitle("采集");
        this.UCSys1.AddTDTitle("说明");
        this.UCSys1.AddTREnd();

        this.UCSys1.AddTRSum();
        this.UCSys1.AddTD("编号");
        TextBox tb = new TextBox();
        tb.ID = "TB_No";
        this.UCSys1.AddTD(tb);
        this.UCSys1.AddTD("编号系统唯一并且以字母或下划线开头");
        this.UCSys1.AddTREnd();

        this.UCSys1.AddTRSum();
        this.UCSys1.AddTD("名称");
        tb = new TextBox();
        tb.ID = "TB_Name";
        this.UCSys1.AddTD(tb);
        this.UCSys1.AddTD("不能为空");
        this.UCSys1.AddTREnd();
        for (int i = 0; i < 20; i++)
        {
            this.UCSys1.AddTR();
            this.UCSys1.AddTD(i);
            tb = new TextBox();
            tb.ID = "TB_" + i;
            tb.Columns = 50;
            this.UCSys1.AddTD(tb);
            this.UCSys1.AddTD("");
            this.UCSys1.AddTREnd();
        }
        this.UCSys1.AddTableEnd();
    }

    void btn_Click(object sender, EventArgs e)
    {
        SysEnums ses = new SysEnums();
        for (int i = 0; i < 20; i++)
        {
            TextBox tb = this.UCSys1.GetTextBoxByID("TB_" + i);
            if (tb == null)
                continue;
            if (string.IsNullOrEmpty(tb.Text))
                continue;

            SysEnum se = new SysEnum();
            se.IntKey = i;
            se.Lab = tb.Text.Trim();
            se.Lang = BP.Web.WebUser.SysLang;
            se.EnumKey = this.RefNo;
            se.MyPK = se.EnumKey + "_" + se.Lang + "_" + se.IntKey;
            ses.AddEntity(se);
        }

        if (ses.Count == 0)
        {
            this.Alert("枚举项目不能为空.");
            return;
        }

        ses.Delete(SysEnumAttr.EnumKey, this.RefNo);

        string lab = "";
        foreach (SysEnum se in ses)
        {
            se.Save();
            lab += "@" + se.IntKey + "=" + se.Lab;
        }
        SysEnumMain main = new SysEnumMain(this.RefNo);
        main.CfgVal = lab;
        main.Update();
        this.Alert("保存成功.");
    }

    void btn_New_Click(object sender, EventArgs e)
    {
        string no = this.UCSys1.GetTextBoxByID("TB_No").Text;
        string name = this.UCSys1.GetTextBoxByID("TB_Name").Text;
        SysEnumMain m = new SysEnumMain();
        m.No = no;
        if (m.RetrieveFromDBSources() == 1)
        {
            this.Alert("枚举编号:" + m.No + " 已经被:" + m.Name + "占用");
            return;
        }
        m.Name = name;
        if (string.IsNullOrEmpty(name))
        {
            this.Alert("枚举名称不能为空");
            return;
        }

        SysEnums ses = new SysEnums();
        for (int i = 0; i < 20; i++)
        {
            TextBox tb = this.UCSys1.GetTextBoxByID("TB_" + i);
            if (tb == null)
                continue;
            if (string.IsNullOrEmpty(tb.Text))
                continue;

            SysEnum se = new SysEnum();
            se.IntKey = i;
            se.Lab = tb.Text.Trim();
            se.Lang = BP.Web.WebUser.SysLang;
            se.EnumKey = m.No;
            se.MyPK = se.EnumKey + "_" + se.Lang + "_" + se.IntKey;
            ses.AddEntity(se);
        }

        if (ses.Count == 0)
        {
            this.Alert("枚举项目不能为空.");
            return;
        }

        string lab = "";
        foreach (SysEnum se in ses)
        {
            se.Save();
            lab += "@" + se.IntKey + "=" + se.Lab;
        }

        m.Lang = BP.Web.WebUser.SysLang;
        m.CfgVal = lab;
        m.Insert();
        this.Response.Redirect("EnumList.aspx?RefNo=" + m.No, true);
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        this.Title = "枚举值编辑";
        if (this.DoType == "Del")
        {
            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.UIBindKey, this.RefNo);
            if (attrs.Count != 0)
            {
                this.UCSys1.AddFieldSet("<a href='EnumList.aspx' ><img src='./../../Images/Btn/Home.gif' border=0/>返回列表</a> - 删除确认");
                this.UCSys1.Add("此枚举值已经被其它的字段所引用，您不能删除它。");
                this.UCSys1.AddH2("<a href='EnumList.aspx' >返回列表</a>");
                this.UCSys1.AddFieldSetEnd();
                return;
            }

            this.UCSys1.AddFieldSet("<a href='EnumList.aspx' ><img src='./../../Images/Btn/Home.gif' border=0/>返回列表</a> - 删除确认");
            SysEnumMain m = new SysEnumMain(this.RefNo);
            this.UCSys1.AddH2("<a href='EnumList.aspx?RefNo=" + this.RefNo + "&DoType=DelReal' >删除:" + m.Name + " 确认.</a>");
            this.UCSys1.AddFieldSetEnd();
            return;
        }

        if (this.DoType == "DelReal")
        {
            SysEnumMain m = new SysEnumMain();
            m.No = this.RefNo;
            m.Delete();
            SysEnums ses = new SysEnums();
            ses.Delete(SysEnumAttr.EnumKey, this.RefNo);
            this.Response.Redirect("EnumList.aspx", true);
            return;
        }

        if (this.DoType == "New")
        {
            this.BindNew();
            return;
        }

        if (this.RefNo != null)
        {
            this.BindRefNo();
            return;
        }

        this.UCSys1.AddTable();
        this.UCSys1.AddCaptionLeftTX("<img src='./../../Images/Btn/Home.gif' border=0/>列表 - <a href='EnumList.aspx?DoType=New' ><img border=0 src='./../../Images/Btn/New.gif' >新建</a>");
        this.UCSys1.AddTR();
        this.UCSys1.AddTDTitle("序");
        this.UCSys1.AddTDTitle("编号");
        this.UCSys1.AddTDTitle("名称");
        this.UCSys1.AddTDTitle("信息");
        this.UCSys1.AddTDTitle("操作");
        this.UCSys1.AddTREnd();

        SysEnumMains sems = new SysEnumMains();
        sems.RetrieveAll();
        int i = 0;
        foreach (SysEnumMain se in sems)
        {
            i++;
            this.UCSys1.AddTR();
            this.UCSys1.AddTDIdx(i);
            this.UCSys1.AddTD(se.No);
            this.UCSys1.AddTDA("EnumList.aspx?RefNo=" + se.No, se.Name);
            this.UCSys1.AddTD(se.CfgVal);
            this.UCSys1.AddTDA("EnumList.aspx?RefNo=" + se.No + "&DoType=Del", "<img src='./../../Images/Btn/Delete.gif' border=0 />删除");
            this.UCSys1.AddTREnd();
        }
        this.UCSys1.AddTableEnd();
    }
}