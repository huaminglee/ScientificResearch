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
                LWZXShenHeOriginResult = parseJSON(data._Json);

                BS_.getPropertygridDataFormJsonResult(LWZXShenHeOriginResult, "LWZXSH_");
                $("#pg").propertygrid("collapseGroup");
                $("#pg").propertygrid("expandGroup", 0);

                if (LWZXShenHeOriginResult.rows[0].ShenHeJieGuo == SHENHEJIEGUO.RETURN) {
                    var newData = $("#pg").propertygrid('getData');
                    for (var i = 0; i < newData.rows.length; i++) {
                        if (newData.rows[i].id == "LWZXSH_ShenHeJieGuo") {
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
                    if (newData.rows[i].id == "LWZXSH_ShenHeJieGuo") {
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
                if (newData.rows[i].id == "LWZXSH_ShenHeYiJian") {
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
            
            if (LWZXShenHeOriginResult == undefined) {
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
                BS_.updateLWZXShenHeOriginResult();
                saveResult = true;
            } else {
                saveResult = false;
            }
        },
        
        update: function () {
            if (BS_.isChange() == true) {
                var newRow = BS_.getNewRowFromPropertygrid();
                newRow["oldOID"] = LWZXShenHeOriginResult.rows[0].OID;
                 
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
                    var prefix = "LWZXSH_";
                    var subid = id.substring(prefix.length);
                    if (LWZXShenHeOriginResult.rows[0][subid] != editData.rows[i].value) {
                        return true;
                    }
                }
            }
            return false;
        },

        ifColumnNeedCompare: function (columnName) {
            if (columnName == "LWZXSH_ShenHeShiJian" ||
                columnName == "LWZXSH_ShenHeJieGuo" ||
                columnName == "LWZXSH_ShenHeYiJian")
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
                BS_.updateLWZXShenHeOriginResult();
                saveResult = true;
            } else {
                saveResult = false;
            }
        },

        getNewRowFromPropertygrid: function () {
            var newRow = {};
            newRow["ShenHeRen"] = currentLoginUser;
            newRow["ModifyTime"] = (new Date()).Format("dd/MM/yyyy hh:mm:ss");
            newRow["ShenHeShiJian"] = BS_.getValueFromPropertygridById("LWZXSH_ShenHeShiJian");
            newRow["ShenHeJieGuo"] = BS_.getValueFromPropertygridById("LWZXSH_ShenHeJieGuo");
            newRow["ShenHeYiJian"] = BS_.getValueFromPropertygridById("LWZXSH_ShenHeYiJian");
            newRow["FK_Flow"] = FK_Flow;
            newRow["FK_Node"] = FK_Node;
            newRow["WorkID"] = WorkID;
            newRow["StepType"] = StepType.LUNWENZHUANXIE;
            return newRow;
        },

        updateLWZXShenHeOriginResult: function () {
            var argsVal = {
                "FK_Flow": FK_Flow,
                "WorkID": WorkID,
                "FK_Node": FK_Node,
                "shenHeRen": currentLoginUser
            }

            $("#pg").propertygrid("loading");
            AT_.AjaxPost("/ShenHe/GetShenHeHistoryData", argsVal, BS_.updateLWZXShenHeOriginResultSuccess);
        },

        updateLWZXShenHeOriginResultSuccess: function (data, status) {
            $("#pg").propertygrid("loaded");

            if (data.state == "0") {
                LWZXShenHeOriginResult = parseJSON(data._Json);
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
                    if (data.rows[i].id == "LWZXSH_ShenHeJieGuo") {
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
                            window.location.href = '/LunWenZhuanXie/LunWenZhuanXieShenHeFaQi?FK_Flow=' + FK_Flow + '&WorkID=' + WorkID;
                        }
                    }
                }
            }
        },

        tuiHui: function () {
            var newRow = BS_.getReturnDataFromPropertygrid();
            AT_.AjaxPost("/LunWenZhuanXie/TuiHui", newRow, BS_.tuiHuiSuccess);
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
            newRow["TuiHuiLiYou"] = BS_.getValueFromPropertygridById("LWZXSH_ShenHeYiJian");
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
            window.open('/LunWenZhuanXie/ChaKanLiuChengTu?FK_Flow=' + FK_Flow);
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
                $("#pg").propertygrid("expandGroup", 20);
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
                $("#pg").propertygrid("expandGroup", 19);
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
                $("#pg").propertygrid("expandGroup", 18);
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
                $("#pg").propertygrid("expandGroup", 17);
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
                $("#pg").propertygrid("expandGroup", 16);
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
                $("#pg").propertygrid("expandGroup", 15);
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
                $("#pg").propertygrid("expandGroup", 14);
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
                $("#pg").propertygrid("expandGroup", 13);
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
                $("#pg").propertygrid("expandGroup", 12);
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
                $("#pg").propertygrid("expandGroup", 11);
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
                $("#pg").propertygrid("expandGroup", 10);
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
                $("#pg").propertygrid("expandGroup", 9);
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
                $("#pg").propertygrid("expandGroup", 8);
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
                $("#pg").propertygrid("expandGroup", 7);
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
                $("#pg").propertygrid("expandGroup", 6);
            }
        },

        loadDuiBiFenXiHistoryData: function () {
            var argsVal = {
                "FK_Flow": FK_Flow,
                "WorkID": WorkID,
                "stepType": StepType.LIANGHUADUIBIFENXI
            }

            $("#pg").propertygrid("loading");
            AT_.AjaxPost("/DuiBiFenXi/GetHistoryDataFromTrack", argsVal, BS_.onLoadDuiBiFenXiHistoryDataSuccess);
        },

        onLoadDuiBiFenXiHistoryDataSuccess: function (data, status) {
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
                    BS_.getPropertygridDataFormJsonResult(tempResult, "DBFX_");
                }
                else {
                    BS_.getAndClonePropertygridDataFormJsonResult(tempResult, "DBFX_");
                }
                $("#pg").propertygrid("collapseGroup");
                $("#pg").propertygrid("expandGroup", 0);
                $("#pg").propertygrid("expandGroup", 5);
            }
        },
     
        loadDuiBiFenXiShenHeHistoryData: function () {
            var argsVal = {
                "FK_Flow": FK_Flow,
                "WorkID": WorkID,
                "stepType": StepType.LIANGHUADUIBIFENXI
            }

            $("#pg").propertygrid("loading");
            AT_.AjaxPost("/ShenHe/GetShenHeHistoryDataByStep", argsVal, BS_.onLoadDuiBiFenXiShenHeHistoryDataSuccess);
        },

        onLoadDuiBiFenXiShenHeHistoryDataSuccess: function (data, status) {
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
                    BS_.getPropertygridDataFormJsonResult(tempResult, "DBFXSH_");
                }
                else {
                    BS_.getAndClonePropertygridDataFormJsonResult(tempResult, "DBFXSH_");
                }
                $("#pg").propertygrid("collapseGroup");
                $("#pg").propertygrid("expandGroup", 0);
                $("#pg").propertygrid("expandGroup", 4);
            }
        },

        loadDeChuJieLunHistoryData: function () {
            var argsVal = {
                "FK_Flow": FK_Flow,
                "WorkID": WorkID,
                "stepType": StepType.DECHUJIELUN
            }

            $("#pg").propertygrid("loading");
            AT_.AjaxPost("/DeChuJieLun/GetHistoryDataFromTrack", argsVal, BS_.onLoadDeChuJieLunHistoryDataSuccess);
        },

        onLoadDeChuJieLunHistoryDataSuccess: function (data, status) {
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
                    BS_.getPropertygridDataFormJsonResult(tempResult, "DCJL_");
                }
                else {
                    BS_.getAndClonePropertygridDataFormJsonResult(tempResult, "DCJL_");
                }
                $("#pg").propertygrid("collapseGroup");
                $("#pg").propertygrid("expandGroup", 0);
                $("#pg").propertygrid("expandGroup", 3);
            }
        },


        loadDeChuJieLunShenHeHistoryData: function () {
            var argsVal = {
                "FK_Flow": FK_Flow,
                "WorkID": WorkID,
                "stepType": StepType.DECHUJIELUN
            }

            $("#pg").propertygrid("loading");
            AT_.AjaxPost("/ShenHe/GetShenHeHistoryDataByStep", argsVal, BS_.onLoadDeChuJieLunShenHeHistoryDataSuccess);
        },

        onLoadDeChuJieLunShenHeHistoryDataSuccess: function (data, status) {
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
                    BS_.getPropertygridDataFormJsonResult(tempResult, "DCJLSH_");
                }
                else {
                    BS_.getAndClonePropertygridDataFormJsonResult(tempResult, "DCJLSH_");
                }
                $("#pg").propertygrid("collapseGroup");
                $("#pg").propertygrid("expandGroup", 0);
                $("#pg").propertygrid("expandGroup", 2);
            }
        },
     
        loadLunWenZhuanXieHistoryData: function () {
                var argsVal = {
                    "FK_Flow": FK_Flow,
                    "WorkID": WorkID,
                    "stepType": StepType.LUNWENZHUANXIE
                }

                $("#pg").propertygrid("loading");
                AT_.AjaxPost("/DeChuJieLun/GetHistoryDataFromTrack", argsVal, BS_.onLoadLunWenZhuanXieHistoryDataSuccess);
            },

        onLoadLunWenZhuanXieHistoryDataSuccess: function (data, status) {
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
                    BS_.getPropertygridDataFormJsonResult(tempResult, "LWZX_");
                }
                else {
                    BS_.getAndClonePropertygridDataFormJsonResult(tempResult, "LWZX_");
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
            if (prefix == "DBFX_") return "量化对比分析结果";
            if (prefix == "DBFXSH_") return "量化对比分析审核结果";
            if (prefix == "DCJL_") return "得出结论结果";
            if (prefix == "DCJLSH_") return "得出结论审核结果";
            if (prefix == "LWZX_") return "论文或专利撰写结果";
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
            if (prefix == "DBFXSH_") {
                $.extend(true, target, DBFXShenHePropertygridData);
                return target;
            }
            if (prefix == "DCJL_") {
                $.extend(true, target, DCJLPropertygridData);
                return target;
            }
            if (prefix == "DCJLSH_") {
                $.extend(true, target, DCJLShenHePropertygridData);
                return target;
            }
            if (prefix == "LWZX_") {
                $.extend(true, target, LWZXPropertygridData);
                return target;
            }
        }
    }