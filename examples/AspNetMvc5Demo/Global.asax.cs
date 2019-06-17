using AspNetMvc5Demo;
using PluginHub.Infrastructure;
using PluginHub.Mvc5;
using PluginHub.Web.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AspNetMvc5Demo
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            RegisterRoutes(RouteTable.Routes);

            //initialize engine context
            EngineContext.Initialize(false);
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            AreaRegistration.RegisterAllAreas();

            //register custom routes (plugins, etc)
            var routePublisher = EngineContext.Current.Resolve<IRoutePublisher>();
            routePublisher.RegisterRoutes(routes);

            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
