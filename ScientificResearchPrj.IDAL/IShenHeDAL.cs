using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IDAL 
{
    public interface IShenHeDAL : IBaseDAL<Process_ShenHe>
    {
        int SelectMaxOid();
    }
}