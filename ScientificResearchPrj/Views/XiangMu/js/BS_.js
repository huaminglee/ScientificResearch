var newRow = {};
 
    var BS_ = {
        onLoad: function () {
            
            var pageopt = $('#tTable').datagrid('getPager').data("pagination").options;
            var pageVal = {
                "pageNow": pageopt.pageNumber == 0 ? 1 : pageopt.pageNumber,
                "pageSize": pageopt.pageSize
            }

            $('#tTable').datagrid("loading");
            AT_.AjaxPost("/XiangMu/GetXiangMu", pageVal, BS_.onLoadSuccess);
        },

        onLoadSuccess:function(data, status){
            $.messager.show({
                title: '加载项目列表',
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
                
                //重新设置分页数据
                BS_.setPageArgs(pageArgs);
                 
                return;
            } else {
                $.messager.confirm("删除项目", "您确定删除 【" + rows[selectedIndex].Name + "】 吗？", function (data) {
                    if (data) {
                        var delXMNo = {
                            //使用jsonResult获取而不是表格，因为可能编辑了一半后直接删除
                            "prjNo": jsonResult.rows[selectedIndex].No
                        }
                        AT_.AjaxPost("/XiangMu/ShanChuXiangMu", delXMNo, BS_.deleteSuccess);
                    }
                });
            }
            
        },

        deleteSuccess: function (data, status) {
            var selectedRow = $('#tTable').datagrid('getSelected');
            if (selectedRow == undefined) return;
            var selectedIndex = $('#tTable').datagrid('getRowIndex', selectedRow);

            $.messager.show({
                title: '删除项目',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            if (data.state == "0") {
                var pageopt = $('#tTable').datagrid('getPager').data("pagination").options;
                var pageArgs = { "pageSize": pageopt.pageSize, "pageNumber": pageopt.pageNumber, "total": pageopt.total - 1 }
                  
                $('#tTable').datagrid('deleteRow', selectedIndex);
                jsonResult.rows.splice(selectedIndex, 1);

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

            $('#tTable').datagrid('appendRow', { No: "Prj_" });
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
            $('#divEditProject').dialog('open');

            if (rows.length == jsonResult.rows.length) {
                var prjVal = { priNo: selectedRow.No }
                AT_.AjaxPost("/XiangMu/GetXiangMuByNo", prjVal, BS_.onLoadProjectSuccess);

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
            $('#divEditProject').dialog('close');
            //刷新
            $("#pg").propertygrid({ data: cloneJSON(propertygridData) });
        },

        onLoadProjectSuccess:function (data, status) {
            $.messager.show({
                title: '加载项目信息',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });

            if (data.state == "0") {
                propertygridJsonResult = parseJSON(data._Json);

                BS_.getPropertygridDataFormJsonResult(propertygridJsonResult);
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







        saveEditProject: function () {
            var row = $("#pg").propertygrid("getSelected");
            var editIndex = $("#pg").propertygrid('getRowIndex', row);
            $("#pg").propertygrid('endEdit', editIndex);

            if (BS_.isChange()) {
                //保存
                BS_.save();
            }
            else
                BS_.endEdit();
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
                columnName == "FK_Xmz" ||
                columnName == "Columns" ||
                columnName == "Tasks" ||
                columnName == "Questions" ||
                columnName == "Remarks")
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
            newRow["FK_Xmz"] = BS_.getValueFromPropertygridById("FK_Xmz");
            newRow["Columns"] = BS_.getValueFromPropertygridById("Columns");
            newRow["Tasks"] = BS_.getValueFromPropertygridById("Tasks");
            newRow["Questions"] = BS_.getValueFromPropertygridById("Questions");
            newRow["Remarks"] = BS_.getValueFromPropertygridById("Remarks");

            //保持数据的显示一致
            newRow["ProposerName"] = currentLoginUserName;
            if (ProjectGroupData != undefined) {
                for (var i = 0; i < ProjectGroupData.length; i++) {
                    if (ProjectGroupData[i].value == newRow["FK_Xmz"]) {
                        newRow["FK_XMZName"] = ProjectGroupData[i].text;
                    }
                }
            }
            if (tableCount > jsonResult.rows.length) {
                AT_.AjaxPost("/XiangMu/TianJiaXiangMu", newRow, BS_.addSuccess);
            } else {
                var mRow = cloneJSON(newRow);
                //不破坏newRow的结构
                mRow["oldNo"] = propertygridJsonResult.rows[0].No;
                AT_.AjaxPost("/XiangMu/XiuGaiXiangMu", mRow, BS_.modifySuccess);
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
                title: '添加项目',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });

            if (data.state == "0") {
                jsonResult.rows.push(newRow);
                $('#tTable').datagrid({ data: cloneJSON(jsonResult) });
                BS_.endEdit();
            }
        },

        modifySuccess: function (data, status) {
            $.messager.show({
                title: '修改项目',
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
            }
        },











        cancelSaveEditProject: function () {
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
        } 
    }