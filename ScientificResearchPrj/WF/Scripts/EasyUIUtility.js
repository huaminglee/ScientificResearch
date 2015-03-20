/// <reference path="easyUI/jquery-1.8.0.min.js" />
/// <reference path="easyUI/jquery.easyui.min.js" />

//added by liuxc,2014-12-1
//此文件可用于存放EasyUI的公用JS方法，建议都给JS方法加上注释，Demo如下

function OpenEasyUiDialog(url, iframeId, dlgTitle, dlgWidth, dlgHeight, dlgIcon, showBtns, okBtnFunc, okBtnFuncArgs, dockObj) {
    ///<summary>使用EasyUiDialog打开一个页面</summary>
    ///<param name="url" type="String">页面链接</param>
    ///<param name="iframeId" type="String">嵌套url页面的iframe的id，在okBtnFunc中，可以通过document.getElementById('eudlgframe').contentWindow获取该页面，然后直接调用该页面的方法，比如获取选中值</param>
    ///<param name="dlgTitle" type="String">Dialog标题</param>
    ///<param name="dlgWidth" type="int">Dialog宽度</param>
    ///<param name="dlgHeight" type="int">Dialog高度</param>
    ///<param name="dlgIcon" type="String">Dialog图标，必须是一个样式class</param>
    ///<param name="showBtns" type="Boolean">Dialog下方是否显示“确定”“取消”按钮，如果显示，则后面的okBtnFunc参数要填写</param>
    ///<param name="okBtnFunc" type="Function">点击“确定”按钮调用的方法</param>
    ///<param name="okBtnFuncArgs" type="Object">okBtnFunc方法使用的参数</param>
    ///<param name="dockObj" type="Object">Dialog绑定的dom对象，随此dom对象有尺寸变化而变化，如：document.getElementById('mainDiv')</param>

    var dlg = $('#eudlg');
    var isTheFirst;

    if (dlg.length == 0) {
        isTheFirst = true;
        var divDom = document.createElement('div');
        divDom.setAttribute('id', 'eudlg');
        document.body.appendChild(divDom);
        dlg = $('#eudlg');
        dlg.append("<iframe frameborder='0' src='' scrolling='auto' id='" + iframeId + "' style='width:100%;height:100%'></iframe>");
    }

    //处理定位外层容器尺寸变化事件
    if (dockObj != null && dockObj != undefined) {
        var dobj = $(dockObj);

        dlgWidth = dobj.innerWidth() - 20;
        dlgHeight = dobj.innerHeight() - 20;

        if (isTheFirst) {
            $(dockObj).resize(function () {
                var obj = $(this);

                $('#eudlg').dialog('resize', {
                    width: obj.innerWidth() - 20,
                    height: obj.innerHeight() - 40
                });
            });
        }
    }

    dlg.dialog({
        title: dlgTitle,
        width: dlgWidth,
        height: dlgHeight,
        iconCls: dlgIcon,
        resizable: true,
        modal: true
    });

    if (showBtns) {
        dlg.dialog({
            buttons: [{
                text: '确定',
                iconCls: 'icon-save',
                handler: function () {
                    okBtnFunc(okBtnFuncArgs)
                    dlg.dialog('close');
                    $('#' + iframeId).attr('src', '');
                }
            }, {
                text: '取消',
                iconCls: 'icon-cancel',
                handler: function () {
                    dlg.dialog('close');
                    $('#' + iframeId).attr('src', '');
                }
            }]
        });
    }
    else {
        dlg.dialog({
            buttons: null,
            onClose: function () {
                dlg.find("iframe").attr('src', '');
            }
        });
    }

    dlg.dialog('open');
    $('#' + iframeId).attr('src', url);
}

function OpenEasyUiSampleEditDialog(editPropertyName, doType, oldValue, dlgWidth, dlgHeight, dlgIcon) {

    var dlg = $('#eueditdlg'),
        val = null;

    if (dlg.length == 0) {
        var divDom = document.createElement('div');
        divDom.setAttribute('id', 'eueditdlg');
        document.body.appendChild(divDom);
        dlg = $('#eudlg');
        dlg.append("<p>" + editPropertyName + ":<br />" +
            "<input id='eutxt' class='easyui-textbox' style='width:300px;height:60px' /></p>");
    }

    dlg.dialog({
        title: doType + editPropertyName,
        width: dlgWidth || 300,
        height: dlgHeight || 60,
        iconCls: dlgIcon,
        resizable: false,
        modal: true,
        buttons: [{
            text: '确定',
            iconCls: 'icon-save',
            handler: function () {
                val = $('#eutxt').val();
            }
        }, {
            text: '取消',
            iconCls: 'icon-cancel',
            handler: function () {
                dlg.dialog('close');
                $('##eutxt').val('');
            }
        }]
    });
    
    dlg.dialog('open');

    return val;
}

function EasyUiConfirm(msg, fn) {

}