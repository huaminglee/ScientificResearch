<%@ Page Title="" Language="C#" MasterPageFile="../WinOpen.master" AutoEventWireup="true" CodeBehind="BatchStartFields.aspx.cs" Inherits="CCFlow.WF.Admin.BatchStartFields" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<h3>此功能的界面化配置还没有完成，请把字段英文名称按照顺序填写到节点属性里。</h3>
    <uc1:Pub ID="Pub1" runat="server" />
</asp:Content>
