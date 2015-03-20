
    var editIndex = undefined;
    var ajaxResult = undefined;
    var newRow = {};

    var editDepts = [];
    var editStations = [];
    var editStationsName = [];
    var editTutor = undefined;
    var editEmpType = undefined;

    var openDeptDialogFlag = false;
    var openStationDialogFlag = false;
    var openTutorDialogFlag = false;

    var BS_ = {
        clearArgs: function () {
            editIndex = undefined;
            editDepts = [];
            editStations = [];
            editStationsName = [];
            editTutor = undefined;
        },

        onLoad: function () {
            var searchType = $('#link_type').combobox('getValue') == '' ? 1 : $('#link_type').combobox('getValue');
            
            var pageopt = $('#tTable').datagrid('getPager').data("pagination").options;
            var pageVal = {
                "pageNow": pageopt.pageNumber == 0 ? 1 : pageopt.pageNumber,
                "pageSize": pageopt.pageSize,
                "type": searchType
            }

            $('#tTable').datagrid("loading");
            AT_.AjaxPost("/Emp/GetEmps", pageVal, BS_.onLoadSuccess);
        },

        onLoadSuccess:function(data, status){
            $.messager.show({
                title: '加载人员列表',
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
                    pageNumber: data.pageNumber,
                    total: data.totalCount,
                    onSelectPage: BS_.onSelectPage
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

            editEmpType = $('#link_type').combobox('getValue');
            $('#tTable').datagrid("loaded");
        },









        selectType: function (record) { 
            if (BS_.endEditing()) {
                //导师
                if (record.value == Type.DAOSHI) {
               //     $('#tTable').datagrid('showColumn', 'FK_DeptName');
                    $('#tTable').datagrid('showColumn', 'FK_StationName');
                    $('#tTable').datagrid('showColumn', 'ChargeWork');
                    $('#tTable').datagrid('showColumn', 'OfficeAddr');
                    $('#tTable').datagrid('showColumn', 'OfficeTel');

                    $('#tTable').datagrid('hideColumn', 'AdmissionYear');
                    $('#tTable').datagrid('hideColumn', 'SchoolingLength');
                    $('#tTable').datagrid('hideColumn', 'FK_TutorName');
                    $('#tTable').datagrid('hideColumn', 'LabAddr');
                } else {
               //     $('#tTable').datagrid('hideColumn', 'FK_DeptName');
                    $('#tTable').datagrid('hideColumn', 'FK_StationName');
                    $('#tTable').datagrid('hideColumn', 'ChargeWork');
                    $('#tTable').datagrid('hideColumn', 'OfficeAddr');
                    $('#tTable').datagrid('hideColumn', 'OfficeTel');

                    $('#tTable').datagrid('showColumn', 'AdmissionYear');
                    $('#tTable').datagrid('showColumn', 'SchoolingLength');
                    $('#tTable').datagrid('showColumn', 'FK_TutorName');
                    $('#tTable').datagrid('showColumn', 'LabAddr');
                }
                editEmpType = $('#link_type').combobox('getValue');

                var searchType = $('#link_type').combobox('getValue') == '' ? 1 : $('#link_type').combobox('getValue');

                //加载第一页
                var pageopt = $('#tTable').datagrid('getPager').data("pagination").options;
                var pageVal = {
                    "pageNow": 1,
                    "pageSize": pageopt.pageSize,
                    "type": searchType
                }
                AT_.AjaxPost("/Emp/GetEmps", pageVal, BS_.onLoadSuccess);
            }
        },
         






        openDeptDialog: function () {
            openDeptDialogFlag = true;
            $('#divSelectDept').dialog('open');

            $('#tTableDept').treegrid("loading");
            AT_.AjaxPost("/Department/GetDepartments", null, BS_.onLoadDeptSuccess);
            
        },

        onLoadDeptSuccess: function (data, status) {
            $.messager.show({
                title: '加载部门树',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            $('#tTableDept').treegrid("loaded");

            if (data.state == "0") {
                var deptJsonResult = parseJSON(data._Json);
                $('#tTableDept').treegrid({ data: deptJsonResult });

                BS_.loadDeptSuccessSelectDept(deptJsonResult);
            } else {
                $('#tTableDept').treegrid("loadData", { total: 0, rows: [] });
            }
        },

        loadDeptSuccessSelectDept:function(deptJsonResult)
        {
            if (editDepts.length == 0) return;
            for (var i = 0; i < editDepts.length; i++) {
                var selectedId = BS_.getTreeIdFromDeptJsonResult(editDepts[i], deptJsonResult);
                if (selectedId != null) {
                    $('#tTableDept').treegrid('select', selectedId);
                }
            }
        },

        getTreeIdFromDeptJsonResult: function (deptNo, deptJsonResult) {
            var rows = deptJsonResult.rows;
            if (rows.length != 0) {
                for (var i = 0; i < rows.length; i++) {
                    if (rows[i].DeptNo == deptNo) {
                        return rows[i].TreeId;
                    }
                }
            }
            return null;
        },

        selectDeptOnClickRow: function (row) {
            //此处的编辑并非真正的编辑，只是为了方便部门描述部分的显示
            $('#tTableDept').treegrid('select', row.TreeId)
                .treegrid('beginEdit', row.TreeId);
        },

        selectDept: function () {
            //每次open dialog显示，虽然没显示选中状态，但是实验证明，会保存上次的选中清空，所以每次关闭或者保存后需要unselectall
            var selectedNodes = $('#tTableDept').treegrid('getSelections');
            var selectDeptName = "";
            var tempEditDepts = [];

            for (var i = 0; i < selectedNodes.length; i++) {
                if(i == selectedNodes.length - 1){
                    selectDeptName += selectedNodes[i].Name;
                }
                else{
                    selectDeptName += selectedNodes[i].Name + ",";
                }

                tempEditDepts.push(selectedNodes[i].DeptNo);
            }

            //将选择的编号与上次编辑数据进行排序，然后比较是否有更改
            if (BS_.isArraysSame(tempEditDepts, editDepts)) {
                $.messager.show({
                    title: '信息提示',
                    msg: "数据没有变化",
                    timeout: 3000,
                    showType: 'show'
                });
            }
            else
            {
                //先接受变化
                $('#tTable').datagrid('acceptChanges');

                var targetRow = $('#tTable').datagrid('getRows')[editIndex];
                targetRow.FK_DeptName = selectDeptName;
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
            editDepts = tempEditDepts;  
            $('#divSelectDept').dialog('close');
            //刷新
            $('#tTableDept').treegrid('unselectAll');
        },

        unSelectDept: function () {
            $('#tTableDept').treegrid('unselectAll');
        },

        spiltDeptsFromStr: function ()
        {
            var row = jsonResult.rows[editIndex];
            if (row.FK_Dept == '') return [];

            var arr = row.FK_Dept.split(',');
            return arr;
        },

        cancelSelectDept: function () {
            //刷新
            $('#tTableDept').treegrid('unselectAll');

            $('#divSelectDept').dialog('close');
        },









        openStationDialog: function () {
            openStationDialogFlag = true;
            $('#divSelectStation').dialog('open');
            
            BS_.loadStations();
        },

        loadStations: function () {
            var pageopt = $('#tTableStation').datagrid('getPager').data("pagination").options;
            var pageVal = {
                "pageNow": pageopt.pageNumber == 0 ? 1 : pageopt.pageNumber,
                "pageSize": pageopt.pageSize
            }

            $('#tTableStation').datagrid("loading");
            AT_.AjaxPost("/Station/GetStations", pageVal, BS_.onLoadStationSuccess);
        },

        onLoadStationSuccess: function (data, status) {
            $.messager.show({
                title: '加载岗位列表',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            $('#tTableStation').datagrid("loaded");

            if (data.state == "0") {
                var stationJsonResult = parseJSON(data._Json);
                $('#tTableStation').datagrid({ data: stationJsonResult });
                
                var selectedNodes = $('#tTableSelectStation').datagrid('getRows');
                if (selectedNodes.length == 0) //不为0说明已加载过
                    BS_.loadStationSuccessSelectStation(stationJsonResult);
                 
                //加载成功后必须重新设置分页数据，因为分页的正确显示是在数据加载成功后设置才有效的
                $('#tTableStation').datagrid("getPager").pagination({
                    pageSize: data.pageSize,
                    pageNumber: data.pageNumber,
                    total: data.totalCount,
                    onSelectPage: BS_.onSelectStationPage
                });
            } else {
                $('#tTableStation').datagrid("loadData", { total: 0, rows: [] });
            }
        },

        loadStationSuccessSelectStation: function (stationJsonResult) {
            if (editStations.length == 0) return;
             
            for (var i = 0; i < editStations.length; i++) {
                if (BS_.ifStationExistInSelected(editStations[i]) == false) {
                    $('#tTableSelectStation').datagrid('appendRow', {
                        StaNo: editStations[i],
                        Name: editStationsName[i]
                    });
                }
            }
        },

        onSelectStationPage: function (pageNumber, pageSize) {
            $('#tTableStation').datagrid("loading");
            BS_.loadStations();
        },

        getIndexFromStationJsonResult: function (staNo, stationJsonResult)
        {
            var rows = stationJsonResult.rows;
            if (rows.length != 0) {
                for (var i = 0; i < rows.length; i++) {
                    if (rows[i].StaNo == staNo) {
                        return i;
                    }
                }
            }
            return null;
        },
        
        selectStationOnClickRow: function (index)
        {
            $('#tTableStation').datagrid('selectRow', index)
                               .datagrid('beginEdit', index);
        },

        selectStation: function ()
        {
            //每次open dialog显示，虽然没显示选中状态，但是实验证明，会保存上次的选中清空，
            //所以每次关闭或者保存后需要unselectall或者清空
            var selectedNodes = $('#tTableSelectStation').datagrid('getRows');
            var tempEditStations = [];
            var tempEditStationsName = [];
           
            for (var i = 0; i < selectedNodes.length; i++) {
                tempEditStations.push(selectedNodes[i].StaNo);
                tempEditStationsName.push(selectedNodes[i].Name);
            }

            //将选择的编号与上次编辑的数据进行排序，然后比较是否有更改
            if (BS_.isArraysSame(tempEditStations, editStations)) {
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
                targetRow.FK_StationName = tempEditStationsName.join(',');
                 
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

                //更新当前编辑数据
                editStations = tempEditStations;
                editStationsName = tempEditStationsName;
            }

            $('#divSelectStation').dialog('close');
            //刷新
            BS_.removeAllSelectedStation();
            //刷新
            $('#tTableStation').datagrid('unselectAll');
        },

        spiltStationsFromStr: function () {
            var row = jsonResult.rows[editIndex];
            if (row.FK_Station == '') return [];

            var arr = row.FK_Station.split(',');
            return arr;
        },
        
        spiltStationsNameFromStr: function () {
            var row = jsonResult.rows[editIndex];
            if (row.FK_StationName == '') return [];

            var arr = row.FK_StationName.split(',');
            return arr;
        },

        
        cancelSelectStation: function () {
            //刷新
            BS_.removeAllSelectedStation();
            //刷新
            $('#tTableStation').datagrid('unselectAll');

            $('#divSelectStation').dialog('close');
        },
       
        getStationToSelected: function () {
            var rows = $('#tTableStation').datagrid('getSelections');
            if (rows.length == 0) return;
            for (var i = 0; i < rows.length; i++) {
                if (BS_.ifStationExistInSelected(rows[i].StaNo) == false) {
                    $('#tTableSelectStation').datagrid('appendRow', {
                        StaNo: rows[i].StaNo,
                        Name: rows[i].Name
                    });
                }
            }
        },
         
        ifStationExistInSelected: function (staNo) {
            var rows = $('#tTableSelectStation').datagrid('getRows');
            for (var i = 0; i < rows.length; i++) {
                if (rows[i].StaNo == staNo) {
                    return true;
                }
            }
            return false;
        },

        removeSelectedStation: function () {
            var rows = $('#tTableSelectStation').datagrid('getSelections');
            if (rows.length == 0) return;
            for (var i = 0; i < rows.length; i++) {
                var index = $('#tTableSelectStation').datagrid('getRowIndex', rows[i]);
                $('#tTableSelectStation').datagrid('deleteRow', index);
            }
        },

        removeAllSelectedStation: function () {
            var rows = $('#tTableSelectStation').datagrid('getRows');
            if (rows.length == 0) return;
            var length = rows.length;
            for (var i = 0; i < length; i++) {
                var index = $('#tTableSelectStation').datagrid('getRowIndex', rows[i]);
                $('#tTableSelectStation').datagrid('deleteRow', index);
            }
        },








        openTutorDialog: function () {
            openTutorDialogFlag = true;
            $('#divSelectTutor').dialog('open');

            BS_.loadTutors();
        },

        loadTutors: function () {
            var pageopt = $('#tTableTutor').datagrid('getPager').data("pagination").options;
            var pageVal = {
                "pageNow": pageopt.pageNumber == 0 ? 1 : pageopt.pageNumber,
                "pageSize": pageopt.pageSize,
                "type": Type.DAOSHI
            }

            $('#tTableTutor').datagrid("loading");
            AT_.AjaxPost("/Emp/GetEmps", pageVal, BS_.onLoadTutorSuccess);
        },

        onLoadTutorSuccess: function (data, status) {
            $.messager.show({
                title: '加载导师列表',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            $('#tTableTutor').datagrid("loaded");

            if (data.state == "0") {
                var tutorJsonResult = parseJSON(data._Json);
                $('#tTableTutor').datagrid({ data: tutorJsonResult });
                
                BS_.loadTutorSuccessSelectTutor(tutorJsonResult);

                //加载成功后必须重新设置分页数据，因为分页的正确显示是在数据加载成功后设置才有效的
                $('#tTableTutor').datagrid("getPager").pagination({
                    pageSize: data.pageSize,
                    pageNumber: data.pageNumber,
                    total: data.totalCount,
                    onSelectPage: BS_.onSelectTutorPage
                });
            } else { 
                $('#tTableTutor').datagrid("loadData", { total: 0, rows: [] });
            }
        },

        loadTutorSuccessSelectTutor: function (tutorJsonResult) {
            if (editTutor == undefined) return;
            var rows = tutorJsonResult.rows;

            if (rows.length != 0) {
                for (var i = 0; i < rows.length; i++) {
                    if (rows[i].EmpNo == editTutor) {
                        $('#tTableTutor').datagrid('selectRow', i);
                        return;
                    }
                }
            }
        },

        onSelectTutorPage: function (pageNumber, pageSize) {
            $('#tTableTutor').datagrid("loading");
            BS_.loadTutors();
        },
 
        selectTutorOnClickRow: function (index) {
            $('#tTableTutor').datagrid('selectRow', index);
        },

        selectTutor: function () {
            //每次open dialog显示，虽然没显示选中状态，但是实验证明，会保存上次的选中清空，
            //所以每次关闭或者保存后需要unselectall或者清空
            var selectedNode = $('#tTableTutor').datagrid('getSelected');
            var selectTutorName = "";
            var tempEditTutor = "";

            if (selectedNode != null) {
                selectTutorName += selectedNode.Name;
                tempEditTutor = selectedNode.EmpNo;
            }

            //将选择的编号与上次编辑的数据比较是否有更改
            if (tempEditTutor == editTutor || (tempEditTutor == "" && editTutor==undefined)) {
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
                targetRow.FK_TutorName = selectTutorName;

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
            if (tempEditTutor.length != 0) {
                //更新当前编辑数据
                editTutor = tempEditTutor;
            }
            $('#divSelectTutor').dialog('close');
            //刷新
            $('#tTableTutor').datagrid('unselectAll');
        },
         

        unSelectTutor: function () {
            $('#tTableTutor').datagrid('unselectAll');
        },

        cancelSelectTutor: function () {
            //刷新
            $('#tTableTutor').datagrid('unselectAll');

            $('#divSelectTutor').dialog('close');
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

                openDeptDialogFlag = false;
                openStationDialogFlag = false;
                openTutorDialogFlag = false;

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
            if (jsonResult.rows.length == 0) return true;//新增的数据
            targetRow = jsonResult.rows[editIndex];
            if (targetRow == undefined) return true;//新增的数据
            //根据表格行内容判断
            if (row.EmpNo != targetRow.EmpNo) return true;
            if (row.Name != targetRow.Name) return true;
            if (row.Tel != targetRow.Tel) return true;
            if (row.Email != targetRow.Email) return true;
            if (row.Type != targetRow.Type) return true;
            if (openDeptDialogFlag == true) {
                var originDeptArr = BS_.spiltDeptsFromStr();
                if (BS_.isArraysSame(editDepts, originDeptArr) == false) return true;
            }

            if (editEmpType == Type.DAOSHI) {
              //  var originDeptArr = BS_.spiltDeptsFromStr();
                //  if (BS_.isArraysSame(editDepts, originDeptArr) == false) return true;
                if (openStationDialogFlag == true) {
                    var originStationArr = BS_.spiltStationsFromStr();
                    if (BS_.isArraysSame(editStations, originStationArr) == false) return true;
                }
                if (row.ChargeWork != targetRow.ChargeWork) return true;
                if (row.OfficeAddr != targetRow.OfficeAddr) return true;
                if (row.OfficeTel != targetRow.OfficeTel) return true;
            }

            else {
                if (row.AdmissionYear != targetRow.AdmissionYear) return true;
                if (row.SchoolingLength != targetRow.SchoolingLength) return true;
                if (row.LabAddr != targetRow.LabAddr) return true;
                if (openTutorDialogFlag == true) {
                    var originTutor = jsonResult.rows[editIndex].FK_Tutor;
                    if (editTutor != originTutor) return true;
                }
            }

            return false;
        },

        save: function () { 
            var row = $('#tTable').datagrid('getRows')[editIndex];

            if (editEmpType == Type.DAOSHI) {
                if (row.Type == "" || row.Type != "1") {
                    $.messager.show({
                        title: '信息提示',
                        msg: "类型请选择【导师】",
                        timeout: 3000,
                        showType: 'show'
                    });
                    return;
                }
            }
            else {
                if (row.Type == "" || row.Type == "1") {
                    $.messager.show({
                        title: '信息提示',
                        msg: "类型请选择【本科生】|【研究生】|【博士生】",
                        timeout: 3000,
                        showType: 'show'
                    });
                    return;
                }
            }

            //新增数据则添加，修改数据则更新
            var tableCount = $('#tTable').datagrid('getRows').length;
            //表格长度大于jsonResult长度，新增数据
            //表格长度等于jsnResult长度，修改数据
            if (tableCount >= jsonResult.rows.length) {
                
                newRow = {};
                newRow["EmpNo"] = row.EmpNo;
                newRow["Name"] = row.Name;
                newRow["Tel"] = row.Tel;
                newRow["Email"] = row.Email;
                newRow["Type"] = row.Type;
                if (openDeptDialogFlag == false) {
                    newRow["FK_Dept"] = jsonResult.rows[editIndex].FK_Dept;
                } else {
                    newRow["FK_Dept"] = editDepts.join(',');
                }
                newRow["FK_DeptName"] = row.FK_DeptName;

                if (editEmpType == Type.DAOSHI) {
                    //  newRow["FK_Dept"] = editDepts.join(',');
                    if (openStationDialogFlag == false) {
                        newRow["FK_Station"] = jsonResult.rows[editIndex].FK_Station;
                    } else {
                        newRow["FK_Station"] = editStations.join(',');
                    }
                    newRow["ChargeWork"] = row.ChargeWork;
                    newRow["OfficeAddr"] = row.OfficeAddr;
                    newRow["OfficeTel"] = row.OfficeTel;

                    //newRow["FK_DeptName"] = row.FK_DeptName;
                    newRow["FK_StationName"] = row.FK_StationName;
                }

                else {
                    newRow["AdmissionYear"] = row.AdmissionYear;
                    newRow["SchoolingLength"] = row.SchoolingLength;
                    newRow["LabAddr"] = row.LabAddr;
                    if (openTutorDialogFlag == false) {
                        newRow["FK_Tutor"] = jsonResult.rows[editIndex].FK_Tutor;
                    } else {
                        newRow["FK_Tutor"] = editTutor;
                    }
                    newRow["FK_TutorName"] = row.FK_TutorName;
                }

                if (row.EmpNo == undefined || row.EmpNo == "") {
                    $.messager.show({
                        title: '信息提示',
                        msg: "编号不能为空",
                        timeout: 3000,
                        showType: 'show'
                    });
                    return;
                }

                if (tableCount > jsonResult.rows.length) {
                    AT_.AjaxPost("/Emp/TianJiaRenYuan", newRow, BS_.addSuccess);
                } else {
                    var mRow = cloneJSON(newRow);
                    //不破坏newRow的结构
                    mRow["oldNo"] = jsonResult.rows[editIndex].EmpNo;
                    AT_.AjaxPost("/Emp/XiuGaiRenYuan", mRow, BS_.modifySuccess);
                }
            }
        },
        addSuccess: function(data, status) {
            $.messager.show({
                title: '添加人员',
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
                title: '修改人员',
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
                $.messager.confirm("删除人员", "您确定删除 【" + rows[editIndex].Name + "】 吗？", function (data) {
                    if (data) {
                        var delEmpNo = {
                            //使用jsonResult获取而不是表格，因为可能编辑了一半后直接删除
                            "empNo": jsonResult.rows[editIndex].EmpNo
                        }
                   
                        AT_.AjaxPost("/Emp/ShanChuRenYuan", delEmpNo, BS_.deleteSuccess);
                    }
                });
            }
            
        },
        deleteSuccess: function(data, status) {
            $.messager.show({
                title: '删除人员',
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

                $('#tTable').datagrid('appendRow', { EmpNo: "Emp_" });
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

                    //如果是导师，初始化部门、岗位字段
                    if (editEmpType == Type.DAOSHI) {
                        editDepts = BS_.spiltDeptsFromStr();
                        editStations = BS_.spiltStationsFromStr();
                        editStationsName = BS_.spiltStationsNameFromStr();
                    }
                    else {
                        editTutor = jsonResult.rows[editIndex].FK_Tutor;
                    }

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