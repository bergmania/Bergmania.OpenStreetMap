using System.Text.Json;
using Bergmania.OpenStreetMap.Core;
using FakeItEasy;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;

namespace Bergmania.OpenStreetMap.Core.Tests;

public class OpenStreetMapPropertyValueConverterTests
{
    private readonly IJsonSerializer _jsonSerializer;
    private readonly IPublishedPropertyType _propertyType = A.Fake<IPublishedPropertyType>();
    private readonly IPublishedElement _owner = A.Fake<IPublishedElement>();
    private readonly OpenStreetMapPropertyValueConverter _converter;

    public OpenStreetMapPropertyValueConverterTests()
    {
        _jsonSerializer = new SystemTextJsonSerializer();
        _converter = new OpenStreetMapPropertyValueConverter(_jsonSerializer);
        SetupDataType(new OpenStreetMapConfiguration());
    }

    [Test]
    public async Task ConvertIntermediateToObject_NullInter_ReturnsFallbackModel()
    {
        var result = Convert(null);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Zoom).IsEqualTo(16);
        await Assert.That(result.BoundingBox).IsNotNull();
        await Assert.That(result.BoundingBox.SouthWestCorner.Latitude).IsEqualTo(54.970495269313204m);
    }

    [Test]
    public async Task ConvertIntermediateToObject_EmptyString_ReturnsFallbackModel()
    {
        var result = Convert("  ");

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Zoom).IsEqualTo(16);
    }

    [Test]
    public async Task ConvertIntermediateToObject_LegacyMarkerOnlyJson_ReturnsMarkerAsSingleMarkers()
    {
        const string json = """{"zoom":12,"marker":{"latitude":55.0,"longitude":10.0},"boundingBox":{"northEastCorner":{"latitude":56.0,"longitude":11.0},"southWestCorner":{"latitude":54.0,"longitude":9.0}}}""";

        var result = Convert(json);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Zoom).IsEqualTo(12);
        await Assert.That(result.Marker!.Latitude).IsEqualTo(55.0m);
        await Assert.That(result.Markers).HasCount().EqualTo(1);
        await Assert.That(result.Markers[0].Latitude).IsEqualTo(55.0m);
    }

    [Test]
    public async Task ConvertIntermediateToObject_NewMarkersJson_ReturnsAllMarkers()
    {
        const string json = """{"zoom":10,"marker":{"latitude":55.0,"longitude":10.0},"markers":[{"latitude":55.0,"longitude":10.0,"title":"A"},{"latitude":56.0,"longitude":11.0,"title":"B"}],"boundingBox":{"northEastCorner":{"latitude":57.0,"longitude":12.0},"southWestCorner":{"latitude":54.0,"longitude":9.0}}}""";

        SetupDataType(new OpenStreetMapConfiguration { AllowMultipleMarkers = true });
        var result = Convert(json);

        await Assert.That(result!.Markers).HasCount().EqualTo(2);
        await Assert.That(result.Markers[0].Title).IsEqualTo("A");
        await Assert.That(result.Markers[1].Title).IsEqualTo("B");
    }

    [Test]
    public async Task ConvertIntermediateToObject_MultipleMarkers_SingleMode_TrimsToFirst()
    {
        const string json = """{"zoom":10,"markers":[{"latitude":55.0,"longitude":10.0,"title":"First"},{"latitude":56.0,"longitude":11.0,"title":"Second"}],"boundingBox":{"northEastCorner":{"latitude":57.0,"longitude":12.0},"southWestCorner":{"latitude":54.0,"longitude":9.0}}}""";

        SetupDataType(new OpenStreetMapConfiguration { AllowMultipleMarkers = false });

        var result = Convert(json);

        await Assert.That(result!.Markers).HasCount().EqualTo(1);
        await Assert.That(result.Markers[0].Title).IsEqualTo("First");
    }

    [Test]
    public async Task ConvertIntermediateToObject_EmptyValue_MultipleMode_ReturnsModelWithoutMarkers()
    {
        var defaultPos = new OpenStreetMapModel
        {
            Zoom = 8,
            Marker = new LatitudeLongitudeModel { Latitude = 55.0m, Longitude = 10.0m },
            BoundingBox = new BoundingBoxModel
            {
                SouthWestCorner = new LatitudeLongitudeModel { Latitude = 54.0m, Longitude = 9.0m },
                NorthEastCorner = new LatitudeLongitudeModel { Latitude = 56.0m, Longitude = 11.0m }
            }
        };
        SetupDataType(new OpenStreetMapConfiguration { AllowMultipleMarkers = true, DefaultPosition = defaultPos });

        var result = Convert("");

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Markers).HasCount().EqualTo(0);
        await Assert.That(result.Marker).IsNull();
        await Assert.That(result.Zoom).IsEqualTo(8);
        await Assert.That(result).IsNotEqualTo(defaultPos);
    }

    [Test]
    public async Task ConvertIntermediateToObject_EmptyValue_SingleMode_ReturnsDefaultMarker()
    {
        var defaultPos = new OpenStreetMapModel
        {
            Zoom = 8,
            Marker = new LatitudeLongitudeModel { Latitude = 55.0m, Longitude = 10.0m },
            BoundingBox = new BoundingBoxModel
            {
                SouthWestCorner = new LatitudeLongitudeModel { Latitude = 54.0m, Longitude = 9.0m },
                NorthEastCorner = new LatitudeLongitudeModel { Latitude = 56.0m, Longitude = 11.0m }
            }
        };
        SetupDataType(new OpenStreetMapConfiguration { AllowMultipleMarkers = false, DefaultPosition = defaultPos });

        var result = Convert("");

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Marker).IsNotNull();
        await Assert.That(result.Marker!.Latitude).IsEqualTo(55.0m);
        await Assert.That(result.Zoom).IsEqualTo(8);
    }

    [Test]
    public async Task ConvertIntermediateToObject_SetsConfigurationOnModel()
    {
        var config = new OpenStreetMapConfiguration { TileLayer = "https://custom/{z}/{x}/{y}.png" };
        SetupDataType(config);

        var result = Convert(null);

        await Assert.That(result!.Configuration).IsNotNull();
        await Assert.That(result.Configuration!.TileLayer).IsEqualTo("https://custom/{z}/{x}/{y}.png");
    }

    private void SetupDataType(OpenStreetMapConfiguration config)
    {
        var dataType = new PublishedDataType(1, Constants.EditorAlias, "Umb.PropertyEditorUi.OpenStreetMap", new Lazy<object?>(() => config));
        A.CallTo(() => _propertyType.DataType).Returns(dataType);
    }

    private OpenStreetMapModel? Convert(object? inter)
    {
        return _converter.ConvertIntermediateToObject(_owner, _propertyType, PropertyCacheLevel.Element, inter, false) as OpenStreetMapModel;
    }

    private sealed class SystemTextJsonSerializer : IJsonSerializer
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        public string Serialize(object? input) => JsonSerializer.Serialize(input, Options);
        public T? Deserialize<T>(string input) => JsonSerializer.Deserialize<T>(input, Options);

        bool IJsonSerializer.TryDeserialize<T>(object input, out T? output) where T : class
        {
            try
            {
                output = JsonSerializer.Deserialize<T>(input.ToString() ?? string.Empty, Options);
                return true;
            }
            catch
            {
                output = default;
                return false;
            }
        }
    }
}
