<%@ Page Title="待办" Language="C#" MasterPageFile="SiteMenu.Master" AutoEventWireup="true" CodeBehind="ToDoList.aspx.cs" Inherits="CCFlow.App.ToDoList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<h2>待办 (<%=BP.WF.Dev2Interface.Todolist_EmpWorks %>) </h2>
<%
   //获取待办。
   System.Data.DataTable dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable();
   // 输出结果
   %> <table border="1" widht="90%" >
   <tr>
   <th>是否读取?</th>
   <th>标题</th>
   <th>流程</th>
   <th>发起时间</th>
   <th>发起人</th>
   <th>停留节点</th>
   <th>(发送/抄送)人</th>
   <th>类型</th>
   </tr>
      <%
    // 生成timke 方式浏览器打开旧的界面，方式界面缓存.      
    string t = DateTime.Now.ToString("MMddhhmmss");
    foreach (System.Data.DataRow dr in dt.Rows)
    {
        //流程引擎的参数。
        string paras = dr["AtPara"] as string;
        if (paras == null)
            paras = "";
        //这个工作是否读取了？根据状态开发人员可以个性化的显示工作读取未读取效果.
        int isRead = int.Parse(dr["IsRead"].ToString());
        
        %>
        <tr>
        <td><%=dr["isRead"]%></td>
       
       <% if (isRead == 0)
          {%>
        <td><b> <a target="_blank" href='/WF/MyFlow.aspx?FK_Flow=<%=dr["FK_Flow"] %>&FK_Node=<%=dr["FK_Node"] %>&WorkID=<%=dr["WorkID"] %>&FID=<%=dr["FID"] %>&IsRead=<%=isRead%>&Paras=<%=paras %>&T=<%=t %>' ><%=dr["Title"].ToString()%> </a>  </b></td>
        <% }
          else
          { %>
        <td><a target="_blank" href='/WF/MyFlow.aspx?FK_Flow=<%=dr["FK_Flow"] %>&FK_Node=<%=dr["FK_Node"] %>&WorkID=<%=dr["WorkID"] %>&FID=<%=dr["FID"] %>&IsRead=<%=isRead%>&Paras=<%=paras %>&T=<%=t %>' ><%=dr["Title"].ToString()%> </a>  </td>
        <%} %>

        <td><%=dr["FlowName"]%></td>
        <td><%=dr["RDT"]%></td>
        <td><%=dr["StarterName"]%></td>
        <td><%=dr["NodeName"]%></td>

       
   <td><%=dr["Sender"]%></td>


        <% if (paras.Contains("IsCC")) { %>

        <td>抄送</td>

        <% } else { %>
        
        <td>发送</td>
        
        <%} %>


        </tr>
   <% } %> 
   </table>

</asp:Content>
