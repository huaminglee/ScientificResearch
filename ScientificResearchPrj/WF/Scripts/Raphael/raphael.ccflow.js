/// <reference path="raphael.js" />
/// <reference path="../easyUI/jquery-1.8.0.min.js" />
/// <reference path="../easyUI/jquery.easyui.min.js" />


//定义样式变量，可在此统一修改流程样式
var STYLE_NODE_WIDTH = 50;
var STYLE_NODE_HEIGHT = 50;
var STYLE_NODE_BORDER_RADIUS = 5;
//var STYLE_NODE_DEFAULT_ICON_PATH = '/ccflow5/ClientBin/NodeIcon/';
var STYLE_NODE_DEFAULT_ICON_PATH = '/ClientBin/NodeIcon/';
var STYLE_NODE_DEFAULT_ICON = '审核.png';
var STYLE_FIRST_NODE_BORDER_COLOR = 'green';
var STYLE_END_NODE_BORDER_COLOR = 'red';
var STYLE_NODE_BORDER_COLOR = 'black';
var STYLE_NODE_BORDER_HOVER_COLOR = 'blue';
var STYLE_NODE_BORDER_NORMAL_WIDTH = 1;
var STYLE_NODE_BORDER_HOVER_WIDTH = 2;
var STYLE_NODE_FONT_SIZE = 16;
var STYLE_NODE_FORE_COLOR = 'none';
var STYLE_NODE_HOVER_FORE_COLOR = 'blue';
var STYLE_LABEL_FONT_SIZE = 14;
var STYLE_LABEL_FORE_COLOR = 'none';
var STYLE_LABEL_HOVER_COLOR = 'blue';
var STYLE_LINE_COLOR = 'green';
var STYLE_LINE_HOVER_COLOR = 'blue';
var STYLE_LINE_WIDTH = 2;
var STYLE_LINE_TRACK_COLOR = 'red';
var STYLE_NODE_TRACK_FONT_SIZE = 16;
var STYLE_NODE_TRACK_FORE_COLOR = 'none';
var STYLE_FOCUS_COLOR = 'lightblue';
//var DATA_USER_ICON_PATH = '/ccflow5/DataUser/UserIcon/';
var DATA_USER_ICON_PATH = '/DataUser/UserIcon/';
var DATA_USER_ICON_DEFAULT = 'Default.png';

var rflow;
var isie = isIE();

function RFlow(sFlowNo) {
    /// <summary>流程</summary>
    /// <param name="sFlowNo" Type="String">流程编号</param>
    this.no = sFlowNo;
    this.nodes = new Array();
    this.labels = new Array();
    this.dirs = new Array();
    this.focusElement = {}; //记录当前选中的控件，{type:'',ele:obj}

    if (typeof RFlow._initialized == "undefined") {
        RFlow.prototype.getRaphaelNodeByNodeId = function (nodeid) {
            /// <summary>根据指定节点ID获取该结点使用Raphael绘制的对象Set</summary>
            /// <param name="nodeid" Type="Int">流程编号</param>
            for (var i = 0, j = this.nodes.length; i < j; i++) {
                if (this.nodes[i].nodeid == nodeid) {
                    return this.nodes[i];
                }
            }

            return null;
        }

        RFlow.prototype.getRaphaelNodeByRIconId = function (raphaelid) {
            /// <summary>根据绘制的节点中的ICON对象的id获取该结点使用Raphael绘制的对象</summary>
            /// <param name="raphaelid" Type="Int">流程编号</param>
            for (var i = 0, j = this.nodes.length; i < j; i++) {
                if (this.nodes[i].rIcon.id == raphaelid) {
                    return this.nodes[i];
                }
            }

            return null;
        }

        RFlow.prototype.clearFocus = function () {
            /// <summary>清除当前选中的对象有选中状态</summary>
            if (this.focusElement.ele) {
                this.focusElement.type = null;
                this.focusElement.ele.clearFocus();
                this.focusElement.ele = null;
            }
        }
    }
}

function Params() {
    /// <summary>JSON传输data参数生成对象</summary>
    /// <desc>可使用Params.push(key,value)来添加参数，value也可为Array数组</desc>
    this.keys = new Array();
    this.values = new Array();

    if (typeof RFlow._initialized == "undefined") {
        Params.prototype.push = function (key, value) {
            if (key == undefined || key == null) {
                $.messager.alert('错误', 'key不能为空', 'error');
            }

            this.keys.push(key);
            this.values.push(value);
        }

        Params.prototype.clear = function () {
            this.keys.length = 0;
            this.values.length = 0;
        }

        Params.prototype.toJsonDataString = function () {
            var s = '{';
            var isString, isArr;

            for (var i = 0, j = this.keys.length; i < j; i++) {
                isString = typeof this.values[i] === 'string';
                isArr = isArray(this.values[i]);
                s += this.keys[i] + ":";

                if (isString) {
                    s += "'" + this.values[i] + "',";
                }
                else if (isArr) {
                    s += "[";
                    isString = typeof this.values[i][0] === 'string';

                    $.each(this.values[i], function () {
                        s += (isString ? "'" : "") + this + (isString ? "'," : ",");
                    });

                    s = removeLastComma(s) + "],";
                }
                else if (this.values[i] == null) {
                    s += "null,";
                }
                else {
                    s += this.values[i] + ",";
                }
            }

            s = removeLastComma(s) + '}';
            return s;
        }
    }
}

