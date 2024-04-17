export type OpenStreetMapModel = {
    zoom: number,
    marker: LatitudeLongitudeModel,
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