using System.Web.Mvc;
using System.Web.Routing;

namespace AspNetMvc5Demo.Models.Cms
{
    public class WidgetModel
    {
        [AllowHtml]
        public string FriendlyName { get; set; }

        [AllowHtml]
        public string SystemName { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }


        public string ConfigurationActionName { get; set; }
        public string ConfigurationControllerName { get; set; }
        public RouteValueDictionary ConfigurationRouteValues { get; set; }
    }
}