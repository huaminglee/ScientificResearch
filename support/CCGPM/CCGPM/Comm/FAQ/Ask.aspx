<%@ Page language="c#" Inherits="BP.Web.WF.Portal.UIAsk" Codebehind="Ask.aspx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="BP.Web.Controls" Assembly="BP.Web.Controls" %>
<!DocType HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>�û�������</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../Rpt.css" type="text/css" rel="stylesheet">
		<LINK href="../Style.css" type="text/css" rel="stylesheet">
		<script language="JavaScript" src="../JScript.js"></script>
		<LINK href="../Table.css" type="text/css" rel="stylesheet">
		<LINK href="../CSS/Link.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body class="Body<%=BP.Web.WebUser.Style%>" topmargin="0" leftmargin="0">
		<form id="Form1" method="post" runat="server">
			<TABLE id="Table1" cellSpacing="1" cellPadding="1" Width="100%" Height="100%" border="1">
				<TR>
					<TD>
						<asp:Label id="Label1" runat="server">Label</asp:Label></TD>
				</TR>
				<TR>
					<TD>
						<cc1:BPToolBar id="BPToolBar1" runat="server" onbuttonclick="BPToolBar1_ButtonClick_1"></cc1:BPToolBar></TD>
				</TR>
				<TR class="D2">
					<TD><STRONG>����:</STRONG>
						<cc1:TB id="TB_Title" runat="server" Width="100%" BorderStyle="Inset"></cc1:TB><STRONG></STRONG></TD>
				</TR>
				<TR align="justify" height='100%'>
					<TD style="HEIGHT: 100%">
						<cc1:TB id="TB_Docs" runat="server" TextMode="MultiLine" Width="100%" Height="100%"></cc1:TB>
					</TD>
				</TR>
				<TR>
					<TD class="D2">
						1)������������\ʵ���ķ�ʽ���͸����ǣ�<BR>
						2)���������ܹ�����������߷����������������ǵ�������ҵ�Ǳ�ڵ����⣮<BR>
						3)�����Ĺ������������ʾ��Ǹ��<BR>
						4)Ҫ���ø���ʱ�ķ����벦�����绰<%=BP.Sys.SystemConfig.ServiceTel%><BR>
						5)�������Ҫ�طã����������ϵ��ʽд�������������森
					</TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
