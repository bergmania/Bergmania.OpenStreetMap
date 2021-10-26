# Bergmania.OpenStreetMap [![NuGet](https://img.shields.io/nuget/v/Bergmania.OpenStreetMap.svg?style=modern&label=nuget)](https://www.nuget.org/packages/Bergmania.OpenStreetMap)
Basic OpenStreetMaps property editor for Umbraco 9 including property value converter.

## Features
- Click on exact location on map to place marker
- Drag marker around
- Set default bounding box & zoomlevel on Data Type settings
- Zoomlevel is saved on the proprety to use the same on your website
- Bounding box is saved on the proprety to use the same on your website

## Test site
A test side is included with basic content saved in a SqlCe db. 

### Authentication
**Username**: me@mail.com
**Password**: 1234567890

## Versioning and push to nuget
Currently no pipeline, just use `dotnet pack -c Release -o dist` and push manually

## Contributions
Thanks for the contributions
- [Bjarke Berg](https://github.com/bergmania) (Initial creator)
- [Bjarne Fyrstenborg](https://github.com/bjarnef)
