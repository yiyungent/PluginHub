using AspNetMvc5Demo.Imples;
using Autofac;
using Autofac.Integration.Mvc;
using PluginHub;
using PluginHub.Domain.Cms;
using PluginHub.Fakes;
using PluginHub.Infrastructure;
using PluginHub.Infrastructure.DependencyManagement;
using PluginHub.Plugins;
using PluginHub.Services.Cms;
using PluginHub.Services.Configuration;
using PluginHub.Web.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PluginHub
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            //HTTP context and other related stuff
            builder.Register(c =>
                //register FakeHttpContext when HttpContext is not available
                HttpContext.Current != null ?
                (new HttpContextWrapper(HttpContext.Current) as HttpContextBase) :
                (new FakeHttpContext("~/") as HttpContextBase))
                .As<HttpContextBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Request)
                .As<HttpRequestBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Response)
                .As<HttpResponseBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Server)
                .As<HttpServerUtilityBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Session)
                .As<HttpSessionStateBase>()
                .InstancePerLifetimeScope();

            //web helper
            builder.RegisterType<WebHelper>().As<IWebHelper>().InstancePerLifetimeScope();

            //controllers
            builder.RegisterControllers(typeFinder.GetAssemblies().ToArray());

            //plugins
            builder.RegisterType<PluginFinder>().As<IPluginFinder>().InstancePerLifetimeScope();

            // services
            builder.RegisterType<WidgetService>().As<IWidgetService>().InstancePerLifetimeScope();

            builder.RegisterType<WidgetSettings>().As<WidgetSettings>().InstancePerLifetimeScope();

            // route
            builder.RegisterType<RoutePublisher>().As<IRoutePublisher>().InstancePerLifetimeScope();


            // custom services imply
            builder.RegisterType<CustomSettingImply>().As<ISettingService>().InstancePerLifetimeScope();


        }

        public int Order
        {
            get { return 0; }
        }
    }
}
