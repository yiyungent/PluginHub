using System.Web.Routing;

namespace PluginHub.Web.Mvc.Routes
{
    /// <summary>
    /// Route publisher
    /// </summary>
    public interface IRoutePublisher
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routes">Routes</param>
        void RegisterRoutes(RouteCollection routes);
    }
}
