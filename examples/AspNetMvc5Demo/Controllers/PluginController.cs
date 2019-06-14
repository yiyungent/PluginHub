using AspNetMvc5Demo.Models.PluginVM;
using PluginHub;
using PluginHub.Domain.Cms;
using PluginHub.Plugins;
using PluginHub.Services.Cms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

        public PluginController(IPluginFinder pluginFinder, IWebHelper webHelper)
        {
            this._pluginFinder = pluginFinder;
            this._webHelper = webHelper;
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

        public ActionResult ReloadList()
        {
            //restart application
            _webHelper.RestartAppDomain();
            return RedirectToAction("Index");
        }

        #endregion
    }
}