using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace ScientificResearchPrj.DALFactory
{
    public class DBSessionFactory
    {
        private static int num = 0;
        public static DBSession GetCurrentDbSession()
        {
            DBSession dbSession = (DBSession)CallContext.GetData("dbSession");
            if (dbSession == null)
            {
                System.Diagnostics.Debug.WriteLine("测试dbSession唯一性" + (++num));
                dbSession = new DBSession();
                CallContext.SetData("dbSession", dbSession);
            }
            return dbSession;
        }
    }
}
