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
    public class DeChuJieLunService : BaseService<Process_DeChuJieLun>, IDeChuJieLunService
    {
        public override void SetCurrentDAL()
        {
            CurrentDAL = this.DbSession.DeChuJieLunDAL;
        }

        public List<Process_DeChuJieLun> GetHistoryData(CCFlowArgs args)
        {
            string fk_node = args.FK_Node.ToString();
            string workid = args.WorkID.ToString();

            List<Process_DeChuJieLun> jlList = this.LoadEntities(
                a => a.Process_BasicData.FK_Flow == args.FK_Flow &&
                a.Process_BasicData.FK_Node == fk_node &&
                a.Process_BasicData.WorkId == workid).ToList();
            if (jlList != null && jlList.Count != 0)
            {
                for (int i = 0; i < jlList.Count; i++)
                {
                    SetProposerName(jlList[i]);
                    SetDuiBiFenXiName(jlList[i]);
                }
                return jlList;
            }
            return new List<Process_DeChuJieLun>();
        }

        private void SetProposerName(Process_DeChuJieLun jielun)
        {
            if (jielun.Process_BasicData.FK_Proposer == null) return;

            MyPort_Emp proposer = this.DbSession.EmpDAL.LoadEntities(e => e.EmpNo.Equals(jielun.Process_BasicData.FK_Proposer)).FirstOrDefault();
            if (proposer != null)
            {
                jielun.Process_BasicData.ProposerName = proposer.Name;
            }
        }

        private void SetDuiBiFenXiName(Process_DeChuJieLun jielun)
        {
            if (jielun.FK_DBFXOID == null) return;

            Process_DuiBiFenXi dbfx = this.DbSession.DuiBiFenXiDAL.LoadEntities(a => a.OID == jielun.FK_DBFXOID).FirstOrDefault();
            if (dbfx != null)
            {
                jielun.FK_DBFXName = dbfx.Process_BasicData.Name;
            }
        }

        public void InsertOrUpdateTrack(CCFlowArgs args)
        {
            string workid = args.WorkID.ToString();
            Process_Track ifExist = this.DbSession.TrackDAL.LoadEntities(a => a.FK_Flow == args.FK_Flow
                && a.WorkID == workid).FirstOrDefault();

            List<Process_DeChuJieLun> jlList = this.LoadEntities(a => a.Process_BasicData.FK_Flow == args.FK_Flow
                && a.Process_BasicData.WorkId == workid).ToList();

            if (ifExist != null)
            {
                if (jlList != null && jlList.Count != 0)
                {
                    ifExist.DCJLOID = CombindOIDStr(jlList);
                }

                this.DbSession.TrackDAL.UpdateEntity(ifExist);
                this.DbSession.SaveChanges();
            }
            else
            {
                ifExist = new Process_Track();
                if (jlList != null && jlList.Count != 0)
                {
                    ifExist.DCJLOID = CombindOIDStr(jlList);
                }

                ifExist.FK_Flow = args.FK_Flow;
                ifExist.WorkID = workid;

                this.DbSession.TrackDAL.AddEntity(ifExist);
                this.DbSession.SaveChanges();
            }
        }

        private string CombindOIDStr(List<Process_DeChuJieLun> jlList)
        {
            string jlOIDStr = "";
            for (int i = 0; i < jlList.Count - 1; i++)
            {
                jlOIDStr += jlList[i].OID.ToString() + ",";
            }
            jlOIDStr += jlList[jlList.Count - 1].OID.ToString();

            return jlOIDStr;
        }

        public Dictionary<string, string> AddJieLun(Process_DeChuJieLun jielun)
        {
            try
            {
                Process_DeChuJieLun ifIdxHasExist = this.LoadEntities(a => a.FK_BDNo == jielun.Process_BasicData.No).FirstOrDefault();
                if (ifIdxHasExist != null) throw new Exception("结论【" + jielun.Process_BasicData.No + "】编号已存在");

                jielun.OID = (CurrentDAL as DeChuJieLunDAL).SelectMaxOid() + 1;
                jielun.FK_BDNo = jielun.Process_BasicData.No;

                this.AddEntity(jielun);

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

        public Dictionary<string, string> DeleteJieLun(string jlNo)
        {
            try
            {
                DeleteJieLunWithoutUpdateColumn(jlNo);

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

        private void DeleteJieLunWithoutUpdateColumn(string jlNo)
        {
            Process_DeChuJieLun jielun = this.LoadEntities(a => a.FK_BDNo == jlNo).FirstOrDefault();
            if (jielun != null)
            {
                this.DeleteEntity(jielun);
            }

            Process_BasicData basicData = this.DbSession.BasicDataDAL.LoadEntities(a => a.No == jlNo).FirstOrDefault();
            if (basicData != null)
            {
                this.DbSession.BasicDataDAL.DeleteEntity(basicData);
                this.DbSession.SaveChanges();
            }
        }

        public Dictionary<string, string> ModifyJieLun(string oldNo, Process_DeChuJieLun jielun)
        {
            try
            {
                if (!oldNo.Equals(jielun.Process_BasicData.No))
                {
                    Process_DeChuJieLun ifIdxHasExist = this.LoadEntities(a => a.FK_BDNo == jielun.Process_BasicData.No).FirstOrDefault();
                    if (ifIdxHasExist != null) throw new Exception("结论【" + jielun.Process_BasicData.No + "】编号已存在");

                    this.AddJieLun(jielun);
                }
                else
                {
                    Process_DeChuJieLun jlDesc = this.LoadEntities(a => a.FK_BDNo == oldNo).FirstOrDefault();
                    if (jlDesc != null)
                    {
                        ModifyJieLun(jlDesc, jielun);
                        this.UpdateEntity(jlDesc);
                    }
                }

                //使用OID作为外键，没有修改编号的话，不改变OID，无需修改外键关系
                if (!oldNo.Equals(jielun.Process_BasicData.No))
                {
                    Process_DeChuJieLun oldjl = this.LoadEntities(a => a.FK_BDNo == oldNo).FirstOrDefault();
                    Process_DeChuJieLun newjl = this.LoadEntities(a => a.FK_BDNo == jielun.Process_BasicData.No).FirstOrDefault();

                    //更新轨迹表的关联关系
                    (CurrentDAL as DeChuJieLunDAL).UpdateTrackDCJLOID(oldjl.OID, newjl.OID);

                    this.DeleteJieLunWithoutUpdateColumn(oldNo);
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

        private void ModifyJieLun(Process_DeChuJieLun jlDesc, Process_DeChuJieLun jlSource)
        {
            jlDesc.OID = jlDesc.OID;

            jlDesc.FK_BDNo = jlSource.Process_BasicData.No;
            jlDesc.Mitigation = jlSource.Mitigation;
            jlDesc.EffectiveSolution = jlSource.EffectiveSolution;
            jlDesc.Arguments = jlSource.Arguments;
            jlDesc.FK_DBFXOID = jlSource.FK_DBFXOID;

            jlDesc.Process_BasicData.No = jlSource.Process_BasicData.No;
            jlDesc.Process_BasicData.Name = jlSource.Process_BasicData.Name;
            jlDesc.Process_BasicData.FK_Proposer = jlSource.Process_BasicData.FK_Proposer;
            jlDesc.Process_BasicData.ProposeTime = jlSource.Process_BasicData.ProposeTime;
            jlDesc.Process_BasicData.Description = jlSource.Process_BasicData.Description;
            jlDesc.Process_BasicData.Keys = jlSource.Process_BasicData.Keys;
            jlDesc.Process_BasicData.Remarks = jlSource.Process_BasicData.Remarks;
            jlDesc.Process_BasicData.ModifyTime = jlSource.Process_BasicData.ModifyTime;

            if (jlSource.Process_BasicData.FK_Flow != null)
            {
                jlDesc.Process_BasicData.FK_Flow = jlSource.Process_BasicData.FK_Flow;
            }
            else
            {
                jlDesc.Process_BasicData.FK_Flow = jlDesc.Process_BasicData.FK_Flow;
            }

            if (jlSource.Process_BasicData.FK_Node != null)
            {
                jlDesc.Process_BasicData.FK_Node = jlSource.Process_BasicData.FK_Node;
            }
            else
            {
                jlDesc.Process_BasicData.FK_Node = jlDesc.Process_BasicData.FK_Node;
            }

            if (jlSource.Process_BasicData.WorkId != null)
            {
                jlDesc.Process_BasicData.WorkId = jlSource.Process_BasicData.WorkId;
            }
            else
            {
                jlDesc.Process_BasicData.WorkId = jlDesc.Process_BasicData.WorkId;
            }
        }

        public List<Process_DeChuJieLun> GetJieLunHistoryDataFromTrack(CCFlowArgs args)
        {
            try
            {
                string workid = args.WorkID.ToString();
                Process_Track track = this.DbSession.TrackDAL.LoadEntities(a => a.FK_Flow == args.FK_Flow
                    && a.WorkID == workid).FirstOrDefault();

                if (track != null)
                {
                    string[] jlOIDArray = track.DCJLOID.Split(',');
                    List<Process_DeChuJieLun> jlList = new List<Process_DeChuJieLun>();

                    for (int i = 0; i < jlOIDArray.Length; i++)
                    {
                        int jlOID = Convert.ToInt32(jlOIDArray[i]);
                        Process_DeChuJieLun jl = this.LoadEntities(a => a.OID == jlOID).FirstOrDefault();

                        SetProposerName(jl);
                        SetDuiBiFenXiName(jl);

                        if (jl != null) jlList.Add(jl);
                    }

                    if (jlList.Count != 0)
                    {
                        return jlList;
                    }
                }
            }
            catch (Exception e)
            {
                return new List<Process_DeChuJieLun>();
            }
            return new List<Process_DeChuJieLun>();
        }
    }
}