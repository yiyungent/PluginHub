using System.Collections.Generic;
using System.Web.Mvc;
using PluginHub.Mvc5;

namespace AspNetMvc5Demo.Models.PluginVM
{
    public partial class PluginModel
    {
        public PluginModel()
        {
            Locales = new List<PluginLocalizedModel>();
        }
        [AllowHtml]
        public string Group { get; set; }

        [AllowHtml]
        public string FriendlyName { get; set; }

        [AllowHtml]
        public string SystemName { get; set; }

        [AllowHtml]
        public string Version { get; set; }

        [AllowHtml]
        public string Author { get; set; }

        public int DisplayOrder { get; set; }

        public string ConfigurationUrl { get; set; }

        public bool Installed { get; set; }

        public bool CanChangeEnabled { get; set; }

        public bool IsEnabled { get; set; }

        public string LogoUrl { get; set; }

        public IList<PluginLocalizedModel> Locales { get; set; }


        ////Store mapping
        //public bool LimitedToStores { get; set; }

        //public List<StoreModel> AvailableStores { get; set; }

        public int[] SelectedStoreIds { get; set; }
    }
    public partial class PluginLocalizedModel
    {
        public int LanguageId { get; set; }

        [AllowHtml]
        public string FriendlyName { get; set; }
    }
}