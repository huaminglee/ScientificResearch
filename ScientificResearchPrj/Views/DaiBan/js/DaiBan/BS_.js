
    var BS_ = {
        open:function (urls, title) {
            window.parent.addPanel({ 'view': urls, 'title': title });
        },
        reload:function () { 
            $('#tTable').treegrid("loading");
            window.location.href = "/DaiBan/DaiBan";
            $('#tTable').treegrid("loaded");
        },
        //结束流程
        jieShuLiuCheng:function (fk_flow,workID) {
            $.messager.confirm("结束流程", "您确定结束当前流程吗？", function (data) {
                if (data) {
                    window.location.href = "/DaiBan/JieShuLiuCheng?FK_Flow=" + fk_flow + "&WorkID=" + workID;
                }
            });
        },
        
        readPress: function (i) {
            $('#div_press').html(jsonData[i]["Press"]);
            $('#pressdlg').dialog('open');
        }

    }