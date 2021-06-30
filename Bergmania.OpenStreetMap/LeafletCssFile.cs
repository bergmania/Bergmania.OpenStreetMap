using Umbraco.Cms.Core.WebAssets;

namespace Bergmania.OpenStreetMap
{
    internal class LeafletCssFile : CssFile
    {
        public LeafletCssFile()
            : base("/App_Plugins/Bergmania.OpenStreetMap/lib/leaflet/leaflet.css")
        { }
    }
}