<%@ Page Title="工作移交" 
Language="C#" MasterPageFile="../WinOpen.master" AutoEventWireup="true"
 CodeBehind="FlowShift.aspx.cs" Inherits="CCFlow.WF.Admin.WatchOneFlow.FlowShift" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  
  <fieldset>
  <legend>工作请选择要把此工作移交的人员</legend>

  请输入人员编号:<asp:TextBox ID="TB_Emp" runat="server" Width="159px">
  </asp:TextBox>
  <br>
  原因:
  <br>

  <asp:TextBox ID="TB_Note" TextMode=MultiLine runat="server" Width="336px" 
          Height="91px"></asp:TextBox>
  说明:只能输入一个人员编号.
&nbsp;<hr>

      <asp:Button ID="Btn_OK" runat="server" Text="确定移交" onclick="Btn_OK_Click" />
      <asp:Button ID="Btn_Cancel" runat="server" Text="取消并关闭" 
          onclick="Btn_Cancel_Click" />

  </fieldset>
</asp:Content>
