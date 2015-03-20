using BP.DA;
using ScientificResearchPrj.DAL;
using ScientificResearchPrj.IBLL;
using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ScientificResearchPrj.BLL
{
    public class LinkService : BaseService<Process_Link>, ILinkService
    {
        public override void SetCurrentDAL()
        {
            CurrentDAL = this.DbSession.LinkDAL;
        }

        public List<Process_Link> GetHistoryData(string No_OID, string isShenHe)
        {
            List<Process_Link> linkList = this.LoadEntities(
                a => a.No_OID == No_OID && a.IsShenHe == isShenHe).ToList();
            if (linkList != null && linkList.Count != 0)
            {
                return linkList;
            }
            return new List<Process_Link>();
        }

        public Dictionary<string, string> AddLink(Process_Link link) {
            try
            {
                link.OID = (CurrentDAL as LinkDAL).SelectMaxOid() + 1;

                this.AddEntity(link);

                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add("state", "0");
                dictionary.Add("message", "添加成功");
                dictionary.Add("OID", link.OID.ToString());
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

        public Dictionary<string, string> DeleteLink(int OID) {
            try
            {
                Process_Link link = this.LoadEntities(a => a.OID == OID).FirstOrDefault();
                if (link != null)
                {
                    this.DeleteEntity(link);
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
