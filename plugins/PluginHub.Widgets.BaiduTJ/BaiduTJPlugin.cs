using System.Collections.Generic;
using System.IO;
using System.Web.Routing;
using PluginHub;
using PluginHub.Plugins;
using PluginHub.Services.Cms;

namespace PluginHub.Widgets.BaiduTJ
{
    /// <summary>
    /// PLugin
    /// </summary>
    public class BaiduTJPlugin : BasePlugin, IWidgetPlugin
    {
        //private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        public BaiduTJPlugin(
             IWebHelper webHelper)
        {
            //this._pictureService = pictureService;
            //this._settingService = settingService;
            this._webHelper = webHelper;
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return new List<string> { "home_page_bottom" };
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "WidgetsBaiduTJ";
            routeValues = new RouteValueDictionary { { "Namespaces", "PluginHub.Widgets.BaiduTJ.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Gets a route for displaying widget
        /// </summary>
        /// <param name="widgetZone">Widget zone where it's displayed</param>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "WidgetsBaiduTJ";
            routeValues = new RouteValueDictionary
            {
                {"Namespaces", "PluginHub.Widgets.BaiduTJ.Controllers"},
                {"area", null},
                {"widgetZone", widgetZone}
            };
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {


            base.Uninstall();
        }
    }
}
