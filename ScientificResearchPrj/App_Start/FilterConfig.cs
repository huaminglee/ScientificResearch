using ScientificResearchPrj.Model;
using System.Web;
using System.Web.Mvc;

namespace ScientificResearchPrj
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new MyActionFilterAttribute());
            filters.Add(new HandleErrorAttribute());
        }
    }
}