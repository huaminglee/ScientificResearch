
    var BS_ = {
        //显示抄送详情
        xianShiDoc:function (rowIndex) {
            $('#divDocDialog').dialog('open');
            var tab = $('#tt').tabs('getSelected');
            var index = $('#tt').tabs('getTabIndex', tab);
            switch (index) {
                case 0:
                    $('#divTitle').html(ccList_UnRead.rows[rowIndex].Title)
                    $('#divDoc').html(ccList_UnRead.rows[rowIndex].Doc);
                    break;
                case 1:
                    $('#divTitle').html(ccList_All.rows[rowIndex].Title)
                    $('#divDoc').html(ccList_All.rows[rowIndex].Doc);
                    break;
                case 2:
                    $('#divTitle').html(ccList_Read.rows[rowIndex].Title)
                    $('#divDoc').html(ccList_Read.rows[rowIndex].Doc);
                    break;
                case 3:
                    $('#divTitle').html(ccList_Delete.rows[rowIndex].Title)
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
        }
    }