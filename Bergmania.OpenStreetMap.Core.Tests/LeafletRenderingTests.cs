using Bergmania.OpenStreetMap.Core;
using Umbraco.Extensions;

namespace Bergmania.OpenStreetMap.Core.Tests;

public class LeafletRenderingTests
{
    [Test]
    public async Task LeafletRendering_EncodesDataAttributes()
    {
        var model = CreateModel();
        model.Markers = new[]
        {
            new MarkerModel { Latitude = 55.0m, Longitude = 10.0m, Title = "it's a <script>test</script>" }
        };
        model.Configuration = new OpenStreetMapConfiguration();

        var html = model.LeafletRendering().ToString()!;

        await Assert.That(html).DoesNotContain("<script>");
        await Assert.That(html).Contains("&lt;script&gt;");
        await Assert.That(html).Contains("&#x27;");
        await Assert.That(html).Contains("data-openstreetmap=");
        await Assert.That(html).Contains("data-openstreetmapdefaultconfig=");
    }

    [Test]
    public async Task LeafletRendering_IncludesTileLayerConfig()
    {
        var model = CreateModel();
        model.Configuration = new OpenStreetMapConfiguration
        {
            TileLayer = "https://custom/{z}/{x}/{y}.png",
            TileLayerAttribution = "Custom tiles"
        };

        var html = model.LeafletRendering().ToString()!;

        await Assert.That(html).Contains("custom");
        await Assert.That(html).Contains("Custom tiles");
    }

    [Test]
    public async Task LeafletRendering_WithNullConfig_DoesNotThrow()
    {
        var model = CreateModel();
        model.Configuration = null;

        var html = model.LeafletRendering().ToString()!;

        await Assert.That(html).Contains("data-openstreetmapdefaultconfig=");
        await Assert.That(html).Contains("&quot;tileLayer&quot;:null");
    }

    private static OpenStreetMapModel CreateModel() => new()
    {
        Zoom = 10,
        Marker = new LatitudeLongitudeModel { Latitude = 55.0m, Longitude = 10.0m },
        BoundingBox = new BoundingBoxModel
        {
            SouthWestCorner = new LatitudeLongitudeModel { Latitude = 54.0m, Longitude = 9.0m },
            NorthEastCorner = new LatitudeLongitudeModel { Latitude = 56.0m, Longitude = 11.0m }
        }
    };
}
