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
            AT_.AjaxPost("/SheJiSuanFa/GetHistoryData", argsVal, BS_.onLoadSuccess);
             
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
                SJSFOriginResult = parseJSON(data._Json);

                if (SJSFOriginResult.rows.length == 1) {
                    BS_.getPropertygridDataFormJsonResult(SJSFOriginResult, "SF_");

                    //只有一个结果时，不给删除
                    var pData = $("#pg").propertygrid('getData');
                    for (var i = pData.rows.length - 1; i >= 0; i--) {
                        if (pData.rows[i].id != undefined && pData.rows[i].id == "SF_del") {
                            $("#pg").propertygrid("deleteRow", i);
                        }
                    }
                }
                else {
                    BS_.getAndCloneSFPropertygridDataFormJsonResult(SJSFOriginResult, "SF_");
                }

                maxEditIndex = SJSFOriginResult.rows.length - 1;
                $("#pg").propertygrid("collapseGroup");
                $("#pg").propertygrid("expandGroup", 0);
            } else {
                //只有一个结果时，不给删除
                var pData = $("#pg").propertygrid('getData');
                for (var i = pData.rows.length - 1; i >= 0; i--) {
                    if (pData.rows[i].id != undefined && pData.rows[i].id == "SF_del") {
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
     
        getAndCloneSFPropertygridDataFormJsonResult: function (result, prefix) {
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






        appendSF: function () {

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
                    group: BS_.getGroupNameByPrefix("SF_"),
                    index: targetIndex
                }
            });

            var tempData = BS_.getClonePropertygridByPrefix("SF_", 0, 0);
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

        deleteSF: function () {
            $.messager.confirm("删除算法结果", "您确定删除该算法结果吗？", function (flag) {
                if (flag == true) {
                    var row = $("#pg").propertygrid("getSelected");
                    var rowIndex = $("#pg").propertygrid('getRowIndex', row);
                    var pData = $("#pg").propertygrid('getData');

                    //非新增的数据
                    if (row.index + 1 <= SJSFOriginResult.rows.length) {
                        var argsVal = {
                            "sfNo": SJSFOriginResult.rows[row.index].No
                        }
                        $("#pg").propertygrid("loading");
                        AT_.AjaxPost("/SheJiSuanFa/ShanChuSuanFa", argsVal, BS_.deleteSuccess);
                    }
                    else {
                        BS_.deleteRowFromPropertygrid();
                    }
                    
                }
            });
        },

        deleteSuccess: function (data, status) {
            $.messager.show({
                title: '删除算法结果',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });

            if (data.state == "0") {
                var row = $("#pg").propertygrid("getSelected");
                SJSFOriginResult.rows.splice(row.index, 1);

                BS_.deleteRowFromPropertygrid();
            }  
        },

        deleteRowFromPropertygrid: function () {
            var row = $("#pg").propertygrid("getSelected");
            var rowIndex = $("#pg").propertygrid('getRowIndex', row);
            var pData = $("#pg").propertygrid('getData');

            for (var i = pData.rows.length - 1; i >= 0; i--) {
                if (pData.rows[i].group == BS_.getGroupNameByPrefix("SF_")) {
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
                var insertIndex = BS_.getLastInsertIndexByGroupName(BS_.getGroupNameByPrefix("SF_"));
                for (var i = 0; i < SJSFPropertygridData.length; i++) {
                    if (SJSFPropertygridData[i].id == "SF_add") {
                        var temp = cloneJSON(SJSFPropertygridData[i]);
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
            
            if (SJSFOriginResult == undefined) {
                //新增
                BS_.add(maxEditIndex);
            }
            else if (SJSFOriginResult.rows.length == maxEditIndex + 1) {
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

            AT_.AjaxPost("/SheJiSuanFa/TianJiaSuanFa", newRow, BS_.addSuccess);
        },

        addSuccess: function (data, status) {
            $.messager.show({
                title: '添加算法结果',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });

            if (data.state == "0") {
                BS_.updateSJSFOriginResultFromPropertygridData("SF_");
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
                    newRow["oldNo"] = SJSFOriginResult.rows[index].No;
                    AT_.AjaxPost("/SheJiSuanFa/XiuGaiSuanFa", newRow, BS_.updateSuccess);
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
                        var prefix = "SF_";
                        var subid = id.substring(prefix.length);
                        if (SJSFOriginResult.rows[index][subid] != editData.rows[i].value) {
                            return true;
                        }
                    }
                }
            }
            return false;
        },

        ifColumnNeedCompare: function (columnName) {
            if (columnName == "SF_No" ||
                columnName == "SF_Name" ||
                columnName == "SF_ProposeTime" ||
                columnName == "SF_FK_XSHOID" ||
                columnName == "SF_Keys" ||
                columnName == "SF_Description" ||
                columnName == "SF_Design" ||
                columnName == "SF_RealizeStep" ||
                columnName == "SF_Remarks")
                return true;
            else
                return false;
        },

        updateSuccess: function (data, status) {
            $.messager.show({
                title: '更新算法结果',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });

            if (data.state == "0") {
                BS_.updateSJSFOriginResultFromPropertygridData("SF_");
                lastSaveResult = true;
            } else {
                lastSaveResult = false;
            }
        },

        getNewRowFromPropertygrid: function (index) {
            var newRow = {};
            newRow["No"] = BS_.getValueFromPropertygridById("SF_No", index);
            newRow["Name"] = BS_.getValueFromPropertygridById("SF_Name", index);
            newRow["FK_Proposer"] = currentLoginUser;
            newRow["ProposeTime"] = BS_.getValueFromPropertygridById("SF_ProposeTime", index);
            newRow["FK_XSHOID"] = BS_.getValueFromPropertygridById("SF_FK_XSHOID", index);
            newRow["Keys"] = BS_.getValueFromPropertygridById("SF_Keys", index);
            newRow["Description"] = BS_.getValueFromPropertygridById("SF_Description", index);
            newRow["Design"] = BS_.getValueFromPropertygridById("SF_Design", index);
            newRow["RealizeStep"] = BS_.getValueFromPropertygridById("SF_RealizeStep", index);
            newRow["Remarks"] = BS_.getValueFromPropertygridById("SF_Remarks", index);
            newRow["ModifyTime"] = (new Date()).Format("dd/MM/yyyy hh:mm:ss");
            
            newRow["FK_Flow"] = FK_Flow;
            newRow["FK_Node"] = FK_Node;
            newRow["WorkID"] = WorkID;
            return newRow;
        },

        updateSJSFOriginResultFromPropertygridData: function (prefix) {
            if (SJSFOriginResult == undefined) SJSFOriginResult = { total: "1", rows: [{}] };//注意格式
            if (SJSFOriginResult.rows[addOrUpdateIndex] == undefined) SJSFOriginResult.rows[addOrUpdateIndex] = {};//注意格式

            var newData = $("#pg").propertygrid('getData');
            for (var i = 0; i < newData.rows.length; i++) {
                if (newData.rows[i].index == addOrUpdateIndex) {
                    var id = newData.rows[i].id;
                    if (id != undefined && id.indexOf(prefix) != -1) {
                        id = id.substring(prefix.length);
                        SJSFOriginResult.rows[addOrUpdateIndex][id] = newData.rows[i].value;
                    }
                }
            } 
        },
         
        faSong: function () {
            var returnFlag = BS_.baoCun();
            if (returnFlag == true) {
                window.location.href = '/SheJiSuanFa/SheJiSuanFaFaQi?FK_Flow=' + FK_Flow + '&WorkID=' + WorkID;
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
            window.open('/XingShiHua/ChaKanLiuChengTu?FK_Flow=' + FK_Flow);
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
                $("#pg").propertygrid("expandGroup", 11);
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
                $("#pg").propertygrid("expandGroup", 10);
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
                $("#pg").propertygrid("expandGroup", 9);
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
                $("#pg").propertygrid("expandGroup", 8);
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
                $("#pg").propertygrid("expandGroup", 7);
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
                $("#pg").propertygrid("expandGroup", 6);
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
                $("#pg").propertygrid("expandGroup", 5);
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
                $("#pg").propertygrid("expandGroup", 4);
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
                $("#pg").propertygrid("expandGroup", 3);
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
                $("#pg").propertygrid("expandGroup", 2);
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
            if (prefix == "SF_") return "设计算法基本信息";
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
        }
    }