using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BP.En;
using BP.DA;
using BP.Web.Controls;
using BP.Sys;
using Microsoft.Web.UI.WebControls;

namespace BP.Web.Comm.UI
{
	/// <summary>
	/// UIEn1ToM ��ժҪ˵����
	/// </summary>
    public partial class UIEn1ToM : WebPage
    {

        #region ����.
        public AttrOfOneVSM AttrOfOneVSM
        {
            get
            {
                Entity en = ClassFactory.GetEn(this.EnsName);
                foreach (AttrOfOneVSM attr in en.EnMap.AttrsOfOneVSM)
                {
                    if (attr.EnsOfMM.ToString() == this.AttrKey)
                    {
                        return attr;
                    }
                }
                throw new Exception("����û���ҵ����ԣ� ");
            }
        }
        /// <summary>
        /// һ�Ĺ�����
        /// </summary>
        public new string EnsName
        {
            get
            {
                return this.Request.QueryString["EnsName"];
            }
        }
        public string AttrKey
        {
            get
            {
                return this.Request.QueryString["AttrKey"];
            }
        }
        public new string PK
        {
            get
            {
                if (ViewState["PK"] == null)
                {
                    string pk = this.Request.QueryString["PK"];
                    if (pk == null)
                        pk = this.Request.QueryString["No"];

                    if (pk == null)
                        pk = this.Request.QueryString["RefNo"];

                    if (pk == null)
                        pk = this.Request.QueryString["OID"];

                    if (pk == null)
                        pk = this.Request.QueryString["MyPK"];



                    if (this.Request.QueryString["PK"] != null)
                    {
                        ViewState["PK"] = pk;
                    }
                    else
                    {
                        Entity mainEn = ClassFactory.GetEn(this.EnsName);
                        ViewState["PK"] = this.Request.QueryString[mainEn.PK];
                    }
                }
                return (string)ViewState["PK"];
            }
        }
        public DropDownList DDL_Group
        {
            get
            {
                return this.ToolBar1.GetDropDownListByID("DDL_Group");
            }
        }

        public bool IsLine
        {
            get
            {
                try
                {
                    return (bool)ViewState["IsLine"];
                }
                catch
                {
                    return false;
                }
            }
            set
            {
                ViewState["IsLine"] = value;
            }
        }
        public string MainEnName
        {
            get
            {
                return ViewState["MainEnName"] as string;
            }
            set
            {
                this.ViewState["MainEnName"] = value;
            }
        }
        public string MainEnPKVal
        {
            get
            {
                return ViewState["MainEnPKVal"] as string;
            }
            set
            {
                this.ViewState["MainEnPKVal"] = value;
            }
        }
        public bool IsTreeShowWay
        {
            get
            {
                if (this.Request.QueryString["IsTreeShowWay"] != null)
                    return true;
                return false;
            }
        }
        /// <summary>
        /// ��ʾ��ʽ
        /// </summary>
        public string ShowWay
        {
            get
            {
                return this.Request.QueryString["ShowWay"];
            }
        }
        public string MainEnPK
        {
            get
            {
                return ViewState["MainEnPK"] as string;
            }
            set
            {
                this.ViewState["MainEnPK"] = value;
            }
        }
        private Entity _MainEn = null;
        public Entity MainEn
        {
            get
            {
                if (_MainEn == null)
                    _MainEn = ClassFactory.GetEn(this.Request.QueryString["EnsName"]);
                return _MainEn;
            }
            set
            {
                _MainEn = value;
            }
        }
        public int ErrMyNum = 0;
        #endregion ����.

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            string url = this.Request.RawUrl.Replace("UIEn1ToM.aspx", "/RefFunc/Dot2Dot.aspx");
            this.Response.Redirect(url, true);
            return;




            try
            {
                #region ������������� ��ʵ�� ��ҵ���߼���
                Entity enP = ClassFactory.GetEn(this.Request.QueryString["EnsName"]);
                this.MainEnName = enP.EnDesc;
                this.MainEnPKVal = this.PK;
                this.MainEnPK = enP.PK;

                if (enP.EnMap.EnType != EnType.View)
                {
                    enP.SetValByKey(enP.PK, this.PK);// =this.PK;
                    enP.Retrieve(); //��ѯ��
                    enP.Update(); // ִ�и��£�����д�� ��ʵ�� ��ҵ���߼���
                }
                MainEn = enP;
                #endregion
            }
            catch (Exception ex)
            {
                this.ToErrorPage(ex.Message);
                //this.WinClose();
                return;
            }

