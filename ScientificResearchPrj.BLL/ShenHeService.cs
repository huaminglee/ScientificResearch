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
    public class ShenHeService : BaseService<Process_ShenHe>, IShenHeService
    {
        public override void SetCurrentDAL()
        {
            CurrentDAL = this.DbSession.ShenHeDAL;
        }
         
        public Process_ShenHe GetShenHeHistoryData(CCFlowArgs args, string shenHeRen)
        {
            string fk_node = args.FK_Node.ToString();
            string workid = args.WorkID.ToString();

            Process_ShenHe shenhe = this.DbSession.ShenHeDAL.LoadEntities(a => a.FK_Flow == args.FK_Flow &&
                a.FK_Node == fk_node && a.WorkID == workid && a.ShenHeRen == shenHeRen).FirstOrDefault();
            if (shenhe != null)
            {
                SetShenHeRenName(shenhe);
                return shenhe;
            }
            return new Process_ShenHe(); 
        }

        public List<Process_ShenHe> GetShenHeHistoryDataWithoutCurrentShenHeRen(CCFlowArgs args, string shenHeRen)
        {
            string fk_node = args.FK_Node.ToString();
            string workid = args.WorkID.ToString();

            List<Process_ShenHe> shenheList = this.DbSession.ShenHeDAL.LoadEntities(a => a.FK_Flow == args.FK_Flow &&
                a.FK_Node == fk_node && a.WorkID == workid && a.ShenHeRen != shenHeRen).ToList();
            if (shenheList != null && shenheList.Count!=0)
            {
                for (int i = 0; i < shenheList.Count; i++) {
                    SetShenHeRenName(shenheList[i]);
                }
                return shenheList;
            }
            return new List<Process_ShenHe>();
        }

        private void SetShenHeRenName(Process_ShenHe shenhe)
        {
            if (shenhe.ShenHeRen == null) return;

            MyPort_Emp shenheren = this.DbSession.EmpDAL.LoadEntities(e => e.EmpNo.Equals(shenhe.ShenHeRen)).FirstOrDefault();
            if (shenheren != null)
            {
                shenhe.ShenHeRenName = shenheren.Name;
            }
        }

        public Dictionary<string, string> AddShenHe(Process_ShenHe shenhe) {
            try
            {
                shenhe.OID = (CurrentDAL as ShenHeDAL).SelectMaxOid() + 1;
                 
                this.AddEntity(shenhe);

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

        public Dictionary<string, string> DeleteShenHe(int OID) {
            try
            {
                Process_ShenHe shenhe = this.LoadEntities(a => a.OID == OID).FirstOrDefault();
                if (shenhe != null)
                {
                    this.DeleteEntity(shenhe);
                }
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

        public Dictionary<string, string> ModifyShenHe(int oldOID, Process_ShenHe shenhe) {
            try
            {
                Process_ShenHe shenheDesc = this.LoadEntities(a => a.OID == oldOID).FirstOrDefault();
                if (shenheDesc != null)
                {
                    ModifyShenHe(shenheDesc, shenhe);
                    this.UpdateEntity(shenheDesc);
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

        private void ModifyShenHe(Process_ShenHe shenheDesc,Process_ShenHe shenheSource) {
            shenheDesc.OID = shenheDesc.OID;

            shenheDesc.ShenHeRen = shenheSource.ShenHeRen;
            shenheDesc.ShenHeShiJian = shenheSource.ShenHeShiJian;
            shenheDesc.ShenHeJieGuo = shenheSource.ShenHeJieGuo;
            shenheDesc.ShenHeYiJian = shenheSource.ShenHeYiJian;
            shenheDesc.ModifyTime = shenheSource.ModifyTime;
            shenheDesc.FK_Flow = shenheSource.FK_Flow;
            shenheDesc.FK_Node = shenheSource.FK_Node;
            shenheDesc.WorkID = shenheSource.WorkID;
            shenheDesc.StepType = shenheSource.StepType;
        }

        public List<Process_ShenHe> GetShenHeHistoryDataByStep(CCFlowArgs args, string stepType) {
            string workid = args.WorkID.ToString();

            List<Process_ShenHe> shenheList = this.CurrentDAL.LoadEntities(a => a.FK_Flow == args.FK_Flow &&
                a.WorkID == workid && a.StepType == stepType).ToList();

            if (shenheList != null && shenheList.Count != 0) {
                for (int i = 0; i < shenheList.Count; i++) {
                    SetShenHeRenName(shenheList[i]);
                }
                    return shenheList;
            }
            return new List<Process_ShenHe>();
        }
    }
}