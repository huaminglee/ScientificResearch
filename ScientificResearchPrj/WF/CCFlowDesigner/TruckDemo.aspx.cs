using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.WF.CCFlowDesigner
{
    public partial class TruckDemo : System.Web.UI.Page
    {
        #region 属性.
        public Int64 WorkID
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Request["WorkID"]))
                    return 133;

                return Int64.Parse(Request["WorkID"]);
            }
        }
        public Int64 FID
        {
            get
            {
                if (Request["FID"] != null)
                    return Int64.Parse(Request["FID"]);

                return 133;
            }
        }
        public string FK_Flow
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Request.QueryString["FK_Flow"]))
                    return "002";

                return Request.QueryString["FK_Flow"];
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}