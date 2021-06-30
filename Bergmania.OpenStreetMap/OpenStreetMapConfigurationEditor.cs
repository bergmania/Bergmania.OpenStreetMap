using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.Validators;
using Umbraco.Cms.Core.Services;

namespace Bergmania.OpenStreetMap
{
    public class OpenStreetMapConfigurationEditor : ConfigurationEditor<OpenStreetMapConfiguration>
    {
        public OpenStreetMapConfigurationEditor(
            IIOHelper ioHelper) : base(ioHelper)
        {
        }
    }
}