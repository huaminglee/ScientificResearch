using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace OrganizationalStructure.Dict
{
    public partial class FrmEmpCondition : ChildWindow
    {
        //声明委托R
        public delegate void RunSqlReturnEmps(string condition);
        public event RunSqlReturnEmps RunSqlReturnEmpsEve; //声明事件
        public string TB_CondtionText
        {
            get 
            {
                return TB_Condition.Text;
            }
        }

        public FrmEmpCondition()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (RunSqlReturnEmpsEve != null) RunSqlReturnEmpsEve(TB_CondtionText);
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

