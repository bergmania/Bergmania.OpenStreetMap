using System.Runtime.Serialization;
using Umbraco.Cms.Core.PropertyEditors;

namespace Bergmania.OpenStreetMap.Core;

[DataContract]
public class OpenStreetMapConfiguration
{
    [ConfigurationField("defaultPosition")]
    public OpenStreetMapModel DefaultPosition { get; set; }

    [ConfigurationField("showSearch")] 
    public bool ShowSearch { get; set; } = false;

    [ConfigurationField("showCoordinates")]
    public bool ShowCoordinates { get; set; } = false;

    [ConfigurationField("showSetMarkerByCoordinates")]
    public bool ShowSetMarkerByCoordinates { get; set; } = false;

    [ConfigurationField("allowClear")] 
    public bool AllowClear { get; set; } = true;

    [ConfigurationField("scrollWheelZoom")]
    public bool ScrollWheelZoom { get; set; } = true;

    [ConfigurationField("tileLayer")]
    public string TileLayer { get; set; } = "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png";

    [ConfigurationField("tileLayerAttribution")]
    public string TileLayerAttribution { get; set; } = "Map data © OpenStreetMap contributors";
}