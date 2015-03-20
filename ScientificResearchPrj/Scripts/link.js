var _link_No_Or_OID = undefined;
var _link_isShenHe = undefined;
var _link_rowIndex = undefined;
var _link_linkIdStr = undefined;
var _link_linkIDStrPrefix = "linkID";
var _link_canLinkDelete = undefined;

function openLinkDialog(No_Or_OID, isShenHe) {
    _link_No_Or_OID = No_Or_OID;
    _link_isShenHe = isShenHe;
    
    $('#linkHref').textbox("setText", "http://");
    $('#linkInstruction').textbox("setText", "");

    $('#divAddLink').dialog('open');
}

function loadLinks(No_Or_OID, isShenHe, rowIndex, canDelete) {
    _link_rowIndex = rowIndex;
    _link_canLinkDelete = canDelete;
    var loadArgs = { "No_OID": No_Or_OID, "isShenHe": isShenHe };

    $.ajax({
        url: "/Link/GetLinkHistoryData",
        async: false,
        type: "POST",
        dataType: "json",
        data: loadArgs,
        success: loadLinkSuccess
    });
}

function loadLinkSuccess(returnObj) {
    /*
    $.messager.show({
        title: '消息提示',
        msg: returnObj.message,
        timeout: 3000,
        showType: 'show'
    });  */
    if (returnObj.state == "0") {
        var rows = parseJSON(returnObj._Json).rows;
        if(rows == undefined) return;
        
        var selectedRow = $("#pg").propertygrid('getRows')[_link_rowIndex];
        selectedRow.value = "";
        for(var i = 0; i < rows.length;i++){
            var linkIDStr = _link_linkIDStrPrefix + rows[i].OID;
            var appendDiv = $('<div id="' + linkIDStr + '"/>');
            var aLink = $('<a href="' + rows[i].LinkHref + '" target="_blank"/>');
            aLink.append(rows[i].LinkHref);
            var span1 = $('<span style="margin-right:20px"/>').append("【链接】" + aLink[0].outerHTML);

            var span2 = $('<span style="margin-right:20px"/>').append("【说明】" + rows[i].LinkDesc);

            appendDiv.append(span1);
            appendDiv.append(span2);

            if (_link_canLinkDelete == CanLinkDelete.YES) {
                var button = $('<a href="javascript:void(0)" style="border:0;" class="textbox-icon icon-cut" onclick=delLink(' + _link_rowIndex + ',"' + linkIDStr + '")></a>');
                var span3 = $('<span/>').append("【删除】" + button[0].outerHTML);
                appendDiv.append(span3);
            }
             
            //加载到显示区域
            selectedRow.value += appendDiv[0].outerHTML;
        }
        $("#pg").propertygrid('refreshRow', _link_rowIndex);
        $("#pg").propertygrid('unselectRow', _link_rowIndex);
    }
}

function saveLink() {
    var newRow = getNewRowForLink();
    $.ajax({
        url: "/Link/TianJiaLink",
        async: false,
        type: "POST",
        dataType: "json",
        data: newRow,
        success: addLinkSuccess
    });

}

function addLinkSuccess(returnObj) {
    $.messager.show({
        title: '消息提示',
        msg: returnObj.message,
        timeout: 3000,
        showType: 'show'
    });
    if (returnObj.state == "0") {

        //点击选择文件框的时候已经选择了该行了
        var row = $("#pg").propertygrid('getSelected');
        var rowIndex = $("#pg").propertygrid('getRowIndex', row);


        var linkIDStr = _link_linkIDStrPrefix + returnObj.OID;
        var appendDiv = $('<div id="' + linkIDStr + '"/>');
        var aLink = $('<a href="' + $('#linkHref').val() + '" target="_blank"/>');
        aLink.append($('#linkHref').val());
        var span1 = $('<span style="margin-right:20px"/>').append("【链接】" + aLink[0].outerHTML);

        var span2 = $('<span style="margin-right:20px"/>').append("【说明】" + $('#linkInstruction').val());
        var button = $('<a href="javascript:void(0)" style="border:0;" class="textbox-icon icon-cut" onclick=delLink(' + rowIndex + ',"' + linkIDStr + '")></a>');
        var span3 = $('<span/>').append("【删除】" + button[0].outerHTML);
        appendDiv.append(span1);
        appendDiv.append(span2);
        appendDiv.append(span3);

        //加载到显示区域
        row.value += appendDiv[0].outerHTML;

        $("#pg").propertygrid('refreshRow', rowIndex);
        $("#pg").propertygrid('unselectRow', rowIndex);

        $('#divAddLink').dialog('close');
    }
}

function getNewRowForLink() {
    var newRow = {};
    newRow["No_OID"] = _link_No_Or_OID;
    newRow["IsShenHe"] = _link_isShenHe;
    newRow["LinkHref"] = $('#linkHref').val();
    newRow["LinkDesc"] = $('#linkInstruction').val();
    return newRow;
}

//删除链接
function delLink(rowIndex, linkIDStr) {
    $.messager.confirm("删除", "您确定删除该链接吗？", function (data) {
        if (!data) return;
        else {
            _link_rowIndex = rowIndex;
            _link_linkIdStr = linkIDStr;
            var delArgs = { "OID": linkIDStr.substring(_link_linkIDStrPrefix.length) };

            $.ajax({
                url: "/Link/ShanChuLink",
                async: false,
                type: "POST",
                dataType: "json",
                data: delArgs,
                success: delLinkSuccess
            });
        }
    });
}

function delLinkSuccess(returnObj) {
    $.messager.show({
        title: '消息提示',
        msg: returnObj.message,
        timeout: 3000,
        showType: 'show'
    });
    if (returnObj.state == "0") {
        //删除匹配id的div，使用最短匹配
        var reg = eval('/\\<div[^\\>]+id=\\"' + _link_linkIdStr + '\\"[^\\>]*\\>[\\s\\S]*\?\<\\/div\\>/');
        $("#pg").propertygrid('getRows')[_link_rowIndex].value = $("#pg").propertygrid('getRows')[_link_rowIndex].value.replace(reg, "");

        //传rowIndex是因为有可能还没选中行的时候就进行了删除操作
        //更新数据
        $("#pg").propertygrid('refreshRow', _link_rowIndex);

    }
}
 