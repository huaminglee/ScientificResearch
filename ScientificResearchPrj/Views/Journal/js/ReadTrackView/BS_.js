    var editIndex = -1;
    var BS_ = {
        onLoad: function () {
            var val = {
                "FK_Flow":FK_Flow,
                "WorkID": WorkID,
                "FID": FID 
            }

            $('#tTable').datagrid("loading");
            AT_.AjaxPost("/Journal/ReadTrack", val, BS_.onLoadSuccess);
        },

        onLoadSuccess:function(data, status){ 
            $.messager.show({
                title: '加载流程日志列表',
                msg: data.message,
                timeout: 3000,
                showType: 'show'
            });  
            if (data.state == "0") {
                jsonResult = parseJSON(data._Json);
                jsonData = cloneJSON(jsonResult);
                if (jsonData != null && jsonData.rows != null) {
                    for (var i = 0; i < jsonData.rows.length; i++) { 
                        jsonData.rows[i]["FormLink"] = "<a onclick=javascript:BS_.openForm('" + jsonData.rows[i]["FormLink"] + "')>" + "表单" + "</a>";
                        jsonData.rows[i]["ChartLink"] = "<a onclick=javascript:BS_.openChart(" + jsonData.rows[i]["ChartLink"] + ")>" + "流程图" + "</a>";
                        if (i != 0) {
                            jsonData.rows[i]["ArriveTime"] = jsonData.rows[i - 1]["RDT"];

                            var diff = GetDateDiff(jsonData.rows[i]["RDT"], jsonData.rows[i]["ArriveTime"]).toString();
                            diff = diff.substr(0, diff.indexOf(".") + 3);
                            jsonData.rows[i]["TimeSpan"] = diff;
                        }
                    }
                }
                $('#tTable').datagrid({ data: jsonData });
            } else {
                jsonResult = { total: 0, rows: [] };
                $('#tTable').datagrid("loadData", cloneJSON(jsonResult));
            }
             
            $('#tTable').datagrid("loaded");
        },

        openForm: function (link) {
            window.open(link + "&random=" + new Date().getTime());
        },

        openChart: function (link) {
            window.open(link + "&random=" + new Date().getTime());
        },

        onClickRow: function (index) {
            if (editIndex != -1) {
                $('#tTable').datagrid('endEdit', editIndex)
            }
            editIndex = index;
            $('#tTable').datagrid('selectRow', index)
                    .datagrid('beginEdit', index);
        }
    }