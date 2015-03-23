using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using ScientificResearchPrj.Model;

namespace ScientificResearchPrj.DAL
{
   public class DbContextFactory
    {
       private static int num = 0;
       public static DbContext GetCurrentDbContext()
       {
           DbContext dbContext = (DbContext) CallContext.GetData("dbContent");
           if (dbContext == null)
           {
               System.Diagnostics.Debug.WriteLine("测试DbContext唯一性" + (++num));
               dbContext = new ScientificResearchPrjEntities();
               CallContext.SetData("dbContent",dbContext);
           }
           return dbContext;
       }
    }
}
