using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;

namespace Bergmania.OpenStreetMap.Core
{
    public class OpenStreetMapPropertyValueEditor: DataValueEditor
    {
        private readonly IJsonSerializer _jsonSerializer;

        public OpenStreetMapPropertyValueEditor(ILocalizedTextService localizedTextService, IShortStringHelper shortStringHelper, IJsonSerializer jsonSerializer) 
            : base(localizedTextService, shortStringHelper, jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        public OpenStreetMapPropertyValueEditor(ILocalizedTextService localizedTextService, IShortStringHelper shortStringHelper, IJsonSerializer jsonSerializer, IIOHelper ioHelper, DataEditorAttribute attribute) 
            : base(localizedTextService, shortStringHelper, jsonSerializer, ioHelper, attribute)
        {
            _jsonSerializer = jsonSerializer;
        }

        public override IValueRequiredValidator RequiredValidator => new OpenStreetMapRequiredValidator(_jsonSerializer);
    }
}