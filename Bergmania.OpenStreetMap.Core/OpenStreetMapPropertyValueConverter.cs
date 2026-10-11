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
            
            var model = string.IsNullOrWhiteSpace(inter?.ToString()) ? configuration.DefaultPosition : _jsonSerializer.Deserialize<OpenStreetMapModel>(inter.ToString());

            // Extra markers stay stored when a data type is switched back to a single marker, but are not exposed.
            if (!configuration.AllowMultipleMarkers && model.Markers.Count > 1)
            {
                model.Markers = new[] { model.Markers[0] };
            }

            model.Configuration = configuration;
            return model;
        }
    }
}