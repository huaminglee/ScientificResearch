
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
                    window.open(url);
                }
            });
        },

        readRP: function (content) {
            $('#div_rp').html(content);
            $('#dlg').dialog('open');
        }
    }