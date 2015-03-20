
var _EmpsSelect = {
    open_EmpsDialog: function () {
        $('#divSelect_Emps').dialog('open');
        _EmpsSelect.load_Emps();
    },

    load_Emps: function () {
        var searchType = $('#link__Empstype').combobox('getValue') == '' ? 1 : $('#link__Empstype').combobox('getValue');

        var pageopt = $('#tTable_Emps').datagrid('getPager').data("pagination").options;
        var pageVal = {
            "pageNow": pageopt.pageNumber == 0 ? 1 : pageopt.pageNumber,
            "pageSize": pageopt.pageSize,
            "type": searchType
        }

        $('#tTable_Emps').datagrid("loading");
        AT_.AjaxPost("/Emp/GetEmps", pageVal, _EmpsSelect.onLoad_EmpsSuccess);
    },

    onLoad_EmpsSuccess: function (data, status) {
        $.messager.show({
            title: '加载人员列表',
            msg: data.message,
            timeout: 3000,
            showType: 'show'
        });
        $('#tTable_Emps').datagrid("loaded");

        if (data.state == "0") {
            var empJsonResult = parseJSON(data._Json);
             
            $('#tTable_Emps').datagrid({
                data: empJsonResult,
                singleSelect: false
            });
             
            //加载成功后必须重新设置分页数据，因为分页的正确显示是在数据加载成功后设置才有效的
            $('#tTable_Emps').datagrid("getPager").pagination({
                pageSize: data.pageSize,
                pageNumber: data.pageNumber,
                total: data.totalCount,
                onSelectPage: _EmpsSelect.onSelect_EmpsPage
            });
        }
    },
     
    onSelect_EmpsPage: function (pageNumber, pageSize) {
        $('#tTable_Emps').datagrid("loading");
        _EmpsSelect.load_Emps();
    },

    select_EmpsOnClickRow: function (index) {
        $('#tTable_Emps').datagrid('selectRow', index);
    },

    select_Emps: function () {
        //每次open dialog显示，虽然没显示选中状态，但是实验证明，会保存上次的选中清空，所以每次关闭或者保存后需要unselectall
        var selectedNode = $('#tTableSelect_Emps').datagrid('getRows');
        var returnEmpsStr = "";

        for (var i = 0; i < selectedNode.length; i++) {
            if (i == selectedNode.length - 1 )
                returnEmpsStr += selectedNode[i].EmpNo + "(" + selectedNode[i].Name + ")";
            else
                returnEmpsStr += selectedNode[i].EmpNo + "(" + selectedNode[i].Name + "),";
        }
         
        $('#divSelect_Emps').dialog('close');
        //刷新
        _EmpsSelect.removeAllSelected_Emps();
        //刷新
        $('#tTable_Emps').datagrid('unselectAll');

        return returnEmpsStr;
    },
     
    cancelSelect_Emps: function () {
        //刷新
        _EmpsSelect.removeAllSelected_Emps();
        //刷新
        $('#tTable_Emps').datagrid('unselectAll');

        $('#divSelect_Emps').dialog('close');
    },
      
    get_EmpsSelected: function () {
        var rows = $('#tTable_Emps').datagrid('getSelections');
        if (rows.length == 0) return;
         
        for (var i = 0; i < rows.length; i++) {
            if (_EmpsSelect.ifMemberExistInSelected(rows[i].EmpNo) == false) {
                $('#tTableSelect_Emps').datagrid('appendRow', {
                    EmpNo: rows[i].EmpNo,
                    Name: rows[i].Name
                });
            }
        }
    },

    ifMemberExistInSelected: function (empNo) {
        var rows = $('#tTableSelect_Emps').datagrid('getRows');
        for (var i = 0; i < rows.length; i++) {
            if (rows[i].EmpNo == empNo) {
                return true;
            }
        }
        return false;
    },

    removeSelected_Emps: function () {
        var rows = $('#tTableSelect_Emps').datagrid('getSelections');
        if (rows.length == 0) return;
        for (var i = 0; i < rows.length; i++) {
            var index = $('#tTableSelect_Emps').datagrid('getRowIndex', rows[i]);
            $('#tTableSelect_Emps').datagrid('deleteRow', index);
        }
    },

    removeAllSelected_Emps: function () {
        var rows = $('#tTableSelect_Emps').datagrid('getRows');
        if (rows.length == 0) return;
        var length = rows.length;
        for (var i = 0; i < length; i++) {
            var index = $('#tTableSelect_Emps').datagrid('getRowIndex', rows[i]);
            $('#tTableSelect_Emps').datagrid('deleteRow', index);
        }
    },
     
    select_EmpsType: function (record) {
        if (record.value == Type.DAOSHI) {
            $('#tTable_Emps').datagrid('showColumn', 'FK_DeptName');
            $('#tTable_Emps').datagrid('showColumn', 'FK_StationName');

            $('#tTable_Emps').datagrid('hideColumn', 'AdmissionYear');
            $('#tTable_Emps').datagrid('hideColumn', 'FK_TutorName');
        } else {
            $('#tTable_Emps').datagrid('hideColumn', 'FK_DeptName');
            $('#tTable_Emps').datagrid('hideColumn', 'FK_StationName');

            $('#tTable_Emps').datagrid('showColumn', 'AdmissionYear');
            $('#tTable_Emps').datagrid('showColumn', 'FK_TutorName');
        }
        _EmpsSelect.load_Emps();
    } 
}



$('#link_get_Empstoselected').bind('click', _EmpsSelect.get_EmpsSelected);
$('#link_removeselect_Emps').bind('click', _EmpsSelect.removeSelected_Emps);
$('#link_removeallselect_Emps').bind('click', _EmpsSelect.removeAllSelected_Emps);
