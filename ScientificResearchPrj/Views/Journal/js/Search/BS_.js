 
    var BS_ = {
        onLoad: function () {
             
            var pageopt = $('#tTable').datagrid('getPager').data("pagination").options;
            var pageVal = {
                "pageNow": pageopt.pageNumber == 0 ? 1 : pageopt.pageNumber,
                "pageSize": pageopt.pageSize,
                "RptNo": RptNo,
                "random" : new Date().getTime()
            }

            $('#tTable').datagrid("loading");
            AT_.AjaxPost("/Journal/SearchRpt", pageVal, BS_.onLoadSuccess);
        },

        onLoadSuccess:function(data, status){
            $.messager.show({
                title: '加载发起列表',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });  
            if (data.state == "0") {
                jsonResult = parseJSON(data._Json);
                jsonData = cloneJSON(jsonResult);

                if (jsonData != null && jsonData.rows != null) {
                    for (var i = 0; i < jsonData.rows.length; i++) {
                        var diff =  GetDateDiff(jsonData.rows[i]["FlowEnderRDT"], jsonData.rows[i]["FlowStartRDT"]).toString();
                        diff=diff.substr(0, diff.indexOf(".") + 3);
                        jsonData.rows[i]["KuaDu"] = diff;

                        if (currentLoginUser == jsonData.rows[i]["FlowStarter"] || jsonData.rows[i]["FlowEmps"].indexOf(currentLoginUser) != -1) {
                            jsonData.rows[i]["Title"] = "<a onclick=javascript:BS_.readTrack(" + jsonData.rows[i]["OID"] + ","+jsonData.rows[i]["FID"] +")>" + jsonData.rows[i]["Title"] + "</a>"
                        }
                    }
                }
                //使用复制数据的原因是：改变表格数据的时候，例如appendRow会改变与表格相关联的json数据，而在判断表格与原始数据是否有更动的时候，这种情况将会导致很多麻烦
                $('#tTable').datagrid({ data: jsonData });
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
        },

        readTrack: function (oid,fid) {
            window.open("/Journal/ReadTrackView?FK_Flow=" + FK_Flow + "&WorkID=" + oid + "&FID=" + fid + "&random=" + new Date().getTime());
        }
    }