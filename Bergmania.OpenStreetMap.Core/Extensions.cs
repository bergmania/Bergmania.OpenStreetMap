using System.Text.Encodings.Web;
using Bergmania.OpenStreetMap.Core;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace Umbraco.Extensions
{
    public static class OpenStreetMapExtensions
    {
        public static IHtmlContent LeafletRendering(this OpenStreetMapModel model)
        {
            // The configuration has no data members, so it serializes to an empty object on its own.
            var config = new
            {
                tileLayer = model.Configuration?.TileLayer,
                tileLayerAttribution = model.Configuration?.TileLayerAttribution
            };

            // Encoded, as marker titles are free text and would otherwise be able to break out of the attribute.
            var data = HtmlEncoder.Default.Encode(JsonConvert.SerializeObject(model));
            var defaultConfig = HtmlEncoder.Default.Encode(JsonConvert.SerializeObject(config));

            return new HtmlString(@$"<div data-openstreetmap='{data}' data-openstreetmapdefaultconfig='{defaultConfig}' style='width: 100%; height: 400px;'></div>");
        }
        
        public static IHtmlContent LeafletScripts(this IHtmlHelper htmlHelper)
        {
            return new HtmlString(@"
<link rel='stylesheet' href='https://unpkg.com/leaflet@1.7.1/dist/leaflet.css' />
<script src='https://unpkg.com/leaflet@1.7.1/dist/leaflet.js'></script>
<script>

let elements = document.querySelectorAll('[data-openstreetmap]');
        
elements.forEach(elem => {
            
let data = JSON.parse(elem.getAttribute('data-openstreetmap'));
let config = JSON.parse(elem.getAttribute('data-openstreetmapdefaultconfig'));

// Create Leaflet map on map element.
let map = L.map(elem).fitBounds(L.latLngBounds(L.latLng(data.boundingBox.southWestCorner.latitude, data.boundingBox.southWestCorner.longitude),
                    L.latLng(data.boundingBox.northEastCorner.latitude, data.boundingBox.northEastCorner.longitude)));;

// Add OSM tile layer to the Leaflet map.
L.tileLayer(config.tileLayer, {
    attribution: config.tileLayerAttribution,
    referrerPolicy: 'strict-origin-when-cross-origin'
}).addTo(map);

let markers = data.markers && data.markers.length ? data.markers : (data.marker ? [data.marker] : []);

markers.forEach(marker => {
    let leafletMarker = L.marker(L.latLng(marker.latitude, marker.longitude), {draggable:false, title: marker.title || '', alt: marker.title || 'Marker'}).addTo(map);
    if(marker.title){
        // set as text, so the title is never treated as HTML
        let popup = document.createElement('span');
        popup.textContent = marker.title;
        leafletMarker.bindPopup(popup);
    }
});

// The saved bounding box is wherever the editor left the map, so widen it to keep every marker in view.
if(markers.length > 1){
    let bounds = map.getBounds();
    markers.forEach(marker => bounds.extend(L.latLng(marker.latitude, marker.longitude)));
    map.fitBounds(bounds, {padding: [30, 30]});
}

    });
</script>");
        }
    }
}