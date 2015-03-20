<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrmExcel.aspx.cs" Inherits="CCFlow.WF.CCForm.FrmExcel" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/jBox/Skins/Blue/jbox.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../Scripts/jBox/jquery.jBox-2.3.min.js" type="text/javascript"></script>
    <script src="../Comm/JScript.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        var isShowAll = false;
        var webOffice = null;
        var strTimeKey;
        var isOpen = false;
        var isInfo = false;
        var marksType = "xls,xlsx";
        var wb = null;
        var win = null;
        var app = null;
        var sheet = null;
        var fields = $.parseJSON('<%=ReplaceFields %>');
        var dtlNos = $.parseJSON('<%=ReplaceDtlNos %>');        
        var exceptions = ["sum", "count", "counta", "average", "max", "min"]; //暂时写这些常用的函数，但excel中支持的更多

        $(function () {
            InitOffice();

            $('body').attr('onunload', 'closeDoc()');
        });

        function InitOffice() {
            /// <summary>
            /// 初始化Office
            /// </summary>
            webOffice = document.all.WebOffice1;


            if ($('#<%=fileName.ClientID %>').val() != "") {
                OpenWeb("1");
            }

            EnableMenu();
        }

        function EnableMenu() {
            /// <summary>
            /// 设置按钮
            /// </summary>
            webOffice.HideMenuItem(0x01 + 0x02 + 0x04 + 0x10 + 0x20);
        }

        //打开本地文件
        function OpenFile() {
            pageLoadding('正在打开...');

            try {
                if (readOnly()) {
                    return false;
                }

                OpenDoc("open", "xls");
            } catch (e) {
                $.jBox.alert("异常\r\nOpenFile Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }

            loaddingOut('打开完成');
            return false;
        }

        function OpenWeb(loadtype) {
            pageLoadding('正在打开模板...');

            try {
                var type = $("#<%=fileType.ClientID %>").val();
                var fileName = $('#<%=fileName.ClientID %>').val();
                var url = location.href + "&action=LoadFile&LoadType=" + loadtype + "&fileName=" + fileName;
                OpenDoc(url, type);
            } catch (e) {
                $.jBox.alert("异常\r\nOpenWeb Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }

            loaddingOut('打开完成。');
        }

        //打开文件excel
        function OpenDoc(url, type) {

            //打开原始的对象. type =excle .
            var openType = webOffice.LoadOriginalFile(url, type);

            app = webOffice.GetDocumentObject().Application;
            wb = app.ActiveWorkbook; // 当前活动的excel文件.
            win = app.ActiveWindow;  // 当前的工作窗口.
            sheet = wb.ActiveSheet; // 当前活动的工作簿.

            if (openType > 0) {  // 成功打开.
             
                if ("<%=IsMarks %>" == "True")
                    SetTrack(1); // 如果留痕. 
                else
                    SetTrack(0);

                isOpen = true; //文件打开.

                if ("<%=IsFirst %>" == "True") {
                    replaceParams();  // 第一次加载,就填充读取的数据.
                }
                else {
                    loadInfos();
                }
            } else {
                $.jBox.alert('打开文档失败', '异常');
            }
        }

        function SetTrack(track) {
            /// <summary>
            /// 设置留痕,显示所有的留痕用户,是否只读文档.
            /// </summary>
            if ("<%= !toolbar.OfficeSaveEnable %>" == "True") {
                webOffice.ProtectDoc(1, 2, "");
            }
            else {
                webOffice.ProtectDoc(0, 1, "");
            }

            webOffice.SetTrackRevisions(track);

            SetUsers();

            var types = $('#<%=fileType.ClientID %>').val();
            if (marksType.indexOf(types) >= 0) {
                ShowUserName();
            }
        }

        //设置当前操作用户.
        function SetUsers() {
            try {

                webOffice.SetCurrUserName("<%=UserName %>");
                InitShowName();

            } catch (e) {
                $.jBox.alert("异常\r\nSetUsers Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }
        }

        //显示指定用户的留痕
        function ShowUserName() {
            /// <summary>
            /// 显当前用户留痕
            /// </summary>
            try {

                var user = $("#marks option:selected").val();

                if (user == "全部" || user == undefined) {
                    webOffice.ShowRevisions(1);
                    if (isShowAll) {
                        isShowAll = false;
                    }

                    //wb.KeepChangeHistory = true;
                    //wb.HighlightChangesOptions(2, 'Everyone');
                    //wb.ListChangesOnNewSheet = false;
                    //wb.HighlightChangesOnScreen = true;
                } else {
                    isShowAll = true;
                    webOffice.ShowRevisions(1);
                    //wb.KeepChangeHistory = true;
                    //wb.HighlightChangesOptions(2, user);
                    //wb.ListChangesOnNewSheet = false;
                    //wb.HighlightChangesOnScreen = true;
                }
            } catch (e) {
                $.jBox.alert("异常\r\nShowUserName Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }
        }

        //加载所留痕用户
        function InitShowName() {
            try {
                var count = webOffice.GetRevCount();

                var showName = $("#marks");
                showName.empty();

                var list = "全部,";

                //GetRevInfo(i,int) int=1 获取时间  int=3 获取内容  int=0 获取名字
                for (var i = 1; i <= count; i++) {
                    var strOpt = webOffice.GetRevInfo(i, 0);

                    if (list.indexOf(strOpt) < 0) {
                        list += strOpt + ",";
                    }
                }

                var data = list.split(',');
                for (var i = 0; i < data.length; i++) {

                    if (data[i] != null && data[i] != "") {
                        var option = $("<option>").text(data[i]).val(data[i]);
                        showName.append(option);
                    }
                }

            } catch (e) {
                $.jBox.alert("异常\r\InitShowName Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }
        }

        function closeDoc() {
            webOffice.SetCurrUserName("");
            webOffice.closeDoc(0);
        }

        function OpenTempLate() {
            if (readOnly()) {
                return false;
            }

            LoadTemplate('excel', '公文模板', OpenWeb);
            return false;
        }

        //加载模板
        function LoadTemplate(type, title, callback) {
            try {
                $.jBox("iframe:/WF/WebOffice/TempLate.aspx?LoadType=" + type, {
                    title: title,
                    width: 800,
                    height: 350,
                    buttons: { '确定': 'ok' },
                    submit: function (v, h, f) {
                        var row = h[0].firstChild.contentWindow.getSelected();

                        if (row == null) {
                            $.jBox.info('请选一个模板');
                            return false;
                        } else {
                            pageLoadding('打开中...');
                            $("#<%=fileName.ClientID %>").val(row.Name);
                            $("#<%=fileType.ClientID %>").val(row.Type);
                            callback();
                            loaddingOut('打开完成...');

                            return true;
                        }
                    }
                });
            } catch (e) {
                $.jBox.alert("异常\r\nLoadTemplate Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }
        }

        //保存到服务器
        function SaveOffice() {

            pageLoadding('正在保存...');
            try {
                if (isOpen) {

                    var fieldJson = getPostJsonString();
                    var dtlsJson = getDtlsPostJsonString();

                    //此处为了解决表单域提交默认最大1146[测试出的，可能是非准确数值]字符的问题，将大于1000字符的分段发送
                    var len = dtlsJson.length;
                    var arrs = new Array();
                    var spanLen = 1000;
                    var startIdx = 0;

                    webOffice.HttpInit();
                    webOffice.HttpAddPostCurrFile("File", "");

                    //明细表
                    for (var i = 1; ; i++) {
                        startIdx = (i - 1) * spanLen;
                        if (startIdx > len)
                            break;
                        arrs.push(dtlsJson.substr(startIdx, Math.min(len - startIdx, spanLen)));
                    }

                    for (var i = 0; i < arrs.length; i++) {
                        webOffice.HttpAddPostString("dtls" + i, arrs[i]);
                    }

                    len = fieldJson.length;
                    arrs = new Array();
                    //主表
                    for (var i = 1; ; i++) {
                        startIdx = (i - 1) * spanLen;

                        if (startIdx > len) break;

                        arrs.push(fieldJson.substr(startIdx, Math.min(len - startIdx, spanLen)));
                    }

                    for (var i = 0; i < arrs.length; i++) {
                        webOffice.HttpAddPostString("field" + i, arrs[i]);
                    }

                    var src = location.href + "&action=SaveFile&filename=" + $('#<%=fileName.ClientID %>').val();
                    webOffice.HttpPost(src);
                } else {
                    $.jBox.alert('请打开公文!', '提示');
                }
            } catch (e) {
                $.jBox.alert("异常\r\nsaveOffice1 Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }

            loaddingOut('保存完成...');
            var types = $('#<%=fileType.ClientID %>').val();

            try {
                if (marksType.indexOf(types) >= 0) {
                    InitShowName();

                    if (isShowAll) {
                        isShowAll = false;
                    }

                    ShowUserName();
                }
            } catch (e) {
                $.jBox.alert("异常\r\nsaveOffice2 Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }
            return false;
        }

        //套红头文件
        function TaoHong() {
            try {
                var type = $('#<%=fileType.ClientID %>').val();
                var name = $('#<%=fileName.ClientID %>').val();

                if (type == "png" || type == "jpg" || type == "bmp") {
                    AddPicture(window.location.protocol + "//" + window.location.host + "/DataUser/OfficeOverTemplate/" + name, app.Selection, 300, 300);
                } else {
                    AddFile(window.location.protocol + "//" + window.location.host + "/DataUser/OfficeOverTemplate/" + name, app.Selection);
                }
            } catch (e) {
                $.jBox.alert("异常\r\nTaoHong Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }
        }

        //接受修订
        function acceptOffice() {
            try {
                if (readOnly()) {
                    return false;
                }

                webOffice.SetTrackRevisions(4);
            } catch (e) {
                $.jBox.alert("异常\r\nacceptOffice Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }

            return false;
        }

        //拒绝修订
        function refuseOffice() {
            try {
                if (readOnly()) {
                    return false;
                }

                var vCount = webOffice.GetRevCount();
                var strUserName;

                for (var i = 1; i <= vCount; i++) {
                    strUserName = webOffice.GetRevInfo(i, 0);
                    webOffice.AcceptRevision(strUserName, 1);
                }
            } catch (e) {
                $.jBox.alert("异常\r\nrefuseOffice Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }

            return false;
        }

        //加载红头文件模板
        function overOffice() {
            if (readOnly()) {
                return false;
            }

            if (isOpen) {
                LoadTemplate('over', '套红模板', TaoHong);
            } else {
                $.jBox.alert('请打开公文!', '提示');
            }

            return false;
        }

        //打印公文
        function printOffice() {
            try {
                if (isOpen) {
                    webOffice.PrintDoc(1);
                } else {
                    $.jBox.alert('请打开公文!', '提示');
                }
            } catch (e) {
                $.jBox.alert("异常\r\nprintOffice Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }

            return false;
        }

        //加载所有的公文印章
        function sealOffice() {
            if (readOnly()) {
                return false;
            }

            if (isOpen) {
                LoadTemplate('seal', '公文盖章', seal);
            } else {
                $.jBox.alert('请打开公文!', '提示');
            }

            return false;
        }

        //盖章
        function seal() {
            try {
                var name = $('#<%=fileName.ClientID %>').val();
                var type = $('#<%=fileType.ClientID %>').val();

                AddPicture(window.location.protocol + "//" + window.location.host + "/DataUser/OfficeSeal/" + name, app.Selection, 160, 160);
            } catch (e) {
                $.jBox.alert("异常\r\nseal Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }
        }

        //插入图片
        function AddPicture(sPicUrl, range, picWidth, picHeight) {
            try {
                //先将图片下载到本地，解决直接插入网络图片速度过慢的问题
                var path = webOffice.GetTempFilePath();
                webOffice.DownLoadFile(sPicUrl, path, "", "");

                sheet.Shapes.AddPicture(path, 0, -1, range.Left, range.Top, picWidth, picHeight);
                //删除下载的图片
                webOffice.DelLocalFile(path);
            } catch (e) {
                $.jBox.alert("异常\r\nAddPicture Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }
        }

        function AddFile(sFileUrl, range) {

            try {
                //先将文件下载到本地
                var fileExt = sFileUrl.substr(sFileUrl.lastIndexOf('.'));
                var path = webOffice.GetTempFilePath();
                path = path.substr(0, path.lastIndexOf('.')) + fileExt;
                webOffice.DownLoadFile(sFileUrl, path, "", "");

                sheet.OLEObjects.Add(path, false, false).Select();
                app.Selection.ShapeRange.Left = range.Left;
                app.Selection.ShapeRange.Top = range.Top;
                //删除下载的文件
                webOffice.DelLocalFile(path);
            } catch (e) {
                $.jBox.alert("异常\r\nAddFile Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }
        }

        function DownLoad() {
            try {
                if (isOpen) {
                    webOffice.ShowDialog(84);
                } else {
                    $.jBox.alert('请打开公文!', '提示');
                }
            } catch (e) {
                $.jBox.alert("异常\r\nDownLoad Error:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }

            return false;
        }

        //文档只读提示
        function readOnly() {
            if ("<%= !toolbar.OfficeSaveEnable %>" == "True") {
                $.jBox.alert('文档只读不能进行此操作!', '提示');
                return true;
            }
        }

        /*以下是为程序设计而定义的辅助对象*/
        function JWDtl(sDtlNo, oWebTable) {
            /// <summary>JS端Word表格</summary>
            /// <param name="sDtlNo" Type="String">编号，对应服务器端的明细表编号</param>
            /// <param name="oWebTable" Type="Object">从服务器获取的存储数据的Table对象</param>
            this.dtlNo = sDtlNo;
            this.webTable = oWebTable;
            this.excelRange = null; //Excel中该明细表对应的Range
            this.excelName = '';    //Excel中该明细表的名称
            this.columns = new Array();  //JWColumn对象数组

            if (typeof JWDtl._initialized == "undefined") {

                JWDtl.prototype.getCellValue = function (rowIdx, colIdx) {
                    /// <summary>获取指定行列单元格的值</summary>
                    /// <param name="rowIdx" Type="Int">行号，从1开始</param>
                    /// <param name="colIdx" Type="Int">列号，从1开始</param>

                    if (this.excelRange == null) return null;

                    var text = this.excelRange.Cells(rowIdx, colIdx).Text;

                    if (text == null) {
                        text = '';
                    }

                    return text;
                };

                JWDtl.prototype.getValuesJsonString = function () {

                    if (this.excelRange == null) return null;

                    var rows = this.excelRange.Rows.Count;
                    var json = '{"dtlno":"' + this.dtlNo + '","dtl":[';
                    var rowstr = '';
                    var isLastRow = true;
                    var cellValue = '';

                    for (var i = 2; i <= rows; i++) {
                        json += '{"rowid":"' + (i - 1) + '","cells":[';

                        rowstr = '';
                        isLastRow = true;
                        cellValue = '';

                        //此处获取明细表的数据时，还未考虑如果列的顺序变化的情况，如果列变化了，则获取数据会错误，待以后完善
                        for (var ci = 0; ci < this.columns.length; ci++) {
                            if (this.columns[ci].field == 'rowid') continue;

                            cellValue = this.getCellValue(i, this.columns[ci].columnIdx);

                            if (isLastRow && cellValue.length > 0) {
                                isLastRow = false;
                            }

                            rowstr += '{"key":"' + this.columns[ci].field + '","value":"' + encodeURIComponent(cellValue) + '"},';
                        }

                        if (!isLastRow) {
                            json += rowstr;
                        }
                        else {
                            json = removeLastComma(json) + ']},';
                            break;
                        }

                        json = removeLastComma(json) + ']},';
                    }

                    json = removeLastComma(json) + ']}';
                    return json;
                }

                JWDtl._initialized = true;
            }
        }

        function JWColumn(sField, sFormula, sRangeName, iColumnIdx) {
            /// <summary>Excel中明细表列字段信息</summary>
            /// <param name="sField" Type="String">字段名称</param>
            /// <param name="sFormula" Type="String">汇总</param>
            /// <param name="sRangeName" Type="String">标识字段的区域名称</param>
            /// <param name="iColumnIdx" Type="Int">字段所在表的列序号</param>
            this.field = sField;
            this.formula = sFormula;
            this.rangeName = sRangeName;
            this.columnIdx = iColumnIdx;
        }

        function JWField(sField, sValue) {
            /// <summary>Excel中主表字段信息</summary>
            /// <param name="sField" Type="String">字段名称</param>
            /// <param name="sValue" Type="String">字段值</param>
            this.field = sField;
            this.value = sValue;
        }

        function getPostJsonString() {
            //获取并生成主表填充字段的新值JSON字符串
            var mainJson = '[';
            var fieldValue = '';
            var fieldNamePrefix = '';
            var fieldCell;

            $.each(jwMains, function () {
                mainJson += '{"key":"' + this.field + '","value":"';
                fieldCell = getRangeName(this.field, fieldNamePrefix);

                if (fieldCell != null) {
                    fieldValue = (fieldCell.RefersToRange.Text == null ? '' : fieldCell.RefersToRange.Text).replace('"', '\"').replace('\\', '\\\\');
                }
                else {
                    fieldValue = '';
                }

                mainJson += encodeURIComponent(fieldValue) + '"},';
            });

            mainJson = removeLastComma(mainJson) + ']';
            return mainJson;
        }

        function getDtlsPostJsonString() {
            var dtlsJson = '[';

            $.each(jwDtls, function () {
                dtlsJson += this.getValuesJsonString() + ',';
            });

            dtlsJson = removeLastComma(dtlsJson) + ']';
            return dtlsJson;
        }

        function removeLastComma(str) {
            /// <summary>去除指定字符串最后的逗号</summary>
            /// <param name="str" Type="String">字符串</param>
            if (str.charAt(str.length - 1) == ',') {
                return str.substr(0, str.length - 1);
            }
            return str;
        }

        function loadInfos() {
            /// <summary>非第一次打开excel，加载填充字段信息</summary>
            var ccflow_main_bm_prefix = '';
            var ccflow_dtl_bm_prefix = 'dtl_';
            var ccflow_dtl_bm_prefix_len = ccflow_dtl_bm_prefix.length;
            var cellNamePrefix = sheet.Name + '!' + ccflow_dtl_bm_prefix;
            var cellNamePrefixLen = cellNamePrefix.length;
            var jwfield;
            var jwdtl;
            var bmName;
            var dtlBmNamePrefix;
            var dtlBmNamePrefix_len;
            var firstCell;
            var colFields;
            var name;
            var formula;

            //获取主表填充字段集合
            for (var i = 0; i < fields.length; i++) {
                name = getRangeName(fields[i], ccflow_main_bm_prefix);

                if (name == null) {
                    continue;
                }

                jwfield = new JWField(fields[i], name.RefersToRange.Text);
                jwMains.push(jwfield);
            }

            //获取明细表填充字段信息
            for (var d = 0; d < dtlNos.length; d++) {
                name = getRangeName(dtlNos[d], ccflow_dtl_bm_prefix);

                if (name == null) {
                    continue;
                }

                jwdtl = new JWDtl(dtlNos[d], $.parseJSON('[]'));
                jwdtl.excelRange = name.RefersToRange;
                jwdtl.excelName = name.Name;
                jwDtls.push(jwdtl);

                firstCell = name.RefersToRange.Cells(1, 1);

                dtlBmNamePrefix = ccflow_dtl_bm_prefix + dtlNos[d] + '_';
                dtlBmNamePrefix_len = dtlBmNamePrefix.length;

                for (var i = 1; i <= sheet.Names.Count; i++) {
                    bmName = sheet.Names(i).NameLocal.split('!')[1];

                    if (bmName.length <= dtlBmNamePrefix_len || bmName.substr(0, dtlBmNamePrefix_len) != dtlBmNamePrefix) {
                        continue;
                    }

                    colFields = bmName.substr(dtlBmNamePrefix_len).split('_');
                    formula = getFormula(colFields);

                    jwdtl.columns.push(new JWColumn(
                        formula.length > 0 ? colFields.slice(0, colFields.length - 1).join('_') : bmName.substr(dtlBmNamePrefix_len),
                        formula,
                        sheet.Names(i).NameLocal,
                        sheet.Names(i).RefersToRange.Column - firstCell.Column + 1));
                }
            }
        }

        var jwMains = new Array();
        var jwDtls = new Array();

        // 给excel 填充数据. 
        function replaceParams() {
            /// <summary>替换所有属性</summary>
            var params = $.parseJSON('<%=ReplaceParams %>'); //主表数据。
            var dtls = $.parseJSON('<%=ReplaceDtls %>'); //从表数据.
            var dtl,
                jwDtl;

            //替换主表数据, 整理数据符合预期的格式.
            $.each(params, function () {
                // 替换区域的值.
                replace(this.key, this.value.replace("\\\\", "\\").replace("\'", "'"), this.type);
            });

            //替换明细表数据,按照明细表名称，来检索明细表进行填充
            $.each(dtlNos, function () {
                dtl = getDtl(this, dtls);
                if (dtl == null) return true;

                jwdtl = new JWDtl(this, dtl);
                jwDtls.push(jwdtl);
                replaceDtl(this, dtl, jwdtl);
            });

            //接受修订, 接受修订后把数据作为一个版本保存. 调用 weboffice 的方法.
            webOffice.SetTrackRevisions(4);
        }

        function getDtl(dtlNo,dtls) {
            /// <summary>从明细表集合中获取指定no的明细表对象</summary>
            /// <param name="dtlNo" type="String">明细表no</param>
            /// <param name="dtls" type="Object">明细表集合</param>
            for (var i = 0, j = dtls.length; i < j; i++) {
                if (dtls[i][dtlNo] != undefined) {
                    return dtls[i][dtlNo];
                }
            }

            return null;
        }

        function replace(field, text, type) {
            /// <summary>替换文本</summary>
            /// <param name="oldStr" type="String">要替换的文本</param>
            /// <param name="newStr" type="String">要替换成的文本</param>
            /// <param name="type" type="String">值的类型，string/sign</param>

            var ccflow_bm_name_prefix = '';
            var na = getRangeName(field, ccflow_bm_name_prefix); //获得单元格区域对象.
            if (na != null && na.RefersToRange != null) {
                na.RefersToRange.Select();

                if (type == "sign") {
                    AddPicture(window.location.protocol + "//" + window.location.host + "/DataUser/Siganture/" + text + ".JPG", app.Selection, 180, 100);
                }
                else {
                    na.RefersToRange.Value = text;
                }

                jwMains.push(new JWField(field, text));
            }
        }

        function replaceDtl(dtlno, rows, jwdtl) {
            /// <summary>填充明细表数据</summary>
            /// <param name="dtlno" type="String">明细表No</param>
            /// <param name="rows" type="Array">明细表数据行集合</param>
            /// <param name="jwdtl" type="JWDtl">明细表数据行集合</param>
            jwdtl.excelName = 'dtl_' + dtlno;

            var ccflow_table_bm_prefix = jwdtl.excelName + '_';
            var ccflow_table = getRangeName(dtlno, 'dtl_');
            var dtlRange;

            if (ccflow_table != null) {
                //存储在内存中，在保存的时候好用
                jwdtl.excelRange = dtlRange = ccflow_table.RefersToRange;
                var currRowsCount = dtlRange.Rows.Count;

                //补全行数
                if (dtlRange.Rows.Count < rows.length + 1) {
                    dtlRange.Rows(currRowsCount).Select();
                    for (var i = rows.length + 1 - dtlRange.Rows.Count; i > 0; i--) {
                        app.Selection.Insert(-4121, 0);
                        dtlRange.Rows(dtlRange.Rows.Count).Select();
                        app.Selection.Copy();
                        dtlRange.Rows(currRowsCount).Select();
                        app.Selection.PasteSpecial(-4122, -4142, false, false);
                        app.CutCopyMode = false;
                    }
                }

                var cellIDPrefixLen = ccflow_table_bm_prefix.length;
                var cellNamePrefix = sheet.Name + '!' + ccflow_table_bm_prefix;
                var cellNamePrefixLen = cellNamePrefix.length;
                var firstRow = dtlRange.Rows(1);
                var firstCell;
                var colField;
                var formula;
                var arrCellFields;
                var isRowId;

                //填充数据
                for (var j = 1; j <= dtlRange.Columns.Count; j++) {
                    firstCell = dtlRange.Cells(1, j);

                    if (firstCell == null || firstCell.Value == undefined || firstCell.Name == null || firstCell.Name == undefined) {
                        continue;
                    }

                    if (firstCell.Name.Name.length <= cellNamePrefixLen || firstCell.Name.Name.substr(0, cellNamePrefixLen) != cellNamePrefix) {
                        continue;
                    }

                    arrCellFields = firstCell.Name.Name.substr(cellNamePrefixLen);
                    colField = getFieldName(arrCellFields);
                    formula = arrCellFields.length > colField.length ? arrCellFields.substr(colField.length + 1) : '';
                    isRowId = colField.toLowerCase() == 'rowid';

                    //记录填充列信息
                    jwdtl.columns.push(new JWColumn(colField, formula, firstCell.Name.Name, j));

                    //明细表中的列不考虑签名列的情况
                    for (var i = 0; i < rows.length; i++) {
                        //dtlRange.Rows(i + 2).Cells(j).Select();
                        dtlRange.Rows(i + 2).Cells(j).Value = isRowId ? (i+1).toString() : rows[i][colField];
                    }

                    //增加汇总
                    if (formula.length > 0) {
                        var colAlpha = String.fromCharCode(64 + firstCell.Column);
                        var formulaString = '=' + formula + '(' + colAlpha + (firstCell.Row + 1) + ':' + colAlpha + (firstCell.Row + rows.length) + ')';
                        var formulaFormat = '';
                        var formulaLower = formula.toLocaleLowerCase();

                        if (formulaLower == 'count' || formulaLower == 'counta') {
                            formulaFormat = '"总数："0';
                        }
                        else if (formulaLower == 'average') {
                            formulaFormat = '"平均："0.00';
                        }

                        //判断是否已经增加了汇总行
                        //汇总行不属于明细表所处的命名区域内
                        var dtlLastRow = dtlRange.Rows(dtlRange.Rows.Count).Row;

                        if (dtlRange.Rows.Count < rows.length + 2) {
                            sheet.Rows(dtlLastRow + 1).Select();
                            app.Selection.Insert(-4121);
                            sheet.Rows(dtlLastRow).Select();
                            app.Selection.Copy();
                            sheet.Rows(dtlLastRow + 1).Select();
                            app.Selection.PasteSpecial(-4122, -4142, false, false);
                            app.CutCopyMode = false;
                            app.Selection.ClearContents();
                        }

                        sheet.Cells(dtlLastRow + 1, firstCell.Column + j - 1).Select();
                        app.ActiveCell.FormulaLocal = formulaString;
                        app.Selection.NumberFormatLocal = formulaFormat;
                    }
                }
            }
        }

        function getFieldName(name) {
            /// <summary>获取属性名称，比如从DeptName_Sum中获得DeptName,去掉后面的是汇总函数名</summary>
            /// <param name="name" type="String">名称</param>
            var arr = name.split('_');
            if (arr.length < 2) return name;
            var last = arr[arr.length - 1];

            for (var i = 0, j = exceptions.length; i < j; i++) {
                if (last.toLowerCase() == exceptions[i]) {
                    return arr.slice(0, arr.length - 1).join('_');
                }
            }

            return name;
        }

        function getFormula(arr) {
            /// <summary>获取汇总函数名</summary>
            /// <param name="arr" type="Array">名称按照'_'进行拆分的数组</param>
            if (arr.length < 2) return '';
            var last = arr[arr.length - 1];

            for (var i = 0, j = exceptions.length; i < j; i++) {
                if (last.toLowerCase() == exceptions[i]) {
                    return last;
                }
            }

            return '';
        }

        // 根据前缀与字段名，返回他的区域对象，如果没有就返回空.
        function getRangeName(sName, sCcflowPrefix) {
            /// <summary>获取指定字段所标识的单元格区域名称对象</summary>
            /// <param name="sName" type="String">字段名称</param>
            /// <param name="sCcflowPrefix" type="String">ccflow的名称标识字符串</param>
            var nms = sheet.Names, //当前工作簿的命名区域集合.
                na = sheet.Name + '!' + sCcflowPrefix + sName,
                na_len = na.length;

            for (var i = 1; i <= nms.Count; i++) {
                if (sheet.Names(i).Name == na ||
                    (sheet.Names(i).Name.length > na_len &&
                    sheet.Names(i).Name.substr(0, na_len) == na && 
                    sheet.Names(i).Name.substr(na_len).split('_').length < 3)) {    //此处过滤掉有相同前缀的属性
                    return sheet.Names(i);
                }
            }
            return null;
        }

        function pageLoadding(msg) {
            $.jBox.tip(msg, 'loading');
        }

        function loaddingOut(msg) {
            $.jBox.tip(msg, 'success');
        }
    </script>
</head>
<body class="easyui-layout">
    <form id="form1" runat="server">
    <div data-options="region:'north',border:false,noheader:true" style="background-color: #E0ECFF;
        line-height: 30px; height: auto; padding: 2px">
        <div id="divMenu" runat="server">
        </div>
    </div>
    <div style="display: none">
        <asp:TextBox ID="fileName" runat="server" Text=""></asp:TextBox>
        <asp:TextBox ID="fileType" runat="server" Text=""></asp:TextBox>
    </div>
    <div data-options="region:'center',border:false,noheader:true">
        <script src="../Scripts/LoadWebOffice.js" type="text/javascript"></script>
    </div>
    </form>
</body>
</html>
