using BP.DA;
using ScientificResearchPrj.DAL;
using ScientificResearchPrj.IBLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ScientificResearchPrj.BLL
{
    public class LiuChengSheJiService :  ILiuChengSheJiService
    {
        public string GetLiuChengSheJiURL()
        {
            string url = "";
            if (BP.Web.WebUser.No.Equals("admin"))
            {
                url = "/WF/Admin/XAP/Designer.aspx?IsCheckUpdate=1&UserNo=" + BP.Web.WebUser.No + "&SID=" + BP.Web.WebUser.SID;
                // url = "/WF/Admin/XAP/Designer.aspx?IsCheckUpdate=1&UserNo=" + BP.Web.WebUser.No;
            }
            return url;
        }
    }
}
