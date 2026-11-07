# Bergmania.OpenStreetMap [![NuGet](https://img.shields.io/nuget/v/Bergmania.OpenStreetMap.svg?style=modern&label=nuget)](https://www.nuget.org/packages/Bergmania.OpenStreetMap)
OpenStreetMaps property editor for Umbraco 17 and 18 including property value converter.

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

With multiple markers, new content starts without any markers. The Data Type's default position only sets the zoom and area the map starts on, and its marker is not used.

The markers are available as `Markers` on the model. `Marker` is still there and always holds the first one, so existing templates keep working:

```cshtml
@foreach (var marker in Model.Map.Markers)
{
    <p>@marker.Title: @marker.Latitude, @marker.Longitude</p>
}
```

Each marker can be given an optional title in the list below the map, e.g. the name of a branch or office.

`@Model.Map.LeafletRendering()` shows all markers, with the title in a popup when a marker is clicked. When there is more than one marker, the map zooms out from the saved view as needed so that every marker is visible. Rendering the model directly (`@Model.Map`) uses the OpenStreetMap embed, which only supports a single marker and shows the first one.

If a Data Type is switched back to a single marker, the extra markers stay stored but are not shown or exposed, until an editor moves or clears the marker.

With the Content Delivery API, the property value includes a `markers` array next to `marker`, and each marker can have a `title`.

## Upgrading to 9.0.0
Version 9 adds multiple markers. Existing Data Types, content and templates keep working without changes, but note:

- **Version 9 supports Umbraco 17 and 18.** Version 8 allowed installing on Umbraco 17 to 20, but 19 and 20 were never tested, so version 9 can't be installed on them yet.
- **Stored values gain a `markers` field.** Values saved by older versions are read as a single marker.
- **Downgrading is not safe once editors have saved more than one marker.** Older versions keep only the first marker the next time the map is edited. For the same reason, editors should hard refresh the backoffice after upgrading, so that a cached copy of the old editor is not used.
- **Code that writes map values** (imports, migrations, custom code) should set both `marker` and `markers`. When both are present, `markers` is used.
- **`LeafletRendering()` works again and uses the tile layer and attribution configured on the Data Type.** In 8.x the Data Type settings were not passed to the map, so the Leaflet script failed before adding the marker.
- **`LeafletRendering()` now HTML-encodes its `data-openstreetmap` attributes.** Scripts reading them with `getAttribute` are not affected.

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
