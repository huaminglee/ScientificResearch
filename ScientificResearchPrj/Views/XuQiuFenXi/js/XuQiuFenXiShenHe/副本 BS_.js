var saveResult = undefined;

var BS_ = {
        onLoad: function () {
            var argsVal = {
                "FK_Flow": FK_Flow,
                "WorkID": WorkID,
                "FK_Node": FK_Node,
                "shenHeRen": currentLoginUser
            }
           
            $("#pg").propertygrid("loading");
            AT_.AjaxPost("/ShenHe/GetShenHeHistoryData", argsVal, BS_.onLoadSuccess);
             
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
                XQFXShenHeOriginResult = parseJSON(data._Json);

                BS_.getPropertygridDataFormJsonResult(XQFXShenHeOriginResult, "XQFXSH_");
                $("#pg").propertygrid("collapseGroup");
                $("#pg").propertygrid("expandGroup", 0);

                if (XQFXShenHeOriginResult.rows[0].ShenHeJieGuo == SHENHEJIEGUO.RETURN) {
                    var newData = $("#pg").propertygrid('getData');
                    for (var i = 0; i < newData.rows.length; i++) {
                        if (newData.rows[i].id == "XQFXSH_ShenHeJieGuo") {
                            for (var j = 0; j < returnPropertygridData.length; j++) {
                                $("#pg").propertygrid('insertRow', { index: i + j + 1, row: returnPropertygridData[j] });
                            }
                            break;
                        }
                    }
                }
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
     

        onSelectShenHeJieGuo: function (record) {
            //注意：此事件为combobox的onSelect事件函数，此函数不能重新加载propertygrid数据，否则会出错
            if (record.value == SHENHEJIEGUO.PASS) {
                var newData = $("#pg").propertygrid('getData');
                for (var i = newData.rows.length - 1; i >= 0; i--) {
                    if (newData.rows[i].id == "ReturnTo" || newData.rows[i].id == "IsBackTracking") {
                        $("#pg").propertygrid('deleteRow',i);
                    }
                }
            } else {
                var newData = $("#pg").propertygrid('getData');
                for (var i = 0; i < newData.rows.length; i++) {
                    if (newData.rows[i].id == "XQFXSH_ShenHeJieGuo") {
                        for (var j = 0; j < returnPropertygridData.length; j++) {
                            $("#pg").propertygrid('insertRow', { index: i + j + 1, row: returnPropertygridData[j] });
                        }
                        break;
                    }
                }
            }
        },

        onSelectReturnToNodeCombobox: function(rec) {
            var recName = rec.text.substring(0, rec.text.indexOf("="));
            var name = rec.text.substring(rec.text.indexOf(">") + 1);

            var newData = $("#pg").propertygrid('getData');
            for (var i = 0; i < newData.rows.length; i++) {
                if (newData.rows[i].id == "XQFXSH_ShenHeYiJian") {
                    newData.rows[i].value = BS_.sheZheTuiHuiLiYou(recName, name);
                    $("#pg").propertygrid('refreshRow',i);
                    break;
                }
            }
        },

        sheZheTuiHuiLiYou:function (recName, name) {
            return recName + "同志: \n  您处理的“" + name + "”工作有错误，需要您重新办理．\n\n此致!!!   \n\n  "
                + new Date().Format("yyyy-MM-dd hh:mm:ss");
        },





        baoCun: function () {
            BS_.propertygridEndEdit();
           
            if (XQFXShenHeOriginResult == undefined) {
                //新增
                BS_.add();
            }
            else {
                //更新
                BS_.update();
            }
        },
        
        add: function () {
            var newRow = BS_.getNewRowFromPropertygrid();

            AT_.AjaxPost("/ShenHe/TianJiaShenHe", newRow, BS_.addSuccess);
        },

        addSuccess: function (data, status) {
            $.messager.show({
                title: '添加审核结果',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });

            if (data.state == "0") {
                BS_.updateXQFXShenHeOriginResult();
                saveResult = true;
            } else {
                saveResult = false;
            }
        },
        
        update: function () {
            if (BS_.isChange() == true) {
                var newRow = BS_.getNewRowFromPropertygrid();
                newRow["oldOID"] = XQFXShenHeOriginResult.rows[0].OID;
                
                AT_.AjaxPost("/ShenHe/XiuGaiShenHe", newRow, BS_.updateSuccess);
            } else {
                saveResult = true;
            }
        },

        isChange: function () {
            var editData = $("#pg").propertygrid('getData');

            //与初始化项目数据进行比较
            for (var i = 0; i < editData.rows.length; i++) {
                var id = editData.rows[i].id;
                if (id != undefined && BS_.ifColumnNeedCompare(id) == true) {
                    var prefix = "XQFXSH_";
                    var subid = id.substring(prefix.length);
                    if (XQFXShenHeOriginResult.rows[0][subid] != editData.rows[i].value) {
                        return true;
                    }
                }
            }
            return false;
        },

        ifColumnNeedCompare: function (columnName) {
            if (columnName == "XQFXSH_ShenHeShiJian" ||
                columnName == "XQFXSH_ShenHeJieGuo" ||
                columnName == "XQFXSH_ShenHeYiJian" )
                return true;
            else
                return false;
        },

        updateSuccess: function (data, status) {
            $.messager.show({
                title: '更新审核结果',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });

            if (data.state == "0") {
                BS_.updateXQFXShenHeOriginResult();
                saveResult = true;
            } else {
                saveResult = false;
            }
        },

        getNewRowFromPropertygrid: function () {
            var newRow = {};
            newRow["ShenHeRen"] = currentLoginUser;
            newRow["ModifyTime"] = (new Date()).Format("dd/MM/yyyy hh:mm:ss");
            newRow["ShenHeShiJian"] = BS_.getValueFromPropertygridById("XQFXSH_ShenHeShiJian");
            newRow["ShenHeJieGuo"] = BS_.getValueFromPropertygridById("XQFXSH_ShenHeJieGuo");
            newRow["ShenHeYiJian"] = BS_.getValueFromPropertygridById("XQFXSH_ShenHeYiJian");
            newRow["FK_Flow"] = FK_Flow;
            newRow["FK_Node"] = FK_Node;
            newRow["WorkID"] = WorkID;
            newRow["StepType"] = StepType.SUBJECT_XUQIUFENXI;
            return newRow;
        },

        updateXQFXShenHeOriginResult: function () {
            var argsVal = {
                "FK_Flow": FK_Flow,
                "WorkID": WorkID,
                "FK_Node": FK_Node,
                "shenHeRen": currentLoginUser
            }

            $("#pg").propertygrid("loading");
            AT_.AjaxPost("/ShenHe/GetShenHeHistoryData", argsVal, BS_.updateXQFXShenHeOriginResultSuccess);
        },

        updateXQFXShenHeOriginResultSuccess: function (data, status) {
            $("#pg").propertygrid("loaded");

            if (data.state == "0") {
                XQFXShenHeOriginResult = parseJSON(data._Json);
            }
        },

        faSong: function () {
            BS_.baoCun();
            if (saveResult == true) {
                saveResult = false;
                
                var data = $("#pg").propertygrid('getData');
                var tempValue = undefined;
                var tempText = undefined;
                for (var i = 0; i < data.rows.length; i++) {
                    if (data.rows[i].id == "XQFXSH_ShenHeJieGuo") {
                        //退回
                        if (data.rows[i].value == SHENHEJIEGUO.RETURN) {
                            for (var j = 0; j < data.rows.length; j++) {
                                if (data.rows[j].id == "ReturnTo") {
                                    tempValue = data.rows[j].value;
                                    if (data.rows[j].value == undefined) {
                                        $.messager.show({
                                            title: '信息提示',
                                            msg: "退回节点不能为空",
                                            timeout: 3000,
                                            showType: 'show'
                                        });
                                        return;
                                    }
                                }
                            }

                            for (var j = 0; j < ReturnToNodeData.length; j++) {
                                if (ReturnToNodeData[j].value == tempValue) {
                                    tempText = ReturnToNodeData[j].text;
                                }
                            }

                            $.messager.confirm("退回", "请确定您是否退回到节点：<br><br>【"
                                + tempText + "】", function (data) {
                                    if (!data) return;
                                    else {
                                        BS_.tuiHui();
                                    }
                                });
                        }
                        //正常发送
                        else {
                            window.location.href = '/XuQiuFenXi/XuQiuFenXiShenHeFaQi?FK_Flow=' + FK_Flow + '&WorkID=' + WorkID;
                        }
                    }
                }
            }
        },

        tuiHui: function () {
            var newRow = BS_.getReturnDataFromPropertygrid();
            AT_.AjaxPost("/XuQiuFenXi/TuiHui", newRow, BS_.tuiHuiSuccess);
        },

        tuiHuiSuccess: function (data, status) {
            $.messager.show({
                title: '退回结果',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
        },

        getReturnDataFromPropertygrid: function () {
            var newRow = {};
            newRow["ReturnNodeInfo"] = BS_.getValueFromPropertygridById("ReturnTo");
            newRow["IsBackTracking"] = BS_.getValueFromPropertygridById("IsBackTracking");
            newRow["TuiHuiLiYou"] = BS_.getValueFromPropertygridById("XQFXSH_ShenHeYiJian");
            newRow["FK_Flow"] =  FK_Flow;
            newRow["WorkID"] =  WorkID;
            newRow["FK_Node"] = FK_Node;
            newRow["FID"] = FID;
            return newRow;
        },

        getValueFromPropertygridById: function (id) {
            var data = $("#pg").propertygrid('getData');
            for (var i = 0; i < data.rows.length; i++) {
                if (data.rows[i].id == id) {
                    return data.rows[i].value;
                }
            }
            return "";
        },


         

        

 
       
     
        

       





        liuChengTu: function () {
            window.open('/XuQiuFenXi/ChaKanLiuChengTu?FK_Flow=' + FK_Flow);
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

        onLoadProjectHistoryDataSuccess: function(data, status){
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
                $("#pg").propertygrid("expandGroup", 2);
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
                $("#pg").propertygrid("expandGroup", 1);
            }
        }

    }