
    var BS_ = {
        //显示抄送详情
        xianShiDoc:function (rowIndex) {
            $('#divDocDialog').dialog('open');
            var tab = $('#tt').tabs('getSelected');
            var index = $('#tt').tabs('getTabIndex', tab);
            switch (index) {
                case 0:
                    $('#divRec').html(ccList_UnRead.rows[rowIndex].Rec);
                    $('#divTitle').html(ccList_UnRead.rows[rowIndex].Title);
                    $('#divDoc').html(ccList_UnRead.rows[rowIndex].Doc);
                    break;
                case 1:
                    $('#divRec').html(ccList_All.rows[rowIndex].Rec);
                    $('#divTitle').html(ccList_All.rows[rowIndex].Title);
                    $('#divDoc').html(ccList_All.rows[rowIndex].Doc);
                    break;
                case 2:
                    $('#divRec').html(ccList_Read.rows[rowIndex].Rec);
                    $('#divTitle').html(ccList_Read.rows[rowIndex].Title);
                    $('#divDoc').html(ccList_Read.rows[rowIndex].Doc);
                    break;
                case 3:
                    $('#divRec').html(ccList_Delete.rows[rowIndex].Rec);
                    $('#divTitle').html(ccList_Delete.rows[rowIndex].Title);
                    $('#divDoc').html(ccList_Delete.rows[rowIndex].Doc);
                    break;
                default:
                    break;
            }

        },

        //关闭显示
        guanBiXianShi:function () {
            $('#divDocDialog').dialog('close');
            var tab = $('#tt').tabs('getSelected');
            var index = $('#tt').tabs('getTabIndex', tab);
            //0代表未读，1代表已读
            if (index == 0 || index == 1) {
                var data = {};
                if (index == 0) {
                    var rowIndex = $('#dg_unread').datagrid("getRowIndex", $('#dg_unread').datagrid("getSelected"));
                    if (rowIndex != -1)
                        data.MyPK = ccList_UnRead.rows[rowIndex].MyPK;
                } else {
                    var rowIndex = $('#dg_all').datagrid("getRowIndex", $('#dg_all').datagrid("getSelected"));
                    if (rowIndex != -1)
                        data.MyPK = ccList_All.rows[rowIndex].MyPK;
                }
                $.post("/ChaoSong/CCYiYue", data, BS_.reloadCallback);
            }
        },

        //重新加载回调函数
        reloadCallback:function () {
            var tab = $('#tt').tabs('getSelected');
            var index = $('#tt').tabs('getTabIndex', tab);
            window.location.href = "/ChaoSong/ChaoSong?tabIndex=" + index;
        },

        //删除
        shanChu: function (mypk) {
            $.messager.confirm("删除", "您确定删除该抄送内容吗？", function (data) {
                if (data) {
                    var deldata = { MyPK: mypk };
                    $.post("/ChaoSong/CCShanChu", deldata, BS_.reloadCallback);
                }
            });
        },

        //彻底删除
        cheDiShanChu: function (mypk) {
            $.messager.confirm("彻底删除", "您确定彻底删除该抄送内容吗？", function (data) {
                if (data) {
                    var deldata = { MyPK: mypk };
                    $.post("/ChaoSong/CCCheDiShanChu", deldata, BS_.reloadCallback);
                }
            });
        },




        onSelectUnreadPage: function (pageNumber, pageSize) { 
            _OriginUnreadArray = { total: 0, rows: [] };
            if (ccList_UnRead != undefined && ccList_UnRead.rows != undefined) {
                var index = 0;
                for (var i = (pageNumber - 1) * pageSize; i < (pageNumber - 1) * pageSize + pageSize; i++) {
                    if (ccList_UnRead.rows[i] != undefined) {
                        _OriginUnreadArray.rows[index++] = ccList_UnRead.rows[i];
                    }
                }
            }
            $('#dg_unread').datagrid({
                data: cloneJSON(_OriginUnreadArray),
            });
            $('#dg_unread').datagrid("getPager").pagination({
                pageSize: pageSize,
                pageNumber: pageNumber,
                total: ccList_UnRead.rows.length,
                onSelectPage: BS_.onSelectUnreadPage
            });
        },

        onSelectAllPage: function (pageNumber, pageSize) {
            _OriginAllArray = { total: pageSize, rows: [] };
            if (ccList_All != undefined && ccList_All.rows != undefined) {
                var index = 0;
                for (var i = (pageNumber - 1) * pageSize; i < (pageNumber - 1) * pageSize + pageSize; i++) {
                    if (ccList_All.rows[i] != undefined) {
                        _OriginAllArray.rows[index++] = ccList_All.rows[i];
                    }
                }
            }
            $('#dg_all').datagrid({
                data: cloneJSON(_OriginAllArray),
            });
            $('#dg_all').datagrid("getPager").pagination({
                pageSize: pageSize,
                pageNumber: pageNumber,
                total: ccList_All.rows.length,
                onSelectPage: BS_.onSelectAllPage
            });
        },

        onSelectReadPage: function (pageNumber, pageSize) {
            _OriginReadArray = { total: pageSize, rows: [] };
            if (ccList_Read != undefined && ccList_Read.rows != undefined) {
                var index = 0;
                for (var i = (pageNumber - 1) * pageSize; i < (pageNumber - 1) * pageSize + pageSize; i++) {
                    if (ccList_Read.rows[i] != undefined) {
                        _OriginReadArray.rows[index++] = ccList_Read.rows[i];
                    }
                }
            }
            $('#dg_read').datagrid({
                data: cloneJSON(_OriginReadArray),
            });
            $('#dg_read').datagrid("getPager").pagination({
                pageSize: pageSize,
                pageNumber: pageNumber,
                total: ccList_Read.rows.length,
                onSelectPage: BS_.onSelectReadPage
            });
        },

        onSelectDeletePage: function (pageNumber, pageSize) {
            _OriginDeleteArray = { total: pageSize, rows: [] };
            if (ccList_Delete != undefined && ccList_Delete.rows != undefined) {
                var index = 0;
                for (var i = (pageNumber - 1) * pageSize; i < (pageNumber - 1) * pageSize + pageSize; i++) {
                    if (ccList_Delete.rows[i] != undefined) {
                        _OriginDeleteArray.rows[index++] = ccList_Delete.rows[i];
                    }
                }
            }
            $('#dg_delete').datagrid({
                data: cloneJSON(_OriginDeleteArray),
            });
            $('#dg_delete').datagrid("getPager").pagination({
                pageSize: pageSize,
                pageNumber: pageNumber,
                total: ccList_Delete.rows.length,
                onSelectPage: BS_.onSelectDeletePage
            });
        }
    }