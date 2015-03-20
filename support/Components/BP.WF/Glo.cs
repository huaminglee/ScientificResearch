using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Threading;
using System.Diagnostics;
//using Word = Microsoft.Office.Interop.Word;
using Microsoft.Win32;
using BP.Sys;
using BP.DA;
using BP.En;
using BP;
using BP.Web;
using System.Security.Cryptography;
using System.Text;
using BP.Port;
using BP.WF.Rpt;
using BP.WF.Data;
using BP.WF.Template;

namespace BP.WF
{
    /// <summary>
    /// 全局(方法处理)
    /// </summary>
    public class Glo
    {
        #region 执行安装/升级.
        /// <summary>
        /// 执行升级
        /// </summary>
        /// <returns></returns>
        public static string UpdataCCFlowVer()
        {
            #region 检查是否需要升级，并更新升级的业务逻辑.
            string val = "20150206";
            /*
             * 升级版本记录:
             * 2, 升级表单树,支持动态表单树.
             * 1, 执行一次Sender发送人的升级，原来由GenerWorkerList 转入WF_GenerWorkFlow.
             * 0, 静默升级启用日期.2014-12
             */
            string sql = "SELECT IntVal FROM Sys_Serial WHERE CfgKey='Ver'";
            string currVer = DBAccess.RunSQLReturnStringIsNull(sql, "");
            if (currVer == val)
                return null; //不需要升级.
            #endregion 检查是否需要升级，并更新升级的业务逻辑.

            string msg = "";
            try
            {
                TransferCustom tc = new TransferCustom();
                tc.CheckPhysicsTable();

                #region 更新表单的边界.2014-10-18
                MapDatas mds = new MapDatas();
                mds.RetrieveAll();

                foreach (MapData md in mds)
                    md.ResetMaxMinXY(); //更新边界.
                #endregion 更新表单的边界.

                #region 基础数据更新.
                //删除枚举值,让其自动生成.
                string enumKey = "";
                BP.DA.DBAccess.RunSQL("DELETE FROM Sys_Enum WHERE EnumKey IN ('SelectAccepterEnable','NodeFormType','StartGuideWay','" + FlowAttr.StartLimitRole + "','BillFileType','EventDoType','FormType','BatchRole','StartGuideWay','NodeFormType')");

                Node wf_Node = new Node();
                wf_Node.CheckPhysicsTable();
                // 设置节点ICON.
                sql = "UPDATE WF_Node SET ICON='/WF/Data/NodeIcon/审核.png' WHERE ICON IS NULL";
                BP.DA.DBAccess.RunSQL(sql);

                BP.WF.Template.Ext.NodeSheet nodeSheet = new BP.WF.Template.Ext.NodeSheet();
                nodeSheet.CheckPhysicsTable();
                // 升级手机应用. 2014-08-02.
                sql = "UPDATE WF_Node SET MPhone_WorkModel=0,MPhone_SrcModel=0,MPad_WorkModel=0,MPad_SrcModel=0 WHERE MPhone_WorkModel IS NULL";
                BP.DA.DBAccess.RunSQL(sql);
                #endregion 基础数据更新.

                #region 标签
                sql = "DELETE FROM Sys_EnCfg WHERE No='BP.WF.Template.Ext.NodeSheet'";
                BP.DA.DBAccess.RunSQL(sql);
                sql = "INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.WF.Template.Ext.NodeSheet','";
                sql += "@NodeID=基本配置";
                sql += "@FormType=表单";
                sql += "@FWCSta=审核组件,适用于sdk表单审核组件与ccform上的审核组件属性设置.";
                sql += "@SendLab=按钮权限,控制工作节点可操作按钮.";
                sql += "@RunModel=运行模式,分合流,父子流程";
                sql += "@AutoJumpRole0=跳转,自动跳转规则当遇到该节点时如何让其自动的执行下一步.";
                sql += "@MPhone_WorkModel=移动,与手机平板电脑相关的应用设置.";
                sql += "@WarningDays=考核,时效考核,质量考核.";
                //  sql += "@MsgCtrl=消息,流程消息信息.";
                sql += "@OfficeOpenLab=公文按钮,只有当该节点是公文流程时候有效";
                sql += "')";
                BP.DA.DBAccess.RunSQL(sql);

                sql = "DELETE FROM Sys_EnCfg WHERE No='BP.WF.Template.Ext.FlowSheet'";
                BP.DA.DBAccess.RunSQL(sql);
                sql = "INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.WF.Template.Ext.FlowSheet','";
                sql += "@No=基本配置";
                sql += "@FlowRunWay=启动方式,配置工作流程如何自动发起，该选项要与流程服务一起工作才有效.";
                sql += "@StartLimitRole=启动限制规则";
                sql += "@StartGuideWay=发起前置导航";
                sql += "@CFlowWay=延续流程";
                sql += "')";
                BP.DA.DBAccess.RunSQL(sql);
                #endregion

                #region 把节点的toolbarExcel, word 信息放入mapdata
                BP.WF.Template.Ext.NodeSheets nss = new Template.Ext.NodeSheets();
                nss.RetrieveAll();
                foreach (BP.WF.Template.Ext.NodeSheet ns in nss)
                {
                    ToolbarExcel te = new ToolbarExcel("ND" + ns.NodeID);
                    //te.Copy(ns);
                    //te.Update();
                }
                #endregion

                #region 升级SelectAccper
                Direction dir = new Direction();
                dir.CheckPhysicsTable();

                SelectAccper selectAccper = new SelectAccper();
                selectAccper.CheckPhysicsTable();
                #endregion

                #region 升级wf-generworkerlist 2014-05-09
                GenerWorkerList gwl = new GenerWorkerList();
                gwl.CheckPhysicsTable();
                #endregion 升级wf-generworkerlist 2014-05-09

                #region  升级 NodeToolbar
                FrmField ff = new FrmField();
                ff.CheckPhysicsTable();

                MapAttr attr = new MapAttr();
                attr.CheckPhysicsTable();

                NodeToolbar bar = new NodeToolbar();
                bar.CheckPhysicsTable();

                BP.WF.Template.FlowFormTree tree = new BP.WF.Template.FlowFormTree();
                tree.CheckPhysicsTable();

                FrmNode nff = new FrmNode();
                nff.CheckPhysicsTable();

                SysForm ssf = new SysForm();
                ssf.CheckPhysicsTable();

                SysFormTree ssft = new SysFormTree();
                ssft.CheckPhysicsTable();

                BP.Sys.FrmAttachment ath = new FrmAttachment();
                ath.CheckPhysicsTable();

                BP.Sys.FrmField ffs = new FrmField();
                ffs.CheckPhysicsTable();
                #endregion

                #region 执行sql．
                BP.DA.DBAccess.RunSQL("delete  from Sys_Enum WHERE EnumKey in ('BillFileType','EventDoType','FormType','BatchRole','StartGuideWay','NodeFormType')");
                DBAccess.RunSQL("UPDATE Sys_FrmSln SET FK_Flow =(SELECT FK_FLOW FROM WF_Node WHERE NODEID=Sys_FrmSln.FK_Node) WHERE FK_Flow IS NULL");
                try
                {
                    DBAccess.RunSQL("UPDATE WF_Flow SET StartGuidePara1=StartGuidePara WHERE  " + BP.Sys.SystemConfig.AppCenterDBLengthStr + "(StartGuidePara) >=1 ");
                }
                catch
                {
                }

                try
                {
                    DBAccess.RunSQL("UPDATE WF_FrmNode SET MyPK=FK_Frm+'_'+convert(varchar,FK_Node )+'_'+FK_Flow");
                }
                catch
                {
                }
                #endregion

                #region 检查必要的升级。
                //部门
                BP.Port.Dept d = new BP.Port.Dept();
                d.CheckPhysicsTable();

                FrmWorkCheck fwc = new FrmWorkCheck();
                fwc.CheckPhysicsTable();

                BP.WF.GenerWorkFlow gwf = new BP.WF.GenerWorkFlow();
                gwf.CheckPhysicsTable();

                Flow myfl = new Flow();
                myfl.CheckPhysicsTable();

                Node nd = new Node();
                nd.CheckPhysicsTable();
                #endregion 检查必要的升级。

                #region 执行更新.wf_node
                sql = "UPDATE WF_Node SET FWCType=0 WHERE FWCType IS NULL";
                sql += "@UPDATE WF_Node SET FWC_X=0 WHERE FWC_X IS NULL";
                sql += "@UPDATE WF_Node SET FWC_Y=0 WHERE FWC_Y IS NULL";
                sql += "@UPDATE WF_Node SET FWC_W=0 WHERE FWC_W IS NULL";
                sql += "@UPDATE WF_Node SET FWC_H=0 WHERE FWC_H IS NULL";
                BP.DA.DBAccess.RunSQLs(sql);
                #endregion 执行更新.

                #region 执行报表设计。
                Flows fls = new Flows();
                fls.RetrieveAll();
                foreach (Flow fl in fls)
                {
                    try
                    {
                        MapRpts rpts = new MapRpts(fl.No);
                    }
                    catch
                    {
                        fl.DoCheck();
                    }
                }
                #endregion

                #region 升级站内消息表 2013-10-20
                BP.WF.SMS sms = new SMS();
                sms.CheckPhysicsTable();
                #endregion

                #region 升级Img
                FrmImg img = new FrmImg();
                img.CheckPhysicsTable();
                #endregion

                #region 修复 mapattr UIHeight, UIWidth 类型错误.
                switch (BP.Sys.SystemConfig.AppCenterDBType)
                {
                    case DBType.Oracle:
                        msg = "@Sys_MapAttr 修改字段";
                        break;
                    case DBType.MSSQL:
                        msg = "@修改sql server控件高度和宽度字段。";
                        DBAccess.RunSQL("ALTER TABLE Sys_MapAttr ALTER COLUMN UIWidth float");
                        DBAccess.RunSQL("ALTER TABLE Sys_MapAttr ALTER COLUMN UIHeight float");
                        break;
                    default:
                        break;
                }
                #endregion

                #region 升级常用词汇
                switch (BP.Sys.SystemConfig.AppCenterDBType)
                {
                    case DBType.Oracle:
                        int i = DBAccess.RunSQLReturnCOUNT("SELECT * FROM USER_TAB_COLUMNS WHERE TABLE_NAME = 'SYS_DEFVAL' AND COLUMN_NAME = 'PARENTNO'");
                        if (i == 0)
                        {
                            DBAccess.RunSQL("drop table Sys_DefVal");
                            DefVal dv = new DefVal();
                            dv.CheckPhysicsTable();
                        }
                        msg = "@Sys_DefVal 修改字段";
                        break;
                    case DBType.MSSQL:
                        msg = "@修改 Sys_DefVal。";
                        i = DBAccess.RunSQLReturnCOUNT("SELECT * FROM SYSCOLUMNS WHERE ID=OBJECT_ID('Sys_DefVal') AND NAME='ParentNo'");
                        if (i == 0)
                        {
                            DBAccess.RunSQL("drop table Sys_DefVal");
                            DefVal dv = new DefVal();
                            dv.CheckPhysicsTable();
                        }
                        break;
                    default:
                        break;
                }
                #endregion

                #region 登陆更新错误
                msg = "@登陆时错误。";
                DBAccess.RunSQL("DELETE FROM Sys_Enum WHERE EnumKey IN ('DeliveryWay','RunModel','OutTimeDeal','FlowAppType')");
                try
                {
                    DBAccess.RunSQL("UPDATE Port_Station SET StaGrade=FK_StationType WHERE StaGrade IS null ");
                }
                catch
                {

                }
                #endregion 登陆更新错误

                #region 升级表单树
                // 首先检查是否升级过.
                sql = "SELECT * FROM Sys_FormTree WHERE No = '0'";
                DataTable formTree_dt = DBAccess.RunSQLReturnTable(sql);
                if (formTree_dt.Rows.Count == 0)
                {
                    /*没有升级过.增加根节点*/
                    SysFormTree formTree = new SysFormTree();
                    formTree.No = "0";
                    formTree.Name = "表单库";
                    formTree.ParentNo = "";
                    formTree.TreeNo = "0";
                    formTree.Idx = 0;
                    formTree.IsDir = true;

                    try
                    {
                        formTree.DirectInsert();
                    }
                    catch
                    {
                    }
                    //将表单库中的数据转入表单树
                    SysFormTrees formSorts = new SysFormTrees();
                    formSorts.RetrieveAll();

                    foreach (SysFormTree item in formSorts)
                    {
                        if (item.No == "0")
                            continue;

                        SysFormTree subFormTree = new SysFormTree();
                        subFormTree.No = item.No;
                        subFormTree.Name = item.Name;
                        subFormTree.ParentNo = "0";
                        subFormTree.Save();
                    }
                    //表单于表单树进行关联
                    sql = "UPDATE Sys_MapData SET FK_FormTree=FK_FrmSort WHERE FK_FrmSort <> '' AND FK_FrmSort is not null";
                    DBAccess.RunSQL(sql);
                }
                #endregion

                #region 重新生成view WF_EmpWorks,  2013-08-06.
                try
                {
                    BP.DA.DBAccess.RunSQL("DROP VIEW WF_EmpWorks");
                }
                catch
                {
                }

                try
                {
                    BP.DA.DBAccess.RunSQL("DROP VIEW WF_NodeExt");
                }
                catch
                {
                }

                //执行必须的sql.
                string sqlscript = BP.Sys.SystemConfig.CCFlowAppPath + "\\WF\\Data\\Install\\SQLScript\\InitCCFlowData.sql";
                BP.DA.DBAccess.RunSQLScript(sqlscript);
                #endregion

                #region 执行admin登陆. 2012-12-25 新版本.
                Emp emp = new Emp();
                emp.No = "admin";
                if (emp.RetrieveFromDBSources() == 1)
                {
                    BP.Web.WebUser.SignInOfGener(emp, true);
                }
                else
                {
                    emp.No = "admin";
                    emp.Name = "admin";
                    emp.FK_Dept = "01";
                    emp.Pass = "pub";
                    emp.Insert();
                    BP.Web.WebUser.SignInOfGener(emp, true);
                    //throw new Exception("admin 用户丢失，请注意大小写。");
                }
                #endregion 执行admin登陆.

                #region 修复 Sys_FrmImg 表字段 ImgAppType Tag0
                switch (BP.Sys.SystemConfig.AppCenterDBType)
                {
                    case DBType.Oracle:
                        int i = DBAccess.RunSQLReturnCOUNT("SELECT * FROM USER_TAB_COLUMNS WHERE TABLE_NAME = 'SYS_FRMIMG' AND COLUMN_NAME = 'TAG0'");
                        if (i == 0)
                        {
                            DBAccess.RunSQL("ALTER TABLE SYS_FRMIMG ADD (ImgAppType number,TAG0 nvarchar(500))");
                        }
                        msg = "@Sys_FrmImg 修改字段";
                        break;
                    case DBType.MSSQL:
                        msg = "@修改 Sys_FrmImg。";
                        i = DBAccess.RunSQLReturnCOUNT("SELECT * FROM SYSCOLUMNS WHERE ID=OBJECT_ID('Sys_FrmImg') AND NAME='Tag0'");
                        if (i == 0)
                        {
                            DBAccess.RunSQL("ALTER TABLE Sys_FrmImg ADD ImgAppType int");
                            DBAccess.RunSQL("ALTER TABLE Sys_FrmImg ADD Tag0 nvarchar(500)");
                        }
                        break;
                    default:
                        break;
                }
                #endregion


                // 最后更新版本号，然后返回.
                sql = "UPDATE Sys_Serial SET IntVal=" + val + " WHERE CfgKey='Ver'";
                if (DBAccess.RunSQL(sql) == 0)
                {
                    sql = "INSERT INTO Sys_Serial (CfgKey,IntVal) VALUES('Ver'," + val + ") ";
                    DBAccess.RunSQL(sql);
                }

                // 返回版本号.
                return val;
            }
            catch (Exception ex)
            {
                return "问题出处:" + ex.Message + "<hr>" + msg + "<br>详细信息:@" + ex.StackTrace + "<br>@<a href='../DBInstall.aspx' >点这里到系统升级界面。</a>";
            }
        }
        /// <summary>
        /// CCFlowAppPath
        /// </summary>
        public static string CCFlowAppPath
        {
            get
            {
                return BP.Sys.SystemConfig.GetValByKey("CCFlowAppPath", "/");
            }
        }
        /// <summary>
        /// 安装包
        /// </summary>
        public static void DoInstallDataBase(string lang, bool isInstallFlowDemo)
        {
            ArrayList al = null;
            string info = "BP.En.Entity";
            al = BP.En.ClassFactory.GetObjects(info);

            #region 1, 创建or修复表
            foreach (Object obj in al)
            {
                Entity en = null;
                en = obj as Entity;
                if (en == null)
                    continue;

                if (isInstallFlowDemo == false)
                {
                    /*如果不安装demo.*/
                    string clsName = en.ToString();
                    if (clsName != null)
                    {
                        if (clsName.Contains("BP.CN")
                            || clsName.Contains("BP.Demo"))
                            continue;
                    }
                }
                if (Glo.OSModel == WF.OSModel.WorkFlow)
                {
                    /*如果不安装gpm 就把bp.gpm 命名空间排除了. */
                    string clsName = en.ToString();
                    if (clsName != null)
                    {
                        if (clsName.Contains("BP.GMP"))
                            continue;
                    }
                }

                string table = null;
                try
                {
                    table = en.EnMap.PhysicsTable;
                    if (table == null)
                        continue;
                }
                catch
                {
                    continue;
                }

                switch (table)
                {
                    case "WF_EmpWorks":
                    case "WF_GenerEmpWorkDtls":
                    case "WF_GenerEmpWorks":
                    case "WF_NodeExt":
                    case "V_FlowData":
                        continue;
                    case "Sys_Enum":
                        en.CheckPhysicsTable();
                        break;
                    default:
                        en.CheckPhysicsTable();
                        break;
                }
                en.PKVal = "123";
                try
                {
                    en.RetrieveFromDBSources();
                }
                catch (Exception ex)
                {
                    Log.DebugWriteWarning(ex.Message);
                    en.CheckPhysicsTable();
                }
            }
            #endregion 修复

            #region 2, 注册枚举类型 SQL
            // 2, 注册枚举类型。
            BP.Sys.Xml.EnumInfoXmls xmls = new BP.Sys.Xml.EnumInfoXmls();
            xmls.RetrieveAll();
            foreach (BP.Sys.Xml.EnumInfoXml xml in xmls)
            {
                BP.Sys.SysEnums ses = new BP.Sys.SysEnums();
                ses.RegIt(xml.Key, xml.Vals);
            }
            #endregion 注册枚举类型

            #region 3, 执行基本的 sql

            if (Glo.OSModel == BP.WF.OSModel.BPM)
            {
                /*如果是BPM模式*/
                try
                {
                    BP.DA.DBAccess.RunSQL("DROP TABLE Port_EmpStation");
                    BP.DA.DBAccess.RunSQL("DROP TABLE Port_EmpDept");
                }
                catch
                {
                    // BP.DA.DBAccess.RunSQL("DROP VIEW Port_EmpStation");
                    //  BP.DA.DBAccess.RunSQL("DROP VIEW Port_EmpDept");
                }
            }

            string sqlscript = "";
            bool isHavePortData = true;
            if (DBAccess.RunSQLReturnValInt("SELECT COUNT(NO) as Num FROM Port_Emp", 0) == 0)
                isHavePortData = false;
            else
                isHavePortData = true;

            if (isHavePortData == false)
            {
                /*如果没有安装oa.*/
                try
                {
                    BP.DA.DBAccess.RunSQL("DROP VIEW Port_EmpDept");
                    BP.DA.DBAccess.RunSQL("DROP VIEW Port_EmpStation");
                }
                catch
                {
                    BP.Port.EmpDept ed = new BP.Port.EmpDept();
                    ed.CheckPhysicsTable();
                    BP.Port.EmpStation es = new BP.Port.EmpStation();
                    es.CheckPhysicsTable();
                }

                try
                {
                    sqlscript = BP.Sys.SystemConfig.CCFlowAppPath + "\\WF\\Data\\Install\\SQLScript\\Port_Inc_CH_WorkFlow.sql";
                    BP.DA.DBAccess.RunSQLScript(sqlscript);
                }
                catch (Exception ex)
                {
                    if (BP.WF.Glo.OSModel == WF.OSModel.WorkFlow)
                        throw new Exception("@ 执行sql失败:有可能的原因是,你安装上了GPM，又安装ccflow，但是web.config的模式不是BPM模式，你需要web.config的OSModel=1");
                    throw ex;
                }

                sqlscript = BP.Sys.SystemConfig.CCFlowAppPath + "\\WF\\Data\\Install\\SQLScript\\Port_Inc_CH_WorkFlow.sql";
                BP.DA.DBAccess.RunSQLScript(sqlscript);
            }

            if (isInstallFlowDemo == false)
            {
                SysFormTree frmSort = new SysFormTree();
                frmSort.No = "01";
                frmSort.Name = "表单类别1";
                frmSort.Insert();
            }
            #endregion 修复

            #region 4, 创建视图与数据.
            //执行必须的sql.
            sqlscript = BP.Sys.SystemConfig.CCFlowAppPath + "\\WF\\Data\\Install\\SQLScript\\InitCCFlowData.sql";
            BP.DA.DBAccess.RunSQLScript(sqlscript);
            #endregion 创建视图与数据

            #region 5, 初始化数据.
            if (isInstallFlowDemo)
            {
                sqlscript = SystemConfig.PathOfData + "\\Install\\SQLScript\\InitPublicData.sql";
                BP.DA.DBAccess.RunSQLScript(sqlscript);
            }
            else
            {
                FlowSort fs = new FlowSort();
                fs.No = "02";
                fs.ParentNo = "99";
                fs.Name = "其他类";
                fs.DirectInsert();
            }
            #endregion 初始化数据

            #region 6, 生成临时的wf数据。
            if (isInstallFlowDemo)
            {
                BP.Port.Emps emps = new BP.Port.Emps();
                emps.RetrieveAllFromDBSource();
                int i = 0;
                foreach (BP.Port.Emp emp in emps)
                {
                    i++;
                    BP.WF.Port.WFEmp wfEmp = new BP.WF.Port.WFEmp();
                    wfEmp.Copy(emp);
                    wfEmp.No = emp.No;

                    if (wfEmp.Email.Length == 0)
                        wfEmp.Email = emp.No + "@ccflow.org";

                    if (wfEmp.Tel.Length == 0)
                        wfEmp.Tel = "82374939-6" + i.ToString().PadLeft(2, '0');

                    if (wfEmp.IsExits)
                        wfEmp.Update();
                    else
                        wfEmp.Insert();
                }

                // 生成简历数据.
                int oid = 1000;
                foreach (BP.Port.Emp emp in emps)
                {
                    //for (int myIdx = 0; myIdx < 6; myIdx++)
                    //{
                    //    BP.WF.Demo.Resume re = new Demo.Resume();
                    //    re.NianYue = "200" + myIdx + "年01月";
                    //    re.FK_Emp = emp.No;
                    //    re.GongZuoDanWei = "工作部门-" + myIdx;
                    //    re.ZhengMingRen = "张" + myIdx;
                    //    re.BeiZhu = emp.Name + "同志工作认真.";
                    //    oid++;
                    //    re.InsertAsOID(oid);
                    //}
                }
                // 生成年度月份数据.
                string sql = "";
                DateTime dtNow = DateTime.Now;
                for (int num = 0; num < 12; num++)
                {
                    sql = "INSERT INTO Pub_NY (No,Name) VALUES ('" + dtNow.ToString("yyyy-MM") + "','" + dtNow.ToString("yyyy-MM") + "')";
                    dtNow = dtNow.AddMonths(1);
                }
            }
            #endregion 初始化数据

            #region 执行补充的sql, 让外键的字段长度都设置成100.
            DBAccess.RunSQL("UPDATE Sys_MapAttr SET maxlen=100 WHERE LGType=2 AND MaxLen<100");
            DBAccess.RunSQL("UPDATE Sys_MapAttr SET maxlen=100 WHERE KeyOfEn='FK_Dept'");

            //Nodes nds = new Nodes();
            //nds.RetrieveAll();
            //foreach (Node nd in nds)
            //    nd.HisWork.CheckPhysicsTable();

            #endregion 执行补充的sql, 让外键的字段长度都设置成100.

            // 删除空白的字段分组.
            BP.WF.DTS.DeleteBlankGroupField dts = new DTS.DeleteBlankGroupField();
            dts.Do();
        }

