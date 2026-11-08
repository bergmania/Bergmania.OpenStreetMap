using Bergmania.OpenStreetMap.Core;
using FakeItEasy;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;

namespace Bergmania.OpenStreetMap.Core.Tests;

public class OpenStreetMapPropertyValueConverterTests
{
    private readonly IJsonSerializer _jsonSerializer = A.Fake<IJsonSerializer>();
    private readonly IPublishedPropertyType _propertyType = A.Fake<IPublishedPropertyType>();
    private readonly IPublishedElement _owner = A.Fake<IPublishedElement>();
    private readonly OpenStreetMapPropertyValueConverter _converter;

    public OpenStreetMapPropertyValueConverterTests()
    {
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
    }

    [Test]
    public async Task ConvertIntermediateToObject_EmptyString_ReturnsFallbackModel()
    {
        var result = Convert("  ");

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Zoom).IsEqualTo(16);
    }

    [Test]
    public async Task ConvertIntermediateToObject_ValidJson_ReturnsDeserializedModel()
    {
        var expected = new OpenStreetMapModel
        {
            Zoom = 12,
            Marker = new LatitudeLongitudeModel { Latitude = 55.0m, Longitude = 10.0m },
            BoundingBox = new BoundingBoxModel
            {
                SouthWestCorner = new LatitudeLongitudeModel { Latitude = 54.0m, Longitude = 9.0m },
                NorthEastCorner = new LatitudeLongitudeModel { Latitude = 56.0m, Longitude = 11.0m }
            }
        };

        A.CallTo(() => _jsonSerializer.Deserialize<OpenStreetMapModel>(A<string>.Ignored)).Returns(expected);

        var result = Convert("{\"zoom\":12}");

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Zoom).IsEqualTo(12);
        await Assert.That(result.Marker!.Latitude).IsEqualTo(55.0m);
    }

    [Test]
    public async Task ConvertIntermediateToObject_MultipleMarkers_SingleMode_TrimsToFirst()
    {
        var model = new OpenStreetMapModel
        {
            Zoom = 10,
            Markers = new[]
            {
                new MarkerModel { Latitude = 55.0m, Longitude = 10.0m, Title = "First" },
                new MarkerModel { Latitude = 56.0m, Longitude = 11.0m, Title = "Second" }
            },
            BoundingBox = new BoundingBoxModel
            {
                SouthWestCorner = new LatitudeLongitudeModel { Latitude = 54.0m, Longitude = 9.0m },
                NorthEastCorner = new LatitudeLongitudeModel { Latitude = 57.0m, Longitude = 12.0m }
            }
        };

        SetupDataType(new OpenStreetMapConfiguration { AllowMultipleMarkers = false });
        A.CallTo(() => _jsonSerializer.Deserialize<OpenStreetMapModel>(A<string>.Ignored)).Returns(model);

        var result = Convert("{\"markers\":[]}");

        await Assert.That(result!.Markers).HasCount().EqualTo(1);
        await Assert.That(result.Markers[0].Title).IsEqualTo("First");
    }

    [Test]
    public async Task ConvertIntermediateToObject_EmptyValue_MultipleMode_ReturnsModelWithoutMarkers()
    {
        SetupDataType(new OpenStreetMapConfiguration { AllowMultipleMarkers = true });

        var result = Convert("");

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Markers).HasCount().EqualTo(0);
        await Assert.That(result.Marker).IsNull();
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
        var dataType = new PublishedDataType(1, Constants.EditorAlias, "Umb.PropertyEditorUi.OpenStreetMap", new Lazy<object>(() => config));
        A.CallTo(() => _propertyType.DataType).Returns(dataType);
    }

    private OpenStreetMapModel? Convert(object? inter)
    {
        return _converter.ConvertIntermediateToObject(_owner, _propertyType, PropertyCacheLevel.Element, inter, false) as OpenStreetMapModel;
    }
}
