using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using GTI619_Lab5.Logger;

namespace GTI619_Lab5
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly ILogger s_logger = LogManager.GetLogger("Request");
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest()
        {
            var request = HttpContext.Current.Request;
            s_logger.Info(string.Format("Request={0}; IP={1}; USER_AGENT={2}", request.Url.AbsoluteUri, request.UserHostAddress, request.UserAgent));
        }
    }
}
