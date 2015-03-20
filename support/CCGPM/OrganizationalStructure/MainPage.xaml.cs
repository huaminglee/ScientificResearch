using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using Silverlight;
using BP;
using OrganizationalStructure.OS;

namespace OrganizationalStructure
{
    public partial class MainPage : UserControl
    {
        #region 定义的变量
        /// <summary>
        /// 实体
        /// </summary>
        public class PeopleNode
        {
            /// <summary>
            /// 编号
            /// </summary>
            public string No { get; set; }
            /// <summary>
            /// 名称
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 员工编号
            /// </summary>
            public string EmpNo { get; set; }
            /// <summary>
            /// 电话
            /// </summary>
            public string Tel { get; set; }
            /// <summary>
            /// 邮件
            /// </summary>
            public string Email { get; set; }
            /// <summary>
            /// 职务编号
            /// </summary>
            public string FK_Duty { get; set; }
            /// <summary>
            /// 部门名称
            /// </summary>
            public string FK_DeptText { get; set; }
            /// <summary>
            /// 职务名称
            /// </summary>
            public string FK_DutyText { get; set; }
            /// <summary>
            /// 职务级别
            /// </summary>
            public int DutyLevel { get; set; }
            /// <summary>
            /// 直属领导
            /// </summary>
            public string Leader { get; set; }
            /// <summary>
            /// 人员岗位
            /// </summary>
            public string Stations { get; set; }
            /// <summary>
            /// 部门
            /// </summary>
            public string FK_Dept { get; set; }
        }
        List<PeopleNode> pList = new List<PeopleNode>();
        PeopleNode DataGridSelectNode = new PeopleNode();  //左键选择的内容
        #endregion

        public MainPage()
        {
            InitializeComponent();
            //检查根节点
            OSSoapClient os = Glo.GetOSServiceInstance();
            os.Dept_CheckRootNodeAsync();
            os.Dept_CheckRootNodeCompleted += new EventHandler<Dept_CheckRootNodeCompletedEventArgs>(os_Dept_CheckRootNodeCompleted);
        }
        void os_Dept_CheckRootNodeCompleted(object sender, Dept_CheckRootNodeCompletedEventArgs e)
        {
            this.BingTree();    
        }

        public void BingTree()
        {
            //绑定部门            
            string sqls = "SELECT * FROM Port_Dept ORDER BY Idx";
            OSSoapClient os = Glo.GetOSServiceInstance();
            os.RunSQLReturnTableSAsync(sqls);
            os.RunSQLReturnTableSCompleted += new EventHandler<RunSQLReturnTableSCompletedEventArgs>(BingTree);
        }

