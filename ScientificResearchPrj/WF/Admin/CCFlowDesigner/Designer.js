
var Action_NodeP = "NodeP";/// 节点属性
var Action_NodeStation = "NodeStation";/// 节点岗位
var Action_Dir = "Dir";/// 设置方向条件
var Action_MapDefFixModel = "MapDefFixModel";/// 节点表单设计-傻瓜
var Action_MapDefFreeModel = "MapDefFreeModel";/// 节点表单设计-自由
var Action_FormFixModel = "FormFixModel";/// 表单设计-傻瓜
var Action_FormFreeModel = "FormFreeModel";/// 表单设计-自由
var Action_FrmLib = "FrmLib";/// 表单库
var Action_Save="Save";
var Action_Login="Login";
var Action_ToolBox="ToolBox";
var Action_FlowSortP = "FlowSortP";/// 目录权限
var Action_FlowP = "FlowP";/// 流程属性
var Action_FlowRun = "FlowRun";/// 运行流程
var Action_FlowCheck = "FlowCheck";/// 流程检查
var Action_FlowRpt = "FlowRpt";/// 报表定义
var Action_FlowFrms = "FlowFrms";/// 流程表单
var Action_FlowDel="FlowDel";
var Action_FlowNew="FlowNew";
var Action_FlowExp="FlowExp";
var Action_FlowOpen="FlowOpen";
var Action_Help="Help";

var WindowModel_Dialog = 0;
var WindowModel_Window = 1;
var WindowModel_Max = 2;
/* the current flowid 
,which changed when selected from flowtree or tabpage selectionchanged
*/
var fk_flow = ""; 
var BPMHost = window.location.protocol + "//" + window.location.hostname + ":" + window.location.port;

function load() {
    $.ajax({
        type: "Post",
        contentType: "application/json;utf-8",
        url: "../XAP/WebService.asmx/GetFlowTree",
        dataType: "json",
        success: function (re) {
            jdata = $.parseJSON(re.d);
            if (jdata) {
                $('#flowTree').tree({
                    data: jdata
                });
            }
            else {
            }
        },
        error: function (re) {
            alert(re.responseText);
        }
    });

    $.ajax({
        type: "Post",
        contentType: "application/json;utf-8",
        url: "../XAP/WebService.asmx/GetFormTree",
        dataType: "json",
        success: function (re) {
            jdata = $.parseJSON(re.d);
            if (jdata) {
                $('#formTree').tree({
                    data: jdata
                });
            }
            else {
            }
        },
        error: function (re) {
            alert(re.responseText);
        }
    });
}
function treeAction() {

    $('#flowTree').tree({

        onDblClick: function (node) {
            if (node.attributes.IsParent == '0') {
                openFlow(node.text, "Designer.htm?FK_Flow=" + fk_flow);
            }
        }
        ,onContextMenu: function (e, node) {
            e.preventDefault();
            $('#flowTree').tree('select', node.target);

            var div = null;
            if (node.attributes.IsRoot == '1') {
                div = $('#mFlowRoot');
            }
            if (node.attributes.IsParent == '1') {
                div = $('#mFlowSort');
            } else {
                div = $('#mFlow');
            };

            div.menu('show', {
                left: e.pageX,
                top: e.pageY
            });
        }
    });

    $('#formTree').tree({
        onDblClick: function (node) {
            if (node.attributes.IsParent == '0') {
                // TO DO : open the form in new page in browser
                alert("will open one form on in new page in browser");
            }
        }
        ,onContextMenu: function (e, node) {
            e.preventDefault();
            $('#flowTree').tree('select', node.target);
           
            var div = null;
            if (node.attributes.IsRoot == '1') {
                div=$('#mFormRoot');
            }
            if (node.attributes.IsParent == '1') {
                div = $('#mFormSort');
            } else {
                div = $('#mForm');
            };

            div.menu('show', {
                left: e.pageX,
                top: e.pageY
            });
        }
    });
}
function openFlow(title, url, flowId) {
    fk_flow = flowId;
    if ($('#designerArea').tabs('exists', title)) {
        $('#designerArea').tabs('select', title);
    } else {
        var content = '<iframe scrolling="auto" frameborder="0"  src="' + url + '" style="width:100%;height:100%;"></iframe>';
        $('#designerArea').tabs('add', {
            title: title,
            content: content,
            closable: true
        });
    }
}
 function maximizeWindow() {
    window.moveTo(0, 0)
    window.resizeTo(screen.width, window.screen.availHeight)
 };
 
 function action(w) {
   switch (w) {
        case Action_Help:
              OpenWindow("http://online.ccflow.org/", "");
              break;
          case Action_Login: // 登录。
              var url = "/AppDemoLigerUI/Login.aspx?DoType=Logout";
              OpenWindow(url, "登录", 850, 990);
              break;
          case Action_FlowRpt:
            
              url = "/WF/Admin/XAP/DoPort.aspx?RefNo=" + fk_flow + "&DoType=WFRpt&Lang=CH&PK=" + fk_flow;
              OpenDialog(url, "流程报表", 850, 990);
              break;
          case Action_FlowP: // 节点属性与流程属性。
              url = "/WF/Admin/XAP/DoPort.aspx?DoType=En&EnName=BP.WF.Flow&PK=" + fk_flow + "&Lang=CH";
              OpenDialog(url, "", 500, 400);
              break;
          case Action_FlowRun:
              url = "/WF/Admin/TestFlow.aspx?FK_Flow=" + fk_flow + "&Lang=CH";
              OpenWindow(url, "运行流程", 850, 990);
              break;
          case Action_FlowCheck:
              url = "/WF/Admin/DoType.aspx?RefNo=" + fk_flow + "&DoType=" + dotype + "&Lang=CH";
              OpenWindow(url, "运行流程", 850, 990);
              break;
         
     
    }
};

