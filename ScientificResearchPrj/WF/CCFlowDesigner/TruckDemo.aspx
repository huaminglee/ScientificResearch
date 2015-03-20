<%@ Page Title="" Language="C#" MasterPageFile="~/WF/CCFlowDesigner/Site.Master"
    AutoEventWireup="true" CodeBehind="TruckDemo.aspx.cs" Inherits="CCFlow.WF.CCFlowDesigner.TruckDemo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head1" runat="server">
    <link href="../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/Raphael/raphael.js" type="text/javascript"></script>
    <script src="../Scripts/Raphael/raphael.ccflow.js" type="text/javascript"></script>
    <link href="../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <style type="text/css">
        #holder
        {
            width: 1000px;
            height: 600px;
            border: 1px dashed #c0c0c0;
        }
        .menu-sep
        {
            height: 1px;
        }
    </style>
    <script type="text/javascript">
        var r;
        var lastMousePoint = {};

        $(function () {
            var ps = new Params();
            ps.push('fk_flow', '<%=this.FK_Flow %>');
            ps.push('workid', '<%=this.WorkID %>');

            ajaxService('GetFlowTrackJsonData', ps.toJsonDataString(), function (data) {
                var jdata = $.parseJSON(data);
                if (!jdata.success) {
                    alert(jdata.msg);
                }
                else {
                    drawStart(jdata.datas);
                }
            });
        });

        function drawStart(datas) {
            /// <summary>加载时绘制流程图/轨迹</summary>
            /// <param name="datas" Type="Object">绘制的数据，包含flow/nodes/labels/dirs，均为Array</param>

            var rNode, rDir, rLabel, rcnode, rcdir, rclabel, dirPoints, dirPath;

            r = new Raphael('holder', 1000, 600);

            $('#holder').bind('mousedown', function (e) {
                //增加左键点击画布空白处时，取消选中的对象
                if ((isie && e.button == 1) || e.button == 0) {
                    if (r.getElementByPoint(e.pageX, e.pageY) == null) {
                        rflow.clearFocus();
                    }
                }
            }).bind("contextmenu", function (e) {
                rflow.clearFocus();
                lastMousePoint.x = e.pageX;
                lastMousePoint.y = e.pageY;

                $('#flowMenu').menu('show', {
                    left: e.pageX,
                    top: e.pageY
                });

                return false;
            });

            rflow = new RFlow('<%=this.FK_Flow %>');

            var nodeBorderColor = STYLE_NODE_BORDER_COLOR;
            var startNodePosType = 0;
            var endNodePosType = getMaxInArray(datas.nodes, 'NodePosType');
            var isExist;
            $.each(datas.nodes, function () {
                //确定边框颜色
                switch (this.NodePosType) {
                    case startNodePosType:
                        nodeBorderColor = STYLE_FIRST_NODE_BORDER_COLOR;
                        break;
                    case endNodePosType:
                        nodeBorderColor = STYLE_END_NODE_BORDER_COLOR;
                        break;
                    default:
                        nodeBorderColor = STYLE_NODE_BORDER_COLOR;
                        break;
                }

                if (this.Icon == null || this.Icon.length == 0) {
                    this.Icon = STYLE_NODE_DEFAULT_ICON_PATH + STYLE_NODE_DEFAULT_ICON;
                }
                else {
                    if (this.Icon.indexOf('.') == -1) {
                        this.Icon = STYLE_NODE_DEFAULT_ICON_PATH + this.Icon + '.png';
                    }
                    else {
                        this.Icon = STYLE_NODE_DEFAULT_ICON_PATH + this.Icon.substr(this.Icon.lastIndexOf('/') + 1);
                    }

                    isExist = checkUrl(this.Icon);
                    if (isExist == false) {
                        this.Icon = STYLE_NODE_DEFAULT_ICON_PATH + STYLE_NODE_DEFAULT_ICON;
                    }
                }

                rNode = new RNode(this.NodeID, this.Name, this.X - STYLE_NODE_WIDTH / 2, this.Y - STYLE_NODE_HEIGHT / 2);
                rNode.rBorderColor = nodeBorderColor;

                rcnode = r.ccnode(rNode.x, rNode.y, this.Icon, this.Name);
                rcnode.rBorderColor = nodeBorderColor;
                rcnode.nodeid = this.NodeID;
                rcnode.Node = rNode;

                rcnode.rBorder.attr({ "stroke": rcnode.rBorderColor, "stroke-width": STYLE_NODE_BORDER_NORMAL_WIDTH });
                rcnode.rText.attr({ "stroke": STYLE_NODE_FORE_COLOR, "font-size": STYLE_NODE_FONT_SIZE });

                rflow.nodes.push(rcnode);
            });

            //绘制节点的连线
            $.each(datas.dirs, function () {
                rDir = new RDirection(this.Node, this.ToNode, this.DirType, this.IsCanBack);
                rDir.FromNode = rflow.getRaphaelNodeByNodeId(this.Node);
                rDir.ToNode = rflow.getRaphaelNodeByNodeId(this.ToNode);

                if (rDir.FromNode == null || rDir.ToNode == null) return true; //跳出本次循环

                dirPoints = r.getLinePoints(rDir.FromNode.rBorder, rDir.ToNode.rBorder);
                dirPath = r.getArrow(dirPoints.x1, dirPoints.y1, dirPoints.x2, dirPoints.y2, 8);
                rcdir = r.ccdirection(dirPath);
                rcdir.Dir = rDir;

                rflow.dirs.push(rcdir);
            });

            //绘制标签
            $.each(datas.labels, function () {
                rLabel = new RLabel(this.MyPK, this.Name, this.X, this.Y);

                rclabel = r.cclabel(this.X, this.Y, this.Name);
                rclabel.Text = rLabel;

                rclabel.rText.attr({ "stroke": STYLE_LABEL_FORE_COLOR, "font-size": STYLE_LABEL_FONT_SIZE, "text-anchor": "start" });
                rclabel.rText.toFront();

                rflow.labels.push(rclabel);
            });
        }

        function getMaxInArray(arr, propName) {
            /// <summary>获取指定对象数组中指定属性的最大值</summary>
            /// <param name="arr" Type="Array">对象数组</param>
            /// <param name="propName" Type="String">属性名称</param>
            var max = 0;

            $.each(arr, function () {
                for (prop in this) {
                    if (prop == propName && !isNaN(this[prop])) {
                        max = Math.max(max, this[prop]);
                    }
                }
            });

            return max;
        }

        function checkUrl(url) {
            /// <summary>判断远程路径是否可以连接成功</summary>
            /// <param name="url" Type="String">远程路径url</param>
            var isSuccess;

            $.ajax({
                type: 'GET',
                cache: false,   //不下载远程url
                async: false,   //同步
                url: url,
                data: '',
                success: function () {
                    isSuccess = true;
                },
                error: function () {
                    isSuccess = false;
                }
            });

            return isSuccess;
        }

        function RNode(iNodeID, sNodeName, iX, iY) {
            /// <summary>节点</summary>
            /// <param name="iNodeID" Type="Int">节点ID</param>
            /// <param name="sNodeName" Type="String">节点名称</param>
            /// <param name="iX" Type="Int">节点中心点X坐标</param>
            /// <param name="iY" Type="Int">节点中心点Y坐标</param>
            this.id = iNodeID;
            this.name = sNodeName;
            this.x = iX;
            this.y = iY;
            this.icon = '';
            this.nodePosType = 0;
            this.hisToNDs = '';
        }

        function RLabel(sPk, sLabelName, iX, iY) {
            /// <summary>标签</summary>
            /// <param name="sPk" Type="String">MyPk</param>
            /// <param name="sLabelName" Type="String">标签文本</param>
            /// <param name="iX" Type="Int">标签左上角X坐标</param>
            /// <param name="iY" Type="Int">标签左上角Y坐标</param>
            this.mypk = sPk;
            this.name = sLabelName;
            this.x = iX;
            this.y = iY;
        }

        function RDirection(iFromNodeID, iToNodeID, iDirType, iIsCanBack) {
            /// <summary>结点连接线</summary>
            /// <param name="iFromNodeID" Type="Int">开始节点ID</param>
            /// <param name="iToNodeID" Type="Int">结束节点ID</param>
            /// <param name="iDirType" Type="Int">节点类型 0-前进 1-返回</param>
            /// <param name="iIsCanBack" Type="Int">是否可以原路返回</param>
            this.fromNodeID = iFromNodeID;
            this.toNodeID = iToNodeID;
            this.dirType = iDirType;
            this.isCanBack = iIsCanBack;
            this.FromNode = null;
            this.ToNode = null;
        }

        function getPointForRaphael() {
            var p = $('#holder').offset();
            return { x: lastMousePoint.x - p.left, y: lastMousePoint.y - p.top };
        }

        function getRaphaelPoint(pageX,pageY) {
            var p = $('#holder').offset();
            return { x: pageX - p.left, y: pageY - p.top };
        }

        function getNewElementName(eleType) {
            var name;
            var maxid = 0;
            var idstr;
            var currId;
            switch (eleType) {
                case 'node':
                    name = '新建节点 ';

                    $.each(rflow.nodes, function () {
                        if (this.Node.name.length > 5 && this.Node.name.substr(0, 5) == name) {
                            currId = parseInt(this.Node.name.substr(5));
                            if (!isNaN(currId)) {
                                maxid = Math.max(maxid, currId);
                            }
                        }
                    });
                    break;
                case 'label':
                    name = '新建标签 ';

                    $.each(rflow.labels, function () {
                        if (this.Text.name.length > 5 && this.Text.name.substr(0, 5) == name) {
                            currId = parseInt(this.Text.name.substr(5));
                            if (!isNaN(currId)) {
                                maxid = Math.max(maxid, currId);
                            }
                        }
                    });
                    break;
                default:
                    break;
            }

            return name + (maxid + 1);
        }

        function ajaxService(method, dataString, fnSuccess, fnSuccessArgs, fnError, fnErrorArgs) {
            /// <summary>ajax异步调用/Admin/XAP/WebService.asmx</summary>
            /// <param name="method" Type="String">WebService公开方法</param>
            /// <param name="dataString" Type="String">调用时发送的数据，格式必须与$.ajax方法的data数据格式一致，如"{name:'xxx',age:12}"
            /// <para>可使用Params类生成该字符串,如：
            /// <para>  var ps = new Params();
            /// <para>  ps.push('name','xxx');
            /// <para>  ps.push('age',12);
            /// <para>  var dataString = ps.toJsonDataString();
            /// <para>输出：{'name':'xxx','age':12}
            /// </param>
            /// <param name="fnSuccess" Type="Function">调用成功后，要运行的方法，如：function(re){}，其中re为异步调用返回的结果</param>
            /// <param name="fnSuccessArgs" Type="Object">调用成功后运行方法的参数</param>
            /// <param name="fnError" Type="Function">调用失败后，要运行的方法，如：function(re){}，其中re为异步调用失败的responseText</param>
            /// <param name="fnErrorArgs" Type="Object">调用失败后运行方法的参数</param>
            $.ajax({
                type: "Post",
                contentType: "application/json;utf-8",
                url: "../Admin/XAP/WebService.asmx/" + method,
                dataType: "json",
                data: dataString,
                success: function (re) {
                    if (fnSuccess != undefined) {
                        fnSuccess(re.d, fnSuccessArgs);
                    }
                },
                error: function (re) {
                    if (fnError != undefined) {
                        fnError(re, fnErrorArgs);
                    }
                    else {
                        alert(re.responseText);
                    }
                }
            });
        }

        function newNode() {
            /// <summary>添加节点</summary>
            var p = getPointForRaphael(),
                nodename = getNewElementName('node'),
                params = new Params(),
                rNode,
                ccnode;

            params.push('isLogin', true);
            params.push('param', [rflow.no, nodename, STYLE_NODE_DEFAULT_ICON.split('.')[0], p.x.toString(), p.y.toString(), '0']);

            ajaxService('DoNewNode', params.toJsonDataString(), function (re) {
                if (re == 0) {
                    $.messager.alert('失败', '添加节点失败!', 'error');
                    return;
                }

                rNode = new RNode(re, nodename, p.x - STYLE_NODE_WIDTH / 2, p.y - STYLE_NODE_HEIGHT / 2);
                rNode.rBorderColor = STYLE_NODE_BORDER_COLOR;

                rcnode = r.ccnode(rNode.x, rNode.y, STYLE_NODE_DEFAULT_ICON_PATH + STYLE_NODE_DEFAULT_ICON, nodename);
                rcnode.rBorderColor = STYLE_NODE_BORDER_COLOR;
                rcnode.nodeid = re;
                rcnode.Node = rNode;

                rcnode.rBorder.attr({ "stroke": rcnode.rBorderColor, "stroke-width": STYLE_NODE_BORDER_NORMAL_WIDTH });
                rcnode.rText.attr({ "stroke": STYLE_NODE_FORE_COLOR, "font-size": STYLE_NODE_FONT_SIZE });

                rflow.nodes.push(rcnode);
            });
        }

        function newLabel() {
            /// <summary>添加标签</summary>
            var p = getPointForRaphael(),
                labelname = getNewElementName('label'),
                params = new Params(),
                rLabel,
                cclabel;

            params.push('fk_flow', rflow.no);
            params.push('x', p.x);
            params.push('y', p.y);
            params.push('name', labelname);
            params.push('lableId', null);

            ajaxService('DoNewLabel', params.toJsonDataString(), function (re) {
                if (re == null || isNaN(re)) {
                    $.messager.alert('失败', '添加标签失败!', 'error');
                    return;
                }

                rLabel = new RLabel(re, labelname, p.x, p.y);
                rclabel = r.cclabel(p.x, p.y, labelname);
                rclabel.Text = rLabel;
                rclabel.rText.attr({ "stroke": STYLE_LABEL_FORE_COLOR, "font-size": STYLE_LABEL_FONT_SIZE, "text-anchor": "start" });
                rclabel.rText.toFront();

                rflow.labels.push(rclabel);
            });
        }

        function newDiretion() {
            /// <summary>添加连线</summary>
            var rele = rflow.focusElement;
            if (rele == null || rele.type != 'node') {
                $.messager.alert('提示', '请选择要连线的起始节点', 'warning');
                return;
            }

            //在鼠标滑动时，在鼠标处定义一个临时的长宽各为1的隐藏矩形，根据开始节点与这个临时矩形，实时划出连接线
            //当移到另一个节点时，单击鼠标，即可以连接该处的节点作为结束节点
            var params = new Params(),
                ccnode = rele.ele,
                rDir,
                ccdirection,
                dirStartPoint = { x: ccnode.rBorder.attr("x") + STYLE_NODE_WIDTH / 2, y: ccnode.rBorder.attr("y") + STYLE_NODE_HEIGHT / 2 },
                tempnode = r.rect(ccnode.rBorder.attr("x") + STYLE_NODE_WIDTH / 2, ccnode.rBorder.attr("y") + STYLE_NODE_HEIGHT + 2, 1, 1),
                mp = { x: ccnode.rBorder.attr("x") + STYLE_NODE_WIDTH / 2, y: ccnode.rBorder.attr("y") + STYLE_NODE_HEIGHT + 1 },
                tps = r.getLinePoints(ccnode.rBorder, tempnode),
                tpath = r.getArrow(tps.x1, tps.y1, tps.x2, tps.y2, 8),
                tdir = r.ccdirection(tpath);

            tempnode.attr({ "stroke": "none", "stroke-width": 0 });

            rflow.isLineing = true;

            $('#holder').bind('mousemove', function (e) {
                $('#msg').text(dirStartPoint.x + ',' + dirStartPoint.y + '=>' + e.pageX + ',' + e.pageY);
                p = getRaphaelPoint(e.pageX, e.pageY);

                //不能将临时矩形放于鼠标处，如果在鼠标处，则在点击时，获取不到下方的节点
                if (p.x >= dirStartPoint.x) {
                    if (p.y != dirStartPoint.y) {
                        p.x -= 10;  //此处如果数值偏小，则会使鼠标点击选不到下方的节点
                    }
                }
                else {
                    p.x += 5;
                }

                if (p.y >= dirStartPoint.y) {
                    if (p.x != dirStartPoint.x) {
                        p.y -= 10;
                    }
                }
                else {
                    p.y += 5;
                }

                tempnode.attr({ x: p.x, y: p.y });

                tps = r.getLinePoints(ccnode.rBorder, tempnode);
                tpath = r.getArrow(tps.x1, tps.y1, tps.x2, tps.y2, 8);
                tdir.attr({ path: tpath });
            }).bind('mouseup', function (e) {
                var ele = r.getElementByPoint(e.pageX, e.pageY);
                if (ele == null || ele.type != 'image') {
                    $('#holder').unbind('mouseup').unbind('mousemove');
                    tdir.remove();
                    tempnode.remove();
                    $.messager.alert('提示', '请选择连线的结束节点', 'warning');
                    return;
                }

                var tonode = rflow.getRaphaelNodeByRIconId(ele.id);
                if (tonode == null) {
                    $('#holder').unbind('mouseup').unbind('mousemove');
                    tdir.remove();
                    tempnode.remove();
                    $.messager.alert('提示', '请选择连线的结束节点', 'warning');
                    return;
                }

                if (tonode.nodeid == ccnode.nodeid) {
                    $('#holder').unbind('mouseup').unbind('mousemove');
                    tdir.remove();
                    tempnode.remove();
                    $.messager.alert('提示', '相同的节点不能添加连接线', 'warning');
                    return;
                }

                tdir.remove();
                tempnode.remove();

                var rDir = new RDirection(ccnode.nodeid, tonode.nodeid, 0, 0),
                    dirPoints,
                    dirPath,
                    rcdir;

                rDir.FromNode = ccnode;
                rDir.ToNode = tonode;
                rDir.isNew = true;  //标识是新加的连接线

                dirPoints = r.getLinePoints(rDir.FromNode.rBorder, rDir.ToNode.rBorder);
                dirPath = r.getArrow(dirPoints.x1, dirPoints.y1, dirPoints.x2, dirPoints.y2, 8);
                rcdir = r.ccdirection(dirPath);
                rcdir.Dir = rDir;

                rflow.dirs.push(rcdir);
                $('#holder').unbind('mouseup').unbind('mousemove');
            });
        }

        function deleteNode() {
            /// <summary>删除节点</summary>
            var rele = rflow.focusElement;
            if (rele == null) {
                $.messager.alert('提示', '请选择要删除的节点', 'warning');
                return;
            }            

            var ps = new Params(),
                delDirs = [];
            ps.push('doWhat', 'DelNode');
            ps.push('para1', rele.ele.nodeid);
            ps.push('isLogin', true);

            ajaxService('Do', ps.toJsonDataString(), function (re) {
                if (re != null && re.length > 0) {
                    $.messager.alert('失败', re, 'error');
                    return;
                }

                //删除关联的连接线
                $.each(rflow.dirs, function () {
                    if (this.Dir.fromNodeID == rele.ele.nodeid || this.Dir.toNodeID == rele.ele.nodeid) {
                        delDirs.push(this);
                    }
                });

                $.each(delDirs, function () {
                    deleteDirection(this);
                });

                rele.ele.remove();
                rflow.nodes.remove(rele.ele);
            });
        }

        function deleteDirection(delDir) {
            /// <summary>删除连线</summary>
            /// <param name="delDir" Type="Raphael Element">绘制的连线对象</param>
            var ps = new Params(),
                rele = rflow.focusElement,
                rdir,
                fromid,
                toid;

            if (delDir != undefined) {
                fromid = delDir.Dir.fromNodeID;
                toid = delDir.Dir.toNodeID;
                rdir = delDir;
            }
            else if (rele == null) {
                $.messager.alert('提示', '请选择要删除的连线', 'warning');
                return;
            }
            else {
                fromid = rele.ele.Dir.fromNodeID;
                toid = rele.ele.Dir.toNodeID;
                rdir = rele.ele;
            }

            ps.push('from', fromid);
            ps.push('to', toid);

            ajaxService('DoDropLine', ps.toJsonDataString(), function (re, dir) {
                if (re != true) {
                    $.messager.alert('失败', '删除连线失败', 'error');
                    throw '删除连线失败';
                }

                dir.remove();
                rflow.dirs.remove(dir);
            }, rdir);
        }

        function deleteLabel() {
            /// <summary>删除标签</summary>
            var rele = rflow.focusElement;
            if (rele == null) {
                $.messager.alert('提示', '请选择要删除的标签', 'warning');
                return;
            }

            var ps = new Params(),
                delDirs = [];
            ps.push('doWhat', 'DelLable');
            ps.push('para1', rele.ele.Text.mypk);
            ps.push('isLogin', true);

            ajaxService('Do', ps.toJsonDataString(), function (re) {
                //                if (re != null && re.length > 0) {
                //                    $.messager.alert('失败', re, 'error');
                //                    return;
                //                }

                rele.ele.remove();
                rflow.labels.remove(rele.ele);
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="holder">
    </div>
    <div id="msg">
    </div>
    <div id="flowMenu" class="easyui-menu" style="width: 180px;">
        <div onclick="newNode()">
            添加节点</div>
        <div onclick="newLabel()">
            添加标签</div>
        <div class="menu-sep">
        </div>
        <div>
            流程属性</div>
        <div>
            运行流程</div>
        <div>
            检查流程</div>
        <div>
            流程报表定义</div>
        <div class="menu-sep">
        </div>
        <div>
            显示/隐藏网格</div>
        <div>
            帮助</div>
        <div>
            异常日志</div>
    </div>
    <div id="nodeMenu" class="easyui-menu" style="width: 180px;">
        <div>
            节点属性</div>
        <div>
            <span>节点类型</span>
            <div style="width: 120px;">
                <div>
                    普通节点</div>
                <div>
                    分流节点</div>
                <div>
                    合流节点</div>
                <div>
                    分合流节点</div>
                <div>
                    子线程节点</div>
            </div>
        </div>
        <div>
            节点工作岗位</div>
        <div>
            修改节点名称</div>
        <div>
            更换图标</div>
        <div class="menu-sep">
        </div>
        <div onclick="newDiretion()">
            添加连线</div>
        <div class="menu-sep">
        </div>
        <div>
            设计节点表单 - 傻瓜模式</div>
        <div>
            设计节点表单 - 自由模式</div>
        <div class="menu-sep">
        </div>
        <div>
            表单库</div>
        <div>
            绑定流程表单</div>
        <div class="menu-sep">
        </div>
        <div data-options="iconCls:'icon-delete'" onclick="deleteNode()">
            删除节点</div>
    </div>
    <div id="labelMenu" class="easyui-menu" style="width: 100px">
        <div>
            修改标签</div>
        <div class="menu-sep">
        </div>
        <div data-options="iconCls:'icon-delete'" onclick="deleteLabel()">
            删除标签</div>
    </div>
    <div id="dirMenu" class="easyui-menu" style="width: 140px">
        <div>
            设置方向转向条件</div>
        <div>
            是否为折线</div>
        <div>
            是否可以原路返回</div>
        <div class="menu-sep">
        </div>
        <div data-options="iconCls:'icon-delete'" onclick="deleteDirection()">
            删除连接线</div>
    </div>
</asp:Content>
