using Umbraco.Cms.Core.DependencyInjection;


namespace Bergmania.OpenStreetMap
{
    public static class UmbracoBuilderExtensions
    {
        public static IUmbracoBuilder AddOpenStreetMap(this IUmbracoBuilder builder)
        {
            builder.BackOfficeAssets()
                .Append<OpenStreetMapControllerJsFile>()
                .Append<LeafletJsFile>()
                .Append<LeafletCssFile>()
                ;

            return builder;
        }
    }
}