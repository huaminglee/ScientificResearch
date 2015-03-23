var BS_ = {

    saveEditInfo: function () {
        if ($('#Name').textbox('getValue') == "") {
            $.messager.show({
                title: '提示',
                msg: "名字不能为空",
                timeout: 3000,
                showType: 'show'
            });
            return;
        }
        
        if (document.getElementById('IfModifyPass').checked) {
            if ($('#Pass').textbox('getValue') == "" || $('#NewPass').textbox('getValue') == "" || $('#RePass').textbox('getValue') == "") {
                $.messager.show({
                    title: '提示',
                    msg: "密码不能为空",
                    timeout: 3000,
                    showType: 'show'
                });
                return;
            }
            if ($('#NewPass').textbox('getValue') != $('#RePass').textbox('getValue')) { 
                $.messager.show({
                    title: '提示',
                    msg: "密码不一致",
                    timeout: 3000,
                    showType: 'show'
                });
                return;
            }
        }

        var newRow = BS_.getNewRow(); 
        AT_.AjaxPost("/Emp/XiuGaiGeRenXinXi", newRow, BS_.modifySuccess);
    },

    modifySuccess: function (data, status) {
        $.messager.show({
            title: '修改信息',
            msg: data.message,
            timeout: 3000,
            showType: 'show'
        });
    },

    ModifyPassword: function () {
        if (document.getElementById('IfModifyPass').checked) {
            $('#Pass').textbox('enable');
            $('#NewPass').textbox('enable');
            $('#RePass').textbox('enable');
        } else {
            $('#Pass').textbox('disable');
            $('#NewPass').textbox('disable');
            $('#RePass').textbox('disable');
        }
    },

    getNewRow: function () {
        var newRow = {};
        newRow["EmpNo"] = $('#EmpNo').textbox('getValue');
        newRow["Name"] = $('#Name').textbox('getValue');
        newRow["Email"] = $('#Email').textbox('getValue');
        newRow["Tel"] = $('#Tel').textbox('getValue');
        if (document.getElementById('IfModifyPass').checked) {
            newRow["Pass"] = $('#Pass').textbox('getValue');
            newRow["NewPass"] = $('#NewPass').textbox('getValue');
        }
        return newRow;
    }
}