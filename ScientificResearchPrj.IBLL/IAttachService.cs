using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ScientificResearchPrj.IBLL
{
    public interface IAttachService : IBaseService<Process_Attach>
    {
        Process_Attach GetAttachByOID(int OID);

        List<Process_Attach> GetHistoryData(string No_OID, string isShenHe);

        Dictionary<string, string> AddAttach(Process_Attach attach);

        Dictionary<string, string> DeleteAttach(int OID);
    }
}
