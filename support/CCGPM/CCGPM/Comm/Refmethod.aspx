<%@ Register TagPrefix="uc1" TagName="UCEn" Src="UC/UCEn.ascx" %>

<%@ Page Language="c#" Inherits="BP.Web.Comm.MyRefMethod" CodeBehind="Refmethod.aspx.cs" %>

<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<!DocType HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
    <title>Esc ���رմ���.</title>
    <meta content="Microsoft FrontPage 5.0" name="GENERATOR" />
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <script language="JavaScript" src="JScript.js"></script>
    <script language="JavaScript" src="Menu.js"></script>
    <script language="JavaScript" src="ActiveX.js"></script>
    <base target="_self" />
    <link href="./Style/Table.css" type="text/css" rel="stylesheet">
    <link href="./Style/Table0.css" type="text/css" rel="stylesheet">

</head>
<body onkeypress="javascript:Esc();"  leftmargin="0" topmargin="0">
    <form id="Form1" method="post" runat="server">
    <table id="Table1" cellspacing="1" cellpadding="1" width="100%" border="1" class="Table"
        border="0">
        <caption >
                <asp:Label ID="Label1" runat="server">Label</asp:Label>
           </caption>
        <tr>
            <td class="TD">
                <asp:Button ID="btnSave" runat="server" Text="����" OnClick="BPToolBar1_ButtonClick" />
                <asp:Button ID="btnClose" runat="server" Text="�ر�" OnClick="btnClose_Click" />
            </td>
        </tr>
        <tr>
            <td class="TD">
                <uc1:UCEn ID="UCEn1" runat="server"></uc1:UCEn>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
