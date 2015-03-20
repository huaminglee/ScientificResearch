/**
 * 庄坚编写 2014-8-17
 * 扩展了jquery部分功能
 * 同步请求
 */

$.extend($.fn.propertygrid.defaults.editors, {
     
    //多行文本框
    ex_textarea: {
        init: function (container, options) {
            var input = $('<textarea style="width:99%;height:100px;border:0px;font-size: 18px;"></textarea>').appendTo(container);
            return input;
        },
        getValue: function (target) {
            return $(target).val();
        },
        setValue: function (target, value) {
            $(target).val(value);
        },
        resize: function (target, width) {
            console.log("resize method invoke!");
            $(target)._outerWidth(width);
        },
        destroy: function(target){
            console.log("destroy method invoke!");
            $(target).remove();
        }
        
    },

  
     
    ex_panel: {
        index: 0,
        init: function (container, options) {
            var me = this;
            me.index = me.index + 1;

            var panelIDStr = "inputID" + me.index;
            var linkIDStr = "linkID" + me.index;
            
            var cur = $('<span id="' + panelIDStr + '" style="border:0px solid blue;margin:auto 0; height:auto;width:90%"/>');
          
            var button = $('<a id="' + linkIDStr + '" href="javascript:void(0)" icon-index="0"  class="textbox-icon icon-tip" style="border:0;float:right;background-color:#fff;height: 32px !important;width: 28px !important;"  ></a>');
            //外层，用以实现文本框加上图标
            var outnerSpan = $('<span  style="height:auto;width:100%"/>');

            outnerSpan.append(cur);
            outnerSpan.append(button);
            outnerSpan.appendTo(container);

            var editor = cur;
            
            $('#' + linkIDStr).click(function () {
                options.callback();
            });
            console.log("init method invoke!");
            editor.enableEdit = false;
            return editor;
        },
        getValue: function (target) {
            //return $(target).val();
            return $(target).html();
        },
        setValue: function (target, value) {
            //$(target).val(value);
            $(target).html(value);
        },
        resize: function (target, width) {
            console.log("resize method invoke!");
        },
        destroy: function (target) {
            console.log("destroy method invoke!");
        }
    },

    ex_expandrows:{
        index: 0,
        init: function (container, options) {
            var me = this;
            me.index = me.index + 1;

            var panelIDStr = "inputID" + me.index;
            var linkIDStr = "linkID" + me.index;

            var cur = $('<span id="' + panelIDStr + '" style="border:0px solid blue;margin-left: 0px; margin-right: 0px; height:auto;width:90%"/>');

            var button = $('<a id="' + linkIDStr + '" href="javascript:void(0)" icon-index="0"  class="textbox-icon ' + options.icon + '" style="border:0;float:right"  ></a>');
            var wordSpan = $('<span style="border:0;float:right"></span>');//点我添加新的调研结果
            //外层，用以实现文本框加上图标
            var outnerSpan = $('<span style="height:auto;width:100%"/>');
            outnerSpan.append(cur);
            outnerSpan.append(button);
            outnerSpan.append(wordSpan);
            outnerSpan.appendTo(container);

            var editor = cur;

            $('#' + linkIDStr).click(function () {
                options.callback();
            });
            console.log("init method invoke!");
            editor.enableEdit = false;
            return editor;
        },
        getValue: function (target) {
            //return $(target).val();
            return $(target).html();
        },
        setValue: function (target, value) {
            //$(target).val(value);
            $(target).html(value);
        },
        resize: function (target, width) {
            console.log("resize method invoke!");
        },
        destroy: function (target) {
            console.log("destroy method invoke!");
        }
    }
});
 
