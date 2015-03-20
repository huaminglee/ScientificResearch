<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebOffice.aspx.cs" Inherits="CCFlow.WF.WorkOpt.WebOffice"
    Async="true" %>

<%@ Register Src="../Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>公文正文</title>
    <script src="../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../Scripts/jBox/jquery.jBox-2.3.min.js" type="text/javascript"></script>
    <link href="../Scripts/jBox/Skins/Blue/jbox.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <link href="../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <script src="../Comm/JScript.js" type="text/javascript"></script>
    <style type="text/css">
        .btn
        {
            border: 0;
            background: #4D77A7;
            color: #FFF;
            font-size: 12px;
            padding: 6px 10px;
            margin: 5px;
        }
    </style>
    <script type="text/javascript">
        var isShowAll = false;
        var webOffice = null;
        var strTimeKey;
        var isOpen = false;
        var isInfo = true;
        var marksType = "doc,docx";

        $(function () {
            InitOffice();
        });


        //初始化公文
        function InitOffice() {
            webOffice = document.all.WebOffice1;

            if ($('#fileName').val() != "") {

                if ('<%=IsLoadTempLate %>' == 'True')
                    OpenWeb("0");
                else
                    OpenWeb("1");
            }
            EnableMenu();
        }
        //设置当前操作用户
        function SetUsers() {
            try {
                webOffice.SetCurrUserName("<%=UserName %>");

                InitShowName();

            } catch (e) {
                $.jBox.alert("异常\r\nError:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }
        }
        var isUser = false;
        //显示指定用户的留痕
        function ShowUserName() {
            /// <summary>
            /// 显当前用户留痕
            /// </summary>

            try {

                var user = $("#marks option:selected").val();

                if (user == "显示留痕" || user == undefined) {
                    if (isShowAll) {
                        webOffice.GetDocumentObject().Application.ActiveWindow.ToggleShowAllReviewers();
                        isShowAll = false;
                    }



                } else if (user == "隐藏留痕") {


                    if (!isShowAll) {
                        webOffice.GetDocumentObject().Application.ActiveWindow.ToggleShowAllReviewers();
                        isShowAll = true;
                    }
                    else {
                        webOffice.GetDocumentObject().Application.ActiveWindow.ToggleShowAllReviewers();
                        webOffice.GetDocumentObject().Application.ActiveWindow.ToggleShowAllReviewers();
                    }
                }
                else {
                    //                    if (!isShowAll) {
                    //                        webOffice.GetDocumentObject().Application.ActiveWindow.ToggleShowAllReviewers();
                    //                        webOffice.GetDocumentObject().Application.ActiveWindow.ToggleShowAllReviewers();
                    //                        isShowAll = true;
                    //                    }
                    if (!isShowAll) {
                        webOffice.GetDocumentObject().Application.ActiveWindow.ToggleShowAllReviewers();
                        isShowAll = true
                    }
                    else {
                        webOffice.GetDocumentObject().Application.ActiveWindow.ToggleShowAllReviewers();
                        webOffice.GetDocumentObject().Application.ActiveWindow.ToggleShowAllReviewers();
                    }


                    webOffice.GetDocumentObject().Application.ActiveWindow.View.Reviewers(user).Visible = true;
                }
            } catch (e) {
                $.jBox.alert("异常\r\nError:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }
        }
        //加载所留痕用户
        function InitShowName() {
            try {
                var count = webOffice.GetRevCount();

                var showName = $("#marks");
                showName.empty();

                var list = "显示留痕,隐藏留痕,";

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

            }
        }

        //隐藏 公文按钮
        function EnableMenu() {
            /// <summary>
            /// 设置按钮
            /// </summary>
            webOffice.HideMenuItem(0x01 + 0x02 + 0x04 + 0x10 + 0x20);
        }
        //设置留痕,显示所有的留痕用户,是否只读文档
        function SetTrack(track) {
            if ("<%=ReadOnly %>" == "True") {
                webOffice.ProtectDoc(1, 2, "");
            }
            else {
                webOffice.ProtectDoc(0, 1, "");
            }
            webOffice.SetTrackRevisions(track);

            SetUsers();
            var types = $('#fileType').val();
            var type = webOffice.IsOpened();
            //如果打开的是word
            if (type == 11) {
                ShowUserName();
            }
        }
        //打开本地文件
        function OpenFile() {
            pageLoadding('正在打开...');
            try {
                if (readOnly()) {
                    return false;
                }

                OpenDoc("open", "doc");
            } catch (e) {
                $.jBox.alert("异常\r\nError:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }
            loaddingOut('打开完成');
            return false;
        }
        function OpenTempLate() {
            if (readOnly()) {
                return false;
            }
            LoadTemplate('word', '公文模板', "File", OpenWeb);

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
                $.jBox.alert("异常\r\nError:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }
            return false;
        }

        function pageLoadding(msg) {
            $.jBox.tip(msg, 'loading');
        }

        function loaddingOut(msg) {
            $.jBox.tip(msg, 'success');
        }

        function DownLoad() {
            try {
                if (isOpen) {
                    webOffice.ProtectDoc(0, 1, "");
                    webOffice.ShowDialog(84);
                } else {
                    $.jBox.alert('请打开公文!', '提示');
                }
            } catch (e) {
                $.jBox.alert("异常\r\nError:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }
            webOffice.ProtectDoc(1, 2, "");
            return false;
        }
        //打开服务器文件
        function OpenWeb(loadtype) {

            try {
                var type = $("#fileType").val();
                var fileName = $('#fileName').val();
                var url = location.href + "&action=LoadFile&LoadType=" + loadtype + "&fileName=" + fileName;
                OpenDoc(url, type);
            } catch (e) {
                $.jBox.alert("异常\r\nError:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }


        }
        //打开文件
        function OpenDoc(url, type) {
            var openType = webOffice.LoadOriginalFile(url, type);
            if (openType > 0) {

                var type = webOffice.IsOpened();
                //如果打开的是word
                if (type == 11) {

                    if ("<%=IsMarks %>" == "True")
                        SetTrack(1);
                    else
                        SetTrack(0);

                    if ("<%=IsTrueTH %>" == "True" && "<%=IsTrueTHTemplate %>" != "") {
                        TaoHong();
                    }
                }


                isOpen = true;


            } else {
                $.jBox.alert('打开文档失败', '异常');
            }
        }
        //加载模板
        function LoadTemplate(type, title, loadType, callback) {
            try {
                $.jBox("iframe:/WF/WebOffice/TempLate.aspx?LoadType=" + type + "&Type=" + loadType, {
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

                            if (type == "word") {
                                $("#fileName").val(row.Name);
                                $("#fileType").val(row.Type);
                            } else {
                                $("#sealName").val(row.Name);
                                $("#sealType").val(row.Type);
                            }
                            callback();
                            loaddingOut('打开完成...');
                            return true;
                        }
                    }
                });
            } catch (e) {
                $.jBox.alert("异常\r\nError:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }
        }
        //加载红头文件模板
        function overOffice() {
            if (readOnly()) {
                return false;
            }
            if (isOpen) {

                LoadTemplate('over', '套红模板', "File", TaoHong);
            } else {
                $.jBox.alert('请打开公文!', '提示');
            }
            return false;
        }
        //套红头文件
        function TaoHong() {
            try {

                var mark = AddBooks();
                var name = $('#sealName').val();
                var type = $('#sealType').val();
                var url = window.location.protocol + "//" + window.location.host + "/DataUser/OfficeOverTemplate/" + name;

                if (type == "png" || type == "jpg" || type == "bmp") {
                    webOffice.SetFieldValue(mark, url, "::JPG::");
                } else {
                    webOffice.SetFieldValue(mark, url, "::FILE::");
                }
            } catch (e) {
                $.jBox.alert("异常\r\nError:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }
        }
        //保存到服务器
        function saveOffice() {
            if (readOnly()) {
                return false;
            }
            var type = webOffice.IsOpened();

            pageLoadding('正在保存...');
            try {
                if (isOpen) {
                    //如果打开的是word
                    if (type == 11) {
                        if ("<%=IsCheckInfo %>" == "True") {
                            if (isInfo) {
                                isInfo = false;
                                webOffice.GetDocumentObject().Application.ActiveDocument.Content.InsertAfter("\n<%=NodeInfo %>");
                            }
                        }
                    }

                    webOffice.HttpInit();
                    webOffice.HttpAddPostCurrFile("File", "");
                    var src = location.href + "&action=SaveFile";

                    webOffice.HttpPost(src);


                } else {
                    $.jBox.alert('请打开公文!', '提示');

                }
            } catch (e) {

                $.jBox.alert("异常\r\nError:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }
            loaddingOut('保存完成...');
            var types = $('#fileType').val();


            try {
                if (type == 11) {
                    InitShowName();
                }

            } catch (e) {

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
                $.jBox.alert("异常\r\nError:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');

            }
            return false;

        }
        //接受修订
        function acceptOffice() {
            try {
                if (readOnly()) {
                    return false;
                }
                webOffice.SetTrackRevisions(4);
            } catch (e) {
                $.jBox.alert("异常\r\nError:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');

            }
            return false;
        }
        //文档只读提示
        function readOnly() {
            if ("<%=ReadOnly %>" == "True") {
                $.jBox.alert('文档只读不能进行此操作!', '提示');
                return true;
            }
        }
        //加载所有的公文印章
        function sealOffice() {

            if (readOnly()) {
                return false;
            }
            if (isOpen) {
                LoadTemplate('seal', '公文盖章', "File", seal);
            } else {
                $.jBox.alert('请打开公文!', '提示');
            }
            return false;
        }
        //盖章
        function seal() {
            try {
                var name = $('#sealName').val();
                var type = $('#sealType').val();
                var url = window.location.protocol + "//" + window.location.host + "/DataUser/OfficeSeal/" + name;

                //                var pRange = webOffice.GetDocumentObject().Application.ActiveCell;
                //                webOffice.GetDocumentObject().Application.ActiveSheet.Shapes.AddPicture(url, 1, 1, pRange.Left, pRange.Top, -1, -1);
                var mark = AddBooks();
                //webOffice.InSertFile(url, 8);
                AddPicture(mark, url, 5);

            } catch (e) {
                $.jBox.alert("异常\r\nError:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');

            }
        }
        //添加书签
        function AddBooks() {

            var date = new Date().getFullYear() + "" + new Date().getMonth() + "" + new Date().getDay() + "" + new Date().getHours() + "" + new Date().getMinutes() + "" + new Date().getSeconds();
            var strMarkName = "mark_" + date;
            var obj = new Object(webOffice.GetDocumentObject());
            if (obj != null) {
                var pBookM;
                var pBookMarks;
                // VAB接口获取书签集合
                pBookMarks = obj.Bookmarks;
                try {
                    pBookM = pBookMarks(strMarkName);
                    return pBookM.Name;
                } catch (e) {
                    webOffice.SetFieldValue(strMarkName, "", "::ADDMARK::");
                }
            }
            return strMarkName;
        }
        //通过VBA 来插入图片
        function AddPicture(strMarkName, strBmpPath, vType) {
            //定义一个对象，用来存储ActiveDocument对象
            var obj = new Object(webOffice.GetDocumentObject());
            if (obj != null) {
                var pBookMarks;
                // VAB接口获取书签集合
                pBookMarks = obj.Bookmarks;
                var pBookM;
                // VAB接口获取书签strMarkName
                pBookM = pBookMarks(strMarkName);
                var pRange;
                // VAB接口获取书签strMarkName的Range对象
                pRange = pBookM.Range;
                var pRangeInlines;
                // VAB接口获取书签strMarkName的Range对象的InlineShapes对象
                pRangeInlines = pRange.InlineShapes;
                var pRangeInline;
                // VAB接口通过InlineShapes对象向文档中插入图片
                pRangeInline = pRangeInlines.AddPicture(strBmpPath, 128);
                //设置图片的样式，5为浮动在文字上面
                pRangeInline.ConvertToShape().WrapFormat.TYPE = vType;
                delete obj;
            }
        }
        ///插入文件测试
        function InsertFileWeb() {
            var url = window.location.protocol + "//" + window.location.host + "/DataUser/OfficeFile/099/112.docx";
            webOffice.LoadOriginalFile(url, "docx");
        }

        function InsertFlow() {
            if (readOnly()) {
                return false;
            }
            try {
                if (isOpen) {
                    LoadTemplate('flow', '流程图', "Dic", FlowInsert);
                } else {
                    $.jBox.alert('请打开公文!', '提示');

                }
            } catch (e) {
                $.jBox.alert("异常\r\nError:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }
            return false;
        }
        function FlowInsert() {
            var name = $('#sealName').val();
            var type = $('#sealType').val();
            var url = window.location.protocol + "//" + window.location.host + "/DataUser/FlowDesc/" + name + "/" + name.replace(".", "_") + ".png";
            var mark = AddBooks();
            //webOffice.InSertFile(url, 8);
            AddPicture(mark, url, 5);
        }


        function closeDoc() {
            webOffice.SetCurrUserName("");
            webOffice.closeDoc(0);
        }

        ///设置word页边距字体
        function SettingWordFont() {
            try {
                //                var obj = new Object(webOffice.GetDocumentObject());
                //                var wdapp = new ActiveXObject("Word.Application")
                var obj = webOffice.GetDocumentObject().Application;


                obj.ActiveDocument.PageSetup.TopMargin = obj.CentimetersToPoints(parseFloat("3.7")); //上页边距
                obj.ActiveDocument.PageSetup.BottomMargin = obj.CentimetersToPoints(parseFloat("3.5")); //下页边距
                obj.ActiveDocument.PageSetup.LeftMargin = obj.CentimetersToPoints(parseFloat("2.8")); //左页边距
                obj.ActiveDocument.PageSetup.RightMargin = obj.CentimetersToPoints(parseFloat("2.6")); //右页边距

                obj.Selection.Font.NameFarEast = "仿宋_GB2312";
                //                obj.Selection.Font.NameAscii = "Times New Roman";
                //                obj.Selection.Font.NameOther = "Times New Roman";

                obj.Selection.Font.Name = "仿宋_GB2312";
                obj.Selection.Font.Size = parseFloat("16");
                obj.Selection.Font.Bold = 0;
                obj.Selection.Font.Italic = 0;
                //                obj.Selection.Font.Underline = Microsoft.Office.Interop.Word.WdUnderline.wdUnderlineNone;
                //                obj.Selection.Font.UnderlineColor = Microsoft.Office.Interop.Word.WdColor.wdColorAutomatic;
                //                obj.Selection.Font.StrikeThrough = 0; //删除线
                //                obj.Selection.Font.DoubleStrikeThrough = 0; //双删除线
                //                obj.Selection.Font.Outline = 0; //空心
                //                obj.Selection.Font.Emboss = 0; //阳文
                //                obj.Selection.Font.Shadow = 0; //阴影
                //                obj.Selection.Font.Hidden = 0; //隐藏文字
                //                obj.Selection.Font.SmallCaps = 0; //小型大写字母
                //                obj.Selection.Font.AllCaps = 0; //全部大写字母
                //                obj.Selection.Font.Color = Microsoft.Office.Interop.Word.WdColor.wdColorAutomatic;
                //                obj.Selection.Font.Engrave = 0; //阴文
                //                obj.Selection.Font.Superscript = 0; //上标
                //                obj.Selection.Font.Subscript = 0; //下标
                //                obj.Selection.Font.Spacing = float.Parse("0"); //字符间距
                //                obj.Selection.Font.Scaling = 100; //字符缩放
                //                obj.Selection.Font.Position = 0; //位置
                //                obj.Selection.Font.Kerning = float.Parse("1"); //字体间距调整
                //                obj.Selection.Font.Animation = Microsoft.Office.Interop.Word.WdAnimation.wdAnimationNone; //文字效果
                //                obj.Selection.Font.DisableCharacterSpaceGrid = false;
                //                obj.Selection.Font.EmphasisMark = Microsoft.Office.Interop.Word.WdEmphasisMark.wdEmphasisMarkNone;
            } catch (e) {
                $.jBox.alert("异常\r\nError:" + e + "\r\nError Code:" + e.number + "\r\nError Des:" + e.description, '异常');
            }
        }

    </script>
</head>
<body style="padding: 0px; margin: 0px;" onunload="closeDoc()" class="easyui-layout">
    <form id="form1" runat="server">
    <div data-options="region:'north',border:false,noheader:true" style="background-color: #E0ECFF;
        line-height: 30px; height: auto; padding: 2px">
        <div id="divMenu" runat="server">
        </div>
    </div>
    <div style="display: none;">
        <asp:TextBox ID="fileName" runat="server" Text=""></asp:TextBox>
        <asp:TextBox ID="fileType" runat="server" Text=""></asp:TextBox>
        <asp:TextBox ID="sealName" runat="server" Text=""></asp:TextBox>
        <asp:TextBox ID="sealType" runat="server" Text=""></asp:TextBox>
    </div>
    <div data-options="region:'center',border:false,noheader:true">
        <script src="../Scripts/LoadWebOffice.js" type="text/javascript"></script>
    </div>
    <div data-options="region:'south'" title="附件" style="height: 150px;background-color: #E0ECFF;">
        <uc1:Pub ID="Pub1" runat="server" />
    </div>
    </form>
</body>
</html>