Raphael.fn.ccnode = function (x, y, icon, label) {
    /// <summary>绘制流程中的节点</summary>
    /// <param name="x" Type="Int">节点的X坐标</param>
    /// <param name="y" Type="Int">节点的Y坐标</param>
    /// <param name="icon" Type="String">节点图标路径</param>
    /// <param name="label" Type="String">节点的名称</param>

    var p = this,
        ccnode = this.set(),
        dragBorderPointStart,
        dragTextPointStart,
        isDrag = false,
        isFocus = false;

    ccnode.push(ccnode.rBorder = p.rect(x, y, STYLE_NODE_WIDTH, STYLE_NODE_HEIGHT, STYLE_NODE_BORDER_RADIUS));
    ccnode.push(ccnode.rIcon = p.image(icon, x + 1, y + 1, STYLE_NODE_WIDTH - 2, STYLE_NODE_HEIGHT - 2));
    ccnode.push(ccnode.rText = p.text(x + STYLE_NODE_WIDTH / 2, y + STYLE_NODE_HEIGHT + 10, label));

    //ccnode.rIcon.data("border", ccnode.rBorder.id);
    //ccnode.rIcon.data("label", ccnode.rText.id);

    ccnode.rIcon.hover(function () {
        if (isFocus) {
            return;
        }

        ccnode.rBorder.attr({ "stroke": STYLE_NODE_BORDER_HOVER_COLOR, "stroke-width": STYLE_NODE_BORDER_HOVER_WIDTH });
        ccnode.rText.attr({ "stroke": STYLE_NODE_HOVER_FORE_COLOR });
    }, function () {
        if (isFocus) {
            return;
        }

        ccnode.rBorder.attr({ "stroke": ccnode.rBorderColor, "stroke-width": STYLE_NODE_BORDER_NORMAL_WIDTH });
        ccnode.rText.attr({ "stroke": STYLE_NODE_FORE_COLOR });
    });

    ccnode.rIcon.drag(iconMove, iconDrag, iconUp);

    $(ccnode.rIcon.node).bind("contextmenu", function (e) {
        ccnode.focus();

        $('#nodeMenu').menu('show', {
            left: e.pageX,
            top: e.pageY
        });

        return false;
    });

    function iconDrag(x, y, e) {
        if (!((isie && e.button == 1) || e.button == 0)) {
            isDrag = false; //此处为解决非IE浏览器一些莫名其妙的问题，增加此变量来解决
            return;
        }

        isDrag = true;
        this.ox = this.attr("x");
        this.oy = this.attr("y");
        this.animate({ "fill-opacity": 0.5 }, 500);

        //记录与ICON绑定的其他对象的原始坐标
        dragBorderPointStart = { x: ccnode.rBorder.attr("x"), y: ccnode.rBorder.attr("y") };
        dragTextPointStart = { x: ccnode.rText.attr("x"), y: ccnode.rText.attr("y") };

        ccnode.focus();
    }

    function iconMove(dx, dy, x, y, e) {
        if (!isDrag) {
            return;
        }

        var att = { x: this.ox + dx, y: this.oy + dy }, ps, path1;
        this.attr(att);

        //动态修改与ICON绑定的其他对象的坐标
        ccnode.rBorder.attr({ x: dragBorderPointStart.x + dx, y: dragBorderPointStart.y + dy });
        ccnode.rText.attr({ x: dragTextPointStart.x + dx, y: dragTextPointStart.y + dy });

        //重绘与该节点相连的连接线
        for (var i = 0, j = rflow.dirs.length; i < j; i++) {
            if (rflow.dirs[i].Dir.FromNode.nodeid == ccnode.nodeid) {
                ps = p.getLinePoints(ccnode.rBorder, rflow.dirs[i].Dir.ToNode.rBorder);
                path1 = p.getArrow(ps.x1, ps.y1, ps.x2, ps.y2, 8);
                rflow.dirs[i].rPath.attr({ path: path1 });
            }
            else if (rflow.dirs[i].Dir.ToNode.nodeid == ccnode.nodeid) {
                ps = p.getLinePoints(rflow.dirs[i].Dir.FromNode.rBorder, ccnode.rBorder);
                path1 = p.getArrow(ps.x1, ps.y1, ps.x2, ps.y2, 8);
                rflow.dirs[i].rPath.attr({ path: path1 });
            }
        }
    }

    function iconUp(e) {
        if (!isDrag) {
            return;
        }

        this.animate({ "fill-opacity": 1 }, 500);
    }

    ccnode.clearFocus = function () {
        this.rBorder.attr({ "stroke": ccnode.rBorderColor || STYLE_NODE_BORDER_COLOR, "stroke-width": STYLE_NODE_BORDER_NORMAL_WIDTH });
        this.rText.attr({ "stroke": STYLE_NODE_FORE_COLOR });
        isFocus = false;
    }

    ccnode.focus = function () {
        rflow.clearFocus();
        rflow.focusElement.type = 'node';
        rflow.focusElement.ele = this;
        this.rBorder.attr({ "stroke": STYLE_FOCUS_COLOR });
        this.rText.attr({ "stroke": STYLE_FOCUS_COLOR });
        isFocus = true;
    }

    return ccnode;
}

