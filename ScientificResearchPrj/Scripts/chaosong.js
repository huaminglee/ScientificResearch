//打开抄送窗口
function openChaoSongDialog(_json) {
    var url = "/ChaoSong/ChaoSongZhunBei?FK_Flow=" + _json.FK_Flow + "&WorkID=" + _json.WorkID
        + "&FID=" + _json.FID + "&FK_Node=" + _json.FK_Node
        + "&random=" + new Date().getTime();
    $('#divChaoSong').dialog('open');
    $.getJSON(url, function (returnObj) {
        $("#divChaoSongJieGuo").html(returnObj.message);
        if (returnObj.state == "-1") {
            return;
        } else {
            var jsonResult = parseJSON(returnObj._Json);
            var index = 0;
            PreviousNodesInfoData = [];

            jsonResult.rows.forEach(function (e) {
                var temp = {
                    text: e.EmpName + " => " + e.FK_NodeText,
                    value: e.FK_Emp + "(" + e.EmpName + ")" + (index++)//后台处理时将该数字清除
                };
                PreviousNodesInfoData.push(temp);
            });
            $('#ChaoSongRenTo').combobox("loadData", PreviousNodesInfoData);
            //注意：PreviousNodeInfo在具体的文件中进行定义
            $('#ChaoSongNeiRong').text("同志: \n  您发起的“" + PreviousNodeInfo.NodeName + "”工作已经过(" +
                        currentLoginUser + ":" + currentLoginUserName + ")处理，现抄送与您．请注意查阅.\n\n此致!!!   \n\n  "
                + new Date().Format("yyyy-MM-dd hh:mm:ss"));
        }

    });
}

//抄送
function chaoSong() {
    var tempEmps = $('#ChaoSongRenTo').combobox("getText");
    if ($('#ChaoSongRenTo').combobox("getText") != "" && $("#ChaoSongRenTo2").textbox("getText") != "") {
        tempEmps += ",";
    }
    tempEmps += $("#ChaoSongRenTo2").textbox("getText");

    $.messager.confirm("抄送", "请确定您是否抄送给：<br><br>【"
        + tempEmps + "】",
        function (data) {
            if (data == true) {
                var newRow = getNewRowForChaoSong();
                $.ajax({
                    url: "/ChaoSong/ChaoSongTo",
                    async: false,
                    type: "POST",
                    dataType: "json",
                    data: newRow,
                    success: chaoSongCallback
                });
            }
        }
    );
}

function getNewRowForChaoSong() {
    var newRow = {};
    newRow["ChaoSongRenTo"] = $('#ChaoSongRenTo').combobox("getValues").toString();//必须转换为toString,否则后台读取不到数据

    if ($('#ChaoSongRenTo').combobox("getValues").toString() != "" && $("#ChaoSongRenTo2").textbox("getText") != "") {
        newRow["ChaoSongRenTo"] += ",";
    }
    newRow["ChaoSongRenTo"] += $("#ChaoSongRenTo2").textbox("getText");

    newRow["ChaoSongBiaoTi"] = $('#ChaoSongBiaoTi').textbox("getText");
    newRow["ChaoSongNeiRong"] = $('#ChaoSongNeiRong').text();

    newRow["FK_Flow"] = FK_Flow;
    newRow["FK_Node"] = FK_Node;
    newRow["WorkID"] = WorkID;
    newRow["FID"] = FID;
    return newRow;
}

//取消抄送
function quXiaoChaoSong() {
    $('#divChaoSong').dialog('close');
}

//抄送回调函数
function chaoSongCallback(returnObj) {
    $.messager.show({
        title: '消息提示',
        msg: returnObj.message,
        timeout: 3000,
        showType: 'show'
    });
    if (returnObj.state == "0") {
        $("#divChaoSongJieGuo").html(returnObj.message);
        closeChaoSongDialog();
    }
}

var _chaosong_closeChaoSongDialogTime = 3;
var _chaosong_chaoSongTimer = undefined;
function closeChaoSongDialog() {   //自定义函数
    _chaosong_chaoSongTimer = window.setTimeout('closeChaoSongDialog()', 1000);  //设置的时间函数
    if (_chaosong_closeChaoSongDialogTime > 0) {
        $("#divChaoSongClose").html("系统将在 <font color=red>" + _chaosong_closeChaoSongDialogTime + "</font> 后关闭该窗口");
        _chaosong_closeChaoSongDialogTime--;
    }
    else {
        $('#divChaoSong').dialog('close');
        clearTimeout(_chaosong_chaoSongTimer);
        _chaosong_closeChaoSongDialogTime = 3;
        $("#divChaoSongClose").html("");
    }
}
 

function select_Emps() {
    var selectChaoSongEmps = _EmpsSelect.select_Emps();
    $("#ChaoSongRenTo2").textbox("setText", selectChaoSongEmps);
}

