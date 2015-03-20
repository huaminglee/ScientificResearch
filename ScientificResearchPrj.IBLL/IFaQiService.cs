using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ScientificResearchPrj.IBLL
{
    public interface IFaQiService
    {
        DataTable GetCanStartFlows();
        DataTable GetHisToryStartFlows(string fk_flow);
       
    }
}
