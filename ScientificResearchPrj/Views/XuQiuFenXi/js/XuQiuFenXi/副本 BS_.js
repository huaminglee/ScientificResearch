var editXMOID = undefined;
var saveProjectResult = undefined;
var saveSubjectResult = undefined;

var BS_ = {
        clearArgs: function () {
            XiangMuPropertygridResult = undefined;
            KeTiPropertygridResult = undefined;
        },
        onLoad: function () {
            var argsVal = {
                "FK_Flow": FK_Flow,
                "WorkID": WorkID,
                "FK_Node": FK_Node
            }

            $("#pg").propertygrid("loading");
            AT_.AjaxPost("/XuQiuFenXi/GetHistoryData", argsVal, BS_.onLoadSuccess);

            BS_.clearArgs();
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
                KeTiOriginResult = parseJSON(data._Json);

                BS_.getPropertygridDataFormJsonResult(KeTiOriginResult, "KT_");
                 
                var prjVal = { priOID: KeTiOriginResult.rows[0].FK_XmOID }
                AT_.AjaxPost("/XiangMu/GetXiangMuByOID", prjVal, BS_.onLoadOriginProjectByOIDSuccess);
            }
        },

        onLoadOriginProjectByOIDSuccess: function (data, status) {
            XiangMuOriginResult = parseJSON(data._Json);

            BS_.getPropertygridDataFormJsonResult(XiangMuOriginResult, "XM_");

            if (data.state == "0") {
                //设置拟定时间为不可编辑
                var pData = $("#pg").propertygrid('getData');
                for (var i = 0; i < pData.rows.length; i++) {
                    var id = pData.rows[i].id;
                    if (id == "XM_ProposeTime" || id == "KT_ProposeTime") {
                        pData.rows[i].editor = "";
                    }
                }
            }
        },

        getPropertygridDataFormJsonResult: function (result, prefix) {
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

        clearPropertygridData: function (prefix) {
            var newData = $("#pg").propertygrid('getData');
            for (var i = 0; i < newData.rows.length; i++) {
                var id = newData.rows[i].id;
                if (id != undefined && id.indexOf(prefix) != -1) {
                    id = id.substring(prefix.length);
                    newData.rows[i].value = "";
                }
            }
            $('#pg').propertygrid({ data: newData });
        },







        openProjectDialog: function () {
            $('#divSelectProject').dialog('open');

            BS_.loadProjects();
        },

        loadProjects: function () {
            var pageopt = $('#tTableProject').datagrid('getPager').data("pagination").options;
            var pageVal = {
                "pageNow": pageopt.pageNumber == 0 ? 1 : pageopt.pageNumber,
                "pageSize": pageopt.pageSize,
            }

            $('#tTableProject').datagrid("loading");
            AT_.AjaxPost("/XiangMu/GetXiangMu", pageVal, BS_.onLoadProjectsSuccess);
        },

        onLoadProjectsSuccess: function (data, status) {
            $.messager.show({
                title: '加载项目列表',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            $('#tTableProject').datagrid("loaded");

            if (data.state == "0") {
                var projectsJsonResult = parseJSON(data._Json);
                $('#tTableProject').datagrid({ data: projectsJsonResult });

                BS_.loadProjectsSuccessSelectProject(projectsJsonResult);

                //加载成功后必须重新设置分页数据，因为分页的正确显示是在数据加载成功后设置才有效的
                $('#tTableProject').datagrid("getPager").pagination({
                    pageSize: data.pageSize,
                    pageNumber: data.pageNumber,
                    total: data.totalCount,
                    onSelectPage: BS_.onSelectProjectPage
                });
            }
        },

        loadProjectsSuccessSelectProject: function (projectsJsonResult) {
            if (XiangMuOriginResult == undefined) return;
            var editProjectOID = XiangMuOriginResult.rows[0].OID;
            if (editProjectOID == undefined) { return; }
            var rows = projectsJsonResult.rows;

            if (rows.length != 0) {
                for (var i = 0; i < rows.length; i++) {
                    if (rows[i].OID == editProjectOID) {
                        $('#tTableProject').datagrid('selectRow', i);
                        return;
                    }
                }
            }
        },

        onSelectProjectPage: function (pageNumber, pageSize) {
            $('#tTableProject').datagrid("loading");
            BS_.loadProjects();
        },

        selectProjectOnClickRow: function (index) {
            $('#tTableProject').datagrid('selectRow', index);
        },

        selectProject: function () {
            //每次open dialog显示，虽然没显示选中状态，但是实验证明，会保存上次的选中清空，
            //所以每次关闭或者保存后需要unselectall或者清空
            var selectedNode = $('#tTableProject').datagrid('getSelected');
            if (selectedNode == null) return;
            
            $.messager.confirm("选择项目", "您确定覆盖当前项目并清空当前课题数据吗？", function (data) {
                if (data) {
                    XiangMuPropertygridResult = { total: "1", rows: [cloneJSON(selectedNode)] };
                    BS_.getPropertygridDataFormJsonResult(XiangMuPropertygridResult, "XM_");

                    BS_.clearPropertygridData("KT_");

                    $.messager.show({
                        title: '信息提示',
                        msg: "数据发生了变化",
                        timeout: 3000,
                        showType: 'show'
                    });

                    $('#divSelectProject').dialog('close');
                    //刷新
                    $('#tTableProject').datagrid('unselectAll');
                }
            });
        },


        unSelectProject: function () {
            $('#tTableProject').datagrid('unselectAll');
        },

        cancelSelectProject: function () {
            //刷新
            $('#tTableProject').datagrid('unselectAll');

            $('#divSelectProject').dialog('close');
        },








        openSunjectDialog: function () {
            $('#divSelectSubject').dialog('open');

            BS_.loadSubjects();
        },

        loadSubjects: function () {
            var pageopt = $('#tTableSubject').datagrid('getPager').data("pagination").options;
            var pageVal = {
                "pageNow": pageopt.pageNumber == 0 ? 1 : pageopt.pageNumber,
                "pageSize": pageopt.pageSize,
            }

            $('#tTableSubject').datagrid("loading");
            AT_.AjaxPost("/KeTi/GetKeTi", pageVal, BS_.onLoadSubjectsSuccess);
        },

        onLoadSubjectsSuccess: function (data, status) {
            $.messager.show({
                title: '加载课题列表',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            $('#tTableSubject').datagrid("loaded");

            if (data.state == "0") {
                var subjectsJsonResult = parseJSON(data._Json);
                $('#tTableSubject').datagrid({ data: subjectsJsonResult });

                BS_.loadSubjectsSuccessSelectSubject(subjectsJsonResult);

                //加载成功后必须重新设置分页数据，因为分页的正确显示是在数据加载成功后设置才有效的
                $('#tTableSubject').datagrid("getPager").pagination({
                    pageSize: data.pageSize,
                    pageNumber: data.pageNumber,
                    total: data.totalCount,
                    onSelectPage: BS_.onSelectSubjectPage
                });
            }
        },

        loadSubjectsSuccessSelectSubject: function (subjectsJsonResult) {
            if (KeTiOriginResult == undefined) return;
            var editSubjectOID = KeTiOriginResult.rows[0].OID;
            if (editSubjectOID == undefined) { return; }
            var rows = subjectsJsonResult.rows;

            if (rows.length != 0) {
                for (var i = 0; i < rows.length; i++) {
                    if (rows[i].OID == editSubjectOID) {
                        $('#tTableSubject').datagrid('selectRow', i);
                        return;
                    }
                }
            }
        },

        onSelectSubjectPage: function (pageNumber, pageSize) {
            $('#tTableSubject').datagrid("loading");
            BS_.loadSubjects();
        },

        selectSubjectOnClickRow: function (index) {
            $('#tTableSubject').datagrid('selectRow', index);
        },

        selectSubject: function () {
            //每次open dialog显示，虽然没显示选中状态，但是实验证明，会保存上次的选中清空，
            //所以每次关闭或者保存后需要unselectall或者清空
            var selectedNode = $('#tTableSubject').datagrid('getSelected');
            if (selectedNode == null) return;

            $.messager.confirm("选择课题", "您确定覆盖当前的项目和课题数据吗？", function (data) {
                if (data) {
                    KeTiPropertygridResult = { total: "1", rows: [cloneJSON(selectedNode)] };

                    BS_.getPropertygridDataFormJsonResult(KeTiPropertygridResult, "KT_");

                    var prjVal = { priOID: selectedNode.FK_XmOID }
                    AT_.AjaxPost("/XiangMu/GetXiangMuByOID", prjVal, BS_.onLoadProjectByOIDSuccess);
                }
            });
        },
        
        onLoadProjectByOIDSuccess: function (data, status) {
            XiangMuPropertygridResult = cloneJSON(parseJSON(data._Json));
            BS_.getPropertygridDataFormJsonResult(XiangMuPropertygridResult, "XM_");
            
            $('#divSelectSubject').dialog('close');
            //刷新
            $('#tTableSubject').datagrid('unselectAll');
        },

        unSelectSubject: function () {
            $('#tTableSubject').datagrid('unselectAll');
        },

        cancelSelectSubject: function () {
            //刷新
            $('#tTableSubject').datagrid('unselectAll');

            $('#divSelectSubject').dialog('close');
        },







        baoCun: function () {
            var row = $("#pg").propertygrid("getSelected");
            var editIndex = $("#pg").propertygrid('getRowIndex', row);
            $("#pg").propertygrid('endEdit', editIndex);
            
            var data = $("#pg").propertygrid('getData');
            for (var i = 0; i < data.rows.length; i++) {
                if (data.rows[i].id == "XM_No") {
                    if (data.rows[i].value == "") {
                        $.messager.show({
                            title: '信息提示',
                            msg: "项目编号不能为空",
                            timeout: 3000,
                            showType: 'show'
                        });
                        return;
                    }
                    else if (XiangMuPropertygridResult != undefined &&
                        data.rows[i].value == XiangMuPropertygridResult.rows[0].No) {
                        if (XiangMuOriginResult == undefined ||
                            (XiangMuOriginResult != undefined && data.rows[i].value != XiangMuOriginResult.rows[0].No)) {
                            $.messager.show({
                                title: '信息提示',
                                msg: "重新选择的项目，请修改项目编号，防止重复",
                                timeout: 3000,
                                showType: 'show'
                            });
                            return;
                        }
                    }
                }
                if (data.rows[i].id == "KT_No") {
                    if (data.rows[i].value == "") {
                        $.messager.show({
                            title: '信息提示',
                            msg: "课题编号不能为空",
                            timeout: 3000,
                            showType: 'show'
                        });
                        return;
                    }
                    else if (KeTiPropertygridResult != undefined &&
                        data.rows[i].value == KeTiPropertygridResult.rows[0].No) {
                        if (KeTiOriginResult == undefined ||
                            (KeTiOriginResult != undefined && data.rows[i].value != KeTiOriginResult.rows[0].No)) {
                            $.messager.show({
                                title: '信息提示',
                                msg: "重新选择的课题，请修改项目编号，防止重复",
                                timeout: 3000,
                                showType: 'show'
                            });
                            return;
                        }
                    }
                }
            }
            //无初始化项目数据
            if (XiangMuOriginResult == undefined) {
                //新增项目
                BS_.addProject();
            }
            //有初始化项目数据
            else {
                //更新项目
                BS_.updateProject();
            }
        },
        
        addOrUpdateSubject: function () {
            //无初始化课题数据
            if (KeTiOriginResult == undefined) {
                //新增课题
                BS_.addSubject();
            }
            else {
                //更新课题
                BS_.updateSubject();
            }
        },

        addProject: function () {
            var projectNewRow = BS_.getNewProjectRowFromPropertygrid();

            AT_.AjaxPost("/XiangMu/TianJiaXiangMu", projectNewRow, BS_.addProjectSuccess);
        },

        updateProject: function () {
            if (BS_.isProjectChange() == true) {
                var projectNewRow = BS_.getNewProjectRowFromPropertygrid();

                if (XiangMuOriginResult == undefined) return;
                projectNewRow["oldNo"] = XiangMuOriginResult.rows[0].No;
                 
                AT_.AjaxPost("/XiangMu/XiuGaiXiangMu", projectNewRow, BS_.updateProjectSuccess);
            }
            else {
                saveProjectResult = true;
                BS_.addOrUpdateSubject();
            }
        },
        
        getNewProjectRowFromPropertygrid: function () {
            var newRow = {};
            newRow["No"] = BS_.getValueFromPropertygridById("XM_No");
            newRow["Name"] = BS_.getValueFromPropertygridById("XM_Name");
            newRow["FK_Proposer"] = currentLoginUser;
            newRow["ProposeTime"] = BS_.getValueFromPropertygridById("XM_ProposeTime");
            newRow["ModifyTime"] = (new Date()).Format("dd/MM/yyyy hh:mm:ss");
            newRow["Keys"] = BS_.getValueFromPropertygridById("XM_Keys");
            newRow["FK_Xmz"] = BS_.getValueFromPropertygridById("XM_FK_Xmz");
            newRow["Columns"] = BS_.getValueFromPropertygridById("XM_Columns");
            newRow["Description"] = BS_.getValueFromPropertygridById("XM_Description");
            newRow["Tasks"] = BS_.getValueFromPropertygridById("XM_Tasks");
            newRow["Questions"] = BS_.getValueFromPropertygridById("XM_Questions");
            newRow["Remarks"] = BS_.getValueFromPropertygridById("XM_Remarks");

            newRow["FK_Flow"] = FK_Flow;
            newRow["WorkID"] = WorkID;
            newRow["FK_Node"] = FK_Node;
            return newRow;
        },

        addProjectSuccess: function (data, status) {
            $.messager.show({
                title: '添加项目',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });

            if (data.state == "0") {
                BS_.updateXiangMuOriginResultFromPropertygridData("XM_");
                XiangMuPropertygridResult = undefined;
                saveProjectResult = true;

                BS_.addOrUpdateSubject();
            } else {
                saveProjectResult = false;
            }
        },

        updateProjectSuccess: function (data, status) {
            $.messager.show({
                title: '更新项目',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });

            if (data.state == "0") {
                BS_.updateXiangMuOriginResultFromPropertygridData("XM_");
                XiangMuPropertygridResult = undefined;
                saveProjectResult = true;

                BS_.addOrUpdateSubject();
            } else {
                saveProjectResult = false;
            }
        },

        isProjectChange: function () {
            var editData = $("#pg").propertygrid('getData');

            //与初始化项目数据进行比较
            for (var i = 0; i < editData.rows.length; i++) {
                var id = editData.rows[i].id;
                if (id != undefined && BS_.ifProjectColumnNeedCompare(id) == true) {
                    var prefix = "XM_";
                    var subid = id.substring(prefix.length);
                    if (XiangMuOriginResult.rows[0][subid] != editData.rows[i].value) {
                        return true;
                    }
                }
            }
            return false;
        },

        ifProjectColumnNeedCompare: function (columnName) {
            if (columnName == "XM_No" ||
                columnName == "XM_Name" ||
                columnName == "XM_ProposeTime" ||
                columnName == "XM_Keys" ||
                columnName == "XM_Description" ||
                columnName == "XM_FK_Xmz" ||
                columnName == "XM_Columns" ||
                columnName == "XM_Tasks" ||
                columnName == "XM_Questions" ||
                columnName == "XM_Remarks")
                return true;
            else
                return false;
        },




        addSubject: function () {
            var subjectNewRow = BS_.getNewSubjectRowFromPropertygrid();

            AT_.AjaxPost("/KeTi/TianJiaKeTi", subjectNewRow, BS_.addSubjectSuccess);
        },

        updateSubject: function () {
            if (BS_.isSubjectChange() == true) {
                var subjectNewRow = BS_.getNewSubjectRowFromPropertygrid();

                if (KeTiOriginResult == undefined) return;
                subjectNewRow["oldNo"] = KeTiOriginResult.rows[0].No;

                AT_.AjaxPost("/KeTi/XiuGaiKeTi", subjectNewRow, BS_.updateSubjectSuccess);
            }
            else {
                saveSubjectResult = true;
            }
        },

        getNewSubjectRowFromPropertygrid: function () {
            var prjVal = { priNo: BS_.getValueFromPropertygridById("XM_No") }
            AT_.AjaxPost("/XiangMu/GetXiangMuByNo", prjVal, BS_.getNewSubjectOnLoadProjectSuccess);

            var newRow = {};
            newRow["No"] = BS_.getValueFromPropertygridById("KT_No");
            newRow["Name"] = BS_.getValueFromPropertygridById("KT_Name");
            newRow["FK_Proposer"] = currentLoginUser;
            newRow["ProposeTime"] = BS_.getValueFromPropertygridById("KT_ProposeTime");
            newRow["ModifyTime"] = (new Date()).Format("dd/MM/yyyy hh:mm:ss");
            newRow["Keys"] = BS_.getValueFromPropertygridById("KT_Keys");
            newRow["Description"] = BS_.getValueFromPropertygridById("KT_Description");
            newRow["SourceDesc"] = BS_.getValueFromPropertygridById("KT_SourceDesc");
            newRow["AnalysisResult"] = BS_.getValueFromPropertygridById("KT_AnalysisResult");
            newRow["TargetTask"] = BS_.getValueFromPropertygridById("KT_TargetTask");
            newRow["Innovation"] = BS_.getValueFromPropertygridById("KT_Innovation");
            newRow["Remarks"] = BS_.getValueFromPropertygridById("KT_Remarks");
            newRow["FK_XmOID"] = editXMOID;
             
            newRow["FK_Flow"] = FK_Flow;
            newRow["WorkID"] = WorkID;
            newRow["FK_Node"] = FK_Node;
            return newRow;
        },

        getNewSubjectOnLoadProjectSuccess:function(data, status) {
            if (data.state == "0") {
                var result = parseJSON(data._Json);
                editXMOID = result.rows[0].OID;
            }
        },
    
        addSubjectSuccess: function (data, status) {
            $.messager.show({
                title: '添加课题',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });

            if (data.state == "0") {
                BS_.updateKeTiOriginResultFromPropertygridData("KT_");
                saveSubjectResult = true;

                KeTiPropertygridResult = undefined;
            } else {
                saveSubjectResult = false;
            }
        },

        updateSubjectSuccess: function (data, status) {
            $.messager.show({
                title: '更新课题',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });

            if (data.state == "0") {
                BS_.updateKeTiOriginResultFromPropertygridData("KT_");
                saveSubjectResult = true;

                KeTiPropertygridResult = undefined;
            } else {
                saveSubjectResult = false;
            }
        },

        isSubjectChange: function () {
            var editData = $("#pg").propertygrid('getData');
             
            //与初始化课题数据进行比较
            for (var i = 0; i < editData.rows.length; i++) {
                var id = editData.rows[i].id;
                if (id != undefined && BS_.ifSubjectColumnNeedCompare(id) == true) {
                    var prefix = "KT_";
                    var subid = id.substring(prefix.length);
                    if (KeTiOriginResult.rows[0][subid] != editData.rows[i].value) {
                        return true;
                    }
                }
            }
            return false;
        },

        ifSubjectColumnNeedCompare: function (columnName) {
            if (columnName == "KT_No" ||
                columnName == "KT_Name" ||
                columnName == "KT_ProposeTime" ||
                columnName == "KT_Keys" ||
                columnName == "KT_Description" ||
                columnName == "KT_Remarks" ||
                columnName == "KT_SourceDesc" ||
                columnName == "KT_AnalysisResult" ||
                columnName == "KT_TargetTask" ||
                columnName == "KT_Innovation"
                )
                return true;
            else
                return false;
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

        updateXiangMuOriginResultFromPropertygridData: function (prefix) {
            if (XiangMuOriginResult == undefined) XiangMuOriginResult = { total: "1", rows: [{}] };//注意格式

            var newData = $("#pg").propertygrid('getData');
            for (var i = 0; i < newData.rows.length; i++) {
                var id = newData.rows[i].id;
                if (id != undefined && id.indexOf(prefix) != -1) {
                    id = id.substring(prefix.length);
                    XiangMuOriginResult.rows[0][id] = newData.rows[i].value;
                }
            }
        },

        updateKeTiOriginResultFromPropertygridData: function (prefix) {
            if (KeTiOriginResult == undefined) KeTiOriginResult = { total: "1", rows: [{}] };//注意格式

            var newData = $("#pg").propertygrid('getData');
            for (var i = 0; i < newData.rows.length; i++) {
                var id = newData.rows[i].id;
                if (id != undefined && id.indexOf(prefix) != -1) {
                    id = id.substring(prefix.length);
                    KeTiOriginResult.rows[0][id] = newData.rows[i].value;
                }
            }
        },






        liuChengTu: function () {
            window.open('/XuQiuFenXi/ChaKanLiuChengTu?FK_Flow=' + FK_Flow);
        },

        faSong: function () {
            BS_.baoCun();

            if (saveProjectResult == true && saveSubjectResult == true) {
                saveProjectResult = false;
                saveSubjectResult = false;
                window.location.href = '/XuQiuFenXi/XuQiuFenXiFaQi?FK_Flow=' + FK_Flow + '&WorkID=' + WorkID;
            }
        },

        propertygridEndEdit: function () {
            var row = $("#pg").propertygrid("getSelected");
            var editIndex = $("#pg").propertygrid('getRowIndex', row);
            if ($("#pg").propertygrid('validateRow', editIndex)) {
                $("#pg").propertygrid('endEdit', editIndex);
            }
        }





        
    }