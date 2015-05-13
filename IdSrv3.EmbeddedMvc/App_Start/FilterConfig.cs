using System.Web;
using System.Web.Mvc;

namespace IdSrv3.EmbeddedMvc
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