            AttrOfOneVSM ensattr = this.AttrOfOneVSM;
            //this.Label1.Text = ensattr.Desc ;
            this.ToolBar1.AddLab("lab_desc",   "����:");
            DropDownList ddl = new DropDownList();
            ddl.ID = "DDL_Group";
            ddl.AutoPostBack = true;
            this.ToolBar1.Add(ddl);

            ddl.Items.Clear();
            ddl.SelectedIndexChanged += new EventHandler(DDL_Group_SelectedIndexChanged);

            Entity open = ensattr.EnsOfM.GetNewEntity;
            Map map = open.EnMap;
            int len = 0;
            switch (map.EnDBUrl.DBType)
            {
                case DBType.Oracle:
                    len = DBAccess.RunSQLReturnValInt("SELECT MAX( Length( " + ensattr.AttrOfMText + " )) FROM " + map.PhysicsTable + "");
                    break;
                default:
                    len = DBAccess.RunSQLReturnValInt("SELECT MAX( Len( " + ensattr.AttrOfMText + " )) FROM " + map.PhysicsTable + "");
                    break;
            }


            // ������ ����  �� 15 ���ȡ�����һ����ʾ��
            if (len > 20)
                this.IsLine = true;
            else
                this.IsLine = false;

            // �ȼ���enum ���͡�
            foreach (Attr attr in map.Attrs)
            {
                /* map */
                if (attr.MyFieldType != FieldType.Enum)
                    continue;

                this.DDL_Group.Items.Add(new ListItem(attr.Desc, attr.Key));
            }

            // �ȼ���enum ���͡�
            foreach (Attr attr in map.Attrs)
            {
                /* map */
                if (attr.MyFieldType != FieldType.FK)
                    continue;
                this.DDL_Group.Items.Add(new ListItem(attr.Desc, attr.Key));
            }
            this.DDL_Group.Items.Add(new ListItem("��", "None"));
            foreach (ListItem li in ddl.Items)
            {
                if (li.Value == this.ShowWay)
                    li.Selected = true;
            }


            this.ToolBar1.AddSpt("spt");
            if (ensattr.EnsOfMM.GetNewEntity.HisUAC.IsInsert == true)
            {
                this.ToolBar1.AddBtn("Btn_Save",   "����");
               // this.ToolBar1.AddBtn("Btn_SaveAndClose", "���沢�ر�");
            }

            this.ToolBar1.Add("<input type=checkbox value='ѡ��ȫ��' text='ѡ��ȫ��'  name=checkedAll onclick='SelectAll()' >");

            //checkedAll

            this.ToolBar1.AddBtn("Btn_Close", "�ر�");

            this.DDL_Group.SelectedIndexChanged += new EventHandler(DDL_Group_SelectedIndexChanged);

            #region ���Ӱ�ť�¼�
            try
            {
                this.ToolBar1.GetBtnByID("Btn_Save").Click += new EventHandler(BPToolBar1_ButtonClick);
            }
            catch
            {
            }

            try
            {
                this.ToolBar1.GetBtnByID("Btn_SaveAndClose").Click += new EventHandler(BPToolBar1_ButtonClick);
            }
            catch
            {

            }
            this.ToolBar1.GetBtnByID("Btn_Close").Click += new EventHandler(BPToolBar1_ButtonClick);
            //this.ToolBar1.GetBtnByID("Btn_Help").Click += new EventHandler(BPToolBar1_ButtonClick);
            #endregion

            this.SetDataV2();

            if (this.IsTreeShowWay == false)
                this.SetDataV2();

