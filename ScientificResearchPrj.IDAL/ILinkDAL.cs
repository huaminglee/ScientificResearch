using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IDAL 
{
    public interface ILinkDAL : IBaseDAL<Process_Link>
    {
        int SelectMaxOid();
    }
}