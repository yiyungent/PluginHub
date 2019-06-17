using System.Web.Routing;

namespace PluginHub.Web.Mvc.Routes
{
    public interface IRouteProvider
    {
        void RegisterRoutes(RouteCollection routes);

        int Priority { get; }
    }
}
