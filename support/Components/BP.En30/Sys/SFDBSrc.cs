using System;
using System.Data;
using System.Collections;
using System.Text;
using BP.DA;
using BP.En;
namespace BP.Sys
{
    /// <summary>
    /// ����Դ����
    /// </summary>
    public enum DBSrcType
    {
        /// <summary>
        /// �������ݿ�
        /// </summary>
        Localhost,
        /// <summary>
        /// SQL
        /// </summary>
        SQLServer,
        /// <summary>
        /// Oracle
        /// </summary>
        Oracle,
        /// <summary>
        /// MySQL
        /// </summary>
        MySQL,
        /// <summary>
        /// Infomax
        /// </summary>
        Infomax
    }
    /// <summary>
    /// ����Դ
    /// </summary>
    public class SFDBSrcAttr : EntityNoNameAttr
    {
        /// <summary>
        /// ����Դ����
        /// </summary>
        public const string DBSrcType = "DBSrcType";
        /// <summary>
        /// �û����
        /// </summary>
        public const string UserID = "UserID";
        /// <summary>
        /// ����
        /// </summary>
        public const string Password = "Password";
        /// <summary>
        /// IP��ַ
        /// </summary>
        public const string IP = "IP";
        /// <summary>
        /// ���ݿ�����
        /// </summary>
        public const string DBName = "DBName";
    }
    /// <summary>
    /// ����Դ
    /// </summary>
    public class SFDBSrc : EntityNoName
    {
        #region ����
        /// <summary>
        /// �Ƿ�������ʵ��?
        /// </summary>
        public string UserID
        {
            get
            {
                return this.GetValStringByKey(SFDBSrcAttr.UserID);
            }
            set
            {
                this.SetValByKey(SFDBSrcAttr.UserID, value);
            }
        }
        /// <summary>
        /// ����
        /// </summary>
        public string Password
        {
            get
            {
                return this.GetValStringByKey(SFDBSrcAttr.Password);
            }
            set
            {
                this.SetValByKey(SFDBSrcAttr.Password, value);
            }
        }
        /// <summary>
        /// ���ݿ�����
        /// </summary>
        public string DBName
        {
            get
            {
                return this.GetValStringByKey(SFDBSrcAttr.DBName);
            }
            set
            {
                this.SetValByKey(SFDBSrcAttr.DBName, value);
            }
        }
        /// <summary>
        /// ���ݿ�����
        /// </summary>
        public DBSrcType DBSrcType
        {
            get
            {
                return (DBSrcType)this.GetValIntByKey(SFDBSrcAttr.DBSrcType);
            }
            set
            {
                this.SetValByKey(SFDBSrcAttr.DBSrcType, (int)value);
            }
        }
        public string IP
        {
            get
            {
                return this.GetValStringByKey(SFDBSrcAttr.IP);
            }
            set
            {
                this.SetValByKey(SFDBSrcAttr.IP, value);
            }
        }
        #endregion

        #region ���췽��
        /// <summary>
        /// ����Դ
        /// </summary>
        public SFDBSrc()
        {
        }
        public SFDBSrc(string mypk)
        {
            this.No = mypk;
            this.Retrieve();
        }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Sys_SFDBSrc");
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "����Դ";
                map.EnType = EnType.Sys;

                map.AddTBStringPK(SFDBSrcAttr.No, null, "����Դ���(������Ӣ��)", true, false, 1, 20, 20);
                map.AddTBString(SFDBSrcAttr.Name, null, "����Դ����", true, false, 0, 30, 20);

                map.AddTBString(SFDBSrcAttr.UserID, null, "���ݿ��¼�û�ID", true, false, 0, 30, 20);
                map.AddTBString(SFDBSrcAttr.Password, null, "���ݿ��¼�û�����", true, false, 0, 30, 20);
                map.AddTBString(SFDBSrcAttr.IP, null, "IP��ַ", true, false, 0, 50, 20);
                map.AddTBString(SFDBSrcAttr.DBName, null, "���ݿ�����", true, false, 0, 30, 20);

                map.AddDDLSysEnum(SFDBSrcAttr.DBSrcType, 0, "����Դ����", true, true,
                    SFDBSrcAttr.DBSrcType, "@0=Ӧ��ϵͳ�����ݿ�@1=SQLServer@2=Oracle@3=MySQL@4=Infomix");

