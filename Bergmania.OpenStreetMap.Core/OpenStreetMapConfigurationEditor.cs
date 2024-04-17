using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;

namespace Bergmania.OpenStreetMap.Core
{
    public class OpenStreetMapConfigurationEditor : ConfigurationEditor<OpenStreetMapConfiguration>
    {
        public OpenStreetMapConfigurationEditor(IIOHelper ioHelper) : base(ioHelper)
        {
        }
    }
}