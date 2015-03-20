<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HelperOfTBEUI.aspx.cs"
    Inherits="CCFlow.WF.Comm.HelperOfTBEUI" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>词汇选择</title>
    <link href="../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../Scripts/CommonUnite.js" type="text/javascript"></script>
    <script type="text/javascript">
        //秦 2015-1-12  若不喜欢这种可以用以前的版本   更新覆盖就行
        var selectId;
        var WordsSort;
        var getText = '';
        var AttrKey = '';
        var NodeName = '';
        var id;
        function LoadDataCallBack(js, scorp) {
            if (js == "") js = "[]";
            if (js.status && js.status == 500) {
                $("body").html("<b>访问页面出错，请联系管理员。<b>");
                return;
            }

            var pushData = eval('(' + js + ')');
            $('#WordsTree').tree({
                idField: 'id',
                iconCls: 'tree-folder',
                data: pushData,
                checkbox: true,
                collapsed: true,
                animate: true,
                width: 300,
                height: 400,
                lines: true,
                onContextMenu: function (e, node) {
                    $('#Div_One').removeClass('One');
                    $('#Div_Two').removeClass('One');
                    $('#Div_Three').removeClass('One');
                    var ParentNode = $('#WordsTree').tree('getParent', node.target);
                    var v;
                    try {
                        v = ParentNode.text;
                    } catch (e) {
                    }
                    if (node.text == '常用词汇' || v == '常用词汇') {
                        return;
                    }
                    if (node.attributes["IsParent"] == '1') {
                        $('#Div_Two').addClass('One');
                        $('#Div_Three').addClass('One');
                    }
                    else {
                        $('#Div_One').addClass('One');
                    }
                    e.preventDefault();
                    $('#WordsTree').tree("select", node.id);
                    $('#whatToDo').menu('show', {
                        left: e.pageX,
                        top: e.pageY
                    });
                    selectId = node.id;
                },
                onClick: function (node) {
                    if (node) {
                        $('#WordsTree').tree("check", node.target);
                    }
                }
            });
        }


        function LoadData() {
            var params = {
                method: "GetTreeData",
                WordsSort: WordsSort,
                AttrKey: AttrKey,
                NodeName: NodeName
            };
            queryData(params, LoadDataCallBack, this);
        }

        //初始化
        $(function () {
            $(document).keyup(function (event) {
                if (event.keyCode == 27) {//Esc关闭
                    window.close();
                }
                if (event.keyCode == 13) {//Enter
                    saveChecked();
                }
            });

            if ($.messager) {
                $.messager.defaults.ok = '确定';
                $.messager.defaults.cancel = '取消';
            }
            WordsSort = Application.common.getArgsFromHref("WordsSort");
            AttrKey = Application.common.getArgsFromHref("AttrKey");
            NodeName = Application.common.getArgsFromHref("FK_Flow");
            id = Application.common.getArgsFromHref("id");
            LoadData();
        });

        //添加数据
        function addNodeData() {
            $.messager.prompt('提示', '请输入词汇', function (r) {
                if (r) {
                    var params = {
                        method: "addNodeData",
                        selectId: selectId,
                        WordsSort: WordsSort,
                        AttrKey: AttrKey,
                        NodeName: NodeName,
                        setText: encodeURI(r)
                    };
                    queryData(params, LoadDataCallBack, this);
                    selectId = "";
                }
            });
        }
        //编辑节点
        function editNode() {
            $.messager.prompt('提示', '请输入修改内容', function (r) {
                if (r) {
                    var params = {
                        method: "editNodeMet",
                        selectId: selectId,
                        WordsSort: WordsSort,
                        AttrKey: AttrKey,
                        NodeName: NodeName,
                        setText: encodeURI(r)
                    };
                    queryData(params, LoadDataCallBack, this);
                    selectId = "";
                }
            });
        }
        //删除节点
        function delNode() {
            $.messager.confirm('提示', '确定删除吗?', function (r) {
                if (r) {
                    var params = {
                        method: "delNodeMet",
                        selectId: selectId,
                        WordsSort: WordsSort,
                        AttrKey: AttrKey,
                        NodeName: NodeName
                    };
                    queryData(params, LoadDataCallBack, this);
                    selectId = "";
                }
            });
        }

        //关闭
        function closeMet() {
            window.close();
        }
        function allText(js, o) {
            getText += js;
        }
        function saveChecked() {
            var nodes = $('#WordsTree').tree('getChecked');
            for (var i = 0; i < nodes.length; i++) {
                if (nodes[i].attributes["IsParent"] == 0) {
                    if (nodes[i].text.indexOf("moreText") != -1) {//包含
                        var params = {
                            method: "moreTextMet",
                            WordsSort: WordsSort,
                            AttrKey: AttrKey,
                            NodeName: NodeName,
                            id: nodes[i].id
                        };
                        queryData(params, allText, this);
                    } else {
                        getText += nodes[i].text;
                    }
                }
            }
            if (getText == '') {
                $.messager.alert("提示", "您没有选中项!", "info");
                return;
            }
            //兼容
            var explorer = window.navigator.userAgent;

            if (explorer.indexOf("Chrome") >= 0) {//谷歌
                window.close();
                //ContentPlaceHolder1_MyFlowUC1_MyFlow1_UCEn1_TB_CBDBSX"
                //ContentPlaceHolder1_ForwardUC1_Top_TB_Doc
                //ContentPlaceHolder1_ReturnWork1_Pub1_TB_Doc
                //Pub1_TB_Doc
                //ContentPlaceHolder1_UCEn1_TB_ee
                if (window.opener.document.getElementById("ContentPlaceHolder1_MyFlowUC1_MyFlow1_UCEn1_" + id)) {
                    window.opener.document.getElementById("ContentPlaceHolder1_MyFlowUC1_MyFlow1_UCEn1_" + id).value = getText;
                }
                if (window.opener.document.getElementById("ContentPlaceHolder1_ForwardUC1_Top_" + id)) {
                    window.opener.document.getElementById("ContentPlaceHolder1_ForwardUC1_Top_" + id).value = getText;
                }
                if (window.opener.document.getElementById("ContentPlaceHolder1_ReturnWork1_Pub1_" + id)) {
                    window.opener.document.getElementById("ContentPlaceHolder1_ReturnWork1_Pub1_" + id).value = getText;
                }
                if (window.opener.document.getElementById("Pub1_" + id)) {
                    window.opener.document.getElementById("Pub1_" + id).value = getText;
                }
                if (window.opener.document.getElementById("ContentPlaceHolder1_UCEn1_" + id)) {
                    window.opener.document.getElementById("ContentPlaceHolder1_UCEn1_" + id).value = getText;
                }
            }
            else {//IE
                window.returnValue = getText;
                window.close(); //关闭子窗口
            }
            //兼容
        }
        function queryData(param, callback, scope, method, showErrMsg) {
            if (!method) method = 'GET';
            $.ajax({
                type: method, //使用GET或POST方法访问后台
                dataType: "text", //返回json格式的数据
                contentType: "application/json; charset=utf-8",
                url: "HelperOfTBEUI.aspx", //要访问的后台地址
                data: param, //要发送的数据
                async: false,
                cache: false,
                complete: function () { },
                error: function (XMLHttpRequest, errorThrown) {
                    callback(XMLHttpRequest);
                },
                success: function (msg) {
                    var data = msg;
                    callback(data, scope);
                }
            });
        }
        function moreText(v) {
            var params = {
                method: "moreTextMet",
                WordsSort: WordsSort,
                AttrKey: AttrKey,
                NodeName: NodeName,
                id: v
            };
            queryData(params, moreTextCallBack, this);
        }
        function moreTextCallBack(js, scorp) {
            $.messager.alert("更多数据", js, "info");
        }
    </script>
    <style type="text/css">
        .One
        {
            display: none;
        }
    </style>