        void BingTree(object sender, RunSQLReturnTableSCompletedEventArgs e)
        {
            DataSet ds = new DataSet();
            ds.FromXml(e.Result);
            DataTable dtDept = ds.Tables[0]; //部门信息。
            this.treeView1.Items.Clear(); // 清楚所有的数据.

            TreeViewItem rootNode = new TreeViewItem();
            //清空树的节点内容
            rootNode.Items.Clear();
            //构造树节点 
            foreach (DataRow dr in dtDept.Rows)
            {
                string parentNo = dr["ParentNo"].ToString();
                if (parentNo != "0")
                    continue;

                rootNode.Header = dr["Name"].ToString();
                rootNode.Name = dr["No"].ToString();
                rootNode.Tag = dr["No"].ToString();
                this.treeView1.Items.Add(rootNode);
                break;
            }
            rootNode.IsExpanded = true;

            foreach (DataRow dr in dtDept.Rows)
            {
                string parentNo = dr["ParentNo"].ToString();
                if (rootNode.Tag == null || parentNo != rootNode.Tag.ToString())
                    continue;

                TreeViewItem subNode = new TreeViewItem();
                subNode.Header = dr["Name"].ToString();
                subNode.Name = dr["No"].ToString();
                subNode.Tag = dr["No"].ToString();
                rootNode.Items.Add(subNode);

                if (Glo.CurrTreeViewNode != null)
                {
                    if (subNode.Tag == Glo.CurrTreeViewNode.Tag)
                        subNode.IsExpanded = true;
                }

                //增加下级.
                AddSubNode(subNode, dtDept);
            }
        }
        /// <summary>
        /// 增加节点.
        /// </summary>
        /// <param name="myNode"></param>
        /// <param name="dt"></param>
        public void AddSubNode(TreeViewItem myNode, DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                string parentNo = dr["ParentNo"].ToString();
                if (parentNo != myNode.Tag.ToString())
                    continue;

                TreeViewItem subNode = new TreeViewItem();
                subNode.Header = dr["Name"].ToString();
                subNode.Name = dr["No"].ToString();
                subNode.Tag = dr["No"].ToString();
                myNode.Items.Add(subNode);

                if (Glo.CurrTreeViewNode != null)
                {
                    if (subNode.Tag == Glo.CurrTreeViewNode.Tag)
                        subNode.IsExpanded = true;
                }

                // 增加sub node.
                AddSubNode(subNode, dt);
            }
        }
        /// <summary>
        /// 重新刷新部门信息
        /// </summary>
        /// <param name="fk_dept">要刷新的部门节点</param>
        public void Refresh()
        {
            this.BingTree();
            return;
        }
        void RefreshTree(object sender, RunSQLReturnTableSCompletedEventArgs e)
        {
            DataSet ds = new DataSet();
            ds.FromXml(e.Result);
            DataTable dtDept = ds.Tables[0]; //部门信息。

            foreach (DataRow dr in dtDept.Rows)
            {
                string no = dr["No"].ToString();
                if (Glo.CurrTreeViewNode.Tag.ToString() != no)
                    continue;

                Glo.CurrTreeViewNode.Header = dr["Name"].ToString();
                string pNo = dr["ParentNo"].ToString();
            }

        }
        /// <summary>
        /// 当打开一个树节点时
        /// </summary>
        /// <param name="deptNo"></param>
        public void OnTreeNodeDBClick()
        {
            //清空链表
            pList.Clear();
            //绑定部门下的人员
            BindEmpByDeptNo(Glo.FK_Dept, Glo.Dept_Name);
        }

        private void BindEmpByDeptNo(string FK_Dept, string Dept_Name)
        {
            OSSoapClient da = Glo.GetOSServiceInstance();
            da.GetEmpsByDeptNoAsync(FK_Dept, Dept_Name);
            da.GetEmpsByDeptNoCompleted += new EventHandler<GetEmpsByDeptNoCompletedEventArgs>(BindEmpAndStation);

            //添加子部门的岗位与人员
            if (CKB_ViewAll.IsChecked == true)
            {
                string sql = "SELECT No,Name FROM PORT_DEPT WHERE ParentNo ='" + FK_Dept + "'";
                OSSoapClient da_cd = Glo.GetOSServiceInstance();
                da_cd.RunSQLReturnTableAsync(sql);
                da_cd.RunSQLReturnTableCompleted += new EventHandler<RunSQLReturnTableCompletedEventArgs>(BindChildDeptMent);
            }
        }

