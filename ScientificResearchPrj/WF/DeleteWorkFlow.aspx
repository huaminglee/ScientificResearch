<%@ Page Title="" Language="C#" MasterPageFile="SDKComponents/Site.Master" AutoEventWireup="true" CodeBehind="DeleteWorkFlow.aspx.cs" Inherits="CCFlow.WF.DeleteWorkFlow" %>
<%@ Register src="UC/DeleteWorkFlowUC.ascx" tagname="DeleteWorkFlowUC" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
 <script language="JavaScript" src="./Comm/JScript.js" type="text/javascript" ></script>
    <link href="Comm/Style/Table.css" rel="stylesheet" type="text/css" />
    <link href="Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<table style=" text-align:left; width:100%">
<caption>您好:<%=BP.WF.Glo.GenerUserImgSmallerHtml(BP.Web.WebUser.No,BP.Web.WebUser.Name) %></caption>
<tr>
<td>
<uc1:DeleteWorkFlowUC ID="DeleteWorkFlowUC1" runat="server" />
 </td>
</tr>
</table>
    
</asp:Content>
