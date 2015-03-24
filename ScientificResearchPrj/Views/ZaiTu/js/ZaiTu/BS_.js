﻿var cuiban_workid = 0;

    var BS_ = {
        open:function (urls, title) {
            window.parent.addPanel({ 'view': urls, 'title': title });
        },
        reload:function () { 
            $('#tTable').treegrid("loading");
            window.location.href = "/ZaiTu/ZaiTu";
            $('#tTable').treegrid("loaded");
        },
        //结束流程
        jieShuLiuCheng:function (fk_flow,workID) {
            $.messager.confirm("结束流程", "您确定结束当前流程吗？", function (data) {
                if (data) {
                    window.location.href = "/ZaiTu/JieShuLiuCheng?FK_Flow=" + fk_flow + "&WorkID=" + workID;
                }
            });
        },

        openTargetUrl: function (openRight, url) {
            var message = "";
            if (openRight == "1") message = "您不是当前流程处理人，但是流程历史处理者，可以查看当前节点内容但不能编辑";
            if (openRight == "2") message = "您不是当前流程处理人，但是流程发起者，可以查看当前节点内容但不能编辑";
            if (openRight == "3") message = "您当前流程处理人，可以查看和编辑当前节点内容";
            $.messager.confirm("提示", message, function (data) {
                if (data) {
                    window.open(url);
                }
            });
        },

        cuiBan: function (todoEmps, workid, nodeName) {
            $('#cuibandlg').dialog('open');
            $('#pressContent').html('');
            $('#divPressContent').css("display","block");

            var msg = "您好:\t" + todoEmps + " \t\n \t\n  此【" + nodeName + "】工作需要您请尽快处理.... \t\n \t\n致! \t\n \t\n   " + currentLoginUserName + "\t" + new Date().Format("yyyy-MM-dd hh:mm:ss");
            $('#pressContent').html(msg);

            cuiban_workid = workid;
        },

        saveCuiBan: function () {
            var newRow = { "workID": cuiban_workid, "msg": $('#pressContent').html() };
            $.ajax({
                url: "/ZaiTu/CuiBan",
                async: false,
                type: "POST",
                dataType: "json",
                data: newRow,
                success: BS_.cuiBanSuccess
            });
        },

        cuiBanSuccess: function (data, status) {
            $.messager.show({
                title: '执行催办',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            if (data.state == "0") {
                $('#cuibandlg').dialog('close');
            }
        },

        readRP: function (content) {
            $('#div_rp').html(content);
            $('#rpdlg').dialog('open');
        }

    }