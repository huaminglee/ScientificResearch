using System;
using System.Web.UI.WebControls;
using System.Drawing;
using System.ComponentModel;


namespace BP.Web.Controls
{
    /// <summary>
    /// GenerButton 的摘要说明。
    /// </summary>
    [System.Drawing.ToolboxBitmap(typeof(System.Web.UI.WebControls.LinkButton))]
    public class LinkBtn : System.Web.UI.WebControls.LinkButton
    {
        public enum LinkBtnType
        {
            Normal,
            Confirm,
            Save,
            Search,
            Cancel,
            Delete,
            Update,
            Insert,
            Edit,
            New,
            View,
            Close,
            Export,
            Print,
            Add,
            Reomve,
            Up,
            Down
        }
        private LinkBtnType _ShowType = LinkBtnType.Normal;
        public LinkBtnType ShowType
        {
            get
            {
                return _ShowType;
            }
            set
            {
                this._ShowType = value;
            }
        }
        private string _Hit = null;
        /// <summary>
        /// 提示信息。
        /// </summary>
        public string Hit
        {
            get
            {
                return _Hit;
            }
            set
            {
                this._Hit = value;
            }
        }

        private bool _isPlain;

        /// <summary>
        /// 获取或设置是否平面样式
        /// </summary>
        public bool IsPlainStyle
        {
            get { return _isPlain; }
            set
            {
                SetDataOption("plain", value.ToString().ToLower());
                _isPlain = value;
            }
        }

        /// <summary>
        /// 设置data-options中的配置项
        /// </summary>
        /// <param name="optionKey">项名称</param>
        /// <param name="optionValue">项值</param>
        public void SetDataOption(string optionKey, object optionValue)
        {
            if (string.IsNullOrWhiteSpace(Attributes["data-options"]))
            {
                Attributes.Add("data-options", string.Format("{0}:{1}", optionKey, optionValue));
                return;
            }

            var ops = Attributes["data-options"].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var newOps = string.Empty;
            var isexist = false;

            foreach (var op in ops)
            {
                if (op.StartsWith(string.Format("{0}:", optionKey)))
                {
                    isexist = true;
                    newOps += string.Format("{0}:{1}", optionKey, optionValue) + ",";
                }
                else
                {
                    newOps += op + ",";
                }
            }

            if (!isexist)
                newOps += string.Format("{0}:{1}", optionKey, optionValue);

            Attributes["data-options"] = newOps.TrimEnd(',');
        }

        public LinkBtn()
            : this(true, null, "")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isPlain">是否平面样式</param>
        public LinkBtn(bool isPlain)
            : this(isPlain, null, "")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isPlain">是否平面样式</param>
        /// <param name="id">ID</param>
        public LinkBtn(bool isPlain, string id)
            : this(isPlain, id, "")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isPlain">是否平面样式</param>
        /// <param name="id">ID</param>
        /// <param name="text">文本</param>
        public LinkBtn(bool isPlain, string id, string text)
        {
            IsPlainStyle = isPlain;
            CssClass = "easyui-linkbutton";
            ID = id;
            Text = text;
            PreRender += new System.EventHandler(this.LinkBtnPreRender);
        }