            // Entity en1 = ClassFactory.GetEn(this.Request.QueryString["EnsName"]);
            string keys = "&" + this.MainEnPK + "=" + this.PK + "&r=" + DateTime.Now.ToString("MMddhhmmss");
            string refstrs = BP.Web.Comm.UC.UCEn.GetRefstrs1(keys, MainEn, MainEn.GetNewEntities);
            this.Label1.Text = this.GenerCaption(this.MainEnName + " ��ع���:" + refstrs);
            // this.GenerLabel(this.Label1, );
            //if (mycb.Selected)
            //    this.BindTree();
            //else
            //    this.SetDataV2();
        }
        /// <summary>
        /// SetDataV2
        /// </summary>
        public void SetDataV2()
        {
            this.Tree1.Nodes.Clear();
            this.UCSys1.ClearViewState();

            AttrOfOneVSM attrOM = this.AttrOfOneVSM;
            Entities ensOfM = attrOM.EnsOfM;
            if (ensOfM.Count == 0)
                ensOfM.RetrieveAll();

            try
            {
                Entities ensOfMM = attrOM.EnsOfMM;
                QueryObject qo = new QueryObject(ensOfMM);
                qo.AddWhere(attrOM.AttrOfOneInMM, this.PK);
                qo.DoQuery();

                if (this.DDL_Group.SelectedValue == "None")
                {
                    if (this.IsLine)
                        this.UCSys1.UIEn1ToM_OneLine(ensOfM, attrOM.AttrOfMValue, attrOM.AttrOfMText, ensOfMM, attrOM.AttrOfMInMM);
                    else
                        this.UCSys1.UIEn1ToM(ensOfM, attrOM.AttrOfMValue, attrOM.AttrOfMText, ensOfMM, attrOM.AttrOfMInMM);
                }
                else
                {
                    if (this.IsLine)
                        this.UCSys1.UIEn1ToMGroupKey_Line(ensOfM, attrOM.AttrOfMValue, attrOM.AttrOfMText, ensOfMM, attrOM.AttrOfMInMM, this.DDL_Group.SelectedValue);
                    else
                        this.UCSys1.UIEn1ToMGroupKey(ensOfM, attrOM.AttrOfMValue, attrOM.AttrOfMText, ensOfMM, attrOM.AttrOfMInMM, this.DDL_Group.SelectedValue);
                }
            }
            catch (Exception ex)
            {
                ensOfM.GetNewEntity.CheckPhysicsTable();
                this.UCSys1.ClearViewState();
                ErrMyNum++;
                if (ErrMyNum > 3)
                {
                    this.UCSys1.AddMsgOfWarning("error", ex.Message);
                    return;
                }
                this.SetDataV2();
            }
        }
        public void BindTree()
        {
            if (this.DDL_Group.SelectedValue == "None")
            {
                BindTreeWithNone();
            }
            else
            {
                BindTreeWithGroup();
            }
        }
        public void BindTreeWithGroup()
        {
            AttrOfOneVSM attrOM = this.AttrOfOneVSM;
            Entities ensOfM = attrOM.EnsOfM;
            ensOfM.RetrieveAll();

            Entities ensOfMM = attrOM.EnsOfMM;
            QueryObject qo = new QueryObject(ensOfMM);
            qo.AddWhere(attrOM.AttrOfOneInMM, this.PK);
            qo.DoQuery();

            this.Tree1.Visible = true;
            this.UCSys1.Clear();
            this.UCSys1.ClearViewState();

            Entity en1 = ClassFactory.GetEn(this.Request.QueryString["EnsName"]);
            string keys = "&" + en1.PK + "=" + this.PK + "&r=" + DateTime.Now.ToString("MMddhhmmss");
            string refstrs = BP.Web.Comm.UC.UCEn.GetRefstrs1(keys, en1, en1.GetNewEntities);
            if (refstrs.Length > 0)
                this.Label1.Text = this.GenerCaption(attrOM.Desc + " =>��ع���:" + refstrs);
            else
                this.Label1.Text = this.GenerCaption(attrOM.Desc);

            #region �󶨷���
            string gropkey = this.DDL_Group.SelectedValue;
            Attr attr = attrOM.EnsOfM.GetNewEntity.EnMap.GetAttrByKey(gropkey);

            if (attr.MyFieldType == FieldType.Enum || attr.MyFieldType == FieldType.PKEnum) // ����Ƿ��� enum ���͡�
            {
                SysEnums ses = new SysEnums(gropkey);
                foreach (SysEnum se in ses)
                {
                    Node tn = this.Tree1.GetNodeByID("TN_T" + se.IntKey) as Node;
                    //   Microsoft.Web.UI.WebControls.TreeNode tn = this.Tree1.GetNodeByID("TN_T" + se.IntKey);

                    if (tn != null)
                        continue;

                    tn = new Node();
                    tn.Text = se.Lab;
                    tn.ID = "TN_T" + se.IntKey;
                    this.Tree1.Nodes.Add(tn);
                }
            }
            else
            {
                Entities ensGroup = attr.HisFKEns;
                ensGroup.RetrieveAll();

                BindIt(ensGroup, 2, false, "T");
                BindIt(ensGroup, 4, false, "T");
                BindIt(ensGroup, 6, false, "T");
                BindIt(ensGroup, 8, false, "T");
                BindIt(ensGroup, 10, false, "T");
            }
            #endregion �󶨷���


            #region �󶨷��� Ԫ��
            foreach (Entity en in ensOfM)
            {
                string val = en.GetValStringByKey(gropkey);
                Microsoft.Web.UI.WebControls.TreeNode tn = this.Tree1.GetNodeByID("TN_T" + val);
                if (tn == null)
                    continue;

                if (this.Tree1.GetNodeByID("TN_" + en.GetValStringByKey("No"), tn) != null)
                    continue;

                Node nd = new Node();
                nd.Text = en.GetValStringByKey("Name");
                nd.ID = "TN_" + en.GetValStringByKey("No");
                nd.CheckBox = true;
                tn.Nodes.Add(nd);

            }

            #endregion �󶨷��� Ԫ��


            #region ���÷��� Ԫ�� ��ѡ����Ŀ

            foreach (Entity en in ensOfMM)
            {
                string val = en.GetValStringByKey(attrOM.AttrOfMInMM);
                Microsoft.Web.UI.WebControls.TreeNode tn = this.Tree1.GetNodeByID("TN_" + val);
                if (tn == null)
                    continue;
                tn.Checked = true;
            }

            #endregion ���÷��� Ԫ�� ��ѡ����Ŀ

        }
        public void BindTreeWithNone()
        {
            AttrOfOneVSM attrOM = this.AttrOfOneVSM;
            Entities ensOfM = attrOM.EnsOfM;
            ensOfM.RetrieveAll();

            Entities ensOfMM = attrOM.EnsOfMM;
            QueryObject qo = new QueryObject(ensOfMM);
            qo.AddWhere(attrOM.AttrOfOneInMM, this.PK);
            qo.DoQuery();

            this.Tree1.Visible = true;
            this.UCSys1.Clear();
            this.UCSys1.ClearViewState();

            Entity en1 = ClassFactory.GetEn(this.Request.QueryString["EnsName"]);
            string keys = "&" + en1.PK + "=" + this.PK + "&r=" + DateTime.Now.ToString("MMddhhmmss");
            string refstrs = BP.Web.Comm.UC.UCEn.GetRefstrs1(keys, en1, en1.GetNewEntities);
            if (refstrs.Length > 0)
                this.Label1.Text = this.GenerCaption(attrOM.Desc + " =>��ع���:" + refstrs);
            else
                this.Label1.Text = this.GenerCaption(attrOM.Desc);


            BindIt(ensOfM, 2, true, "");
            BindIt(ensOfM, 4, true, "");
            BindIt(ensOfM, 6, true, "");
            BindIt(ensOfM, 8, true, "");
            BindIt(ensOfM, 10, true, "");

            foreach (Entity en in ensOfMM)
            {
                string val = en.GetValStringByKey(attrOM.AttrOfMInMM);
                Microsoft.Web.UI.WebControls.TreeNode tn = this.Tree1.GetNodeByID("TN_" + val);
                if (tn == null)
                    continue;
                tn.Checked = true;
            }
        }
        public void BindIt(Entities ens, int len, bool isCheckBok, string idPro)
        {
            foreach (Entity en in ens)
            {
                string no = en.GetValStringByKey("No");
                if (no.Length != len)
                    continue;

                Microsoft.Web.UI.WebControls.TreeNode myyy = this.Tree1.GetNodeByID("TN_" + idPro + no);
                if (myyy != null)
                    continue;

                Node tn = new Node();
                tn.Text = en.GetValStringByKey("Name");
                tn.ID = "TN_" + idPro + en.GetValStringByKey("No");
                tn.CheckBox = isCheckBok;
                if (len == 2)
                {
                    this.Tree1.Nodes.Add(tn);
                    continue;
                }

                no = no.Substring(0, no.Length - 2);
                Microsoft.Web.UI.WebControls.TreeNode mtn = this.Tree1.GetNodeByID("TN_" + idPro + no);
                if (mtn == null)
                {
                    this.Tree1.Nodes.Add(tn);
                }
                else
                {
                    mtn.Nodes.Add(tn);
                }
            }
        }
        private void BPToolBar1_ButtonClick(object sender, System.EventArgs e)
        {
            //ToolbarCB cb = sender as ToolbarCB;
            //if (cb != null)
            //{
            //    if (cb.Selected)
            //        this.BindTree();
            //    else
            //        SetDataV2();
            //    return;
            //}

            Btn btn = (Btn)sender;
            switch (btn.ID)
            {
                case NamesOfBtn.SelectNone:
                    //this.CBL1.SelectNone();
                    break;
                case NamesOfBtn.SelectAll:
                    //this.CBL1.SelectAll();
                    break;
                case NamesOfBtn.Save:
                    if (this.IsTreeShowWay)
                    {
                        SaveTree();
                    }
                    else
                    {
                        Save();
                        this.SetDataV2();
                    }

                    string str = this.Request.RawUrl;
                    if (str.Contains("ShowWay="))
                    {
                        str.Replace("&ShowWay=", "&1=");
                    }

                    // this.Response.Redirect(str + "&ShowWay=" + this.DDL_Group.SelectedValue, true);
                    break;
                case "Btn_SaveAndClose":
                    if (this.IsTreeShowWay)
                    {
                        SaveTree();
                    }
                    else
                    {
                        Save();
                    }
                    this.WinClose();
                    break;
                case "Btn_Close":
                    this.WinClose();
                    break;
                case "Btn_EditMEns":
                    this.EditMEns();
                    break;
                default:
                    throw new Exception("@û���ҵ�" + btn.ID);
            }
        }

