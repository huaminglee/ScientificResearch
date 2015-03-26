
    var BS_ = {
        open:function (urls, title) {
            window.parent.addPanel({ 'view': urls, 'title': title });
        },
        reload:function () { 
            $('#tTable').treegrid("loading");
            window.location.href = "/ProcessAll/AllProcess";
            $('#tTable').treegrid("loaded");
        },
        //结束流程
        jieShuLiuCheng:function (fk_flow,workID) {
            $.messager.confirm("结束流程", "您确定结束当前流程吗？", function (data) {
                if (data) {
                    window.location.href = "/ProcessAll/JieShuLiuCheng?FK_Flow=" + fk_flow + "&WorkID=" + workID;
                }
            });
        },

        openTargetUrl: function (openRight, url) {
            var message = "";
            if (openRight == "0") message = "您不是当前流程处理人，也不是发起者，只能查看流程图";
            if (openRight == "1") message = "您不是当前流程处理人，但是流程历史处理者，可以查看当前节点内容但不能编辑";
            if (openRight == "2") message = "您不是当前流程处理人，但是流程发起者，可以查看当前节点内容但不能编辑";
            if (openRight == "3") message = "您当前流程处理人，可以查看和编辑当前节点内容";
            $.messager.confirm("提示", message, function (data) {
                if (data) {
                    window.open(url + "&random=" + new Date().getTime());
                }
            });
        },

        huiGun: function (fk_flow, workID,fid) {
            var url = "/ProcessAll/GetAllNodes?FK_Flow=" + fk_flow + "&random=" + new Date().getTime();
            $('#divHuiGun').dialog('open');
            $.getJSON(url, function (returnObj) {
                $.messager.show({
                    title: '消息提示',
                    msg: returnObj.message,
                    timeout: 3000,
                    showType: 'show'
                });
                NodesInfoForBack = [];

                if (returnObj.state == "-1") {
                    $('#HuiGunTo').combobox("loadData", NodesInfoForBack);
                    return;
                } else {
                    var jsonResult = parseJSON(returnObj._Json);
                    
                    jsonResult.rows.forEach(function (e) {
                        var temp = {
                            text: e.FK_Node + " => " + e.FK_NodeText,
                            value: e.FK_Node 
                        };
                        NodesInfoForBack.push(temp);
                    });
                    $('#HuiGunTo').combobox("loadData", NodesInfoForBack);
                }
            });
        },

        queDingHuiGUn: function () {
            var node = $('#tTable').treegrid('getSelected');
            if ($('#HuiGunTo').combobox("getValue") == "") {
                $.messager.show({
                    title: '消息提示',
                    msg: "请先选择要回滚到的节点",
                    timeout: 3000,
                    showType: 'show'
                });
                return;
            }
            if (document.getElementById("BackNote").innerText == "") {
                $.messager.show({
                    title: '消息提示',
                    msg: "请填写回滚原因",
                    timeout: 3000,
                    showType: 'show'
                });
                return;
            }
             
            var val = {
                "FK_Flow": node["FK_Flow"],
                "WorkID": node["WorkID"],
                "FK_Node": $('#HuiGunTo').combobox("getValue"),
                "note": document.getElementById("BackNote").innerText
            }

            $('#tTable').datagrid("loading");
            AT_.AjaxPost("/ProcessAll/HuiGun", val, BS_.huiGunSuccess);
        },

        huiGunSuccess: function (data, status) {
            $('#tTable').datagrid("loaded");
            $.messager.show({
                title: '回滚',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            if (data.state == "0") {
                $('#divHuiGun').dialog('close');
            }
        },

        quXiaoHuiGun: function () {
            $('#divHuiGun').dialog('close');
        }
    }