<%@ Page Language="c#" Inherits="CCFlow.Web.WF.Comm.UIContrastDtl" CodeBehind="ContrastDtl.aspx.cs" %>

<%@ Register TagPrefix="uc1" TagName="UCSys" Src="UC/UCSys.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="BP.Web.Controls" Assembly="BP.Web.Controls" %>
<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head id="Head1" runat="server">
    <title>œÍœ∏–≈œ¢</title>
    <link href="Style/Table0.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script language="JavaScript" src="JScript.js"></script>
    <script language="javascript">
        function ShowEn(url, wName) {
            val = window.showModalDialog(url, wName, 'dialogHeight: 550px; dialogWidth: 650px; dialogTop: 100px; dialogLeft: 150px; center: yes; help: no');
            // window.location.href=window.location.href;
        }
    </script>
    <base target="_self" />
</head>
<body class="easyui-layout">
    <form id="Form1" method="post" runat="server">
    <div data-options="region:'center',border:false,title:'<%=this.ShowTitle %>'">
        <uc1:UCSys ID="UCSys1" runat="server"></uc1:UCSys>
    </div>
    </form>
</body>
</html>
