<%@ Page Title="" Language="C#" MasterPageFile="SDKComponents/Site.master" AutoEventWireup="true" Inherits="CCFlow.WF.WF_FHLFlow" Codebehind="FHLFlow.aspx.cs" %>
<%@ Register src="UC/FHLFlow.ascx" tagname="FHLFlow" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script language="JavaScript" src="Comm/JScript.js" type="text/javascript" ></script>
    <link href="Comm/Style/Table.css" rel="stylesheet" type="text/css" />
    <link href="Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
    <link href="Style/FormThemes/MyFlow.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    
 <table  style=" text-align:left; width:100%">
<caption>您好:<%=BP.WF.Glo.GenerUserImgSmallerHtml(BP.Web.WebUser.No,BP.Web.WebUser.Name) %></caption>
<tr>
<td>
<uc1:FHLFlow ID="FHLFlow1" runat="server" />
</td>
</tr>
</table>
</asp:Content>

