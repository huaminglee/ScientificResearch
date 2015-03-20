var _attach_No_Or_OID = undefined;
var _attach_isShenHe = undefined;
var _attach_rowIndex = undefined;
var _attach_fileIdStr = undefined;
var _attach_fileIDStrPrefix = "attachID";
var _attach_canFileDelete = undefined;
var _attach_currentFileListIndex = undefined;
var _attach_fileInfoStr = undefined;

    var $ = jQuery,
        $_attach_list = $('#thelist'),
        $_attach_btn = $('#ctlBtn').addClass('webuploader-upload'),
        _attach_state = 'pending',
        _attach_uploader;

    var _attach_fileJsonData = {
        fileList: []
    };
     
    // 文件上传
    jQuery(function () {
        _attach_uploader = WebUploader.create({

            // 不压缩image
            resize: false,

            // swf文件路径
            swf: 'Scripts/webuploader-0.1.5/Uploader.swf',

            // 文件接收服务端。
            server: '/File/Upload',

            //文件上传请求的参数表，每次发送都会发送此对象中的参数
            formData: { "WorkID": WorkID, "FK_Node": FK_Node },

            // 选择文件的按钮。可选。
            // 内部根据当前运行是创建，可能是input元素，也可能是flash.
            pick: '#picker',

            fileSingleSizeLimit: 100 * 1024 * 1024    // 100 M
        });

        // 当有文件添加进来的时候
        _attach_uploader.on('fileQueued', function (file) {  
            $_attach_list.append('<div id="' + file.id + '" class="file-item">' +
                '<span class="info">' + file.name + "(" + Math.round(file.size / 1024 * 100) / 100 + "K)" + '</span>' +
                '<span class="state">等待上传...</span>' + '</div>'+
                '<div  id="' + file.id + "_" + '" class="file-item" style="display:none">' +
                '<input class="easyui-textbox" style="width:100%;border:0" value="说明：">' +
                '</div>'
            );
        });

        //data {Object}默认的上传参数，可以扩展此对象来控制上传参数
        _attach_uploader.on('uploadBeforeSend', function (object, data, headers) {
            
        });

        // 文件上传过程中创建进度条实时显示。
        _attach_uploader.on('uploadProgress', function (file, percentage) {
            $('#' + file.id).find('span.state').text('上传中...' + Math.round(percentage * 1000) / 10 + "%");
        });

        _attach_uploader.on('uploadSuccess', function (file, response) {
            if (response.state == "-1") {
                _attach_uploader.removeFile(file);
                $('#' + file.id).find('span.state').text(response.message);
                return;
            }
            
            $('#' + file.id).find('span.state').text('已上传');
            $('#' + file.id + "_").css({ "display": "block" });
            var fileEvent = {
                queueId: file.id,
                name: file.name,
                size: file.size,
                filePath: response.filePath,
                desc: ""
            };
            _attach_fileJsonData.fileList.push(fileEvent)
        });

        _attach_uploader.on('uploadError', function (file) {
            _attach_uploader.removeFile(file);
            $('#' + file.id).find('span.state').text('上传出错');
        });

        _attach_uploader.on('uploadComplete', function (file) {
            
        });
          
        _attach_uploader.on('all', function (type) {
            if (type === 'startUpload') {
                _attach_state = 'uploading';
            } else if (type === 'stopUpload') {
                _attach_state = 'paused';
            } else if (type === 'uploadFinished') {
                _attach_state = 'done';
            }

            if (_attach_state === 'uploading') {
                $_attach_btn.text('暂停上传');
            } else {
                $_attach_btn.text('开始上传');
            }
        });

        $_attach_btn.on('click', function () {
            if (_attach_state === 'uploading') {
                _attach_uploader.stop();
            } else {
                _attach_uploader.upload();
            }
        });
    });

    function openAttachDialog(No_Or_OID, isShenHe) {
        _attach_No_Or_OID = No_Or_OID;
        _attach_isShenHe = isShenHe;

        _attach_fileInfoStr = "";
        $_attach_list.html(""),
        _attach_state = 'pending',
        _attach_fileJsonData.fileList = [];
        //清空文件队列
        var arrays = _attach_uploader.getFiles();
        for (var i = 0; i < arrays.length; i++) {
            _attach_uploader.removeFile(arrays[i]);
        }

        $('#divSelectFile').dialog('open');
    }

    function loadAttachs(No_Or_OID, isShenHe, rowIndex, canDelete) {
        _attach_rowIndex = rowIndex;
        _attach_canFileDelete = canDelete;
        var loadArgs = { "No_OID": No_Or_OID, "isShenHe": isShenHe };

        $.ajax({
            url: "/File/GetFileHistoryData",
            async: false,
            type: "POST",
            dataType: "json",
            data: loadArgs,
            success: loadFileSuccess
        });
    }

    function loadFileSuccess(returnObj) {
        /*
        $.messager.show({
            title: '消息提示',
            msg: returnObj.message,
            timeout: 3000,
            showType: 'show'
        });  */
        if (returnObj.state == "0") {
            var rows = parseJSON(returnObj._Json).rows;
            if (rows == undefined) return;
            var selectedRow = $("#pg").propertygrid('getRows')[_attach_rowIndex];
            selectedRow.value = "";
            for (var i = 0; i < rows.length; i++) {
                var fileIDStr = _attach_fileIDStrPrefix + rows[i].OID;
                var appendDiv = $('<div id="' + fileIDStr + '"/>');
                var aLink = $('<a href="' + rows[i].Path + '" target="_blank"/>');
                aLink.append(rows[i].AttachName);
                var span1 = $('<span style="margin-right:20px"/>').append("【附件】" + aLink[0].outerHTML);

                var span2 = $('<span style="margin-right:20px"/>').append("【说明】" + rows[i].AttachDesc);

                appendDiv.append(span1);
                appendDiv.append(span2);

                if (_attach_canFileDelete == CanAttachDelete.YES) {
                    var button = $('<a href="javascript:void(0)" style="border:0;" class="textbox-icon icon-cut" onclick=delFile(' + _attach_rowIndex + ',"' + fileIDStr + '")></a>');
                    var span3 = $('<span/>').append("【删除】" + button[0].outerHTML);
                    appendDiv.append(span3);
                }

                //加载到显示区域
                selectedRow.value += appendDiv[0].outerHTML;
            }
            $("#pg").propertygrid('refreshRow', _attach_rowIndex);
            $("#pg").propertygrid('unselectRow', _attach_rowIndex);
        }
    }

    function saveFileInfo() {
        if (_attach_state == "done") {
            saveSuccessFileInfo();
        } else {
            if ($_attach_list.children().length != 0) {
                $.messager.confirm("警告", "您确定放弃未成功上传的文件吗？", function (flag) {
                    if (flag) {
                        saveSuccessFileInfo();
                    }
                });
            } else {
                closeFileDialog();
            }
        }
    }

    function saveSuccessFileInfo() {
        var successCount = _attach_fileJsonData.fileList.length;
        if (successCount == 0) {
            closeFileDialog();
            return;
        }

        for (var i = 0; i < successCount; i++) {
            var newRow = {};
            newRow["No_OID"] = _attach_No_Or_OID;
            newRow["IsShenHe"] = _attach_isShenHe;
            newRow["AttachName"] = _attach_fileJsonData.fileList[i].name;
            newRow["AttachDesc"] = $($('#' + _attach_fileJsonData.fileList[i].queueId + "_")[0].children[0]).val();
            newRow["Path"] = _attach_fileJsonData.fileList[i].filePath;

            _attach_fileJsonData.fileList[i].desc = newRow["AttachDesc"];
            _attach_currentFileListIndex = i;

            $.ajax({
                url: "/File/TianJiaFile",
                async: false,
                type: "POST",
                dataType: "json",
                data: newRow,
                success: addFileInfoSuccess
            });
        }

        //点击选择文件框的时候已经选择了该行了
        var row = $("#pg").propertygrid('getSelected');
        var rowIndex = $("#pg").propertygrid('getRowIndex', row);
        //加载到显示区域
        row.value += _attach_fileInfoStr;

        $("#pg").propertygrid('refreshRow', rowIndex);
        $("#pg").propertygrid('unselectRow', rowIndex);
        
        closeFileDialog();
    }

    function addFileInfoSuccess(returnObj) {
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

            var lfileIDStr = _attach_fileIDStrPrefix + returnObj.OID;
            var appendDiv = $('<div id="' + lfileIDStr + '"/>'); 
            var aLink = $('<a href="' + _attach_fileJsonData.fileList[_attach_currentFileListIndex].filePath + '" target="_blank"/>');
            aLink.append(_attach_fileJsonData.fileList[_attach_currentFileListIndex].name);
            var span1 = $('<span style="margin-right:20px"/>').append("【附件】" + aLink[0].outerHTML);

            var span2 = $('<span style="margin-right:20px"/>').append("【说明】" + _attach_fileJsonData.fileList[_attach_currentFileListIndex].desc);
            var button = $('<a href="javascript:void(0)" style="border:0;" class="textbox-icon icon-cut" onclick=delFile(' + rowIndex + ',"' + lfileIDStr + '")></a>');
            var span3 = $('<span/>').append("【删除】" + button[0].outerHTML);
            appendDiv.append(span1);
            appendDiv.append(span2);
            appendDiv.append(span3);

            _attach_fileInfoStr += appendDiv[0].outerHTML;
        }
    }

    //删除附件
    function delFile(rowIndex, fileIDStr) {
        $.messager.confirm("删除", "您确定删除该文件吗？", function (data) {
            if (!data) return;
            else {
                _attach_rowIndex = rowIndex;
                _attach_fileIdStr = fileIDStr;
                var delArgs = { "OID": fileIDStr.substring(_attach_fileIDStrPrefix.length) };

                $.ajax({
                    url: "/File/ShanChuFile",
                    async: false,
                    type: "POST",
                    dataType: "json",
                    data: delArgs,
                    success: delFileSuccess
                });
            }
        });
    }

    function delFileSuccess(returnObj) {
        $.messager.show({
            title: '消息提示',
            msg: returnObj.message,
            timeout: 3000,
            showType: 'show'
        });
        if (returnObj.state == "0") {
            //删除匹配id的div，使用最短匹配
            var reg = eval('/\\<div[^\\>]+id=\\"' + _attach_fileIdStr + '\\"[^\\>]*\\>[\\s\\S]*\?\<\\/div\\>/');
            $("#pg").propertygrid('getRows')[_attach_rowIndex].value = $("#pg").propertygrid('getRows')[_attach_rowIndex].value.replace(reg, "");

            //传rowIndex是因为有可能还没选中行的时候就进行了删除操作
            //更新数据
            $("#pg").propertygrid('refreshRow', _attach_rowIndex);

        }
    }

    function closeFileDialog(){
        $('#divSelectFile').dialog('close');
    }