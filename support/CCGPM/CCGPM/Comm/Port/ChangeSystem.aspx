<%@ Register TagPrefix="uc1" TagName="UCSys" Src="../UC/UCSys.ascx" %>
<%@ Page language="c#" Inherits="BP.Web.Comm.ChangeSystem" Codebehind="ChangeSystem.aspx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="BP.Web.Controls" Assembly="BP.Web.Controls" %>
<!DocType HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>ChangeSystem</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../Style.css" type="text/css" rel="stylesheet">
		<LINK href="../Table.css" type="text/css" rel="stylesheet">
		<script language=javascript>
<!--
//-->
</script>

	</HEAD>
	<body class="Body<%=BP.Web.WebUser.Style%>"  topmargin="0" leftmargin="0">
		<form id="Form1" method="post" runat="server">
			<TABLE id="Table1" align="left" height='100%' cellSpacing="1" cellPadding="1" width="100%"
				border="1">
				<TR Width="100%">
					<TD Width="100%" colSpan="2" height='1%'>
						<asp:Label id="Label1" runat="server">Label</asp:Label><FONT face="����"></FONT></TD>
				</TR>
				<TR>
					<TD bgcolor=ActiveBorder style="WIDTH: 30%" valign="top">
						<P>&nbsp;&nbsp;&nbsp;</P>
						<P><FONT face="����">&nbsp;&nbsp;&nbsp; ����ߵ�ϵͳ�б�, <FONT face="����">��Щϵͳ֮����������һ�𣬹���������ݣ������š�Ա����������λ��ְ�񡢵���Ϣ�������ݵ������ԡ���ʱ�Եõ��˱�֤��
						</P>
						<P>&nbsp;&nbsp;&nbsp; ��һ��ϵͳת������һ��ϵͳ��ֻ��Ҫһ���л���</P>
						<P>&nbsp;&nbsp;&nbsp; û�����ӵ�ϵͳ���Ǳ�ϵͳ.</P>
						</FONT></FONT>
					</TD>
					<TD vAlign="top" width="70%">
						<P><FONT face="����">
								<uc1:UCSys id="UCSys1" runat="server"></uc1:UCSys></P>
						</FONT><FONT face="����"></FONT></TD>
				</TR>
			</TABLE>
			</FONT>
		</form>
	</body>
</HTML>
