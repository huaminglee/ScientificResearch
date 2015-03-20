//当前编辑页有多少个PropertygridData，用于计算具体展开某一项，
//审核页需要CurrentPropertygridDataCount再加1，因为多了其他人审核的项,且该项永远放在最前面
var CurrentPropertygridDataCount = undefined;


var _LoadHistory = {
    
    loadShenHeHistoryDataWithoutCurrentLoginUser: function () {
        var argsVal = {
            "FK_Flow": FK_Flow,
            "WorkID": WorkID,
            "FK_Node": FK_Node,
            "shenHeRen": currentLoginUser
        }

        $("#pg").propertygrid("loading");
        AT_.AjaxPost("/ShenHe/GetShenHeHistoryDataWithoutCurrentLoginUser", argsVal, _LoadHistory.onLoadShenHeHistoryDataWithoutCurrentLoginUserSuccess);
    },

    onLoadShenHeHistoryDataWithoutCurrentLoginUserSuccess: function (data, status) {
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
                _CommomOperation.getPropertygridDataFormJsonResult(tempResult, "NOTCURRENTLOGINUSERSH_");
            }
            else {
                _CommomOperation.getAndClonePropertygridDataFormJsonResult(tempResult, "NOTCURRENTLOGINUSERSH_");
            }
            $("#pg").propertygrid("collapseGroup");
            $("#pg").propertygrid("expandGroup", 0);//存在的话永远放在最前面

            for (var i = 0; i < tempResult.rows.length; i++) {
                //加载链接
                var rowIndex = _CommomOperation.getRowIndexByIdAndIndex("NOTCURRENTLOGINUSERSH_Link", i);
                loadLinks(tempResult.rows[i].OID, IsShenHe.YES, rowIndex, CanLinkDelete.NO);
                //加载附件
                var rowIndex2 = _CommomOperation.getRowIndexByIdAndIndex("NOTCURRENTLOGINUSERSH_Attach", i);
                loadAttachs(tempResult.rows[i].OID, IsShenHe.YES, rowIndex2, CanAttachDelete.NO);
            }

        }
    },

    loadProjectHistoryData: function () {
        var argsVal = {
            "FK_Flow": FK_Flow,
            "WorkID": WorkID,
            "stepType": StepType.PROJECT_XUQIUFENXI
        }

        $("#pg").propertygrid("loading");
        AT_.AjaxPost("/XuQiuFenXi/GetHistoryDataFromTrack", argsVal, _LoadHistory.onLoadProjectHistoryDataSuccess);
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
            _CommomOperation.getPropertygridDataFormJsonResult(tempResult, "XM_");
            $("#pg").propertygrid("collapseGroup");
            $("#pg").propertygrid("expandGroup", CurrentPropertygridDataCount - 1);
        }
    },

    loadSubjectHistoryData: function () {
        var argsVal = {
            "FK_Flow": FK_Flow,
            "WorkID": WorkID,
            "stepType": StepType.SUBJECT_XUQIUFENXI
        }

        $("#pg").propertygrid("loading");
        AT_.AjaxPost("/XuQiuFenXi/GetHistoryDataFromTrack", argsVal, _LoadHistory.onLoadSubjectHistoryDataSuccess);
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
            _CommomOperation.getPropertygridDataFormJsonResult(tempResult, "KT_");
            $("#pg").propertygrid("collapseGroup");
            $("#pg").propertygrid("expandGroup", CurrentPropertygridDataCount - 2);

            //加载链接
            var rowIndex = _CommomOperation.getRowIndexById("KT_Link");
            loadLinks(tempResult.rows[0].No, IsShenHe.NO, rowIndex, CanLinkDelete.NO);
            //加载附件
            var rowIndex2 = _CommomOperation.getRowIndexById("KT_Attach");
            loadAttachs(tempResult.rows[0].No, IsShenHe.NO, rowIndex2, CanAttachDelete.NO);

        }
    },

    loadXuQiuFenXiShenHeHistoryData: function () {
        var argsVal = {
            "FK_Flow": FK_Flow,
            "WorkID": WorkID,
            "stepType": StepType.SUBJECT_XUQIUFENXI
        }

        $("#pg").propertygrid("loading");
        AT_.AjaxPost("/ShenHe/GetShenHeHistoryDataByStep", argsVal, _LoadHistory.onLoadXuQiuFenXiShenHeHistoryDataSuccess);
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
                _CommomOperation.getPropertygridDataFormJsonResult(tempResult, "XQFXSH_");
            }
            else {
                _CommomOperation.getAndClonePropertygridDataFormJsonResult(tempResult, "XQFXSH_");
            }
            $("#pg").propertygrid("collapseGroup");
            $("#pg").propertygrid("expandGroup", CurrentPropertygridDataCount - 3);

            for (var i = 0; i < tempResult.rows.length; i++) {
                //加载链接
                var rowIndex = _CommomOperation.getRowIndexByIdAndIndex("XQFXSH_Link", i);
                loadLinks(tempResult.rows[i].OID, IsShenHe.YES, rowIndex, CanLinkDelete.NO);
                //加载附件
                var rowIndex2 = _CommomOperation.getRowIndexByIdAndIndex("XQFXSH_Attach", i);
                loadAttachs(tempResult.rows[i].OID, IsShenHe.YES, rowIndex2, CanAttachDelete.NO);
            }
             
        }
    },

    loadDiaoYanHistoryData: function () {
        var argsVal = {
            "FK_Flow": FK_Flow,
            "WorkID": WorkID,
            "stepType": StepType.DIAOYAN
        }

        $("#pg").propertygrid("loading");
        AT_.AjaxPost("/DiaoYan/GetHistoryDataFromTrack", argsVal, _LoadHistory.onLoadDiaoYanHistoryDataSuccess);
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
                _CommomOperation.getPropertygridDataFormJsonResult(tempResult, "DY_");
            }
            else {
                _CommomOperation.getAndClonePropertygridDataFormJsonResult(tempResult, "DY_");
            }
            $("#pg").propertygrid("collapseGroup");
            $("#pg").propertygrid("expandGroup", CurrentPropertygridDataCount - 4);

            for (var i = 0; i < tempResult.rows.length; i++) {
                //加载链接
                var rowIndex = _CommomOperation.getRowIndexByIdAndIndex("DY_Link", i);
                loadLinks(tempResult.rows[i].No, IsShenHe.NO, rowIndex, CanLinkDelete.NO);
                //加载附件
                var rowIndex2 = _CommomOperation.getRowIndexByIdAndIndex("DY_Attach", i);
                loadAttachs(tempResult.rows[i].No, IsShenHe.NO, rowIndex2, CanAttachDelete.NO);   
            }

        }
    },

    loadDiaoYanShenHeHistoryData: function () {
        var argsVal = {
            "FK_Flow": FK_Flow,
            "WorkID": WorkID,
            "stepType": StepType.DIAOYAN
        }

        $("#pg").propertygrid("loading");
        AT_.AjaxPost("/ShenHe/GetShenHeHistoryDataByStep", argsVal, _LoadHistory.onLoadDiaoYanShenHeHistoryDataSuccess);
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
                _CommomOperation.getPropertygridDataFormJsonResult(tempResult, "DYSH_");
            }
            else {
                _CommomOperation.getAndClonePropertygridDataFormJsonResult(tempResult, "DYSH_");
            }
            $("#pg").propertygrid("collapseGroup");
            $("#pg").propertygrid("expandGroup", CurrentPropertygridDataCount - 5);

            for (var i = 0; i < tempResult.rows.length; i++) {
                //加载链接
                var rowIndex = _CommomOperation.getRowIndexByIdAndIndex("DYSH_Link", i);
                loadLinks(tempResult.rows[i].OID, IsShenHe.YES, rowIndex, CanLinkDelete.NO);
                //加载附件
                var rowIndex2 = _CommomOperation.getRowIndexByIdAndIndex("DYSH_Attach", i);
                loadAttachs(tempResult.rows[i].OID, IsShenHe.YES, rowIndex2, CanAttachDelete.NO);
            }

        }
    },

    loadTiChuWenTiHistoryData: function () {
        var argsVal = {
            "FK_Flow": FK_Flow,
            "WorkID": WorkID,
            "stepType": StepType.TICHUWENTI
        }

        $("#pg").propertygrid("loading");
        AT_.AjaxPost("/TiChuWenTi/GetHistoryDataFromTrack", argsVal, _LoadHistory.onLoadTiChuWenTiHistoryDataSuccess);
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
                _CommomOperation.getPropertygridDataFormJsonResult(tempResult, "TCWT_");
            }
            else {
                _CommomOperation.getAndClonePropertygridDataFormJsonResult(tempResult, "TCWT_");
            }
            $("#pg").propertygrid("collapseGroup");
            $("#pg").propertygrid("expandGroup", CurrentPropertygridDataCount - 6);

            for (var i = 0; i < tempResult.rows.length; i++) {
                //加载链接
                var rowIndex = _CommomOperation.getRowIndexByIdAndIndex("TCWT_Link", i);
                loadLinks(tempResult.rows[i].No, IsShenHe.NO, rowIndex, CanLinkDelete.NO);
                //加载附件
                var rowIndex2 = _CommomOperation.getRowIndexByIdAndIndex("TCWT_Attach", i);
                loadAttachs(tempResult.rows[i].No, IsShenHe.NO, rowIndex2, CanAttachDelete.NO);
            }

        }
    },

    loadTiChuWenTiShenHeHistoryData: function () {
        var argsVal = {
            "FK_Flow": FK_Flow,
            "WorkID": WorkID,
            "stepType": StepType.TICHUWENTI
        }

        $("#pg").propertygrid("loading");
        AT_.AjaxPost("/ShenHe/GetShenHeHistoryDataByStep", argsVal, _LoadHistory.onLoadTiChuWenTiShenHeHistoryDataSuccess);
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
                _CommomOperation.getPropertygridDataFormJsonResult(tempResult, "TCWTSH_");
            }
            else {
                _CommomOperation.getAndClonePropertygridDataFormJsonResult(tempResult, "TCWTSH_");
            }
            $("#pg").propertygrid("collapseGroup");
            $("#pg").propertygrid("expandGroup", CurrentPropertygridDataCount - 7);

            for (var i = 0; i < tempResult.rows.length; i++) {
                //加载链接
                var rowIndex = _CommomOperation.getRowIndexByIdAndIndex("TCWTSH_Link", i);
                loadLinks(tempResult.rows[i].OID, IsShenHe.YES, rowIndex, CanLinkDelete.NO);
                //加载附件
                var rowIndex2 = _CommomOperation.getRowIndexByIdAndIndex("TCWTSH_Attach", i);
                loadAttachs(tempResult.rows[i].OID, IsShenHe.YES, rowIndex2, CanAttachDelete.NO);
            }

        }
    },

    loadJieJueSiLuHistoryData: function () {
        var argsVal = {
            "FK_Flow": FK_Flow,
            "WorkID": WorkID,
            "stepType": StepType.JIEJUESILU
        }

        $("#pg").propertygrid("loading");
        AT_.AjaxPost("/JieJueSiLu/GetHistoryDataFromTrack", argsVal, _LoadHistory.onLoadJieJueSiLuHistoryDataSuccess);
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
                _CommomOperation.getPropertygridDataFormJsonResult(tempResult, "JJSL_");
            }
            else {
                _CommomOperation.getAndClonePropertygridDataFormJsonResult(tempResult, "JJSL_");
            }
            $("#pg").propertygrid("collapseGroup");
            $("#pg").propertygrid("expandGroup", CurrentPropertygridDataCount - 8);

            for (var i = 0; i < tempResult.rows.length; i++) {
                //加载链接
                var rowIndex = _CommomOperation.getRowIndexByIdAndIndex("JJSL_Link", i);
                loadLinks(tempResult.rows[i].No, IsShenHe.NO, rowIndex, CanLinkDelete.NO);
                //加载附件
                var rowIndex2 = _CommomOperation.getRowIndexByIdAndIndex("JJSL_Attach", i);
                loadAttachs(tempResult.rows[i].No, IsShenHe.NO, rowIndex2, CanAttachDelete.NO);
            }

        }
    },

    loadJieJueSiLuShenHeHistoryData: function () {
        var argsVal = {
            "FK_Flow": FK_Flow,
            "WorkID": WorkID,
            "stepType": StepType.JIEJUESILU
        }

        $("#pg").propertygrid("loading");
        AT_.AjaxPost("/ShenHe/GetShenHeHistoryDataByStep", argsVal, _LoadHistory.onLoadJieJueSiLuShenHeHistoryDataSuccess);
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
                _CommomOperation.getPropertygridDataFormJsonResult(tempResult, "JJSLSH_");
            }
            else {
                _CommomOperation.getAndClonePropertygridDataFormJsonResult(tempResult, "JJSLSH_");
            }
            $("#pg").propertygrid("collapseGroup");
            $("#pg").propertygrid("expandGroup", CurrentPropertygridDataCount - 9);

            for (var i = 0; i < tempResult.rows.length; i++) {
                //加载链接
                var rowIndex = _CommomOperation.getRowIndexByIdAndIndex("JJSLSH_Link", i);
                loadLinks(tempResult.rows[i].OID, IsShenHe.YES, rowIndex, CanLinkDelete.NO);
                //加载附件
                var rowIndex2 = _CommomOperation.getRowIndexByIdAndIndex("JJSLSH_Attach", i);
                loadAttachs(tempResult.rows[i].OID, IsShenHe.YES, rowIndex2, CanAttachDelete.NO);
            }

        }
    },

    loadXingShiHuaHistoryData: function () {
        var argsVal = {
            "FK_Flow": FK_Flow,
            "WorkID": WorkID,
            "stepType": StepType.XINGSHIHUA
        }

        $("#pg").propertygrid("loading");
        AT_.AjaxPost("/XingShiHua/GetHistoryDataFromTrack", argsVal, _LoadHistory.onLoadXingShiHuaHistoryDataSuccess);
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
                _CommomOperation.getPropertygridDataFormJsonResult(tempResult, "XSH_");
            }
            else {
                _CommomOperation.getAndClonePropertygridDataFormJsonResult(tempResult, "XSH_");
            }
            $("#pg").propertygrid("collapseGroup");
            $("#pg").propertygrid("expandGroup", CurrentPropertygridDataCount - 10);

            for (var i = 0; i < tempResult.rows.length; i++) {
                //加载链接
                var rowIndex = _CommomOperation.getRowIndexByIdAndIndex("XSH_Link", i);
                loadLinks(tempResult.rows[i].No, IsShenHe.NO, rowIndex, CanLinkDelete.NO);
                //加载附件
                var rowIndex2 = _CommomOperation.getRowIndexByIdAndIndex("XSH_Attach", i);
                loadAttachs(tempResult.rows[i].No, IsShenHe.NO, rowIndex2, CanAttachDelete.NO);
            }

        }
    },

    loadXingShiHuaShenHeHistoryData: function () {
        var argsVal = {
            "FK_Flow": FK_Flow,
            "WorkID": WorkID,
            "stepType": StepType.XINGSHIHUA
        }

        $("#pg").propertygrid("loading");
        AT_.AjaxPost("/ShenHe/GetShenHeHistoryDataByStep", argsVal, _LoadHistory.onLoadXingShiHuaShenHeHistoryDataSuccess);
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
                _CommomOperation.getPropertygridDataFormJsonResult(tempResult, "XSHSH_");
            }
            else {
                _CommomOperation.getAndClonePropertygridDataFormJsonResult(tempResult, "XSHSH_");
            }
            $("#pg").propertygrid("collapseGroup");
            $("#pg").propertygrid("expandGroup", CurrentPropertygridDataCount - 11);

            for (var i = 0; i < tempResult.rows.length; i++) {
                //加载链接
                var rowIndex = _CommomOperation.getRowIndexByIdAndIndex("XSHSH_Link", i);
                loadLinks(tempResult.rows[i].OID, IsShenHe.YES, rowIndex, CanLinkDelete.NO);
                //加载附件
                var rowIndex2 = _CommomOperation.getRowIndexByIdAndIndex("XSHSH_Attach", i);
                loadAttachs(tempResult.rows[i].OID, IsShenHe.YES, rowIndex2, CanAttachDelete.NO);
            }

        }
    },

    loadSheJiSuanFaHistoryData: function () {
        var argsVal = {
            "FK_Flow": FK_Flow,
            "WorkID": WorkID,
            "stepType": StepType.SHEJISUANFA
        }

        $("#pg").propertygrid("loading");
        AT_.AjaxPost("/SheJiSuanFa/GetHistoryDataFromTrack", argsVal, _LoadHistory.onLoadSheJiSuanFaHistoryDataSuccess);
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
                _CommomOperation.getPropertygridDataFormJsonResult(tempResult, "SF_");
            }
            else {
                _CommomOperation.getAndClonePropertygridDataFormJsonResult(tempResult, "SF_");
            }
            $("#pg").propertygrid("collapseGroup");
            $("#pg").propertygrid("expandGroup", CurrentPropertygridDataCount - 12);

            for (var i = 0; i < tempResult.rows.length; i++) {
                //加载链接
                var rowIndex = _CommomOperation.getRowIndexByIdAndIndex("SF_Link", i);
                loadLinks(tempResult.rows[i].No, IsShenHe.NO, rowIndex, CanLinkDelete.NO);
                //加载附件
                var rowIndex2 = _CommomOperation.getRowIndexByIdAndIndex("SF_Attach", i);
                loadAttachs(tempResult.rows[i].No, IsShenHe.NO, rowIndex2, CanAttachDelete.NO);
            }

        }
    },

    loadSheJiSuanFaShenHeHistoryData: function () {
        var argsVal = {
            "FK_Flow": FK_Flow,
            "WorkID": WorkID,
            "stepType": StepType.SHEJISUANFA
        }

        $("#pg").propertygrid("loading");
        AT_.AjaxPost("/ShenHe/GetShenHeHistoryDataByStep", argsVal, _LoadHistory.onLoadSheJiSuanFaShenHeHistoryDataSuccess);
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
                _CommomOperation.getPropertygridDataFormJsonResult(tempResult, "SJSFSH_");
            }
            else {
                _CommomOperation.getAndClonePropertygridDataFormJsonResult(tempResult, "SJSFSH_");
            }
            $("#pg").propertygrid("collapseGroup");
            $("#pg").propertygrid("expandGroup", CurrentPropertygridDataCount - 13);

            for (var i = 0; i < tempResult.rows.length; i++) {
                //加载链接
                var rowIndex = _CommomOperation.getRowIndexByIdAndIndex("SJSFSH_Link", i);
                loadLinks(tempResult.rows[i].OID, IsShenHe.YES, rowIndex, CanLinkDelete.NO);
                //加载附件
                var rowIndex2 = _CommomOperation.getRowIndexByIdAndIndex("SJSFSH_Attach", i);
                loadAttachs(tempResult.rows[i].OID, IsShenHe.YES, rowIndex2, CanAttachDelete.NO);
            }

        }
    },

    loadSheJiShiYanHistoryData: function () {
        var argsVal = {
            "FK_Flow": FK_Flow,
            "WorkID": WorkID,
            "stepType": StepType.SHEJISHIYAN
        }

        $("#pg").propertygrid("loading");
        AT_.AjaxPost("/SheJiShiYan/GetHistoryDataFromTrack", argsVal, _LoadHistory.onLoadSheJiShiYanHistoryDataSuccess);
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
                _CommomOperation.getPropertygridDataFormJsonResult(tempResult, "SY_");
            }
            else {
                _CommomOperation.getAndClonePropertygridDataFormJsonResult(tempResult, "SY_");
            }
            $("#pg").propertygrid("collapseGroup");
            $("#pg").propertygrid("expandGroup", CurrentPropertygridDataCount - 14);

            for (var i = 0; i < tempResult.rows.length; i++) {
                //加载链接
                var rowIndex = _CommomOperation.getRowIndexByIdAndIndex("SY_Link", i);
                loadLinks(tempResult.rows[i].No, IsShenHe.NO, rowIndex, CanLinkDelete.NO);
                //加载附件
                var rowIndex2 = _CommomOperation.getRowIndexByIdAndIndex("SY_Attach", i);
                loadAttachs(tempResult.rows[i].No, IsShenHe.NO, rowIndex2, CanAttachDelete.NO);
            }

        }
    },

    loadSheJiShiYanShenHeHistoryData: function () {
        var argsVal = {
            "FK_Flow": FK_Flow,
            "WorkID": WorkID,
            "stepType": StepType.SHEJISHIYAN
        }

        $("#pg").propertygrid("loading");
        AT_.AjaxPost("/ShenHe/GetShenHeHistoryDataByStep", argsVal, _LoadHistory.onLoadSheJiShiYanShenHeHistoryDataSuccess);
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
                _CommomOperation.getPropertygridDataFormJsonResult(tempResult, "SJSYSH_");
            }
            else {
                _CommomOperation.getAndClonePropertygridDataFormJsonResult(tempResult, "SJSYSH_");
            }
            $("#pg").propertygrid("collapseGroup");
            $("#pg").propertygrid("expandGroup", CurrentPropertygridDataCount - 15);

            for (var i = 0; i < tempResult.rows.length; i++) {
                //加载链接
                var rowIndex = _CommomOperation.getRowIndexByIdAndIndex("SJSYSH_Link", i);
                loadLinks(tempResult.rows[i].OID, IsShenHe.YES, rowIndex, CanLinkDelete.NO);
                //加载附件
                var rowIndex2 = _CommomOperation.getRowIndexByIdAndIndex("SJSYSH_Attach", i);
                loadAttachs(tempResult.rows[i].OID, IsShenHe.YES, rowIndex2, CanAttachDelete.NO);
            }

        }
    },

    loadDuiBiFenXiHistoryData: function () {
        var argsVal = {
            "FK_Flow": FK_Flow,
            "WorkID": WorkID,
            "stepType": StepType.LIANGHUADUIBIFENXI
        }

        $("#pg").propertygrid("loading");
        AT_.AjaxPost("/DuiBiFenXi/GetHistoryDataFromTrack", argsVal, _LoadHistory.onLoadDuiBiFenXiHistoryDataSuccess);
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
                _CommomOperation.getPropertygridDataFormJsonResult(tempResult, "DBFX_");
            }
            else {
                _CommomOperation.getAndClonePropertygridDataFormJsonResult(tempResult, "DBFX_");
            }
            $("#pg").propertygrid("collapseGroup");
            $("#pg").propertygrid("expandGroup", CurrentPropertygridDataCount - 16);

            for (var i = 0; i < tempResult.rows.length; i++) {
                //加载链接
                var rowIndex = _CommomOperation.getRowIndexByIdAndIndex("DBFX_Link", i);
                loadLinks(tempResult.rows[i].No, IsShenHe.NO, rowIndex, CanLinkDelete.NO);
                //加载附件
                var rowIndex2 = _CommomOperation.getRowIndexByIdAndIndex("DBFX_Attach", i);
                loadAttachs(tempResult.rows[i].No, IsShenHe.NO, rowIndex2, CanAttachDelete.NO);
            }

        }
    },

    loadDuiBiFenXiShenHeHistoryData: function () {
        var argsVal = {
            "FK_Flow": FK_Flow,
            "WorkID": WorkID,
            "stepType": StepType.LIANGHUADUIBIFENXI
        }

        $("#pg").propertygrid("loading");
        AT_.AjaxPost("/ShenHe/GetShenHeHistoryDataByStep", argsVal, _LoadHistory.onLoadDuiBiFenXiShenHeHistoryDataSuccess);
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
                _CommomOperation.getPropertygridDataFormJsonResult(tempResult, "DBFXSH_");
            }
            else {
                _CommomOperation.getAndClonePropertygridDataFormJsonResult(tempResult, "DBFXSH_");
            }
            $("#pg").propertygrid("collapseGroup");
            $("#pg").propertygrid("expandGroup", CurrentPropertygridDataCount - 17);

            for (var i = 0; i < tempResult.rows.length; i++) {
                //加载链接
                var rowIndex = _CommomOperation.getRowIndexByIdAndIndex("DBFXSH_Link", i);
                loadLinks(tempResult.rows[i].OID, IsShenHe.YES, rowIndex, CanLinkDelete.NO);
                //加载附件
                var rowIndex2 = _CommomOperation.getRowIndexByIdAndIndex("DBFXSH_Attach", i);
                loadAttachs(tempResult.rows[i].OID, IsShenHe.YES, rowIndex2, CanAttachDelete.NO);
            }

        }
    },

    loadDeChuJieLunHistoryData: function () {
        var argsVal = {
            "FK_Flow": FK_Flow,
            "WorkID": WorkID,
            "stepType": StepType.DECHUJIELUN
        }

        $("#pg").propertygrid("loading");
        AT_.AjaxPost("/DeChuJieLun/GetHistoryDataFromTrack", argsVal, _LoadHistory.onLoadDeChuJieLunHistoryDataSuccess);
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
                _CommomOperation.getPropertygridDataFormJsonResult(tempResult, "DCJL_");
            }
            else {
                _CommomOperation.getAndClonePropertygridDataFormJsonResult(tempResult, "DCJL_");
            }
            $("#pg").propertygrid("collapseGroup");
            $("#pg").propertygrid("expandGroup", CurrentPropertygridDataCount - 18);

            for (var i = 0; i < tempResult.rows.length; i++) {
                //加载链接
                var rowIndex = _CommomOperation.getRowIndexByIdAndIndex("DCJL_Link", i);
                loadLinks(tempResult.rows[i].No, IsShenHe.NO, rowIndex, CanLinkDelete.NO);
                //加载附件
                var rowIndex2 = _CommomOperation.getRowIndexByIdAndIndex("DCJL_Attach", i);
                loadAttachs(tempResult.rows[i].No, IsShenHe.NO, rowIndex2, CanAttachDelete.NO);
            }

        }
    },

    loadDeChuJieLunShenHeHistoryData: function () {
        var argsVal = {
            "FK_Flow": FK_Flow,
            "WorkID": WorkID,
            "stepType": StepType.DECHUJIELUN
        }

        $("#pg").propertygrid("loading");
        AT_.AjaxPost("/ShenHe/GetShenHeHistoryDataByStep", argsVal, _LoadHistory.onLoadDeChuJieLunShenHeHistoryDataSuccess);
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
                _CommomOperation.getPropertygridDataFormJsonResult(tempResult, "DCJLSH_");
            }
            else {
                _CommomOperation.getAndClonePropertygridDataFormJsonResult(tempResult, "DCJLSH_");
            }
            $("#pg").propertygrid("collapseGroup");
            $("#pg").propertygrid("expandGroup", CurrentPropertygridDataCount - 19);

            for (var i = 0; i < tempResult.rows.length; i++) {
                //加载链接
                var rowIndex = _CommomOperation.getRowIndexByIdAndIndex("DCJLSH_Link", i);
                loadLinks(tempResult.rows[i].OID, IsShenHe.YES, rowIndex, CanLinkDelete.NO);
                //加载附件
                var rowIndex2 = _CommomOperation.getRowIndexByIdAndIndex("DCJLSH_Attach", i);
                loadAttachs(tempResult.rows[i].OID, IsShenHe.YES, rowIndex2, CanAttachDelete.NO);
            }

        }
    },

    loadLunWenZhuanXieHistoryData: function () {
        var argsVal = {
            "FK_Flow": FK_Flow,
            "WorkID": WorkID,
            "stepType": StepType.LUNWENZHUANXIE
        }

        $("#pg").propertygrid("loading");
        AT_.AjaxPost("/DeChuJieLun/GetHistoryDataFromTrack", argsVal, _LoadHistory.onLoadLunWenZhuanXieHistoryDataSuccess);
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
                _CommomOperation.getPropertygridDataFormJsonResult(tempResult, "LWZX_");
            }
            else {
                _CommomOperation.getAndClonePropertygridDataFormJsonResult(tempResult, "LWZX_");
            }
            $("#pg").propertygrid("collapseGroup");
            $("#pg").propertygrid("expandGroup", CurrentPropertygridDataCount - 20);

            for (var i = 0; i < tempResult.rows.length; i++) {
                //加载链接
                var rowIndex = _CommomOperation.getRowIndexByIdAndIndex("LWZX_Link", i);
                loadLinks(tempResult.rows[i].No, IsShenHe.NO, rowIndex, CanLinkDelete.NO);
                //加载附件
                var rowIndex2 = _CommomOperation.getRowIndexByIdAndIndex("LWZX_Attach", i);
                loadAttachs(tempResult.rows[i].No, IsShenHe.NO, rowIndex2, CanAttachDelete.NO);
            }

        }
    },

    loadLunWenZhuanXieShenHeHistoryData: function () {
        var argsVal = {
            "FK_Flow": FK_Flow,
            "WorkID": WorkID,
            "stepType": StepType.LUNWENZHUANXIE
        }

        $("#pg").propertygrid("loading");
        AT_.AjaxPost("/ShenHe/GetShenHeHistoryDataByStep", argsVal, _LoadHistory.onLoadLunWenZhuanXieShenHeHistoryDataSuccess);
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
                _CommomOperation.getPropertygridDataFormJsonResult(tempResult, "LWZXSH_");
            }
            else {
                _CommomOperation.getAndClonePropertygridDataFormJsonResult(tempResult, "LWZXSH_");
            }
            $("#pg").propertygrid("collapseGroup");
            $("#pg").propertygrid("expandGroup", CurrentPropertygridDataCount - 21);

            for (var i = 0; i < tempResult.rows.length; i++) {
                //加载链接
                var rowIndex = _CommomOperation.getRowIndexByIdAndIndex("LWZXSH_Link", i);
                loadLinks(tempResult.rows[i].OID, IsShenHe.YES, rowIndex, CanLinkDelete.NO);
                //加载附件
                var rowIndex2 = _CommomOperation.getRowIndexByIdAndIndex("LWZXSH_Attach", i);
                loadAttachs(tempResult.rows[i].OID, IsShenHe.YES, rowIndex2, CanAttachDelete.NO);
            }

        }
    }

}