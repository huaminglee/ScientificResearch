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
    public class DiaoYanService : BaseService<Process_DiaoYan>, IDiaoYanService
    {
        public override void SetCurrentDAL()
        {
            CurrentDAL = this.DbSession.DiaoYanDAL;
        }

        public List<Process_DiaoYan> GetHistoryData(CCFlowArgs args)
        {
            string fk_node = args.FK_Node.ToString();
            string workid = args.WorkID.ToString();

            List<Process_DiaoYan> diaoyanList = this.LoadEntities(
                a => a.Process_BasicData.FK_Flow == args.FK_Flow &&
                a.Process_BasicData.FK_Node == fk_node &&
                a.Process_BasicData.WorkId == workid).ToList();
            if (diaoyanList != null && diaoyanList.Count!=0)
            {
                for (int i = 0; i < diaoyanList.Count; i++) {
                    SetProposerName(diaoyanList[i]);
                }
                return diaoyanList;
            }
            return new List<Process_DiaoYan>();
        }

        private void SetProposerName(Process_DiaoYan diaoyan)
        {
            if (diaoyan.Process_BasicData.FK_Proposer == null) return;

            MyPort_Emp proposer = this.DbSession.EmpDAL.LoadEntities(e => e.EmpNo.Equals(diaoyan.Process_BasicData.FK_Proposer)).FirstOrDefault();
            if (proposer != null)
            {
                diaoyan.Process_BasicData.ProposerName = proposer.Name;
            }
        }
          
        public void InsertOrUpdateTrack(CCFlowArgs args)
        {
            string workid = args.WorkID.ToString();
            Process_Track ifExist = this.DbSession.TrackDAL.LoadEntities(a => a.FK_Flow == args.FK_Flow
                && a.WorkID == workid).FirstOrDefault();

            List<Process_DiaoYan> diaoyanList = this.LoadEntities(a => a.Process_BasicData.FK_Flow == args.FK_Flow
                && a.Process_BasicData.WorkId == workid).ToList();
            
            if (ifExist != null)
            {
                if (diaoyanList != null && diaoyanList.Count != 0)
                {
                    ifExist.DYOID = CombindOIDStr(diaoyanList);
                }

                this.DbSession.TrackDAL.UpdateEntity(ifExist);
                this.DbSession.SaveChanges();
            }
            else
            {
                ifExist = new Process_Track();
                if (diaoyanList != null && diaoyanList.Count != 0)
                {
                    ifExist.DYOID = CombindOIDStr(diaoyanList);
                }
                 
                ifExist.FK_Flow = args.FK_Flow;
                ifExist.WorkID = workid;

                this.DbSession.TrackDAL.AddEntity(ifExist);
                this.DbSession.SaveChanges();
            }
        }

        private string CombindOIDStr(List<Process_DiaoYan> diaoyanList)
        {
            string dyOIDStr = "";
            for (int i = 0; i < diaoyanList.Count - 1; i++)
            {
                dyOIDStr += diaoyanList[i].OID.ToString() + ",";
            }
            dyOIDStr += diaoyanList[diaoyanList.Count - 1].OID.ToString();

            return dyOIDStr;
        }
        
        public Dictionary<string, string> AddDiaoYan(Process_DiaoYan diaoyan) {
            try
            {
                Process_DiaoYan ifIdxHasExist = this.LoadEntities(a => a.FK_BDNo == diaoyan.Process_BasicData.No).FirstOrDefault();
                if (ifIdxHasExist != null) throw new Exception("调研【" + diaoyan.Process_BasicData.No + "】编号已存在");

                diaoyan.OID = (CurrentDAL as DiaoYanDAL).SelectMaxOid() + 1;
                diaoyan.FK_BDNo = diaoyan.Process_BasicData.No;

                this.AddEntity(diaoyan);

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

        public Dictionary<string, string> DeleteDiaoYan(string dyNo) {
            try
            {
                DeleteDiaoYanWithoutUpdateColumn(dyNo);

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

        private void DeleteDiaoYanWithoutUpdateColumn(string dyNo)
        {
            Process_DiaoYan diaoyan = this.LoadEntities(a => a.FK_BDNo == dyNo).FirstOrDefault();
            if (diaoyan != null)
            {
                this.DeleteEntity(diaoyan);
            }

            Process_BasicData basicData = this.DbSession.BasicDataDAL.LoadEntities(a => a.No == dyNo).FirstOrDefault();
            if (basicData != null)
            {
                this.DbSession.BasicDataDAL.DeleteEntity(basicData);
                this.DbSession.SaveChanges();
            }
        }

        public Dictionary<string, string> ModifyDiaoYan(string oldNo, Process_DiaoYan diaoyan) {
            try
            {
                if (!oldNo.Equals(diaoyan.Process_BasicData.No))
                {
                    Process_DiaoYan ifIdxHasExist = this.LoadEntities(a => a.FK_BDNo == diaoyan.Process_BasicData.No).FirstOrDefault();
                    if (ifIdxHasExist != null) throw new Exception("调研【" + diaoyan.Process_BasicData.No + "】编号已存在");

                    this.AddDiaoYan(diaoyan);
                }
                else
                {
                    Process_DiaoYan dyDesc = this.LoadEntities(a => a.FK_BDNo == oldNo).FirstOrDefault();
                    if (dyDesc != null)
                    {
                        ModifyDiaoYan(dyDesc, diaoyan);
                        this.UpdateEntity(dyDesc);
                    }
                }

                //使用OID作为外键，没有修改编号的话，不改变OID，无需修改外键关系
                if (!oldNo.Equals(diaoyan.Process_BasicData.No))
                {
                    Process_DiaoYan oldDiaoYan = this.LoadEntities(a => a.FK_BDNo == oldNo).FirstOrDefault();
                    Process_DiaoYan newDiaoYan = this.LoadEntities(a => a.FK_BDNo == diaoyan.Process_BasicData.No).FirstOrDefault();

                    //更新轨迹表的关联关系
                    (CurrentDAL as DiaoYanDAL).UpdateTrackDYOID(oldDiaoYan.OID, newDiaoYan.OID);

                    this.DeleteDiaoYanWithoutUpdateColumn(oldNo);
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

        private void ModifyDiaoYan(Process_DiaoYan diaoyanDesc, Process_DiaoYan diaoyanSource)
        {
            diaoyanDesc.OID = diaoyanDesc.OID;

            diaoyanDesc.FK_BDNo = diaoyanSource.Process_BasicData.No;
            diaoyanDesc.SumType = diaoyanSource.SumType;
            diaoyanDesc.Sum = diaoyanSource.Sum;
            diaoyanDesc.SurveryAddr = diaoyanSource.SurveryAddr;
            diaoyanDesc.Investigator = diaoyanSource.Investigator;
            diaoyanDesc.AnalysisResult = diaoyanSource.AnalysisResult;
            diaoyanDesc.AdvantageValue = diaoyanSource.AdvantageValue;
            diaoyanDesc.WeaknessValue = diaoyanSource.WeaknessValue;
            diaoyanDesc.UnsolvedProblem = diaoyanSource.UnsolvedProblem;
            diaoyanDesc.TechTrends = diaoyanSource.TechTrends;
            diaoyanDesc.BeyondPoint = diaoyanSource.BeyondPoint;
            
            diaoyanDesc.Process_BasicData.No = diaoyanSource.Process_BasicData.No;
            diaoyanDesc.Process_BasicData.Name = diaoyanSource.Process_BasicData.Name;
            diaoyanDesc.Process_BasicData.FK_Proposer = diaoyanSource.Process_BasicData.FK_Proposer;
            diaoyanDesc.Process_BasicData.ProposeTime = diaoyanSource.Process_BasicData.ProposeTime;
            diaoyanDesc.Process_BasicData.Description = diaoyanSource.Process_BasicData.Description;
            diaoyanDesc.Process_BasicData.Keys = diaoyanSource.Process_BasicData.Keys;
            diaoyanDesc.Process_BasicData.Remarks = diaoyanSource.Process_BasicData.Remarks;
            diaoyanDesc.Process_BasicData.ModifyTime = diaoyanSource.Process_BasicData.ModifyTime;

            if (diaoyanSource.Process_BasicData.FK_Flow != null)
            {
                diaoyanDesc.Process_BasicData.FK_Flow = diaoyanSource.Process_BasicData.FK_Flow;
            }
            else
            {
                diaoyanDesc.Process_BasicData.FK_Flow = diaoyanDesc.Process_BasicData.FK_Flow;
            }

            if (diaoyanSource.Process_BasicData.FK_Node != null)
            {
                diaoyanDesc.Process_BasicData.FK_Node = diaoyanSource.Process_BasicData.FK_Node;
            }
            else
            {
                diaoyanDesc.Process_BasicData.FK_Node = diaoyanDesc.Process_BasicData.FK_Node;
            }

            if (diaoyanSource.Process_BasicData.WorkId != null)
            {
                diaoyanDesc.Process_BasicData.WorkId = diaoyanSource.Process_BasicData.WorkId;
            }
            else
            {
                diaoyanDesc.Process_BasicData.WorkId = diaoyanDesc.Process_BasicData.WorkId;
            }
        }

        public List<Process_DiaoYan> GetDiaoYanHistoryDataFromTrack(CCFlowArgs args) {
            try
            {
                string workid = args.WorkID.ToString();
                Process_Track track = this.DbSession.TrackDAL.LoadEntities(a => a.FK_Flow == args.FK_Flow
                    && a.WorkID == workid).FirstOrDefault();

                if (track != null)
                {
                    string[] dyOIDArray = track.DYOID.Split(',');
                    List<Process_DiaoYan> dyList = new List<Process_DiaoYan>();

                    for (int i = 0; i < dyOIDArray.Length; i++)
                    {
                        int dyOID = Convert.ToInt32(dyOIDArray[i]);
                        Process_DiaoYan dy = this.LoadEntities(a => a.OID == dyOID).FirstOrDefault();

                        SetProposerName(dy);

                        if (dy != null) dyList.Add(dy);
                    }

                    if (dyList.Count != 0)
                    {
                        return dyList;
                    }
                }
            }
            catch (Exception e)
            {
                return new List<Process_DiaoYan>();
            }
            return new List<Process_DiaoYan>();
        }
    }
}