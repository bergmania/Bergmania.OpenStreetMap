using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;

namespace Bergmania.OpenStreetMap.Core
{
    
    [DataEditor(
        Constants.EditorAlias,
        ValueType = ValueTypes.Json)]
    public class OpenStreetMapPropertyEditor(IDataValueEditorFactory dataValueEditorFactory, IIOHelper ioHelper)
        : DataEditor(dataValueEditorFactory)
    {
        private readonly IIOHelper _ioHelper = ioHelper;


        /// <inheritdoc />
        protected override IDataValueEditor CreateValueEditor() => DataValueEditorFactory.Create<OpenStreetMapPropertyValueEditor>(Attribute);
        
        /// <inheritdoc />
       protected override IConfigurationEditor CreateConfigurationEditor() => new OpenStreetMapConfigurationEditor(_ioHelper);
    }
}