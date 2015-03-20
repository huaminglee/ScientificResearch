using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using System.Data;
using BP.DA;
using BP.Web;
using BP.En;
using System.Text;
using BP.Port;
using BP.Sys;
using BP.WF.Template;
using BP.WF.Data;

namespace CCFlow.WF.CCFlowDesigner
{
    public partial class Truck : System.Web.UI.Page
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
            //  取得轨迹信息.
            //string sql = "SELECT * FROM ND" + int.Parse(this.FK_Flow) + "Track WHERE WorkID=" + this.WorkID + " ORDER BY RDT";
            //DataTable dtTrack = DBAccess.RunSQLReturnTable(sql);
            ////  ActionType.

            ////节点集合.
            //Nodes nds = new Nodes(this.FK_Flow);

            //// 标签集合.
            //LabNotes labes = new LabNotes(this.FK_Flow);

            ////方向集合.
            //Directions dirs = new Directions(this.FK_Flow);
            
        }
    }
}