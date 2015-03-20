var currentPrefix = "JJSLSH_";

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
                JJSLShenHeOriginResult = parseJSON(data._Json);

                _CommomOperation.getPropertygridDataFormJsonResult(JJSLShenHeOriginResult, currentPrefix);
                $("#pg").propertygrid("collapseGroup");
                $("#pg").propertygrid("expandGroup", 1);

                if (JJSLShenHeOriginResult.rows[0].ShenHeJieGuo == SHENHEJIEGUO.RETURN) {
                    var newData = $("#pg").propertygrid('getData');
                    for (var i = 0; i < newData.rows.length; i++) {
                        if (newData.rows[i].id == currentPrefix + "ShenHeJieGuo") {
                            for (var j = 0; j < returnPropertygridData.length; j++) {
                                $("#pg").propertygrid('insertRow', { index: i + j + 1, row: returnPropertygridData[j] });
                            }
                            break;
                        }
                    }
                }

                //加载链接
                var rowIndex = _CommomOperation.getRowIndexById(currentPrefix + "Link");
                loadLinks(JJSLShenHeOriginResult.rows[0].OID, IsShenHe.YES, rowIndex, CanLinkDelete.YES);
                //加载附件
                var rowIndex2 = _CommomOperation.getRowIndexById(currentPrefix + "Attach");
                loadAttachs(JJSLShenHeOriginResult.rows[0].OID, IsShenHe.YES, rowIndex2, CanAttachDelete.YES);

            } else {
                _ShenHe.sheZheTongGuoLiYou();
            }
        },
       
       baoCun: function () {
           _CommomOperation.propertygridEndEdit();
            
            if (JJSLShenHeOriginResult == undefined) {
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
                BS_.updateJJSLShenHeOriginResult();
                saveResult = true;
            } else {
                saveResult = false;
            }
        },
        
        update: function () {
            if (BS_.isChange() == true) {
                var newRow = BS_.getNewRowFromPropertygrid();
                newRow["oldOID"] = JJSLShenHeOriginResult.rows[0].OID;
                 
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
                    var prefix = currentPrefix;
                    var subid = id.substring(prefix.length);
                    if (JJSLShenHeOriginResult.rows[0][subid] != editData.rows[i].value) {
                        return true;
                    }
                }
            }
            return false;
        },

        ifColumnNeedCompare: function (columnName) {
            if (columnName == currentPrefix + "ShenHeShiJian" ||
                columnName == currentPrefix + "ShenHeJieGuo" ||
                columnName == currentPrefix + "ShenHeYiJian")
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
                BS_.updateJJSLShenHeOriginResult();
                saveResult = true;
            } else {
                saveResult = false;
            }
        },

        getNewRowFromPropertygrid: function () {
            var newRow = {};
            newRow["ShenHeRen"] = currentLoginUser;
            newRow["ModifyTime"] = (new Date()).Format("dd/MM/yyyy hh:mm:ss");
            newRow["ShenHeShiJian"] = _CommomOperation.getValueFromPropertygridById(currentPrefix + "ShenHeShiJian");
            newRow["ShenHeJieGuo"] = _CommomOperation.getValueFromPropertygridById(currentPrefix + "ShenHeJieGuo");
            newRow["ShenHeYiJian"] = _CommomOperation.getValueFromPropertygridById(currentPrefix + "ShenHeYiJian");
            newRow["FK_Flow"] = FK_Flow;
            newRow["FK_Node"] = FK_Node;
            newRow["WorkID"] = WorkID;
            newRow["StepType"] = StepType.JIEJUESILU;
            return newRow;
        },

        updateJJSLShenHeOriginResult: function () {
            var argsVal = {
                "FK_Flow": FK_Flow,
                "WorkID": WorkID,
                "FK_Node": FK_Node,
                "shenHeRen": currentLoginUser
            }

            $("#pg").propertygrid("loading");
            AT_.AjaxPost("/ShenHe/GetShenHeHistoryData", argsVal, BS_.updateJJSLShenHeOriginResultSuccess);
        },

        updateJJSLShenHeOriginResultSuccess: function (data, status) {
            $("#pg").propertygrid("loaded");

            if (data.state == "0") {
                JJSLShenHeOriginResult = parseJSON(data._Json);
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
                    if (data.rows[i].id == currentPrefix + "ShenHeJieGuo") {
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
                            window.location.href = '/JieJueSiLu/JieJueSiLuShenHeFaQi?FK_Flow=' + FK_Flow + '&WorkID=' + WorkID;
                        }
                    }
                }
            }
        },

        tuiHui: function () {
            var newRow = BS_.getReturnDataFromPropertygrid();
            AT_.AjaxPost("/JieJueSiLu/TuiHui", newRow, BS_.tuiHuiSuccess);
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
            newRow["ReturnNodeInfo"] = _CommomOperation.getValueFromPropertygridById("ReturnTo");
            newRow["IsBackTracking"] = _CommomOperation.getValueFromPropertygridById("IsBackTracking");
            newRow["TuiHuiLiYou"] = _CommomOperation.getValueFromPropertygridById(currentPrefix + "ShenHeYiJian");
            newRow["FK_Flow"] =  FK_Flow;
            newRow["WorkID"] =  WorkID;
            newRow["FK_Node"] = FK_Node;
            newRow["FID"] = FID;
            return newRow;
        },
     

        

 
       
     
        

       





        liuChengTu: function () {
            window.open('/JieJueSiLu/ChaKanLiuChengTu?FK_Flow=' + FK_Flow);
        },

        chaoSong: function () {
            var _json = {
                'FK_Flow': FK_Flow, 'FK_Node': FK_Node,
                'WorkID': WorkID, 'FID': FID
            }
            openChaoSongDialog(_json);
        },

        addLink: function () {
            BS_.baoCun();
            if (saveResult == true) {
                saveResult = false;

                var OID = JJSLShenHeOriginResult.rows[0].OID;
                openLinkDialog(OID, IsShenHe.YES);
            }
        },

        addAttach: function () {
            BS_.baoCun();
            if (saveResult == true) {
                saveResult = false;

                var OID = JJSLShenHeOriginResult.rows[0].OID;
                openAttachDialog(OID, IsShenHe.YES);
            }
        }
    }