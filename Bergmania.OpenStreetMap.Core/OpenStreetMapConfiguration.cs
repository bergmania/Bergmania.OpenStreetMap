using System.Runtime.Serialization;
using Umbraco.Cms.Core.PropertyEditors;

namespace Bergmania.OpenStreetMap.Core
{
    [DataContract]
    public class OpenStreetMapConfiguration
    {
        [DataMember(Name ="defaultPosition")]
        [ConfigurationField("defaultPosition", "Default Position", Constants.EditorView)]
        public OpenStreetMapModel DefaultPosition { get; set; }

        [DataMember(Name = "showSearch")]
        [ConfigurationField("showSearch", "Show Search", Constants.BooleanView, Description = "Show search field above map.")]
        public bool ShowSearch { get; set; } = false;

        [DataMember(Name = "showCoordinates")]
        [ConfigurationField("showCoordinates", "Show Coordinates", Constants.BooleanView, Description = "Show marker coordinates below map.")]
        public bool ShowCoordinates { get; set; } = false;

        [DataMember(Name = "allowClear")]
        [ConfigurationField("allowClear", "Allow Clear", Constants.BooleanView, Description = "Allow clearing previous marker.")]
        public bool AllowClear { get; set; } = false;

        [DataMember(Name ="tileLayer")]
        [ConfigurationField("tileLayer", "Tile Layer", Constants.TextStringView)]
        public string TileLayer { get; set; } = "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png";

        [DataMember(Name ="tileLayerAttribution")]
        [ConfigurationField("tileLayerAttribution", "Tile Layer Attribution", Constants.TextStringView)]
        public string TileLayerAttribution { get; set; } = "Map data © OpenStreetMap contributors";
    }
}