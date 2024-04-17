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
            return new HtmlString(@$"<div data-openstreetmap='{JsonConvert.SerializeObject(model)}' data-openstreetmapdefaultconfig='{JsonConvert.SerializeObject(model.Configuration)}' style='width: 100%; height: 400px;'></div>");
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
    attribution: config.tileLayerAttribution
}).addTo(map);

if(data.marker){
    L.marker(L.latLng(data.marker.latitude, data.marker.longitude), {draggable:false,}).addTo(map);
}

    });
</script>");
        }
    }
}