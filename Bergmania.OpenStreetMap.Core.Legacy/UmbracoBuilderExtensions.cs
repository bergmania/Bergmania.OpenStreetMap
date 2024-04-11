using Umbraco.Cms.Core.DependencyInjection;

namespace Bergmania.OpenStreetMap.Core
{
    public static class UmbracoBuilderExtensions
    {
        public static IUmbracoBuilder AddOpenStreetMap(this IUmbracoBuilder builder)
        {
            builder.BackOfficeAssets()
                .Append<AutocompleteJsFile>()
                .Append<AutocompleteCssFile>()
                .Append<OpenStreetMapControllerJsFile>()
                .Append<LeafletJsFile>()
                .Append<LeafletCssFile>()
                ;

            return builder;
        }
    }
}