using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Browser;
using System.IO;
using Silverlight;
using BP;
using OrganizationalStructure.OS;

namespace BP
{
    /// <summary>
    /// Glo
    /// </summary>
    public class Glo
    {
        /// <summary>
        /// 当前部门编号
        /// </summary>
        public static string FK_Dept = null;
        /// <summary>
        /// 当前部门名称
        /// </summary>
        public static string Dept_Name = null;
        /// <summary>
        /// CurrTreeViewNode
        /// </summary>
        public static TreeViewItem CurrTreeViewNode = null;
        /// <summary>
        /// 是否是数字
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static bool IsNum(string exp)
        {
            try
            {
                Int64 i= Int64.Parse(exp);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region 与控件有关的操作方法
        public static bool Ctrl_DDL_SetSelectVal(ComboBox ddl, string setVal)
        {
            string oldVal = "";
            foreach (ListBoxItem item in ddl.Items)
            {
                if (item.IsEnabled == true)
                {
                    oldVal = item.Tag.ToString();
                    item.IsSelected = false;
                    break;
                }
            }
            foreach (ListBoxItem item in ddl.Items)
            {
                if (item.Tag.ToString() == setVal)
                {
                    item.IsSelected = true;
                    return true;
                }
            }

            foreach (ListBoxItem item in ddl.Items)
            {
                if (item.Tag.ToString() == oldVal)
                {
                    item.IsSelected = true;
                    break;
                }
            }
            return false;
        }
        public static void Ctrl_DDL_BindDataTable(ComboBox ddl, DataTable dt, string selectVal)
        {
            ddl.Items.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                string no = dr[0].ToString();
                string name = dr[1].ToString();

                ComboBoxItem cbi = new ComboBoxItem();
                cbi.Content = name;
                cbi.Tag = no;
                if (no == selectVal)
                    cbi.IsSelected = true;
                else
                    cbi.IsSelected = false;
                ddl.Items.Add(cbi);
            }

            if (ddl.Items.Count == 0)
                return;

            if (ddl.SelectedIndex == -1)
                ddl.SelectedIndex = 0;
        }
        public static int GetDDLValOfInt(ComboBox cb)
        {
            ComboBoxItem it = cb.SelectedItem as ComboBoxItem;
            if (it == null)
                throw new Exception("没有选择数据" + cb.Name);
            return int.Parse(it.Tag.ToString());
        }
        public static string GetDDLValOfString(ComboBox cb)
        {
            ComboBoxItem it = cb.SelectedItem as ComboBoxItem;
            if (it == null)
                throw new Exception("没有选择数据" + cb.Name);
            return it.Tag.ToString();
        }
        #endregion
            
        #region 属性
        /// <summary>
        /// 临时变量.
        /// </summary>
        public static string TempVar = null;
        /// <summary>
        /// 当前BPMHost 
        /// </summary>
        private static string _BPMHost = null;
        /// <summary>
        /// 当前BPMHost 
        /// 比如:http://demo.ccflow.org:8888
        /// </summary>
        public static string BPMHost
        {
            get
            {
                if (_BPMHost != null)
                    return _BPMHost;

                string myurl = System.Windows.Browser.HtmlPage.Document.DocumentUri.AbsoluteUri;

                myurl = myurl.Replace("//", "");
                int posStart = myurl.IndexOf("/");

                string appPath = myurl.Substring(posStart);
                if (appPath.Contains("/GPM"))
                {
                    appPath = appPath.Substring(0, appPath.IndexOf("/GPM/", StringComparison.CurrentCultureIgnoreCase));
                }

                var location = (HtmlPage.Window.GetProperty("location")) as ScriptObject;
                _BPMHost = "http://" + location.GetProperty("host") + appPath;
                return _BPMHost;
            }
        }
        /// <summary>
        /// 当前的流程编号
        /// </summary>
        public static string FK_Flow = null;
        #endregion

        #region 共用方法
        /// <summary>
        /// 设置打开网页窗口的属性
        /// </summary>
        /// <param name="lang">语言</param>
        /// <param name="dotype">窗口类型</param>
        /// <param name="fk_flow">工作流ID</param>
        /// <param name="node1">结点1</param>
        /// <param name="node2">结点2</param>
        public static void WinOpenByDoType(string lang, string dotype, string fk_flow, string node1, string node2)
        {
            Glo.DoTypeNow = dotype;
            string url = "";
            switch (Glo.DoTypeNow)
            {
                //case UrlFlag.StaDef:
                //    url = "/WF/Admin/XAP/DoPort.aspx?DoType=StaDef&PK=" + node1 + "&Lang=CH";
                //    Glo.OpenDialog(Glo.BPMHost + url, "执行", 500, 400);
                //    return;
                //case UrlFlag.FrmLib:
                //    url = "/WF/Admin/XAP/DoPort.aspx?DoType=FrmLib&FK_Flow=" + fk_flow + "&FK_Node=0&Lang=CH";
                //    Glo.WinOpen(Glo.BPMHost + url, "执行", 800, 760);
                //    return;
                //case UrlFlag.FlowFrms:
                //    url = "/WF/Admin/XAP/DoPort.aspx?DoType=FlowFrms&FK_Flow=" + fk_flow + "&FK_Node="+node1+"&Lang=CH";
                //    Glo.WinOpen(Glo.BPMHost + url, "执行", 800, 760);
                //    return;
                //case UrlFlag.NodeP:
                //    url = "/WF/Admin/XAP/DoPort.aspx?DoType=En&EnName=BP.WF.Node&PK=" + node1 + "&Lang=CH";
                //    Glo.OpenDialog(Glo.BPMHost + url, "执行", 600, 500);
                //    return;
                //case UrlFlag.FlowP: // 节点属性与流程属性。
                //    url = "/WF/Admin/XAP/DoPort.aspx?DoType=En&EnName=BP.WF.Flow&PK=" + fk_flow + "&Lang=CH";
                //    Glo.OpenDialog(Glo.BPMHost + url, "执行", 500, 400);
                //    return;
                 
                //case UrlFlag.Dir: // 方向条件。
                //    url = "/WF/Admin/Cond.aspx?FK_Flow=" + fk_flow + "&FK_MainNode=" + node1 + "&FK_Node=" + node1 + "&ToNodeID=" + node2 + "&CondType=2&Lang=CH";
                //    Glo.OpenDialog(Glo.BPMHost + url, "方向条件", 550, 500);
                //    return;
                //case "RunFlow": // 运行流程。
                //    url = "/WF/Admin/TestFlow.aspx?FK_Flow=" + fk_flow + "&Lang=CH";
                //    Glo.WinOpen(Glo.BPMHost + url, "运行流程", 850, 990);
                //    return;
                //case "FlowCheck": // 流程设计。
                //    url = "/WF/Admin/DoType.aspx?RefNo=" + fk_flow + "&DoType=" + dotype + "&Lang=CH";
                //    Glo.WinOpen(Glo.BPMHost + url, "运行流程", 850, 990);
                //    return;
                //case "LoginPage": // 流程设计。
                //    url = @"/WF/Login.aspx?Lang=CH";
                //    Glo.WinOpen(Glo.BPMHost + url, "运行流程", 850, 990);
                //    return;
                //case "WFRpt": // 流程设计。
                //    url = "/WF/Admin/XAP/DoPort.aspx?RefNo=" + fk_flow + "&DoType=" + dotype + "&Lang=CH&PK="+fk_flow;
                //    Glo.WinOpen(Glo.BPMHost + url, "运行流程", 850, 990);
                //    return;
                default:
                    MessageBox.Show("没有判断的url执行标记:" + dotype);
                    return;
            }

            Glo.WinOpen(Glo.BPMHost + "/WF/Admin/XAP/DoType=" + dotype + "&FK_Flow=" + fk_flow + "&FK_Node1=" + node1 + "&Lang=CH", "节点表单设计", 850, 990);
            return;
        }
        private static string DoTypeNow = "";
        public static void OpenDialog(string url, string title, int h, int w)
        {
            OpenWindowOrDialog(url, title, string.Format("dialogHeight:{0}px;dialogWidth:{1}px", h, w), WindowModelEnum.Dialog);
        }
        public static void WinOpen(string url, string title, int h, int w)
        {
            OpenWindowOrDialog(url, title, string.Format("height={0},width={1}", h, w), WindowModelEnum.Window);
        }
        public static void OpenDialog(string url, string title)
        {
            OpenWindowOrDialog(url, title, "dialogHeight:600px;dialogWidth:800px", WindowModelEnum.Dialog);
        }
        /// <summary>
        /// 得到WebService对象
        /// </summary>
        /// <returns></returns>
        public static OSSoapClient GetOSServiceInstance()
        {
            var basicBinding = new BasicHttpBinding()
            {
                MaxBufferSize = 2147483647,
                MaxReceivedMessageSize = 2147483647,
                Name = "WSOSSoap"
            };
            basicBinding.Security.Mode = BasicHttpSecurityMode.None;
            string url = Glo.BPMHost + "/GPM/OrganizationalStructure.asmx";
            url = url.Replace("//", "/");
            url = url.Replace(":/", "://");

            var endPoint = new EndpointAddress(url);
            var ctor =
                typeof(OSSoapClient).GetConstructor(new Type[] { typeof(Binding), typeof(EndpointAddress) });
            return (OSSoapClient)ctor.Invoke(new object[] { basicBinding, endPoint });
        }
        #endregion 共用方法

        /// <summary>
        /// 弹出网页窗口
        /// </summary>
        /// <param name="url">网页地址</param>
        private static void OpenWindowOrDialog(string url, string title, string property, WindowModelEnum windowModel)
        {
            if (url.Contains("ttp://") == false)
                url = Glo.BPMHost + url;

            if (WindowModelEnum.Dialog == windowModel)
            {
                HtmlPage.Window.Eval(
                    string.Format("window.showModalDialog('{0}',window,'dialogHeight:600px;dialogWidth:950px;help:no;scroll:auto;resizable:yes;status:no;');",
                        url));
            }
            else
            {
                HtmlPage.Window.Eval("window.open('" + url + "','_blank')");
            }
        }
    }
}
