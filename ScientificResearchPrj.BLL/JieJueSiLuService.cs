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
    public class JieJueSiLuService : BaseService<Process_SiLu>, IJieJueSiLuService
    {
        public override void SetCurrentDAL()
        {
            CurrentDAL = this.DbSession.JieJueSiLuDAL;
        }

        public List<Process_SiLu> GetHistoryData(CCFlowArgs args)
        {
            string fk_node = args.FK_Node.ToString();
            string workid = args.WorkID.ToString();

            List<Process_SiLu> slList = this.LoadEntities(
                a => a.Process_BasicData.FK_Flow == args.FK_Flow &&
                a.Process_BasicData.FK_Node == fk_node &&
                a.Process_BasicData.WorkId == workid).ToList();
            if (slList != null && slList.Count != 0)
            {
                for (int i = 0; i < slList.Count; i++)
                {
                    SetProposerName(slList[i]);
                    SetTiChuWenTiName(slList[i]);
                }
                return slList;
            }
            return new List<Process_SiLu>();
        }

        private void SetProposerName(Process_SiLu silu)
        {
            if (silu.Process_BasicData.FK_Proposer == null) return;

            MyPort_Emp proposer = this.DbSession.EmpDAL.LoadEntities(e => e.EmpNo.Equals(silu.Process_BasicData.FK_Proposer)).FirstOrDefault();
            if (proposer != null)
            {
                silu.Process_BasicData.ProposerName = proposer.Name;
            }
        }

        private void SetTiChuWenTiName(Process_SiLu silu)
        {
            if (silu.FK_WTOID == null) return;

            Process_TiChuWenTi wenti = this.DbSession.TiChuWenTiDAL.LoadEntities(a => a.OID == silu.FK_WTOID).FirstOrDefault();
            if (silu != null)
            {
                silu.FK_WTName = silu.Process_BasicData.Name;
            }
        }

        public void InsertOrUpdateTrack(CCFlowArgs args)
        {
            string workid = args.WorkID.ToString();
            Process_Track ifExist = this.DbSession.TrackDAL.LoadEntities(a => a.FK_Flow == args.FK_Flow
                && a.WorkID == workid).FirstOrDefault();

            List<Process_SiLu> siluList = this.LoadEntities(a => a.Process_BasicData.FK_Flow == args.FK_Flow
                && a.Process_BasicData.WorkId == workid).ToList();

            if (ifExist != null)
            {
                if (siluList != null && siluList.Count != 0)
                {
                    ifExist.JJSLOID = CombindOIDStr(siluList);
                }

                this.DbSession.TrackDAL.UpdateEntity(ifExist);
                this.DbSession.SaveChanges();
            }
            else
            {
                ifExist = new Process_Track();
                if (siluList != null && siluList.Count != 0)
                {
                    ifExist.JJSLOID = CombindOIDStr(siluList);
                }

                ifExist.FK_Flow = args.FK_Flow;
                ifExist.WorkID = workid;

                this.DbSession.TrackDAL.AddEntity(ifExist);
                this.DbSession.SaveChanges();
            }
        }

        private string CombindOIDStr(List<Process_SiLu> siluList)
        {
            string slOIDStr = "";
            for (int i = 0; i < siluList.Count - 1; i++)
            {
                slOIDStr += siluList[i].OID.ToString() + ",";
            }
            slOIDStr += siluList[siluList.Count - 1].OID.ToString();

            return slOIDStr;
        }

        public Dictionary<string, string> AddSiLu(Process_SiLu silu)
        {
            try
            {
                Process_SiLu ifIdxHasExist = this.LoadEntities(a => a.FK_BDNo == silu.Process_BasicData.No).FirstOrDefault();
                if (ifIdxHasExist != null) throw new Exception("解决思路【" + silu.Process_BasicData.No + "】编号已存在");

                silu.OID = (CurrentDAL as JieJueSiLuDAL).SelectMaxOid() + 1;
                silu.FK_BDNo = silu.Process_BasicData.No;

                this.AddEntity(silu);

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

        public Dictionary<string, string> DeleteSiLu(string slNo)
        {
            try
            {
                DeleteSiLuWithoutUpdateColumn(slNo);

                /// //更新轨迹表的关联关系
                //ToDo---------------------------------------------------------------------
                //更新形式化表的外键关系

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

        private void DeleteSiLuWithoutUpdateColumn(string slNo)
        {
            Process_SiLu silu = this.LoadEntities(a => a.FK_BDNo == slNo).FirstOrDefault();
            if (silu != null)
            {
                this.DeleteEntity(silu);
            }

            Process_BasicData basicData = this.DbSession.BasicDataDAL.LoadEntities(a => a.No == slNo).FirstOrDefault();
            if (basicData != null)
            {
                this.DbSession.BasicDataDAL.DeleteEntity(basicData);
                this.DbSession.SaveChanges();
            }
        }

        public Dictionary<string, string> ModifySiLu(string oldNo, Process_SiLu silu)
        {
            try
            {
                if (!oldNo.Equals(silu.Process_BasicData.No))
                {
                    Process_SiLu ifIdxHasExist = this.LoadEntities(a => a.FK_BDNo == silu.Process_BasicData.No).FirstOrDefault();
                    if (ifIdxHasExist != null) throw new Exception("解决思路【" + silu.Process_BasicData.No + "】编号已存在");

                    this.AddSiLu(silu);
                }
                else
                {
                    Process_SiLu slDesc = this.LoadEntities(a => a.FK_BDNo == oldNo).FirstOrDefault();
                    if (slDesc != null)
                    {
                        ModifySiLu(slDesc, silu);
                        this.UpdateEntity(slDesc);
                    }
                }

                //使用OID作为外键，没有修改编号的话，不改变OID，无需修改外键关系
                if (!oldNo.Equals(silu.Process_BasicData.No))
                {
                    Process_SiLu oldSiLu = this.LoadEntities(a => a.FK_BDNo == oldNo).FirstOrDefault();
                    Process_SiLu newSiLu = this.LoadEntities(a => a.FK_BDNo == silu.Process_BasicData.No).FirstOrDefault();

                    //更新轨迹表的关联关系
                    (CurrentDAL as JieJueSiLuDAL).UpdateTrackSLOID(oldSiLu.OID, newSiLu.OID);
                    //更新形式化表的外键关系
                    (CurrentDAL as JieJueSiLuDAL).UpdateXingShiHuaSLOID(oldSiLu.OID, newSiLu.OID);

                    this.DeleteSiLuWithoutUpdateColumn(oldNo);
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

        private void ModifySiLu(Process_SiLu siluDesc, Process_SiLu siluSource)
        {
            siluDesc.OID = siluDesc.OID;

            siluDesc.FK_BDNo = siluSource.Process_BasicData.No;
            siluDesc.Type = siluSource.Type;
            siluDesc.FK_WTOID = siluSource.FK_WTOID;

            siluDesc.Process_BasicData.No = siluSource.Process_BasicData.No;
            siluDesc.Process_BasicData.Name = siluSource.Process_BasicData.Name;
            siluDesc.Process_BasicData.FK_Proposer = siluSource.Process_BasicData.FK_Proposer;
            siluDesc.Process_BasicData.ProposeTime = siluSource.Process_BasicData.ProposeTime;
            siluDesc.Process_BasicData.Description = siluSource.Process_BasicData.Description;
            siluDesc.Process_BasicData.Keys = siluSource.Process_BasicData.Keys;
            siluDesc.Process_BasicData.Remarks = siluSource.Process_BasicData.Remarks;
            siluDesc.Process_BasicData.ModifyTime = siluSource.Process_BasicData.ModifyTime;

            if (siluSource.Process_BasicData.FK_Flow != null)
            {
                siluDesc.Process_BasicData.FK_Flow = siluSource.Process_BasicData.FK_Flow;
            }
            else
            {
                siluDesc.Process_BasicData.FK_Flow = siluDesc.Process_BasicData.FK_Flow;
            }

            if (siluSource.Process_BasicData.FK_Node != null)
            {
                siluDesc.Process_BasicData.FK_Node = siluSource.Process_BasicData.FK_Node;
            }
            else
            {
                siluDesc.Process_BasicData.FK_Node = siluDesc.Process_BasicData.FK_Node;
            }

            if (siluSource.Process_BasicData.WorkId != null)
            {
                siluDesc.Process_BasicData.WorkId = siluSource.Process_BasicData.WorkId;
            }
            else
            {
                siluDesc.Process_BasicData.WorkId = siluDesc.Process_BasicData.WorkId;
            }
        }

        public List<Process_SiLu> GetJieJueSiLuHistoryDataFromTrack(CCFlowArgs args)
        {
            try
            {
                string workid = args.WorkID.ToString();
                Process_Track track = this.DbSession.TrackDAL.LoadEntities(a => a.FK_Flow == args.FK_Flow
                    && a.WorkID == workid).FirstOrDefault();

                if (track != null)
                {
                    string[] slOIDArray = track.JJSLOID.Split(',');
                    List<Process_SiLu> slList = new List<Process_SiLu>();

                    for (int i = 0; i < slOIDArray.Length; i++)
                    {
                        int slOID = Convert.ToInt32(slOIDArray[i]);
                        Process_SiLu sl = this.LoadEntities(a => a.OID == slOID).FirstOrDefault();

                        SetProposerName(sl);
                        SetTiChuWenTiName(sl);

                        if (sl != null) slList.Add(sl);
                    }

                    if (slList.Count != 0)
                    {
                        return slList;
                    }
                }
            }
            catch (Exception e) { 
                return new List<Process_SiLu>(); 
            }
            return new List<Process_SiLu>();
        }
    }
}