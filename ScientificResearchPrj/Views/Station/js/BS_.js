
    var editIndex = undefined;
    var ajaxResult = undefined;
    var newRow = {};

    var BS_ = {
        onLoad: function() {
            var pageopt = $('#tTable').datagrid('getPager').data("pagination").options;
            var pageVal = {
                "pageNow": pageopt.pageNumber == 0 ? 1 : pageopt.pageNumber,
                "pageSize": pageopt.pageSize
            }

            $('#tTable').datagrid("loading");
            AT_.AjaxPost("/Station/GetStations", pageVal, BS_.onLoadSuccess);
        },

        onLoadSuccess:function(data, status){
            $.messager.show({
                title: '加载岗位列表',
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

            editIndex = undefined;
            $('#tTable').datagrid("loaded");
        },




        endEditing: function() {
            if (editIndex == undefined) { return true }
            if ($('#tTable').datagrid('validateRow', editIndex)) {
                $('#tTable').datagrid('endEdit', editIndex);

                //先保存,acceptChanges以后就不能撤回了
                $('#tTable').datagrid('acceptChanges');
                //人工判断是否改变数据
                if (BS_.isChange()) {
                    //保存
                    BS_.save();
                    
                    //添加或者修改失败
                    if (ajaxResult == false || ajaxResult == undefined) {
                        $('#tTable').datagrid('selectRow', editIndex)
                             .datagrid('beginEdit', editIndex);
                        return false;
                    } else {
                        ajaxResult = false;
                    }
                }
                editIndex = undefined;
                return true;
            } else {
                return false;
            }
        },

        isChange: function() {
            var row = $('#tTable').datagrid('getRows')[editIndex];
            var targetRow = undefined;
            //新增的数据，会因为表格（已acceptChange）长度大于jsonResult长度而获取失败
            targetRow = jsonResult.rows[editIndex];
            if (targetRow == undefined) return true;//新增的数据
            //根据表格行内容判断
            if (row.StaNo != targetRow.StaNo) return true;
            if (row.Name != targetRow.Name) return true;
            if (row.Description != targetRow.Description) return true;
            if (row.StaGrade != targetRow.StaGrade) return true;
            return false;
        },

        save:function() {
            //新增数据则添加，修改数据则更新
            var tableCount = $('#tTable').datagrid('getRows').length;
            //表格长度大于jsonResult长度，新增数据
            //表格长度等于jsnResult长度，修改数据
            if (tableCount >= jsonResult.rows.length) {
                var row = $('#tTable').datagrid('getRows')[editIndex];
                
                newRow = {};
                newRow["StaNo"] = row.StaNo;
                newRow["Name"] = row.Name;
                newRow["Description"] = row.Description;
                newRow["StaGrade"] = row.StaGrade;
                  
                if (row.StaNo == undefined || row.StaNo == "") {
                    $.messager.show({
                        title: '信息提示',
                        msg: "编号不能为空",
                        timeout: 3000,
                        showType: 'show'
                    });
                    return;
                }

                if (tableCount > jsonResult.rows.length) {
                    AT_.AjaxPost("/Station/TianJiaGangWei", newRow, BS_.addSuccess); 
                } else {
                    var mRow = cloneJSON(newRow);
                    //不破坏newRow的结构
                    mRow["oldNo"] = jsonResult.rows[editIndex].StaNo;
                    AT_.AjaxPost("/Station/XiuGaiGangWei", mRow, BS_.modifySuccess);
                }
            }
        },
        addSuccess: function(data, status) {
            $.messager.show({
                title: '添加岗位',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            if (data.state == "0") {
                jsonResult.rows.push(newRow);
                ajaxResult = true;
            } else {
                ajaxResult = false;
            }
        },
        modifySuccess: function(data, status) {
            $.messager.show({
                title: '修改岗位',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            if (data.state == "0") {
                jsonResult.rows.splice(editIndex, 1, newRow);
                ajaxResult = true;
            } else {
                ajaxResult = false;
            }
             
        },
       



        remove: function() {
            if (editIndex == undefined) return;
            var rows = $('#tTable').datagrid('getRows');
            //表格长度大于jsonResult.rows长度，说明是新的未保存数据，直接删除表格内容即可
            if (rows.length > jsonResult.rows.length) {
                // alert("数据尚未保存，直接删除表格数据");
                var pageopt = $('#tTable').datagrid('getPager').data("pagination").options;
                var pageArgs = { "pageSize": pageopt.pageSize, "pageNumber": pageopt.pageNumber, "total": pageopt.total - 1 }

                $('#tTable').datagrid('deleteRow', editIndex);
                editIndex = undefined;

                //重新设置分页数据
                BS_.setPageArgs(pageArgs);
                return;
            } else {
                $.messager.confirm("删除岗位", "您确定删除 【" + rows[editIndex].Name + "】 吗？", function (data) {
                    if (data) {
                        var delStaNo = {
                            //使用jsonResult获取而不是表格，因为可能编辑了一半后直接删除
                            "staNo":jsonResult.rows[editIndex].StaNo
                        }
                        AT_.AjaxPost("/Station/ShanChuGangWei", delStaNo, BS_.deleteSuccess);
                    }
                });
            }
            
        },
        deleteSuccess: function(data, status) {
            $.messager.show({
                title: '删除岗位',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            if (data.state == "0") {
                var pageopt = $('#tTable').datagrid('getPager').data("pagination").options;
                var pageArgs = { "pageSize": pageopt.pageSize, "pageNumber": pageopt.pageNumber, "total": pageopt.total - 1 }
                

                $('#tTable').datagrid('deleteRow', editIndex);
                jsonResult.rows.splice(editIndex, 1);
                editIndex = undefined;
                
                BS_.setPageArgs(pageArgs);

                //当前页数据删除完，自动加载对应的数据
                var isNoData = ($('#tTable').datagrid('getRows').length == 0);
                if (isNoData) {
                    BS_.onLoad();
                }
            }
        },




        appendRow: function() {
            if (BS_.endEditing()) {
                var pageopt = $('#tTable').datagrid('getPager').data("pagination").options;
                var pageArgs = { "pageSize": pageopt.pageSize, "pageNumber": pageopt.pageNumber, "total": pageopt.total + 1 }
               
                $('#tTable').datagrid('appendRow', { StaNo: "Sta_" });
                editIndex = $('#tTable').datagrid('getRows').length - 1;
                $('#tTable').datagrid('selectRow', editIndex)
                        .datagrid('beginEdit', editIndex);

                //重新设置分页数据
                BS_.setPageArgs(pageArgs);
               
            }
        },




        onClickRow: function(index) {
            if (editIndex != index) {
                if (BS_.endEditing()) {
                    $('#tTable').datagrid('selectRow', index)
                            .datagrid('beginEdit', index);
                    editIndex = index;
                } else {
                    $('#tTable').datagrid('selectRow', editIndex);
                }
            }
        },

        onSelectPage: function (pageNumber, pageSize) {
            if (BS_.endEditing()) {
                $('#tTable').datagrid("loading");
                BS_.onLoad();
 
            }
        },



        setPageArgs: function (pageArgs) {
            $('#tTable').datagrid("getPager").pagination({
                pageSize: pageArgs.pageSize == 0 ? 1 : pageArgs.pageSize,
                pageNumber: pageArgs.pageNumber,
                total: pageArgs.total
            });
        }
    }