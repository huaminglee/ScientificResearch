using ScientificResearchPrj.DAL;
using ScientificResearchPrj.IBLL;
using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.BLL 
{
    public class LunWenZhuanXieService : BaseService<Process_LunWen>, ILunWenZhuanXieService
    {
        public override void SetCurrentDAL()
        {
            CurrentDAL = this.DbSession.LunWenZhuanXieDAL;
        }

        public List<Process_LunWen> GetHistoryData(CCFlowArgs args)
        {
            string fk_node = args.FK_Node.ToString();
            string workid = args.WorkID.ToString();

            List<Process_LunWen> lwList = this.LoadEntities(
                a => a.Process_BasicData.FK_Flow == args.FK_Flow &&
                a.Process_BasicData.FK_Node == fk_node &&
                a.Process_BasicData.WorkId == workid).ToList();
            if (lwList != null && lwList.Count != 0)
            {
                for (int i = 0; i < lwList.Count; i++)
                {
                    SetProposerName(lwList[i]);
                }
                return lwList;
            }
            return new List<Process_LunWen>();
        }

        private void SetProposerName(Process_LunWen lunwen)
        {
            if (lunwen.Process_BasicData.FK_Proposer == null) return;

            MyPort_Emp proposer = this.DbSession.EmpDAL.LoadEntities(e => e.EmpNo.Equals(lunwen.Process_BasicData.FK_Proposer)).FirstOrDefault();
            if (proposer != null)
            {
                lunwen.Process_BasicData.ProposerName = proposer.Name;
            }
        }
         
        public void InsertOrUpdateTrack(CCFlowArgs args)
        {
            string workid = args.WorkID.ToString();
            Process_Track ifExist = this.DbSession.TrackDAL.LoadEntities(a => a.FK_Flow == args.FK_Flow
                && a.WorkID == workid).FirstOrDefault();

            List<Process_LunWen> lwList = this.LoadEntities(a => a.Process_BasicData.FK_Flow == args.FK_Flow
                && a.Process_BasicData.WorkId == workid).ToList();

            if (ifExist != null)
            {
                if (lwList != null && lwList.Count != 0)
                {
                    ifExist.LWZXOID = CombindOIDStr(lwList);
                }

                this.DbSession.TrackDAL.UpdateEntity(ifExist);
                this.DbSession.SaveChanges();
            }
            else
            {
                ifExist = new Process_Track();
                if (lwList != null && lwList.Count != 0)
                {
                    ifExist.LWZXOID = CombindOIDStr(lwList);
                }

                ifExist.FK_Flow = args.FK_Flow;
                ifExist.WorkID = workid;

                this.DbSession.TrackDAL.AddEntity(ifExist);
                this.DbSession.SaveChanges();
            }
        }

        private string CombindOIDStr(List<Process_LunWen> lwList)
        {
            string lwOIDStr = "";
            for (int i = 0; i < lwList.Count - 1; i++)
            {
                lwOIDStr += lwList[i].OID.ToString() + ",";
            }
            lwOIDStr += lwList[lwList.Count - 1].OID.ToString();

            return lwOIDStr;
        }

        public Dictionary<string, string> AddLunWen(Process_LunWen lunwen)
        {
            try
            {
                Process_LunWen ifIdxHasExist = this.LoadEntities(a => a.FK_BDNo == lunwen.Process_BasicData.No).FirstOrDefault();
                if (ifIdxHasExist != null) throw new Exception("论文【" + lunwen.Process_BasicData.No + "】编号已存在");

                lunwen.OID = (CurrentDAL as LunWenZhuanXieDAL).SelectMaxOid() + 1;
                lunwen.FK_BDNo = lunwen.Process_BasicData.No;

                this.AddEntity(lunwen);

                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add("state", "0");
                dictionary.Add("message", "添加成功");
                return dictionary;
            }
            catch (Exception ex)
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add("state", "-1");
                dictionary.Add("message", "添加失败~~~" + ex.Message);
                return dictionary;
            }
        }

        public Dictionary<string, string> DeleteLunWen(string lwNo)
        {
            try
            {
                DeleteLunWenWithoutUpdateColumn(lwNo);

                /// //更新轨迹表的关联关系
                //ToDo---------------------------------------------------------------------

                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add("state", "0");
                dictionary.Add("message", "删除成功");
                return dictionary;
            }
            catch (Exception ex)
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add("state", "-1");
                dictionary.Add("message", "删除失败~~~" + ex.Message);
                return dictionary;
            }
        }

        private void DeleteLunWenWithoutUpdateColumn(string lwNo)
        {
            Process_LunWen lunwen = this.LoadEntities(a => a.FK_BDNo == lwNo).FirstOrDefault();
            if (lunwen != null)
            {
                this.DeleteEntity(lunwen);
            }

            Process_BasicData basicData = this.DbSession.BasicDataDAL.LoadEntities(a => a.No == lwNo).FirstOrDefault();
            if (basicData != null)
            {
                this.DbSession.BasicDataDAL.DeleteEntity(basicData);
                this.DbSession.SaveChanges();
            }
        }

        public Dictionary<string, string> ModifyLunWen(string oldNo, Process_LunWen lunwen)
        {
            try
            {
                if (!oldNo.Equals(lunwen.Process_BasicData.No))
                {
                    Process_LunWen ifIdxHasExist = this.LoadEntities(a => a.FK_BDNo == lunwen.Process_BasicData.No).FirstOrDefault();
                    if (ifIdxHasExist != null) throw new Exception("论文【" + lunwen.Process_BasicData.No + "】编号已存在");

                    this.AddLunWen(lunwen);
                }
                else
                {
                    Process_LunWen lwDesc = this.LoadEntities(a => a.FK_BDNo == oldNo).FirstOrDefault();
                    if (lwDesc != null)
                    {
                        ModifyLunWen(lwDesc, lunwen);
                        this.UpdateEntity(lwDesc);
                    }
                }

                //使用OID作为外键，没有修改编号的话，不改变OID，无需修改外键关系
                if (!oldNo.Equals(lunwen.Process_BasicData.No))
                {
                    Process_LunWen oldlw = this.LoadEntities(a => a.FK_BDNo == oldNo).FirstOrDefault();
                    Process_LunWen newlw = this.LoadEntities(a => a.FK_BDNo == lunwen.Process_BasicData.No).FirstOrDefault();

                    //更新轨迹表的关联关系
                    (CurrentDAL as LunWenZhuanXieDAL).UpdateTrackLWZXOID(oldlw.OID, newlw.OID);

                    this.DeleteLunWenWithoutUpdateColumn(oldNo);
                }

                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add("state", "0");
                dictionary.Add("message", "修改成功");
                return dictionary;
            }
            catch (Exception ex)
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add("state", "-1");
                dictionary.Add("message", "修改失败~~~" + ex.Message);
                return dictionary;
            }
        }

        private void ModifyLunWen(Process_LunWen lwDesc, Process_LunWen lwSource)
        {
            lwDesc.OID = lwDesc.OID;

            lwDesc.FK_BDNo = lwSource.Process_BasicData.No;
            lwDesc.Motivation = lwSource.Motivation;
            lwDesc.Questions = lwSource.Questions;
            lwDesc.Design = lwSource.Design;
            lwDesc.Realize = lwSource.Realize;
            lwDesc.TestData = lwSource.TestData;
            lwDesc.Result = lwSource.Result;

            lwDesc.Process_BasicData.No = lwSource.Process_BasicData.No;
            lwDesc.Process_BasicData.Name = lwSource.Process_BasicData.Name;
            lwDesc.Process_BasicData.FK_Proposer = lwSource.Process_BasicData.FK_Proposer;
            lwDesc.Process_BasicData.ProposeTime = lwSource.Process_BasicData.ProposeTime;
            lwDesc.Process_BasicData.Description = lwSource.Process_BasicData.Description;
            lwDesc.Process_BasicData.Keys = lwSource.Process_BasicData.Keys;
            lwDesc.Process_BasicData.Remarks = lwSource.Process_BasicData.Remarks;
            lwDesc.Process_BasicData.ModifyTime = lwSource.Process_BasicData.ModifyTime;

            if (lwSource.Process_BasicData.FK_Flow != null)
            {
                lwDesc.Process_BasicData.FK_Flow = lwSource.Process_BasicData.FK_Flow;
            }
            else
            {
                lwDesc.Process_BasicData.FK_Flow = lwDesc.Process_BasicData.FK_Flow;
            }

            if (lwSource.Process_BasicData.FK_Node != null)
            {
                lwDesc.Process_BasicData.FK_Node = lwSource.Process_BasicData.FK_Node;
            }
            else
            {
                lwDesc.Process_BasicData.FK_Node = lwDesc.Process_BasicData.FK_Node;
            }

            if (lwSource.Process_BasicData.WorkId != null)
            {
                lwDesc.Process_BasicData.WorkId = lwSource.Process_BasicData.WorkId;
            }
            else
            {
                lwDesc.Process_BasicData.WorkId = lwDesc.Process_BasicData.WorkId;
            }
        }

        public List<Process_LunWen> GetLunWenHistoryDataFromTrack(CCFlowArgs args)
        {
            try
            {
                string workid = args.WorkID.ToString();
                Process_Track track = this.DbSession.TrackDAL.LoadEntities(a => a.FK_Flow == args.FK_Flow
                    && a.WorkID == workid).FirstOrDefault();

                if (track != null)
                {
                    string[] lwOIDArray = track.LWZXOID.Split(',');
                    List<Process_LunWen> lwList = new List<Process_LunWen>();

                    for (int i = 0; i < lwOIDArray.Length; i++)
                    {
                        int lwOID = Convert.ToInt32(lwOIDArray[i]);
                        Process_LunWen lw = this.LoadEntities(a => a.OID == lwOID).FirstOrDefault();

                        SetProposerName(lw);

                        if (lw != null) lwList.Add(lw);
                    }

                    if (lwList.Count != 0)
                    {
                        return lwList;
                    }
                }
            }
            catch (Exception e)
            {
                return new List<Process_LunWen>();
            }
            return new List<Process_LunWen>();
        }
    }
}