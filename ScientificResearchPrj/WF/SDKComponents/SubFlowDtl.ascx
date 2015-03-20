<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SubFlowDtl.ascx.cs" Inherits="CCFlow.WF.SDKComponents.SubFlowDtl" %>
<script type="text/javascript">
    function Del(fk_flow, workid) {
        if (window.confirm('您确定要删除吗？') == false)
            return;
    }
</script>
<div id="DelMsg" />