function openFlow(title, url) {
    if ($('#designerArea').tabs('exists', title)) {
        $('#designerArea').tabs('select', title);
    } else {
        var content = '<iframe scrolling="auto" frameborder="0"  src="' + url + '" style="width:100%;height:100%;"></iframe>';
        $('#designerArea').tabs('add', {
            title: title,
            content: content,
            closable: true
        });
    }
};

function OpenWinByDoType(lang, dotype, fk_flow, node1, node2){
    var url = "";
    switch (dotype) {
        case Action_NodeStation:
            url = "/WF/Admin/XAP/DoPort.aspx?DoType=NodeStation&PK=" + node1 + "&Lang=CH";
            OpenDialog( url, "执行", 500, 400);
            break;
        case Action_FrmLib:
            url = "/WF/Admin/XAP/DoPort.aspx?DoType=FrmLib&FK_Flow=" + fk_flow + "&FK_Node=0&Lang=CH";
            OpenWindow( url, "执行", 800, 760);
            break;
        case Action_FlowFrms:
            url = "/WF/Admin/XAP/DoPort.aspx?DoType=FlowFrms&FK_Flow=" + fk_flow + "&FK_Node="+node1+"&Lang=CH";
            OpenWindow( url, "执行", 800, 760);
            break;
        case Action_FlowSortP:
            url = "/WF/Admin/XAP/DoPort.aspx?DoType=En&EnName=BP.WF.FlowSort&PK=" + node1 + "&Lang=CH";
            OpenDialog( url, "执行", 600, 500);
            break;
        case Action_NodeP:
            url = "/WF/Admin/XAP/DoPort.aspx?DoType=En&EnName=BP.WF.Node&PK=" + node1 + "&Lang=CH";
            OpenDialog( url, "执行", 600, 500);
            break;
    
        case Action_MapDefFixModel: // SDK表单设计。
            url = "/WF/Admin/XAP/DoPort.aspx?DoType=MapDefFixModel&FK_MapData=ND" + node1 + "&FK_Node=" + node1 + "&Lang=CH&FK_Flow=" + fk_flow;
            OpenDialog(url, "节点表单设计");
            break;
        case Action_MapDefFreeModel: // 自由表单设计。

            var fk_MapData = "ND" + node1;
            var title = "表单ID: {0} 存储表:{1} 名称:{2}";
            title = string.Format(title, fk_MapData, fk_MapData, node2);
           
            url = "/WF/Admin/XAP/DoPort.aspx?DoType=MapDefFreeModel&FK_MapData="
                + fk_MapData + "&FK_Node=" + node1 + "&Lang=CH&FK_Flow=" + fk_flow;
           
                  
            break;
        case Action_FormFixModel: // 节点表单设计。
            url = "/WF/Admin/XAP/DoPort.aspx?DoType=MapDefFixModel&FK_MapData=" + fk_flow;
                 
            OpenDialog( url, "节点表单设计");
            break;
        case Action_FormFreeModel: // 节点表单设计。
            url = "/WF/Admin/XAP/DoPort.aspx?DoType=MapDefFreeModel&FK_MapData=" + fk_flow;
            OpenDialog( url, "节点表单设计");
            break;
        case Action_Dir: // 方向条件。
            url = "/WF/Admin/ConditionLine.aspx?FK_Flow=" + fk_flow + "&FK_MainNode=" + node1 + "&FK_Node=" + node1 + "&ToNodeID=" + node2 + "&CondType=2&Lang=CH";
            OpenDialog( url, "方向条件", 550, 500);
            break;
       
     
        default:
            alert("没有判断的url执行标记:" + dotype);
            break;
    }
};

