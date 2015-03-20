<%@ Control Language="C#" AutoEventWireup="true" Inherits="CCFlow.WF.UC.Login" Codebehind="Login.ascx.cs" %>
<script  language=javascript>
function ExitAuth( fk_emp )
{
   if (window.confirm('您确定要退出授权登陆模式吗？')==false)
       return;
       
    var url='Do.aspx?DoType=ExitAuth&FK_Emp='+fk_emp;
    WinShowModalDialog(url,'');
    window.location.href='Tools.aspx';
}
function NoSubmit(fk_emp) {
}
</script>

