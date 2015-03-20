using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.Model
{
    public class EmpType
    {
        public static string DAOSHI{get{return "1";}}//导师
        public static string XUESHENG { get { return "2"; } }//学生
        public static string BENKESHENG { get { return "3"; } }//本科生
        public static string YANJIUSHENG { get { return "4"; } }//研究生
        public static string BOSHISHENG { get { return "5"; } }//博士生
      
    }
}