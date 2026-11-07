# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Bergmania.OpenStreetMap is an Umbraco CMS property editor plugin that provides OpenStreetMap integration. It allows content editors to place markers on maps, with the location data persisted and available for frontend rendering.

## Build Commands

### .NET (from repository root)
```bash
dotnet build                          # Debug build
dotnet build --configuration Release  # Release build
dotnet pack                           # Create NuGet package
```

### Frontend (from Bergmania.OpenStreetMap.Client/)
```bash
npm ci           # Install dependencies
npm run build    # TypeScript + Vite compilation
npm run watch    # Watch mode for development
npm run dev      # Vite dev server
```

The StaticAssets project automatically runs `npm ci && npm run build` during .NET build (unless `UmbracoBuild` env var is set).

## Running the Test Site

```bash
cd Bergmania.OpenStreetMap.Testsite.V17
dotnet run
```
- URL: http://localhost:5000
- Login: me@mail.com / 1234567890

## Architecture

```
Bergmania.OpenStreetMap.Core/          # Backend C# library
├── OpenStreetMapPropertyEditor        # Umbraco property editor registration
├── OpenStreetMapPropertyValueConverter # JSON to model conversion
├── OpenStreetMapModel                 # Data model (Marker, Zoom, BoundingBox)
└── Extensions/                        # Leaflet rendering helpers

Bergmania.OpenStreetMap.Client/        # Frontend TypeScript/Lit
├── src/
│   ├── property-editor/               # Umbraco backoffice property editor
│   ├── components/                    # Lit web components (map, auto-suggest)
│   └── index.ts                       # Entry point with manifest registration

Bergmania.OpenStreetMap.StaticAssets/  # Razor SDK project for asset distribution
└── wwwroot/App_Plugins/               # Built frontend assets

Bergmania.OpenStreetMap/               # Main NuGet package (bundles Core + StaticAssets)
```

## Key Technologies

- **Backend**: .NET 10, Umbraco 17+ (supports 17-20)
- **Frontend**: TypeScript, Lit 3, Leaflet, Vite
- **Umbraco Integration**: `@umbraco-cms/backoffice` for UI components and APIs

## Releasing

Create a git tag with format `release/x.x.x` to trigger Azure Pipeline release to NuGet and GitHub.
