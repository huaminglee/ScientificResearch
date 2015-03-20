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
                BS_.getPropertygridDataFormJsonResult(DBFXOriginResult, "DBFX_");

                //只有一个结果时，不给删除
                var pData = $("#pg").propertygrid('getData');
                for (var i = pData.rows.length - 1; i >= 0; i--) {
                    if (pData.rows[i].id != undefined && pData.rows[i].id == "DBFX_del") {
                        $("#pg").propertygrid("deleteRow", i);
                    }
                }
            }
            else {
                BS_.getAndCloneDBFXPropertygridDataFormJsonResult(DBFXOriginResult, "DBFX_");
            }

            maxEditIndex = DBFXOriginResult.rows.length - 1;
            $("#pg").propertygrid("collapseGroup");
            $("#pg").propertygrid("expandGroup", 0);
        } else {
            //只有一个结果时，不给删除
            var pData = $("#pg").propertygrid('getData');
            for (var i = pData.rows.length - 1; i >= 0; i--) {
                if (pData.rows[i].id != undefined && pData.rows[i].id == "DBFX_del") {
                    $("#pg").propertygrid("deleteRow", i);
                }
            }

            maxEditIndex = 0;
        }
    },
     
    getPropertygridDataFormJsonResult: function (result, prefix) {
        BS_.propertygridEndEdit();

        //prefix是前缀，防止id重复
        var rows = result.rows;
        if (rows == undefined || rows.length == 0) {
            return;
        }
        var newData = $("#pg").propertygrid('getData');
        for (var i = 0; i < newData.rows.length; i++) {
            var id = newData.rows[i].id;
            if (id != undefined && id.indexOf(prefix) != -1) {
                id = id.substring(prefix.length);
                console.log(id + ":" + result.rows[0][id]);
                newData.rows[i].value = result.rows[0][id];
            }
        }
        $('#pg').propertygrid({ data: newData });
    },
     
    getAndCloneDBFXPropertygridDataFormJsonResult: function (result, prefix) {
        BS_.propertygridEndEdit();
            
        //prefix是前缀，防止id重复
        var rows = result.rows;
        if (rows == undefined || rows.length == 0) {
            return;
        }
        var insertIndex = BS_.getFirstInsertIndexByGroupName(BS_.getGroupNameByPrefix(prefix));

        BS_.deletePropertygridDataByGroupName(prefix);
              
        var newData = $("#pg").propertygrid('getData');
        for (var i = 0; i < rows.length; i++) {
            var tempData = undefined;
            if (i == 0)
                tempData = BS_.getClonePropertygridByPrefix(prefix, 0, 2);
            else if (i < rows.length-1)
                tempData = BS_.getClonePropertygridByPrefix(prefix, 0, 1);
            else
                tempData = BS_.getClonePropertygridByPrefix(prefix, 0, 0);

            for (var j = 0; j < tempData.length; j++) {
                //更新index
                tempData[j].index = i;

                var id = tempData[j].id;
                if (id != undefined && id.indexOf(prefix) != -1) {
                    id = id.substring(prefix.length);
                    tempData[j].value = result.rows[i][id];
                }
            }

            if (i != rows.length - 1) {
                tempData.push({
                    name: "分隔位置",
                    value: ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>",
                    group: BS_.getGroupNameByPrefix(prefix),
                    index: i + 1
                });
            }
              
            tempData.forEach(function (e) {
                newData.rows.splice(insertIndex++, 0, e);
            });
                 
        }
        $('#pg').propertygrid({ data: newData });
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
                group: BS_.getGroupNameByPrefix("DBFX_"),
                index: targetIndex
            }
        });

        var tempData = BS_.getClonePropertygridByPrefix("DBFX_", 0, 0);
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
            if (pData.rows[i].group == BS_.getGroupNameByPrefix("DBFX_")) {
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
            var insertIndex = BS_.getLastInsertIndexByGroupName(BS_.getGroupNameByPrefix("DBFX_"));
            for (var i = 0; i < DBFXPropertygridData.length; i++) {
                if (DBFXPropertygridData[i].id == "DBFX_add") {
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
        BS_.propertygridEndEdit();
            
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
            BS_.updateDBFXOriginResultFromPropertygridData("DBFX_");
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
                    var prefix = "DBFX_";
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
        if (columnName == "DBFX_No" ||
                columnName == "DBFX_Name" ||
                columnName == "DBFX_ProposeTime" ||
                columnName == "DBFX_FK_SYOID" ||
                columnName == "DBFX_Keys" ||
                columnName == "DBFX_Description" ||
                columnName == "DBFX_Methods" ||
                columnName == "DBFX_Data" ||
                columnName == "DBFX_AnalysisResult" ||
                columnName == "DBFX_InferType" ||
                columnName == "DBFX_InferContent" ||
                columnName == "DBFX_Remarks" )
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
                BS_.updateDBFXOriginResultFromPropertygridData("DBFX_");
                lastSaveResult = true;
            } else {
                lastSaveResult = false;
            }
        },

        getNewRowFromPropertygrid: function (index) {
            var newRow = {};
            newRow["No"] = BS_.getValueFromPropertygridById("DBFX_No", index);
            newRow["Name"] = BS_.getValueFromPropertygridById("DBFX_Name", index);
            newRow["FK_Proposer"] = currentLoginUser;
            newRow["ProposeTime"] = BS_.getValueFromPropertygridById("DBFX_ProposeTime", index);
            newRow["FK_SYOID"] = BS_.getValueFromPropertygridById("DBFX_FK_SYOID", index);
            newRow["Keys"] = BS_.getValueFromPropertygridById("DBFX_Keys", index);
            newRow["Description"] = BS_.getValueFromPropertygridById("DBFX_Description", index);
            newRow["Methods"] = BS_.getValueFromPropertygridById("DBFX_Methods", index);
            newRow["Data"] = BS_.getValueFromPropertygridById("DBFX_Data", index);
            newRow["AnalysisResult"] = BS_.getValueFromPropertygridById("DBFX_AnalysisResult", index);
            newRow["InferType"] = BS_.getValueFromPropertygridById("DBFX_InferType", index);
            newRow["InferContent"] = BS_.getValueFromPropertygridById("DBFX_InferContent", index);
            newRow["Remarks"] = BS_.getValueFromPropertygridById("DBFX_Remarks", index);
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
  
        getValueFromPropertygridById: function (id, index) {
            var data = $("#pg").propertygrid('getData');
            for (var i = 0; i < data.rows.length; i++) {
                if (data.rows[i].id == id && data.rows[i].index == index) {
                    return data.rows[i].value;
                }
            }
            return "";
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
        
        propertygridEndEdit: function () {
            var row = $("#pg").propertygrid("getSelected");
            var editIndex = $("#pg").propertygrid('getRowIndex', row);
            if ($("#pg").propertygrid('validateRow', editIndex)) {
                $("#pg").propertygrid('endEdit', editIndex);
            }
        },















        loadProjectHistoryData: function () {
            var argsVal = {
                "FK_Flow": FK_Flow,
                "WorkID": WorkID,
                "stepType": StepType.PROJECT_XUQIUFENXI
            }

            $("#pg").propertygrid("loading");
            AT_.AjaxPost("/XuQiuFenXi/GetHistoryDataFromTrack", argsVal, BS_.onLoadProjectHistoryDataSuccess);
        },

        onLoadProjectHistoryDataSuccess: function (data, status) {
            $.messager.show({
                title: '加载数据',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            $("#pg").propertygrid("loaded");

            if (data.state == "0") {
                var tempResult = parseJSON(data._Json);
                BS_.getPropertygridDataFormJsonResult(tempResult, "XM_");
                $("#pg").propertygrid("collapseGroup");
                $("#pg").propertygrid("expandGroup", 0);
                $("#pg").propertygrid("expandGroup", 15);
            }
        },

        loadSubjectHistoryData: function () {
            var argsVal = {
                "FK_Flow": FK_Flow,
                "WorkID": WorkID,
                "stepType": StepType.SUBJECT_XUQIUFENXI
            }

            $("#pg").propertygrid("loading");
            AT_.AjaxPost("/XuQiuFenXi/GetHistoryDataFromTrack", argsVal, BS_.onLoadSubjectHistoryDataSuccess);
        },

        onLoadSubjectHistoryDataSuccess: function (data, status) {
            $.messager.show({
                title: '加载数据',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            $("#pg").propertygrid("loaded");

            if (data.state == "0") {
                var tempResult = parseJSON(data._Json);
                BS_.getPropertygridDataFormJsonResult(tempResult, "KT_");
                $("#pg").propertygrid("collapseGroup");
                $("#pg").propertygrid("expandGroup", 0);
                $("#pg").propertygrid("expandGroup", 14);
            }
        },

        loadXuQiuFenXiShenHeHistoryData: function () {
            var argsVal = {
                "FK_Flow": FK_Flow,
                "WorkID": WorkID,
                "stepType": StepType.SUBJECT_XUQIUFENXI
            }

            $("#pg").propertygrid("loading");
            AT_.AjaxPost("/ShenHe/GetShenHeHistoryDataByStep", argsVal, BS_.onLoadXuQiuFenXiShenHeHistoryDataSuccess);
        },

        onLoadXuQiuFenXiShenHeHistoryDataSuccess: function (data, status) {
            $.messager.show({
                title: '加载数据',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            $("#pg").propertygrid("loaded");

            if (data.state == "0") {
                var tempResult = parseJSON(data._Json);
                if (tempResult.rows.length == 1) {
                    BS_.getPropertygridDataFormJsonResult(tempResult, "XQFXSH_");
                }
                else {
                    BS_.getAndClonePropertygridDataFormJsonResult(tempResult, "XQFXSH_");
                }
                $("#pg").propertygrid("collapseGroup");
                $("#pg").propertygrid("expandGroup", 0);
                $("#pg").propertygrid("expandGroup", 13);
            }
        },

        loadDiaoYanHistoryData: function () {
            var argsVal = {
                "FK_Flow": FK_Flow,
                "WorkID": WorkID,
                "stepType": StepType.DIAOYAN
            }

            $("#pg").propertygrid("loading");
            AT_.AjaxPost("/DiaoYan/GetHistoryDataFromTrack", argsVal, BS_.onLoadDiaoYanHistoryDataSuccess);
        },

        onLoadDiaoYanHistoryDataSuccess: function (data, status) {
            $.messager.show({
                title: '加载数据',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            $("#pg").propertygrid("loaded");

            if (data.state == "0") {
                var tempResult = parseJSON(data._Json);
                if (tempResult.rows.length == 1) {
                    BS_.getPropertygridDataFormJsonResult(tempResult, "DY_");
                }
                else {
                    BS_.getAndClonePropertygridDataFormJsonResult(tempResult, "DY_");
                }
                $("#pg").propertygrid("collapseGroup");
                $("#pg").propertygrid("expandGroup", 0);
                $("#pg").propertygrid("expandGroup", 12);
            }
        },

        loadDiaoYanShenHeHistoryData: function () {
            var argsVal = {
                "FK_Flow": FK_Flow,
                "WorkID": WorkID,
                "stepType": StepType.DIAOYAN
            }

            $("#pg").propertygrid("loading");
            AT_.AjaxPost("/ShenHe/GetShenHeHistoryDataByStep", argsVal, BS_.onLoadDiaoYanShenHeHistoryDataSuccess);
        },

        onLoadDiaoYanShenHeHistoryDataSuccess: function (data, status) {
            $.messager.show({
                title: '加载数据',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            $("#pg").propertygrid("loaded");

            if (data.state == "0") {
                var tempResult = parseJSON(data._Json);
                if (tempResult.rows.length == 1) {
                    BS_.getPropertygridDataFormJsonResult(tempResult, "DYSH_");
                }
                else {
                    BS_.getAndClonePropertygridDataFormJsonResult(tempResult, "DYSH_");
                }
                $("#pg").propertygrid("collapseGroup");
                $("#pg").propertygrid("expandGroup", 0);
                $("#pg").propertygrid("expandGroup", 11);
            }
        },

        loadTiChuWenTiHistoryData: function () {
            var argsVal = {
                "FK_Flow": FK_Flow,
                "WorkID": WorkID,
                "stepType": StepType.TICHUWENTI
            }

            $("#pg").propertygrid("loading");
            AT_.AjaxPost("/TiChuWenTi/GetHistoryDataFromTrack", argsVal, BS_.onLoadTiChuWenTiHistoryDataSuccess);
        },

        onLoadTiChuWenTiHistoryDataSuccess: function (data, status) {
            $.messager.show({
                title: '加载数据',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            $("#pg").propertygrid("loaded");

            if (data.state == "0") {
                var tempResult = parseJSON(data._Json);
                if (tempResult.rows.length == 1) {
                    BS_.getPropertygridDataFormJsonResult(tempResult, "TCWT_");
                }
                else {
                    BS_.getAndClonePropertygridDataFormJsonResult(tempResult, "TCWT_");
                }
                $("#pg").propertygrid("collapseGroup");
                $("#pg").propertygrid("expandGroup", 0);
                $("#pg").propertygrid("expandGroup", 10);
            }
        },

        loadTiChuWenTiShenHeHistoryData: function () {
            var argsVal = {
                "FK_Flow": FK_Flow,
                "WorkID": WorkID,
                "stepType": StepType.TICHUWENTI
            }

            $("#pg").propertygrid("loading");
            AT_.AjaxPost("/ShenHe/GetShenHeHistoryDataByStep", argsVal, BS_.onLoadTiChuWenTiShenHeHistoryDataSuccess);
        },

        onLoadTiChuWenTiShenHeHistoryDataSuccess: function (data, status) {
            $.messager.show({
                title: '加载数据',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            $("#pg").propertygrid("loaded");

            if (data.state == "0") {
                var tempResult = parseJSON(data._Json);
                if (tempResult.rows.length == 1) {
                    BS_.getPropertygridDataFormJsonResult(tempResult, "TCWTSH_");
                }
                else {
                    BS_.getAndClonePropertygridDataFormJsonResult(tempResult, "TCWTSH_");
                }
                $("#pg").propertygrid("collapseGroup");
                $("#pg").propertygrid("expandGroup", 0);
                $("#pg").propertygrid("expandGroup", 9);
            }
        },

        loadJieJueSiLuHistoryData: function () {
            var argsVal = {
                "FK_Flow": FK_Flow,
                "WorkID": WorkID,
                "stepType": StepType.JIEJUESILU
            }

            $("#pg").propertygrid("loading");
            AT_.AjaxPost("/JieJueSiLu/GetHistoryDataFromTrack", argsVal, BS_.onLoadJieJueSiLuHistoryDataSuccess);
        },

        onLoadJieJueSiLuHistoryDataSuccess: function (data, status) {
            $.messager.show({
                title: '加载数据',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            $("#pg").propertygrid("loaded");

            if (data.state == "0") {
                var tempResult = parseJSON(data._Json);
                if (tempResult.rows.length == 1) {
                    BS_.getPropertygridDataFormJsonResult(tempResult, "JJSL_");
                }
                else {
                    BS_.getAndClonePropertygridDataFormJsonResult(tempResult, "JJSL_");
                }
                $("#pg").propertygrid("collapseGroup");
                $("#pg").propertygrid("expandGroup", 0);
                $("#pg").propertygrid("expandGroup", 8);
            }
        },

        loadJieJueSiLuShenHeHistoryData: function () {
            var argsVal = {
                "FK_Flow": FK_Flow,
                "WorkID": WorkID,
                "stepType": StepType.JIEJUESILU
            }

            $("#pg").propertygrid("loading");
            AT_.AjaxPost("/ShenHe/GetShenHeHistoryDataByStep", argsVal, BS_.onLoadJieJueSiLuShenHeHistoryDataSuccess);
        },

        onLoadJieJueSiLuShenHeHistoryDataSuccess: function (data, status) {
            $.messager.show({
                title: '加载数据',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            $("#pg").propertygrid("loaded");

            if (data.state == "0") {
                var tempResult = parseJSON(data._Json);
                if (tempResult.rows.length == 1) {
                    BS_.getPropertygridDataFormJsonResult(tempResult, "JJSLSH_");
                }
                else {
                    BS_.getAndClonePropertygridDataFormJsonResult(tempResult, "JJSLSH_");
                }
                $("#pg").propertygrid("collapseGroup");
                $("#pg").propertygrid("expandGroup", 0);
                $("#pg").propertygrid("expandGroup", 7);
            }
        },

        loadXingShiHuaHistoryData: function () {
            var argsVal = {
                "FK_Flow": FK_Flow,
                "WorkID": WorkID,
                "stepType": StepType.XINGSHIHUA
            }

            $("#pg").propertygrid("loading");
            AT_.AjaxPost("/XingShiHua/GetHistoryDataFromTrack", argsVal, BS_.onLoadXingShiHuaHistoryDataSuccess);
        },

        onLoadXingShiHuaHistoryDataSuccess: function (data, status) {
            $.messager.show({
                title: '加载数据',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            $("#pg").propertygrid("loaded");

            if (data.state == "0") {
                var tempResult = parseJSON(data._Json);
                if (tempResult.rows.length == 1) {
                    BS_.getPropertygridDataFormJsonResult(tempResult, "XSH_");
                }
                else {
                    BS_.getAndClonePropertygridDataFormJsonResult(tempResult, "XSH_");
                }
                $("#pg").propertygrid("collapseGroup");
                $("#pg").propertygrid("expandGroup", 0);
                $("#pg").propertygrid("expandGroup", 6);
            }
        },

        loadXingShiHuaShenHeHistoryData: function () {
            var argsVal = {
                "FK_Flow": FK_Flow,
                "WorkID": WorkID,
                "stepType": StepType.XINGSHIHUA
            }

            $("#pg").propertygrid("loading");
            AT_.AjaxPost("/ShenHe/GetShenHeHistoryDataByStep", argsVal, BS_.onLoadXingShiHuaShenHeHistoryDataSuccess);
        },

        onLoadXingShiHuaShenHeHistoryDataSuccess: function (data, status) {
            $.messager.show({
                title: '加载数据',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            $("#pg").propertygrid("loaded");

            if (data.state == "0") {
                var tempResult = parseJSON(data._Json);
                if (tempResult.rows.length == 1) {
                    BS_.getPropertygridDataFormJsonResult(tempResult, "XSHSH_");
                }
                else {
                    BS_.getAndClonePropertygridDataFormJsonResult(tempResult, "XSHSH_");
                }
                $("#pg").propertygrid("collapseGroup");
                $("#pg").propertygrid("expandGroup", 0);
                $("#pg").propertygrid("expandGroup", 5);
            }
        },

        loadSheJiSuanFaHistoryData: function () {
            var argsVal = {
                "FK_Flow": FK_Flow,
                "WorkID": WorkID,
                "stepType": StepType.SHEJISUANFA
            }

            $("#pg").propertygrid("loading");
            AT_.AjaxPost("/SheJiSuanFa/GetHistoryDataFromTrack", argsVal, BS_.onLoadSheJiSuanFaHistoryDataSuccess);
        },

        onLoadSheJiSuanFaHistoryDataSuccess: function (data, status) {
            $.messager.show({
                title: '加载数据',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            $("#pg").propertygrid("loaded");

            if (data.state == "0") {
                var tempResult = parseJSON(data._Json);
                if (tempResult.rows.length == 1) {
                    BS_.getPropertygridDataFormJsonResult(tempResult, "SF_");
                }
                else {
                    BS_.getAndClonePropertygridDataFormJsonResult(tempResult, "SF_");
                }
                $("#pg").propertygrid("collapseGroup");
                $("#pg").propertygrid("expandGroup", 0);
                $("#pg").propertygrid("expandGroup", 4);
            }
        },
         
        loadSheJiSuanFaShenHeHistoryData: function () {
            var argsVal = {
                "FK_Flow": FK_Flow,
                "WorkID": WorkID,
                "stepType": StepType.SHEJISUANFA
            }

            $("#pg").propertygrid("loading");
            AT_.AjaxPost("/ShenHe/GetShenHeHistoryDataByStep", argsVal, BS_.onLoadSheJiSuanFaShenHeHistoryDataSuccess);
        },

        onLoadSheJiSuanFaShenHeHistoryDataSuccess: function (data, status) {
            $.messager.show({
                title: '加载数据',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            $("#pg").propertygrid("loaded");

            if (data.state == "0") {
                var tempResult = parseJSON(data._Json);
                if (tempResult.rows.length == 1) {
                    BS_.getPropertygridDataFormJsonResult(tempResult, "SJSFSH_");
                }
                else {
                    BS_.getAndClonePropertygridDataFormJsonResult(tempResult, "SJSFSH_");
                }
                $("#pg").propertygrid("collapseGroup");
                $("#pg").propertygrid("expandGroup", 0);
                $("#pg").propertygrid("expandGroup", 3);
            }
        },
     
        loadSheJiShiYanHistoryData: function () {
            var argsVal = {
                "FK_Flow": FK_Flow,
                "WorkID": WorkID,
                "stepType": StepType.SHEJISHIYAN
            }

            $("#pg").propertygrid("loading");
            AT_.AjaxPost("/SheJiShiYan/GetHistoryDataFromTrack", argsVal, BS_.onLoadSheJiShiYanHistoryDataSuccess);
        },

        onLoadSheJiShiYanHistoryDataSuccess: function (data, status) {
            $.messager.show({
                title: '加载数据',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            $("#pg").propertygrid("loaded");

            if (data.state == "0") {
                var tempResult = parseJSON(data._Json);
                if (tempResult.rows.length == 1) {
                    BS_.getPropertygridDataFormJsonResult(tempResult, "SY_");
                }
                else {
                    BS_.getAndClonePropertygridDataFormJsonResult(tempResult, "SY_");
                }
                $("#pg").propertygrid("collapseGroup");
                $("#pg").propertygrid("expandGroup", 0);
                $("#pg").propertygrid("expandGroup", 2);
            }
        },

        loadSheJiShiYanShenHeHistoryData: function () {
            var argsVal = {
                "FK_Flow": FK_Flow,
                "WorkID": WorkID,
                "stepType": StepType.SHEJISHIYAN
            }

            $("#pg").propertygrid("loading");
            AT_.AjaxPost("/ShenHe/GetShenHeHistoryDataByStep", argsVal, BS_.onLoadSheJiShiYanShenHeHistoryDataSuccess);
        },

        onLoadSheJiShiYanShenHeHistoryDataSuccess: function (data, status) {
            $.messager.show({
                title: '加载数据',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            $("#pg").propertygrid("loaded");

            if (data.state == "0") {
                var tempResult = parseJSON(data._Json);
                if (tempResult.rows.length == 1) {
                    BS_.getPropertygridDataFormJsonResult(tempResult, "SJSYSH_");
                }
                else {
                    BS_.getAndClonePropertygridDataFormJsonResult(tempResult, "SJSYSH_");
                }
                $("#pg").propertygrid("collapseGroup");
                $("#pg").propertygrid("expandGroup", 0);
                $("#pg").propertygrid("expandGroup", 1);
            }
        },

        getAndClonePropertygridDataFormJsonResult: function (result, prefix) {
            BS_.propertygridEndEdit();

            //prefix是前缀，防止id重复
            var rows = result.rows;
            if (rows == undefined || rows.length == 0) {
                return;
            }
            var insertIndex = BS_.getFirstInsertIndexByGroupName(BS_.getGroupNameByPrefix(prefix));

            BS_.deletePropertygridDataByGroupName(prefix);

            var newData = $("#pg").propertygrid('getData');
            for (var i = 0; i < rows.length; i++) {
                var tempData = undefined;
                if (i == 0)
                    tempData = BS_.getClonePropertygridByPrefix(prefix, 0, 0);
                else
                    tempData = BS_.getClonePropertygridByPrefix(prefix, 1, 0);

                for (var j = 0; j < tempData.length; j++) {
                    var id = tempData[j].id;
                    if (id != undefined && id.indexOf(prefix) != -1) {
                        id = id.substring(prefix.length);
                        tempData[j].value = result.rows[i][id];
                    }
                }

                tempData.forEach(function (e) {
                    newData.rows.splice(insertIndex++, 0, e);
                });
                if (i != rows.length - 1) {
                    newData.rows.splice(insertIndex++, 0, {
                        name: "分隔位置",
                        value: ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>",
                        group: BS_.getGroupNameByPrefix(prefix),
                    });
                }
            }

            $('#pg').propertygrid({ data: newData });
        },

        deletePropertygridDataByGroupName: function (prefix) {
            var newData = $("#pg").propertygrid('getData');
            for (var i = newData.rows.length - 1; i >= 0; i--) {
                if (newData.rows[i].group == BS_.getGroupNameByPrefix(prefix)) {
                    $("#pg").propertygrid('deleteRow', i);
                }
            }
        },

        getGroupNameByPrefix: function (prefix) {
            if (prefix == "XQFXSH_") return "需求分析审核结果";
            if (prefix == "DY_") return "调研结果";
            if (prefix == "DYSH_") return "调研审核结果";
            if (prefix == "TCWT_") return "提出问题结果";
            if (prefix == "TCWTSH_") return "提出问题审核结果";
            if (prefix == "JJSL_") return "解决思路结果";
            if (prefix == "JJSLSH_") return "解决思路审核结果";
            if (prefix == "XSH_") return "形式化结果";
            if (prefix == "XSHSH_") return "形式化审核结果";
            if (prefix == "SF_") return "设计算法结果";
            if (prefix == "SJSFSH_") return "设计算法审核结果";
            if (prefix == "SY_") return "设计实验结果";
            if (prefix == "SJSYSH_") return "设计实验审核结果";
            if (prefix == "DBFX_") return "量化对比分析基本信息";
        },

        getFirstInsertIndexByGroupName: function (groupName) {
            var newData = $("#pg").propertygrid('getData');
            for (var i = 0; i < newData.rows.length; i++) {
                var group = newData.rows[i].group;
                if (group != undefined && group == groupName) {
                    return i;
                }
            }
            return 0;
        },

        getLastInsertIndexByGroupName: function (groupName) {
            var index = 0;
            var newData = $("#pg").propertygrid('getData');
            for (var i = 0; i < newData.rows.length; i++) {
                var group = newData.rows[i].group;
                if (group != undefined && group == groupName) {
                    index = i;
                }
            }
            return index + 1;
        },

        getClonePropertygridByPrefix: function (prefix, frontRowRemoveCount, lastRowRemoveCount) {
            var tempData = BS_.getTargetPropertygridDataByPrefix(prefix);
            var temp = [];

            if ((frontRowRemoveCount > tempData.length) ||
                (lastRowRemoveCount > tempData.length) ||
                (frontRowRemoveCount + lastRowRemoveCount > tempData.length))
                return;

            for (var i = frontRowRemoveCount; i < tempData.length - lastRowRemoveCount; i++) {
                temp.push(tempData[i]);
            }
            return temp;
        },

        getTargetPropertygridDataByPrefix: function (prefix) {
            var target = [];
            if (prefix == "XQFXSH_") {
                $.extend(true, target, XQFXShenHePropertygridData);
                return target;
            }
            if (prefix == "DY_") {
                $.extend(true, target, DYPropertygridData);
                return target;
            }
            if (prefix == "DYSH_") {
                $.extend(true, target, DYShenHePropertygridData);
                return target;
            }
            if (prefix == "TCWT_") {
                $.extend(true, target, TCWTPropertygridData);
                return target;
            }
            if (prefix == "TCWTSH_") {
                $.extend(true, target, TCWTShenHePropertygridData);
                return target;
            }
            if (prefix == "JJSL_") {
                $.extend(true, target, JJSLPropertygridData);
                return target;
            }
            if (prefix == "JJSLSH_") {
                $.extend(true, target, JJSLShenHePropertygridData);
                return target;
            }
            if (prefix == "XSH_") {
                $.extend(true, target, XSHPropertygridData);
                return target;
            }
            if (prefix == "XSHSH_") {
                $.extend(true, target, XSHShenHePropertygridData);
                return target;
            }
            if (prefix == "SF_") {
                $.extend(true, target, SJSFPropertygridData);
                return target;
            }
            if (prefix == "SJSFSH_") {
                $.extend(true, target, SJSFShenHePropertygridData);
                return target;
            }
            if (prefix == "SY_") {
                $.extend(true, target, SJSYPropertygridData);
                return target;
            }
            if (prefix == "SJSYSH_") {
                $.extend(true, target, SJSYShenHePropertygridData);
                return target;
            }
            if (prefix == "DBFX_") {
                $.extend(true, target, DBFXPropertygridData);
                return target;
            }
        }
    }