<%@ Page Title="流程跳转" Language="C#" MasterPageFile="../WinOpen.master" AutoEventWireup="true" CodeBehind="FlowSkip.aspx.cs" Inherits="CCFlow.WF.Admin.WatchOneFlow.FlowSkip" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<fieldset>
<legend>流程跳转
    
    </legend>
    跳转到节点：<asp:DropDownList ID="DDL_SkipToNode" runat="server">
    </asp:DropDownList>

    <br>跳转给(请输入人员编号)：
 <asp:TextBox ID="TB_SkipToEmp" 
    Rows=5 runat="server"></asp:TextBox>

     <br>
跳转原因：
     <br>
<asp:TextBox ID="TB_Note" TextMode="MultiLine" 
    Rows=5 runat="server" Width="308px"></asp:TextBox>
   
     <br>

    <asp:Button ID="Btn_OK" runat="server" Text="确定跳转" onclick="Btn_OK_Click" />
    <asp:Button ID="Btn_Cancel" runat="server" Text="取消" 
        onclick="Btn_Cancel_Click" />

</fieldset>
</asp:Content>
