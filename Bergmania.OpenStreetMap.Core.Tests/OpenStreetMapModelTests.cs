using System.Text.Encodings.Web;
using Bergmania.OpenStreetMap.Core;

namespace Bergmania.OpenStreetMap.Core.Tests;

public class OpenStreetMapModelTests
{
    [Test]
    public async Task Markers_WhenOnlyMarkerSet_ReturnsSingleItemList()
    {
        var model = new OpenStreetMapModel
        {
            Marker = new LatitudeLongitudeModel { Latitude = 55.0m, Longitude = 10.0m }
        };

        await Assert.That(model.Markers).HasCount().EqualTo(1);
        await Assert.That(model.Markers[0].Latitude).IsEqualTo(55.0m);
        await Assert.That(model.Markers[0].Longitude).IsEqualTo(10.0m);
    }

    [Test]
    public async Task Markers_WhenMarkersSet_ReturnsMarkers()
    {
        var markers = new[]
        {
            new MarkerModel { Latitude = 55.0m, Longitude = 10.0m, Title = "A" },
            new MarkerModel { Latitude = 56.0m, Longitude = 11.0m, Title = "B" }
        };

        var model = new OpenStreetMapModel { Markers = markers };

        await Assert.That(model.Markers).HasCount().EqualTo(2);
        await Assert.That(model.Markers[0].Title).IsEqualTo("A");
        await Assert.That(model.Markers[1].Title).IsEqualTo("B");
    }

    [Test]
    public async Task Markers_WhenNeitherSet_ReturnsEmptyList()
    {
        var model = new OpenStreetMapModel();

        await Assert.That(model.Markers).HasCount().EqualTo(0);
    }

    [Test]
    public async Task Marker_WhenOnlyMarkersSet_ReturnsFirstMarker()
    {
        var model = new OpenStreetMapModel
        {
            Markers = new[]
            {
                new MarkerModel { Latitude = 55.0m, Longitude = 10.0m },
                new MarkerModel { Latitude = 56.0m, Longitude = 11.0m }
            }
        };

        await Assert.That(model.Marker).IsNotNull();
        await Assert.That(model.Marker!.Latitude).IsEqualTo(55.0m);
    }

    [Test]
    public async Task Marker_WhenOnlyMarkerSet_ReturnsMarker()
    {
        var model = new OpenStreetMapModel
        {
            Marker = new LatitudeLongitudeModel { Latitude = 55.0m, Longitude = 10.0m }
        };

        await Assert.That(model.Marker).IsNotNull();
        await Assert.That(model.Marker!.Latitude).IsEqualTo(55.0m);
    }

    [Test]
    public async Task Marker_WhenNeitherSet_ReturnsNull()
    {
        var model = new OpenStreetMapModel();

        await Assert.That(model.Marker).IsNull();
    }

    [Test]
    public async Task Markers_WhenBothSet_PrefersMarkers()
    {
        var model = new OpenStreetMapModel
        {
            Marker = new LatitudeLongitudeModel { Latitude = 1.0m, Longitude = 2.0m },
            Markers = new[]
            {
                new MarkerModel { Latitude = 55.0m, Longitude = 10.0m, Title = "From markers" }
            }
        };

        await Assert.That(model.Markers).HasCount().EqualTo(1);
        await Assert.That(model.Markers[0].Title).IsEqualTo("From markers");
    }

    [Test]
    public async Task Marker_WhenBothSet_ReturnsLegacyMarker()
    {
        var model = new OpenStreetMapModel
        {
            Marker = new LatitudeLongitudeModel { Latitude = 1.0m, Longitude = 2.0m },
            Markers = new[]
            {
                new MarkerModel { Latitude = 55.0m, Longitude = 10.0m }
            }
        };

        await Assert.That(model.Marker!.Latitude).IsEqualTo(1.0m);
        await Assert.That(model.Marker.Longitude).IsEqualTo(2.0m);
    }

    [Test]
    public async Task WriteTo_WithMarker_IncludesMarkerInUrl()
    {
        var model = new OpenStreetMapModel
        {
            Zoom = 10,
            Marker = new LatitudeLongitudeModel { Latitude = 55.5m, Longitude = 10.5m },
            BoundingBox = CreateBoundingBox()
        };

        var html = RenderModel(model);

        await Assert.That(html).Contains("&amp;marker=55.5%2C10.5");
    }

    [Test]
    public async Task WriteTo_WithoutMarker_OmitsMarkerParam()
    {
        var model = new OpenStreetMapModel
        {
            Zoom = 10,
            BoundingBox = CreateBoundingBox()
        };

        var html = RenderModel(model);

        await Assert.That(html).DoesNotContain("marker=");
    }

    [Test]
    public async Task Markers_FallbackFromMarker_ReturnsSameReference()
    {
        var model = new OpenStreetMapModel
        {
            Marker = new LatitudeLongitudeModel { Latitude = 55.0m, Longitude = 10.0m }
        };

        var first = model.Markers;
        var second = model.Markers;

        await Assert.That(first).IsSameReferenceAs(second);
    }

    [Test]
    public async Task Markers_FallbackCache_ClearedWhenMarkerSet()
    {
        var model = new OpenStreetMapModel
        {
            Marker = new LatitudeLongitudeModel { Latitude = 55.0m, Longitude = 10.0m }
        };

        var before = model.Markers;
        model.Marker = new LatitudeLongitudeModel { Latitude = 60.0m, Longitude = 12.0m };
        var after = model.Markers;

        await Assert.That(before).IsNotSameReferenceAs(after);
        await Assert.That(after[0].Latitude).IsEqualTo(60.0m);
    }

    private static BoundingBoxModel CreateBoundingBox() => new()
    {
        SouthWestCorner = new LatitudeLongitudeModel { Latitude = 54.0m, Longitude = -2.0m },
        NorthEastCorner = new LatitudeLongitudeModel { Latitude = 56.0m, Longitude = 0.0m }
    };

    private static string RenderModel(OpenStreetMapModel model)
    {
        var writer = new StringWriter();
        model.WriteTo(writer, HtmlEncoder.Default);
        return writer.ToString();
    }
}
