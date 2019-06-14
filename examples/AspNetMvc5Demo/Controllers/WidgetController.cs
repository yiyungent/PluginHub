using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using PluginHub;
using PluginHub.Services.Cms;

using AspNetMvc5Demo.Models.Cms;
using PluginHub.Domain.Cms;
using PluginHub.Plugins;

namespace AspNetMvc5Demo.Controllers
{
    public partial class WidgetController : Controller
    {
        #region Fields

        private readonly IWidgetService _widgetService;
        //private readonly IStoreContext _storeContext;
        //private readonly ICacheManager _cacheManager;

        #endregion

        #region Constructors

        public WidgetController()
        {
            this._widgetService = new WidgetService(new PluginFinder(), new WidgetSettings());
            //this._storeContext = storeContext;
            //this._cacheManager = new PerRequestCacheManager(HttpContext);
        }

        #endregion

        #region Methods

        [ChildActionOnly]
        public ActionResult WidgetsByZone(string widgetZone, object additionalData = null)
        {
            var cacheKey = string.Format("Nop.pres.widget-{0}-{1}", 1, widgetZone);
            //var cacheModel = _cacheManager.Get(cacheKey, () =>
            //{
            //    //model
            //    var model = new List<RenderWidgetModel>();

            //    var widgets = _widgetService.LoadActiveWidgetsByWidgetZone(widgetZone);
            //    foreach (var widget in widgets)
            //    {
            //        var widgetModel = new RenderWidgetModel();

            //        string actionName;
            //        string controllerName;
            //        RouteValueDictionary routeValues;
            //        widget.GetDisplayWidgetRoute(widgetZone, out actionName, out controllerName, out routeValues);
            //        widgetModel.ActionName = actionName;
            //        widgetModel.ControllerName = controllerName;
            //        widgetModel.RouteValues = routeValues;

            //        model.Add(widgetModel);
            //    }
            //    return model;
            //});
            var model = new List<RenderWidgetModel>();

            var widgets = _widgetService.LoadActiveWidgetsByWidgetZone(widgetZone);
            foreach (var widget in widgets)
            {
                var widgetModel = new RenderWidgetModel();

                string actionName;
                string controllerName;
                RouteValueDictionary routeValues;
                widget.GetDisplayWidgetRoute(widgetZone, out actionName, out controllerName, out routeValues);
                widgetModel.ActionName = actionName;
                widgetModel.ControllerName = controllerName;
                widgetModel.RouteValues = routeValues;

                model.Add(widgetModel);
            }

            //no data?
            if (model.Count == 0)
                return Content("");

            //"RouteValues" property of widget models depends on "additionalData".
            //We need to clone the cached model before modifications (the updated one should not be cached)
            var clonedModel = new List<RenderWidgetModel>();
            foreach (var widgetModel in model)
            {
                var clonedWidgetModel = new RenderWidgetModel();
                clonedWidgetModel.ActionName = widgetModel.ActionName;
                clonedWidgetModel.ControllerName = widgetModel.ControllerName;
                if (widgetModel.RouteValues != null)
                    clonedWidgetModel.RouteValues = new RouteValueDictionary(widgetModel.RouteValues);

                if (additionalData != null)
                {
                    if (clonedWidgetModel.RouteValues == null)
                        clonedWidgetModel.RouteValues = new RouteValueDictionary();
                    clonedWidgetModel.RouteValues.Add("additionalData", additionalData);
                }

                clonedModel.Add(clonedWidgetModel);
            }

            return PartialView(clonedModel);
        }

        #endregion



    }
}
