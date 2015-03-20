﻿var currentPrefix = "DBFX_";

var lastSaveResult = undefined;
var maxEditIndex = undefined;
var addOrUpdateIndex = undefined;

var BS_ = {
    onLoad: function () {
        var argsVal = {
            "FK_Flow": FK_Flow,
            "WorkID": WorkID,
            "FK_Node": FK_Node 
        }
           
        $("#pg").propertygrid("loading");
        AT_.AjaxPost("/DuiBiFenXi/GetHistoryData", argsVal, BS_.onLoadSuccess);
             
    },

    onLoadSuccess:function(data, status){
        $.messager.show({
            title: '加载数据',
            msg: data.message,
            timeout: 3000,
            showType: 'show'
        });
        $("#pg").propertygrid("loaded");

        if (data.state == "0") {
            DBFXOriginResult = parseJSON(data._Json);

            if (DBFXOriginResult.rows.length == 1) {
                _CommomOperation.getPropertygridDataFormJsonResult(DBFXOriginResult, currentPrefix);

                //只有一个结果时，不给删除
                var pData = $("#pg").propertygrid('getData');
                for (var i = pData.rows.length - 1; i >= 0; i--) {
                    if (pData.rows[i].id != undefined && pData.rows[i].id == currentPrefix + "del") {
                        $("#pg").propertygrid("deleteRow", i);
                    }
                }
            }
            else {
                _CommomOperation.getAndCloneCurrentPropertygridDataFormJsonResult(DBFXOriginResult, currentPrefix);
            }

            maxEditIndex = DBFXOriginResult.rows.length - 1;
            $("#pg").propertygrid("collapseGroup");
            $("#pg").propertygrid("expandGroup", 0);

            for (var i = 0; i < DBFXOriginResult.rows.length; i++) {
                //加载链接
                var rowIndex = _CommomOperation.getRowIndexByIdAndIndex(currentPrefix + "Link", i);
                loadLinks(DBFXOriginResult.rows[i].No, IsShenHe.NO, rowIndex, CanLinkDelete.YES);
                //加载附件
                var rowIndex2 = _CommomOperation.getRowIndexByIdAndIndex(currentPrefix + "Attach", i);
                loadAttachs(DBFXOriginResult.rows[i].No, IsShenHe.NO, rowIndex2, CanAttachDelete.YES);
            }

        } else {
            //只有一个结果时，不给删除
            var pData = $("#pg").propertygrid('getData');
            for (var i = pData.rows.length - 1; i >= 0; i--) {
                if (pData.rows[i].id != undefined && pData.rows[i].id == currentPrefix + "del") {
                    $("#pg").propertygrid("deleteRow", i);
                }
            }

            maxEditIndex = 0;
        }
    },
       
    appendDBFX: function () {

        var returnFlag = BS_.baoCun();
        if (returnFlag == false) return;

        var row = $("#pg").propertygrid("getSelected");
        var rowIndex = $("#pg").propertygrid('getRowIndex', row);
        var targetIndex = row.index + 1;

        maxEditIndex = targetIndex;

        //删掉上一个结果的添加项
        $("#pg").propertygrid('deleteRow', rowIndex);

        $("#pg").propertygrid("insertRow", {
            index: rowIndex++,
            row: {
                name: "分隔位置",
                value: ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>",
                group: _CommomOperation.getGroupNameByPrefix(currentPrefix),
                index: targetIndex
            }
        });

        var tempData = _CommomOperation.getClonePropertygridByPrefix(currentPrefix, 0, 0);
        for (var j = 0; j < tempData.length; j++) {
            //更新index
            tempData[j].index = targetIndex;
            $("#pg").propertygrid("insertRow", {
                index: rowIndex++,
                row: tempData[j]
            });
        }
        $("#pg").propertygrid("acceptChanges");
    },

    deleteDBFX: function () {
        $.messager.confirm("删除量化对比分析结果", "您确定删除该量化对比分析结果吗？", function (flag) {
            if (flag == true) {
                var row = $("#pg").propertygrid("getSelected");
                var rowIndex = $("#pg").propertygrid('getRowIndex', row);
                var pData = $("#pg").propertygrid('getData');

                //非新增的数据
                if (row.index + 1 <= DBFXOriginResult.rows.length) {
                    var argsVal = {
                        "dbfxNo": DBFXOriginResult.rows[row.index].No
                    }
                    $("#pg").propertygrid("loading");
                    AT_.AjaxPost("/DuiBiFenXi/ShanChuDuiBiFenXi", argsVal, BS_.deleteSuccess);
                }
                else {
                    BS_.deleteRowFromPropertygrid();
                }
                    
            }
        });
    },

    deleteSuccess: function (data, status) {
        $.messager.show({
            title: '删除量化对比分析结果',
            msg: data.message,
            timeout: 3000,
            showType: 'show'
        });

        if (data.state == "0") {
            var row = $("#pg").propertygrid("getSelected");
            DBFXOriginResult.rows.splice(row.index, 1);

            BS_.deleteRowFromPropertygrid();
        }  
    },

    deleteRowFromPropertygrid: function () {
        var row = $("#pg").propertygrid("getSelected");
        var rowIndex = $("#pg").propertygrid('getRowIndex', row);
        var pData = $("#pg").propertygrid('getData');

        for (var i = pData.rows.length - 1; i >= 0; i--) {
            if (pData.rows[i].group == _CommomOperation.getGroupNameByPrefix(currentPrefix)) {
                if (pData.rows[i].index == row.index) {
                    $("#pg").propertygrid('deleteRow', i);
                }
                else if (pData.rows[i].index > row.index) {
                    pData.rows[i].index--;
                }
            }
        }

        //若是最后一个结果时，为上一个增加添加项
        if (row.index == maxEditIndex) {
            var insertIndex = _CommomOperation.getLastInsertIndexByGroupName(_CommomOperation.getGroupNameByPrefix(currentPrefix));
            for (var i = 0; i < DBFXPropertygridData.length; i++) {
                if (DBFXPropertygridData[i].id == currentPrefix + "add") {
                    var temp = cloneJSON(DBFXPropertygridData[i]);
                    temp.index = row.index - 1;
                    $("#pg").propertygrid("insertRow", {
                        index: insertIndex,
                        row: temp
                    });
                }
            }
        }
        maxEditIndex--;
    },














    baoCun: function () {
        _CommomOperation.propertygridEndEdit();
            
        if (DBFXOriginResult == undefined) {
            //新增
            BS_.add(maxEditIndex);
        }
        else if (DBFXOriginResult.rows.length == maxEditIndex + 1) {
            //更新
            BS_.update(maxEditIndex);
        }
        else {
            //前面的更新
            BS_.update(maxEditIndex - 1);
            //最后的添加
            BS_.add(maxEditIndex);
        }

        var returnFlag = (lastSaveResult == true) ? true : false;
        //更新lastSaveResult，防止首次保存失败后第二次不能保存（判断为false直接退出）
        lastSaveResult = undefined;

        return returnFlag;
    },
        
    add: function (index) {

        if (lastSaveResult == false) return;

        var newRow = BS_.getNewRowFromPropertygrid(index);
        addOrUpdateIndex = index;

        AT_.AjaxPost("/DuiBiFenXi/TianJiaDuiBiFenXi", newRow, BS_.addSuccess);
    },

    addSuccess: function (data, status) {
        $.messager.show({
            title: '添加量化对比分析结果',
            msg: data.message,
            timeout: 3000,
            showType: 'show'
        });

        if (data.state == "0") {
            BS_.updateDBFXOriginResultFromPropertygridData(currentPrefix);
            lastSaveResult = true;
        } else {
            lastSaveResult = false;
        }
    },
        
    update: function (maxIndex) {
        for (var index = 0; index <= maxIndex; index++) {
                
            if (lastSaveResult == false) return;

            if (BS_.isChange(index) == true) {
                addOrUpdateIndex = index;

                var newRow = BS_.getNewRowFromPropertygrid(index);
                newRow["oldNo"] = DBFXOriginResult.rows[index].No;
                AT_.AjaxPost("/DuiBiFenXi/XiuGaiDuiBiFenXi", newRow, BS_.updateSuccess);
            } else {
                lastSaveResult = true;
            }
        }
    },

    isChange: function (index) {
        var editData = $("#pg").propertygrid('getData');

        //与初始化项目数据进行比较
        for (var i = 0; i < editData.rows.length; i++) {
            if (editData.rows[i].index == index) {
                var id = editData.rows[i].id;
                if (id != undefined && BS_.ifColumnNeedCompare(id) == true) {
                    var prefix = currentPrefix;
                    var subid = id.substring(prefix.length);
                    if (DBFXOriginResult.rows[index][subid] != editData.rows[i].value) {
                        return true;
                    }
                }
            }
        }
        return false;
    },

    ifColumnNeedCompare: function (columnName) {
        if (columnName == currentPrefix + "No" ||
                columnName == currentPrefix + "Name" ||
                columnName == currentPrefix + "ProposeTime" ||
                columnName == currentPrefix + "FK_SYOID" ||
                columnName == currentPrefix + "Keys" ||
                columnName == currentPrefix + "Description" ||
                columnName == currentPrefix + "Methods" ||
                columnName == currentPrefix + "Data" ||
                columnName == currentPrefix + "AnalysisResult" ||
                columnName == currentPrefix + "InferType" ||
                columnName == currentPrefix + "InferContent" ||
                columnName == currentPrefix + "Remarks")
                return true;
            else
                return false;
        },

        updateSuccess: function (data, status) {
            $.messager.show({
                title: '更新量化对比分析结果',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });

            if (data.state == "0") {
                BS_.updateDBFXOriginResultFromPropertygridData(currentPrefix);
                lastSaveResult = true;
            } else {
                lastSaveResult = false;
            }
        },

        getNewRowFromPropertygrid: function (index) {
            var newRow = {};
            newRow["No"] = _CommomOperation.getValueFromPropertygridById(currentPrefix + "No", index);
            newRow["Name"] = _CommomOperation.getValueFromPropertygridById(currentPrefix + "Name", index);
            newRow["FK_Proposer"] = currentLoginUser;
            newRow["ProposeTime"] = _CommomOperation.getValueFromPropertygridById(currentPrefix + "ProposeTime", index);
            newRow["FK_SYOID"] = _CommomOperation.getValueFromPropertygridById(currentPrefix + "FK_SYOID", index);
            newRow["Keys"] = _CommomOperation.getValueFromPropertygridById(currentPrefix + "Keys", index);
            newRow["Description"] = _CommomOperation.getValueFromPropertygridById(currentPrefix + "Description", index);
            newRow["Methods"] = _CommomOperation.getValueFromPropertygridById(currentPrefix + "Methods", index);
            newRow["Data"] = _CommomOperation.getValueFromPropertygridById(currentPrefix + "Data", index);
            newRow["AnalysisResult"] = _CommomOperation.getValueFromPropertygridById(currentPrefix + "AnalysisResult", index);
            newRow["InferType"] = _CommomOperation.getValueFromPropertygridById(currentPrefix + "InferType", index);
            newRow["InferContent"] = _CommomOperation.getValueFromPropertygridById(currentPrefix + "InferContent", index);
            newRow["Remarks"] = _CommomOperation.getValueFromPropertygridById(currentPrefix + "Remarks", index);
            newRow["ModifyTime"] = (new Date()).Format("dd/MM/yyyy hh:mm:ss");
            
            newRow["FK_Flow"] = FK_Flow;
            newRow["FK_Node"] = FK_Node;
            newRow["WorkID"] = WorkID;
            return newRow;
        },

        updateDBFXOriginResultFromPropertygridData: function (prefix) {
            if (DBFXOriginResult == undefined) DBFXOriginResult = { total: "1", rows: [{}] };//注意格式
            if (DBFXOriginResult.rows[addOrUpdateIndex] == undefined) DBFXOriginResult.rows[addOrUpdateIndex] = {};//注意格式

            var newData = $("#pg").propertygrid('getData');
            for (var i = 0; i < newData.rows.length; i++) {
                if (newData.rows[i].index == addOrUpdateIndex) {
                    var id = newData.rows[i].id;
                    if (id != undefined && id.indexOf(prefix) != -1) {
                        id = id.substring(prefix.length);
                        DBFXOriginResult.rows[addOrUpdateIndex][id] = newData.rows[i].value;
                    }
                }
            } 
        },
         
        faSong: function () {
            var returnFlag = BS_.baoCun();
            if (returnFlag == true) {
                window.location.href = '/DuiBiFenXi/DuiBiFenXiFaQi?FK_Flow=' + FK_Flow + '&WorkID=' + WorkID;
            }
        },
    
        liuChengTu: function () {
            window.open('/DuiBiFenXi/ChaKanLiuChengTu?FK_Flow=' + FK_Flow);
        },

        chaoSong: function () {
            var _json = {
                'FK_Flow': FK_Flow, 'FK_Node': FK_Node,
                'WorkID': WorkID, 'FID': FID
            }
            openChaoSongDialog(_json);
        },

        addLink: function () {
            var returnFlag = BS_.baoCun();
            if (returnFlag == true) {
                //点击选择文件框的时候已经选择了该行了
                var row = $("#pg").propertygrid('getSelected');
                var rowIndex = $("#pg").propertygrid('getRowIndex', row);

                var No = _CommomOperation.selectNearestOverNo(rowIndex, currentPrefix + "No");
                openLinkDialog(No, IsShenHe.NO);
            }
        },

        addAttach: function () {
            var returnFlag = BS_.baoCun();
            if (returnFlag == true) {
                //点击选择文件框的时候已经选择了该行了
                var row = $("#pg").propertygrid('getSelected');
                var rowIndex = $("#pg").propertygrid('getRowIndex', row);

                var No = _CommomOperation.selectNearestOverNo(rowIndex, currentPrefix + "No");
                openAttachDialog(No, IsShenHe.NO);
            }
        }
    }