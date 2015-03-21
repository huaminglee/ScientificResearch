using BP.WF.Template;
using ScientificResearchPrj.DALFactory;
using ScientificResearchPrj.IBLL;
using ScientificResearchPrj.IDAL;
using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ScientificResearchPrj.BLL
{
    public class ChaoSongService : BaseService<Object>, IChaoSongService
    {
        public override void SetCurrentDAL()
        {
            CurrentDAL = this.DbSession.ChaoSongDAL;
        }

        public DataTable GetAllCClist()
        {
            DataTable ccList_All = BP.WF.Dev2Interface.DB_CCList(BP.Web.WebUser.No);
            if (ccList_All != null && ccList_All.Rows != null) {
                for (int i = ccList_All.Rows.Count-1; i >=0; i--)
                {
                    if (Convert.ToInt32(ccList_All.Rows[i]["Sta"]) == (int)CCSta.Del)
                    { 
                        ccList_All.Rows.RemoveAt(i);
                    }
                }
            }
            SetRecName(ccList_All);
            return ccList_All;
        }

        public DataTable GetReadCClist()
        {
            DataTable ccList_Read = BP.WF.Dev2Interface.DB_CCList_Read(BP.Web.WebUser.No);
            SetRecName(ccList_Read); 
            return ccList_Read;
        }

        public DataTable GetUnReadCClist()
        {
            DataTable ccList_UnRead = BP.WF.Dev2Interface.DB_CCList_UnRead(BP.Web.WebUser.No);
            SetRecName(ccList_UnRead); 
            return ccList_UnRead;
        }

        public DataTable GetDeleteCClist()
        {
            DataTable ccList_Delete = BP.WF.Dev2Interface.DB_CCList_Delete(BP.Web.WebUser.No);
            SetRecName(ccList_Delete); 
            return ccList_Delete;
        }

        private void SetRecName(DataTable ccList){
            if(ccList==null || ccList.Rows==null|| ccList.Rows.Count==0) return;
            for (int i = 0; i < ccList.Rows.Count; i++) {
                string empNo = ccList.Rows[i]["Rec"].ToString();
                MyPort_Emp emp = DBSessionFactory.GetCurrentDbSession().EmpDAL.LoadEntities(a => a.EmpNo == empNo).FirstOrDefault();
                if (emp != null) {
                    ccList.Rows[i]["Rec"] = empNo + ":" + emp.Name;
                }
            }
        }

        public void CCSetRead(string myPK)
        {
            BP.WF.Dev2Interface.Node_CC_SetRead(myPK);
        }

        public void CCLogicalDelete(string myPK)
        {
            (CurrentDAL as IChaoSongDAL).CCLogicalDelete(myPK);
        }

        public void CCPhysicalDelete(string myPK)
        {
            (CurrentDAL as IChaoSongDAL).CCPhysicalDelete(myPK);
        }

        public string WriteToCCList(CCModel cc)
        {
            string[] chaoSongRenTo = cc.ChaoSongRenTo.Split(',');
            string returnStr = "";
            foreach (var item in chaoSongRenTo)
            {
                var temp = item.Remove(item.LastIndexOf(")"));
                int index = temp.IndexOf("(");
                string no = temp.Substring(0, index);
                string name = temp.Substring(index + 1);
                returnStr += BP.WF.Dev2Interface.Node_CC_WriteTo_CClist(cc.FK_Node, cc.FK_Node, cc.WorkID, no, name, cc.ChaoSongBiaoTi, cc.ChaoSongNeiRong) + ";";
            }
            return returnStr;
        }
    }
}