</head>
<body class="easyui-layout">
    <div data-options="region:'center',fit:true" style="width: 400px; height: 500px;">
        <div id="tb" style="background-color: #E1ECFF; height: 27px;">
            <a id="btn_ok" style="float: left; background-color: #E1ECFF; margin-left: 25px;"
                href="#" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-save'"
                onclick="saveChecked()">确定(Enter)</a>&nbsp;<a id="btn_close" href="#" class="easyui-linkbutton"
                    data-options="plain:true,iconCls:'icon-cancel'" onclick="closeMet()">关闭(Esc)</a>
           <%-- <a id="OpInfo" href="#" class="easyui-linkbutton" style="margin-left: 1px; font-weight: bold;
                color: Red;" data-options="plain:true">操作常用词汇无效,相同值不会被添加</a>--%>
        </div>
        <ul class="easyui-tree" id="WordsTree" toolbar="#tb" style="margin-left: 3px; margin-top: 5px;">
        </ul>
    </div>
    <div id="whatToDo" class="easyui-menu" style="width: 100px;">
        <div id="Div_One" data-options="iconCls:'icon-add'" onclick="addNodeData()">
            添加数据</div>
        <div id="Div_Two" data-options="iconCls:'icon-edit'" onclick="editNode()">
            修改</div>
        <div id="Div_Three" data-options="iconCls:'icon-delete'" onclick="delNode()">
            删除</div>
    </div>
</body>
</html>