        private void LinkBtnPreRender(object sender, System.EventArgs e)
        {
            if (this.Hit != null)
                this.Attributes["onclick"] = "javascript: return confirm('是否继续？'); ";

            switch (this.ID)
            {
                case NamesOfBtn.Save:
                case NamesOfBtn.SaveAndNew:
                    SetDataOption("iconCls", "'icon-save'");
                    break;
                case NamesOfBtn.SaveAndClose:
                    SetDataOption("iconCls", "'icon-save-close'");
                    break;
                case NamesOfBtn.Delete:
                    SetDataOption("iconCls", "'icon-delete'");
                    break;
                case NamesOfBtn.Reomve:
                    SetDataOption("iconCls", "'icon-remove'");
                    break;
                case NamesOfBtn.New:
                    SetDataOption("iconCls", "'icon-add'");
                    break;
                case NamesOfBtn.Search:
                    SetDataOption("iconCls", "'icon-search'");
                    break;
                case NamesOfBtn.Cancel:
                    SetDataOption("iconCls", "'icon-cancel'");
                    break;
                case NamesOfBtn.Print:
                    SetDataOption("iconCls", "'icon-print'");
                    break;
                case NamesOfBtn.Back:
                    SetDataOption("iconCls", "'icon-back'");
                    break;
                case NamesOfBtn.UnDo:
                    SetDataOption("iconCls", "'icon-undo'");
                    break;
                case NamesOfBtn.Edit:
                    SetDataOption("iconCls", "'icon-edit'");
                    break;
                case NamesOfBtn.Help:
                    SetDataOption("iconCls", "'icon-help'");
                    break;
                case NamesOfBtn.Up:
                    SetDataOption("iconCls", "'icon-up'");
                    break;
                case NamesOfBtn.Down:
                    SetDataOption("iconCls", "'icon-down'");
                    break;
                case NamesOfBtn.Excel:
                case NamesOfBtn.Export:
                    SetDataOption("iconCls", "'icon-excel'");
                    break;
                case NamesOfBtn.Open:
                    SetDataOption("iconCls", "'icon-open'");
                    break;
                case NamesOfBtn.Accept:
                    SetDataOption("iconCls", "'icon-accept'");
                    break;
                case NamesOfBtn.Refuse:
                    SetDataOption("iconCls", "'icon-refuse'");
                    break;
                case NamesOfBtn.Seal:
                    SetDataOption("iconCls", "'icon-seal'");
                    break;
                case NamesOfBtn.Picture:
                    SetDataOption("iconCls", "'icon-picture'");
                    break;
                case NamesOfBtn.FlowImage:
                    SetDataOption("iconCls", "'icon-flow'");
                    break;
                case NamesOfBtn.Download:
                    SetDataOption("iconCls", "'icon-download'");
                    break;
                default:
                    break;
            }

            return;

            switch (this.ShowType)
            {
                case LinkBtnType.Edit:
                    if (this.Text == null || this.Text == "")
                        this.Text = "修改(E)";
                    if (this.AccessKey == null)
                        this.AccessKey = "e";
                    break;
                case LinkBtnType.Close:
                    if (this.Text == null || this.Text == "")
                        this.Text = "关闭(Q)";
                    if (this.AccessKey == null)
                        this.AccessKey = "q";
                    break;
                case LinkBtnType.Cancel:
                    if (this.Text == null || this.Text == "")
                        this.Text = "取消(C)";
                    if (this.AccessKey == null)
                        this.AccessKey = "c";
                    break;
                case LinkBtnType.Confirm:
                    if (this.Text == null || this.Text == "")
                        this.Text = "确定(O)";
                    if (this.AccessKey == null)
                        this.AccessKey = "o";
                    break;
                case LinkBtnType.Search:
                    if (this.Text == null || this.Text == "")
                        this.Text = "查找(F)";
                    if (this.AccessKey == null)
                        this.AccessKey = "f";
                    break;
                case LinkBtnType.New:
                    if (this.Text == null || this.Text == "")
                        this.Text = "新建(N)";
                    if (this.AccessKey == null)
                        this.AccessKey = "n";
                    break;
                case LinkBtnType.Delete:
                    if (this.Text == null || this.Text == "")
                        this.Text = "删除(D)";
                    if (this.AccessKey == null)
                        this.AccessKey = "c";
                    if (this.Hit == null)
                        this.Attributes["onclick"] = " return confirm('此操作要执行删除，是否继续？');";
                    else
                        this.Attributes["onclick"] = " return confirm('此操作要执行删除　[" + this.Hit + "]，是否继续？');";

                    break;
                case LinkBtnType.Export:
                    if (this.Text == null || this.Text == "")
                        this.Text = "导出(G)";
                    if (this.AccessKey == null)
                        this.AccessKey = "g";
                    break;
                case LinkBtnType.Insert:
                    if (this.Text == null || this.Text == "")
                        this.Text = "插入(I)";
                    if (this.AccessKey == null)
                        this.AccessKey = "i";
                    break;
                case LinkBtnType.Print:
                    if (this.Text == null || this.Text == "")
                        this.Text = "打印(P)";
                    if (this.AccessKey == null)
                        this.AccessKey = "p";

                    if (this.Hit == null)
                        this.Attributes["onclick"] = " return confirm('此操作要执行打印，是否继续？');";
                    else
                        this.Attributes["onclick"] = " return confirm('此操作要执行打印　[" + this.Hit + "]，是否继续？');";
                    break;
                case LinkBtnType.Save:
                    if (this.Text == null || this.Text == "")
                        this.Text = "保存(S)";
                    if (this.AccessKey == null)
                        this.AccessKey = "s";
                    break;
                case LinkBtnType.View:
                    if (this.Text == null || this.Text == "")
                        this.Text = "浏览(V)";
                    if (this.AccessKey == null)
                        this.AccessKey = "v";
                    break;
                case LinkBtnType.Add:
                    if (this.Text == null || this.Text == "")
                        this.Text = "增加(A)";
                    if (this.AccessKey == null)
                        this.AccessKey = "a";
                    break;
                case LinkBtnType.Reomve:
                    if (this.Text == null || this.Text == "")
                        this.Text = "移除(M)";
                    if (this.AccessKey == null)
                        this.AccessKey = "m";

                    if (this.Hit == null)
                        this.Attributes["onclick"] = " return confirm('此操作要执行移除，是否继续？');";
                    else
                        this.Attributes["onclick"] = " return confirm('此操作要执行移除　[" + this.Hit + "]，是否继续？');";
                    break;
                case LinkBtnType.Up:
                    break;
                case LinkBtnType.Down:
                    break;
                default:
                    if (this.Text == null || this.Text == "")
                        this.Text = "确定(O)";
                    if (this.AccessKey == null)
                        this.AccessKey = "o";
                    break;
            }

            //this.PublicScheme();			 
            //this.StyleScheme();	
        }
        private void PublicScheme()
        {
            if (this.Text == null || this.Text == "")
            {
                this.Text = "确定(O)";
            }
            this.BorderStyle = BorderStyle.Ridge;
            //this.Font.Name="华文中宋";
            //this.BorderWidth=Unit.Pixel(1); 
        }

        public void StyleScheme()
        {
            //this.BorderStyle=BorderStyle="Ridge"
            if (WebUser.Style == "1")
                this.Style1();
            else if (WebUser.Style == "2")
                this.Style2();
            else
                this.Style3();

        }
        public void Style3()
        {
            this.BorderColor = Color.Transparent;
            this.BackColor = Color.FromName("#006699");
            this.ForeColor = Color.White;
        }
        public void Style2()
        {
            this.BorderColor = System.Drawing.Color.FromName("#DEBA84");
            this.BackColor = Color.FromName("#DEBA84");
            this.ForeColor = Color.Black;
        }
        /// <summary>
        /// Style1
        /// </summary>
        public void Style1()
        {

        }
    }
}
