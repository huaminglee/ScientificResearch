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
    public class SheJiShiYanService : BaseService<Process_ShiYan>, ISheJiShiYanService
    {
        public override void SetCurrentDAL()
        {
            CurrentDAL = this.DbSession.SheJiShiYanDAL;
        }

        public  List<Process_ShiYan> GetHistoryData(CCFlowArgs args)
        {
            string fk_node = args.FK_Node.ToString();
            string workid = args.WorkID.ToString();

            List<Process_ShiYan> syList = this.LoadEntities(
                a => a.Process_BasicData.FK_Flow == args.FK_Flow &&
                a.Process_BasicData.FK_Node == fk_node &&
                a.Process_BasicData.WorkId == workid).ToList();
            if (syList != null && syList.Count != 0)
            {
                for (int i = 0; i < syList.Count; i++)
                {
                    SetProposerName(syList[i]);
                    SetSuanFaName(syList[i]);
                }
                return syList;
            }
            return new List<Process_ShiYan>();
        }

        private void SetProposerName(Process_ShiYan shiyan)
        {
            if (shiyan.Process_BasicData.FK_Proposer == null) return;

            MyPort_Emp proposer = this.DbSession.EmpDAL.LoadEntities(e => e.EmpNo.Equals(shiyan.Process_BasicData.FK_Proposer)).FirstOrDefault();
            if (proposer != null)
            {
                shiyan.Process_BasicData.ProposerName = proposer.Name;
            }
        }

        private void SetSuanFaName(Process_ShiYan shiyan)
        {
            if (shiyan.FK_SFOID == null) return;

            Process_SuanFa suanfa = this.DbSession.SheJiSuanFaDAL.LoadEntities(a => a.OID == shiyan.FK_SFOID).FirstOrDefault();
            if (suanfa != null)
            {
                shiyan.FK_SFName = suanfa.Process_BasicData.Name;
            }
        }

        public void InsertOrUpdateTrack(CCFlowArgs args)
        {
            string workid = args.WorkID.ToString();
            Process_Track ifExist = this.DbSession.TrackDAL.LoadEntities(a => a.FK_Flow == args.FK_Flow
                && a.WorkID == workid).FirstOrDefault();

            List<Process_ShiYan> shiyanList = this.LoadEntities(a => a.Process_BasicData.FK_Flow == args.FK_Flow
                && a.Process_BasicData.WorkId == workid).ToList();

            if (ifExist != null)
            {
                if (shiyanList != null && shiyanList.Count != 0)
                {
                    ifExist.SJSYOID = CombindOIDStr(shiyanList);
                }

                this.DbSession.TrackDAL.UpdateEntity(ifExist);
                this.DbSession.SaveChanges();
            }
            else
            {
                ifExist = new Process_Track();
                if (shiyanList != null && shiyanList.Count != 0)
                {
                    ifExist.SJSYOID = CombindOIDStr(shiyanList);
                }

                ifExist.FK_Flow = args.FK_Flow;
                ifExist.WorkID = workid;

                this.DbSession.TrackDAL.AddEntity(ifExist);
                this.DbSession.SaveChanges();
            }
        }

        private string CombindOIDStr(List<Process_ShiYan> shiyanList)
        {
            string syOIDStr = "";
            for (int i = 0; i < shiyanList.Count - 1; i++)
            {
                syOIDStr += shiyanList[i].OID.ToString() + ",";
            }
            syOIDStr += shiyanList[shiyanList.Count - 1].OID.ToString();

            return syOIDStr;
        }

        public Dictionary<string, string> AddShiYan(Process_ShiYan shiyan)
        {
            try
            {
                Process_ShiYan ifIdxHasExist = this.LoadEntities(a => a.FK_BDNo == shiyan.Process_BasicData.No).FirstOrDefault();
                if (ifIdxHasExist != null) throw new Exception("实验【" + shiyan.Process_BasicData.No + "】编号已存在");

                shiyan.OID = (CurrentDAL as SheJiShiYanDAL).SelectMaxOid() + 1;
                shiyan.FK_BDNo = shiyan.Process_BasicData.No;

                this.AddEntity(shiyan);

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

        public Dictionary<string, string> DeleteShiYan(string syNo)
        {
            try
            {
                DeleteShiYanWithoutUpdateColumn(syNo);

                /// //更新轨迹表的关联关系
                //ToDo---------------------------------------------------------------------
                //更新对比分析表的外键关系

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

        private void DeleteShiYanWithoutUpdateColumn(string syNo)
        {
            Process_ShiYan shiyan = this.LoadEntities(a => a.FK_BDNo == syNo).FirstOrDefault();
            if (shiyan != null)
            {
                this.DeleteEntity(shiyan);
            }

            Process_BasicData basicData = this.DbSession.BasicDataDAL.LoadEntities(a => a.No == syNo).FirstOrDefault();
            if (basicData != null)
            {
                this.DbSession.BasicDataDAL.DeleteEntity(basicData);
                this.DbSession.SaveChanges();
            }
        }

        public Dictionary<string, string> ModifyShiYan(string oldNo, Process_ShiYan shiyan)
        {
            try
            {
                if (!oldNo.Equals(shiyan.Process_BasicData.No))
                {
                    Process_ShiYan ifIdxHasExist = this.LoadEntities(a => a.FK_BDNo == shiyan.Process_BasicData.No).FirstOrDefault();
                    if (ifIdxHasExist != null) throw new Exception("实验【" + shiyan.Process_BasicData.No + "】编号已存在");

                    this.AddShiYan(shiyan);
                }
                else
                {
                    Process_ShiYan syDesc = this.LoadEntities(a => a.FK_BDNo == oldNo).FirstOrDefault();
                    if (syDesc != null)
                    {
                        ModifyShiYan(syDesc, shiyan);
                        this.UpdateEntity(syDesc);
                    }
                }

                //使用OID作为外键，没有修改编号的话，不改变OID，无需修改外键关系
                if (!oldNo.Equals(shiyan.Process_BasicData.No))
                {
                    Process_ShiYan oldShiYan = this.LoadEntities(a => a.FK_BDNo == oldNo).FirstOrDefault();
                    Process_ShiYan newShiYan = this.LoadEntities(a => a.FK_BDNo == shiyan.Process_BasicData.No).FirstOrDefault();

                    //更新轨迹表的关联关系
                    (CurrentDAL as SheJiShiYanDAL).UpdateTrackSJSYOID(oldShiYan.OID, newShiYan.OID);
                    //更新对比分析表的外键关系
                    (CurrentDAL as SheJiShiYanDAL).UpdateDuiBiFenXiSYOID(oldShiYan.OID, newShiYan.OID);

                    this.DeleteShiYanWithoutUpdateColumn(oldNo);
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

        private void ModifyShiYan(Process_ShiYan shiyanDesc, Process_ShiYan shiyanSource)
        {
            shiyanDesc.OID = shiyanDesc.OID;

            shiyanDesc.FK_BDNo = shiyanSource.Process_BasicData.No;
            shiyanDesc.Design = shiyanSource.Design;
            shiyanDesc.IndexSys = shiyanSource.IndexSys;
            shiyanDesc.RealizeStep = shiyanSource.RealizeStep;
            shiyanDesc.TestCondition = shiyanSource.TestCondition;
            shiyanDesc.Data = shiyanSource.Data;
            shiyanDesc.StatistacalResult = shiyanSource.StatistacalResult;
            shiyanDesc.Result = shiyanSource.Result;
            shiyanDesc.FK_SFOID = shiyanSource.FK_SFOID;

            shiyanDesc.Process_BasicData.No = shiyanSource.Process_BasicData.No;
            shiyanDesc.Process_BasicData.Name = shiyanSource.Process_BasicData.Name;
            shiyanDesc.Process_BasicData.FK_Proposer = shiyanSource.Process_BasicData.FK_Proposer;
            shiyanDesc.Process_BasicData.ProposeTime = shiyanSource.Process_BasicData.ProposeTime;
            shiyanDesc.Process_BasicData.Description = shiyanSource.Process_BasicData.Description;
            shiyanDesc.Process_BasicData.Keys = shiyanSource.Process_BasicData.Keys;
            shiyanDesc.Process_BasicData.Remarks = shiyanSource.Process_BasicData.Remarks;
            shiyanDesc.Process_BasicData.ModifyTime = shiyanSource.Process_BasicData.ModifyTime;

            if (shiyanSource.Process_BasicData.FK_Flow != null)
            {
                shiyanDesc.Process_BasicData.FK_Flow = shiyanSource.Process_BasicData.FK_Flow;
            }
            else
            {
                shiyanDesc.Process_BasicData.FK_Flow = shiyanDesc.Process_BasicData.FK_Flow;
            }

            if (shiyanSource.Process_BasicData.FK_Node != null)
            {
                shiyanDesc.Process_BasicData.FK_Node = shiyanSource.Process_BasicData.FK_Node;
            }
            else
            {
                shiyanDesc.Process_BasicData.FK_Node = shiyanDesc.Process_BasicData.FK_Node;
            }

            if (shiyanSource.Process_BasicData.WorkId != null)
            {
                shiyanDesc.Process_BasicData.WorkId = shiyanSource.Process_BasicData.WorkId;
            }
            else
            {
                shiyanDesc.Process_BasicData.WorkId = shiyanDesc.Process_BasicData.WorkId;
            }
        }

        public List<Process_ShiYan> GetShiYanHistoryDataFromTrack(CCFlowArgs args)
        {
            try
            {
                string workid = args.WorkID.ToString();
                Process_Track track = this.DbSession.TrackDAL.LoadEntities(a => a.FK_Flow == args.FK_Flow
                    && a.WorkID == workid).FirstOrDefault();

                if (track != null)
                {
                    string[] syOIDArray = track.SJSYOID.Split(',');
                    List<Process_ShiYan> syList = new List<Process_ShiYan>();

                    for (int i = 0; i < syOIDArray.Length; i++)
                    {
                        int syOID = Convert.ToInt32(syOIDArray[i]);
                        Process_ShiYan sy = this.LoadEntities(a => a.OID == syOID).FirstOrDefault();

                        SetProposerName(sy);
                        SetSuanFaName(sy);

                        if (sy != null) syList.Add(sy);
                    }

                    if (syList.Count != 0)
                    {
                        return syList;
                    }
                }
            }
            catch (Exception e)
            {
                return new List<Process_ShiYan>();
            }
            return new List<Process_ShiYan>();
        }
    }
}