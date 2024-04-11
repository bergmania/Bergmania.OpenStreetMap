using Umbraco.Cms.Core.WebAssets;

namespace Bergmania.OpenStreetMap.Core
{
    internal class LeafletCssFile : CssFile
    {
        public LeafletCssFile()
            : base("/App_Plugins/Bergmania.OpenStreetMap/lib/leaflet/leaflet.css")
        { }
    }
}