        void BindChildDeptMent(object sender, RunSQLReturnTableCompletedEventArgs e)
        {
            DataSet ds = new DataSet();
            ds.FromXml(e.Result);
            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable dt_Dept = ds.Tables[0];
                foreach (DataRow row in dt_Dept.Rows)
                {
                    BindEmpByDeptNo(row["No"].ToString(), row["Name"].ToString());
                }
            }
        }

        /// <summary>
        /// 绑定人员与岗位在同一个列表里.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BindEmpAndStation(object sender, GetEmpsByDeptNoCompletedEventArgs e)
        {
            DataSet ds = new DataSet();
            ds.FromXml(e.Result);
            DataTable dtEmp = ds.Tables[0]; //人员信息。
            DataTable dtStation = ds.Tables[1]; //岗位信息.

            // 先放岗位.
            foreach (DataRow dr in dtStation.Rows)
            {
                PeopleNode emp = new PeopleNode();
                emp.No = dr["No"];
                emp.Name = dr["Name"];
                emp.FK_Duty = "99999";//与人员进行区分
                pList.Add(emp);
            }

            foreach (DataRow dr in dtEmp.Rows)
            {
                PeopleNode emp = new PeopleNode();
                emp.No = dr["No"];
                emp.Name = dr["Name"];
                emp.EmpNo = dr["EmpNo"];
                emp.Tel = dr["Tel"];
                emp.Email = dr["Email"];
                emp.FK_Duty = dr["FK_Duty"];
                emp.FK_DeptText = dr["DetpName"];
                emp.FK_Dept = dr["FK_Dept"];
                emp.FK_DutyText = dr["DutyName"];
                emp.DutyLevel = int.Parse(dr["DutyLevel"] == null ? "3" : dr["DutyLevel"]);
                emp.Leader = dr["Leader"];
                emp.Stations = dr["Stations"];
                pList.Add(emp);
            }
            this.DG_Emp.ItemsSource = pList.ToList(); //.ToList();
        }

        #region 右键菜单相关
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            popMenu.Visibility = Visibility.Collapsed;
            MenuItem mi = sender as MenuItem;

            TreeViewItem td = treeView1.SelectedItem as TreeViewItem;
            if (td == null)
                return;

            string deptNo = td.Tag.ToString();
            Glo.FK_Dept = deptNo;
            Glo.Dept_Name = td.Header.ToString();
            Glo.CurrTreeViewNode = td;

            FrmDept fd = new FrmDept();
            fd.ReFreshParentEve += new FrmDept.ReFreshParent(fd_ReFreshParentEve);//注册事件，刷新父窗体
            switch (mi.Name)
            {
                case "Btn_RefreshDept": //刷新.
                    this.Refresh();
                    break;
                case "Btn_Edit": //编辑
                    fd.InitDeptInfo("EditDept", deptNo);
                    fd.Show();
                    break;
                case "Btn_CrateSameLevel": // 创建同级目录.
                    fd.InitDeptInfo("CrateSameLevel", deptNo);
                    fd.Show();
                    //this.Refresh();
                    break;
                case "Btn_CrateSubLevel": // 创建下级目录.
                    fd.InitDeptInfo("CrateSubLevel", deptNo);
                    fd.Show();
                    //this.Refresh();
                    break;
                case "Btn_Delete": // 删除部门.
                    if (MessageBox.Show("您确定要删除吗？", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        OSSoapClient daDelDept = BP.Glo.GetOSServiceInstance();
                        daDelDept.Dept_DeleteAsync(deptNo,false);
                        daDelDept.Dept_DeleteCompleted += new EventHandler<Dept_DeleteCompletedEventArgs>(daDelDept_Dept_DeleteCompleted);
                    }
                    break;
                case "Btn_EditEmp": // 编辑人员.
                    OnTreeNodeDBClick();  //读入相关的数据
                    break;
                case "Btn_AddEmp": // 增加人员.
                    FrmDeptEmp fde = new FrmDeptEmp();
                    fde.ReFreshParentEve += new FrmDeptEmp.ReFreshParent(fde_ReFreshParentEve);
                    fde.InitEmp("New", deptNo, null);
                    fde.Show();
                    break;
                case "Btn_RelatedEmp"://关联人员
                    FrmEmps emps = new FrmEmps();
                    emps.ReFreshParentEve += new FrmEmps.ReFreshParent(fde_ReFreshParentEve);
                    emps.InitEmps(deptNo);
                    emps.Show();
                    break;
                case "Btn_Up": //上移.
                    OS.OSSoapClient da = Glo.GetOSServiceInstance();
                    da.DoAsync("DeptUp", deptNo, true);
                    da.DoCompleted += new EventHandler<DoCompletedEventArgs>(da_DoCompleted);
                    break;
                case "Btn_Down": //下移.
                    OS.OSSoapClient da1 = Glo.GetOSServiceInstance();
                    da1.DoAsync("DeptDown", deptNo, true);
                    da1.DoCompleted += new EventHandler<DoCompletedEventArgs>(da_DoCompleted);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 刷新人员列表
        /// </summary>
        void fde_ReFreshParentEve()
        {
            if (string.IsNullOrEmpty(Glo.FK_Dept)) return;
            OnTreeNodeDBClick();  //读入相关的数据
        }
        /// <summary>
        /// 刷新树
        /// </summary>
        void fd_ReFreshParentEve()
        {
            this.Refresh();
        }
        //增加人员
        private void Btn_Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Glo.FK_Dept))
            {
                MessageBox.Show("请选择部门。", "提示", MessageBoxButton.OK);
                return;
            }

            FrmDeptEmp fde = new FrmDeptEmp();
            fde.InitEmp("New", Glo.FK_Dept, null);
            fde.ReFreshParentEve += new FrmDeptEmp.ReFreshParent(fde_ReFreshParentEve);
            fde.Show();
        }
        //删除人员
        private void Btn_Delete1_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("您确定要删除吗？", "提示", MessageBoxButton.OKCancel)
                == MessageBoxResult.Cancel)
                return;

            DataGridSelectNode = this.DG_Emp.SelectedItem as PeopleNode;
            if (DataGridSelectNode == null) //DataGridSelectNode 放的是左键选中的数据
            {
                return;
            }
            if (DataGridSelectNode.FK_Duty == "99999")
            {
                MessageBox.Show("您选择的是岗位信息，请选择人员。", "提示", MessageBoxButton.OK);
                return;//如果选择的是岗位，不进行编辑
            }

            OS.OSSoapClient daDeleteEmp = Glo.GetOSServiceInstance();
            daDeleteEmp.Emp_DeleteAsync(DataGridSelectNode.No, DataGridSelectNode.FK_Dept);//("DeleteEmp", DataGridSelectNode.No, true);
            daDeleteEmp.Emp_DeleteCompleted += new EventHandler<Emp_DeleteCompletedEventArgs>(DeleteEmp_DoCompleted);
        }
        //刷新
        private void Btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Glo.FK_Dept))
            {
                MessageBox.Show("请选择部门。", "提示", MessageBoxButton.OK);
                return;
            }
            OnTreeNodeDBClick();  //读入相关的数据
        }
        void da_DoCompleted(object sender, DoCompletedEventArgs e)
        {
            this.BingTree();
        }
        void daDelDept_Dept_DeleteCompleted(object sender, Dept_DeleteCompletedEventArgs e)
        {
            if (e.Result != null && e.Result.Contains("err"))
            {
                if (MessageBox.Show(e.Result + ",您确定要强制删除吗？", "提示", MessageBoxButton.OKCancel)
== MessageBoxResult.OK)
                {
                    OSSoapClient daDelDept = BP.Glo.GetOSServiceInstance();
                    daDelDept.Dept_DeleteAsync(Glo.FK_Dept, true);
                    daDelDept.Dept_DeleteCompleted += new EventHandler<Dept_DeleteCompletedEventArgs>(daDelDept_Dept_DeleteCompleted);   
                    return;
                }
            }

            // 移除Node.
            this.treeView1.Items.Remove(Glo.CurrTreeViewNode);
            Glo.CurrTreeViewNode = null;
            this.Refresh();
        }
        private void treeView1_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            //取得当前菜单位置坐标 
            Point pos = e.GetPosition(this.treeView1);
            // 调整x,y 值 ，以防止菜单被遮盖住
            var x = pos.X;
            var y = pos.Y;
            var menuHeight = 230;
            var menuWidth = 170;
            if (x + menuWidth > 220)
            {
                x = x - (x + menuWidth - 220);
            }
            if (y + menuHeight > Application.Current.Host.Content.ActualHeight)
            {
                y = y - (y + menuHeight - Application.Current.Host.Content.ActualHeight);
            }
            //定位右键菜单 
            popMenu.Margin = new Thickness(x, y, 0, 0);
            //显示右键菜单 
            popMenu.Visibility = Visibility.Visible;
        }
        private void treeView1_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;//屏蔽系统默认的右键菜单
        }
        private void popMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            //鼠标移开 关闭菜单 
            popMenu.Visibility = Visibility.Collapsed;
        }

        private void treeView1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //左键按下 获得deptNo
            TreeViewItem td = treeView1.SelectedItem as TreeViewItem;
            if (td == null)
                return;

            Glo.FK_Dept = td.Tag.ToString();
            Glo.Dept_Name = td.Header.ToString();

            OnTreeNodeDBClick();  //读入相关的数据
        }
        #endregion


        #region DataGrid 相关事件
        DateTime _lastTime;
        private void DG_Emp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // DataGrid 左键按下
            DataGridSelectNode = this.DG_Emp.SelectedItem as PeopleNode; //获得当前的元组
            if (DataGridSelectNode != null)
            {
                foreach (TreeViewItem item in treeView1.Items)
                {
                    if (item.Name == DataGridSelectNode.FK_Dept)
                    {
                        item.IsSelected = true;
                        item.Focus();

                        TreeViewItem parentNode = item.Parent as TreeViewItem;
                        if (parentNode != null) parentNode.IsExpanded = true;
                    }
                    SetDeptNodeSelected(item, DataGridSelectNode.FK_Dept);
                }
            }
            //双击
            if ((DateTime.Now.Subtract(_lastTime).TotalMilliseconds) < 300)
            {
                if (DataGridSelectNode == null) //DataGridSelectNode 放的是左键选中的数据
                {
                    return;
                }
                if (DataGridSelectNode.FK_Duty == "99999")
                {
                    MessageBox.Show("您选择的是岗位信息，请选择人员。", "提示", MessageBoxButton.OK);
                    return;//如果选择的是岗位，不进行编辑
                }
                FrmDeptEmp fde = new FrmDeptEmp();
                fde.InitEmp("Edit", DataGridSelectNode.FK_Dept, DataGridSelectNode.No);
                fde.ReFreshParentEve += new FrmDeptEmp.ReFreshParent(fde_ReFreshParentEve);
                fde.Show();
            }
            _lastTime = DateTime.Now;
        }
        /// <summary>
        /// 选择人员后，自动定位相应部门
        /// </summary>
        /// <param name="treeNode"></param>
        /// <param name="FK_Dept"></param>
        private void SetDeptNodeSelected(TreeViewItem treeNode, string FK_Dept)
        {
            foreach (TreeViewItem item in treeNode.Items)
            {
                if (item.Name == FK_Dept)
                {
                    item.IsSelected = true;
                    item.Focus();
                    TreeViewItem parentNode = item.Parent as TreeViewItem;
                    if (parentNode != null) parentNode.IsExpanded = true;
                }
                SetDeptNodeSelected(item, FK_Dept);
            }
        }
        private void DG_Emp_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;//屏蔽系统默认的右键菜单
        }
        private void DG_Emp_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            //取得当前菜单位置坐标 
            Point pos = e.GetPosition(this.DG_Emp);
            //定位右键菜单 
            popMenuDataGrid.Margin = new Thickness(pos.X, pos.Y, 0, 0);
            //显示右键菜单 
            popMenuDataGrid.Visibility = Visibility.Visible;
        }
        private void popMenuDataGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            popMenuDataGrid.Visibility = Visibility.Collapsed;
        }
        #endregion DataGrid 相关事件

        private void Men_Click(object sender, RoutedEventArgs e)
        {
            popMenuDataGrid.Visibility = Visibility.Collapsed;
            DataGridSelectNode = this.DG_Emp.SelectedItem as PeopleNode;
            if (DataGridSelectNode == null) //DataGridSelectNode 放的是左键选中的数据
            {
                //MessageBox.Show("没有选择行","", MessageBoxButton.;
                return;
            }
            if (DataGridSelectNode.FK_Duty == "99999")
            {
                MessageBox.Show("您选择的是岗位信息，请选择人员。", "提示", MessageBoxButton.OK);
                return;//如果选择的是岗位，不进行编辑
            }

            MenuItem mi = sender as MenuItem;

            //   MessageBox.Show(mi.Name + " DataGridSelectNode =" + DataGridSelectNode.No );
            switch (mi.Name)
            {
                case "Men_Edit": //编辑
                    FrmDeptEmp fde = new FrmDeptEmp();
                    fde.InitEmp("Edit", DataGridSelectNode.FK_Dept, DataGridSelectNode.No);
                    fde.ReFreshParentEve += new FrmDeptEmp.ReFreshParent(fde_ReFreshParentEve);
                    fde.Show();
                    break;
                case "Men_Del": // 删除
                    if (MessageBox.Show("您确定要删除吗？", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        OS.OSSoapClient daDeleteEmp = Glo.GetOSServiceInstance();
                        daDeleteEmp.Emp_DeleteAsync(DataGridSelectNode.No, DataGridSelectNode.FK_Dept);//("DeleteEmp", DataGridSelectNode.No, true);
                        daDeleteEmp.Emp_DeleteCompleted += new EventHandler<Emp_DeleteCompletedEventArgs>(DeleteEmp_DoCompleted);
                    }
                    break;
                case "Men_Reset": // 恢复密码
                    OS.OSSoapClient ReSetPass = Glo.GetOSServiceInstance();
                    ReSetPass.DoAsync("ResetPassword", DataGridSelectNode.No, true);
                    ReSetPass.DoCompleted += new EventHandler<DoCompletedEventArgs>(ReSetPass_DoCompleted);
                    break;
                default:
                    break;
            }
        }

        void ReSetPass_DoCompleted(object sender, DoCompletedEventArgs e)
        {
            if (e.Result != null && e.Result.Contains("err"))
            {
                MessageBox.Show(e.Result, "重置密码错误", MessageBoxButton.OK);
                return;
            }
            MessageBox.Show("重置密码成功，现在" + DataGridSelectNode.Name + "，可以以123登录系统了。",
                     "提示", MessageBoxButton.OK);
        }

        void DeleteEmp_DoCompleted(object sender, Emp_DeleteCompletedEventArgs e)
        {
            if (e.Result != null && e.Result.Contains("err"))
            {
                MessageBox.Show(e.Result, "删除错误", MessageBoxButton.OK);
                return;
            }
            //重新绑定它.
            OnTreeNodeDBClick();
        }
        //显示该部门下所有人员
        private void CKB_ViewAll_Checked(object sender, RoutedEventArgs e)
        {
            OnTreeNodeDBClick();
        }

        private void TreeViewDragDropTarget_ItemDroppedOnTarget(object sender, ItemDragEventArgs e)
        {
            //没有数据进行返回
            if (e.Data == null) return;
            if (!treeView1.AllowDrop) return;

            SelectionCollection nodes = ((SelectionCollection)e.Data);
            Selection ites = nodes[0];
            //源节点
            TreeViewItem nodeSource = (TreeViewItem)ites.Item;
            //判断父节点是否存在
            if (nodeSource.Parent != null)
            {
                //判断类型是否是树节点
                if (nodeSource.Parent.ToString().Contains("TreeViewItem"))
                {
                    TreeViewItem nodeTarget = (TreeViewItem)nodeSource.Parent;
                    OS.OSSoapClient dept_DrapTarget = Glo.GetOSServiceInstance();
                    dept_DrapTarget.Dept_DragTargetAsync(nodeSource.Tag.ToString(), nodeTarget.Tag.ToString());
                    dept_DrapTarget.Dept_DragTargetCompleted += new EventHandler<Dept_DragTargetCompletedEventArgs>(dept_DrapTarget_Dept_DragTargetCompleted);
                    nodeTarget.IsExpanded = true;
                }
                else
                {
                    this.Refresh();
                }
            }
        }

        void dept_DrapTarget_Dept_DragTargetCompleted(object sender, Dept_DragTargetCompletedEventArgs e)
        {
            if (e.Result != null && e.Result.Contains("err"))
            {
                MessageBox.Show(e.Result, "拖动节点操作失败！", MessageBoxButton.OK);
                return;
            }
        }

        private void TreeViewDragDropTarget_ItemDragCompleted(object sender, ItemDragEventArgs e)
        {
            if (e.Data == null) return;

            SelectionCollection nodes = ((SelectionCollection)e.Data);
            Selection ites = nodes[0];
            //源节点
            TreeViewItem nodeSource = (TreeViewItem)ites.Item;
            //判断是否节点信息
            if (nodeSource.Parent != null && nodeSource.Parent.ToString().Contains("TreeViewItem"))
            {
                //父节点
                TreeViewItem nodeParent = (TreeViewItem)nodeSource.Parent;
                bool isNextNode = false;
                //查找源节点拖动后的上一个节点和下一个节点
                TreeViewItem nodeSourceNext = null, nodeSourcePre = null;
                string nextNodeNos = "";
                foreach (TreeViewItem item in nodeParent.Items)
                {
                    //获取新结构中的拖动节点的下一个节点
                    if (isNextNode == true)
                    {
                        nextNodeNos += item.Tag + ",";
                        //只赋值一次
                        if (nodeSourceNext == null)
                            nodeSourceNext = item;
                    }
                    //如果是拖动的节点
                    if (item.Tag == nodeSource.Tag)
                    {
                        isNextNode = true;
                    }
                    //获取新结构中的拖动节点的上一个节点
                    if (isNextNode == false)
                    {
                        nodeSourcePre = item;
                    }
                }
                //执行数据排序存储
                OS.OSSoapClient dept_DrapSort = Glo.GetOSServiceInstance();
                if (nodeSourceNext != null)
                {
                    dept_DrapSort.Dept_DragSortAsync(nodeSource.Tag.ToString(), nodeSourceNext.Tag.ToString(), nextNodeNos, false);
                    dept_DrapSort.Dept_DragSortCompleted += new EventHandler<Dept_DragSortCompletedEventArgs>(dept_DrapSort_Dept_DragSortCompleted);
                }
                else if (nodeSourcePre != null)
                {
                    dept_DrapSort.Dept_DragSortAsync(nodeSource.Tag.ToString(), nodeSourcePre.Tag.ToString(), null, true);
                    dept_DrapSort.Dept_DragSortCompleted += new EventHandler<Dept_DragSortCompletedEventArgs>(dept_DrapSort_Dept_DragSortCompleted);
                }
            }
        }

        void dept_DrapSort_Dept_DragSortCompleted(object sender, Dept_DragSortCompletedEventArgs e)
        {
            if (e.Result != null && e.Result.Contains("err"))
            {
                MessageBox.Show(e.Result, "拖动节点排序失败！", MessageBoxButton.OK);
                return;
            }
        }
        //是否启用拖动
        private void ckbEnbelDrop_Click(object sender, RoutedEventArgs e)
        {
            treeView1.AllowDrop = (bool)ckbEnbelDrop.IsChecked;
            //treeDropTool.IsEnabled = (bool)ckbEnbelDrop.IsChecked;
        }
        //查询人员
        private void Btn_QueryEmp_Click(object sender, RoutedEventArgs e)
        {
            Dict.FrmEmpCondition form_EmpCondition = new Dict.FrmEmpCondition();
            form_EmpCondition.RunSqlReturnEmpsEve += new Dict.FrmEmpCondition.RunSqlReturnEmps(form_EmpCondition_RunSqlReturnEmpsEve);
            form_EmpCondition.Show();
        }

        void form_EmpCondition_RunSqlReturnEmpsEve(string condition)
        {
            string sql = "";
            sql = "SELECT a.No,a.EmpNo,a.Name,a.Email,a.Tel,b.FK_Duty,b.FK_Dept,d.Name as DetpName, c.Name as DutyName, b.DutyLevel,b.Leader"
                            + ",STUFF((SELECT ','+e.Name FROM Port_DeptEmpStation d,Port_Station e"
                            + " WHERE d.FK_Station = e.No AND d.FK_Emp=a.no FOR XML PATH('')),1,1,'') AS Stations"
                            + " FROM Port_Emp a, Port_DeptEmp b , Port_Duty c,Port_Dept d"
                            + " WHERE A.No=B.FK_Emp AND b.FK_Duty=c.No "
                            + " and b.FK_Dept=d.No and (a.No like '%" + condition + "%' OR a.Name like '%" + condition + "%' OR a.EmpNo like '%" + condition + "%') order by a.No";
            OSSoapClient da = Glo.GetOSServiceInstance();
            da.RunSQLReturnTableSAsync(sql);
            da.RunSQLReturnTableSCompleted += new EventHandler<RunSQLReturnTableSCompletedEventArgs>(BindEmpByCondition);
        }

        /// <summary>
        /// 绑定通过查询的人员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BindEmpByCondition(object sender, RunSQLReturnTableSCompletedEventArgs e)
        {
            DataSet ds = new DataSet();
            ds.FromXml(e.Result);
            DataTable dtEmp = ds.Tables[0]; //人员信息。
            pList.Clear();
            foreach (DataRow dr in dtEmp.Rows)
            {
                PeopleNode emp = new PeopleNode();
                emp.No = dr["No"];
                emp.Name = dr["Name"];
                emp.EmpNo = dr["EmpNo"];
                emp.Tel = dr["Tel"];
                emp.Email = dr["Email"];
                emp.FK_Duty = dr["FK_Duty"];
                emp.FK_DeptText = dr["DetpName"];
                emp.FK_Dept = dr["FK_Dept"];
                emp.FK_DutyText = dr["DutyName"];
                emp.DutyLevel = int.Parse(dr["DutyLevel"] == null ? "3" : dr["DutyLevel"]);
                emp.Leader = dr["Leader"];
                emp.Stations = dr["Stations"];
                pList.Add(emp);
            }
            this.DG_Emp.ItemsSource = pList.ToList(); //.ToList();
        }
    }
}
