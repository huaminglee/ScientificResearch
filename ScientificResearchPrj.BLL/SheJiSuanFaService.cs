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
    public class SheJiSuanFaService : BaseService<Process_SuanFa>, ISheJiSuanFaService
    {
        public override void SetCurrentDAL()
        {
            CurrentDAL = this.DbSession.SheJiSuanFaDAL;
        }

        public List<Process_SuanFa> GetHistoryData(CCFlowArgs args)
        {
            string fk_node = args.FK_Node.ToString();
            string workid = args.WorkID.ToString();

            List<Process_SuanFa> sfList = this.LoadEntities(
                a => a.Process_BasicData.FK_Flow == args.FK_Flow &&
                a.Process_BasicData.FK_Node == fk_node &&
                a.Process_BasicData.WorkId == workid).ToList();
            if (sfList != null && sfList.Count != 0)
            {
                for (int i = 0; i < sfList.Count; i++)
                {
                    SetProposerName(sfList[i]);
                    SetXingShiHuaName(sfList[i]);
                }
                return sfList;
            }
            return new List<Process_SuanFa>();
        }

        private void SetProposerName(Process_SuanFa suanfa)
        {
            if (suanfa.Process_BasicData.FK_Proposer == null) return;

            MyPort_Emp proposer = this.DbSession.EmpDAL.LoadEntities(e => e.EmpNo.Equals(suanfa.Process_BasicData.FK_Proposer)).FirstOrDefault();
            if (proposer != null)
            {
                suanfa.Process_BasicData.ProposerName = proposer.Name;
            }
        }

        private void SetXingShiHuaName(Process_SuanFa suanfa)
        {
            if (suanfa.FK_XSHOID == null) return;

            Process_XingShiHua xingshihua = this.DbSession.XingShiHuaDAL.LoadEntities(a => a.OID == suanfa.FK_XSHOID).FirstOrDefault();
            if (xingshihua != null)
            {
                suanfa.FK_XSHName = xingshihua.Process_BasicData.Name;
            }
        }

        public void InsertOrUpdateTrack(CCFlowArgs args)
        {
            string workid = args.WorkID.ToString();
            Process_Track ifExist = this.DbSession.TrackDAL.LoadEntities(a => a.FK_Flow == args.FK_Flow
                && a.WorkID == workid).FirstOrDefault();

            List<Process_SuanFa> suanfaList = this.LoadEntities(a => a.Process_BasicData.FK_Flow == args.FK_Flow
                && a.Process_BasicData.WorkId == workid).ToList();

            if (ifExist != null)
            {
                if (suanfaList != null && suanfaList.Count != 0)
                {
                    ifExist.SJSFOID = CombindOIDStr(suanfaList);
                }

                this.DbSession.TrackDAL.UpdateEntity(ifExist);
                this.DbSession.SaveChanges();
            }
            else
            {
                ifExist = new Process_Track();
                if (suanfaList != null && suanfaList.Count != 0)
                {
                    ifExist.SJSFOID = CombindOIDStr(suanfaList);
                }

                ifExist.FK_Flow = args.FK_Flow;
                ifExist.WorkID = workid;

                this.DbSession.TrackDAL.AddEntity(ifExist);
                this.DbSession.SaveChanges();
            }
        }

        private string CombindOIDStr(List<Process_SuanFa> suanfaList)
        {
            string sfOIDStr = "";
            for (int i = 0; i < suanfaList.Count - 1; i++)
            {
                sfOIDStr += suanfaList[i].OID.ToString() + ",";
            }
            sfOIDStr += suanfaList[suanfaList.Count - 1].OID.ToString();

            return sfOIDStr;
        }

        public Dictionary<string, string> AddSuanFa(Process_SuanFa suanfa)
        {
            try
            {
                Process_SuanFa ifIdxHasExist = this.LoadEntities(a => a.FK_BDNo == suanfa.Process_BasicData.No).FirstOrDefault();
                if (ifIdxHasExist != null) throw new Exception("算法【" + suanfa.Process_BasicData.No + "】编号已存在");

                suanfa.OID = (CurrentDAL as SheJiSuanFaDAL).SelectMaxOid() + 1;
                suanfa.FK_BDNo = suanfa.Process_BasicData.No;

                this.AddEntity(suanfa);

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

        public Dictionary<string, string> DeleteSuanFa(string sfNo)
        {
            try
            {
                DeleteSuanFaWithoutUpdateColumn(sfNo);

                /// //更新轨迹表的关联关系
                //ToDo---------------------------------------------------------------------
                //更新设计实验表的外键关系

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

        private void DeleteSuanFaWithoutUpdateColumn(string sfNo)
        {
            Process_SuanFa suanfa = this.LoadEntities(a => a.FK_BDNo == sfNo).FirstOrDefault();
            if (suanfa != null)
            {
                this.DeleteEntity(suanfa);
            }

            Process_BasicData basicData = this.DbSession.BasicDataDAL.LoadEntities(a => a.No == sfNo).FirstOrDefault();
            if (basicData != null)
            {
                this.DbSession.BasicDataDAL.DeleteEntity(basicData);
                this.DbSession.SaveChanges();
            }
        }

        public Dictionary<string, string> ModifySuanFa(string oldNo, Process_SuanFa suanfa)
        {
            try
            {
                if (!oldNo.Equals(suanfa.Process_BasicData.No))
                {
                    Process_SuanFa ifIdxHasExist = this.LoadEntities(a => a.FK_BDNo == suanfa.Process_BasicData.No).FirstOrDefault();
                    if (ifIdxHasExist != null) throw new Exception("算法【" + suanfa.Process_BasicData.No + "】编号已存在");

                    this.AddSuanFa(suanfa);
                }
                else
                {
                    Process_SuanFa sfDesc = this.LoadEntities(a => a.FK_BDNo == oldNo).FirstOrDefault();
                    if (sfDesc != null)
                    {
                        ModifySuanFa(sfDesc, suanfa);
                        this.UpdateEntity(sfDesc);
                    }
                }

                //使用OID作为外键，没有修改编号的话，不改变OID，无需修改外键关系
                if (!oldNo.Equals(suanfa.Process_BasicData.No))
                {
                    Process_SuanFa oldSuanFa = this.LoadEntities(a => a.FK_BDNo == oldNo).FirstOrDefault();
                    Process_SuanFa newSuanFa = this.LoadEntities(a => a.FK_BDNo == suanfa.Process_BasicData.No).FirstOrDefault();

                    //更新轨迹表的关联关系
                    (CurrentDAL as SheJiSuanFaDAL).UpdateTrackSJSFOID(oldSuanFa.OID, newSuanFa.OID);
                    //更新设计实验表的外键关系
                    (CurrentDAL as SheJiSuanFaDAL).UpdateShiYanSFOID(oldSuanFa.OID, newSuanFa.OID);

                    this.DeleteSuanFaWithoutUpdateColumn(oldNo);
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

        private void ModifySuanFa(Process_SuanFa suanfaDesc, Process_SuanFa suanfaSource)
        {
            suanfaDesc.OID = suanfaDesc.OID;

            suanfaDesc.FK_BDNo = suanfaSource.Process_BasicData.No;
            suanfaDesc.Design = suanfaSource.Design;
            suanfaDesc.RealizeStep = suanfaSource.RealizeStep;
            suanfaDesc.FK_XSHOID = suanfaSource.FK_XSHOID;

            suanfaDesc.Process_BasicData.No = suanfaSource.Process_BasicData.No;
            suanfaDesc.Process_BasicData.Name = suanfaSource.Process_BasicData.Name;
            suanfaDesc.Process_BasicData.FK_Proposer = suanfaSource.Process_BasicData.FK_Proposer;
            suanfaDesc.Process_BasicData.ProposeTime = suanfaSource.Process_BasicData.ProposeTime;
            suanfaDesc.Process_BasicData.Description = suanfaSource.Process_BasicData.Description;
            suanfaDesc.Process_BasicData.Keys = suanfaSource.Process_BasicData.Keys;
            suanfaDesc.Process_BasicData.Remarks = suanfaSource.Process_BasicData.Remarks;
            suanfaDesc.Process_BasicData.ModifyTime = suanfaSource.Process_BasicData.ModifyTime;

            if (suanfaSource.Process_BasicData.FK_Flow != null)
            {
                suanfaDesc.Process_BasicData.FK_Flow = suanfaSource.Process_BasicData.FK_Flow;
            }
            else
            {
                suanfaDesc.Process_BasicData.FK_Flow = suanfaDesc.Process_BasicData.FK_Flow;
            }

            if (suanfaSource.Process_BasicData.FK_Node != null)
            {
                suanfaDesc.Process_BasicData.FK_Node = suanfaSource.Process_BasicData.FK_Node;
            }
            else
            {
                suanfaDesc.Process_BasicData.FK_Node = suanfaDesc.Process_BasicData.FK_Node;
            }

            if (suanfaSource.Process_BasicData.WorkId != null)
            {
                suanfaDesc.Process_BasicData.WorkId = suanfaSource.Process_BasicData.WorkId;
            }
            else
            {
                suanfaDesc.Process_BasicData.WorkId = suanfaDesc.Process_BasicData.WorkId;
            }
        }

        public List<Process_SuanFa> GetSuanFaHistoryDataFromTrack(CCFlowArgs args)
        {
            try
            {
                string workid = args.WorkID.ToString();
                Process_Track track = this.DbSession.TrackDAL.LoadEntities(a => a.FK_Flow == args.FK_Flow
                    && a.WorkID == workid).FirstOrDefault();

                if (track != null)
                {
                    string[] sfOIDArray = track.SJSFOID.Split(',');
                    List<Process_SuanFa> sfList = new List<Process_SuanFa>();

                    for (int i = 0; i < sfOIDArray.Length; i++)
                    {
                        int sfOID = Convert.ToInt32(sfOIDArray[i]);
                        Process_SuanFa sf = this.LoadEntities(a => a.OID == sfOID).FirstOrDefault();

                        SetProposerName(sf);
                        SetXingShiHuaName(sf);

                        if (sf != null) sfList.Add(sf);
                    }

                    if (sfList.Count != 0)
                    {
                        return sfList;
                    }
                }
            }
            catch (Exception e)
            {
                return new List<Process_SuanFa>();
            }
            return new List<Process_SuanFa>();
        }
    }
}