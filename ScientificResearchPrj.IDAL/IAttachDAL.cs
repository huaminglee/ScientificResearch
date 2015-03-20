using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IDAL 
{
    public interface IAttachDAL : IBaseDAL<Process_Attach>
    {
        int SelectMaxOid();
    }
}