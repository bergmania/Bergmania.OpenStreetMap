using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace Bergmania.OpenStreetMap.Core
{
    public class OpenStreetMapConfigurationEditor : ConfigurationEditor<OpenStreetMapConfiguration>
    {
        public OpenStreetMapConfigurationEditor(IIOHelper ioHelper, IEditorConfigurationParser editorConfigurationParser) : base(ioHelper, editorConfigurationParser)
        {
        }
    }
}