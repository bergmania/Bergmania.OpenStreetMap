using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Bergmania.OpenStreetMap
{
    public class OpenStreetMapComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddOpenStreetMap();
        }
    }
}