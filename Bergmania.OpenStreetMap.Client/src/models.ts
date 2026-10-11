export type OpenStreetMapModel = {
    zoom: number,
    marker: LatitudeLongitudeModel | null,
    markers?: MarkerModel[],
    boundingBox: BoundingBoxModel
};

export type LatitudeLongitudeModel = {
    latitude: number,
    longitude: number
};

export type BoundingBoxModel = {
    northEastCorner: LatitudeLongitudeModel,
    southWestCorner: LatitudeLongitudeModel
};

export type MarkerModel = LatitudeLongitudeModel & {
    title?: string
};

export function sameMarkers(a: MarkerModel[], b: MarkerModel[]) {
    return a.length === b.length
        && a.every((marker, i) => marker.latitude === b[i].latitude
            && marker.longitude === b[i].longitude
            && (marker.title ?? '') === (b[i].title ?? ''));
}
