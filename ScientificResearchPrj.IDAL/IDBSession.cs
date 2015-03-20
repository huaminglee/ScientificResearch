using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ScientificResearchPrj.IDAL
{
   public interface IDBSession
    {
       DbContext dbEntities { get; }
       List<string> ExecuteSql(string sql, params SqlParameter[] pars);
        int SaveChanges();
    }
}