var dgId = "dgFrame";
function callback() {
    var innerWin = document.getElementById(dgId).contentWindow;
    $('#' + txtId).val(innerWin.getReturnText());
    $('#' + hiddenId).val(innerWin.getReturnValue());

};
function OpenDialog(url, title, h, w, callBack) {
    //    OpenWindowOrDialog(url, title,  WindowModel_Dialog,h,w);
    OpenEasyUiDialog(url, dgId, '选择人员', h, w, 'icon-user', true, callBack);
};


 function OpenMax(url, title){
    OpenWindowOrDialog(url, title, WindowModel_Max);
};

function OpenWindow(url, title, h, w ){
    OpenWindowOrDialog(url, title, WindowModel_Window,h,w);
};

function OpenWindowOrDialog(url, title, windowModel
    , height, width , left, top, resizable){

    if(height == undefined || height ==null) height= 0;
    if(width == undefined || width ==null) width= 0;
    if(left == undefined || left ==null) left= 0;
    if(top == undefined || top ==null) top= 0;
    if(resizable == undefined || resizable ==null) resizable= true;
    
    if (url.indexOf("ttp://") == -1 && url.indexOf("http") ==-1 )
        url = BPMHost + url;

    try {
        if (windowModel == WindowModel_Dialog) {
           
        }
        else if (windowModel == WindowModel_Max)  {
            var parms = "left=0,top=0,height={0},width={1},resizable={2},scrollbars=yes,help=no,toolbar=no,menubar=no,scrollbars=yes,status=yes,location=no";
            width = 0 < width ? width : window.screen.width;
            height = 0 < height ? height : window.screen.height-100;// 系统任务栏高度？？
            var resize = resizable ? "yes" : "no";
            parms = parms.format( height, width, resize);
            window.open(url, title, parms);
        }
        else  {
            if (0 < height && 0 < width)  {
                var parms = 'height={0},width={1},resizable=yes,help=no,toolbar =no, menubar=no, scrollbars=yes,status=yes,location=no';
                parms = parms.format( height, width);
                window.open(url, title, parms);
            }
            else
                window.open(url,'_blank');
        }
    }
    catch (e) {
    }
};


String.prototype.format = function (args) {
    var result = this;
    if (arguments.length > 0) {
        if (arguments.length == 1 && typeof (args) == "object") {
            for (var key in args) {
                if (args[key] != undefined) {
                    var reg = new RegExp("({" + key + "})", "g");
                    result = result.replace(reg, args[key]);
                }
            }
        }
        else {
            for (var i = 0; i < arguments.length; i++) {
                if (arguments[i] != undefined) {
                    //var reg = new RegExp("({[" + i + "]})", "g");//这个在索引大于9时会有问题，谢谢何以笙箫的指出
                    var reg = new RegExp("({)" + i + "(})", "g");
                    result = result.replace(reg, arguments[i]);
                }
            }
        }
    }
    return result;
}

