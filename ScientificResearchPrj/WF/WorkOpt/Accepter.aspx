<%@ Page Language="C#" MasterPageFile="../SDKComponents/AccSite.Master" AutoEventWireup="true"
    Inherits="CCFlow.WF.WF_Accepter" Title="接受人选择器" CodeBehind="Accepter.aspx.cs" %>

<%@ Register Src="../Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<%@ Register Assembly="BP.Web.Controls" Namespace="BP.Web.Controls" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../Scripts/CommonUnite.js" type="text/javascript"></script>
    <style type="text/css">
        .cart
        {
            float: right;
            position: relative;
            width: 80%;
            height: 100%;
            padding: 0px 10px;
        }
        .ctitle
        {
            text-align: center;
            color: #555;
            font-size: 18px;
            padding: 10px;
        }
    </style>
    <script type="text/javascript">
        //调用发送按钮
        function send() {
            var btn = window.opener.document.getElementById('ContentPlaceHolder1_MyFlowUC1_MyFlow1_ToolBar1_Btn_Send');
            if (btn) {
                window.opener.document.getElementById('ContentPlaceHolder1_MyFlowUC1_MyFlow1_ToolBar1_Btn_Send').click();
            }
            window.close();
        }
        function SetSelected(cb, ids) {
            var arrmp = ids.split(',');
            var arrObj = document.all;
            var isCheck = false;
            if (cb.checked)
                isCheck = true;
            else
                isCheck = false;
            for (var i = 0; i < arrObj.length; i++) {
                if (typeof arrObj[i].type != "undefined" && arrObj[i].type == 'checkbox') {
                    for (var idx = 0; idx <= arrmp.length; idx++) {
                        if (arrmp[idx] == '')
                            continue;
                        var cid = arrObj[i].name + ',';
                        var ctmp = arrmp[idx] + ',';
                        if (cid.indexOf(ctmp) > 1) {
                            arrObj[i].checked = isCheck;
                        }
                    }
                }
            }
        }

        //
        function LoadDataGridCallBack(js, scorp) {

            $("#pageloading").hide();
            if (js == "") js = "[]";
            //系统错误
            if (js.status && js.status == 500) {
                $("body").html("<b>访问页面出错，请联系管理员。<b>");
                return;
            }

            var pushData = eval('(' + js + ')');

            $('#cc').combobox({
                data: pushData.ddl,
                valueField: 'id',
                textField: 'text',
                onSelect: function (r) {
                    var ee = $('#checkedTt').tree('find', 'CheId');
                    var children = $('#checkedTt').tree('getChildren');
                    for (var i = 0; i < children.length; i++) {
                        if (children[i].id == r.id) {
                            return;
                        }
                    }
                    $('#checkedTt').tree('append', {
                        parent: ee.target,
                        data: [{
                            id: r.id,
                            iconCls: "icon-user",
                            text: r.text
                        }]
                    });
                }
            });
            $("#checkedTt").tree({
                idField: 'id',
                data: pushData.CheId,
                animate: true,
                lines: true,
                onClick: function (n) {
                    if (n.id != 'CheId') {
                        $('#checkedTt').tree('remove', n.target);
                    }
                }
            });
            $("#tt").tree({
                idField: 'id',
                iconCls: 'tree-folder',
                data: pushData.tt,
                animate: true,
                width: 300,
                height: 400,
                lines: true,
                onClick: function (node) {
                    var ee = $('#checkedTt').tree('find', 'CheId');
                    var children = $('#checkedTt').tree('getChildren');
                    if (node.attributes["IsParent"] == 0) {
                        for (var i = 0; i < children.length; i++) {
                            if (children[i].id == node.id) {
                                return;
                            }
                        }
                        $('#checkedTt').tree('append', {
                            parent: ee.target,
                            data: [{
                                id: node.id,
                                iconCls: "icon-user",
                                text: node.text
                            }]
                        });
                    }
                }
            });
        }

        //加载DeptTree
        function LoadTreeData() {
            //获取url传的值
            var FK_Node = Application.common.getArgsFromHref("FK_Node");
            var WorkID = Application.common.getArgsFromHref("WorkID");
            var FK_Flow = Application.common.getArgsFromHref("FK_Flow");
            var FID = Application.common.getArgsFromHref("FID");
            var ToNode = Application.common.getArgsFromHref("ToNode");
            var IsWinOpen = Application.common.getArgsFromHref("IsWinOpen");
            var FK_Dept = Application.common.getArgsFromHref("FK_Dept");
            var FK_Station = Application.common.getArgsFromHref("FK_Station");
            var WorkIDs = Application.common.getArgsFromHref("WorkIDs");
            var DoFunc = Application.common.getArgsFromHref("DoFunc");
            var CFlowNo = Application.common.getArgsFromHref("CFlowNo");

            //参数
            var params = {
                method: "getTreeDateMet",
                FK_Node: FK_Node,
                WorkID: WorkID,
                FK_Flow: FK_Flow,
                FID: FID,
                ToNode: ToNode,
                IsWinOpen: IsWinOpen,
                FK_Dept: FK_Dept,
                FK_Station: FK_Station,
                WorkIDs: WorkIDs,
                DoFunc: DoFunc,
                CFlowNo: CFlowNo,
                FK_Flow: FK_Flow
            };

            queryData(params, LoadDataGridCallBack, this);
        }
        //初始化
        $(function () {
            if ($.messager) {
                $.messager.defaults.ok = '确定';
                $.messager.defaults.cancel = '取消';
            }

            LoadTreeData();
        });

        function getChecked() {
            var childrenNodes = $('#checkedTt').tree('getChildren');
            if (childrenNodes.length == 1) {
                $.messager.alert("提示", "您没有选择人员!", "info");
                return;
            }

            var getSaveNo = '';
            for (var i = 0; i < childrenNodes.length; i++) {
                if (childrenNodes[i].id != 'CheId') {
                    if (getSaveNo != '') getSaveNo += ',';
                    getSaveNo += childrenNodes[i].id;
                }
            }
            var FK_Node = Application.common.getArgsFromHref("FK_Node");
            var WorkID = Application.common.getArgsFromHref("WorkID");
            var FK_Flow = Application.common.getArgsFromHref("FK_Flow");
            var FID = Application.common.getArgsFromHref("FID");
            var ToNode = Application.common.getArgsFromHref("ToNode");
            var IsWinOpen = Application.common.getArgsFromHref("IsWinOpen");
            var FK_Dept = Application.common.getArgsFromHref("FK_Dept");
            var FK_Station = Application.common.getArgsFromHref("FK_Station");
            var WorkIDs = Application.common.getArgsFromHref("WorkIDs");
            var DoFunc = Application.common.getArgsFromHref("DoFunc");
            var CFlowNo = Application.common.getArgsFromHref("CFlowNo");
            var Type = Application.common.getArgsFromHref("FK_Type");
            var userDo = Application.common.getArgsFromHref("userDo"); //由用户进行选择

            var getSaveName = ''; //选择前五个,多余的...表示
            var hasSelect = $('#checkedTt').tree('getChildren');
            for (var selectN = 1; selectN < hasSelect.length; selectN++) {
                if (selectN == 5) {
                    getSaveName += hasSelect[selectN].text + '...';
                    break;
                } else {
                    if (selectN == hasSelect.length - 1) {
                        getSaveName += hasSelect[selectN].text;
                    } else {
                        getSaveName += hasSelect[selectN].text + ',';
                    }
                }
            }
//            try {
                window.opener.document.getElementById("acc_link_" + ToNode).innerHTML = "选择接受人员" + "<span style='color:black;'>(" + getSaveName + ")</span>";
//            } catch (e) {
//                window.parent.document.getElementById("acc_link_" + ToNode).innerHTML = "选择接受人员" + "<span style='color:black;'>(" + getSaveName + ")</span>";
//            }

            var params = {
                method: "saveMet",
                getSaveNo: getSaveNo,
                FK_Node: FK_Node,
                WorkID: WorkID,
                FK_Flow: FK_Flow,
                FID: FID,
                ToNode: ToNode,
                IsWinOpen: IsWinOpen,
                FK_Dept: FK_Dept,
                FK_Station: FK_Station,
                WorkIDs: WorkIDs,
                DoFunc: DoFunc,
                CFlowNo: CFlowNo,
                FK_Flow: FK_Flow
            };
            queryData(params, function (js, scope) { }, this);
            window.close();
        }
        //关闭
        function cancelMet() {
            window.close();
        }


        function setVal(node) {
            var ee = $('#checkedTt').tree('find', 'CheId');
            var children = $('#checkedTt').tree('getChildren');
            for (var i = 0; i < children.length; i++) {
                if (children[i].id == node.id) {
                    return;
                }
            }
            $('#checkedTt').tree('append', {
                parent: ee.target,
                data: [{
                    id: node.id,
                    iconCls: "icon-user",
                    text: node.innerHTML
                }]
            });
        }
        function clearData() {
            $.messager.confirm('警告', '确定重置已选人员吗?', function (y) {
                if (y) {
                    var ee = $('#checkedTt').tree('find', 'CheId');
                    var son = $('#checkedTt').tree('getChildren', ee.target);
                    for (var i = 0; i < son.length; i++) {
                        $('#checkedTt').tree('remove', son[i].target);
                    }
                }
            });
        }
        //公共方法
        function queryData(param, callback, scope, method, showErrMsg) {
            if (!method) method = 'GET';
            $.ajax({
                type: method, //使用GET或POST方法访问后台
                dataType: "text", //返回json格式的数据
                contentType: "application/json; charset=utf-8",
                url: "Accepter.aspx", //要访问的后台地址
                data: param, //要发送的数据
                async: false,
                cache: false,
                complete: function () { }, //AJAX请求完成时隐藏loading提示
                error: function (XMLHttpRequest, errorThrown) {
                    callback(XMLHttpRequest);
                },
                success: function (msg) {//msg为返回的数据，在这里做数据绑定
                    var data = msg;
                    callback(data, scope);
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="easyui-panel" data-options="fit:true,border:false" title="接受人选择器" style="padding: 2px;
        width: 545px; height: 100%; margin: 0px; overflow: hidden;">
        <div class="easyui-layout" data-options="fit:true">
            <div data-options="region:'west',split:false,border:1" style="width: 300px;">
                <div id="tb" style="height: 27px; background-color: #E1ECFF;">
                    <a id="isOk" style="margin-left: 30px;" href="#" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-save'"
                        onclick="getChecked()">确定</a>&nbsp; &nbsp;<a id="clear" href="#" class="easyui-linkbutton"
                            data-options="plain:true,iconCls:'icon-reset'" onclick="clearData()">重置</a>
                    &nbsp; &nbsp; <a id="delMeeting" href="#" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-cancel'"
                        onclick="cancelMet()">关闭</a>
                </div>
                <div style="height: 25px; background-color: #E1ECFF; padding-left: 10px;">
                    关键字查询：
                    <input id="cc" name="dept" style="width: 180px;" />
                </div>
                <div region="center" border="true" style="margin-top: 5px; padding: 0; overflow: auto;">
                    <ul class="easyui-tree" id="tt" style="margin-left: 10px;">
                    </ul>
                </div>
            </div>
            <div data-options="region:'center'" style="width: 50%; overflow-y: auto; overflow-x: hidden;">
                <div class="cart" style="float: left; width: 245px;">
                    <div class="ctitle" style="background: #E1ECFF; height: 32px; margin-left: -10px;
                        line-height: 32px; color: #2D3E5C; text-align: center;">
                        单击左侧添加人员
                    </div>
                    <div>
                        <ul class="easyui-tree" id="checkedTt" style="margin-left: 0px; margin-top: 5px;">
                        </ul>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <td class="BigDoc" style="display: none; height: 1px; background-color: red;">
        <uc1:Pub ID="Left" runat="server" Visible="false" />
        <uc1:Pub ID="Pub1" runat="server" />
    </td>
</asp:Content>
