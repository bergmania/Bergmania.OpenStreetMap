using Umbraco.Cms.Core.WebAssets;

namespace Bergmania.OpenStreetMap
{
    internal class LeafletJsFile : JavaScriptFile
    {
        public LeafletJsFile()
            : base("/App_Plugins/Bergmania.OpenStreetMap/lib/leaflet/leaflet-src.js")
        { }
    }
}