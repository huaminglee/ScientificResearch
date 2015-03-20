var newRow = {};
var editProjectOID = undefined;
var saveResult = undefined;
var _closeAfterSave = true;

var openProjectDialogFlag = false;

    var BS_ = {
        clearArgs: function () {
            editProjectOID = undefined;
        },

        onLoad: function () {
            var pageopt = $('#tTable').datagrid('getPager').data("pagination").options;
            var pageVal = {
                "pageNow": pageopt.pageNumber == 0 ? 1 : pageopt.pageNumber,
                "pageSize": pageopt.pageSize
            }

            $('#tTable').datagrid("loading");
            AT_.AjaxPost("/KeTi/GetKeTi", pageVal, BS_.onLoadSuccess);
        },

        onLoadSuccess:function(data, status){
            $.messager.show({
                title: '加载课题列表',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            if (data.state == "0") {
                jsonResult = parseJSON(data._Json);
                jsonData = cloneJSON(jsonResult);

                //使用复制数据的原因是：改变表格数据的时候，例如appendRow会改变与表格相关联的json数据，而在判断表格与原始数据是否有更动的时候，这种情况将会导致很多麻烦
                $('#tTable').datagrid({ data: jsonData, });
                //加载成功后必须重新设置分页数据，因为分页的正确显示是在数据加载成功后设置才有效的
                $('#tTable').datagrid("getPager").pagination({
                    pageSize: data.pageSize,
                    pageNumber :data.pageNumber,
                    total:data.totalCount,
                    onSelectPage:  BS_.onSelectPage
                });
            } else {
                jsonResult = { total: 0, rows: [] };
                $('#tTable').datagrid("loadData", cloneJSON(jsonResult));
                //加载成功后必须重新设置分页数据，因为分页的正确显示是在数据加载成功后设置才有效的
                $('#tTable').datagrid("getPager").pagination({
                    pageSize: data.pageSize,
                    pageNumber: data.pageNumber,
                    total: data.totalCount,
                    onSelectPage: BS_.onSelectPage
                });
            }

            BS_.clearArgs();

            $('#tTable').datagrid("loaded");
            
        },
 
       




        remove: function () {
            var selectedRow = $('#tTable').datagrid('getSelected');
            if (selectedRow == undefined) return;
            var selectedIndex = $('#tTable').datagrid('getRowIndex', selectedRow);

            var rows = $('#tTable').datagrid('getRows');
            //表格长度大于jsonResult.rows长度，说明是新的未保存数据，直接删除表格内容即可
            if (rows.length > jsonResult.rows.length) {
                // alert("数据尚未保存，直接删除表格数据");
                var pageopt = $('#tTable').datagrid('getPager').data("pagination").options;
                var pageArgs = { "pageSize": pageopt.pageSize, "pageNumber": pageopt.pageNumber, "total": pageopt.total - 1 }

                $('#tTable').datagrid('deleteRow', selectedIndex);
                
                BS_.clearArgs();
                //重新设置分页数据
                BS_.setPageArgs(pageArgs);
                 
                return;
            } else {
                $.messager.confirm("删除课题", "您确定删除 【" + rows[selectedIndex].Name + "】 吗？", function (data) {
                    if (data) {
                        var delKTNo = {
                            //使用jsonResult获取而不是表格，因为可能编辑了一半后直接删除
                            "subNo": jsonResult.rows[selectedIndex].No
                        }
                        AT_.AjaxPost("/KeTi/ShanChuKeTi", delKTNo, BS_.deleteSuccess);
                    }
                });
            }
            
        },

        deleteSuccess: function (data, status) {
            var selectedRow = $('#tTable').datagrid('getSelected');
            if (selectedRow == undefined) return;
            var selectedIndex = $('#tTable').datagrid('getRowIndex', selectedRow);

            $.messager.show({
                title: '删除课题',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            if (data.state == "0") {
                var pageopt = $('#tTable').datagrid('getPager').data("pagination").options;
                var pageArgs = { "pageSize": pageopt.pageSize, "pageNumber": pageopt.pageNumber, "total": pageopt.total - 1 }
                  
                $('#tTable').datagrid('deleteRow', selectedIndex);
                jsonResult.rows.splice(selectedIndex, 1);

                BS_.clearArgs();
                BS_.setPageArgs(pageArgs);

                //当前页数据删除完，自动加载对应的数据
                var isNoData = ($('#tTable').datagrid('getRows').length == 0);
                if (isNoData) {
                    BS_.onLoad();
                }
            }
        },
         





        appendRow: function() {
            var pageopt = $('#tTable').datagrid('getPager').data("pagination").options;
            var pageArgs = { "pageSize": pageopt.pageSize, "pageNumber": pageopt.pageNumber, "total": pageopt.total + 1 }

            $('#tTable').datagrid('appendRow', { No: "Sub_" });
            var selectedIndex = $('#tTable').datagrid('getRows').length - 1;
            $('#tTable').datagrid('selectRow', selectedIndex);

            //重新设置分页数据
            BS_.setPageArgs(pageArgs);
 
            BS_.edit();
        },
         








        edit: function () {
            //若非新建的数据，则请求当前项目详情
            var selectedRow = $('#tTable').datagrid('getSelected');
            var rows = $('#tTable').datagrid('getRows');
            if (selectedRow == null) return;
            $('#divEditSubject').dialog('open');

            if (rows.length == jsonResult.rows.length) {
                var subVal = { subNo: selectedRow.No }
                AT_.AjaxPost("/KeTi/GetKeTiByNo", subVal, BS_.onLoadKeTiSuccess);

                //设置拟定时间为不可编辑
                var pData = $("#pg").propertygrid('getData');
                for (var i = 0; i < pData.rows.length; i++) {
                    var id = pData.rows[i].id;
                    if (id == "ProposeTime") {
                        pData.rows[i].editor = "";
                    }
                }
            }  
        },

        endEdit: function () {
            BS_.clearArgs();

            $('#divEditSubject').dialog('close');
            //刷新
            $("#pg").propertygrid({ data: cloneJSON(propertygridData) });
        },

        onLoadKeTiSuccess: function (data, status) {
            $.messager.show({
                title: '加载课题信息',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });

            if (data.state == "0") {
                propertygridJsonResult = parseJSON(data._Json);

                BS_.getPropertygridDataFormJsonResult(propertygridJsonResult);
                //初始化editProjectOID
                editProjectOID = propertygridJsonResult.rows[0].FK_XmOID;
                
                //加载链接
                var rowIndex = BS_.getRowIndexById("Link");
                loadLinks(propertygridJsonResult.rows[0].No, IsShenHe.NO, rowIndex, CanLinkDelete.YES);
                //加载链接
                var rowIndex2 = BS_.getRowIndexById("Attach");
                loadAttachs(propertygridJsonResult.rows[0].No, IsShenHe.NO, rowIndex2, CanAttachDelete.YES);

            }
        },

        getRowIndexById: function (id) {
            var rows = $("#pg").propertygrid('getRows');
            for (var i = 0; i < rows.length; i++) {
                if (rows[i].id == id) {
                    return i;
                }
            }
        },

        getPropertygridDataFormJsonResult: function (result) {
            var rows = result.rows;
            if (rows == undefined || rows.length == 0) {
                return;
            }
            
            var newData = $("#pg").propertygrid('getData');
            for (var i = 0; i < newData.rows.length; i++) {
                var id = newData.rows[i].id;
                newData.rows[i].value = result.rows[0][id];
            }
            $('#pg').propertygrid({ data: newData });
        },







        saveEditSubject: function () {
            var row = $("#pg").propertygrid("getSelected");
            var editIndex = $("#pg").propertygrid('getRowIndex', row);
            $("#pg").propertygrid('endEdit', editIndex);

            if (BS_.isChange() == true) {
                //保存
                BS_.save();
                 
                if (saveResult == true) {
                    openProjectDialogFlag = false;
                }
            }
            else {
                openProjectDialogFlag = false;

                saveResult = true;
                if (_closeAfterSave == true) BS_.endEdit();
            }
        },

        isChange: function () {
            var selectedRow = $('#tTable').datagrid('getSelected');
            var selectedIndex = $('#tTable').datagrid('getRowIndex', selectedRow);
            var rows = $('#tTable').datagrid('getRows');

            if (rows.length > jsonResult.rows.length) {
                //新增的数据
                return true;
            }
            else {
                if (openProjectDialogFlag == true) {
                    var originFK_XmOID = propertygridJsonResult.rows[0].FK_XmOID;
                    if (originFK_XmOID != editProjectOID)
                        return true;
                }
                return BS_.comparePropertygridDataWithPropertygridJsonResult();
            }
        },

        comparePropertygridDataWithPropertygridJsonResult: function () {
            var editData = $("#pg").propertygrid('getData');
            
            for (var i = 0; i < editData.rows.length; i++) {
                var id = editData.rows[i].id;

                if (BS_.ifColumnNeedCompare(id) == true) {
                    if (propertygridJsonResult.rows[0][id] != editData.rows[i].value) {
                        return true;
                    }
                }
            }
            return false;
        },

        ifColumnNeedCompare: function (columnName) {
            if (columnName == "No" ||
                columnName == "Name" ||
                columnName == "ProposeTime" ||
                columnName == "Keys" ||
                columnName == "Description" ||
                columnName == "Remarks" ||
                columnName == "SourceDesc" ||
                columnName == "AnalysisResult" ||
                columnName == "TargetTask" ||
                columnName == "Innovation"
                )
                return true;
            else
                return false;
        },

        save: function () {
            var data = $("#pg").propertygrid('getData');
            for (var i = 0; i < data.rows.length; i++) {
                if (data.rows[i].id == "No") {
                    if (data.rows[i].value == "") {
                        $.messager.show({
                            title: '信息提示',
                            msg: "编号不能为空",
                            timeout: 3000,
                            showType: 'show'
                        });
                        return;
                    }
                }
            }

            var tableCount = $('#tTable').datagrid('getRows').length;

            newRow = {};
            newRow["No"] = BS_.getValueFromPropertygridById("No");
            newRow["Name"] = BS_.getValueFromPropertygridById("Name");
            newRow["FK_Proposer"] = currentLoginUser;
            newRow["ProposeTime"] = BS_.getValueFromPropertygridById("ProposeTime");
            newRow["ModifyTime"] = (new Date()).Format("dd/MM/yyyy hh:mm:ss");
            newRow["Keys"] = BS_.getValueFromPropertygridById("Keys");
            newRow["Description"] = BS_.getValueFromPropertygridById("Description");
            newRow["Remarks"] = BS_.getValueFromPropertygridById("Remarks");

            if (openProjectDialogFlag == false) {
                newRow["FK_XmOID"] = propertygridJsonResult.rows[0].FK_XmOID;
            } else {
                newRow["FK_XmOID"] = editProjectOID;
            }
            newRow["SourceDesc"] = BS_.getValueFromPropertygridById("SourceDesc");
            newRow["AnalysisResult"] = BS_.getValueFromPropertygridById("AnalysisResult");
            newRow["TargetTask"] = BS_.getValueFromPropertygridById("TargetTask");
            newRow["Innovation"] = BS_.getValueFromPropertygridById("Innovation");

            //保持数据的显示一致
            newRow["ProposerName"] = currentLoginUserName;

            var proTempData = $("#pg").propertygrid('getData');
            for (var i = 0; i < proTempData.rows.length; i++) {
                var id = proTempData.rows[i].id;
                if (id == "FK_XmName") {
                    newRow["FK_XmName"] = proTempData.rows[i].value;
                    break;
                }
            }
             
            if (tableCount > jsonResult.rows.length) {
                AT_.AjaxPost("/KeTi/TianJiaKeTi", newRow, BS_.addSuccess);
            } else {
                var mRow = cloneJSON(newRow);
                //不破坏newRow的结构
                mRow["oldNo"] = propertygridJsonResult.rows[0].No;
                
                AT_.AjaxPost("/KeTi/XiuGaiKeTi", mRow, BS_.modifySuccess);
            }
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

        addSuccess: function (data, status) {
            $.messager.show({
                title: '添加课题',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });

            if (data.state == "0") {
                jsonResult.rows.push(newRow);
                $('#tTable').datagrid({ data: cloneJSON(jsonResult) });
                BS_.endEdit();
                saveResult = true;
            } else {
                saveResult = false;
            }
        },

        modifySuccess: function (data, status) {
            $.messager.show({
                title: '修改课题',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            if (data.state == "0") {
                var selectedRow = $('#tTable').datagrid('getSelected');
                var selectedIndex = $('#tTable').datagrid('getRowIndex', selectedRow);

                jsonResult.rows.splice(selectedIndex, 1, newRow);
                $('#tTable').datagrid({ data: cloneJSON(jsonResult) });
                BS_.endEdit();
                saveResult = true;
            } else {
                saveResult = false;
            }
        },











        cancelSaveEditSubject: function () {
            var row = $("#pg").propertygrid("getSelected");
            var editIndex = $("#pg").propertygrid('getRowIndex', row);
            $("#pg").propertygrid('endEdit', editIndex);

            $.messager.confirm("取消", "您确定放弃保存吗？", function (data) {
                if (data) {
                    BS_.endEdit();
                    
                    //若新建的数据，则删除
                    var selectedRow = $('#tTable').datagrid('getSelected');
                    var selectedIndex = $('#tTable').datagrid('getRowIndex', selectedRow);
                    var rows = $('#tTable').datagrid('getRows');
                     
                    if (rows.length > jsonResult.rows.length) {
                        var pageopt = $('#tTable').datagrid('getPager').data("pagination").options;
                        var pageArgs = { "pageSize": pageopt.pageSize, "pageNumber": pageopt.pageNumber, "total": pageopt.total - 1 }

                        $('#tTable').datagrid('deleteRow', selectedIndex);

                        //重新设置分页数据
                        BS_.setPageArgs(pageArgs);
                        return;
                    }
                }
            });
        },










        onDblClickRow:function (index) {
            BS_.edit();
        },

        onSelectPage: function (pageNumber, pageSize) {
            $('#tTable').datagrid("loading");
            BS_.onLoad();
        },
         
        setPageArgs: function (pageArgs) {
            $('#tTable').datagrid("getPager").pagination({
                pageSize: pageArgs.pageSize == 0 ? 1 : pageArgs.pageSize,
                pageNumber: pageArgs.pageNumber,
                total: pageArgs.total
            });
        },



















        openProjectDialog: function () {
            openProjectDialogFlag = true;
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
            } else {
                $('#tTableProject').datagrid("loadData", { total: 0, rows: [] });
            }
        },

        loadProjectsSuccessSelectProject: function (projectsJsonResult) {
            if (editProjectOID == undefined) return;
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
            var tempEditProjectName = "";
            var tempEditProjectOID = "";

            if (selectedNode != null) {
                tempEditProjectName = selectedNode.Name;
                tempEditProjectOID = selectedNode.OID;
            }

            //将选择的编号与上次编辑的数据比较是否有更改
            if (tempEditProjectOID == editProjectOID || (tempEditProjectOID == "" && editProjectOID==undefined)) {
                $.messager.show({
                    title: '信息提示',
                    msg: "数据没有变化",
                    timeout: 3000,
                    showType: 'show'
                });
            }
            else {
                var newData = $("#pg").propertygrid('getData');
                for (var i = 0; i < newData.rows.length; i++) {
                    var id = newData.rows[i].id;
                    if (id == "FK_XmName")
                        newData.rows[i].value = tempEditProjectName;
                }
                $('#pg').propertygrid({ data: newData });

                $.messager.show({
                    title: '信息提示',
                    msg: "数据发生了变化",
                    timeout: 3000,
                    showType: 'show'
                });
            }
            if (tempEditProjectOID.length != 0) {
                //更新当前编辑数据
                editProjectOID = tempEditProjectOID;
            }
            $('#divSelectProject').dialog('close');
            //刷新
            $('#tTableProject').datagrid('unselectAll');
        },


        unSelectProject: function () {
            $('#tTableProject').datagrid('unselectAll');
        },

        cancelSelectProject: function () {
            //刷新
            $('#tTableProject').datagrid('unselectAll');

            $('#divSelectProject').dialog('close');
        },
         
        addLink: function () {
            _closeAfterSave = false;

            BS_.saveEditSubject();
            if (saveResult == true) {
                _closeAfterSave = true;
                saveResult = false;
                 
                var No = BS_.getValueFromPropertygridById("No");  
                openLinkDialog(No, IsShenHe.NO);
            }
        },

        addAttach: function () {
            _closeAfterSave = false;

            BS_.saveEditSubject();
            if (saveResult == true) {
                _closeAfterSave = true;
                saveResult = false;

                var No = BS_.getValueFromPropertygridById("No");  
                openAttachDialog(No, IsShenHe.NO);
            }
        }
    }