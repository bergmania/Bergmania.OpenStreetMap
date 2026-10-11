export type OpenStreetMapModel = {
    zoom: number,
    marker: LatitudeLongitudeModel | null,
    markers?: LatitudeLongitudeModel[],
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

export function sameMarkers(a: LatitudeLongitudeModel[], b: LatitudeLongitudeModel[]) {
    return a.length === b.length
        && a.every((marker, i) => marker.latitude === b[i].latitude && marker.longitude === b[i].longitude);
}
