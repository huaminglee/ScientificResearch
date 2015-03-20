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
    public class TiChuWenTiService : BaseService<Process_TiChuWenTi>, ITiChuWenTiService
    {
        public override void SetCurrentDAL()
        {
            CurrentDAL = this.DbSession.TiChuWenTiDAL;
        }

        public List<Process_TiChuWenTi> GetHistoryData(CCFlowArgs args) {
            string fk_node = args.FK_Node.ToString();
            string workid = args.WorkID.ToString();

            List<Process_TiChuWenTi> wtList = this.LoadEntities(
                a => a.Process_BasicData.FK_Flow == args.FK_Flow &&
                a.Process_BasicData.FK_Node == fk_node &&
                a.Process_BasicData.WorkId == workid).ToList();
            if (wtList != null && wtList.Count != 0)
            {
                for (int i = 0; i < wtList.Count; i++)
                {
                    SetProposerName(wtList[i]);
                }
                return wtList;
            }
            return new List<Process_TiChuWenTi>();
        }

        private void SetProposerName(Process_TiChuWenTi wenti)
        {
            if (wenti.Process_BasicData.FK_Proposer == null) return;

            MyPort_Emp proposer = this.DbSession.EmpDAL.LoadEntities(e => e.EmpNo.Equals(wenti.Process_BasicData.FK_Proposer)).FirstOrDefault();
            if (proposer != null)
            {
                wenti.Process_BasicData.ProposerName = proposer.Name;
            }
        }

        public void InsertOrUpdateTrack(CCFlowArgs args) {
            string workid = args.WorkID.ToString();
            Process_Track ifExist = this.DbSession.TrackDAL.LoadEntities(a => a.FK_Flow == args.FK_Flow
                && a.WorkID == workid).FirstOrDefault();

            List<Process_TiChuWenTi> wentiList = this.LoadEntities(a => a.Process_BasicData.FK_Flow == args.FK_Flow
                && a.Process_BasicData.WorkId == workid).ToList();

            if (ifExist != null)
            {
                if (wentiList != null && wentiList.Count != 0)
                {
                    ifExist.TCWTOID = CombindOIDStr(wentiList);
                }

                this.DbSession.TrackDAL.UpdateEntity(ifExist);
                this.DbSession.SaveChanges();
            }
            else
            {
                ifExist = new Process_Track();
                if (wentiList != null && wentiList.Count != 0)
                {
                    ifExist.TCWTOID = CombindOIDStr(wentiList);
                }

                ifExist.FK_Flow = args.FK_Flow;
                ifExist.WorkID = workid;

                this.DbSession.TrackDAL.AddEntity(ifExist);
                this.DbSession.SaveChanges();
            }
        }

        private string CombindOIDStr(List<Process_TiChuWenTi> wentiList)
        {
            string wtOIDStr = "";
            for (int i = 0; i < wentiList.Count - 1; i++)
            {
                wtOIDStr += wentiList[i].OID.ToString() + ",";
            }
            wtOIDStr += wentiList[wentiList.Count - 1].OID.ToString();

            return wtOIDStr;
        }

        public Dictionary<string, string> AddWenTi(Process_TiChuWenTi wenti) {
            try
            {
                Process_TiChuWenTi ifIdxHasExist = this.LoadEntities(a => a.FK_BDNo == wenti.Process_BasicData.No).FirstOrDefault();
                if (ifIdxHasExist != null) throw new Exception("问题【" + wenti.Process_BasicData.No + "】编号已存在");

                wenti.OID = (CurrentDAL as TiChuWenTiDAL).SelectMaxOid() + 1;
                wenti.FK_BDNo = wenti.Process_BasicData.No;

                this.AddEntity(wenti);

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

        public Dictionary<string, string> DeleteWenTi(string wtNo) {
            try
            {
                DeleteWenTiWithoutUpdateColumn(wtNo);

                /// //更新轨迹表的关联关系
                //ToDo---------------------------------------------------------------------
                //更新解决思路表的外键关系

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

        private void DeleteWenTiWithoutUpdateColumn(string wtNo)
        {
            Process_TiChuWenTi wenti = this.LoadEntities(a => a.FK_BDNo == wtNo).FirstOrDefault();
            if (wenti != null)
            {
                this.DeleteEntity(wenti);
            }

            Process_BasicData basicData = this.DbSession.BasicDataDAL.LoadEntities(a => a.No == wtNo).FirstOrDefault();
            if (basicData != null)
            {
                this.DbSession.BasicDataDAL.DeleteEntity(basicData);
                this.DbSession.SaveChanges();
            }
        }

        public Dictionary<string, string> ModifyWenTi(string oldNo, Process_TiChuWenTi wenti) {
            try
            {
                if (!oldNo.Equals(wenti.Process_BasicData.No))
                {
                    Process_TiChuWenTi ifIdxHasExist = this.LoadEntities(a => a.FK_BDNo == wenti.Process_BasicData.No).FirstOrDefault();
                    if (ifIdxHasExist != null) throw new Exception("问题【" + wenti.Process_BasicData.No + "】编号已存在");

                    this.AddWenTi(wenti);
                }
                else
                {
                    Process_TiChuWenTi wtDesc = this.LoadEntities(a => a.FK_BDNo == oldNo).FirstOrDefault();
                    if (wtDesc != null)
                    {
                        ModifyWenTi(wtDesc, wenti);
                        this.UpdateEntity(wtDesc);
                    }
                }

                //使用OID作为外键，没有修改编号的话，不改变OID，无需修改外键关系
                if (!oldNo.Equals(wenti.Process_BasicData.No))
                {
                    Process_TiChuWenTi oldWenTi = this.LoadEntities(a => a.FK_BDNo == oldNo).FirstOrDefault();
                    Process_TiChuWenTi newWenTi = this.LoadEntities(a => a.FK_BDNo == wenti.Process_BasicData.No).FirstOrDefault();

                    //更新轨迹表的关联关系
                    (CurrentDAL as TiChuWenTiDAL).UpdateTrackWTOID(oldWenTi.OID, newWenTi.OID);
                    //更新解决思路表的外键关系
                    (CurrentDAL as TiChuWenTiDAL).UpdateSiLuWTOID(oldWenTi.OID, newWenTi.OID);

                    this.DeleteWenTiWithoutUpdateColumn(oldNo);
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

        private void ModifyWenTi(Process_TiChuWenTi wentiDesc, Process_TiChuWenTi wentiSource)
        {
            wentiDesc.OID = wentiDesc.OID;

            wentiDesc.FK_BDNo = wentiSource.Process_BasicData.No;
            wentiDesc.Mitigation = wentiSource.Mitigation;
            wentiDesc.OvercomeMethod = wentiSource.OvercomeMethod;
            wentiDesc.Argument = wentiSource.Argument;

            wentiDesc.Process_BasicData.No = wentiSource.Process_BasicData.No;
            wentiDesc.Process_BasicData.Name = wentiSource.Process_BasicData.Name;
            wentiDesc.Process_BasicData.FK_Proposer = wentiSource.Process_BasicData.FK_Proposer;
            wentiDesc.Process_BasicData.ProposeTime = wentiSource.Process_BasicData.ProposeTime;
            wentiDesc.Process_BasicData.Description = wentiSource.Process_BasicData.Description;
            wentiDesc.Process_BasicData.Keys = wentiSource.Process_BasicData.Keys;
            wentiDesc.Process_BasicData.Remarks = wentiSource.Process_BasicData.Remarks;
            wentiDesc.Process_BasicData.ModifyTime = wentiSource.Process_BasicData.ModifyTime;

            if (wentiSource.Process_BasicData.FK_Flow != null)
            {
                wentiDesc.Process_BasicData.FK_Flow = wentiSource.Process_BasicData.FK_Flow;
            }
            else
            {
                wentiDesc.Process_BasicData.FK_Flow = wentiDesc.Process_BasicData.FK_Flow;
            }

            if (wentiSource.Process_BasicData.FK_Node != null)
            {
                wentiDesc.Process_BasicData.FK_Node = wentiSource.Process_BasicData.FK_Node;
            }
            else
            {
                wentiDesc.Process_BasicData.FK_Node = wentiDesc.Process_BasicData.FK_Node;
            }

            if (wentiSource.Process_BasicData.WorkId != null)
            {
                wentiDesc.Process_BasicData.WorkId = wentiSource.Process_BasicData.WorkId;
            }
            else
            {
                wentiDesc.Process_BasicData.WorkId = wentiDesc.Process_BasicData.WorkId;
            }
        }

        public List<Process_TiChuWenTi> GetTiChuWenTiHistoryDataFromTrack(CCFlowArgs args) {
            try
            {
                string workid = args.WorkID.ToString();
                Process_Track track = this.DbSession.TrackDAL.LoadEntities(a => a.FK_Flow == args.FK_Flow
                    && a.WorkID == workid).FirstOrDefault();

                if (track != null)
                {
                    string[] wtOIDArray = track.TCWTOID.Split(',');
                    List<Process_TiChuWenTi> wtList = new List<Process_TiChuWenTi>();

                    for (int i = 0; i < wtOIDArray.Length; i++)
                    {
                        int wtOID = Convert.ToInt32(wtOIDArray[i]);
                        Process_TiChuWenTi wt = this.LoadEntities(a => a.OID == wtOID).FirstOrDefault();

                        SetProposerName(wt);

                        if (wt != null) wtList.Add(wt);
                    }

                    if (wtList.Count != 0)
                    {
                        return wtList;
                    }
                }
            }
            catch (Exception e)
            {
                return new List<Process_TiChuWenTi>();
            }
            return new List<Process_TiChuWenTi>();
        }
    }
}