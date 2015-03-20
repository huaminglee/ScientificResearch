<%@ Page Title="撤销发送" Language="C#" MasterPageFile="~/WF/SDKComponents/Site.Master" AutoEventWireup="true" CodeBehind="UnSend.aspx.cs" Inherits="CCFlow.WF.WorkOpt.UnSend" %>
<%@ Register Src="../Pub.ascx" TagName="Pub" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
   <link href="../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
    <link href="../Comm/Style/Table.css" rel="stylesheet" type="text/css" />

 <table style=" text-align:left; width:100%">
<caption>您好:<%=BP.WF.Glo.GenerUserImgSmallerHtml(BP.Web.WebUser.No,BP.Web.WebUser.Name) %></caption>
<tr>
<td valign=top style="text-align:center">

<%
    // 参数.
    Int64 workid = Int64.Parse(this.Request.QueryString["WorkID"]);
    string flowNo = this.Request.QueryString["FK_Flow"];
    
 //   Int64 fid = Int64.Parse(this.Request.QueryString["FID"]);
   // int nodeID = int.Parse(this.Request.QueryString["FK_Node"]);

    string doIt = this.Request.QueryString["DoIt"];
    BP.WF.GenerWorkFlow gwf = new BP.WF.GenerWorkFlow(workid);
    BP.WF.Node nd = new BP.WF.Node(gwf.FK_Node);
    if (string.IsNullOrEmpty(doIt) == true)
    {
        %>
         <fieldset>
         <legend>节点撤销信息</legend>

         <ul>
         <li>流程标题：<%=gwf.Title %>   </li>
         <li>停留节点：<%=gwf.NodeName %></li>
         <li>工作人员：<%=gwf.TodoEmps %></li>
         </ul>
         
         <br>

         <center>
          [<a href='UnSend.aspx?DoIt=xxx&FK_Flow=<%=flowNo %>&WorkID=<%=workid %>'>确定撤销</a>] - [<a href='UnSend.aspx?FK_Flow=<%=flowNo %>&WorkID=<%=workid %>'>取消撤销</a>]</center>
          
         </fieldset>
        <%

    }
    else
    {
        /* 执行撤销... */
        try
        {
            string str1 = BP.WF.Dev2Interface.Flow_DoUnSend(flowNo, workid);
            BP.WF.Glo.ToMsg(str1);
            return;
        }
        catch (Exception ex)
        {
            BP.WF.Glo.ToMsgErr(ex.Message);
            return;
        }
    }
    
    /*
     *  songhonggang：来处理。
     *  要求：
     *  1，要有两个选择项 【确定撤销】【不撤销返回】
     *  2，如果是当前处理人员是分流节点的人员，就提示正确的信息。
     *  3，如果
     * 
     */
 %>
   
    
    </td>
</tr>
</table>

</asp:Content>
