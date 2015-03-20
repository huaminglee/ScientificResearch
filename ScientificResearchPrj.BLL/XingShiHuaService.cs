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
    public class XingShiHuaService : BaseService<Process_XingShiHua>, IXingShiHuaService
    {
        public override void SetCurrentDAL()
        {
            CurrentDAL = this.DbSession.XingShiHuaDAL;
        }
         
        public List<Process_XingShiHua> GetHistoryData(CCFlowArgs args)
        {
            string fk_node = args.FK_Node.ToString();
            string workid = args.WorkID.ToString();

            List<Process_XingShiHua> xshList = this.LoadEntities(
                a => a.Process_BasicData.FK_Flow == args.FK_Flow &&
                a.Process_BasicData.FK_Node == fk_node &&
                a.Process_BasicData.WorkId == workid).ToList();
            if (xshList != null && xshList.Count != 0)
            {
                for (int i = 0; i < xshList.Count; i++)
                {
                    SetProposerName(xshList[i]);
                    SetJieJueSiLuName(xshList[i]);
                }
                return xshList;
            }
            return new List<Process_XingShiHua>();
        }

        private void SetProposerName(Process_XingShiHua xingshihua)
        {
            if (xingshihua.Process_BasicData.FK_Proposer == null) return;

            MyPort_Emp proposer = this.DbSession.EmpDAL.LoadEntities(e => e.EmpNo.Equals(xingshihua.Process_BasicData.FK_Proposer)).FirstOrDefault();
            if (proposer != null)
            {
                xingshihua.Process_BasicData.ProposerName = proposer.Name;
            }
        }

        private void SetJieJueSiLuName(Process_XingShiHua xsh)
        {
            if (xsh.FK_SLOID == null) return;

            Process_SiLu silu = this.DbSession.JieJueSiLuDAL.LoadEntities(a => a.OID == xsh.FK_SLOID).FirstOrDefault();
            if (silu != null)
            {
                xsh.FK_SLName = silu.Process_BasicData.Name;
            }
        }

        public void InsertOrUpdateTrack(CCFlowArgs args)
        {
            string workid = args.WorkID.ToString();
            Process_Track ifExist = this.DbSession.TrackDAL.LoadEntities(a => a.FK_Flow == args.FK_Flow
                && a.WorkID == workid).FirstOrDefault();

            List<Process_XingShiHua> xingshihuaList = this.LoadEntities(a => a.Process_BasicData.FK_Flow == args.FK_Flow
                && a.Process_BasicData.WorkId == workid).ToList();

            if (ifExist != null)
            {
                if (xingshihuaList != null && xingshihuaList.Count != 0)
                {
                    ifExist.XSHOID = CombindOIDStr(xingshihuaList);
                }

                this.DbSession.TrackDAL.UpdateEntity(ifExist);
                this.DbSession.SaveChanges();
            }
            else
            {
                ifExist = new Process_Track();
                if (xingshihuaList != null && xingshihuaList.Count != 0)
                {
                    ifExist.XSHOID = CombindOIDStr(xingshihuaList);
                }

                ifExist.FK_Flow = args.FK_Flow;
                ifExist.WorkID = workid;

                this.DbSession.TrackDAL.AddEntity(ifExist);
                this.DbSession.SaveChanges();
            }
        }

        private string CombindOIDStr(List<Process_XingShiHua> xingshihuaList)
        {
            string xshOIDStr = "";
            for (int i = 0; i < xingshihuaList.Count - 1; i++)
            {
                xshOIDStr += xingshihuaList[i].OID.ToString() + ",";
            }
            xshOIDStr += xingshihuaList[xingshihuaList.Count - 1].OID.ToString();

            return xshOIDStr;
        }

        public Dictionary<string, string> AddXingShiHua(Process_XingShiHua xingshihua)
        {
            try
            {
                Process_XingShiHua ifIdxHasExist = this.LoadEntities(a => a.FK_BDNo == xingshihua.Process_BasicData.No).FirstOrDefault();
                if (ifIdxHasExist != null) throw new Exception("形式化【" + xingshihua.Process_BasicData.No + "】编号已存在");

                xingshihua.OID = (CurrentDAL as XingShiHuaDAL).SelectMaxOid() + 1;
                xingshihua.FK_BDNo = xingshihua.Process_BasicData.No;

                this.AddEntity(xingshihua);

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

        public Dictionary<string, string> DeleteXingShiHua(string xshNo)
        {
            try
            {
                DeleteXingShiHuaWithoutUpdateColumn(xshNo);

                /// //更新轨迹表的关联关系
                //ToDo---------------------------------------------------------------------
                //更新设计算法表的外键关系

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

        private void DeleteXingShiHuaWithoutUpdateColumn(string xshNo)
        {
            Process_XingShiHua xingshihua = this.LoadEntities(a => a.FK_BDNo == xshNo).FirstOrDefault();
            if (xingshihua != null)
            {
                this.DeleteEntity(xingshihua);
            }

            Process_BasicData basicData = this.DbSession.BasicDataDAL.LoadEntities(a => a.No == xshNo).FirstOrDefault();
            if (basicData != null)
            {
                this.DbSession.BasicDataDAL.DeleteEntity(basicData);
                this.DbSession.SaveChanges();
            }
        }

        public Dictionary<string, string> ModifyXingShiHua(string oldNo, Process_XingShiHua xingshihua)
        {
            try
            {
                if (!oldNo.Equals(xingshihua.Process_BasicData.No))
                {
                    Process_XingShiHua ifIdxHasExist = this.LoadEntities(a => a.FK_BDNo == xingshihua.Process_BasicData.No).FirstOrDefault();
                    if (ifIdxHasExist != null) throw new Exception("形式化【" + xingshihua.Process_BasicData.No + "】编号已存在");

                    this.AddXingShiHua(xingshihua);
                }
                else
                {
                    Process_XingShiHua xshDesc = this.LoadEntities(a => a.FK_BDNo == oldNo).FirstOrDefault();
                    if (xshDesc != null)
                    {
                        ModifyXingShiHua(xshDesc, xingshihua);
                        this.UpdateEntity(xshDesc);
                    }
                }

                //使用OID作为外键，没有修改编号的话，不改变OID，无需修改外键关系
                if (!oldNo.Equals(xingshihua.Process_BasicData.No))
                {
                    Process_XingShiHua oldXingShiHua = this.LoadEntities(a => a.FK_BDNo == oldNo).FirstOrDefault();
                    Process_XingShiHua newXingShiHua = this.LoadEntities(a => a.FK_BDNo == xingshihua.Process_BasicData.No).FirstOrDefault();

                    //更新轨迹表的关联关系
                    (CurrentDAL as XingShiHuaDAL).UpdateTrackXSHOID(oldXingShiHua.OID, newXingShiHua.OID);
                    //更新形式化表的外键关系
                    (CurrentDAL as XingShiHuaDAL).UpdateSuanFaXSHOID(oldXingShiHua.OID, newXingShiHua.OID);

                    this.DeleteXingShiHuaWithoutUpdateColumn(oldNo);
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

        private void ModifyXingShiHua(Process_XingShiHua xingshihuaDesc, Process_XingShiHua xingshihuaSource)
        {
            xingshihuaDesc.OID = xingshihuaDesc.OID;

            xingshihuaDesc.FK_BDNo = xingshihuaSource.Process_BasicData.No;
            xingshihuaDesc.FK_SLOID = xingshihuaSource.FK_SLOID;

            xingshihuaDesc.Process_BasicData.No = xingshihuaSource.Process_BasicData.No;
            xingshihuaDesc.Process_BasicData.Name = xingshihuaSource.Process_BasicData.Name;
            xingshihuaDesc.Process_BasicData.FK_Proposer = xingshihuaSource.Process_BasicData.FK_Proposer;
            xingshihuaDesc.Process_BasicData.ProposeTime = xingshihuaSource.Process_BasicData.ProposeTime;
            xingshihuaDesc.Process_BasicData.Description = xingshihuaSource.Process_BasicData.Description;
            xingshihuaDesc.Process_BasicData.Keys = xingshihuaSource.Process_BasicData.Keys;
            xingshihuaDesc.Process_BasicData.Remarks = xingshihuaSource.Process_BasicData.Remarks;
            xingshihuaDesc.Process_BasicData.ModifyTime = xingshihuaSource.Process_BasicData.ModifyTime;

            if (xingshihuaSource.Process_BasicData.FK_Flow != null)
            {
                xingshihuaDesc.Process_BasicData.FK_Flow = xingshihuaSource.Process_BasicData.FK_Flow;
            }
            else
            {
                xingshihuaDesc.Process_BasicData.FK_Flow = xingshihuaDesc.Process_BasicData.FK_Flow;
            }

            if (xingshihuaSource.Process_BasicData.FK_Node != null)
            {
                xingshihuaDesc.Process_BasicData.FK_Node = xingshihuaSource.Process_BasicData.FK_Node;
            }
            else
            {
                xingshihuaDesc.Process_BasicData.FK_Node = xingshihuaDesc.Process_BasicData.FK_Node;
            }

            if (xingshihuaSource.Process_BasicData.WorkId != null)
            {
                xingshihuaDesc.Process_BasicData.WorkId = xingshihuaSource.Process_BasicData.WorkId;
            }
            else
            {
                xingshihuaDesc.Process_BasicData.WorkId = xingshihuaDesc.Process_BasicData.WorkId;
            }
        }

        public List<Process_XingShiHua> GetXingShiHuaHistoryDataFromTrack(CCFlowArgs args)
        {
            try
            {
                string workid = args.WorkID.ToString();
                Process_Track track = this.DbSession.TrackDAL.LoadEntities(a => a.FK_Flow == args.FK_Flow
                    && a.WorkID == workid).FirstOrDefault();

                if (track != null)
                {
                    string[] xshOIDArray = track.XSHOID.Split(',');
                    List<Process_XingShiHua> xshList = new List<Process_XingShiHua>();

                    for (int i = 0; i < xshOIDArray.Length; i++)
                    {
                        int xshOID = Convert.ToInt32(xshOIDArray[i]);
                        Process_XingShiHua xsh = this.LoadEntities(a => a.OID == xshOID).FirstOrDefault();

                        SetProposerName(xsh);
                        SetJieJueSiLuName(xsh);

                        if (xsh != null) xshList.Add(xsh);
                    }

                    if (xshList.Count != 0)
                    {
                        return xshList;
                    }
                }
            }
            catch (Exception e)
            {
                return new List<Process_XingShiHua>();
            }
            return new List<Process_XingShiHua>();
        }

       
    }
}