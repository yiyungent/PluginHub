using PluginHub.Services.Configuration;
using PluginHub.Widgets.BaiduTJ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PluginHub.Widgets.BaiduTJ.Controllers
{
    public class WidgetsBaiduTJController : Controller
    {
        private readonly ISettingService _settingService;

        public WidgetsBaiduTJController(ISettingService settingService)
        {
            this._settingService = settingService;
        }

        [ChildActionOnly]
        public ActionResult PublicInfo(string widgetZone, object additionalData = null)
        {
            BaiduTJSettings baiduTJSettings = _settingService.LoadSetting<BaiduTJSettings>();
            ViewBag.TJCode = baiduTJSettings.TJCode;

            return View("~/Plugins/Widgets.BaiduTJ/Views/WidgetsBaiduTJ/PublicInfo.cshtml");
        }

        [HttpGet]
        //[ChildActionOnly]
        public ActionResult Configure()
        {
            BaiduTJSettings baiduTJSettings = _settingService.LoadSetting<BaiduTJSettings>();

            ConfigurationViewModel viewModel = new ConfigurationViewModel();
            viewModel.TJCode = baiduTJSettings.TJCode;

            return View("~/Plugins/Widgets.BaiduTJ/Views/WidgetsBaiduTJ/Configure.cshtml", viewModel);
        }

        [HttpPost]
        //[ChildActionOnly]
        public ActionResult Configure(ConfigurationViewModel inputModel)
        {
            BaiduTJSettings baiduTJSettings = _settingService.LoadSetting<BaiduTJSettings>();
            baiduTJSettings.TJCode = inputModel.TJCode;
            try
            {
                _settingService.SaveSetting<BaiduTJSettings>(baiduTJSettings);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("BaiduTJ.Configure", "保存失败");
            }

            return View("~/Plugins/Widgets.BaiduTJ/Views/WidgetsBaiduTJ/Configure.cshtml");
        }
    }
}