

var _ShenHe = {
    //cshtml
    onSelectShenHeJieGuo: function (record) {
        //注意：此事件为combobox的onSelect事件函数，此函数不能重新加载propertygrid数据，否则会出错
        if (record.value == SHENHEJIEGUO.PASS) {
            var newData = $("#pg").propertygrid('getData');
            for (var i = newData.rows.length - 1; i >= 0; i--) {
                if (newData.rows[i].id == "ReturnTo" || newData.rows[i].id == "IsBackTracking") {
                    $("#pg").propertygrid('deleteRow', i);
                }
            }

            _ShenHe.sheZheTongGuoLiYou();

        } else {
            var newData = $("#pg").propertygrid('getData');
            for (var i = 0; i < newData.rows.length; i++) {
                if (newData.rows[i].id == currentPrefix + "ShenHeJieGuo") {
                    for (var j = 0; j < returnPropertygridData.length; j++) {
                        $("#pg").propertygrid('insertRow', { index: i + j + 1, row: returnPropertygridData[j] });
                    }
                    break;
                }
            }
        }
    },
    //cshtml
    onSelectReturnToNodeCombobox: function (rec) {
        var recName = rec.text.substring(0, rec.text.indexOf("="));
        var name = rec.text.substring(rec.text.indexOf(">") + 1);

        var newData = $("#pg").propertygrid('getData');
        for (var i = 0; i < newData.rows.length; i++) {
            if (newData.rows[i].id == currentPrefix + "ShenHeYiJian") {
                newData.rows[i].value = _ShenHe.sheZheTuiHuiLiYou(recName, name);
                $("#pg").propertygrid('refreshRow', i);
                break;
            }
        }
    },

    sheZheTuiHuiLiYou: function (recName, name) {
        return recName + "同志: \n  您发起的“" + name + "”工作有错误，需要您重新办理．\n\n此致!!!   \n\n  "
            + new Date().Format("yyyy-MM-dd hh:mm:ss");
    },
    //BS_
    sheZheTongGuoLiYou: function () {
        var newData = $("#pg").propertygrid('getData');
        for (var i = 0; i < newData.rows.length; i++) {
            if (newData.rows[i].id == currentPrefix + "ShenHeYiJian") {
                newData.rows[i].value =
                    NodeInfo.rows[0].Sender +
                    "同志: \n  您发起的“" + PreviousNodeInfo.NodeName + "”工作已通过(" +
                    currentLoginUser + ":" + currentLoginUserName + ")的审批．\n\n此致!!!   \n\n  "
            + new Date().Format("yyyy-MM-dd hh:mm:ss");;
                $("#pg").propertygrid('refreshRow', i);
                break;
            }
        }
    },


}