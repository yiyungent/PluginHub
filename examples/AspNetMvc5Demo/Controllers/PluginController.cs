using AspNetMvc5Demo.Models.PluginVM;
using PluginHub;
using PluginHub.Domain.Cms;
using PluginHub.Infrastructure;
using PluginHub.Plugins;
using PluginHub.Services.Cms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AspNetMvc5Demo.Controllers
{
    public class PluginController : Controller
    {
        #region Fields

        private readonly IPluginFinder _pluginFinder;
        private readonly IWebHelper _webHelper;
        private readonly WidgetSettings _widgetSettings;

        #endregion

        #region Constructors

        //public PluginController(IPluginFinder pluginFinder, IWebHelper webHelper)
        //{
        //    this._pluginFinder = pluginFinder;
        //    this._webHelper = webHelper;
        //    this._widgetSettings = new WidgetSettings();
        //}

        public PluginController()
        {
            this._pluginFinder = EngineContext.Current.Resolve<IPluginFinder>();
            this._webHelper = EngineContext.Current.Resolve<IWebHelper>();
            this._widgetSettings = new WidgetSettings();
        }

        //public PluginController()
        //{
        //    this._pluginFinder = new PluginFinder();
        //    this._webHelper = new WebHelper(HttpContext);
        //    this._widgetSettings = new WidgetSettings();
        //}

        #endregion

        #region Utilities

        #endregion

        #region Methods

        #region 列表
        public ActionResult Index()
        {
            var viewModel = new PluginListViewModel();
            var allPluginDescriptor = _pluginFinder.GetPluginDescriptors(LoadPluginsMode.All);
            foreach (var item in allPluginDescriptor)
            {
                viewModel.List.Add(item);
            }

            return View(viewModel);
        }
        #endregion

        #region 安装
        public ActionResult Install(string systemName)
        {
            try
            {
                var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName(systemName, LoadPluginsMode.All);
                if (pluginDescriptor == null)
                    //No plugin found with the specified id
                    return RedirectToAction("List");

                //check whether plugin is not installed
                if (pluginDescriptor.Installed)
                    return RedirectToAction("List");

                //install plugin
                pluginDescriptor.Instance().Install();

                //restart application
                _webHelper.RestartAppDomain();
            }
            catch (Exception exc)
            {
            }

            return RedirectToAction("Index");
        }
        #endregion

        #region 卸载
        public ActionResult Uninstall(string systemName)
        {
            try
            {
                var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName(systemName, LoadPluginsMode.All);
                if (pluginDescriptor == null)
                    //No plugin found with the specified id
                    return RedirectToAction("List");

                //check whether plugin is installed
                if (!pluginDescriptor.Installed)
                    return RedirectToAction("List");

                //uninstall plugin
                pluginDescriptor.Instance().Uninstall();

                //restart application
                _webHelper.RestartAppDomain();
            }
            catch (Exception exc)
            {
            }

            return RedirectToAction("Index");
        }
        #endregion

        #region 重新加载
        public ActionResult ReloadList()
        {
            //restart application
            _webHelper.RestartAppDomain();
            return RedirectToAction("Index");
        }
        #endregion

        #region 配置
        [HttpGet]
        public ActionResult Configure(string systemName)
        {
            var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName(systemName, LoadPluginsMode.All);

            ConfigureViewModel viewModel = new ConfigureViewModel();
            viewModel.FriendlyName = pluginDescriptor.FriendlyName;
            viewModel.SystemName = pluginDescriptor.SystemName;

            #region 是否具有配置页
            string actionName = null, controllerName = null;
            RouteValueDictionary routeValues = null;
            if (pluginDescriptor.Instance() is IWidgetPlugin)
            {
                var widgetPlugin = pluginDescriptor.Instance<IWidgetPlugin>();
                widgetPlugin.GetConfigurationRoute(out actionName, out controllerName, out routeValues);
            }
            else
            {
                viewModel = null;
                return View(viewModel);
            }
            viewModel.ConfigurationActionName = actionName;
            viewModel.ConfigurationControllerName = controllerName;
            viewModel.ConfigurationRouteValues = routeValues;
            #endregion

            return View(viewModel);
        }
        #endregion

        #endregion
    }
}