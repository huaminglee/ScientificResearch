
    var editIndex = undefined;
    var ajaxResult = undefined;
    var newRow = {};

    var editLeader = undefined;
    var editLeaderName = undefined;
    var editMembers = [];
    var editMembersName = [];
    var editSelectLeaderOrMemberType = undefined;

    var openLeaderDialogFlag = false;
    var openMemberDialogFlag = false;

    var BS_ = {
        clearArgs: function () {
            editIndex = undefined;
            editLeader = undefined;
            editLeaderName = undefined;
            editMembers = [];
            editMembersName = [];
        },

        onLoad: function () {
            var pageopt = $('#tTable').datagrid('getPager').data("pagination").options;
            var pageVal = {
                "pageNow": pageopt.pageNumber == 0 ? 1 : pageopt.pageNumber,
                "pageSize": pageopt.pageSize
            }

            $('#tTable').datagrid("loading");
            AT_.AjaxPost("/XiangMuZu/GetXiangMuZu", pageVal, BS_.onLoadSuccess);
        },

        onLoadSuccess:function(data, status){
            $.messager.show({
                title: '加载项目组列表',
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












        openLeaderOrMemberDialog: function (type) {
            if (type == LeaderOrMemberType.ZUZHANG) {
                openLeaderDialogFlag = true;
            } else {
                openMemberDialogFlag = true;
            }

            editSelectLeaderOrMemberType = type;

            $('#divSelectLeaderOrMember').dialog('open');

            BS_.loadLeaderOrMember();
        },

        loadLeaderOrMember: function () {
            var searchType = $('#link_leaderormembertype').combobox('getValue') == '' ? 1 : $('#link_leaderormembertype').combobox('getValue');

            var pageopt = $('#tTableLeaderOrMember').datagrid('getPager').data("pagination").options;
            var pageVal = {
                "pageNow": pageopt.pageNumber == 0 ? 1 : pageopt.pageNumber,
                "pageSize": pageopt.pageSize,
                "type": searchType
            }

            $('#tTableLeaderOrMember').datagrid("loading");
            AT_.AjaxPost("/Emp/GetEmps", pageVal, BS_.onLoadLeaderOrMemberSuccess);
        },

        onLoadLeaderOrMemberSuccess: function (data, status) {
            $.messager.show({
                title: '加载人员列表',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            $('#tTableLeaderOrMember').datagrid("loaded");

            if (data.state == "0") {
                var empJsonResult = parseJSON(data._Json);

                if (editSelectLeaderOrMemberType == LeaderOrMemberType.ZUZHANG) {
                    $('#tTableLeaderOrMember').datagrid({
                        data: empJsonResult,
                        singleSelect: true 
                    });
                    $("#divSelectLeaderOrMember").panel({ title: "选择组长" });
                }
                else {
                    $('#tTableLeaderOrMember').datagrid({
                        data: empJsonResult,
                        singleSelect: false 
                    });
                    $("#divSelectLeaderOrMember").panel({ title: "选择组员" });
                }
                
                var selectedNodes = $('#tTableSelectLeaderOrMember').datagrid('getRows');
                if (selectedNodes.length == 0) //不为0说明已加载过
                    BS_.loadEmpSuccessSelectLeaderOrMember(empJsonResult);
               
                //加载成功后必须重新设置分页数据，因为分页的正确显示是在数据加载成功后设置才有效的
                $('#tTableLeaderOrMember').datagrid("getPager").pagination({
                    pageSize: data.pageSize,
                    pageNumber: data.pageNumber,
                    total: data.totalCount,
                    onSelectPage: BS_.onSelectLeaderOrMemberPage
                });
            }
        },

        loadEmpSuccessSelectLeaderOrMember: function (empJsonResult) {
            if (editSelectLeaderOrMemberType == LeaderOrMemberType.ZUZHANG) {
                if (editLeader == undefined) return;

                if (BS_.ifMemberExistInSelected(editLeader) == false) {
                    $('#tTableSelectLeaderOrMember').datagrid('appendRow', {
                        EmpNo: editLeader,
                        Name: editLeaderName
                    });
                }
            }
            else {
                if (editMembers.length == 0) return;

                for (var i = 0; i < editMembers.length; i++) {
                    if (BS_.ifMemberExistInSelected(editMembers[i]) == false) {
                        $('#tTableSelectLeaderOrMember').datagrid('appendRow', {
                            EmpNo: editMembers[i],
                            Name: editMembersName[i]
                        });
                    }
                }
            } 
        },

        getIndexFromEmpJsonResult: function (empNo, empJsonResult) {
            var rows = empJsonResult.rows;
            if (rows.length != 0) {
                for (var i = 0; i < rows.length; i++) {
                    if (rows[i].EmpNo == empNo) {
                        return i;
                    }
                }
            }
            return null;
        },

        onSelectLeaderOrMemberPage: function (pageNumber, pageSize) {
            $('#tTableLeaderOrMember').datagrid("loading");
            BS_.loadLeaderOrMember();
        },

        selectLeaderOrMemberOnClickRow: function (index) {
            $('#tTableLeaderOrMember').datagrid('selectRow', index);
        },

        selectLeaderOrMember: function () {
            //每次open dialog显示，虽然没显示选中状态，但是实验证明，会保存上次的选中清空，所以每次关闭或者保存后需要unselectall
            var selectedNode = $('#tTableSelectLeaderOrMember').datagrid('getRows');
            var tempEditLeaderName = "";
            var tempEditLeader = "";
            var tempEditMembersName = [];
            var tempEditMembers = [];

            if (editSelectLeaderOrMemberType == LeaderOrMemberType.ZUZHANG) {
                if (selectedNode != null && selectedNode.length!=0) {
                    tempEditLeader = selectedNode[0].EmpNo;
                    tempEditLeaderName = selectedNode[0].Name;
                }
                //将选择的编号与上次编辑的数据比较是否有更改
                if (tempEditLeader == editLeader || (tempEditLeader == "" && editLeader==undefined)) {
                    $.messager.show({
                        title: '信息提示',
                        msg: "数据没有变化",
                        timeout: 3000,
                        showType: 'show'
                    });
                }
                else {
                    //先接受变化
                    $('#tTable').datagrid('acceptChanges');

                    var targetRow = $('#tTable').datagrid('getRows')[editIndex];
                    targetRow.FK_GroupLeaderName = tempEditLeaderName;

                    //需要先refreshRow再endEdit
                    $('#tTable').datagrid('refreshRow', editIndex)
                                .datagrid('endEdit', editIndex)
                                .datagrid('selectRow', editIndex)
                                .datagrid('beginEdit', editIndex);

                    $.messager.show({
                        title: '信息提示',
                        msg: "数据发生了变化",
                        timeout: 3000,
                        showType: 'show'
                    });
                }
                if (tempEditLeader.length != 0) {
                    //更新当前编辑数据
                    editLeader = tempEditLeader;
                    editLeaderName = tempEditLeaderName;
                    //若组员中没有组长，将组长添加至组员
                    BS_.selectLeaderToMembers();
                }
            }

            else {
                for (var i = 0; i < selectedNode.length; i++) {
                    tempEditMembers.push(selectedNode[i].EmpNo);
                    tempEditMembersName.push(selectedNode[i].Name);
                }
                //将选择的编号与上次编辑的数据比较是否有更改
                if (BS_.isArraysSame(tempEditMembers, editMembers)) {
                    $.messager.show({
                        title: '信息提示',
                        msg: "数据没有变化",
                        timeout: 3000,
                        showType: 'show'
                    });
                }
                else {
                    //先接受变化
                    $('#tTable').datagrid('acceptChanges');

                    var targetRow = $('#tTable').datagrid('getRows')[editIndex];
                    //旧数据在前，新数据在后
                    targetRow.FK_GroupMemberName = tempEditMembersName.join(',');

                    //需要先refreshRow再endEdit
                    $('#tTable').datagrid('refreshRow', editIndex)
                                .datagrid('endEdit', editIndex)
                                .datagrid('selectRow', editIndex)
                                .datagrid('beginEdit', editIndex);

                    $.messager.show({
                        title: '信息提示',
                        msg: "数据发生了变化",
                        timeout: 3000,
                        showType: 'show'
                    });
                }
                //更新当前编辑数据
                editMembers = tempEditMembers;
                editMembersName = tempEditMembersName;

                //若组员中没有组长，将组长添加至组员
                BS_.selectLeaderToMembers();
            }
            
            $('#divSelectLeaderOrMember').dialog('close');
            //刷新
            BS_.removeAllSelectedLeaderOrMember();
            //刷新
            $('#tTableLeaderOrMember').datagrid('unselectAll');

            editSelectLeaderOrMemberType = undefined;
        },

        selectLeaderToMembers: function () {
            if (editLeader == undefined || editLeader==null ) return;
            else {
                if (editMembers.indexOf(editLeader) != -1)
                    return;
                editMembers.push(editLeader);
                editMembersName.push(editLeaderName);
            }

            var targetRow = $('#tTable').datagrid('getRows')[editIndex];
            targetRow.FK_GroupMemberName = editMembersName.join(',');

            //需要先refreshRow再endEdit
            $('#tTable').datagrid('refreshRow', editIndex)
                        .datagrid('endEdit', editIndex)
                        .datagrid('selectRow', editIndex)
                        .datagrid('beginEdit', editIndex);

        },

        cancelSelectLeaderOrMember: function () {
            //刷新
            BS_.removeAllSelectedLeaderOrMember();
            //刷新
            $('#tTableLeaderOrMember').datagrid('unselectAll');

            $('#divSelectLeaderOrMember').dialog('close');

            editSelectLeaderOrMemberType = undefined;
        },

        spiltMembersFromStr: function () {
            var row = jsonResult.rows[editIndex];
            if (row.FK_GroupMember == '') return [];

            var arr = row.FK_GroupMember.split(',');
            return arr;
        },

        spiltMembersNameFromStr: function () {
            var row = jsonResult.rows[editIndex];
            if (row.FK_GroupMemberName == '') return [];

            var arr = row.FK_GroupMemberName.split(',');
            return arr;
        },

        getLeaderOrMemberSelected: function () {
            var rows = $('#tTableLeaderOrMember').datagrid('getSelections');
            if (rows.length == 0) return;

            if (editSelectLeaderOrMemberType == LeaderOrMemberType.ZUZHANG) {
                BS_.removeAllSelectedLeaderOrMember();
                $('#tTableSelectLeaderOrMember').datagrid('appendRow', {
                    EmpNo: rows[0].EmpNo,
                    Name: rows[0].Name
                });
            }
            else {
                for (var i = 0; i < rows.length; i++) {
                    if (BS_.ifMemberExistInSelected(rows[i].EmpNo) == false) {
                        $('#tTableSelectLeaderOrMember').datagrid('appendRow', {
                            EmpNo: rows[i].EmpNo,
                            Name: rows[i].Name
                        });
                    }
                }
            }
            
        },

        ifMemberExistInSelected: function (empNo) {
            var rows = $('#tTableSelectLeaderOrMember').datagrid('getRows');
            for (var i = 0; i < rows.length; i++) {
                if (rows[i].EmpNo == empNo) {
                    return true;
                }
            }
            return false;
        },

        removeSelectedLeaderOrMember: function () {
            var rows = $('#tTableSelectLeaderOrMember').datagrid('getSelections');
            if (rows.length == 0) return;
            for (var i = 0; i < rows.length; i++) {
                var index = $('#tTableSelectLeaderOrMember').datagrid('getRowIndex', rows[i]);
                $('#tTableSelectLeaderOrMember').datagrid('deleteRow', index);
            }
        },

        removeAllSelectedLeaderOrMember: function () {
            var rows = $('#tTableSelectLeaderOrMember').datagrid('getRows');
            if (rows.length == 0) return;
            var length = rows.length;
            for (var i = 0; i < length; i++) {
                var index = $('#tTableSelectLeaderOrMember').datagrid('getRowIndex', rows[i]);
                $('#tTableSelectLeaderOrMember').datagrid('deleteRow', index);
            }
        },











        selectLeaderOrMemberType: function (record) {
            if (record.value == Type.DAOSHI) {
                $('#tTableLeaderOrMember').datagrid('showColumn', 'FK_DeptName');
                $('#tTableLeaderOrMember').datagrid('showColumn', 'FK_StationName');
                 
                $('#tTableLeaderOrMember').datagrid('hideColumn', 'AdmissionYear');
                $('#tTableLeaderOrMember').datagrid('hideColumn', 'FK_TutorName');
            } else {
                $('#tTableLeaderOrMember').datagrid('hideColumn', 'FK_DeptName');
                $('#tTableLeaderOrMember').datagrid('hideColumn', 'FK_StationName');
                 
                $('#tTableLeaderOrMember').datagrid('showColumn', 'AdmissionYear');
                $('#tTableLeaderOrMember').datagrid('showColumn', 'FK_TutorName');
            }
            BS_.loadLeaderOrMember();
        },






        isArraysSame: function (array1, array2) {
            if (array1.length != array2.length) {
                return false;
            }

            var tempArr1 = BS_.copyArray(array1);
            var tempArr2 = BS_.copyArray(array2);

            tempArr1.sort();//改变自身
            tempArr2.sort();//改变自身

            for (var i = 0; i < tempArr1.length; i++) {
                if (tempArr1[i] != tempArr2[i]) {
                    return false;
                }
            }
            return true;
        },

        copyArray: function (array) {
            var retArr = [];
            for (var i = 0; i < array.length; i++) {
                retArr[i] = array[i];
            }
            return retArr;
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
                openLeaderDialogFlag = false;
                openMemberDialogFlag = false;

                BS_.clearArgs();

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
            if (row.No != targetRow.No) return true;
            if (row.Name != targetRow.Name) return true;
            if (row.Description != targetRow.Description) return true;
            if (openLeaderDialogFlag == true) {
                var originLeader = jsonResult.rows[editIndex].FK_GroupLeader;
                if (editLeader != originLeader) return true;
            }
            if (openMemberDialogFlag == true) {
                var originMemberArr = BS_.spiltMembersFromStr();
                if (BS_.isArraysSame(editMembers, originMemberArr) == false) return true;
            }
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
                newRow["No"] = row.No;
                newRow["Name"] = row.Name;
                newRow["Description"] = row.Description;
                if (tableCount > jsonResult.rows.length && openLeaderDialogFlag == false) {
                    newRow["FK_GroupLeader"] = "";
                }
                else if (openLeaderDialogFlag == false) {
                    newRow["FK_GroupLeader"] = jsonResult.rows[editIndex].FK_GroupLeader;
                } else {
                    newRow["FK_GroupLeader"] = editLeader;
                }

                if (tableCount > jsonResult.rows.length && openMemberDialogFlag == false) {
                    newRow["FK_GroupMember"] = "";
                }
                else if (openMemberDialogFlag == false) {
                    newRow["FK_GroupMember"] = jsonResult.rows[editIndex].FK_GroupMember;
                } else {
                    newRow["FK_GroupMember"] = editMembers.join(',');
                }

                // 注意组长名字与组员名字
                newRow["FK_GroupLeaderName"] = editLeaderName;
                newRow["FK_GroupMemberName"] = editMembersName.join(',');
                newRow["Projects"] = row.Projects;

                if (row.No == undefined || row.No == "") {
                    $.messager.show({
                        title: '信息提示',
                        msg: "编号不能为空",
                        timeout: 3000,
                        showType: 'show'
                    });
                    return;
                }

                if (tableCount > jsonResult.rows.length) {
                    AT_.AjaxPost("/XiangMuZu/TianJiaXiangMuZu", newRow, BS_.addSuccess);
                } else {
                    var mRow = cloneJSON(newRow);
                    //不破坏newRow的结构
                    mRow["oldNo"] = jsonResult.rows[editIndex].No;
                    AT_.AjaxPost("/XiangMuZu/XiuGaiXiangMuZu", mRow, BS_.modifySuccess);
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
               
                BS_.clearArgs();

                //重新设置分页数据
                BS_.setPageArgs(pageArgs);
                return;
            } else {
                $.messager.confirm("删除项目组", "您确定删除 【" + rows[editIndex].Name + "】 吗？", function (data) {
                    if (data) {
                        var delXMZNo = {
                            //使用jsonResult获取而不是表格，因为可能编辑了一半后直接删除
                            "prjGroupNo": jsonResult.rows[editIndex].No
                        }
                        AT_.AjaxPost("/XiangMuZu/ShanChuXiangMuZu", delXMZNo, BS_.deleteSuccess);
                    }
                });
            }
            
        },
        deleteSuccess: function(data, status) {
            $.messager.show({
                title: '删除项目组',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            if (data.state == "0") {
                var pageopt = $('#tTable').datagrid('getPager').data("pagination").options;
                var pageArgs = { "pageSize": pageopt.pageSize, "pageNumber": pageopt.pageNumber, "total": pageopt.total - 1 }
                

                $('#tTable').datagrid('deleteRow', editIndex);
                jsonResult.rows.splice(editIndex, 1);
                
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
            if (BS_.endEditing()) {
                var pageopt = $('#tTable').datagrid('getPager').data("pagination").options;
                var pageArgs = { "pageSize": pageopt.pageSize, "pageNumber": pageopt.pageNumber, "total": pageopt.total + 1 }

                $('#tTable').datagrid('appendRow', { No: "XMZ_" });
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
                    
                    editLeader = jsonResult.rows[editIndex].FK_GroupLeader;
                    editLeaderName = jsonResult.rows[editIndex].FK_GroupLeaderName;
                    editMembers = BS_.spiltMembersFromStr();
                    editMembersName = BS_.spiltMembersNameFromStr();
                    
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