using AspNetMvc5Demo.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AspNetMvc5Demo
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //var route = RouteTable.Routes.MapRoute(
            //      name: "plugins",
            //      url: "plugin-{pluginName}/{controller}/{action}/{id}",
            //      defaults: new { controller = "WidgetsBaiduTJ", action = "Configure", id = UrlParameter.Optional },
            //      constraints: new { pluginName = new PluginRouteConstraint() }
            //  );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );


        }
    }
}
