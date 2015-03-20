
    var editIndex = undefined;
    var ajaxResult = undefined;
    var newRow = {};

    var BS_ = {
        onLoad: function () {
            $('#tTable').treegrid("loading");

            AT_.AjaxPost("/Department/GetDepartments", null, BS_.onLoadSuccess);
        },

        onLoadSuccess:function(data, status){
            $.messager.show({
                title: '加载部门树',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            if (data.state == "0") {
                jsonResult = parseJSON(data._Json);
                jsonData = cloneJSON(jsonResult);
                
                //使用复制数据的原因是：改变表格数据的时候，例如appendRow会改变与表格相关联的json数据，而在判断表格与原始数据是否有更动的时候，这种情况将会导致很多麻烦
                $('#tTable').treegrid({ data: jsonData });
            }
            $('#tTable').treegrid("loaded");
            editIndex = undefined;
        },




        endEditing: function() {
            if (editIndex == undefined) { return true }
             
            $('#tTable').treegrid('endEdit', editIndex);
 
            //人工判断是否改变数据
            if (BS_.isChange()) {
                //保存
                BS_.save();

                //添加或者修改失败
                if (ajaxResult == false || ajaxResult==undefined) {
                    $('#tTable').treegrid('select', editIndex)
                            .treegrid('beginEdit', editIndex);
                    return false;
                } else {
                    ajaxResult = false;
                }
            }
            editIndex = undefined;
            return true;
            
        },
        


        isChange: function () {
           // var row = BS_.getNodeFromTreeGridByTreeId($('#tTable').treegrid('getData'), editIndex);
            var row = $('#tTable').treegrid('find', editIndex);

            var targetRow = undefined;
            //新增的数据，会因为表格（已acceptChange）长度大于jsonResult长度而获取失败
            targetRow = BS_.getNodeFromJsonResultByTreeId(editIndex);
           
            if (targetRow == undefined) return true;//新增的数据
            //根据表格行内容判断
            if (row.DeptNo != targetRow.DeptNo) return true;
            if (row.Name != targetRow.Name) return true;
            if (row.Description != targetRow.Description) return true;
            return false;
        },
        save:function() {
            //新增数据则添加，修改数据则更新
            var tableCount = BS_.getCountFromTreeGrid($('#tTable').treegrid('getData'));
             
            //表格长度大于jsonResult长度，新增数据
            //表格长度等于jsnResult长度，修改数据
            if (tableCount >= jsonResult.rows.length) {
               // var row = BS_.getNodeFromTreeGridByTreeId($('#tTable').treegrid('getData'), editIndex);
                var row = $('#tTable').treegrid('find', editIndex);

                newRow = {};
                newRow["DeptNo"] = row.DeptNo;
                newRow["Name"] = row.Name;
                newRow["Description"] = row.Description;
                newRow["ParentNo"] = row.ParentNo;
                //TreeId不需要，添加成功以后再更新  

                if (row.DeptNo == undefined || row.DeptNo=="") {
                    $.messager.show({
                        title: '信息提示',
                        msg: "编号不能为空",
                        timeout: 3000,
                        showType: 'show'
                    });
                    return;
                }


                if (tableCount > jsonResult.rows.length) {
                    AT_.AjaxPost("/Department/TianJiaBuMen", newRow, BS_.addSuccess);
                } else {
                    var mRow = cloneJSON(newRow);
                    var rowFromJsonResult = BS_.getNodeFromJsonResultByTreeId(editIndex);
                    //不破坏newRow的结构
                    mRow["oldNo"] = rowFromJsonResult.DeptNo;
                    AT_.AjaxPost("/Department/XiuGaiBuMen", mRow, BS_.modifySuccess);
                }
            }
        },
        addSuccess: function(data, status) {
            $.messager.show({
                title: '添加部门',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            if (data.state == "0") {
                //更新TreeId
                newRow.TreeId = newRow.DeptNo + (new Date()).Format("yyyyMMddhhmmssS");
                //更新_parentId
                //var oldRow = BS_.getNodeFromTreeGridByTreeId($('#tTable').treegrid('getData'), editIndex);
                var oldRow = $('#tTable').treegrid('find', editIndex);
                newRow._parentId = oldRow._parentId;
               
                jsonResult.rows.push(newRow);

                //$('#tTable').treegrid('getData')的treeId也要更新,此时选择重新加载
                jsonData = cloneJSON(jsonResult);
                $('#tTable').treegrid('loadData', jsonData);
                 
                //以下乃特殊做法
                //此举是因为连续appendRow的时候，若不先选择一下，第二次的父部门（即当前选中部门）TreeId无法更新到
                $('#tTable').treegrid('select', newRow.TreeId);

                ajaxResult = true;
            } else {
                ajaxResult = false;
            }
        },
        modifySuccess: function(data, status) {
            $.messager.show({
                title: '修改部门',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            if (data.state == "0") {
                //更新TreeId
                newRow.TreeId = newRow.DeptNo + (new Date()).Format("yyyyMMddhhmmssS");
                //更新_parentId
                // var currentRow = BS_.getNodeFromTreeGridByTreeId($('#tTable').treegrid('getData'), editIndex);
                var currentRow = $('#tTable').treegrid('find', editIndex);
                newRow._parentId = currentRow._parentId;

                var oldRow = BS_.getNodeFromJsonResultByTreeId(editIndex);
                //修改子节点的_parentId,并返回本节点的下标
                var oldRowIndex = BS_.modifyParentIdFromJsonResult(oldRow.TreeId, newRow.TreeId);
                //修改子节点的ParentNo
                BS_.modifyParentNoFromJsonResult(oldRow.DeptNo, newRow.DeptNo);

                jsonResult.rows.splice(oldRowIndex, 1, newRow);

                //应该更新$('#tTable').treegrid('getData')的treeId,及其他数据，此时选择重新加载
                jsonData = cloneJSON(jsonResult);
                $('#tTable').treegrid('loadData', jsonData);

                
                ajaxResult = true;
            } else {
                ajaxResult = false;
            }
             
        },
       

         

        //递归查询树节点,可使用find替代
        getNodeFromTreeGridByTreeId: function (data, treeid) {
            var result = undefined;
            for (var i = 0; i < data.length; i++) {
                result = BS_.recurGetNodeFromTreeGridByTreeId(data[i], treeid);
                if (result != null) {
                    return result;
                }
            }
            return result;
        },
        recurGetNodeFromTreeGridByTreeId: function (node, treeid) {
            if (node.TreeId == treeid) {
                return node;
            } else {
                if (node.children != undefined) {
                    for (var i = 0; i < node.children.length; i++) {
                        var result = BS_.recurGetNodeFromTreeGridByTreeId(node.children[i], treeid);
                        if (result != null) {
                            return result;
                        }
                    }
                }
                else {
                    return null;
                }
            }
        },


        //递归查询树节点总数
        getCountFromTreeGrid: function (data) {
            var count = 0;
            for (var i = 0; i < data.length; i++) {
                count += BS_.recurGetCountFromTreeGrid(data[i]);
            }
            return count;
        },
        recurGetCountFromTreeGrid: function (node) {
            if (node.children == undefined) {
                return 1;
            }
            else {
                var result = 0;
                for (var i = 0; i < node.children.length; i++) {
                    result += BS_.recurGetCountFromTreeGrid(node.children[i]);
                }
                return result + 1;
            }
        },




        getNodeFromJsonResultByTreeId: function (treeid) { 
            if (jsonResult.rows.length != 0) {
                for (var i = 0; i < jsonResult.rows.length; i++) {
                    if (jsonResult.rows[i].TreeId == treeid) {
                        return jsonResult.rows[i];
                    }
                }
            }
            return undefined;
        },
        getIndexFromJsonResultByTreeId: function (treeid) {
            if (jsonResult.rows.length != 0) {
                for (var i = 0; i < jsonResult.rows.length; i++) {
                    if (jsonResult.rows[i].TreeId == treeid) {
                        return i;
                    }
                }
            }
            return -1;
        },



        //更新子节点的_parentId,返回TreeId=oldTreeId的下标
        modifyParentIdFromJsonResult: function (oldTreeId, newTreeId) {
            var index = -1;
            for (var i = 0; i < jsonResult.rows.length; i++) {
                if (jsonResult.rows[i].TreeId == oldTreeId) {
                    index = i;
                }
                if (jsonResult.rows[i]._parentId == oldTreeId) {
                    jsonResult.rows[i]._parentId = newTreeId;
                }
            }
            return index;
        },
        //更新子节点的ParentNo 
        modifyParentNoFromJsonResult: function (oldParentNo, newParentNo) {
            for (var i = 0; i < jsonResult.rows.length; i++) {
                if (jsonResult.rows[i].ParentNo == oldParentNo) {
                    jsonResult.rows[i].ParentNo = newParentNo;
                }
            }
        },





        remove: function() {
            if (editIndex == undefined) return;
            var children = $('#tTable').treegrid('getChildren', editIndex);
            if (children.length != 0) {
                $.messager.show({
                    title: '信息提示',
                    msg: "请先删除子部门",
                    timeout: 3000,
                    showType: 'show'
                });
                return;
            }

            var tableCount = BS_.getCountFromTreeGrid($('#tTable').treegrid('getData'));

            //表格长度大于jsonResult.rows长度，说明是新的未保存数据，直接删除表格内容即可
            if (tableCount > jsonResult.rows.length) {
                // alert("数据尚未保存，直接删除表格数据");
                $('#tTable').treegrid('remove', editIndex);
                editIndex = undefined;
                return;
            } else {
                //var currentRow = BS_.getNodeFromTreeGridByTreeId($('#tTable').treegrid('getData'), editIndex);
                var currentRow = $('#tTable').treegrid('find', editIndex);
                $.messager.confirm("删除部门", "您确定删除 【" + currentRow.Name + "】 吗？", function (data) {
                    if (data) {
                        var targetRow = BS_.getNodeFromJsonResultByTreeId(editIndex);
                        var delDeptNo = {
                            //使用jsonResult获取而不是表格，因为可能编辑了一半后直接删除
                            "deptNo": targetRow.DeptNo
                        }
                        
                        AT_.AjaxPost("/Department/ShanChuBuMen", delDeptNo, BS_.deleteSuccess);
                    }
                });
            }
            
        },
        deleteSuccess: function(data, status) {
            $.messager.show({
                title: '删除部门',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });
            if (data.state == "0") {
                $('#tTable').treegrid('remove', editIndex);
                var delIndex = BS_.getIndexFromJsonResultByTreeId(editIndex);
                jsonResult.rows.splice(delIndex, 1);
                 
                editIndex = undefined;
            }
        },




        appendRow: function () {
            if (BS_.endEditing()) { 
                //实验证明，当连续两次appendRow，BS_.endEditing()在loadData更新数据后，
                //$('#tTable').treegrid('getSelected')的TreeId并没有更新
                //故需要在BS_.endEditing()loadData更新数据后,将当前行选中一下
                var parentNode = $('#tTable').treegrid('getSelected');
                
                if (parentNode == undefined || parentNode==null) {
                    $.messager.confirm("添加部门", "未选择上一级部门，是否在【根】上创建新部门？", function (data) {
                        if (data == false) { return; }
                        else {
                            parentNode = {};
                            parentNode.TreeId = "";
                            parentNode.DeptNo = 0;
                            BS_.appendRowData(parentNode);
                        }
                    });
                }
                else {
                    BS_.appendRowData(parentNode);
                }
            }
        },

        appendRowData: function (parentNode) {
            var newDeptNo = "Dept_";
            var newTreeId = newDeptNo + (new Date()).Format("yyyyMMddhhmmssS");
            //此处点击速度太快的话会导致parentNode.TreeId与newTreeId相同，出错
          //  console.log("first:" + parentNode.TreeId)
          //  console.log("first:" + newTreeId)
            $('#tTable').treegrid('append', {
                parent: parentNode.TreeId,
                data: [{
                    TreeId: newTreeId,
                    DeptNo: newDeptNo,
                    Name: "部门",
                    ParentNo: parentNode.DeptNo
                }]
            }); console.log("end")
            editIndex = newTreeId; 
            $('#tTable').treegrid('select', editIndex)
                    .treegrid('beginEdit', editIndex); 
        },

        unselect: function () {
            if (BS_.endEditing()) {
                $('#tTable').treegrid('unselectAll');
                editIndex = undefined;
            }
        },


        onClickRow: function (row) {
            if (editIndex != row.TreeId) {
                if (BS_.endEditing()) {
                    $('#tTable').treegrid('showLines');

                    var selectedId = row.TreeId;
                    $('#tTable').treegrid('select', selectedId)
                        .treegrid('beginEdit', selectedId);
                    editIndex = selectedId;
                } else {
                    $('#tTable').treegrid('select', editIndex);
                }
            }
        } 
    }