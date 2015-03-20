using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.En;
using System.Text;
using System.Data;
using BP.WF;
using BP.DA;
using BP.Sys;
using BP.Web;

namespace CCFlow.WF.Comm
{
    public partial class HelperOfTBEUI : System.Web.UI.Page
    {
        #region
        public string getUTF8ToString(string param)
        {
            return HttpUtility.UrlDecode(Request[param], System.Text.Encoding.UTF8);
        }
        public int WordsSort
        {
            get
            {
                try
                {
                    return int.Parse(this.getUTF8ToString("WordsSort"));
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 节点编号
        /// </summary>
        public string NodeName
        {
            get
            {
                return this.Request.QueryString["NodeName"];
            }
        }
        /// <summary>
        /// 节点字段
        /// </summary>
        public string NodeAttrKey
        {
            get
            {
                if (this.Request.QueryString["attrKey"] == "undefined")
                    return "";
                return this.Request.QueryString["attrKey"];
            }
        }
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            string method = string.Empty;
            //返回值
            string s_responsetext = string.Empty;
            if (string.IsNullOrEmpty(Request["method"]))
                return;

            method = Request["method"].ToString();
            switch (method)
            {
                case "GetTreeData":
                    s_responsetext = GetTreeData();
                    break;
                case "addNodeData":
                    s_responsetext = addNodeData();
                    break;
                case "editNodeMet":
                    s_responsetext = editNodeMet();
                    break;
                case "delNodeMet":
                    s_responsetext = delNodeMet();
                    break;
                case "moreTextMet":
                    s_responsetext = moreTextMet();
                    break;
            }
            if (string.IsNullOrEmpty(s_responsetext))
                s_responsetext = "";
            //组装ajax字符串格式,返回调用客户端
            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.ContentType = "text/html";
            Response.Expires = 0;
            Response.Write(s_responsetext);
            Response.End();
        }
        private string GetTreeData()
        {
            DefVal dv = new DefVal();
            dv.CheckPhysicsTable();

            SonMethod(WebUser.No, 1, "常用词汇", 0, "0");//检查数据库

            //读取数据
            string OneMyDataSql = string.Format("select  No,CurValue,ParentNo,IsParent from Sys_DefVal where WordsSort={0}" +
            "AND NodeName='{1}' and fk_emp='{2}'  and NodeAttrKey='{3}'", WordsSort, NodeName, WebUser.No, NodeAttrKey);

            string TwoMyDataSql = string.Format("select  No,CurValue,ParentNo,IsParent from Sys_DefVal where WordsSort={0}" +
            "AND NodeName='{1}' and fk_emp=''  and NodeAttrKey='{2}'", WordsSort, NodeName, NodeAttrKey);
            DataTable OneDataDt = DBAccess.RunSQLReturnTable(OneMyDataSql);
            DataTable TwoDataDt = DBAccess.RunSQLReturnTable(TwoMyDataSql);

            DataSet ds = new DataSet();
            ds.Tables.Add(OneDataDt.Copy());
            ds.Merge(TwoDataDt.Copy());

            string s_responsetext = string.Empty;
            string s_checkded = string.Empty;

            s_responsetext = GetTreeJsonByTable(ds.Tables[0], "No", "CurValue", "ParentNo", "0", "IsParent", s_checkded);

            return s_responsetext;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Sort">类别</param>
        /// <param name="UserNo">人员编号</param>
        /// <param name="IsParent">是否父节点1/0   Y/N</param>
        /// <param name="CurValue">节点文本</param>
        /// <param name="IsHisWords">是否历史词汇1/0   Y/N</param>
        /// <param name="ParentNo">父节点编号</param>
        private void SonMethod(string UserNo, int IsParent, string CurValue, int IsHisWords, string ParentNo)
        {
            string CkeckSortSql = string.Format("SELECT No FROM Sys_DefVal where ParentNo='{7}' AND WordsSort={0} AND " +
                                        "NodeName='{1}'  AND NodeAttrKey='{3}' AND IsParent={4} AND " +
                                        "CurValue='{5}' AND IsHisWords={6} AND FK_Emp='{2}'", WordsSort, NodeName, UserNo, NodeAttrKey, IsParent,
                                        CurValue, IsHisWords, ParentNo);

            if (DBAccess.RunSQLReturnCOUNT(CkeckSortSql) == 0)//不存在根节点 ---当前流程  节点  类别 节点字段 我的
            {
                for (int i = 1; i < 4; i++)
                {
                    DefVal MyDv = new DefVal();
                    MyDv.ParentNo = "0";
                    MyDv.IsParent = "1";
                    MyDv.IsHisWords = "0";
                    MyDv.WordsSort = WordsSort.ToString();
                    MyDv.NodeName = NodeName;
                    MyDv.NodeAttrKey = NodeAttrKey;
                    if (i == 1)
                    {
                        MyDv.FK_Emp = WebUser.No;
                        MyDv.CurValue = "常用词汇";
                        MyDv.Insert();
                        MyDv.Retrieve();
                        InsertSortSonDMethod("常用词汇", MyDv.No);
                    }
                    else
                        if (i == 2)
                        {
                            MyDv.FK_Emp = WebUser.No;
                            MyDv.CurValue = "我的词汇";
                            MyDv.Insert();
                            MyDv.Retrieve();
                            InsertSortSonDMethod("我的词汇", MyDv.No);
                        }
                        else if (i == 3)
                        {
                            string justOneSql = string.Format("SELECT No FROM Sys_DefVal where ParentNo='0' AND WordsSort={0} AND " +
                                                              "NodeName='{1}' AND FK_Emp='' AND CurValue = '全局词汇'  " +
                                                              "AND NodeAttrKey='{2}' AND IsHisWords = 0 AND IsParent = 1", WordsSort, NodeName, NodeAttrKey);
                            if (DBAccess.RunSQLReturnCOUNT(justOneSql) == 0)//全局词汇只有一个
                            {
                                MyDv.FK_Emp = "";
                                MyDv.CurValue = "全局词汇";
                                MyDv.Insert();
                                MyDv.Retrieve();
                                InsertSortSonDMethod("全局词汇", MyDv.No);
                            }
                        }
                }
            }
            else//已经存在数据  执行常用词汇的更新逻辑
            {
                updataMethod();
            }
        }
        public string moreTextMet()
        {
            string id = this.getUTF8ToString("id");
            string TextSql = string.Format("select CurValue from Sys_DefVal where No='{0}'", id);
            return DBAccess.RunSQLReturnTable(TextSql).Rows[0][0].ToString();
        }
        private void updataMethod()
        {
            string AType = "";
            string StrFlow;
            try
            {
                StrFlow = "ND" + Convert.ToInt16(NodeName) + "Track";
            }
            catch (Exception)
            {
                StrFlow = NodeName;
            }

            switch (WordsSort)
            {
                case 0://审核组件
                    AType = "22";
                    break;
                case 1://退回
                    AType = "2";
                    break;
                case 2://移交
                    AType = "3";
                    break;
                default://多行文本框
                    break;
            }
            string checkSql = "";
            string delSql = "";
            string Sql = "";
            checkSql = string.Format("select No from Sys_DefVal where WordsSort={0} and CurValue='常用词汇' and fk_emp='{1}' and NodeName='{2}' " +
                                     "and NodeAttrKey='{3}'", WordsSort, WebUser.No, NodeName, NodeAttrKey);
            delSql = string.Format("delete  from Sys_DefVal where ParentNo={0}", DBAccess.RunSQLReturnTable(checkSql).Rows[0][0].ToString());
            DBAccess.RunSQL(delSql);

            DataTable Dt;
            DBType AppCenterDBType = DBAccess.AppCenterDBType;
            if (WordsSort == 3)
            {
                try//节点表单  还是表单库里的
                {
                    switch (AppCenterDBType)
                    {
                        case DBType.MSSQL:
                            Sql = string.Format("SELECT top (10) {0} FROM {1} where Emps='{2}' order by RDT desc", NodeAttrKey, StrFlow, WebUser.No);
                            break;
                        case DBType.Oracle:
                            Sql = string.Format("select {0} from {1} WHERE ROWNUM<=10 and Emps='{2}' order by RDT desc ", NodeAttrKey, StrFlow, WebUser.No);
                            break;
                        case DBType.MySQL:
                            Sql = string.Format("SELECT {0} FROM {1} where Emps='{2}' order by RDT desc limit 10 ", NodeAttrKey, StrFlow, WebUser.No);
                            break;
                        case DBType.Informix:
                        case DBType.Access:
                            Sql = string.Format("SELECT top (10) {0} FROM {1} where Emps='{2}' order by RDT desc", NodeAttrKey, StrFlow, WebUser.No);
                            break;
                        default:
                            throw new Exception("发现未知的数据库连接类型！");
                    }
                    Dt = DBAccess.RunSQLReturnTable(Sql);
                }
                catch (Exception)
                {
                    switch (AppCenterDBType)
                    {
                        case DBType.MSSQL:
                            Sql = string.Format("SELECT top (10) {0} FROM {1} order by OID desc", NodeAttrKey, StrFlow);
                            break;
                        case DBType.Oracle:
                            Sql = string.Format("select {0} from {1} WHERE ROWNUM<=10  order by OID desc ", NodeAttrKey, StrFlow);
                            break;
                        case DBType.MySQL:
                            Sql = string.Format("SELECT {0} FROM {1}  order by OID desc limit 10 ", NodeAttrKey, StrFlow);
                            break;
                        case DBType.Informix:
                        case DBType.Access:
                            Sql = string.Format("SELECT top (10) {0} FROM {1}  order by OID desc", NodeAttrKey, StrFlow);
                            break;
                        default:
                            throw new Exception("发现未知的数据库连接类型！");
                    }
                    //Sql = string.Format("SELECT top (10) {0} FROM {1} ", NodeAttrKey, StrFlow);
                    Dt = DBAccess.RunSQLReturnTable(Sql);
                }

            }
            else
            {
                switch (AppCenterDBType)
                {
                    case DBType.MSSQL:
                        Sql = string.Format("SELECT top (10) Msg FROM {0} where EmpFrom='{1}' AND ActionType={2} order by RDT desc", StrFlow, WebUser.No, AType);
                        break;
                    case DBType.Oracle:
                        Sql = string.Format("select Msg from {0} WHERE ROWNUM<=10 and EmpFrom='{1}' AND ActionType={2} order by RDT desc ", StrFlow, WebUser.No, AType);
                        break;
                    case DBType.MySQL:
                        Sql = string.Format("SELECT Msg FROM {0} where EmpFrom='{1}' AND ActionType={2} order by RDT desc limit 10 ", StrFlow, WebUser.No, AType);
                        break;
                    case DBType.Informix:
                    case DBType.Access:
                        Sql = string.Format("SELECT top (10) Msg FROM {0} where EmpFrom='{1}' AND ActionType={2} order by RDT desc", StrFlow, WebUser.No, AType);
                        break;
                    default:
                        throw new Exception("发现未知的数据库连接类型！");
                }
                //Sql = string.Format("select top (10) Msg  from {0} where EmpFrom='{1}' AND ActionType={2} order by RDT desc", StrFlow, WebUser.No, AType);
                Dt = DBAccess.RunSQLReturnTable(Sql);
            }


            foreach (DataRow r in Dt.Rows)
            {
                DefVal Dv = new DefVal();
                Dv.IsHisWords = "1";
                Dv.ParentNo = DBAccess.RunSQLReturnTable(checkSql).Rows[0][0].ToString();
                Dv.IsParent = "0";
                Dv.WordsSort = WordsSort.ToString();
                if (WordsSort == 3)
                {
                    Dv.CurValue = r[NodeAttrKey].ToString();
                    if (r[NodeAttrKey].ToString() == "")
                    {
                        continue;
                    }
                }
                else
                {
                    Dv.CurValue = r["Msg"].ToString();
                    if (r["Msg"].ToString() == "")
                    {
                        continue;
                    }
                }
                Dv.FK_Emp = WebUser.No;
                Dv.NodeName = NodeName;
                Dv.NodeAttrKey = NodeAttrKey;
                Dv.Insert();
            }
        }
        /// <summary>
        /// 初始化插入------------OK
        /// </summary>
        /// <param name="n"></param>
        /// <param name="s"></param>
        private void InsertSortSonDMethod(string n, string ParentNo)
        {
            DefVal SonMyDv = new DefVal();
            if (n == "常用词汇")
            {
                SonMyDv.IsHisWords = "1";
                SonMyDv.ParentNo = ParentNo;
                SonMyDv.IsParent = "0";
                SonMyDv.WordsSort = WordsSort.ToString();
                SonMyDv.CurValue = "初始化数据";
                SonMyDv.FK_Emp = WebUser.No;
                SonMyDv.NodeName = NodeName;
                SonMyDv.NodeAttrKey = NodeAttrKey;
                SonMyDv.Insert();

                updataMethod();
            }
            else
            {
                if (n == "我的词汇")
                {
                    SonMyDv.FK_Emp = WebUser.No;
                }
                else
                {
                    SonMyDv.FK_Emp = "";
                }
                SonMyDv.ParentNo = ParentNo;
                SonMyDv.IsParent = "0";
                SonMyDv.WordsSort = WordsSort.ToString();
                SonMyDv.CurValue = "初始化数据";
                SonMyDv.NodeName = NodeName;
                SonMyDv.IsHisWords = "0";
                SonMyDv.NodeName = NodeName;
                SonMyDv.NodeAttrKey = NodeAttrKey;
                SonMyDv.Insert();
            }
        }
        /// <summary>
        /// 添加数据---OK
        /// </summary>
        /// <returns></returns>
        private string addNodeData()//哪一类别 什么数据
        {
            string selectId = getUTF8ToString("selectId");
            string setText = getUTF8ToString("setText");

            string pp = string.Format("select * from Sys_DefVal where  No ='{0}'", selectId);
            DataTable dt = DBAccess.RunSQLReturnTable(pp);
            DefVal Dv = new DefVal();

            if (dt.Rows[0]["CurValue"].ToString() == "全局词汇")
            {
                Dv.FK_Emp = "";
            }
            else
            {
                Dv.FK_Emp = WebUser.No;
            }
            Dv.ParentNo = dt.Rows[0]["No"].ToString();
            Dv.IsParent = "0";
            Dv.IsHisWords = "0";
            Dv.WordsSort = WordsSort.ToString();
            Dv.NodeName = NodeName;
            Dv.CurValue = setText;
            Dv.NodeAttrKey = NodeAttrKey;
            Dv.Insert();
            return GetTreeData();
        }
        /// <summary>
        /// 编辑节点---OK
        /// </summary>
        /// <returns></returns>
        private string editNodeMet()
        {
            string selectId = getUTF8ToString("selectId");
            string setText = getUTF8ToString("setText");

            DefVal Dv = new DefVal();
            string pp = string.Format("select * from Sys_DefVal where  No  ='{0}')", selectId);
            Dv.Retrieve(DefValAttr.No, selectId);
            Dv.CurValue = setText;
            Dv.Update();
            return GetTreeData();
        }
        /// <summary>
        /// 删除节点----OK
        /// </summary>
        /// <returns></returns>
        private string delNodeMet()
        {
            string selectId = getUTF8ToString("selectId");
            string sql = string.Format("select No,IsParent,IsHisWords from Sys_DefVal where No in  ({0})", selectId);
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            foreach (DataRow row in dt.Rows)
            {
                if (row["IsParent"].ToString() == "1" || row["IsHisWords"].ToString() == "1")//不可以删除
                {
                    continue;
                }
                DefVal Dv = new DefVal();
                Dv.Delete(DefValAttr.No, row["No"].ToString());
            }
            return GetTreeData();
        }
        /// <summary>
        /// 根据DataTable生成Json树结构
        /// </summary>
        StringBuilder treeResult = new StringBuilder();
        StringBuilder treesb = new StringBuilder();
        public string GetTreeJsonByTable(DataTable tabel, string idCol, string txtCol, string rela, object pId, string IsParent, string CheckedString)
        {
            string treeJson = string.Empty;
            treeResult.Append(treesb.ToString());

            treesb.Clear();
            if (tabel.Rows.Count > 0)
            {
                treesb.Append("[");
                string filer = string.Empty;
                if (pId.ToString() == "")
                {
                    filer = string.Format("{0} is null", rela);
                }
                else
                {
                    filer = string.Format("{0}='{1}'", rela, pId);
                }
                DataRow[] rows = tabel.Select(filer);
                if (rows.Length > 0)
                {
                    foreach (DataRow row in rows)
                    {
                        string deptNo = row[idCol].ToString();
                        string backText = row[txtCol].ToString();
                        if (backText.Length > 25)
                        {
                            backText = backText.Substring(0, 25) + "<img src='../Scripts/easyUI/themes/icons/add2.png' onclick='moreText(" + row[idCol].ToString() + ")'/>";
                        }
                        if (treeResult.Length == 0)
                        {

                            treesb.Append("{\"id\":\"" + row[idCol]
                                + "\",\"text\":\"" + backText
                                //+ "\",\"IsParent\":\"" + row[IsParent]
                                  + "\",\"attributes\":{\"IsParent\":\"" + row[IsParent] + "\"}"
                                   + ",\"checked\":" + CheckedString.Contains("," + row[idCol] + ",").ToString().ToLower() + ",\"state\":\"open\"");
                            //+ "\",\"checked\":" + CheckedString.Contains("," + row[idCol] + ",").ToString().ToLower() + ",\"state\":\"open\"");
                        }
                        else if (tabel.Select(string.Format("{0}='{1}'", rela, row[idCol])).Length > 0)
                        {
                            treesb.Append("{\"id\":\"" + row[idCol]
                                + "\",\"text\":\"" + backText
                                //+ "\",\"IsParent\":\"" + row[IsParent]
                                   + "\",\"attributes\":{\"IsParent\":\"" + row[IsParent] + "\"}"
                                   + ",\"checked\":" + CheckedString.Contains("," + row[idCol] + ",").ToString().ToLower() + ",\"state\":\"open\"");
                            //+ "\",\"checked\":" + CheckedString.Contains("," + row[idCol] + ",").ToString().ToLower() + ",\"state\":\"open\"");
                        }
                        else
                        {
                            treesb.Append("{\"id\":\"" + row[idCol]
                                + "\",\"text\":\"" + backText
                                //+ "\",\"IsParent\":\"" +row[IsParent]
                                 + "\",\"attributes\":{\"IsParent\":\"" + row[IsParent] + "\"}"
                                     + ",\"checked\":" + CheckedString.Contains("," + row[idCol] + ",").ToString().ToLower());
                            //+ "\",\"checked\":" + CheckedString.Contains("," + row[idCol] + ",").ToString().ToLower());
                        }


                        if (tabel.Select(string.Format("{0}='{1}'", rela, row[idCol])).Length > 0)
                        {
                            treesb.Append(",\"children\":");
                            GetTreeJsonByTable(tabel, idCol, txtCol, rela, row[idCol], IsParent, CheckedString);
                            treeResult.Append(treesb.ToString());
                            treesb.Clear();
                        }
                        treeResult.Append(treesb.ToString());
                        treesb.Clear();
                        treesb.Append("},");
                    }
                    treesb = treesb.Remove(treesb.Length - 1, 1);
                }
                treesb.Append("]");
                treeResult.Append(treesb.ToString());
                treeJson = treeResult.ToString();
                treesb.Clear();
            }
            return treeJson;
        }
    }
}