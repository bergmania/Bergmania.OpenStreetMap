using Umbraco.Cms.Core.PropertyEditors;

namespace Bergmania.OpenStreetMap
{
    public class OpenStreetMapConfiguration
    {
        [ConfigurationField("defaultPosition", "Default Position", Constants.EditorView)]
        public OpenStreetMapModel DefaultPosition { get; set; }

        [ConfigurationField("tileLayer", "Tile Layer", Constants.TextStringView)]
        public string TileLayer { get; set; } = "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png";

        [ConfigurationField("tileLayerAttribution", "Tile Layer Attribution", Constants.TextStringView)]
        public string TileLayerAttribution { get; set; } = "Map data © OpenStreetMap contributors";
    }
}