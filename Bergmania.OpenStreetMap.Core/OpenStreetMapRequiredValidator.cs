using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Extensions;

namespace Bergmania.OpenStreetMap.Core
{
    public class OpenStreetMapRequiredValidator : IValueRequiredValidator
    {
        private readonly IJsonSerializer _jsonSerializer;

        public OpenStreetMapRequiredValidator(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        public IEnumerable<ValidationResult> ValidateRequired(object value, string valueType)
        {
            
            if (value == null)
            {
                yield return new ValidationResult("Value cannot be null", new[] {"value"});
                yield break;
            }

            if (value.ToString().DetectIsEmptyJson())
            {
                yield return new ValidationResult("Value cannot be empty", new[] { "value" });
            }
            
            var model = _jsonSerializer.Deserialize<OpenStreetMapModel>(value.ToString());

            if (model?.Marker is null)
            {
                yield return new ValidationResult("The marker has to be set on this map", new[] { "value" });
            }
        }
    }
}