﻿using BP.DA;
using ScientificResearchPrj.IDAL;
using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.DAL 
{
    public class LinkDAL : BaseDAL<Process_Link>, ILinkDAL
    {
        public int SelectMaxOid()
        {
            try
            {
                string sql = "Select Max(OID) From Process_Link";
                return BP.DA.DBAccess.RunSQLReturnValInt(sql);
            }
            catch (Exception e)
            {
                return 0;
            }
        }
    }
}