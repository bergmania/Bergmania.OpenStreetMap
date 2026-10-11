# Bergmania.OpenStreetMap [![NuGet](https://img.shields.io/nuget/v/Bergmania.OpenStreetMap.svg?style=modern&label=nuget)](https://www.nuget.org/packages/Bergmania.OpenStreetMap)
OpenStreetMaps property editor for Umbraco 10+ including property value converter.

## Features
- Click on exact location on map to place marker
- Drag marker around
- Set default bounding box & zoom level on Data Type settings
- Zoom level is saved on the property to use the same on your website
- Bounding box is saved on the property to use the same on your website
- Set the marker on specific coordinates
- Optionally allow multiple markers on the same map

## Multiple markers
Enable **Allow Multiple Markers** on the Data Type to let editors place more than one marker. It is off by default, so existing Data Types keep working as before.

The markers are available as `Markers` on the model. `Marker` is still there and always holds the first one, so existing templates keep working:

```cshtml
@foreach (var marker in Model.Map.Markers)
{
    <p>@marker.Latitude, @marker.Longitude</p>
}
```

`@Model.Map.LeafletRendering()` shows all markers. Rendering the model directly (`@Model.Map`) uses the OpenStreetMap embed, which only supports a single marker and shows the first one.

If a Data Type is switched back to a single marker, the extra markers stay stored but are not shown or exposed, until an editor moves or clears the marker.

## Test site
A test side is included with basic content saved in a SqLite. 

### Authentication
**Username**: me@mail.com
**Password**: 1234567890

## Versioning and push to nuget
Make a git-tag with this format `release/x.x.x`. The pipeline will pick it up and release to nuget and create github release.

## Contributions
Thanks for the contributions
- [Bjarke Berg](https://github.com/bergmania) (Initial creator)
- [Filip Bech-Larsen](https://github.com/filipbech) (Maintainer)
- [Bjarne Fyrstenborg](https://github.com/bjarnef)
- [Arjan](https://github.com/creativesuspects)
- [Yevhen Duyun](https://github.com/duyun-yevhen)
- [David Thibault](https://github.com/leddt)
