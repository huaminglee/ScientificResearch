<%@ Page Language="c#" Inherits="CCFlow.Web.Comm.GroupEnsNum" CodeBehind="Group.aspx.cs" %>

<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register TagPrefix="cc1" Namespace="BP.Web.Controls" Assembly="BP.Web.Controls" %>
<%@ Register TagPrefix="uc1" TagName="UCSys" Src="UC/UCSys.ascx" %>
<%@ Register Src="UC/ToolBar.ascx" TagName="ToolBar" TagPrefix="uc2" %>
<%@ Register Src="UC/UCGraphics.ascx" TagName="UCGraphics" TagPrefix="uc3" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>分组分析</title>
    <link href="Style/Table0.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="JS/Calendar/WdatePicker.js" type="text/javascript"></script>
    <script src="../Scripts/EasyUIUtility.js" type="text/javascript"></script>
    <script language="JavaScript" src="JScript.js"></script>
    <base target="_self" />
    <script type="text/javascript">
        //  事件.
        function DDL_mvals_OnChange(ctrl, ensName, attrKey) {

            var idx_Old = ctrl.selectedIndex;

            if (ctrl.options[ctrl.selectedIndex].value != 'mvals')
                return;
            if (attrKey == null)
                return;

            //alert(ensName);
            //alert(attrKey);

            var url = 'SelectMVals.aspx?EnsName=' + ensName + '&AttrKey=' + attrKey;
            var val = window.showModalDialog(url, 'dg', 'dialogHeight: 450px; dialogWidth: 450px; center: yes; help: no');

            if (val == '' || val == null) {
                // if (idx_Old==ctrl.options.cont
                ctrl.selectedIndex = 0;
                //    ctrl.options[0].selected = true;
            }
        }
    </script>
</head>
<body onkeypress="Esc()" class="easyui-layout">
    <form id="Form1" method="post" runat="server">
    <div id="mainDiv" data-options="region:'center',title:'<%=this.Title %>'" style="padding: 5px">
        <div class="easyui-layout" data-options="fit:true">
            <div data-options="region:'north',noheader:true,split:false" style="padding: 2px;
                height: auto; background-color: #E0ECFF; line-height: 30px">
                <uc2:ToolBar ID="ToolBar1" runat="server" />
            </div>
            <div data-options="region:'west',title:'分组条件',split:true" style="padding: 5px; width: 260px;">
                <div class="easyui-panel" title="显示内容" style="padding: 5px; margin-bottom: 5px">
                    <asp:CheckBoxList ID="CheckBoxList1" runat="server" AutoPostBack="true" BorderStyle="None"
                        Width="100%">
                    </asp:CheckBoxList>
                </div>
                <div class="easyui-panel" title="分析项目" style="padding: 5px; margin-bottom: 5px">
                    <uc1:UCSys ID="UCSys2" runat="server"></uc1:UCSys>
                </div>
                <div class="easyui-panel" title="图表" style="padding: 5px; margin-bottom: 5px">
                    <table class="Table" cellpadding="0" cellspacing="0" border="0" style="border-style: none;
                        width: 100%">
                        <tr>
                            <td colspan="2">
                                <asp:CheckBox ID="CB_IsShowPict" runat="server" Text="显示图表" AutoPostBack="true" ToolTip="注意：仅当“显示内容”选择1项时，图表功能才可用！" />
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 80px">
                                高度:
                            </td>
                            <td>
                                <asp:TextBox ID="TB_H" runat="server" Text="460" Style="width: 100px; height: auto;
                                    text-align: right"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                宽度:
                            </td>
                            <td>
                                <asp:TextBox ID="TB_W" runat="server" Text="500" Style="width: 100px; height: auto;
                                    text-align: right"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="text-align: right">
                                <asp:LinkButton ID="lbtnApply" runat="server" CssClass="easyui-linkbutton" data-options="iconCls:'icon-ok'"
                                    OnClick="lbtnApply_Click">应用</asp:LinkButton>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <div data-options="region:'center'" style="padding: 5px;">
                <uc1:UCSys ID="UCSys3" runat="server"></uc1:UCSys>
                <uc1:UCSys ID="UCSys1" runat="server"></uc1:UCSys>
                <uc1:UCSys ID="UCSys4" runat="server"></uc1:UCSys>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
