
var _CommomOperation = {

    getPropertygridDataFormJsonResult: function (result, prefix) {
        _CommomOperation.propertygridEndEdit();

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

    getAndCloneCurrentPropertygridDataFormJsonResult: function (result, prefix) {
        _CommomOperation.propertygridEndEdit();

        //prefix是前缀，防止id重复
        var rows = result.rows;
        if (rows == undefined || rows.length == 0) {
            return;
        }
        var insertIndex = _CommomOperation.getFirstInsertIndexByGroupName(_CommomOperation.getGroupNameByPrefix(prefix));

        _CommomOperation.deletePropertygridDataByGroupName(prefix);

        var newData = $("#pg").propertygrid('getData');
        for (var i = 0; i < rows.length; i++) {
            var tempData = undefined;
            if (i == 0)
                tempData = _CommomOperation.getClonePropertygridByPrefix(prefix, 0, 2);
            else if (i < rows.length - 1)
                tempData = _CommomOperation.getClonePropertygridByPrefix(prefix, 0, 1);
            else
                tempData = _CommomOperation.getClonePropertygridByPrefix(prefix, 0, 0);

            for (var j = 0; j < tempData.length; j++) {
                //更新index
                tempData[j].index = i;

                var id = tempData[j].id;
                if (id != undefined && id.indexOf(prefix) != -1) {
                    id = id.substring(prefix.length);
                    tempData[j].value = result.rows[i][id];
                }
            }

            if (i != rows.length - 1) {
                tempData.push({
                    name: "分隔位置",
                    value: ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>",
                    group: _CommomOperation.getGroupNameByPrefix(prefix),
                    index: i + 1
                });
            }

            tempData.forEach(function (e) {
                newData.rows.splice(insertIndex++, 0, e);
            });
        }
        $('#pg').propertygrid({ data: newData });
    },

    getValueFromPropertygridById: function (id, index) {
        var data = $("#pg").propertygrid('getData');
        for (var i = 0; i < data.rows.length; i++) {
            if (data.rows[i].id == id && data.rows[i].index == index) {
                return data.rows[i].value;
            }
        }
        return "";
    },
     
    getAndClonePropertygridDataFormJsonResult: function (result, prefix) {
        _CommomOperation.propertygridEndEdit();

        //prefix是前缀，防止id重复
        var rows = result.rows;
        if (rows == undefined || rows.length == 0) {
            return;
        }
        var insertIndex = _CommomOperation.getFirstInsertIndexByGroupName(_CommomOperation.getGroupNameByPrefix(prefix));

        _CommomOperation.deletePropertygridDataByGroupName(prefix);

        var newData = $("#pg").propertygrid('getData');
        for (var i = 0; i < rows.length; i++) {
            var tempData = undefined;
            if (i == 0)
                tempData = _CommomOperation.getClonePropertygridByPrefix(prefix, 0, 0);
            else
                tempData = _CommomOperation.getClonePropertygridByPrefix(prefix, 1, 0);

            for (var j = 0; j < tempData.length; j++) {
                //更新index
                tempData[j].index = i;

                var id = tempData[j].id;
                if (id != undefined && id.indexOf(prefix) != -1) {
                    id = id.substring(prefix.length);
                    tempData[j].value = result.rows[i][id];
                }
            }

            tempData.forEach(function (e) {
                newData.rows.splice(insertIndex++, 0, e);
            });
            if (i != rows.length - 1) {
                newData.rows.splice(insertIndex++, 0, {
                    name: "分隔位置",
                    value: ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>",
                    group: _CommomOperation.getGroupNameByPrefix(prefix),
                });
            }
        }

        $('#pg').propertygrid({ data: newData });
    },

    deletePropertygridDataByGroupName: function (prefix) {
        var newData = $("#pg").propertygrid('getData');
        for (var i = newData.rows.length - 1; i >= 0; i--) {
            if (newData.rows[i].group == _CommomOperation.getGroupNameByPrefix(prefix)) {
                $("#pg").propertygrid('deleteRow', i);
            }
        }
    },

    getGroupNameByPrefix: function (prefix) {
        if (prefix == "XM_") return "需求分析-项目结果";
        if (prefix == "KT_") return "需求分析-课题结果";
        if (prefix == "XQFXSH_") return "需求分析审核结果";
        if (prefix == "DY_") return "调研结果";
        if (prefix == "DYSH_") return "调研审核结果";
        if (prefix == "TCWT_") return "提出问题结果";
        if (prefix == "TCWTSH_") return "提出问题审核结果";
        if (prefix == "JJSL_") return "解决思路结果";
        if (prefix == "JJSLSH_") return "解决思路审核结果";
        if (prefix == "XSH_") return "形式化结果";
        if (prefix == "XSHSH_") return "形式化审核结果";
        if (prefix == "SF_") return "设计算法结果";
        if (prefix == "SJSFSH_") return "设计算法审核结果";
        if (prefix == "SY_") return "设计实验结果";
        if (prefix == "SJSYSH_") return "设计实验审核结果";
        if (prefix == "DBFX_") return "量化对比分析结果";
        if (prefix == "DBFXSH_") return "量化对比分析审核结果";
        if (prefix == "DCJL_") return "得出结论结果";
        if (prefix == "DCJLSH_") return "得出结论审核结果";
        if (prefix == "LWZX_") return "论文或专利撰写结果";
        if (prefix == "LWZXSH_") return "论文或专利撰写审核结果";
    },

    getFirstInsertIndexByGroupName: function (groupName) {
        var newData = $("#pg").propertygrid('getData');
        for (var i = 0; i < newData.rows.length; i++) {
            var group = newData.rows[i].group;
            if (group != undefined && group == groupName) {
                return i;
            }
        }
        return 0;
    },

    getLastInsertIndexByGroupName: function (groupName) {
        var index = 0;
        var newData = $("#pg").propertygrid('getData');
        for (var i = 0; i < newData.rows.length; i++) {
            var group = newData.rows[i].group;
            if (group != undefined && group == groupName) {
                index = i;
            }
        }
        return index + 1;
    },

    getClonePropertygridByPrefix: function (prefix, frontRowRemoveCount, lastRowRemoveCount) {
        var tempData = _CommomOperation.getTargetPropertygridDataByPrefix(prefix);
        var temp = [];

        if ((frontRowRemoveCount > tempData.length) ||
            (lastRowRemoveCount > tempData.length) ||
            (frontRowRemoveCount + lastRowRemoveCount > tempData.length))
            return;

        for (var i = frontRowRemoveCount; i < tempData.length - lastRowRemoveCount; i++) {
            temp.push(tempData[i]);
        }
        return temp;
    },

    getTargetPropertygridDataByPrefix: function (prefix) {
        var target = [];
        if (prefix == "XM_") {
            $.extend(true, target, XiangMuPropertygridData);
            return target;
        }
        if (prefix == "KT_") {
            $.extend(true, target, KeTiPropertygridData);
            return target;
        }
        if (prefix == "XQFXSH_") {
            $.extend(true, target, XQFXShenHePropertygridData);
            return target;
        }
        if (prefix == "DY_") {
            $.extend(true, target, DYPropertygridData);
            return target;
        }
        if (prefix == "DYSH_") {
            $.extend(true, target, DYShenHePropertygridData);
            return target;
        }
        if (prefix == "TCWT_") {
            $.extend(true, target, TCWTPropertygridData);
            return target;
        }
        if (prefix == "TCWTSH_") {
            $.extend(true, target, TCWTShenHePropertygridData);
            return target;
        }
        if (prefix == "JJSL_") {
            $.extend(true, target, JJSLPropertygridData);
            return target;
        }
        if (prefix == "JJSLSH_") {
            $.extend(true, target, JJSLShenHePropertygridData);
            return target;
        }
        if (prefix == "XSH_") {
            $.extend(true, target, XSHPropertygridData);
            return target;
        }
        if (prefix == "XSHSH_") {
            $.extend(true, target, XSHShenHePropertygridData);
            return target;
        }
        if (prefix == "SF_") {
            $.extend(true, target, SJSFPropertygridData);
            return target;
        }
        if (prefix == "SJSFSH_") {
            $.extend(true, target, SJSFShenHePropertygridData);
            return target;
        }
        if (prefix == "SY_") {
            $.extend(true, target, SJSYPropertygridData);
            return target;
        }
        if (prefix == "SJSYSH_") {
            $.extend(true, target, SJSYShenHePropertygridData);
            return target;
        }
        if (prefix == "DBFX_") {
            $.extend(true, target, DBFXPropertygridData);
            return target;
        }
        if (prefix == "DBFXSH_") {
            $.extend(true, target, DBFXShenHePropertygridData);
            return target;
        }
        if (prefix == "DCJL_") {
            $.extend(true, target, DCJLPropertygridData);
            return target;
        }
        if (prefix == "DCJLSH_") {
            $.extend(true, target, DCJLShenHePropertygridData);
            return target;
        }
        if (prefix == "LWZX_") {
            $.extend(true, target, LWZXPropertygridData);
            return target;
        }
        if (prefix == "LWZXSH_") {
            $.extend(true, target, LWZXShenHePropertygridData);
            return target;
        }
    },

    propertygridEndEdit: function () {
        var row = $("#pg").propertygrid("getSelected");
        var editIndex = $("#pg").propertygrid('getRowIndex', row);
        if ($("#pg").propertygrid('validateRow', editIndex)) {
            $("#pg").propertygrid('endEdit', editIndex);
        }
    },

    selectNearestOverNo: function (beginIndex, id) {
        var rows = $("#pg").propertygrid('getRows');
        for (var i = beginIndex; i >= 0; i--) {
            if (rows[i].id == id) {
                return rows[i].value;
            }
        }
        return "";
    },

    getRowIndexById: function (id) {
        var rows = $("#pg").propertygrid('getRows');
        for (var i = 0; i < rows.length; i++) {
            if (rows[i].id == id) {
                return i;
            }
        }
    },

    getRowIndexByIdAndIndex: function (id, index) {
        var rows = $("#pg").propertygrid('getRows');
        for (var i = 0; i < rows.length; i++) {
            if (rows[i].id == id && rows[i].index == index) {
                return i;
            }
        }
    }
}