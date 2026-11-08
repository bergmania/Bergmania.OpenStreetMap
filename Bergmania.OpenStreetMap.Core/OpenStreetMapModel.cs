using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;

namespace Bergmania.OpenStreetMap.Core
{
    [DataContract]
    public class OpenStreetMapModel : IHtmlContent
    {
        [DataMember(Name = "zoom", IsRequired = true)]
        public int Zoom { get; set; }
        
        private LatitudeLongitudeModel _marker;
        private IReadOnlyList<MarkerModel> _markers;
        private IReadOnlyList<MarkerModel> _markerFallback;

        /// <summary>
        /// The first marker. Falls back to the first of <see cref="Markers"/> when only those are stored.
        /// </summary>
        [DataMember(Name = "marker")]
        public LatitudeLongitudeModel Marker
        {
            get => _marker ?? _markers?.FirstOrDefault();
            set { _marker = value; _markerFallback = null; }
        }

        /// <summary>
        /// All markers. Falls back to <see cref="Marker"/> for values saved before multiple markers were supported.
        /// </summary>
        [DataMember(Name = "markers")]
        public IReadOnlyList<MarkerModel> Markers
        {
            get
            {
                if (_markers is { Count: > 0 })
                {
                    return _markers;
                }

                if (_marker is null)
                    return Array.Empty<MarkerModel>();

                return _markerFallback ??= new[] { new MarkerModel { Latitude = _marker.Latitude, Longitude = _marker.Longitude } };
            }
            set => _markers = value;
        }
        
        [DataMember(Name = "boundingBox", IsRequired = true)]
        public BoundingBoxModel BoundingBox { get; set; }
        
        public OpenStreetMapConfiguration Configuration { get; set; }
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
                $"https://www.openstreetmap.org/export/embed.html?bbox={southWestLng}%2C{southWestLat}%2C{northEastLng}%2C{northEastLat}&amp;layer=mapnik";

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

    [DataContract]
    public class MarkerModel : LatitudeLongitudeModel
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }
    }
}