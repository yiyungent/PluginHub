using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using PluginHub.Infrastructure;
using PluginHub.Plugins;

namespace PluginHub.Web.Mvc.Routes
{
    /// <summary>
    /// Route publisher
    /// </summary>
    public class RoutePublisher : IRoutePublisher
    {
        protected readonly ITypeFinder typeFinder;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="typeFinder"></param>
        public RoutePublisher(ITypeFinder typeFinder)
        {
            this.typeFinder = typeFinder;
        }

        /// <summary>
        /// Find a plugin descriptor by some type which is located into its assembly
        /// </summary>
        /// <param name="providerType">Provider type</param>
        /// <returns>Plugin descriptor</returns>
        protected virtual PluginDescriptor FindPlugin(Type providerType)
        {
            if (providerType == null)
                throw new ArgumentNullException("providerType");

            foreach (var plugin in PluginManager.ReferencedPlugins)
            {
                if (plugin.ReferencedAssembly == null)
                    continue;

                // 路由提供者所在程序集 与 当前插件所在程序集 相同，说明此路由提供者在当前插件程序集
                if (plugin.ReferencedAssembly.FullName == providerType.Assembly.FullName)
                    return plugin;
            }

            return null;
        }

        /// <summary>
        /// 创建所有实现了<see cref="IRouteProvider"/>的类，（排除非插件，和插件未安装），调用他们的注册路由方法
        /// Register routes
        /// </summary>
        /// <param name="routes">Routes</param>
        public virtual void RegisterRoutes(RouteCollection routes)
        {
            var routeProviderTypes = typeFinder.FindClassesOfType<IRouteProvider>();
            var routeProviders = new List<IRouteProvider>();
            foreach (var providerType in routeProviderTypes)
            {
                //Ignore not installed plugins
                var plugin = FindPlugin(providerType);
                if (plugin != null && !plugin.Installed)
                    continue;
                // 旧方法：无法实现 实现 IRouteProvider 的类 采用构造函数注入（必须有无参构造函数，否则无法创建 IRouteProvider）
                // 但只能使用此旧方法，因为插件主类没有被注册在IoC容器
                var provider = Activator.CreateInstance(providerType) as IRouteProvider;
                //var provider = EngineContext.Current.Resolve(providerType) as IRouteProvider;
                routeProviders.Add(provider);
            }
            routeProviders = routeProviders.OrderByDescending(rp => rp.Priority).ToList();
            routeProviders.ForEach(rp => rp.RegisterRoutes(routes));
        }
    }
}
