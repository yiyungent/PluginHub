using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AspNetMvc5Demo.Widgets.BaiduTJ.Controllers
{
    public class WidgetsBaiduTJController : Controller
    {
        [ChildActionOnly]
        public ActionResult PublicInfo(string widgetZone, object additionalData = null)
        {
            return View("~/Plugins/Widgets.BaiduTJ/Views/WidgetsBaiduTJ/PublicInfo.cshtml");
        }
    }
}