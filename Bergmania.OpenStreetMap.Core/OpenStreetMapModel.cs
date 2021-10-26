using System;
using System.Globalization;
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

            var southWestLng = BoundingBox.SouthWestCorner.Longitude.ToString(CultureInfo.InvariantCulture);
            var southWestLat = BoundingBox.SouthWestCorner.Latitude.ToString(CultureInfo.InvariantCulture);
            
            var northEastLat = BoundingBox.NorthEastCorner.Latitude.ToString(CultureInfo.InvariantCulture);
            var northEastLng = BoundingBox.NorthEastCorner.Longitude.ToString(CultureInfo.InvariantCulture);

            var url = 
                $"https://www.openstreetmap.org/export/embed.html?bbox={southWestLng}%2C{southWestLat}%2C{northEastLat}%2C{northEastLng}&amp;layer=mapnik";

            if (Marker is not null)
            {
                var lat = Marker.Latitude.ToString(CultureInfo.InvariantCulture);
                var lng = Marker.Longitude.ToString(CultureInfo.InvariantCulture);

                url += $"&amp;marker={lat}%2C{lng}";
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