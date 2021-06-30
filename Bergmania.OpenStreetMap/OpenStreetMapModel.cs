using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;

namespace Bergmania.OpenStreetMap
{
    [DataContract]
    public class OpenStreetMapModel : IHtmlContent
    {
        [DataMember(Name = "zoom", IsRequired = true)]
        public int Zoom { get; set; }
        
        [DataMember(Name = "marker")]
        public LatitudeLongitudeModel Marker { get; set; }
        
        [DataMember(Name = "boundingBox", IsRequired = true)]
        public BoundingBoxModel BoundingBox { get; set; }

        public void WriteTo(TextWriter writer, HtmlEncoder encoder)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (encoder == null)
            {
                throw new ArgumentNullException(nameof(encoder));
            }

            var url = 
                $"https://www.openstreetmap.org/export/embed.html?bbox={BoundingBox.SouthWestCorner.Longitude}%2C{BoundingBox.SouthWestCorner.Latitude}%2C{BoundingBox.NorthEastCorner.Longitude}%2C{BoundingBox.NorthEastCorner.Latitude}&amp;layer=mapnik";

            if (Marker is not null)
            {
                url += $"&amp;marker={Marker.Latitude}%2C{Marker.Longitude}";
            }

            writer.Write($"<iframe width=\"100%\" height=\"400\" frameborder=\"0\" scrolling=\"no\" marginheight=\"0\" marginwidth=\"0\" src=\"{url}\" style=\"border: 1px solid black\"></iframe>");
        }
    }

    [DataContract]
    public class BoundingBoxModel
    {
        [DataMember(Name = "northEastCorner", IsRequired = true)]
        public LatitudeLongitudeModel NorthEastCorner { get; set; }
        
        [DataMember(Name = "southWestCorner", IsRequired = true)]
        public LatitudeLongitudeModel SouthWestCorner { get; set; }
    }

    [DataContract]
    public class LatitudeLongitudeModel
    {
        [DataMember(Name = "latitude", IsRequired = true)]
        public decimal Latitude { get; set; }
        
        [DataMember(Name = "longitude", IsRequired = true)]
        public decimal Longitude { get; set; }

    }
}