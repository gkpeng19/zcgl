using G.Zc.Entity.Eap;
using GOMFrameWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;

namespace G.Zc.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);

            //DbContext.InitContext<SimpleBase>("SimpleSqlServer");
            //DbContext.InitContext<SimpleSearchModel>("SimpleSqlServer");

            /*Eap数据配置*/
            DbContext.InitContext<EapBaseEntity>("ConnectionString_EAP");
            DbContext.InitContext<EapSearchEntity>("ConnectionString_EAP");
            DbContext.InitContext<EapOracleProcEntity>("ConnectionString_EAP");
        }
    }
}