Raphael.fn.ccdirection = function (path) {
    var p = this,
        ccdir = this.set(),
        isFocus = false;

    ccdir.push(ccdir.rPath = p.path(path));
    ccdir.rPath.attr({ "stroke": STYLE_LINE_COLOR, "stroke-width": STYLE_LINE_WIDTH }); //, "arrow-end": "classic-wide-long"设置在hover时有问题
    $(ccdir.rPath.node).bind("contextmenu", function (e) {
        ccdir.focus();

        $('#dirMenu').menu('show', {
            left: e.pageX,
            top: e.pageY
        });

        return false;
    });

    ccdir.rPath.hover(function () {
        if (isFocus) {
            return;
        }

        this.attr("stroke", STYLE_LINE_HOVER_COLOR);
    }, function () {
        if (isFocus) {
            return;
        }

        this.attr("stroke", STYLE_LINE_COLOR);
    });

    ccdir.rPath.mousedown(function (e) {
        if (!((isie && e.button == 1) || e.button == 0)) {
            return;
        }

        ccdir.focus();
    });

    ccdir.clearFocus = function () {
        ccdir.rPath.attr({ "stroke": STYLE_LINE_COLOR });
        isFocus = false;
    }

    ccdir.focus = function () {
        rflow.clearFocus();
        rflow.focusElement.type = 'dir';
        rflow.focusElement.ele = this;
        this.rPath.attr({ "stroke": STYLE_FOCUS_COLOR });
        isFocus = true;
    }

    return ccdir;
}

Raphael.fn.cclabel = function (x, y, label) {
    var p = this,
        cctext = this.set(),
        isDrag = false,
        isFocus = false;

    cctext.push(cctext.rText = p.text(x, y, label));
    cctext.rText.drag(txtMove, txtDrag, txtUp);
    $(cctext.rText.node).bind("contextmenu", function (e) {
        cctext.focus();

        $('#labelMenu').menu('show', {
            left: e.pageX,
            top: e.pageY
        });

        return false;
    });

    cctext.rText.hover(function () {
        if (isFocus) {
            return;
        }

        this.attr("stroke", STYLE_LABEL_HOVER_COLOR);
    }, function () {
        if (isFocus) {
            return;
        }

        this.attr("stroke", STYLE_LABEL_FORE_COLOR);
    });

    function txtDrag(x, y, e) {
        if (!((isie && e.button == 1) || e.button == 0)) {
            isDrag = false;
            return;
        }

        isDrag = true;
        this.ox = this.attr("x");
        this.oy = this.attr("y");
        this.animate({ "fill-opacity": 0.5 }, 500);

        cctext.focus();
    }

    function txtMove(dx, dy) {
        if (!isDrag) {
            return;
        }

        var att = { x: this.ox + dx, y: this.oy + dy };
        this.attr(att);
    }

    function txtUp() {
        if (!isDrag) {
            return;
        }

        this.animate({ "fill-opacity": 1 }, 500);
    }

    cctext.clearFocus = function () {
        cctext.rText.attr({ "stroke": STYLE_LABEL_FORE_COLOR });
        isFocus = false;
    }

    cctext.focus = function () {
        rflow.clearFocus();
        rflow.focusElement.type = 'label';
        rflow.focusElement.ele = this;
        this.rText.attr({ "stroke": STYLE_FOCUS_COLOR });
        isFocus = true;
    }

    return cctext;
}