$.extend($.fn.datagrid.defaults.editors, {
    
    //多行文本框，弹出dialog
    ex_wintextarea: {
        index: 0,
        init: function (container, options) {

            var me = this;
            me.index = me.index + 1;

            var inputWidth = (container.width() - 24) + "px";
            var containerWidth = container.width() + "px";
            //非ie下可能出现container.width()为空的情况
            if (container.width() == "") {
                inputWidth = "90%";
                containerWidth = "100%";
            }

            var inputIDStr = "inputID" + me.index;
            var linkIDStr = "linkID" + me.index;

            var cur = $('<input id="' + inputIDStr + '" type="text" class="textbox-text"  style="margin-left: 0px; margin-right: 16px; width:' + inputWidth + '"/>');
            //内层，用于实现图标
            var innerSpan = $("<span class='textbox-addon textbox-addon-right' style='right: 0px;'/>");
            var button = $('<a id="' + linkIDStr + '" href="javascript:void(0)" icon-index="0"  class="textbox-icon icon-edit" style="border:0" onmouseover="this.style.backgroundColor=\'transparent\';" ></a>');
            innerSpan.append(button);
            //外层，用以实现文本框加上图标
            var outnerSpan = $('<span class="textbox" style="height:20px;width:' + containerWidth + '"/>');
            outnerSpan.append(innerSpan);
            outnerSpan.append(cur);

            outnerSpan.appendTo(container);

            var editor = cur;

            $('#' + linkIDStr).click(function () {
                var textareaWin = undefined;
                var winTitle = undefined;
                if (!textareaWin) {
                    if (options.readonly != undefined && options.readonly == true) {
                        $('<div id="win"><textarea readonly="true" rows="10" cols="40" style="border:0" id="textareaID" class="datagrid-editable-input"></textarea></div>').appendTo($("body"));
                        winTitle = "【不可编辑】";
                    } else {
                        $('<div id="win"><textarea rows="10" cols="40" style="border:0" id="textareaID" class="datagrid-editable-input"></textarea></div>').appendTo($("body"));
                        winTitle = "【可编辑】";
                    }

                    $("#textareaID").val($("#" + inputIDStr).val());
                    textareaWin = $('#win').dialog({
                        title: winTitle,
                        width: 353,
                        height: 230,
                        closable: false,
                        resizable: false,
                        closed: false,
                        collapsible: false,
                        maximizable: false,
                        minimizable: false,
                        modal: true,
                        buttons: [{
                            text: '保存',
                            iconCls: 'icon-add',
                            handler: function () {
                                var textVal = $("#textareaID").val();
                                $("#" + inputIDStr).val(textVal);
                                textareaWin.window('close');
                                textareaWin = undefined;
                                $('#win').remove();
                            }
                        }, {
                            iconCls: 'icon-cancel',
                            text: '取消',
                            handler: function () {
                                textareaWin.window('close');
                                textareaWin = undefined;
                                $('#win').remove();
                            }
                        }]
                    });
                    $("#textareaID").focus();
                }
            });
            console.log("init method invoke!");
            editor.enableEdit = false;
            return editor;
        },
        getValue: function (target) {
            return $(target).val();
        },
        setValue: function (target, value) {
            $(target).val(value);
        },
        resize: function (target, width) {
            console.log("resize method invoke!");
        },
        destroy: function (target) {
            console.log("destroy method invoke!");
            textareaWin = undefined;
        }
    },


    ex_wintextareareadonly: {
        index: 0,
        init: function (container, options) {

            var me = this;
            me.index = me.index + 1;

            var inputWidth = (container.width() - 24) + "px";
            var containerWidth = container.width() + "px";
            //非ie下可能出现container.width()为空的情况
            if (container.width() == "") {
                inputWidth = "90%";
                containerWidth = "100%";
            }

            var inputIDStr = "inputID" + me.index;
            var linkIDStr = "linkID" + me.index;

            var cur = $('<input id="' + inputIDStr + '" type="text" class="textbox-text"  style="margin-left: 0px; margin-right: 16px; width:' + inputWidth + '"/>');
            //内层，用于实现图标
            var innerSpan = $("<span class='textbox-addon textbox-addon-right' style='right: 0px;'/>");
            var button = $('<a id="' + linkIDStr + '" href="javascript:void(0)" icon-index="0"  class="textbox-icon icon-edit" style="border:0" onmouseover="this.style.backgroundColor=\'transparent\';" ></a>');
            innerSpan.append(button);
            //外层，用以实现文本框加上图标
            var outnerSpan = $('<span class="textbox" style="height:20px;width:' + containerWidth + '"/>');
            outnerSpan.append(innerSpan);
            outnerSpan.append(cur);

            outnerSpan.appendTo(container);

            var editor = cur;

            $('#' + linkIDStr).click(function () {
                var textareaWin = undefined;
                var winTitle = undefined;
                if (!textareaWin) {
                    $('<div id="win"><textarea readonly="true" rows="10" cols="40" style="border:0" id="textareaID" class="datagrid-editable-input"></textarea></div>').appendTo($("body"));
                    winTitle = "【不可编辑】";
                    
                    $("#textareaID").val($("#" + inputIDStr).val());
                    textareaWin = $('#win').dialog({
                        title: winTitle,
                        width: 353,
                        height: 230,
                        closable: false,
                        resizable: false,
                        closed: false,
                        collapsible: false,
                        maximizable: false,
                        minimizable: false,
                        modal: true,
                        buttons: [{
                            text: '关闭窗口',
                            iconCls: 'icon-remove',
                            handler: function () {
                                var textVal = $("#textareaID").val();
                                $("#" + inputIDStr).val(textVal);
                                textareaWin.window('close');
                                textareaWin = undefined;
                                $('#win').remove();
                            }
                        } ]
                    });
                    $("#textareaID").focus();
                }
            });
            console.log("init method invoke!");
            editor.enableEdit = false;
            return editor;
        },
        getValue: function (target) {
            return $(target).val();
        },
        setValue: function (target, value) {
            $(target).val(value);
        },
        resize: function (target, width) {
            console.log("resize method invoke!");
        },
        destroy: function (target) {
            console.log("destroy method invoke!");
            textareaWin = undefined;
        }
    }
})