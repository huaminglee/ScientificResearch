using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;

namespace ScientificResearchPrj.Controllers.Base
{
    public class SRServiceFactory
    {
        private static int num = 0;
        public static SRService GetCurrentSRService()
        {
            SRService srService = (SRService)CallContext.GetData("srService");
            if (srService == null)
            {
                System.Diagnostics.Debug.WriteLine("测试srService唯一性" + (++num));
                srService = new SRService();
                CallContext.SetData("srService", srService);
            }
            return srService;
        }
    }
}