﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WorkCheck.aspx.cs" Inherits="CCFlow.WF.WorkOpt.FrmWorkCheck" %>

<%@ Register Src="../Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<script type="text/javascript" language="javascript">
    function NoSubmit(ev) {
        if (window.event.srcElement.tagName == "TEXTAREA")
            return true;
        if (ev.keyCode == 13) {
            window.event.keyCode = 9;
            ev.keyCode = 9;
            return true;
        }
        return true;
    }

    function show_and_hide_tr(tb_id, obj) {
        $("#" + tb_id).find("tr").each(function (i) {
            i > 0 ? (this.style.display == "none" ? this.style.display = "" : this.style.display = "none") : ($(this).next().css("display") == "none" ? (obj.src = '../Img/Tree/Cut.gif') : (obj.src = '../Img/Tree/Add.gif'));
        });
    }

    function TBHelp(ctrl, enName, attrKey) {
        var explorer = window.navigator.userAgent;
        var str = "";
        var url = "../Comm/HelperOfTBEUI.aspx?EnsName=" + enName + "&AttrKey=" + attrKey + "&WordsSort=0" + "&FK_Flow=<%=FK_Flow %>" + "&id=" + ctrl;
        if (explorer.indexOf("Chrome") >= 0) {
             window.open(url, "sd", "left=200,height=500,top=150,width=400,location=yes,menubar=no,resizable=yes,scrollbars=yes,status=no,toolbar=no");
        }
        else {
            str = window.showModalDialog(url, 'sd', 'dialogHeight: 500px; dialogWidth:400px; dialogTop: 150px; dialogLeft: 200px; center: no; help: no');
            if (str == undefined)
                return;

            $("*[id$=" + ctrl + "]").focus().val(str);
        }
    }

</script>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <link href="../Style/FormThemes/Table0.css" rel="stylesheet" type="text/css" />
</head>
<body leftmargin="0" topmargin="0">
    <form id="form1" runat="server">
    <uc1:Pub ID="Pub1" runat="server" />
    </form>
</body>
</html>