        #region ����
        public void EditMEns()
        {
            this.WinOpen( "/Comm/UIEns.aspx?EnsName=" + this.AttrOfOneVSM.EnsOfM.ToString());
        }
        public void Save()
        {
            AttrOfOneVSM attr = this.AttrOfOneVSM;
            Entities ensOfMM = attr.EnsOfMM;
            ensOfMM.Delete(attr.AttrOfOneInMM, this.PK);

            string msg = "";

            AttrOfOneVSM attrOM = this.AttrOfOneVSM;
            Entities ensOfM = attrOM.EnsOfM;
            ensOfM.RetrieveAll();
            foreach (Entity en in ensOfM)
            {
                string pk = en.GetValStringByKey(attr.AttrOfMValue);
                CheckBox cb = (CheckBox)this.UCSys1.FindControl("CB_" + pk);
                if (cb == null)
                    continue;

                if (cb.Checked == false)
                    continue;

                Entity en1 = ensOfMM.GetNewEntity;
                en1.SetValByKey(attr.AttrOfOneInMM, this.PK);
                en1.SetValByKey(attr.AttrOfMInMM, pk);
                try
                {
                    en1.Insert();
                }
                catch (Exception ex)
                {
                    msg += "ִ�в������" + en1.EnDesc + " " + ex.Message;
                }
            }

            Entity enP = ClassFactory.GetEn(this.Request.QueryString["EnsName"]);
            if (enP.EnMap.EnType != EnType.View)
            {
                enP.SetValByKey(enP.PK, this.PK);// =this.PK;
                enP.Retrieve(); //��ѯ��
                try
                {
                    enP.Update(); // ִ�и��£�����д�� ��ʵ�� ��ҵ���߼���
                }
                catch (Exception ex)
                {
                    msg += "ִ�и��´���" + enP.EnDesc + " " + ex.Message;
                }
            }
            if (msg != "")
                this.ResponseWriteBlueMsg(msg);
        }
        public void SaveTree()
        {
            AttrOfOneVSM attr = this.AttrOfOneVSM;
            Entities ensOfMM = attr.EnsOfMM;
            ensOfMM.Delete(attr.AttrOfOneInMM, this.PK); //ɾ���Ѿ���������ݡ�

            AttrOfOneVSM attrOM = this.AttrOfOneVSM;
            Entities ensOfM = attrOM.EnsOfM;
            ensOfM.RetrieveAll();

            foreach (Entity en in ensOfM)
            {
                string pk = en.GetValStringByKey(attr.AttrOfMValue);
                Microsoft.Web.UI.WebControls.TreeNode cb = this.Tree1.GetNodeByID("TN_" + pk);
                if (cb == null)
                    continue;

                if (cb.Checked == false)
                    continue;

                Entity en1 = ensOfMM.GetNewEntity;
                en1.SetValByKey(attr.AttrOfOneInMM, this.PK);
                en1.SetValByKey(attr.AttrOfMInMM, pk);
                en1.Insert();
            }

            Entity enP = ClassFactory.GetEn(this.Request.QueryString["EnsName"]);
            if (enP.EnMap.EnType != EnType.View)
            {
                enP.SetValByKey(enP.PK, this.PK);// =this.PK;
                enP.Retrieve(); //��ѯ��
                enP.Update(); // ִ�и��£�����д�� ��ʵ�� ��ҵ���߼���
            }
            //  this.Response.Redirect(this.Request.RawUrl + "&IsCheckTree=1", true);
        }
        #endregion

        #region Web ������������ɵĴ���
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: �õ����� ASP.NET Web ���������������ġ�
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// �����֧������ķ��� - ��Ҫʹ�ô���༭���޸�
        /// �˷��������ݡ�
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion

        private void DDL_Group_SelectedIndexChanged(object sender, EventArgs e)
        {
            // this.SetDataV2();
            CheckBox mycb = this.ToolBar1.GetCBByID("RB_Tree");
            if (mycb == null)
                this.SetDataV2();
            else
                this.BindTree();
        }
    }
}
