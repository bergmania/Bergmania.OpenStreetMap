using Umbraco.Cms.Core.WebAssets;

namespace Bergmania.OpenStreetMap
{
    internal class OpenStreetMapControllerJsFile : JavaScriptFile
    {
        public OpenStreetMapControllerJsFile()
            : base("/App_Plugins/Bergmania.OpenStreetMap/bergmania.openstreetmap.controller.js")
        { }
    }
}