using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;

namespace Bergmania.OpenStreetMap.Core
{

    /// <summary>
    /// Represents a decimal property and parameter editor.
    /// </summary>
    [DataEditor(
        Constants.EditorAlias,
        EditorType.PropertyValue | EditorType.MacroParameter,
        Constants.EditorName,
        Constants.EditorView,
        Icon = Constants.EditorIcon,
        ValueType = ValueTypes.Json)]
    public class OpenStreetMapPropertyEditor : DataEditor
    {
        private readonly IIOHelper _ioHelper;

        public OpenStreetMapPropertyEditor(
            IDataValueEditorFactory dataValueEditorFactory, 
            IIOHelper ioHelper,
            EditorType type = EditorType.PropertyValue)
            : base(dataValueEditorFactory, type)
        {
            _ioHelper = ioHelper;
        }
        
        /// <inheritdoc />
        protected override IDataValueEditor CreateValueEditor() => DataValueEditorFactory.Create<OpenStreetMapPropertyValueEditor>(Attribute);
        
        /// <inheritdoc />
        protected override IConfigurationEditor CreateConfigurationEditor() => new OpenStreetMapConfigurationEditor(_ioHelper);
    }
}