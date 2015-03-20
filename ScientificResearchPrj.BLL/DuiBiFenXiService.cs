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
    public class DuiBiFenXiService : BaseService<Process_DuiBiFenXi>, IDuiBiFenXiService
    {
        public override void SetCurrentDAL()
        {
            CurrentDAL = this.DbSession.DuiBiFenXiDAL;
        }

        public List<Process_DuiBiFenXi> GetHistoryData(CCFlowArgs args)
        {
            string fk_node = args.FK_Node.ToString();
            string workid = args.WorkID.ToString();

            List<Process_DuiBiFenXi> dbfxList = this.LoadEntities(
                a => a.Process_BasicData.FK_Flow == args.FK_Flow &&
                a.Process_BasicData.FK_Node == fk_node &&
                a.Process_BasicData.WorkId == workid).ToList();
            if (dbfxList != null && dbfxList.Count != 0)
            {
                for (int i = 0; i < dbfxList.Count; i++)
                {
                    SetProposerName(dbfxList[i]);
                    SetShiYanName(dbfxList[i]);
                }
                return dbfxList;
            }
            return new List<Process_DuiBiFenXi>();
        }

        private void SetProposerName(Process_DuiBiFenXi dbfx)
        {
            if (dbfx.Process_BasicData.FK_Proposer == null) return;

            MyPort_Emp proposer = this.DbSession.EmpDAL.LoadEntities(e => e.EmpNo.Equals(dbfx.Process_BasicData.FK_Proposer)).FirstOrDefault();
            if (proposer != null)
            {
                dbfx.Process_BasicData.ProposerName = proposer.Name;
            }
        }

        private void SetShiYanName(Process_DuiBiFenXi dbfx)
        {
            if (dbfx.FK_SYOID == null) return;

            Process_ShiYan shiyan = this.DbSession.SheJiShiYanDAL.LoadEntities(a => a.OID == dbfx.FK_SYOID).FirstOrDefault();
            if (shiyan != null)
            {
                dbfx.FK_SYName = shiyan.Process_BasicData.Name;
            }
        }

        public void InsertOrUpdateTrack(CCFlowArgs args)
        {
            string workid = args.WorkID.ToString();
            Process_Track ifExist = this.DbSession.TrackDAL.LoadEntities(a => a.FK_Flow == args.FK_Flow
                && a.WorkID == workid).FirstOrDefault();

            List<Process_DuiBiFenXi> dbfxList = this.LoadEntities(a => a.Process_BasicData.FK_Flow == args.FK_Flow
                && a.Process_BasicData.WorkId == workid).ToList();

            if (ifExist != null)
            {
                if (dbfxList != null && dbfxList.Count != 0)
                {
                    ifExist.DBFXOID = CombindOIDStr(dbfxList);
                }

                this.DbSession.TrackDAL.UpdateEntity(ifExist);
                this.DbSession.SaveChanges();
            }
            else
            {
                ifExist = new Process_Track();
                if (dbfxList != null && dbfxList.Count != 0)
                {
                    ifExist.DBFXOID = CombindOIDStr(dbfxList);
                }

                ifExist.FK_Flow = args.FK_Flow;
                ifExist.WorkID = workid;

                this.DbSession.TrackDAL.AddEntity(ifExist);
                this.DbSession.SaveChanges();
            }
        }

        private string CombindOIDStr(List<Process_DuiBiFenXi> dbfxList)
        {
            string dbfxOIDStr = "";
            for (int i = 0; i < dbfxList.Count - 1; i++)
            {
                dbfxOIDStr += dbfxList[i].OID.ToString() + ",";
            }
            dbfxOIDStr += dbfxList[dbfxList.Count - 1].OID.ToString();

            return dbfxOIDStr;
        }

        public Dictionary<string, string> AddDuiBiFenXi(Process_DuiBiFenXi duibifenxi)
        {
            try
            {
                Process_DuiBiFenXi ifIdxHasExist = this.LoadEntities(a => a.FK_BDNo == duibifenxi.Process_BasicData.No).FirstOrDefault();
                if (ifIdxHasExist != null) throw new Exception("量化对比分析【" + duibifenxi.Process_BasicData.No + "】编号已存在");

                duibifenxi.OID = (CurrentDAL as DuiBiFenXiDAL).SelectMaxOid() + 1;
                duibifenxi.FK_BDNo = duibifenxi.Process_BasicData.No;

                this.AddEntity(duibifenxi);

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

        public Dictionary<string, string> DeleteDuiBiFenXi(string dbfxNo)
        {
            try
            {
                DeleteDuiBiFenXiWithoutUpdateColumn(dbfxNo);

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

        private void DeleteDuiBiFenXiWithoutUpdateColumn(string dbfxNo)
        {
            Process_DuiBiFenXi dbfx = this.LoadEntities(a => a.FK_BDNo == dbfxNo).FirstOrDefault();
            if (dbfx != null)
            {
                this.DeleteEntity(dbfx);
            }

            Process_BasicData basicData = this.DbSession.BasicDataDAL.LoadEntities(a => a.No == dbfxNo).FirstOrDefault();
            if (basicData != null)
            {
                this.DbSession.BasicDataDAL.DeleteEntity(basicData);
                this.DbSession.SaveChanges();
            }
        }

        public Dictionary<string, string> ModifyDuiBiFenXi(string oldNo, Process_DuiBiFenXi duibifenxi)
        {
            try
            {
                if (!oldNo.Equals(duibifenxi.Process_BasicData.No))
                {
                    Process_DuiBiFenXi ifIdxHasExist = this.LoadEntities(a => a.FK_BDNo == duibifenxi.Process_BasicData.No).FirstOrDefault();
                    if (ifIdxHasExist != null) throw new Exception("量化对比分析【" + duibifenxi.Process_BasicData.No + "】编号已存在");

                    this.AddDuiBiFenXi(duibifenxi);
                }
                else
                {
                    Process_DuiBiFenXi dbfxDesc = this.LoadEntities(a => a.FK_BDNo == oldNo).FirstOrDefault();
                    if (dbfxDesc != null)
                    {
                        ModifyDuiBiFenXi(dbfxDesc, duibifenxi);
                        this.UpdateEntity(dbfxDesc);
                    }
                }

                //使用OID作为外键，没有修改编号的话，不改变OID，无需修改外键关系
                if (!oldNo.Equals(duibifenxi.Process_BasicData.No))
                {
                    Process_DuiBiFenXi oldDbfx = this.LoadEntities(a => a.FK_BDNo == oldNo).FirstOrDefault();
                    Process_DuiBiFenXi newDbfx = this.LoadEntities(a => a.FK_BDNo == duibifenxi.Process_BasicData.No).FirstOrDefault();

                    //更新轨迹表的关联关系
                    (CurrentDAL as DuiBiFenXiDAL).UpdateTrackDBFXOID(oldDbfx.OID, newDbfx.OID);

                    this.DeleteDuiBiFenXiWithoutUpdateColumn(oldNo);
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

        private void ModifyDuiBiFenXi(Process_DuiBiFenXi dbfxDesc, Process_DuiBiFenXi dbfxSource)
        {
            dbfxDesc.OID = dbfxDesc.OID;

            dbfxDesc.FK_BDNo = dbfxSource.Process_BasicData.No;
            dbfxDesc.Data = dbfxSource.Data;
            dbfxDesc.Methods = dbfxSource.Methods;
            dbfxDesc.AnalysisResult = dbfxSource.AnalysisResult;
            dbfxDesc.InferType = dbfxSource.InferType;
            dbfxDesc.InferContent = dbfxSource.InferContent;
            dbfxDesc.FK_SYOID = dbfxSource.FK_SYOID;

            dbfxDesc.Process_BasicData.No = dbfxSource.Process_BasicData.No;
            dbfxDesc.Process_BasicData.Name = dbfxSource.Process_BasicData.Name;
            dbfxDesc.Process_BasicData.FK_Proposer = dbfxSource.Process_BasicData.FK_Proposer;
            dbfxDesc.Process_BasicData.ProposeTime = dbfxSource.Process_BasicData.ProposeTime;
            dbfxDesc.Process_BasicData.Description = dbfxSource.Process_BasicData.Description;
            dbfxDesc.Process_BasicData.Keys = dbfxSource.Process_BasicData.Keys;
            dbfxDesc.Process_BasicData.Remarks = dbfxSource.Process_BasicData.Remarks;
            dbfxDesc.Process_BasicData.ModifyTime = dbfxSource.Process_BasicData.ModifyTime;

            if (dbfxSource.Process_BasicData.FK_Flow != null)
            {
                dbfxDesc.Process_BasicData.FK_Flow = dbfxSource.Process_BasicData.FK_Flow;
            }
            else
            {
                dbfxDesc.Process_BasicData.FK_Flow = dbfxDesc.Process_BasicData.FK_Flow;
            }

            if (dbfxSource.Process_BasicData.FK_Node != null)
            {
                dbfxDesc.Process_BasicData.FK_Node = dbfxSource.Process_BasicData.FK_Node;
            }
            else
            {
                dbfxDesc.Process_BasicData.FK_Node = dbfxDesc.Process_BasicData.FK_Node;
            }

            if (dbfxSource.Process_BasicData.WorkId != null)
            {
                dbfxDesc.Process_BasicData.WorkId = dbfxSource.Process_BasicData.WorkId;
            }
            else
            {
                dbfxDesc.Process_BasicData.WorkId = dbfxDesc.Process_BasicData.WorkId;
            }
        }

        public List<Process_DuiBiFenXi> GetDuiBiFenXiHistoryDataFromTrack(CCFlowArgs args)
        {
            try
            {
                string workid = args.WorkID.ToString();
                Process_Track track = this.DbSession.TrackDAL.LoadEntities(a => a.FK_Flow == args.FK_Flow
                    && a.WorkID == workid).FirstOrDefault();

                if (track != null)
                {
                    string[] dbfxOIDArray = track.DBFXOID.Split(',');
                    List<Process_DuiBiFenXi> dbfxList = new List<Process_DuiBiFenXi>();

                    for (int i = 0; i < dbfxOIDArray.Length; i++)
                    {
                        int dbfxOID = Convert.ToInt32(dbfxOIDArray[i]);
                        Process_DuiBiFenXi dbfx = this.LoadEntities(a => a.OID == dbfxOID).FirstOrDefault();

                        SetProposerName(dbfx);
                        SetShiYanName(dbfx);

                        if (dbfx != null) dbfxList.Add(dbfx);
                    }

                    if (dbfxList.Count != 0)
                    {
                        return dbfxList;
                    }
                }
            }
            catch (Exception e)
            {
                return new List<Process_DuiBiFenXi>();
            }
            return new List<Process_DuiBiFenXi>();
        }
    }
}