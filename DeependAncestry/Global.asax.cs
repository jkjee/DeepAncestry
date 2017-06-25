using System.Web.Mvc;
using System.Web.Http;
using System.Web.Routing;

namespace DeependAncestry
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