        /// <summary>
        /// 安装CCIM
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="yunXingHuanjing"></param>
        /// <param name="isDemo"></param>
        public static void DoInstallCCIM()
        {
            string sqlscript = SystemConfig.PathOfData + "Install\\SQLScript\\CCIM.sql";
            BP.DA.DBAccess.RunSQLScriptGo(sqlscript);
        }
        public static void KillProcess(string processName) //杀掉进程的方法
        {
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();
            foreach (System.Diagnostics.Process pro in processes)
            {
                string name = pro.ProcessName + ".exe";
                if (name.ToLower() == processName.ToLower())
                    pro.Kill();
            }
        }
        /// <summary>
        /// 产生新的编号
        /// </summary>
        /// <param name="rptKey"></param>
        /// <returns></returns>
        public static string GenerFlowNo(string rptKey)
        {
            rptKey = rptKey.Replace("ND", "");
            rptKey = rptKey.Replace("Rpt", "");
            switch (rptKey.Length)
            {
                case 0:
                    return "001";
                case 1:
                    return "00" + rptKey;
                case 2:
                    return "0" + rptKey;
                case 3:
                    return rptKey;
                default:
                    return "001";
            }
            return rptKey;
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool IsShowFlowNum
        {
            get
            {
                switch (SystemConfig.AppSettings["IsShowFlowNum"])
                {
                    case "1":
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// 产生word文档.
        /// </summary>
        /// <param name="wk"></param>
        public static void GenerWord(object filename, Work wk)
        {
            BP.WF.Glo.KillProcess("WINWORD.EXE");
            string enName = wk.EnMap.PhysicsTable;
            try
            {
                RegistryKey delKey = Registry.LocalMachine.OpenSubKey(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Shared Tools\Text Converters\Import\",
                    true);
                delKey.DeleteValue("MSWord6.wpc");
                delKey.Close();
            }
            catch
            {
            }

            GroupField currGF = new GroupField();
            MapAttrs mattrs = new MapAttrs(enName);
            GroupFields gfs = new GroupFields(enName);
            MapDtls dtls = new MapDtls(enName);
            foreach (MapDtl dtl in dtls)
                dtl.IsUse = false;

            // 计算出来单元格的行数。
            int rowNum = 0;
            foreach (GroupField gf in gfs)
            {
                rowNum++;
                bool isLeft = true;
                foreach (MapAttr attr in mattrs)
                {
                    if (attr.UIVisible == false)
                        continue;

                    if (attr.GroupID != gf.OID)
                        continue;

                    if (attr.UIIsLine)
                    {
                        rowNum++;
                        isLeft = true;
                        continue;
                    }

                    if (isLeft == false)
                        rowNum++;
                    isLeft = !isLeft;
                }
            }

            rowNum = rowNum + 2 + dtls.Count;

            // 创建Word文档
            string CheckedInfo = "";
            string message = "";
            Object Nothing = System.Reflection.Missing.Value;

            #region 没用代码
            //  object filename = fileName;

            //Word.Application WordApp = new Word.ApplicationClass();
            //Word.Document WordDoc = WordApp.Documents.Add(ref  Nothing, ref  Nothing, ref  Nothing, ref  Nothing);
            //try
            //{
            //    WordApp.ActiveWindow.View.Type = Word.WdViewType.wdOutlineView;
            //    WordApp.ActiveWindow.View.SeekView = Word.WdSeekView.wdSeekPrimaryHeader;

            //    #region 增加页眉
            //    // 添加页眉 插入图片
            //    string pict = SystemConfig.PathOfDataUser + "log.jpg"; // 图片所在路径
            //    if (System.IO.File.Exists(pict))
            //    {
            //        System.Drawing.Image img = System.Drawing.Image.FromFile(pict);
            //        object LinkToFile = false;
            //        object SaveWithDocument = true;
            //        object Anchor = WordDoc.Application.Selection.Range;
            //        WordDoc.Application.ActiveDocument.InlineShapes.AddPicture(pict, ref  LinkToFile,
            //            ref  SaveWithDocument, ref  Anchor);
            //        //    WordDoc.Application.ActiveDocument.InlineShapes[1].Width = img.Width; // 图片宽度
            //        //    WordDoc.Application.ActiveDocument.InlineShapes[1].Height = img.Height; // 图片高度
            //    }
            //    WordApp.ActiveWindow.ActivePane.Selection.InsertAfter("[驰骋业务流程管理系统 http://ccFlow.org]");
            //    WordApp.Selection.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft; // 设置右对齐
            //    WordApp.ActiveWindow.View.SeekView = Word.WdSeekView.wdSeekMainDocument; // 跳出页眉设置
            //    WordApp.Selection.ParagraphFormat.LineSpacing = 15f; // 设置文档的行间距
            //    #endregion

            //    // 移动焦点并换行
            //    object count = 14;
            //    object WdLine = Word.WdUnits.wdLine; // 换一行;
            //    WordApp.Selection.MoveDown(ref  WdLine, ref  count, ref  Nothing); // 移动焦点
            //    WordApp.Selection.TypeParagraph(); // 插入段落

            //    // 文档中创建表格
            //    Word.Table newTable = WordDoc.Tables.Add(WordApp.Selection.Range, rowNum, 4, ref  Nothing, ref  Nothing);

            //    // 设置表格样式
            //    newTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleThickThinLargeGap;
            //    newTable.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;

            //    newTable.Columns[1].Width = 100f;
            //    newTable.Columns[2].Width = 100f;
            //    newTable.Columns[3].Width = 100f;
            //    newTable.Columns[4].Width = 100f;

            //    // 填充表格内容
            //    newTable.Cell(1, 1).Range.Text = wk.EnDesc;
            //    newTable.Cell(1, 1).Range.Bold = 2; // 设置单元格中字体为粗体

            //    // 合并单元格
            //    newTable.Cell(1, 1).Merge(newTable.Cell(1, 4));
            //    WordApp.Selection.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter; // 垂直居中
            //    WordApp.Selection.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter; // 水平居中

            //    int groupIdx = 1;
            //    foreach (GroupField gf in gfs)
            //    {
            //        groupIdx++;
            //        // 填充表格内容
            //        newTable.Cell(groupIdx, 1).Range.Text = gf.Lab;
            //        newTable.Cell(groupIdx, 1).Range.Font.Color = Word.WdColor.wdColorDarkBlue; // 设置单元格内字体颜色
            //        newTable.Cell(groupIdx, 1).Shading.BackgroundPatternColor = Word.WdColor.wdColorGray25;
            //        // 合并单元格
            //        newTable.Cell(groupIdx, 1).Merge(newTable.Cell(groupIdx, 4));
            //        WordApp.Selection.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

            //        groupIdx++;

            //        bool isLeft = true;
            //        bool isColumns2 = false;
            //        int currColumnIndex = 0;
            //        foreach (MapAttr attr in mattrs)
            //        {
            //            if (attr.UIVisible == false)
            //                continue;

            //            if (attr.GroupID != gf.OID)
            //                continue;

            //            if (newTable.Rows.Count < groupIdx)
            //                continue;

            //            #region 增加从表
            //            foreach (MapDtl dtl in dtls)
            //            {
            //                if (dtl.IsUse)
            //                    continue;

            //                if (dtl.RowIdx != groupIdx - 3)
            //                    continue;

            //                if (gf.OID != dtl.GroupID)
            //                    continue;

            //                GEDtls dtlsDB = new GEDtls(dtl.No);
            //                QueryObject qo = new QueryObject(dtlsDB);
            //                switch (dtl.DtlOpenType)
            //                {
            //                    case DtlOpenType.ForEmp:
            //                        qo.AddWhere(GEDtlAttr.RefPK, wk.OID);
            //                        break;
            //                    case DtlOpenType.ForWorkID:
            //                        qo.AddWhere(GEDtlAttr.RefPK, wk.OID);
            //                        break;
            //                    case DtlOpenType.ForFID:
            //                        qo.AddWhere(GEDtlAttr.FID, wk.OID);
            //                        break;
            //                }
            //                qo.DoQuery();

            //                newTable.Rows[groupIdx].SetHeight(100f, Word.WdRowHeightRule.wdRowHeightAtLeast);
            //                newTable.Cell(groupIdx, 1).Merge(newTable.Cell(groupIdx, 4));

            //                Attrs dtlAttrs = dtl.GenerMap().Attrs;
            //                int colNum = 0;
            //                foreach (Attr attrDtl in dtlAttrs)
            //                {
            //                    if (attrDtl.UIVisible == false)
            //                        continue;
            //                    colNum++;
            //                }

            //                newTable.Cell(groupIdx, 1).Select();
            //                WordApp.Selection.Delete(ref Nothing, ref Nothing);
            //                Word.Table newTableDtl = WordDoc.Tables.Add(WordApp.Selection.Range, dtlsDB.Count + 1, colNum,
            //                    ref Nothing, ref Nothing);

            //                newTableDtl.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            //                newTableDtl.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;

            //                int colIdx = 1;
            //                foreach (Attr attrDtl in dtlAttrs)
            //                {
            //                    if (attrDtl.UIVisible == false)
            //                        continue;
            //                    newTableDtl.Cell(1, colIdx).Range.Text = attrDtl.Desc;
            //                    colIdx++;
            //                }

            //                int idxRow = 1;
            //                foreach (GEDtl item in dtlsDB)
            //                {
            //                    idxRow++;
            //                    int columIdx = 0;
            //                    foreach (Attr attrDtl in dtlAttrs)
            //                    {
            //                        if (attrDtl.UIVisible == false)
            //                            continue;
            //                        columIdx++;

            //                        if (attrDtl.IsFKorEnum)
            //                            newTableDtl.Cell(idxRow, columIdx).Range.Text = item.GetValRefTextByKey(attrDtl.Key);
            //                        else
            //                        {
            //                            if (attrDtl.MyDataType == DataType.AppMoney)
            //                                newTableDtl.Cell(idxRow, columIdx).Range.Text = item.GetValMoneyByKey(attrDtl.Key).ToString("0.00");
            //                            else
            //                                newTableDtl.Cell(idxRow, columIdx).Range.Text = item.GetValStrByKey(attrDtl.Key);

            //                            if (attrDtl.IsNum)
            //                                newTableDtl.Cell(idxRow, columIdx).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
            //                        }
            //                    }
            //                }

            //                groupIdx++;
            //                isLeft = true;
            //            }
            //            #endregion 增加从表

            //            if (attr.UIIsLine)
            //            {
            //                currColumnIndex = 0;
            //                isLeft = true;
            //                if (attr.IsBigDoc)
            //                {
            //                    newTable.Rows[groupIdx].SetHeight(100f, Word.WdRowHeightRule.wdRowHeightAtLeast);
            //                    newTable.Cell(groupIdx, 1).Merge(newTable.Cell(groupIdx, 4));
            //                    newTable.Cell(groupIdx, 1).Range.Text = attr.Name + ":\r\n" + wk.GetValStrByKey(attr.KeyOfEn);
            //                }
            //                else
            //                {
            //                    newTable.Cell(groupIdx, 2).Merge(newTable.Cell(groupIdx, 4));
            //                    newTable.Cell(groupIdx, 1).Range.Text = attr.Name;
            //                    newTable.Cell(groupIdx, 2).Range.Text = wk.GetValStrByKey(attr.KeyOfEn);
            //                }
            //                groupIdx++;
            //                continue;
            //            }
            //            else
            //            {
            //                if (attr.IsBigDoc)
            //                {
            //                    if (currColumnIndex == 2)
            //                    {
            //                        currColumnIndex = 0;
            //                    }

            //                    newTable.Rows[groupIdx].SetHeight(100f, Word.WdRowHeightRule.wdRowHeightAtLeast);
            //                    if (currColumnIndex == 0)
            //                    {
            //                        newTable.Cell(groupIdx, 1).Merge(newTable.Cell(groupIdx, 2));
            //                        newTable.Cell(groupIdx, 1).Range.Text = attr.Name + ":\r\n" + wk.GetValStrByKey(attr.KeyOfEn);
            //                        currColumnIndex = 3;
            //                        continue;
            //                    }
            //                    else if (currColumnIndex == 3)
            //                    {
            //                        newTable.Cell(groupIdx, 2).Merge(newTable.Cell(groupIdx, 3));
            //                        newTable.Cell(groupIdx, 2).Range.Text = attr.Name + ":\r\n" + wk.GetValStrByKey(attr.KeyOfEn);
            //                        currColumnIndex = 0;
            //                        groupIdx++;
            //                        continue;
            //                    }
            //                    else
            //                    {
            //                        continue;
            //                    }
            //                }
            //                else
            //                {
            //                    string s = "";
            //                    if (attr.LGType == FieldTypeS.Normal)
            //                    {
            //                        if (attr.MyDataType == DataType.AppMoney)
            //                            s = wk.GetValDecimalByKey(attr.KeyOfEn).ToString("0.00");
            //                        else
            //                            s = wk.GetValStrByKey(attr.KeyOfEn);
            //                    }
            //                    else
            //                    {
            //                        s = wk.GetValRefTextByKey(attr.KeyOfEn);
            //                    }

            //                    switch (currColumnIndex)
            //                    {
            //                        case 0:
            //                            newTable.Cell(groupIdx, 1).Range.Text = attr.Name;
            //                            if (attr.IsSigan)
            //                            {
            //                                string path = BP.Sys.SystemConfig.PathOfDataUser + "\\Siganture\\" + s + ".jpg";
            //                                if (System.IO.File.Exists(path))
            //                                {
            //                                    System.Drawing.Image img = System.Drawing.Image.FromFile(path);
            //                                    object LinkToFile = false;
            //                                    object SaveWithDocument = true;
            //                                    //object Anchor = WordDoc.Application.Selection.Range;
            //                                    object Anchor = newTable.Cell(groupIdx, 2).Range;

            //                                    WordDoc.Application.ActiveDocument.InlineShapes.AddPicture(path, ref  LinkToFile,
            //                                        ref  SaveWithDocument, ref  Anchor);
            //                                    //    WordDoc.Application.ActiveDocument.InlineShapes[1].Width = img.Width; // 图片宽度
            //                                    //    WordDoc.Application.ActiveDocument.InlineShapes[1].Height = img.Height; // 图片高度
            //                                }
            //                                else
            //                                {
            //                                    newTable.Cell(groupIdx, 2).Range.Text = s;
            //                                }
            //                            }
            //                            else
            //                            {
            //                                if (attr.IsNum)
            //                                {
            //                                    newTable.Cell(groupIdx, 2).Range.Text = s;
            //                                    newTable.Cell(groupIdx, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
            //                                }
            //                                else
            //                                {
            //                                    newTable.Cell(groupIdx, 2).Range.Text = s;
            //                                }
            //                            }
            //                            currColumnIndex = 1;
            //                            continue;
            //                            break;
            //                        case 1:
            //                            newTable.Cell(groupIdx, 3).Range.Text = attr.Name;
            //                            if (attr.IsSigan)
            //                            {
            //                                string path = BP.Sys.SystemConfig.PathOfDataUser + "\\Siganture\\" + s + ".jpg";
            //                                if (System.IO.File.Exists(path))
            //                                {
            //                                    System.Drawing.Image img = System.Drawing.Image.FromFile(path);
            //                                    object LinkToFile = false;
            //                                    object SaveWithDocument = true;
            //                                    object Anchor = newTable.Cell(groupIdx, 4).Range;
            //                                    WordDoc.Application.ActiveDocument.InlineShapes.AddPicture(path, ref  LinkToFile,
            //                                        ref  SaveWithDocument, ref  Anchor);
            //                                }
            //                                else
            //                                {
            //                                    newTable.Cell(groupIdx, 4).Range.Text = s;
            //                                }
            //                            }
            //                            else
            //                            {
            //                                if (attr.IsNum)
            //                                {
            //                                    newTable.Cell(groupIdx, 4).Range.Text = s;
            //                                    newTable.Cell(groupIdx, 4).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
            //                                }
            //                                else
            //                                {
            //                                    newTable.Cell(groupIdx, 4).Range.Text = s;
            //                                }
            //                            }
            //                            currColumnIndex = 0;
            //                            groupIdx++;
            //                            continue;
            //                            break;
            //                        default:
            //                            break;
            //                    }
            //                }
            //            }
            //        }
            //    }  //结束循环

            //    #region 添加页脚
            //    WordApp.ActiveWindow.View.SeekView = Word.WdSeekView.wdSeekPrimaryFooter;
            //    WordApp.ActiveWindow.ActivePane.Selection.InsertAfter("模板由ccflow自动生成，严谨转载。此流程的详细内容请访问 http://doc.ccFlow.org。 建造流程管理系统请致电: 0531-82374939  ");
            //    WordApp.Selection.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
            //    #endregion

            //    // 文件保存
            //    WordDoc.SaveAs(ref  filename, ref  Nothing, ref  Nothing, ref  Nothing,
            //        ref  Nothing, ref  Nothing, ref  Nothing, ref  Nothing,
            //        ref  Nothing, ref  Nothing, ref  Nothing, ref  Nothing, ref  Nothing,
            //        ref  Nothing, ref  Nothing, ref  Nothing);

            //    WordDoc.Close(ref  Nothing, ref  Nothing, ref  Nothing);
            //    WordApp.Quit(ref  Nothing, ref  Nothing, ref  Nothing);
            //    try
            //    {
            //        string docFile = filename.ToString();
            //        string pdfFile = docFile.Replace(".doc", ".pdf");
            //        Glo.Rtf2PDF(docFile, pdfFile);
            //    }
            //    catch (Exception ex)
            //    {
            //        BP.DA.Log.DebugWriteInfo("@生成pdf失败." + ex.Message);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //    // WordApp.Quit(ref  Nothing, ref  Nothing, ref  Nothing);
            //    WordDoc.Close(ref  Nothing, ref  Nothing, ref  Nothing);
            //    WordApp.Quit(ref  Nothing, ref  Nothing, ref  Nothing);
            //}
            #endregion
        }
        #endregion 执行安装.

        #region 全局的方法处理
        public static List<string> FlowFields
        {
            get
            {
                return typeof(GERptAttr).GetFields().Select(o => o.Name).ToList();
            }
        }
        /// <summary>
        /// 根据文字处理抄送，与发送人
        /// </summary>
        /// <param name="note"></param>
        /// <param name="emps"></param>
        public static void DealNote(string note, BP.Port.Emps emps)
        {
            note = "请综合处阅知。李江龙核示。请王薇、田晓红批示。";
            note = note.Replace("阅知", "阅知@");

            note = note.Replace("请", "@");
            note = note.Replace("呈", "@");
            note = note.Replace("报", "@");
            string[] strs = note.Split('@');

            string ccTo = "";
            string sendTo = "";
            foreach (string str in strs)
            {
                if (string.IsNullOrEmpty(str))
                    continue;

                if (str.Contains("阅知") == true
                    || str.Contains("阅度") == true)
                {
                    /*抄送的.*/
                    foreach (BP.Port.Emp emp in emps)
                    {
                        if (str.Contains(emp.No) == false)
                            continue;
                        ccTo += emp.No + ",";
                    }
                    continue;
                }

                if (str.Contains("阅处") == true
                  || str.Contains("阅办") == true)
                {
                    /*发送送的.*/
                    foreach (BP.Port.Emp emp in emps)
                    {
                        if (str.Contains(emp.No) == false)
                            continue;
                        sendTo += emp.No + ",";
                    }
                    continue;
                }
            }
        }



        #region 与流程事件实体相关.
        private static Hashtable Htable_FlowFEE = null;
        /// <summary>
        /// 获得节点事件实体
        /// </summary>
        /// <param name="enName">实例名称</param>
        /// <returns>获得节点事件实体,如果没有就返回为空.</returns>
        public static FlowEventBase GetFlowEventEntityByEnName(string enName)
        {
            if (Htable_FlowFEE == null || Htable_FlowFEE.Count == 0)
            {
                Htable_FlowFEE = new Hashtable();
                ArrayList al = BP.En.ClassFactory.GetObjects("BP.WF.FlowEventBase");
                foreach (FlowEventBase en in al)
                {
                    Htable_FlowFEE.Add(en.ToString(), en);
                }
            }
            FlowEventBase myen = Htable_FlowFEE[enName] as FlowEventBase;
            if (myen == null)
                throw new Exception("@根据类名称获取流程事件实体实例出现错误:" + enName + ",没有找到该类的实体.");
            return myen;
        }
        /// <summary>
        /// 获得节点事件实体根据节点编码.
        /// </summary>
        /// <param name="NodeMark">节点编码</param>
        /// <returns>返回实体，或者null</returns>
        public static FlowEventBase GetFlowEventEntityByFlowMark(string flowMark)
        {
            if (Htable_FlowFEE == null || Htable_FlowFEE.Count == 0)
            {
                Htable_FlowFEE = new Hashtable();
                ArrayList al = BP.En.ClassFactory.GetObjects("BP.WF.FlowEventBase");
                Htable_FlowFEE.Clear();
                foreach (FlowEventBase en in al)
                {
                    Htable_FlowFEE.Add(en.ToString(), en);
                }
            }

            foreach (string key in Htable_FlowFEE.Keys)
            {
                FlowEventBase fee = Htable_FlowFEE[key] as FlowEventBase;
                if (fee.FlowMark == flowMark)
                    return fee;
            }

            //for (int i = 0; i < Htable_FlowFEE.Count; i++)
            //{
            //    FlowEventBase fee = Htable_FlowFEE[i] as FlowEventBase;
            //}
            return null;
        }
        #endregion 与流程事件实体相关.

        /// <summary>
        /// 执行发送工作后处理的业务逻辑
        /// 用于流程发送后事件调用.
        /// 如果处理失败，就会抛出异常.
        /// </summary>
        public static void DealBuinessAfterSendWork(string fk_flow, Int64 workid,
            string doFunc, string WorkIDs, string cFlowNo, int cNodeID, string cEmp)
        {
            if (doFunc == "SetParentFlow")
            {
                /* 如果需要设置子父流程信息.
                 * 应用于合并审批,当多个子流程合并审批,审批后发起一个父流程.
                 */
                string[] workids = WorkIDs.Split(',');
                string okworkids = ""; //成功发送后的workids.
                GenerWorkFlow gwf = new GenerWorkFlow();
                foreach (string id in workids)
                {
                    if (string.IsNullOrEmpty(id))
                        continue;

                    // 把数据copy到里面,让子流程也可以得到父流程的数据。
                    Int64 workidC = Int64.Parse(id);

                    //设置当前流程的ID
                    BP.WF.Dev2Interface.SetParentInfo(cFlowNo, workidC, fk_flow, workid, cNodeID, cEmp);

                    // 判断是否可以执行，不能执行也要发送下去.
                    gwf.WorkID = workidC;
                    if (gwf.RetrieveFromDBSources() == 0)
                        continue;

                    // 是否可以执行？
                    if (BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(gwf.FK_Flow, gwf.FK_Node, workidC, WebUser.No) == false)
                        continue;

                    //执行向下发送.
                    try
                    {
                        BP.WF.Dev2Interface.Node_SendWork(cFlowNo, workidC);
                        okworkids += workidC;
                    }
                    catch (Exception ex)
                    {
                        #region 如果有一个发送失败，就撤销子流程与父流程.
                        //首先把主流程撤销发送.
                        BP.WF.Dev2Interface.Flow_DoUnSend(fk_flow, workid);

                        //把已经发送成功的子流程撤销发送.
                        string[] myokwokid = okworkids.Split(',');
                        foreach (string okwokid in myokwokid)
                        {
                            if (string.IsNullOrEmpty(id))
                                continue;

                            // 把数据copy到里面,让子流程也可以得到父流程的数据。
                            workidC = Int64.Parse(id);
                            BP.WF.Dev2Interface.Flow_DoUnSend(cFlowNo, workidC);
                        }
                        #endregion 如果有一个发送失败，就撤销子流程与父流程.
                        throw new Exception("@在执行子流程(" + gwf.Title + ")发送时出现如下错误:" + ex.Message);
                    }
                }
            }

        }
        #endregion 全局的方法处理

        #region web.config 属性.
        /// <summary>
        /// 是否admin
        /// </summary>
        public static bool IsAdmin
        {
            get
            {
                string s = BP.Sys.SystemConfig.AppSettings["adminers"];
                if (string.IsNullOrEmpty(s))
                    s = "admin,";
                return s.Contains(BP.Web.WebUser.No);
            }
        }
        /// <summary>
        /// 获取mapdata字段查询Like。
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="colName">列编号</param>
        public static string MapDataLikeKey(string flowNo, string colName)
        {
            flowNo = int.Parse(flowNo).ToString();
            string len = BP.Sys.SystemConfig.AppCenterDBLengthStr;
            if (flowNo.Length == 1)
                return " " + colName + " LIKE 'ND" + flowNo + "%' AND " + len + "(" + colName + ")=5";
            if (flowNo.Length == 2)
                return " " + colName + " LIKE 'ND" + flowNo + "%' AND " + len + "(" + colName + ")=6";
            if (flowNo.Length == 3)
                return " " + colName + " LIKE 'ND" + flowNo + "%' AND " + len + "(" + colName + ")=7";

            return " " + colName + " LIKE 'ND" + flowNo + "%' AND " + len + "(" + colName + ")=8";
        }
        /// <summary>
        /// 短信时间发送从
        /// 默认从 8 点开始.
        /// </summary>
        public static int SMSSendTimeFromHour
        {
            get
            {
                try
                {
                    return int.Parse(BP.Sys.SystemConfig.AppSettings["SMSSendTimeFromHour"]);
                }
                catch
                {
                    return 8;
                }
            }
        }
        /// <summary>
        /// 短信时间发送到
        /// 默认到 20 点结束.
        /// </summary>
        public static int SMSSendTimeToHour
        {
            get
            {
                try
                {
                    return int.Parse(BP.Sys.SystemConfig.AppSettings["SMSSendTimeToHour"]);
                }
                catch
                {
                    return 8;
                }
            }
        }
        #endregion webconfig属性.

        #region 常用方法
        private static string html = "";
        private static ArrayList htmlArr = new ArrayList();
        private static string backHtml = "";
        private static Int64 workid = 0;
        /// <summary>
        /// 模拟运行
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="empNo">要执行的人员.</param>
        /// <returns>执行信息.</returns>
        public static string Simulation_RunOne(string flowNo, string empNo, string paras)
        {
            backHtml = "";//需要重新赋空值
            Hashtable ht = null;
            if (string.IsNullOrEmpty(paras) == false)
            {
                AtPara ap = new AtPara(paras);
                ht = ap.HisHT;
            }

            Emp emp = new Emp(empNo);
            backHtml += " **** 开始使用:" + Glo.GenerUserImgSmallerHtml(emp.No, emp.Name) + "登录模拟执行工作流程";
            BP.WF.Dev2Interface.Port_Login(empNo);

            workid = BP.WF.Dev2Interface.Node_CreateBlankWork(flowNo, ht, null, emp.No, null);
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(flowNo, workid, ht);
            backHtml += objs.ToMsgOfHtml().Replace("@", "<br>@");  //记录消息.


            string[] accepters = objs.VarAcceptersID.Split(',');


            foreach (string acce in accepters)
            {
                if (string.IsNullOrEmpty(acce) == true)
                    continue;

                // 执行发送.
                Simulation_Run_S1(flowNo, workid, acce, ht, empNo);
                break;
            }
            //return html;
            //return htmlArr;
            return backHtml;
        }
        private static bool isAdd = true;
        private static void Simulation_Run_S1(string flowNo, Int64 workid, string empNo, Hashtable ht, string beginEmp)
        {
            //htmlArr.Add(html);
            Emp emp = new Emp(empNo);
            //html = "";
            backHtml += "empNo" + beginEmp;
            backHtml += "<br> **** 让:" + Glo.GenerUserImgSmallerHtml(emp.No, emp.Name) + "执行模拟登录. ";
            // 让其登录.
            BP.WF.Dev2Interface.Port_Login(empNo);

            //执行发送.
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(flowNo, workid, ht);
            backHtml += "<br>" + objs.ToMsgOfHtml().Replace("@", "<br>@");

            if (objs.VarAcceptersID == null)
            {
                isAdd = false;
                backHtml += " <br> **** 流程结束,查看<a href='/WF/WFRpt.aspx?WorkID=" + workid + "&FK_Flow=" + flowNo + "' target=_blank >流程轨迹</a> ====";
                //htmlArr.Add(html);
                //backHtml += "nextEmpNo";
                return;
            }

            if (string.IsNullOrEmpty(objs.VarAcceptersID))//此处添加为空判断，跳过下面方法的执行，否则出错。
            {
                return;
            }
            string[] accepters = objs.VarAcceptersID.Split(',');

            foreach (string acce in accepters)
            {
                if (string.IsNullOrEmpty(acce) == true)
                    continue;

                //执行发送.
                Simulation_Run_S1(flowNo, workid, acce, ht, beginEmp);
                break; //就不让其执行了.
            }
        }
        /// <summary>
        /// 是否手机访问?
        /// </summary>
        /// <returns></returns>
        public static bool IsMobile()
        {
            if (SystemConfig.IsBSsystem == false)
                return false;

            string agent = (BP.Sys.Glo.Request.UserAgent + "").ToLower().Trim();
            if (agent == "" || agent.IndexOf("mozilla") != -1 || agent.IndexOf("opera") != -1)
                return false;
            return true;
        }
        /// <summary>
        /// 产生单据编号
        /// </summary>
        /// <param name="billFormat"></param>
        /// <param name="en"></param>
        /// <returns></returns>
        public static string GenerBillNo(string billNo, Int64 workid, Entity en, string flowPTable)
        {
            if (string.IsNullOrEmpty(billNo))
                return "";
            /*如果，Bill 有规则 */
            billNo = billNo.Replace("{YYYY}", DateTime.Now.ToString("yyyy"));
            billNo = billNo.Replace("{yyyy}", DateTime.Now.ToString("yyyy"));

            billNo = billNo.Replace("{yy}", DateTime.Now.ToString("yy"));
            billNo = billNo.Replace("{YY}", DateTime.Now.ToString("YY"));

            billNo = billNo.Replace("{MM}", DateTime.Now.ToString("MM"));
            billNo = billNo.Replace("{DD}", DateTime.Now.ToString("DD"));
            billNo = billNo.Replace("{dd}", DateTime.Now.ToString("dd"));
            billNo = billNo.Replace("{HH}", DateTime.Now.ToString("HH"));
            billNo = billNo.Replace("{mm}", DateTime.Now.ToString("mm"));
            billNo = billNo.Replace("{LSH}", workid.ToString());
            billNo = billNo.Replace("{WorkID}", workid.ToString());
            billNo = billNo.Replace("{OID}", workid.ToString());

            if (billNo.Contains("@WebUser.DeptZi"))
            {
                string val = DBAccess.RunSQLReturnStringIsNull("SELECT Zi FROM Port_Dept where no='" + WebUser.FK_Dept + "'", "");
                billNo = billNo.Replace("@WebUser.DeptZi", val.ToString());
            }

            if (billNo.Contains("{ParentBillNo}"))
            {
                string pWorkID = DBAccess.RunSQLReturnStringIsNull("SELECT PWorkID FROM " + flowPTable + " WHERE OID=" + workid, "0");
                string parentBillNo = DBAccess.RunSQLReturnStringIsNull("SELECT BillNo FROM WF_GenerWorkFlow WHERE WorkID=" + pWorkID, "");
                billNo = billNo.Replace("{ParentBillNo}", parentBillNo);

                string sql = "";
                int num = 0;
                for (int i = 2; i < 7; i++)
                {
                    if (billNo.Contains("{LSH" + i + "}") == false)
                        continue;

                    sql = "SELECT COUNT(OID) FROM " + flowPTable + " WHERE PWorkID =" + pWorkID;
                    num = BP.DA.DBAccess.RunSQLReturnValInt(sql, 0);
                    billNo = billNo + num.ToString().PadLeft(i, '0');
                    billNo = billNo.Replace("{LSH" + i + "}", "");
                    break;
                }
            }
            else
            {
                string sql = "";
                int num = 0;
                for (int i = 2; i < 7; i++)
                {
                    if (billNo.Contains("{LSH" + i + "}") == false)
                        continue;

                    billNo = billNo.Replace("{LSH" + i + "}", "");
                    sql = "SELECT COUNT(*) FROM " + flowPTable + " WHERE BillNo LIKE '" + billNo + "%'";
                    num = BP.DA.DBAccess.RunSQLReturnValInt(sql, 0) + 1;
                    billNo = billNo + num.ToString().PadLeft(i, '0');
                }
            }
            return billNo;
        }
        /// <summary>
        /// 加入track
        /// </summary>
        /// <param name="at">事件类型</param>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="fid">流程ID</param>
        /// <param name="fromNodeID">从节点编号</param>
        /// <param name="fromNodeName">从节点名称</param>
        /// <param name="fromEmpID">从人员ID</param>
        /// <param name="fromEmpName">从人员名称</param>
        /// <param name="toNodeID">到节点编号</param>
        /// <param name="toNodeName">到节点名称</param>
        /// <param name="toEmpID">到人员ID</param>
        /// <param name="toEmpName">到人员名称</param>
        /// <param name="note">消息</param>
        /// <param name="tag">参数用@分开</param>
        public static void AddToTrack(ActionType at, string flowNo, Int64 workID, Int64 fid, int fromNodeID, string fromNodeName, string fromEmpID, string fromEmpName,
            int toNodeID, string toNodeName, string toEmpID, string toEmpName, string note, string tag)
        {
            if (toNodeID == 0)
            {
                toNodeID = fromNodeID;
                toNodeName = fromNodeName;
            }

            Track t = new Track();
            t.WorkID = workID;
            t.FID = fid;
            t.RDT = DataType.CurrentDataTimess;
            t.HisActionType = at;

            t.NDFrom = fromNodeID;
            t.NDFromT = fromNodeName;

            t.EmpFrom = fromEmpID;
            t.EmpFromT = fromEmpName;
            t.FK_Flow = flowNo;

            t.NDTo = toNodeID;
            t.NDToT = toNodeName;

            t.EmpTo = toEmpID;
            t.EmpToT = toEmpName;
            t.Msg = note;

            //参数.
            if (tag != null)
                t.Tag = tag;

            try
            {
                t.Insert();
            }
            catch
            {
                t.CheckPhysicsTable();
                t.Insert();
            }
        }
        /// <summary>
        /// 计算表达式是否通过(或者是否正确.)
        /// </summary>
        /// <param name="exp">表达式</param>
        /// <param name="en">实体</param>
        /// <returns>true/false</returns>
        public static bool ExeExp(string exp, Entity en)
        {
            exp = exp.Replace("@WebUser.No", WebUser.No);
            exp = exp.Replace("@WebUser.Name", WebUser.Name);
            exp = exp.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
            exp = exp.Replace("@WebUser.FK_DeptName", WebUser.FK_DeptName);


            string[] strs = exp.Split(' ');
            bool isPass = false;

            string key = strs[0].Trim();
            string oper = strs[1].Trim();
            string val = strs[2].Trim();
            val = val.Replace("'", "");
            val = val.Replace("%", "");
            val = val.Replace("~", "");
            BP.En.Row row = en.Row;
            foreach (string item in row.Keys)
            {
                if (key != item.Trim())
                    continue;

                string valPara = row[key].ToString();
                if (oper == "=")
                {
                    if (valPara == val)
                        return true;
                }

                if (oper.ToUpper() == "LIKE")
                {
                    if (valPara.Contains(val))
                        return true;
                }

                if (oper == ">")
                {
                    if (float.Parse(valPara) > float.Parse(val))
                        return true;
                }
                if (oper == ">=")
                {
                    if (float.Parse(valPara) >= float.Parse(val))
                        return true;
                }
                if (oper == "<")
                {
                    if (float.Parse(valPara) < float.Parse(val))
                        return true;
                }
                if (oper == "<=")
                {
                    if (float.Parse(valPara) <= float.Parse(val))
                        return true;
                }

                if (oper == "!=")
                {
                    if (float.Parse(valPara) != float.Parse(val))
                        return true;
                }

                throw new Exception("@参数格式错误:" + exp + " Key=" + key + " oper=" + oper + " Val=" + val);
            }

            return false;
        }
        /// <summary>
        /// 执行PageLoad装载数据
        /// </summary>
        /// <param name="item"></param>
        /// <param name="en"></param>
        /// <param name="mattrs"></param>
        /// <param name="dtls"></param>
        /// <returns></returns>
        public static Entity DealPageLoadFull(Entity en, MapExt item, MapAttrs mattrs, MapDtls dtls)
        {
            if (item == null)
                return en;

            DataTable dt = null;
            string sql = item.Tag;
            if (string.IsNullOrEmpty(sql) == false)
            {
                /* 如果有填充主表的sql  */
                sql = Glo.DealExp(sql, en, null);

                if (string.IsNullOrEmpty(sql) == false)
                {
                    if (sql.Contains("@"))
                        throw new Exception("设置的sql有错误可能有没有替换的变量:" + sql);
                    dt = DBAccess.RunSQLReturnTable(sql);
                    if (dt.Rows.Count == 1)
                    {
                        DataRow dr = dt.Rows[0];
                        foreach (DataColumn dc in dt.Columns)
                        {
                            if (string.IsNullOrEmpty(en.GetValStringByKey(dc.ColumnName)))
                                en.SetValByKey(dc.ColumnName, dr[dc.ColumnName].ToString());
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(item.Tag1)
                || item.Tag1.Length < 15)
                return en;

            // 填充从表.
            foreach (MapDtl dtl in dtls)
            {
                string[] sqls = item.Tag1.Split('*');
                foreach (string mysql in sqls)
                {
                    if (string.IsNullOrEmpty(mysql))
                        continue;

                    if (mysql.Contains(dtl.No + "=") == false)
                        continue;

                    #region 处理sql.
                    sql = mysql;
                    sql = Glo.DealExp(sql, en, null);
                    #endregion 处理sql.

                    if (string.IsNullOrEmpty(sql))
                        continue;

                    if (sql.Contains("@"))
                        throw new Exception("设置的sql有错误可能有没有替换的变量:" + sql);

                    GEDtls gedtls = null;

                    try
                    {
                        gedtls = new GEDtls(dtl.No);
                        gedtls.Delete(GEDtlAttr.RefPK, en.PKVal);
                    }
                    catch (Exception ex)
                    {
                        dtl.CheckPhysicsTable();
                        //gedtls = new GEDtls(dtl.No);
                    }

                    dt = DBAccess.RunSQLReturnTable(sql.TrimStart((dtl.No + "=").ToCharArray()));
                    foreach (DataRow dr in dt.Rows)
                    {
                        GEDtl gedtl = gedtls.GetNewEntity as GEDtl;
                        foreach (DataColumn dc in dt.Columns)
                        {
                            gedtl.SetValByKey(dc.ColumnName, dr[dc.ColumnName].ToString());
                        }

                        gedtl.RefPK = en.PKVal.ToString();
                        gedtl.RDT = DataType.CurrentDataTime;
                        gedtl.Rec = WebUser.No;
                        gedtl.Insert();
                    }
                }
            }
            return en;
        }
        /// <summary>
        /// 处理表达式
        /// </summary>
        /// <param name="exp">表达式</param>
        /// <param name="en">数据源</param>
        /// <param name="errInfo">错误</param>
        /// <returns></returns>
        public static string DealExp(string exp, Entity en, string errInfo)
        {
            exp = exp.Replace("~", "'");

            //首先替换加; 的。
            exp = exp.Replace("@WebUser.No;", WebUser.No);
            exp = exp.Replace("@WebUser.Name;", WebUser.Name);
            exp = exp.Replace("@WebUser.FK_Dept;", WebUser.FK_Dept);
            exp = exp.Replace("@WebUser.FK_DeptName;", WebUser.FK_DeptName);

            // 替换没有 ; 的 .
            exp = exp.Replace("@WebUser.No", WebUser.No);
            exp = exp.Replace("@WebUser.Name", WebUser.Name);
            exp = exp.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
            exp = exp.Replace("@WebUser.FK_DeptName", WebUser.FK_DeptName);

            if (exp.Contains("@") == false)
            {
                exp = exp.Replace("~", "'");
                return exp;
            }

            //增加对新规则的支持. @MyField; 格式.
            foreach (Attr item in en.EnMap.Attrs)
            {
                if (exp.Contains("@" + item.Key + ";"))
                    exp = exp.Replace("@" + item.Key + ";", en.GetValStrByKey(item.Key));
            }
            if (exp.Contains("@") == false)
                return exp;

            #region 解决排序问题.
            Attrs attrs = en.EnMap.Attrs;
            string mystrs = "";
            foreach (Attr attr in attrs)
            {
                if (attr.MyDataType == DataType.AppString)
                    mystrs += "@" + attr.Key + ",";
                else
                    mystrs += "@" + attr.Key;
            }
            string[] strs = mystrs.Split('@');
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("No", typeof(string)));
            foreach (string str in strs)
            {
                if (string.IsNullOrEmpty(str))
                    continue;

                DataRow dr = dt.NewRow();
                dr[0] = str;
                dt.Rows.Add(dr);
            }
            DataView dv = dt.DefaultView;
            dv.Sort = "No DESC";
            DataTable dtNew = dv.Table;
            #endregion  解决排序问题.

            #region 替换变量.
            foreach (DataRow dr in dtNew.Rows)
            {
                string key = dr[0].ToString();
                bool isStr = key.Contains(",");
                if (isStr == true)
                {
                    key = key.Replace(",", "");
                    exp = exp.Replace("@" + key, en.GetValStrByKey(key));
                }
                else
                {
                    exp = exp.Replace("@" + key, en.GetValStrByKey(key));
                }
            }

            // 处理Para的替换.
            if (exp.Contains("@") && Glo.SendHTOfTemp != null)
            {
                foreach (string key in Glo.SendHTOfTemp.Keys)
                    exp = exp.Replace("@" + key, Glo.SendHTOfTemp[key].ToString());
            }

            if (exp.Contains("@") && SystemConfig.IsBSsystem == true)
            {
                /*如果是bs*/
                foreach (string key in System.Web.HttpContext.Current.Request.QueryString.Keys)
                    exp = exp.Replace("@" + key, System.Web.HttpContext.Current.Request.QueryString[key]);
            }
            #endregion

            exp = exp.Replace("~", "'");
            //exp = exp.Replace("''", "'");
            //exp = exp.Replace("''", "'");
            //exp = exp.Replace("=' ", "=''");
            //exp = exp.Replace("= ' ", "=''");
            return exp;
        }
        /// <summary>
        /// 加密MD5
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string GenerMD5(BP.WF.Work wk)
        {
            string s = null;
            foreach (Attr attr in wk.EnMap.Attrs)
            {
                switch (attr.Key)
                {
                    case WorkAttr.MD5:
                    case WorkAttr.RDT:
                    case WorkAttr.CDT:
                    case WorkAttr.Rec:
                    case StartWorkAttr.Title:
                    case StartWorkAttr.Emps:
                    case StartWorkAttr.FK_Dept:
                    case StartWorkAttr.PRI:
                    case StartWorkAttr.FID:
                        continue;
                    default:
                        break;
                }

                string obj = attr.DefaultVal as string;
                //if (obj == null)
                //    continue;
                if (obj != null && obj.Contains("@"))
                    continue;

                s += wk.GetValStrByKey(attr.Key);
            }
            s += "ccflow";
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(s, "MD5").ToLower();
        }
        /// <summary>
        /// 装载流程数据 
        /// </summary>
        /// <param name="xlsFile"></param>
        public static string LoadFlowDataWithToSpecNode(string xlsFile)
        {
            DataTable dt = BP.DA.DBLoad.GetTableByExt(xlsFile);
            string err = "";
            string info = "";

            foreach (DataRow dr in dt.Rows)
            {
                string flowPK = dr["FlowPK"].ToString();
                string starter = dr["Starter"].ToString();
                string executer = dr["Executer"].ToString();
                int toNode = int.Parse(dr["ToNodeID"].ToString().Replace("ND", ""));
                Node nd = new Node();
                nd.NodeID = toNode;
                if (nd.RetrieveFromDBSources() == 0)
                {
                    err += "节点ID错误:" + toNode;
                    continue;
                }
                string sql = "SELECT count(*) as Num FROM ND" + int.Parse(nd.FK_Flow) + "01 WHERE FlowPK='" + flowPK + "'";
                int i = DBAccess.RunSQLReturnValInt(sql);
                if (i == 1)
                    continue; // 此数据已经调度了。

                #region 检查数据是否完整。
                BP.Port.Emp emp = new BP.Port.Emp();
                emp.No = executer;
                if (emp.RetrieveFromDBSources() == 0)
                {
                    err += "@账号:" + starter + ",不存在。";
                    continue;
                }
                if (string.IsNullOrEmpty(emp.FK_Dept))
                {
                    err += "@账号:" + starter + ",没有部门。";
                    continue;
                }

                emp.No = starter;
                if (emp.RetrieveFromDBSources() == 0)
                {
                    err += "@账号:" + executer + ",不存在。";
                    continue;
                }
                if (string.IsNullOrEmpty(emp.FK_Dept))
                {
                    err += "@账号:" + executer + ",没有部门。";
                    continue;
                }
                #endregion 检查数据是否完整。

                BP.Web.WebUser.SignInOfGener(emp);
                Flow fl = nd.HisFlow;
                Work wk = fl.NewWork();

                Attrs attrs = wk.EnMap.Attrs;
                //foreach (Attr attr in wk.EnMap.Attrs)
                //{
                //}

                foreach (DataColumn dc in dt.Columns)
                {
                    Attr attr = attrs.GetAttrByKey(dc.ColumnName.Trim());
                    if (attr == null)
                        continue;

                    string val = dr[dc.ColumnName].ToString().Trim();
                    switch (attr.MyDataType)
                    {
                        case DataType.AppString:
                        case DataType.AppDate:
                        case DataType.AppDateTime:
                            wk.SetValByKey(attr.Key, val);
                            break;
                        case DataType.AppInt:
                        case DataType.AppBoolean:
                            wk.SetValByKey(attr.Key, int.Parse(val));
                            break;
                        case DataType.AppMoney:
                        case DataType.AppDouble:
                        case DataType.AppRate:
                        case DataType.AppFloat:
                            wk.SetValByKey(attr.Key, decimal.Parse(val));
                            break;
                        default:
                            wk.SetValByKey(attr.Key, val);
                            break;
                    }
                }

                wk.SetValByKey(WorkAttr.Rec, BP.Web.WebUser.No);
                wk.SetValByKey(StartWorkAttr.FK_Dept, BP.Web.WebUser.FK_Dept);
                wk.SetValByKey("FK_NY", DataType.CurrentYearMonth);
                wk.SetValByKey(WorkAttr.MyNum, 1);
                wk.Update();

                Node ndStart = nd.HisFlow.HisStartNode;
                WorkNode wn = new WorkNode(wk, ndStart);
                try
                {
                    info += "<hr>" + wn.NodeSend(nd, executer).ToMsgOfHtml();
                }
                catch (Exception ex)
                {
                    err += "<hr>" + ex.Message;
                    WorkFlow wf = new WorkFlow(fl, wk.OID);
                    wf.DoDeleteWorkFlowByReal(true);
                    continue;
                }

                #region 更新 下一个节点数据。
                Work wkNext = nd.HisWork;
                wkNext.OID = wk.OID;
                wkNext.RetrieveFromDBSources();
                attrs = wkNext.EnMap.Attrs;
                foreach (DataColumn dc in dt.Columns)
                {
                    Attr attr = attrs.GetAttrByKey(dc.ColumnName.Trim());
                    if (attr == null)
                        continue;

                    string val = dr[dc.ColumnName].ToString().Trim();
                    switch (attr.MyDataType)
                    {
                        case DataType.AppString:
                        case DataType.AppDate:
                        case DataType.AppDateTime:
                            wkNext.SetValByKey(attr.Key, val);
                            break;
                        case DataType.AppInt:
                        case DataType.AppBoolean:
                            wkNext.SetValByKey(attr.Key, int.Parse(val));
                            break;
                        case DataType.AppMoney:
                        case DataType.AppDouble:
                        case DataType.AppRate:
                        case DataType.AppFloat:
                            wkNext.SetValByKey(attr.Key, decimal.Parse(val));
                            break;
                        default:
                            wkNext.SetValByKey(attr.Key, val);
                            break;
                    }
                }

                wkNext.DirectUpdate();

                GERpt rtp = fl.HisGERpt;
                rtp.SetValByKey("OID", wkNext.OID);
                rtp.RetrieveFromDBSources();
                rtp.Copy(wkNext);
                rtp.DirectUpdate();

                #endregion 更新 下一个节点数据。
            }
            return info + err;
        }
        public static string LoadFlowDataWithToSpecEndNode(string xlsFile)
        {
            DataTable dt = BP.DA.DBLoad.GetTableByExt(xlsFile);
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            ds.WriteXml("C:\\已完成.xml");

            string err = "";
            string info = "";
            int idx = 0;
            foreach (DataRow dr in dt.Rows)
            {
                string flowPK = dr["FlowPK"].ToString().Trim();
                if (string.IsNullOrEmpty(flowPK))
                    continue;

                string starter = dr["Starter"].ToString();
                string executer = dr["Executer"].ToString();
                int toNode = int.Parse(dr["ToNodeID"].ToString().Replace("ND", ""));
                Node ndOfEnd = new Node();
                ndOfEnd.NodeID = toNode;
                if (ndOfEnd.RetrieveFromDBSources() == 0)
                {
                    err += "节点ID错误:" + toNode;
                    continue;
                }

                if (ndOfEnd.IsEndNode == false)
                {
                    err += "节点ID错误:" + toNode + ", 非结束节点。";
                    continue;
                }

                string sql = "SELECT count(*) as Num FROM ND" + int.Parse(ndOfEnd.FK_Flow) + "01 WHERE FlowPK='" + flowPK + "'";
                int i = DBAccess.RunSQLReturnValInt(sql);
                if (i == 1)
                    continue; // 此数据已经调度了。

                #region 检查数据是否完整。
                //发起人发起。
                BP.Port.Emp emp = new BP.Port.Emp();
                emp.No = executer;
                if (emp.RetrieveFromDBSources() == 0)
                {
                    err += "@账号:" + starter + ",不存在。";
                    continue;
                }

                if (string.IsNullOrEmpty(emp.FK_Dept))
                {
                    err += "@账号:" + starter + ",没有设置部门。";
                    continue;
                }

                emp = new BP.Port.Emp();
                emp.No = starter;
                if (emp.RetrieveFromDBSources() == 0)
                {
                    err += "@账号:" + starter + ",不存在。";
                    continue;
                }
                else
                {
                    emp.RetrieveFromDBSources();
                    if (string.IsNullOrEmpty(emp.FK_Dept))
                    {
                        err += "@账号:" + starter + ",没有设置部门。";
                        continue;
                    }
                }
                #endregion 检查数据是否完整。


                BP.Web.WebUser.SignInOfGener(emp);
                Flow fl = ndOfEnd.HisFlow;
                Work wk = fl.NewWork();
                foreach (DataColumn dc in dt.Columns)
                    wk.SetValByKey(dc.ColumnName.Trim(), dr[dc.ColumnName].ToString().Trim());

                wk.SetValByKey(WorkAttr.Rec, BP.Web.WebUser.No);
                wk.SetValByKey(StartWorkAttr.FK_Dept, BP.Web.WebUser.FK_Dept);
                wk.SetValByKey("FK_NY", DataType.CurrentYearMonth);
                wk.SetValByKey(WorkAttr.MyNum, 1);
                wk.Update();

                Node ndStart = fl.HisStartNode;
                WorkNode wn = new WorkNode(wk, ndStart);
                try
                {
                    info += "<hr>" + wn.NodeSend(ndOfEnd, executer).ToMsgOfHtml();
                }
                catch (Exception ex)
                {
                    err += "<hr>启动错误:" + ex.Message;
                    DBAccess.RunSQL("DELETE FROM ND" + int.Parse(ndOfEnd.FK_Flow) + "01 WHERE FlowPK='" + flowPK + "'");
                    WorkFlow wf = new WorkFlow(fl, wk.OID);
                    wf.DoDeleteWorkFlowByReal(true);
                    continue;
                }

                //结束点结束。
                emp = new BP.Port.Emp(executer);
                BP.Web.WebUser.SignInOfGener(emp);

                Work wkEnd = ndOfEnd.GetWork(wk.OID);
                foreach (DataColumn dc in dt.Columns)
                    wkEnd.SetValByKey(dc.ColumnName.Trim(), dr[dc.ColumnName].ToString().Trim());

                wkEnd.SetValByKey(WorkAttr.Rec, BP.Web.WebUser.No);
                wkEnd.SetValByKey(StartWorkAttr.FK_Dept, BP.Web.WebUser.FK_Dept);
                wkEnd.SetValByKey("FK_NY", DataType.CurrentYearMonth);
                wkEnd.SetValByKey(WorkAttr.MyNum, 1);
                wkEnd.Update();

                try
                {
                    WorkNode wnEnd = new WorkNode(wkEnd, ndOfEnd);
                    //  wnEnd.AfterNodeSave();
                    info += "<hr>" + wnEnd.NodeSend().ToMsgOfHtml();
                }
                catch (Exception ex)
                {
                    err += "<hr>结束错误(系统直接删除它):" + ex.Message;
                    WorkFlow wf = new WorkFlow(fl, wk.OID);
                    wf.DoDeleteWorkFlowByReal(true);
                    continue;
                }
            }
            return info + err;
        }
        /// <summary>
        /// 判断是否登陆当前UserNo
        /// </summary>
        /// <param name="userNo"></param>
        public static void IsSingleUser(string userNo)
        {
            if (string.IsNullOrEmpty(WebUser.No) || WebUser.No != userNo)
            {
                if (!string.IsNullOrEmpty(userNo))
                {
                    BP.WF.Dev2Interface.Port_Login(userNo);
                }
            }
        }
        //public static void ResetFlowView()
        //{
        //    string sql = "DROP VIEW V_WF_Data ";
        //    try
        //    {
        //        BP.DA.DBAccess.RunSQL(sql);
        //    }
        //    catch
        //    {
        //    }

        //    Flows fls = new Flows();
        //    fls.RetrieveAll();
        //    sql = "CREATE VIEW V_WF_Data AS ";
        //    foreach (Flow fl in fls)
        //    {
        //        fl.CheckRpt();
        //        sql += "\t\n SELECT '" + fl.No + "' as FK_Flow, '" + fl.Name + "' AS FlowName, '" + fl.FK_FlowSort + "' as FK_FlowSort,CDT,Emps,FID,FK_Dept,FK_NY,";
        //        sql += "MyNum,OID,RDT,Rec,Title,WFState,FlowEmps,";
        //        sql += "FlowStarter,FlowStartRDT,FlowEnder,FlowEnderRDT,FlowDaySpan FROM ND" + int.Parse(fl.No) + "Rpt";
        //        sql += "\t\n  UNION";
        //    }
        //    sql = sql.Substring(0, sql.Length - 6);
        //    sql += "\t\n GO";
        //    BP.DA.DBAccess.RunSQL(sql);
        //}
        public static void Rtf2PDF(object pathOfRtf, object pathOfPDF)
        {
            //        Object Nothing = System.Reflection.Missing.Value;
            //        //创建一个名为WordApp的组件对象    
            //        Microsoft.Office.Interop.Word.Application wordApp =
            //new Microsoft.Office.Interop.Word.ApplicationClass();
            //        //创建一个名为WordDoc的文档对象并打开    
            //        Microsoft.Office.Interop.Word.Document doc = wordApp.Documents.Open(ref pathOfRtf, ref Nothing, ref Nothing, ref Nothing, ref Nothing,
            // ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing,
            //ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing);

            //        //设置保存的格式    
            //        object filefarmat = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatPDF;

            //        //保存为PDF    
            //        doc.SaveAs(ref pathOfPDF, ref filefarmat, ref Nothing, ref Nothing, ref Nothing, ref Nothing,
            //ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing,
            // ref Nothing, ref Nothing, ref Nothing);
            //        //关闭文档对象    
            //        doc.Close(ref Nothing, ref Nothing, ref Nothing);
            //        //推出组建    
            //        wordApp.Quit(ref Nothing, ref Nothing, ref Nothing);
            //        GC.Collect();
        }
        #endregion 常用方法

        #region 属性
        public static string SessionMsg
        {
            get
            {
                Paras p = new Paras();
                p.SQL = "SELECT Msg FROM WF_Emp where No=" + SystemConfig.AppCenterDBVarStr + "FK_Emp";
                p.AddFK_Emp();
                return DBAccess.RunSQLReturnString(p);

                //string SQL = "SELECT Msg FROM WF_Emp where No='"+BP.Web.WebUser.No+"'";
                //return DBAccess.RunSQLReturnString(SQL);
            }
            set
            {
                if (string.IsNullOrEmpty(value) == true)
                    return;

                Paras p = new Paras();
                p.SQL = "UPDATE WF_Emp SET Msg=" + SystemConfig.AppCenterDBVarStr + "v WHERE No=" + SystemConfig.AppCenterDBVarStr + "FK_Emp";
                p.AddFK_Emp();
                p.Add("v", value);
                DBAccess.RunSQL(p);

                //string SQL = "UPDATE WF_Emp SET Msg='" + value + "' WHERE No='" + BP.Web.WebUser.No + "'";
                //DBAccess.RunSQL(SQL);
            }
        }
        #endregion 属性

        #region 属性
        private static string _FromPageType = null;
        public static string FromPageType
        {
            get
            {
                _FromPageType = null;
                if (_FromPageType == null)
                {
                    try
                    {
                        string url = BP.Sys.Glo.Request.RawUrl;
                        int i = url.LastIndexOf("/") + 1;
                        int i2 = url.IndexOf(".aspx") - 6;

                        url = url.Substring(i);
                        url = url.Substring(0, url.IndexOf(".aspx"));
                        _FromPageType = url;
                        if (_FromPageType.Contains("SmallSingle"))
                            _FromPageType = "SmallSingle";
                        else if (_FromPageType.Contains("Small"))
                            _FromPageType = "Small";
                        else
                            _FromPageType = "";
                    }
                    catch (Exception ex)
                    {
                        _FromPageType = "";
                        //  throw new Exception(ex.Message + url + " i=" + i + " i2=" + i2);
                    }
                }
                return _FromPageType;
            }
        }
        /// <summary>
        /// 临时的发送传输变量.
        /// </summary>
        public static Hashtable SendHTOfTemp = null;
        /// <summary>
        /// 报表属性集合
        /// </summary>
        private static Attrs _AttrsOfRpt = null;
        /// <summary>
        /// 报表属性集合
        /// </summary>
        public static Attrs AttrsOfRpt
        {
            get
            {
                if (_AttrsOfRpt == null)
                {
                    _AttrsOfRpt = new Attrs();
                    _AttrsOfRpt.AddTBInt(GERptAttr.OID, 0, "WorkID", true, true);
                    _AttrsOfRpt.AddTBInt(GERptAttr.FID, 0, "FlowID", false, false);

                    _AttrsOfRpt.AddTBString(GERptAttr.Title, null, "标题", true, false, 0, 10, 10);
                    _AttrsOfRpt.AddTBString(GERptAttr.FlowStarter, null, "发起人", true, false, 0, 10, 10);
                    _AttrsOfRpt.AddTBString(GERptAttr.FlowStartRDT, null, "发起时间", true, false, 0, 10, 10);
                    _AttrsOfRpt.AddTBString(GERptAttr.WFState, null, "状态", true, false, 0, 10, 10);

                    //Attr attr = new Attr();
                    //attr.Desc = "流程状态";
                    //attr.Key = "WFState";
                    //attr.MyFieldType = FieldType.Enum;
                    //attr.UIBindKey = "WFState";
                    //attr.UITag = "@0=进行中@1=已经完成";

                    _AttrsOfRpt.AddDDLSysEnum(GERptAttr.WFState, 0, "流程状态", true, true, GERptAttr.WFState);
                    _AttrsOfRpt.AddTBString(GERptAttr.FlowEmps, null, "参与人", true, false, 0, 10, 10);
                    _AttrsOfRpt.AddTBString(GERptAttr.FlowEnder, null, "结束人", true, false, 0, 10, 10);
                    _AttrsOfRpt.AddTBString(GERptAttr.FlowEnderRDT, null, "结束时间", true, false, 0, 10, 10);
                    _AttrsOfRpt.AddTBDecimal(GERptAttr.FlowEndNode, 0, "结束节点", true, false);
                    _AttrsOfRpt.AddTBDecimal(GERptAttr.FlowDaySpan, 0, "跨度(天)", true, false);
                    //_AttrsOfRpt.AddTBString(GERptAttr.FK_NY, null, "隶属月份", true, false, 0, 10, 10);
                }
                return _AttrsOfRpt;
            }
        }
        #endregion 属性

        #region 其他配置.
        public static string GenerHelp(string helpId)
        {
            return "";
            switch (helpId)
            {
                case "Bill":
                    return "<a href=\"http://ccFlow.org\"  target=_blank><img src='../../WF/Img/FileType/rm.gif' border=0/>操作录像</a>";
                case "FAppSet":
                    return "<a href=\"http://ccFlow.org\"  target=_blank><img src='../../WF/Img/FileType/rm.gif' border=0/>操作录像</a>";
                default:
                    return "<a href=\"http://ccFlow.org\"  target=_blank><img src='../../WF/Img/FileType/rm.gif' border=0/>操作录像</a>";
                    break;
            }
        }
        public static string NodeImagePath
        {
            get
            {
                return Glo.IntallPath + "\\Data\\Node\\";
            }
        }
        public static void ClearDBData()
        {
            string sql = "DELETE FROM WF_GenerWorkFlow WHERE fk_flow not in (select no from wf_flow )";
            BP.DA.DBAccess.RunSQL(sql);

            sql = "DELETE FROM WF_GenerWorkerlist WHERE fk_flow not in (select no from wf_flow )";
            BP.DA.DBAccess.RunSQL(sql);
        }
        public static string OEM_Flag = "CCS";
        public static string FlowFileBill
        {
            get { return Glo.IntallPath + "\\DataUser\\Bill\\"; }
        }
        private static string _IntallPath = null;
        public static string IntallPath
        {
            get
            {
                if (_IntallPath == null)
                {
                    if (SystemConfig.IsBSsystem == true)
                        _IntallPath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;
                }

                if (_IntallPath == null)
                    throw new Exception("@没有实现如何获得 cs 下的根目录.");

                return _IntallPath;
            }
            set
            {
                _IntallPath = value;
            }
        }
        private static string _ServerIP = null;
        public static string ServerIP
        {
            get
            {
                if (_ServerIP == null)
                {
                    string ip = "127.0.0.1";
                    System.Net.IPAddress[] addressList = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;
                    if (addressList.Length > 1)
                        _ServerIP = addressList[1].ToString();
                    else
                        _ServerIP = addressList[0].ToString();
                }
                return _ServerIP;
            }
            set
            {
                _ServerIP = value;
            }
        }
        /// <summary>
        /// 流程控制器按钮
        /// </summary>
        public static string FlowCtrlBtnPos
        {
            get
            {
                string s = BP.Sys.SystemConfig.AppSettings["FlowCtrlBtnPos"] as string;
                if (s == null || s == "Top")
                    return "Top";
                return "Bottom";
            }
        }
        /// <summary>
        /// 全局的安全验证码
        /// </summary>
        public static string GloSID
        {
            get
            {
                string s = BP.Sys.SystemConfig.AppSettings["GloSID"] as string;
                if (s == null || s == "")
                    s = "sdfq2erre-2342-234sdf23423-323";
                return s;
            }
        }
        /// <summary>
        /// 是否启用检查用户的状态?
        /// 如果启用了:在MyFlow.aspx中每次都会检查当前的用户状态是否被禁
        /// 用，如果禁用了就不能执行任何操作了。启用后，就意味着每次都要
        /// 访问数据库。
        /// </summary>
        public static bool IsEnableCheckUseSta
        {
            get
            {
                string s = BP.Sys.SystemConfig.AppSettings["IsEnableCheckUseSta"] as string;
                if (s == null || s == "0")
                    return false;
                return true;
            }
        }
        /// <summary>
        /// 检查一下当前的用户是否仍旧有效使用？
        /// </summary>
        /// <returns></returns>
        public static bool CheckIsEnableWFEmp()
        {
            Paras ps = new Paras();
            ps.SQL = "SELECT UseSta FROM WF_Emp WHERE No=" + SystemConfig.AppCenterDBVarStr + "FK_Emp";
            ps.AddFK_Emp();
            string s = DBAccess.RunSQLReturnStringIsNull(ps, "1");
            if (s == "1" || s == null)
                return true;
            return false;
        }
        /// <summary>
        /// 语言
        /// </summary>
        public static string Language = "CH";
        public static bool IsQL
        {
            get
            {
                string s = BP.Sys.SystemConfig.AppSettings["IsQL"];
                if (s == null || s == "0")
                    return false;
                return true;
            }
        }
        /// <summary>
        /// 是否启用共享任务池？
        /// </summary>
        public static bool IsEnableTaskPool
        {
            get
            {
                return BP.Sys.SystemConfig.GetValByKeyBoolen("IsEnableTaskPool", false);
            }
        }
        /// <summary>
        /// 是否显示标题
        /// </summary>
        public static bool IsShowTitle
        {
            get
            {
                return BP.Sys.SystemConfig.GetValByKeyBoolen("IsShowTitle", false);
            }
        }
        /// <summary>
        /// 是否为工作增加一个优先级
        /// </summary>
        public static bool IsEnablePRI
        {
            get
            {
                return BP.Sys.SystemConfig.GetValByKeyBoolen("IsEnablePRI", false);
            }
        }
        /// <summary>
        /// 用户信息显示格式
        /// </summary>
        public static UserInfoShowModel UserInfoShowModel
        {
            get
            {
                return (UserInfoShowModel)BP.Sys.SystemConfig.GetValByKeyInt("UserInfoShowModel", 0);
            }
        }
        /// <summary>
        /// 产生用户数字签名
        /// </summary>
        /// <returns></returns>
        public static string GenerUserSigantureHtml(string serverPath, string userNo, string userName)
        {
            string siganturePath = serverPath + "/" + CCFlowAppPath + "DataUser/Siganture/" + userNo + ".jpg";
            if (System.IO.File.Exists(siganturePath))
            {
                return "<img src='" + CCFlowAppPath + "DataUser/Siganture/" + userNo + ".jpg' width='90' height='30' title='" + userName + "' border=0 onerror=\"src='" + CCFlowAppPath + "DataUser/UserICON/DefaultSmaller.png'\" />";
            }
            return "<img src='" + CCFlowAppPath + "DataUser/UserICON/" + userNo + "Smaller.png' border=0 width='24px' onerror=\"src='" + CCFlowAppPath + "DataUser/UserICON/DefaultSmaller.png'\" />" + userName;
        }
        /// <summary>
        /// 产生用户小图片
        /// </summary>
        /// <returns></returns>
        public static string GenerUserImgSmallerHtml(string userNo, string userName)
        {
            return "<img src='" + CCFlowAppPath + "DataUser/UserICON/" + userNo + "Smaller.png' border=0 width='24px' onerror=\"src='" + CCFlowAppPath + "DataUser/UserICON/DefaultSmaller.png'\" />" + userName;
        }
        /// <summary>
        /// 更新主表的SQL
        /// </summary>
        public static string UpdataMainDeptSQL
        {
            get
            {
                return BP.Sys.SystemConfig.GetValByKey("UpdataMainDeptSQL", "UPDATE Port_Emp SET FK_Dept=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "FK_Dept WHERE No=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "No");
            }
        }
        /// <summary>
        /// 更新SID的SQL
        /// </summary>
        public static string UpdataSID
        {
            get
            {
                return BP.Sys.SystemConfig.GetValByKey("UpdataSID", "UPDATE Port_Emp SET SID=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "SID WHERE No=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "No");
            }
        }
        /// <summary>
        /// 下载sl的地址
        /// </summary>
        public static string SilverlightDownloadUrl
        {
            get
            {
                return BP.Sys.SystemConfig.GetValByKey("SilverlightDownloadUrl", "http://go.microsoft.com/fwlink/?LinkID=124807");
            }
        }
        /// <summary>
        /// 处理显示格式
        /// </summary>
        /// <param name="no"></param>
        /// <param name="name"></param>
        /// <returns>现实格式</returns>
        public static string DealUserInfoShowModel(string no, string name)
        {
            switch (BP.WF.Glo.UserInfoShowModel)
            {
                case UserInfoShowModel.UserIDOnly:
                    return "(" + no + ")";
                case UserInfoShowModel.UserIDUserName:
                    return "(" + no + "," + name + ")";
                case UserInfoShowModel.UserNameOnly:
                    return "(" + name + ")";
                default:
                    throw new Exception("@没有判断的格式类型.");
                    break;
            }
        }
        /// <summary>
        /// 运行模式
        /// </summary>
        public static OSModel OSModel
        {
            get
            {
                OSModel os = (OSModel)BP.Sys.SystemConfig.GetValByKeyInt("OSModel", 0);
                return os;
            }
        }
        /// <summary>
        /// 是否是集团使用
        /// </summary>
        public static bool IsUnit
        {
            get
            {
                return BP.Sys.SystemConfig.GetValByKeyBoolen("IsUnit", false);
            }
        }
        /// <summary>
        /// 是否启用制度
        /// </summary>
        public static bool IsEnableZhiDu
        {
            get
            {
                return BP.Sys.SystemConfig.GetValByKeyBoolen("IsEnableZhiDu", false);
            }
        }
        /// <summary>
        /// 是否启用草稿
        /// </summary>
        public static bool IsEnableDraft
        {
            get
            {
                return BP.Sys.SystemConfig.GetValByKeyBoolen("IsEnableDraft", false);
            }
        }
        /// <summary>
        /// 是否删除流程注册表数据？
        /// </summary>
        public static bool IsDeleteGenerWorkFlow
        {
            get
            {
                return BP.Sys.SystemConfig.GetValByKeyBoolen("IsDeleteGenerWorkFlow", false);
            }
        }
        /// <summary>
        /// 是否检查表单树字段填写是否为空
        /// </summary>
        public static bool IsEnableCheckFrmTreeIsNull
        {
            get
            {
                return BP.Sys.SystemConfig.GetValByKeyBoolen("IsEnableCheckFrmTreeIsNull", true);
            }
        }

        /// <summary>
        /// 是否启动工作时打开新窗口
        /// </summary>
        public static int IsWinOpenStartWork
        {
            get
            {
                return BP.Sys.SystemConfig.GetValByKeyInt("IsWinOpenStartWork", 1);
            }
        }
        /// <summary>
        /// 是否打开待办工作时打开窗口
        /// </summary>
        public static bool IsWinOpenEmpWorks
        {
            get
            {
                return BP.Sys.SystemConfig.GetValByKeyBoolen("IsWinOpenEmpWorks", true);
            }
        }
        /// <summary>
        /// 是否启用消息系统消息。
        /// </summary>
        public static bool IsEnableSysMessage
        {
            get
            {
                return BP.Sys.SystemConfig.GetValByKeyBoolen("IsEnableSysMessage", true);
            }
        }
        /// <summary>
        /// 与ccflow流程服务相关的配置: 执行自动任务节点，间隔的时间，以分钟计算，默认为2分钟。
        /// </summary>
        public static int AutoNodeDTSTimeSpanMinutes
        {
            get
            {
                return BP.Sys.SystemConfig.GetValByKeyInt("AutoNodeDTSTimeSpanMinutes", 2);
            }
        }
        /// <summary>
        /// ccim集成的数据库.
        /// 是为了向ccim写入消息.
        /// </summary>
        public static string CCIMDBName
        {
            get
            {
                string baseUrl = BP.Sys.SystemConfig.AppSettings["CCIMDBName"];
                if (string.IsNullOrEmpty(baseUrl) == true)
                    baseUrl = "ccPort.dbo";
                return baseUrl;
            }
        }
        /// <summary>
        /// 主机
        /// </summary>
        public static string HostURL
        {
            get
            {
                string baseUrl = BP.Sys.SystemConfig.AppSettings["HostURL"];
                if (string.IsNullOrEmpty(baseUrl) == true)
                    baseUrl = BP.Sys.SystemConfig.AppSettings["BaseURL"];

                if (string.IsNullOrEmpty(baseUrl) == true)
                    baseUrl = "http://127.0.0.1/";

                if (baseUrl.Substring(baseUrl.Length - 1) != "/")
                    baseUrl = baseUrl + "/";
                return baseUrl;
            }
        }
        public static string CurrPageID
        {
            get
            {
                try
                {
                    string url = BP.Sys.Glo.Request.RawUrl;

                    int i = url.LastIndexOf("/") + 1;
                    int i2 = url.IndexOf(".aspx") - 6;
                    try
                    {
                        url = url.Substring(i);
                        return url.Substring(0, url.IndexOf(".aspx"));

                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message + url + " i=" + i + " i2=" + i2);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("获取当前PageID错误:" + ex.Message);
                }
            }
        }


        //用户表单风格控制
        public static string GetUserStyle
        {
            get
            {
                //BP.WF.Port.WFEmp emp = new Port.WFEmp(WebUser.No);
                //if(string.IsNullOrEmpty(emp.Style) || emp.Style=="0")
                //{
                string userStyle = BP.Sys.SystemConfig.AppSettings["UserStyle"];
                if (string.IsNullOrEmpty(userStyle))
                    return "ccflow默认";
                else
                    return userStyle;
                //}
                //else
                //    return emp.Style;
            }

        }
        #endregion 其他配置.

        #region 其他方法。
        /// <summary>
        /// 转到消息显示界面.
        /// </summary>
        /// <param name="info"></param>
        public static void ToMsg(string info)
        {
            //string rowUrl = BP.Sys.Glo.Request.RawUrl;
            //if (rowUrl.Contains("&IsClient=1"))
            //{
            //    /*说明这是vsto调用的.*/
            //    return;
            //}

            System.Web.HttpContext.Current.Session["info"] = info;
            System.Web.HttpContext.Current.Response.Redirect(Glo.CCFlowAppPath + "WF/MyFlowInfo.aspx?Msg=" + DataType.CurrentDataTimess, false);
        }
        public static void ToMsgErr(string info)
        {
            info = "<font color=red>" + info + "</font>";
            System.Web.HttpContext.Current.Session["info"] = info;
            System.Web.HttpContext.Current.Response.Redirect(Glo.CCFlowAppPath + "WF/MyFlowInfo.aspx?Msg=" + DataType.CurrentDataTimess, false);
        }
        /// <summary>
        /// 检查流程发起限制
        /// </summary>
        /// <param name="flow">流程</param>
        /// <param name="wk">开始节点工作</param>
        /// <returns></returns>
        public static bool CheckIsCanStartFlow_InitStartFlow(Flow flow, Work wk)
        {
            StartLimitRole role = flow.StartLimitRole;
            if (role == StartLimitRole.None)
                return true;

            string sql = "";
            string ptable = flow.PTable;

            #region 按照时间的必须是，在表单加载后判断, 不管用户设置是否正确.
            DateTime dtNow = DateTime.Now;
            if (role == StartLimitRole.Day)
            {
                /* 仅允许一天发起一次 */
                sql = "SELECT COUNT(*) as Num FROM " + ptable + " WHERE RDT LIKE '" + DataType.CurrentData + "%' AND WFState NOT IN(0,1) AND FlowStarter='" + WebUser.No + "'";
                if (DBAccess.RunSQLReturnValInt(sql, 0) == 0)
                {
                    if (flow.StartLimitPara == "")
                        return true;

                    //判断时间是否在设置的发起范围内. 配置的格式为 @11:00-12:00@15:00-13:45
                    string[] strs = flow.StartLimitPara.Split('@');
                    foreach (string str in strs)
                    {
                        if (string.IsNullOrEmpty(str))
                            continue;
                        string[] timeStrs = str.Split('-');
                        string tFrom = DateTime.Now.ToString("yyyy-MM-dd") + " " + timeStrs[0].Trim();
                        string tTo = DateTime.Now.ToString("yyyy-MM-dd") + " " + timeStrs[1].Trim();
                        if (DataType.ParseSysDateTime2DateTime(tFrom) <= dtNow && dtNow >= DataType.ParseSysDateTime2DateTime(tTo))
                            return true;
                    }
                    return false;
                }
                else
                    return false;
            }

            if (role == StartLimitRole.Week)
            {
                /*
                 * 1, 找出周1 与周日分别是第几日.
                 * 2, 按照这个范围去查询,如果查询到结果，就说明已经启动了。
                 */
                sql = "SELECT COUNT(*) as Num FROM " + ptable + " WHERE RDT >= '" + DataType.WeekOfMonday(dtNow) + "' AND WFState NOT IN(0,1) AND FlowStarter='" + WebUser.No + "'";
                if (DBAccess.RunSQLReturnValInt(sql, 0) == 0)
                {
                    if (flow.StartLimitPara == "")
                        return true; /*如果没有时间的限制.*/

                    //判断时间是否在设置的发起范围内. 
                    // 配置的格式为 @Sunday,11:00-12:00@Monday,15:00-13:45, 意思是.周日，周一的指定的时间点范围内可以启动流程.

                    string[] strs = flow.StartLimitPara.Split('@');
                    foreach (string str in strs)
                    {
                        if (string.IsNullOrEmpty(str))
                            continue;

                        string weekStr = DateTime.Now.DayOfWeek.ToString().ToLower();
                        if (str.ToLower().Contains(weekStr) == false)
                            continue; // 判断是否当前的周.

                        string[] timeStrs = str.Split(',');
                        string tFrom = DateTime.Now.ToString("yyyy-MM-dd") + " " + timeStrs[0].Trim();
                        string tTo = DateTime.Now.ToString("yyyy-MM-dd") + " " + timeStrs[1].Trim();
                        if (DataType.ParseSysDateTime2DateTime(tFrom) <= dtNow && dtNow >= DataType.ParseSysDateTime2DateTime(tTo))
                            return true;
                    }
                    return false;
                }
                else
                    return false;
            }

            // #warning 没有考虑到周的如何处理.

            if (role == StartLimitRole.Month)
            {
                sql = "SELECT COUNT(*) as Num FROM " + ptable + " WHERE FK_NY = '" + DataType.CurrentYearMonth + "' AND WFState NOT IN(0,1) AND FlowStarter='" + WebUser.No + "'";
                if (DBAccess.RunSQLReturnValInt(sql, 0) == 0)
                {
                    if (flow.StartLimitPara == "")
                        return true;

                    //判断时间是否在设置的发起范围内. 配置格式: @-01 12:00-13:11@-15 12:00-13:11 , 意思是：在每月的1号,15号 12:00-13:11可以启动流程.
                    string[] strs = flow.StartLimitPara.Split('@');
                    foreach (string str in strs)
                    {
                        if (string.IsNullOrEmpty(str))
                            continue;
                        string[] timeStrs = str.Split('-');
                        string tFrom = DateTime.Now.ToString("yyyy-MM-") + " " + timeStrs[0].Trim();
                        string tTo = DateTime.Now.ToString("yyyy-MM-") + " " + timeStrs[1].Trim();
                        if (DataType.ParseSysDateTime2DateTime(tFrom) <= dtNow && dtNow >= DataType.ParseSysDateTime2DateTime(tTo))
                            return true;
                    }
                    return false;
                }
                else
                    return false;
            }

            if (role == StartLimitRole.JD)
            {
                sql = "SELECT COUNT(*) as Num FROM " + ptable + " WHERE FK_NY = '" + DataType.CurrentAPOfJD + "' AND WFState NOT IN(0,1) AND FlowStarter='" + WebUser.No + "'";
                if (DBAccess.RunSQLReturnValInt(sql, 0) == 0)
                {
                    if (flow.StartLimitPara == "")
                        return true;

                    //判断时间是否在设置的发起范围内.
                    string[] strs = flow.StartLimitPara.Split('@');
                    foreach (string str in strs)
                    {
                        if (string.IsNullOrEmpty(str))
                            continue;
                        string[] timeStrs = str.Split('-');
                        string tFrom = DateTime.Now.ToString("yyyy-") + " " + timeStrs[0].Trim();
                        string tTo = DateTime.Now.ToString("yyyy-") + " " + timeStrs[1].Trim();
                        if (DataType.ParseSysDateTime2DateTime(tFrom) <= dtNow && dtNow >= DataType.ParseSysDateTime2DateTime(tTo))
                            return true;
                    }
                    return false;
                }
                else
                    return false;
            }

            if (role == StartLimitRole.Year)
            {
                sql = "SELECT COUNT(*) as Num FROM " + ptable + " WHERE FK_NY LIKE '" + DataType.CurrentYear + "%' AND WFState NOT IN(0,1) AND FlowStarter='" + WebUser.No + "'";
                if (DBAccess.RunSQLReturnValInt(sql, 0) == 0)
                {
                    if (flow.StartLimitPara == "")
                        return true;

                    //判断时间是否在设置的发起范围内.
                    string[] strs = flow.StartLimitPara.Split('@');
                    foreach (string str in strs)
                    {
                        if (string.IsNullOrEmpty(str))
                            continue;
                        string[] timeStrs = str.Split('-');
                        string tFrom = DateTime.Now.ToString("yyyy-") + " " + timeStrs[0].Trim();
                        string tTo = DateTime.Now.ToString("yyyy-") + " " + timeStrs[1].Trim();
                        if (DataType.ParseSysDateTime2DateTime(tFrom) <= dtNow && dtNow >= DataType.ParseSysDateTime2DateTime(tTo))
                            return true;
                    }
                    return false;
                }
                else
                    return false;
            }
            #endregion 按照时间的必须是，在表单加载后判断, 不管用户设置是否正确.

            return true;
        }
        /// <summary>
        /// 当要发送是检查流程是否可以允许发起.
        /// </summary>
        /// <param name="flow">流程</param>
        /// <param name="wk">开始节点工作</param>
        /// <returns></returns>
        public static bool CheckIsCanStartFlow_SendStartFlow(Flow flow, Work wk)
        {
            StartLimitRole role = flow.StartLimitRole;
            if (role == StartLimitRole.None)
                return true;

            string sql = "";
            string ptable = flow.PTable;

            if (role == StartLimitRole.ColNotExit)
            {
                /* 指定的列名集合不存在，才可以发起流程。*/

                //求出原来的值.
                string[] strs = flow.StartLimitPara.Split(',');
                string val = "";
                foreach (string str in strs)
                {
                    if (string.IsNullOrEmpty(str) == true)
                        continue;
                    try
                    {
                        val += wk.GetValStringByKey(str);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("@流程设计错误,您配置的检查参数(" + flow.StartLimitPara + "),中的列(" + str + ")已经不存在表单里.");
                    }
                }

                //找出已经发起的全部流程.
                sql = "SELECT " + flow.StartLimitPara + " FROM " + ptable + " WHERE  WFState NOT IN(0,1) AND FlowStarter='" + WebUser.No + "'";
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    string v = dr[0] + "" + dr[1] + "" + dr[2];
                    if (v == val)
                        return false;
                }
                return true;
            }

            // 配置的sql,执行后,返回结果是 0 .
            if (role == StartLimitRole.ResultIsZero)
            {
                sql = BP.WF.Glo.DealExp(flow.StartLimitPara, wk, null);
                if (DBAccess.RunSQLReturnValInt(sql, 0) == 0)
                    return true;
                else
                    return false;
            }

            // 配置的sql,执行后,返回结果是 <> 0 .
            if (role == StartLimitRole.ResultIsNotZero)
            {
                sql = BP.WF.Glo.DealExp(flow.StartLimitPara, wk, null);
                if (DBAccess.RunSQLReturnValInt(sql, 0) != 0)
                    return true;
                else
                    return false;
            }
            return true;
        }
        #endregion 其他方法。
    }
}
