
var BS_ = {

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

    faSong: function () {
        window.location.href = '/ShouYue/FaQiRenShouYueFaQi?FK_Flow=' + FK_Flow + '&WorkID=' + WorkID;
    },

    liuChengTu: function () {
        window.open('/ShouYue/ChaKanLiuChengTu?FK_Flow=' + FK_Flow);
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
            $("#pg").propertygrid("expandGroup",5);
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
            $("#pg").propertygrid("expandGroup", 1);
        }
    },
     
    loadLunWenZhuanXieShenHeHistoryData: function () {
        var argsVal = {
            "FK_Flow": FK_Flow,
            "WorkID": WorkID,
            "stepType": StepType.LUNWENZHUANXIE
        }

        $("#pg").propertygrid("loading");
        AT_.AjaxPost("/ShenHe/GetShenHeHistoryDataByStep", argsVal, BS_.onLoadLunWenZhuanXieShenHeHistoryDataSuccess);
    },

    onLoadLunWenZhuanXieShenHeHistoryDataSuccess: function (data, status) {
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
                BS_.getPropertygridDataFormJsonResult(tempResult, "LWZXSH_");
            }
            else {
                BS_.getAndClonePropertygridDataFormJsonResult(tempResult, "LWZXSH_");
            }
            $("#pg").propertygrid("collapseGroup");
            $("#pg").propertygrid("expandGroup", 0);
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
        if (prefix == "LWZXSH_") return "论文或专利撰写审核结果";
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
        if (prefix == "LWZXSH_") {
            $.extend(true, target, LWZXShenHePropertygridData);
            return target;
        }
    }
}