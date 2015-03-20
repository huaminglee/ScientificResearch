using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ScientificResearchPrj.IBLL
{
    public interface ILinkService : IBaseService<Process_Link>
    {
        List<Process_Link> GetHistoryData(string No_OID, string isShenHe);

        Dictionary<string, string> AddLink(Process_Link link);

        Dictionary<string, string> DeleteLink(int OID);
    }
}
