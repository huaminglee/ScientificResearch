using BP.DA;
using BP.WF;
using BP.WF.Template;
using ScientificResearchPrj.IDAL;
using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScientificResearchPrj.DAL
{
    public class ChaoSongDAL : BaseDAL<Object>, IChaoSongDAL
    {
        public void CCLogicalDelete(string myPK) {
            string sql = "Update WF_CCList Set Sta = " + (int)CCSta.Del + " Where MyPK = " + myPK;
            BP.DA.DBAccess.RunSQL(sql);
        }

        public void CCPhysicalDelete(string myPK) {
            string sql = "Delete From WF_CCList Where MyPK = " + myPK;
            BP.DA.DBAccess.RunSQL(sql);
        }
    }
}