                RefMethod rm = new RefMethod();
                rm = new RefMethod();
                rm.Title = "��������";
                rm.ClassMethodName = this.ToString() + ".DoConn";
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
        /// <summary>
        /// ִ������
        /// </summary>
        /// <returns></returns>
        public string DoConn()
        {
            if (this.No == "local")
                return "�������Ӳ���Ҫ�����Ƿ����ӳɹ�.";

            if (this.DBSrcType == Sys.DBSrcType.Localhost)
                throw new Exception("@�ڸ�ϵͳ��ֻ����һ����������.");

            string dsn = "";
            if (this.DBSrcType == Sys.DBSrcType.SQLServer)
            {
                try
                {
                    dsn = "Password=" + this.Password + ";Persist Security Info=True;User ID=" + this.UserID + ";Initial Catalog=" + this.DBName + ";Data Source=" + this.IP + ";Timeout=999;MultipleActiveResultSets=true";
                    System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection();
                    conn.ConnectionString = dsn;
                    conn.Open();
                    conn.Close();

                    //ɾ��Ӧ��.
                    try
                    {
                        BP.DA.DBAccess.RunSQL("Exec sp_droplinkedsrvlogin " + this.No + ",Null ");
                        BP.DA.DBAccess.RunSQL("Exec sp_dropserver " + this.No);
                    }
                    catch
                    {
                    }

                    //����Ӧ��.
                    string sql = "";
                    sql += "sp_addlinkedserver @server='" + this.No + "', @srvproduct='', @provider='SQLOLEDB', @datasrc='" + this.IP + "'";
                    BP.DA.DBAccess.RunSQL(sql);

                    //ִ�е�¼.
                    sql = "";
                    sql += " EXEC sp_addlinkedsrvlogin '" + this.No + "','false', NULL, '" + this.UserID + "', '" + this.Password + "'";
                    BP.DA.DBAccess.RunSQL(sql);

                    return "��ϲ������(" + this.Name + ")�������óɹ���";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }

            if (this.DBSrcType == Sys.DBSrcType.Oracle)
            {
                try
                {
                    dsn = "user id=" + this.UserID + ";data source=" + this.DBName + ";password=" + this.Password + ";Max Pool Size=200";
                    System.Data.OracleClient.OracleConnection conn = new System.Data.OracleClient.OracleConnection();
                    conn.ConnectionString = dsn;
                    conn.Open();
                    conn.Close();
                    return "��ϲ������(" + this.Name + ")�������óɹ���";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }

            if (this.DBSrcType == Sys.DBSrcType.MySQL)
            {
                try
                {
                    dsn = "Data Source=" + this.IP + ";Persist Security info=True;Initial Catalog=" + this.DBName + ";User ID=" + this.UserID + ";Password=" + this.Password + ";";
                    MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection();
                    conn.ConnectionString = dsn;
                    conn.Open();
                    conn.Close();
                    return "��ϲ������(" + this.Name + ")�������óɹ���";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }

            //  if (this.DBSrcType == Sys.DBSrcType.Infomax)
            //{
            //    try
            //    {
            ////Host=10.0.2.36;Service=8001;Server=niosserver; Database=ccflow; UId=npmuser; Password=npmoptr2012;Database locale=en_US.819;Client Locale=en_US.CP1252
            //        dsn = "Data Source="+this.IP+";Persist Security info=True;Initial Catalog="+this.DBName+";User ID="+this.UserID+";Password="+this.Password+";";
            //        MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection();
            //        conn.ConnectionString = dsn;
            //        conn.Open();
            //        conn.Close();
            //        return "��ϲ������(" + this.Name + ")�������óɹ���";
            //    }
            //    catch (Exception ex)
            //    {
            //        return ex.Message;
            //    }
            //}
            return "û���漰�������Ӳ�������...";
        }
        /// <summary>
        /// ��������б�.
        /// </summary>
        /// <returns></returns>
        public DataTable GetTables()
        {
            var sql = new StringBuilder();
            sql.AppendFormat("SELECT ss.SrcTable FROM Sys_SFTable ss WHERE ss.FK_SFDBSrc = '{0}'", this.No);

            var allTablesExist = DBAccess.RunSQLReturnTable(sql.ToString());

            sql.Clear();
            sql.AppendLine("SELECT NAME AS No,");
            sql.AppendLine("       [Name] = '[' + (CASE xtype WHEN 'U' THEN '��' ELSE '��ͼ' END) + '] ' + ");
            sql.AppendLine("       NAME,");
            sql.AppendLine("       xtype");
            sql.AppendLine("FROM   sysobjects");
            sql.AppendLine("WHERE  (xtype = 'U' OR xtype = 'V')");
            sql.AppendLine("       AND (NAME NOT LIKE 'ND%')");
            sql.AppendLine("       AND (NAME NOT LIKE 'Demo_%')");
            sql.AppendLine("       AND (NAME NOT LIKE 'Sys_%')");
            sql.AppendLine("       AND (NAME NOT LIKE 'WF_%')");
            sql.AppendLine("       AND (NAME NOT LIKE 'GPM_%')");
            sql.AppendLine("ORDER BY");
            sql.AppendLine("       xtype,");
            sql.AppendLine("       NAME");

            DataTable allTables = null;

            if (this.No == "local")
            {
                allTables = DBAccess.RunSQLReturnTable(sql.ToString());
            }
            else
            {
                string dsn = "";
                try
                {
                    dsn = "Password=" + this.Password + ";Persist Security Info=True;User ID=" + this.UserID +
                          ";Initial Catalog=" + this.DBName + ";Data Source=" + this.IP +
                          ";Timeout=999;MultipleActiveResultSets=true";
                    System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection();
                    conn.ConnectionString = dsn;
                    conn.Open();

                    allTables = BP.DA.DBAccess.RunSQLReturnTable(sql.ToString(), conn, dsn, CommandType.Text, null);
                }
                catch (Exception ex)
                {
                    throw new Exception("@ʧ��:" + ex.Message + " dns:" + dsn);
                }
            }

            //ȥ���Ѿ�ʹ�õı�
            var filter = string.Empty;

            foreach (DataRow dr in allTablesExist.Rows)
                filter += string.Format("No='{0}' OR ", dr[0]);

            var deletedRows = allTables.Select(filter.TrimEnd(" OR ".ToCharArray()));

            foreach (DataRow dr in deletedRows)
            {
                allTables.Rows.Remove(dr);
            }

            return allTables;
        }

        /// <summary>
        /// ��ȡ����ֶ���Ϣ
        /// </summary>
        /// <param name="tableName">��/��ͼ����</param>
        /// <returns></returns>
        public DataTable GetColumns(string tableName)
        {
            //SqlServer���ݿ�
            var sql = new StringBuilder();
            sql.AppendLine("SELECT sc.name,");
            sql.AppendLine("       st.name AS [type],");
            sql.AppendLine("       (");
            sql.AppendLine("           CASE ");
            sql.AppendLine("                WHEN st.name = 'nchar' OR st.name = 'nvarchar' THEN sc.length / 2");
            sql.AppendLine("                ELSE sc.length");
            sql.AppendLine("           END");
            sql.AppendLine("       ) AS length,");
            sql.AppendLine("       sc.colid,");
            sql.AppendLine("       ISNULL(ep.[value], '') AS [Desc]");
            sql.AppendLine("FROM   dbo.syscolumns sc");
            sql.AppendLine("       INNER JOIN dbo.systypes st");
            sql.AppendLine("            ON  sc.xtype = st.xusertype");
            sql.AppendLine("       LEFT OUTER JOIN sys.extended_properties ep");
            sql.AppendLine("            ON  sc.id = ep.major_id");
            sql.AppendLine("            AND sc.colid = ep.minor_id");
            sql.AppendLine("            AND ep.name = 'MS_Description'");
            sql.AppendLine(string.Format("WHERE  sc.id = OBJECT_ID('dbo.{0}')", tableName));

            if (this.No == "local")
                return DBAccess.RunSQLReturnTable(sql.ToString());

            string dsn = "";

            try
            {
                dsn = "Password=" + this.Password + ";Persist Security Info=True;User ID=" + this.UserID + ";Initial Catalog=" + this.DBName + ";Data Source=" + this.IP + ";Timeout=999;MultipleActiveResultSets=true";
                System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection();
                conn.ConnectionString = dsn;
                conn.Open();

                return BP.DA.DBAccess.RunSQLReturnTable(sql.ToString(), conn, dsn, CommandType.Text, null);
            }
            catch (Exception ex)
            {
                throw new Exception("@ʧ��:" + ex.Message + " dns:" + dsn);
            }

            return null;
        }

        protected override bool beforeDelete()
        {
            if (this.No == "local")
                throw new Exception("@Ĭ������(local)������ɾ��������.");
            return base.beforeDelete();
        }
        protected override bool beforeUpdate()
        {
            if (this.No == "local")
                throw new Exception("@Ĭ������(local)������ɾ��������.");
            return base.beforeUpdate();
        }

    }
    /// <summary>
    /// ����Դs
    /// </summary>
    public class SFDBSrcs : EntitiesNoName
    {
        #region ����
        /// <summary>
        /// ����Դs
        /// </summary>
        public SFDBSrcs()
        {
        }
        /// <summary>
        /// �õ����� Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SFDBSrc();
            }
        }
        #endregion

        public override int RetrieveAll()
        {
            int i = this.RetrieveAllFromDBSource();
            if (i == 0)
            {
                SFDBSrc src = new SFDBSrc();
                src.No = "local";
                src.Name = "Ӧ��ϵͳ�����ݿ�(Ĭ��)";
                src.Insert();
                this.AddEntity(src);
                return 1;
            }
            return i;
        }
    }
}