Raphael.fn.getArrow = function (x1, y1, x2, y2, size) {
    /// <summary>获取两点之间连线的路径，带箭头</summary>
    /// <param name="x1" Type="Int">开始点X坐标</param>
    /// <param name="y1" Type="Int">开始点Y坐标</param>
    /// <param name="x2" Type="Int">结束点X坐标</param>
    /// <param name="y2" Type="Int">结束点Y坐标</param>
    /// <param name="size" Type="Int">箭头长度</param>
    var angle = Raphael.angle(x1, y1, x2, y2); //得到两点之间的角度
    var a45 = Raphael.rad(angle - 45); //角度转换成弧度
    var a45m = Raphael.rad(angle + 45);
    var x2a = x2 + Math.cos(a45) * size;
    var y2a = y2 + Math.sin(a45) * size;
    var x2b = x2 + Math.cos(a45m) * size;
    var y2b = y2 + Math.sin(a45m) * size;
    return ["M", x1, y1, "L", x2, y2, "L", x2a, y2a, "M", x2, y2, "L", x2b, y2b];
}

Raphael.fn.getLinePoints = function (rElementFrom, rElementTo) {
    /// <summary>获取两个绘图元素之间的连接端点的坐标</summary>
    /// <param name="rElementFrom" Type="Raphael Element">连接线开始的绘图元素</param>
    /// <param name="rElementTo" Type="Raphael Element">连接线结束的绘图元素</param>
    var bb1 = rElementFrom.getBBox(),
        bb2 = rElementTo.getBBox();

    var p = [
            { x: bb1.x + bb1.width / 2, y: bb1.y - 1 },
            { x: bb1.x + bb1.width / 2, y: bb1.y + bb1.height + 1 },
            { x: bb1.x - 1, y: bb1.y + bb1.height / 2 },
            { x: bb1.x + bb1.width + 1, y: bb1.y + bb1.height / 2 },
            { x: bb2.x + bb2.width / 2, y: bb2.y - 1 },
            { x: bb2.x + bb2.width / 2, y: bb2.y + bb2.height + 1 },
            { x: bb2.x - 1, y: bb2.y + bb2.height / 2 },
            { x: bb2.x + bb2.width + 1, y: bb2.y + bb2.height / 2 }
                ];

    var d = {}, dis = [];

    for (var i = 0; i < 4; i++) {
        for (var j = 4; j < 8; j++) {
            var dx = Math.abs(p[i].x - p[j].x),
                            dy = Math.abs(p[i].y - p[j].y);
            if (
                             (i == j - 4) ||
                             (((i != 3 && j != 6) || p[i].x < p[j].x) &&
                             ((i != 2 && j != 7) || p[i].x > p[j].x) &&
                             ((i != 0 && j != 5) || p[i].y > p[j].y) &&
                             ((i != 1 && j != 4) || p[i].y < p[j].y))
                           ) {
                dis.push(dx + dy);
                d[dis[dis.length - 1]] = [i, j];
            }
        }
    }

    if (dis.length == 0) {
        var res = [0, 4];
    } else {
        res = d[Math.min.apply(Math, dis)];
    }

    return { x1: p[res[0]].x, y1: p[res[0]].y, x2: p[res[1]].x, y2: p[res[1]].y };
}

function getNavigatorInfo() {
    ///<summary>获取浏览器及版本信息</summary>
    var Sys = {};
    var ua = navigator.userAgent.toLowerCase();
    var s;
    (s = ua.match(/msie ([\d.]+)/)) ? Sys.ie = s[1] :
        (s = ua.match(/firefox\/([\d.]+)/)) ? Sys.firefox = s[1] :
        (s = ua.match(/chrome\/([\d.]+)/)) ? Sys.chrome = s[1] :
        (s = ua.match(/opera.([\d.]+)/)) ? Sys.opera = s[1] :
        (s = ua.match(/version\/([\d.]+).*safari/)) ? Sys.safari = s[1] : 0;
    return Sys;
}

function isIE() {
    return navigator.userAgent.toLowerCase().match(/msie ([\d.]+)/);
}

function removeLastComma(str) {
    /// <summary>去除指定字符串最后的逗号</summary>
    /// <param name="str" Type="String">字符串</param>
    if (str.charAt(str.length - 1) == ',') {
        return str.substr(0, str.length - 1);
    }

    return str;
}

function isArray(object) {
    /// <summary>判断是否是数组</summary>
    /// <param name="object" Type="Object">要判断的对象</param>
    return object && typeof object === 'object' &&
            Array == object.constructor;
}

Array.prototype.index = function (obj) {
    for (var i = 0; i < this.length; i++) {
        if (this[i] != undefined && this[i] == obj) {
            return i;
        }
    }

    return -1;
}

Array.prototype.remove = function (obj) {
    var isExist = false;
    for (var i = 0, n = 0; i < this.length; i++) {
        if (this[i] != obj) {
            this[n++] = this[i]
        }
        else if (isExist) {
            continue;
        }
        else {
            isExist = true;
        }
    }

    if (isExist) {
        this.length -= 1
    }
}