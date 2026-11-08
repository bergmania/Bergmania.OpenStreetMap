using System;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;

namespace Bergmania.OpenStreetMap.Core
{
    public class OpenStreetMapPropertyValueConverter : PropertyValueConverterBase
    {
        private readonly IJsonSerializer _jsonSerializer;

        public OpenStreetMapPropertyValueConverter(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        public override bool IsConverter(IPublishedPropertyType propertyType) => propertyType.EditorAlias.Equals(Constants.EditorAlias);
        
        public override Type GetPropertyValueType(IPublishedPropertyType propertyType) => typeof(OpenStreetMapModel);
        
        public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) => PropertyCacheLevel.Element;
        
        public override object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            var configuration = propertyType.DataType.ConfigurationAs<OpenStreetMapConfiguration>();
            
            var isEmpty = string.IsNullOrWhiteSpace(inter?.ToString());
            var model = isEmpty ? configuration.DefaultPosition : _jsonSerializer.Deserialize<OpenStreetMapModel>(inter.ToString());

            // No default position configured, or a stored value that deserializes to null.
            // Templates expect a usable model, so fall back to the area the backoffice map starts on, without markers.
            model ??= CreateFallbackModel();

            // With multiple markers, content without a value has no markers; the default position only sets the zoom and area.
            // A new model, as the default position is shared by every property using this data type.
            if (isEmpty && configuration.AllowMultipleMarkers)
            {
                model = new OpenStreetMapModel { Zoom = model.Zoom, BoundingBox = model.BoundingBox };
            }

            // Extra markers stay stored when a data type is switched back to a single marker, but are not exposed.
            if (!configuration.AllowMultipleMarkers && model.Markers.Count > 1)
            {
                model.Markers = new[] { model.Markers[0] };
            }

            model.Configuration = configuration;
            return model;
        }

        // A new model each time, as the converted value is cached and could be changed by the caller.
        private static OpenStreetMapModel CreateFallbackModel() => new()
        {
            Zoom = 16,
            BoundingBox = new BoundingBoxModel
            {
                SouthWestCorner = new LatitudeLongitudeModel { Latitude = 54.970495269313204m, Longitude = -1.6278648376464846m },
                NorthEastCorner = new LatitudeLongitudeModel { Latitude = 54.97911600936982m, Longitude = -1.609625816345215m }
            }
        };
    }
}