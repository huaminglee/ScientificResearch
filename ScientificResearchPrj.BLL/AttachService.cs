using BP.DA;
using ScientificResearchPrj.DAL;
using ScientificResearchPrj.IBLL;
using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace ScientificResearchPrj.BLL
{
    public class AttachService : BaseService<Process_Attach>, IAttachService
    {
        public override void SetCurrentDAL()
        {
            CurrentDAL = this.DbSession.AttachDAL;
        }

        public Process_Attach GetAttachByOID(int OID) {
            return this.CurrentDAL.LoadEntities(a => a.OID == OID).FirstOrDefault();
        }

        public List<Process_Attach> GetHistoryData(string No_OID, string isShenHe)
        {
            List<Process_Attach> fileList = this.LoadEntities(
                a => a.No_OID == No_OID && a.IsShenHe == isShenHe).ToList();
            if (fileList != null && fileList.Count != 0)
            {
                return fileList;
            }
            return new List<Process_Attach>();
        }

        public Dictionary<string, string> AddAttach(Process_Attach attach)
        {
            try
            {
                attach.OID = (CurrentDAL as AttachDAL).SelectMaxOid() + 1;

                this.AddEntity(attach);

                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add("state", "0");
                dictionary.Add("message", "添加成功");
                dictionary.Add("OID", attach.OID.ToString());
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

        public Dictionary<string, string> DeleteAttach(int OID) {
            try
            {
                Process_Attach attach = this.LoadEntities(a => a.OID == OID).FirstOrDefault();
                if (attach != null)
                {
                    this.DeleteEntity(attach);
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

    }
}
