using AspNetMvc5Demo;
using AspNetMvc5Demo.Mvc5;
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
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // 注册容器
            //AutofacConfig.Initialise();
            // 插件框架IoC注册
            //DependencyRegistrar.Initialise();
        }
    }
}
