<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FrmCheck.ascx.cs" Inherits="CCFlow.WF.App.Comm.FrmCheck" %>
<%
    string enName = "ND" + this.Request.QueryString["FK_Node"];
    string src = "/WF/WorkOpt/WorkCheck.aspx?FID=" + this.Request.QueryString["FID"];
    src += "&WorkID=" + this.Request.QueryString["WorkID"];
    src += "&FK_Node=" + this.Request.QueryString["FK_Node"];
    src += "&FK_Flow=" + this.Request.QueryString["FK_Flow"];
    src += "&IsHidden=" + this.IsHidden;
    string srcTrack = "/WF/WorkOpt/TrackChart.aspx?FK_Flow="+this.Request.QueryString["FK_Flow"];
    srcTrack += "&FK_Node=" + this.Request.QueryString["FK_Node"];
    srcTrack += "&WorkID=" + this.Request.QueryString["WorkID"];
    BP.Sys.FrmWorkCheck wc = new BP.Sys.FrmWorkCheck(int.Parse(this.Request.QueryString["FK_Node"]));

    int nodeID = int.Parse(this.Request.QueryString["FK_Node"]);
    
    BP.Sys.FrmWorkCheck frmWorkCheck = new BP.Sys.FrmWorkCheck(nodeID);
    
    if (wc.HisFrmWorkCheckSta == BP.Sys.FrmWorkCheckSta.Disable==false)
    {
%>

<div id="tt" class="easyui-tabs"  style="width: <%=frmWorkCheck.FWC_Wstr%>; height:<%= frmWorkCheck.FWC_Hstr %>;">
    <div title="审核信息" id='CheckInfo'   >
        <iframe id='F' src='<%=src%>' frameborder="0" style=' padding: 0px; border: 0px; margin:0px; height:99%'
            leftmargin='0' topmargin='0' width='100%'   >
        </iframe>

    </div>
    <% if (wc.FWCTrackEnable == true)
       { %>
    <div title="运行轨迹图" data-options="closable:false" >
        <iframe id='Iframe1' src='<%=srcTrack%>' frameborder="0" style=' overflow:hidden;padding: 0; margin:0px; border: 0px; height:99%'
            leftmargin='0' topmargin='0' width='100%'  >
        </iframe>
    </div>
    <%} %>
</div>
<% } %>
