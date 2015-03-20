<%@ Control Language="C#" AutoEventWireup="true" Inherits="CCFlow.WF.UC.FHLFlow"
    CodeBehind="FHLFlow.ascx.cs" %>
<%@ Register Src="./../Comm/UC/ToolBar.ascx" TagName="ToolBar" TagPrefix="uc1" %>
<%@ Register Src="UCEn.ascx" TagName="UCEn" TagPrefix="uc2" %>
<script type="text/javascript">
    //然浏览器最大化.
    function ResizeWindow() {
        if (window.screen) {  //判断浏览器是否支持window.screen判断浏览器是否支持screen     
            var myw = screen.availWidth;   //定义一个myw，接受到当前全屏的宽     
            var myh = screen.availHeight;  //定义一个myw，接受到当前全屏的高     
            window.moveTo(0, 0);           //把window放在左上脚     
            window.resizeTo(myw, myh);     //把当前窗体的长宽跳转为myw和myh     
        }
    }
    window.onload = ResizeWindow();
    function ReinitIframe(frmID, tdID) {
        try {
            var iframe = document.getElementById(frmID);
            var tdF = document.getElementById(tdID);
            iframe.height = iframe.contentWindow.document.body.scrollHeight;
            iframe.width = iframe.contentWindow.document.body.scrollWidth;
            if (tdF.width < iframe.width) {
                tdF.width = iframe.width;
            } else {
                iframe.width = tdF.width;
            }
            tdF.height = iframe.height;
            return;
        } catch (ex) {
            return;
        }
        return;
    }
    function GroupBarClick(rowIdx) {
        var alt = document.getElementById('Img' + rowIdx).alert;
        var sta = 'block';
        if (alt == 'Max') {
            sta = 'block';
            alt = 'Min';
        } else {
            sta = 'none';
            alt = 'Max';
        }
        document.getElementById('Img' + rowIdx).src = './Img/' + alt + '.gif';
        document.getElementById('Img' + rowIdx).alert = alt;
        var i = 0
        for (i = 0; i <= 40; i++) {
            if (document.getElementById(rowIdx + '_' + i) == null)
                continue;
            if (sta == 'block') {
                document.getElementById(rowIdx + '_' + i).style.display = '';
            } else {
                document.getElementById(rowIdx + '_' + i).style.display = sta;
            }
        }

    }
</script>
<script language="javascript">
    function Do(warning, url) {
        if (window.confirm(warning) == false)
            return;
        window.location.href = url;
        // WinOpen(url);
    }
</script>
<div id="tabForm">
    <div id="topBar">
        <uc1:ToolBar ID="ToolBar1" runat="server" />
    </div>
    <div id="divCCForm">
        <uc2:UCEn ID="UCEn1" runat="server" />
    </div>
</div>
