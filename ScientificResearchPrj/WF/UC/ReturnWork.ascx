<%@ Control Language="C#" AutoEventWireup="true" Inherits="CCFlow.WF.UC.UCReturnWork"
    CodeBehind="ReturnWork.ascx.cs" %>
<%@ Register Src="../Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<%@ Register Src="./../Comm/UC/ToolBar.ascx" TagName="ToolBar" TagPrefix="uc3" %>
<div align="center">
    <div align="center" style='height: 30px;'>
        <uc3:ToolBar ID="ToolBar1" runat="server" />
    </div>
    <div style='height: 4px;'>
    </div>
    <div>
        <uc1:Pub ID="Pub1" runat="server" />
    </div>
</div>
<script src="../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
<script src="../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
<script type="text/javascript">
    function OnChange(ctrl) {
        var text = ctrl.options[ctrl.selectedIndex].text;
        var user = text.substring(0, text.indexOf('='));
        var nodeName = text.substring(text.indexOf('>') + 1, 1000);
        var objVal = '您好' + user + ':';
        objVal += "  \t\n ";
        objVal += "  \t\n ";
        objVal += "   您处理的 “" + nodeName + "” 工作有错误，需要您重新办理． ";
        objVal += "\t\n   \t\n 礼! ";
        objVal += "  \t\n ";

        try {
            document.getElementById("<%=TBClientID %>").value = objVal;
        } catch (e) {
        }
    }

    function TBHelp(ctrl, enName, attrKey) {
        var explorer = window.navigator.userAgent;
        var url = "../Comm/HelperOfTBEUI.aspx?EnsName=" + enName + "&AttrKey=" + attrKey + "&WordsSort=1" + "&FK_Flow=<%=FK_Flow %>" + "&id=" + ctrl;
        var str;
        if (explorer.indexOf("Chrome") >= 0) {
            window.open(url, "sd", "left=200,height=500,top=150,width=400,location=yes,menubar=no,resizable=yes,scrollbars=yes,status=no,toolbar=no");
        } else {
            str = window.showModalDialog(url, "sd", "dialogHeight:500px;dialogWidth:400px;dialogTop:150px;dialogLeft:200px;center:no;help:no");
            if (str == undefined)
                return;

            $("*[id$=" + ctrl + "]").focus().val(str);
        }
    }
</script>
