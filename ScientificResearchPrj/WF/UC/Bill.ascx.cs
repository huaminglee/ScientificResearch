using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using BP.WF;
using BP.Port;
using BP.Sys;
using BP.DA;
using BP.En;
using BP.Web;
namespace CCFlow.WF.UC
{
    public partial class WF_UC_Bill : BP.Web.UC.UCBase3
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            BP.WF.Data.Bills Bills = new BP.WF.Data.Bills();
            Bills.RetrieveAll();

            this.AddTable();
            this.AddTR();
            this.AddTDTitle("ID");
            this.AddTDTitle("流程");
            this.AddTDTitle("节点");
            this.AddTDTitle("部门");
            this.AddTDTitle("单据名称");
            this.AddTDTitle("打印日期");
            this.AddTDTitle("打印人");
            this.AddTREnd();

            int i = 0;
            bool is1 = false;
            foreach (BP.WF.Data.Bill bill in Bills)
            {
                this.AddTR(is1);
                i++;
                this.AddTDIdx(i);
                this.AddTD(bill.FK_FlowT);
                this.AddTD(bill.FK_NodeT);
                this.AddTD(bill.FK_DeptT);
                this.AddTDA("javascript:WinOpen('" + bill.Url + "')", "<img src='/WF/Img/Btn/Word.gif' border=0 />" + bill.FK_BillText);
                this.AddTD(bill.RDT);
                this.AddTD(bill.FK_EmpT);
                this.AddTREnd();
            }
            this.AddTableEnd();
        }
    }
}