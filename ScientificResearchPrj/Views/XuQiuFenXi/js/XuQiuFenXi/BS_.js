var currentXMPrefix = "XM_";
var currentKTPrefix = "KT_";


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

                _CommomOperation.getPropertygridDataFormJsonResult(KeTiOriginResult, currentKTPrefix);
                 
                var prjVal = { priOID: KeTiOriginResult.rows[0].FK_XmOID }
                AT_.AjaxPost("/XiangMu/GetXiangMuByOID", prjVal, BS_.onLoadOriginProjectByOIDSuccess);

                //加载链接
                var rowIndex = _CommomOperation.getRowIndexById(currentKTPrefix+"Link");
                loadLinks(KeTiOriginResult.rows[0].No, IsShenHe.NO, rowIndex, CanLinkDelete.YES);

                //加载附件
                var rowIndex2 = _CommomOperation.getRowIndexById(currentKTPrefix + "Attach");
                loadAttachs(KeTiOriginResult.rows[0].No, IsShenHe.NO, rowIndex2, CanAttachDelete.YES);

            }
        },

        onLoadOriginProjectByOIDSuccess: function (data, status) {
            XiangMuOriginResult = parseJSON(data._Json);

            _CommomOperation.getPropertygridDataFormJsonResult(XiangMuOriginResult, currentXMPrefix);

            if (data.state == "0") {
                //设置拟定时间为不可编辑
                var pData = $("#pg").propertygrid('getData');
                for (var i = 0; i < pData.rows.length; i++) {
                    var id = pData.rows[i].id;
                    if (id == currentXMPrefix + "ProposeTime" || id == currentKTPrefix + "ProposeTime") {
                        pData.rows[i].editor = "";
                    }
                }
            }
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
                    _CommomOperation.getPropertygridDataFormJsonResult(XiangMuPropertygridResult, currentXMPrefix);

                    BS_.clearPropertygridData(currentKTPrefix);

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

                    _CommomOperation.getPropertygridDataFormJsonResult(KeTiPropertygridResult, currentKTPrefix);

                    var prjVal = { priOID: selectedNode.FK_XmOID }
                    AT_.AjaxPost("/XiangMu/GetXiangMuByOID", prjVal, BS_.onLoadProjectByOIDSuccess);

                    //加载链接
                    var rowIndex = _CommomOperation.getRowIndexById(currentKTPrefix + "Link");
                    loadLinks(KeTiPropertygridResult.rows[0].No, IsShenHe.NO, rowIndex, CanLinkDelete.NO);

                    //加载链接
                    var rowIndex2 = _CommomOperation.getRowIndexById(currentKTPrefix + "Attach");
                    loadAttachs(KeTiPropertygridResult.rows[0].No, IsShenHe.NO, rowIndex2, CanAttachDelete.NO);
                }
            });
        },
        
        onLoadProjectByOIDSuccess: function (data, status) {
            XiangMuPropertygridResult = cloneJSON(parseJSON(data._Json));
            _CommomOperation.getPropertygridDataFormJsonResult(XiangMuPropertygridResult, currentXMPrefix);
            
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
                if (data.rows[i].id == currentXMPrefix + "No") {
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
                if (data.rows[i].id == currentKTPrefix + "No") {
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
                                msg: "重新选择的课题，请修改课题编号，防止重复",
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
            newRow["No"] = _CommomOperation.getValueFromPropertygridById(currentXMPrefix + "No");
            newRow["Name"] = _CommomOperation.getValueFromPropertygridById(currentXMPrefix + "Name");
            newRow["FK_Proposer"] = currentLoginUser;
            newRow["ProposeTime"] = _CommomOperation.getValueFromPropertygridById(currentXMPrefix + "ProposeTime");
            newRow["ModifyTime"] = (new Date()).Format("dd/MM/yyyy hh:mm:ss");
            newRow["Keys"] = _CommomOperation.getValueFromPropertygridById(currentXMPrefix + "Keys");
            newRow["FK_Xmz"] = _CommomOperation.getValueFromPropertygridById(currentXMPrefix + "FK_Xmz");
            newRow["Columns"] = _CommomOperation.getValueFromPropertygridById(currentXMPrefix + "Columns");
            newRow["Description"] = _CommomOperation.getValueFromPropertygridById(currentXMPrefix + "Description");
            newRow["Tasks"] = _CommomOperation.getValueFromPropertygridById(currentXMPrefix + "Tasks");
            newRow["Questions"] = _CommomOperation.getValueFromPropertygridById(currentXMPrefix + "Questions");
            newRow["Remarks"] = _CommomOperation.getValueFromPropertygridById(currentXMPrefix + "Remarks");

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
                BS_.updateXiangMuOriginResultFromPropertygridData(currentXMPrefix);
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
                BS_.updateXiangMuOriginResultFromPropertygridData(currentXMPrefix);
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
                    var prefix = currentXMPrefix;
                    var subid = id.substring(prefix.length);
                    if (XiangMuOriginResult.rows[0][subid] != editData.rows[i].value) {
                        return true;
                    }
                }
            }
            return false;
        },

        ifProjectColumnNeedCompare: function (columnName) {
            if (columnName == currentXMPrefix + "No" ||
                columnName == currentXMPrefix + "Name" ||
                columnName == currentXMPrefix + "ProposeTime" ||
                columnName == currentXMPrefix + "Keys" ||
                columnName == currentXMPrefix + "Description" ||
                columnName == currentXMPrefix + "FK_Xmz" ||
                columnName == currentXMPrefix + "Columns" ||
                columnName == currentXMPrefix + "Tasks" ||
                columnName == currentXMPrefix + "Questions" ||
                columnName == currentXMPrefix + "Remarks")
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
            var prjVal = { priNo: _CommomOperation.getValueFromPropertygridById(currentXMPrefix + "No") }
            AT_.AjaxPost("/XiangMu/GetXiangMuByNo", prjVal, BS_.getNewSubjectOnLoadProjectSuccess);

            var newRow = {};
            newRow["No"] = _CommomOperation.getValueFromPropertygridById(currentKTPrefix + "No");
            newRow["Name"] = _CommomOperation.getValueFromPropertygridById(currentKTPrefix + "Name");
            newRow["FK_Proposer"] = currentLoginUser;
            newRow["ProposeTime"] = _CommomOperation.getValueFromPropertygridById(currentKTPrefix + "ProposeTime");
            newRow["ModifyTime"] = (new Date()).Format("dd/MM/yyyy hh:mm:ss");
            newRow["Keys"] = _CommomOperation.getValueFromPropertygridById(currentKTPrefix + "Keys");
            newRow["Description"] = _CommomOperation.getValueFromPropertygridById(currentKTPrefix + "Description");
            newRow["SourceDesc"] = _CommomOperation.getValueFromPropertygridById(currentKTPrefix + "SourceDesc");
            newRow["AnalysisResult"] = _CommomOperation.getValueFromPropertygridById(currentKTPrefix + "AnalysisResult");
            newRow["TargetTask"] = _CommomOperation.getValueFromPropertygridById(currentKTPrefix + "TargetTask");
            newRow["Innovation"] = _CommomOperation.getValueFromPropertygridById(currentKTPrefix + "Innovation");
            newRow["Remarks"] = _CommomOperation.getValueFromPropertygridById(currentKTPrefix + "Remarks");
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
                BS_.updateKeTiOriginResultFromPropertygridData(currentKTPrefix);
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
                BS_.updateKeTiOriginResultFromPropertygridData(currentKTPrefix);
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
                    var prefix = currentKTPrefix;
                    var subid = id.substring(prefix.length);
                    if (KeTiOriginResult.rows[0][subid] != editData.rows[i].value) {
                        return true;
                    }
                }
            }
            return false;
        },

        ifSubjectColumnNeedCompare: function (columnName) {
            if (columnName == currentKTPrefix + "No" ||
                columnName == currentKTPrefix + "Name" ||
                columnName == currentKTPrefix + "ProposeTime" ||
                columnName == currentKTPrefix + "Keys" ||
                columnName == currentKTPrefix + "Description" ||
                columnName == currentKTPrefix + "Remarks" ||
                columnName == currentKTPrefix + "SourceDesc" ||
                columnName == currentKTPrefix + "AnalysisResult" ||
                columnName == currentKTPrefix + "TargetTask" ||
                columnName == currentKTPrefix + "Innovation"
                )
                return true;
            else
                return false;
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

        addLink: function () {
            BS_.baoCun();

            if (saveProjectResult == true && saveSubjectResult == true) {
                saveProjectResult = false;
                saveSubjectResult = false;

                //点击选择文件框的时候已经选择了该行了
                var row = $("#pg").propertygrid('getSelected');
                var rowIndex = $("#pg").propertygrid('getRowIndex', row);

                var No = _CommomOperation.selectNearestOverNo(rowIndex, currentKTPrefix + "No");
                openLinkDialog(No, IsShenHe.NO);
            }
        },

        addAttach: function () {
            BS_.baoCun();

            if (saveProjectResult == true && saveSubjectResult == true) {
                saveProjectResult = false;
                saveSubjectResult = false;

                //点击选择文件框的时候已经选择了该行了
                var row = $("#pg").propertygrid('getSelected');
                var rowIndex = $("#pg").propertygrid('getRowIndex', row);

                var No = _CommomOperation.selectNearestOverNo(rowIndex, currentKTPrefix + "No");
                openAttachDialog(No, IsShenHe.NO);
            }
        }
    }