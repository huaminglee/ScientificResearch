using System;
using System.Web.UI.WebControls;
using System.Drawing;
using System.ComponentModel;


namespace BP.Web.Controls
{
    /// <summary>
    /// GenerButton ��ժҪ˵����
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
        /// ��ʾ��Ϣ��
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
        /// ��ȡ�������Ƿ�ƽ����ʽ
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
        /// ����data-options�е�������
        /// </summary>
        /// <param name="optionKey">������</param>
        /// <param name="optionValue">��ֵ</param>
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
        /// <param name="isPlain">�Ƿ�ƽ����ʽ</param>
        public LinkBtn(bool isPlain)
            : this(isPlain, null, "")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isPlain">�Ƿ�ƽ����ʽ</param>
        /// <param name="id">ID</param>
        public LinkBtn(bool isPlain, string id)
            : this(isPlain, id, "")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isPlain">�Ƿ�ƽ����ʽ</param>
        /// <param name="id">ID</param>
        /// <param name="text">�ı�</param>
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
                this.Attributes["onclick"] = "javascript: return confirm('�Ƿ������'); ";

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
                        this.Text = "�޸�(E)";
                    if (this.AccessKey == null)
                        this.AccessKey = "e";
                    break;
                case LinkBtnType.Close:
                    if (this.Text == null || this.Text == "")
                        this.Text = "�ر�(Q)";
                    if (this.AccessKey == null)
                        this.AccessKey = "q";
                    break;
                case LinkBtnType.Cancel:
                    if (this.Text == null || this.Text == "")
                        this.Text = "ȡ��(C)";
                    if (this.AccessKey == null)
                        this.AccessKey = "c";
                    break;
                case LinkBtnType.Confirm:
                    if (this.Text == null || this.Text == "")
                        this.Text = "ȷ��(O)";
                    if (this.AccessKey == null)
                        this.AccessKey = "o";
                    break;
                case LinkBtnType.Search:
                    if (this.Text == null || this.Text == "")
                        this.Text = "����(F)";
                    if (this.AccessKey == null)
                        this.AccessKey = "f";
                    break;
                case LinkBtnType.New:
                    if (this.Text == null || this.Text == "")
                        this.Text = "�½�(N)";
                    if (this.AccessKey == null)
                        this.AccessKey = "n";
                    break;
                case LinkBtnType.Delete:
                    if (this.Text == null || this.Text == "")
                        this.Text = "ɾ��(D)";
                    if (this.AccessKey == null)
                        this.AccessKey = "c";
                    if (this.Hit == null)
                        this.Attributes["onclick"] = " return confirm('�˲���Ҫִ��ɾ�����Ƿ������');";
                    else
                        this.Attributes["onclick"] = " return confirm('�˲���Ҫִ��ɾ����[" + this.Hit + "]���Ƿ������');";

                    break;
                case LinkBtnType.Export:
                    if (this.Text == null || this.Text == "")
                        this.Text = "����(G)";
                    if (this.AccessKey == null)
                        this.AccessKey = "g";
                    break;
                case LinkBtnType.Insert:
                    if (this.Text == null || this.Text == "")
                        this.Text = "����(I)";
                    if (this.AccessKey == null)
                        this.AccessKey = "i";
                    break;
                case LinkBtnType.Print:
                    if (this.Text == null || this.Text == "")
                        this.Text = "��ӡ(P)";
                    if (this.AccessKey == null)
                        this.AccessKey = "p";

                    if (this.Hit == null)
                        this.Attributes["onclick"] = " return confirm('�˲���Ҫִ�д�ӡ���Ƿ������');";
                    else
                        this.Attributes["onclick"] = " return confirm('�˲���Ҫִ�д�ӡ��[" + this.Hit + "]���Ƿ������');";
                    break;
                case LinkBtnType.Save:
                    if (this.Text == null || this.Text == "")
                        this.Text = "����(S)";
                    if (this.AccessKey == null)
                        this.AccessKey = "s";
                    break;
                case LinkBtnType.View:
                    if (this.Text == null || this.Text == "")
                        this.Text = "���(V)";
                    if (this.AccessKey == null)
                        this.AccessKey = "v";
                    break;
                case LinkBtnType.Add:
                    if (this.Text == null || this.Text == "")
                        this.Text = "����(A)";
                    if (this.AccessKey == null)
                        this.AccessKey = "a";
                    break;
                case LinkBtnType.Reomve:
                    if (this.Text == null || this.Text == "")
                        this.Text = "�Ƴ�(M)";
                    if (this.AccessKey == null)
                        this.AccessKey = "m";

                    if (this.Hit == null)
                        this.Attributes["onclick"] = " return confirm('�˲���Ҫִ���Ƴ����Ƿ������');";
                    else
                        this.Attributes["onclick"] = " return confirm('�˲���Ҫִ���Ƴ���[" + this.Hit + "]���Ƿ������');";
                    break;
                case LinkBtnType.Up:
                    break;
                case LinkBtnType.Down:
                    break;
                default:
                    if (this.Text == null || this.Text == "")
                        this.Text = "ȷ��(O)";
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
                this.Text = "ȷ��(O)";
            }
            this.BorderStyle = BorderStyle.Ridge;
            //this.Font.Name="��������";
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
