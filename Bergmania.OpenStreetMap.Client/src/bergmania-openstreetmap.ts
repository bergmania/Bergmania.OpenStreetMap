import { css, html, LitElement, customElement } from '@umbraco-cms/backoffice/external/lit';
import {  Icon, LeafletMouseEvent, Map, Marker, TileLayerOptions } from 'leaflet';

// @ts-ignore
import * as L from 'leaflet/dist/leaflet-src.esm.js';
import { BoundingBoxModel, LatitudeLongitudeModel } from './models';

const DEFAULT_BOUNDING_BOX = {"southWestCorner":{"latitude":54.970495269313204,"longitude":-1.6278648376464846},"northEastCorner":{"latitude":54.97911600936982,"longitude":-1.609625816345215}};

const DEFAULT_TILELAYER = 'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png';

const ICON: Icon = new L.Icon({
  iconUrl: '/App_Plugins/Bergmania.OpenStreetMap/marker-icon.png',
  shadowUrl: '/App_Plugins/Bergmania.OpenStreetMap/marker-shadow.png',
  iconSize: [25, 41],
  iconAnchor: [12, 41],
  popupAnchor: [1, -34],
  shadowSize: [41, 41],
});

@customElement('bergmania-openstreetmap')
export class BermaniaOpenstreetmap extends LitElement {

  static styles = [css`
    :host {
      display:block;
    }
    .map {
      width:100%;
      height:400px;
    }
  `];

  scrollWheelZoom: boolean = false;


  private _boundingBox: BoundingBoxModel = DEFAULT_BOUNDING_BOX;
  set boundingBox(boundingBox: BoundingBoxModel) {
    if(!boundingBox || boundingBox === this._boundingBox) {
      return;
    }
    this._boundingBox = boundingBox;
    if(this._map) {
      this.fitBounds(boundingBox);
    }
  };
  get boundingBox() {
    return this._boundingBox || DEFAULT_BOUNDING_BOX;
  }

  private _markerLocation!:LatitudeLongitudeModel;
  set markerLocation(location: LatitudeLongitudeModel) {
    if(!location || location === this._markerLocation) {
      return;
    }
    this._markerLocation = location;
    if(this._map) {
      this.updateMarker(location);
    }
  };
  get markerLocation() {
    return this._markerLocation;
  }

  private _zoom: number = 16;
  set zoom(value) {
    if(this._map && value != this._zoom) {
      this.setZoom(value);
    }
  } 
  get zoom() {
    return this._zoom || 16;
  }


  tileLayerPath: string = DEFAULT_TILELAYER;
  tileLayerOptions: any | null = { attribution: 'Map data © OpenStreetMap contributors' };

  _map!: Map;
  _marker?: Marker;

  async firstUpdated(_changedProperties: any) {
    super.firstUpdated(_changedProperties);

    const mapDomElement = this.renderRoot.querySelector('.map') as HTMLElement;
    if (!mapDomElement) {
      console.warn('no DOM element found. Bailing!')
      return;
    }

    //create the map
    this._map = L.map(mapDomElement,{ scrollWheelZoom: this.scrollWheelZoom });

    //initial position and tiles
    this.fitBounds(this.boundingBox);
    this.tileLayer(this.tileLayerPath, this.tileLayerOptions);

    // add map-listeners
    this._map.on('click', this.onMapClick.bind(this));
    this._map.on('moveend', this.onMoveEnd.bind(this));
    this._map.on('zoomend', this.onZoomEnd.bind(this));

    //just disabling right-click
    this._map.on('contextmenu', () => {});

    this.updateMarker(this.markerLocation, true);
  }

  private fitBounds(boundingBox: BoundingBoxModel) {
    this._boundingBox = boundingBox;
    return this._map.fitBounds(
      L.latLngBounds(
        L.latLng(boundingBox.southWestCorner.latitude, boundingBox.southWestCorner.longitude),
        L.latLng(boundingBox.northEastCorner.latitude, boundingBox.northEastCorner.longitude)
      )
    );
  }

  private tileLayer(tileLayePath:string , tileLayerOptions: TileLayerOptions) {
    return L.tileLayer(tileLayePath ? tileLayePath : DEFAULT_TILELAYER, tileLayerOptions).addTo(this._map);
  }

  private clearMarker() {
    this._marker!.remove();
    this._marker = undefined;
  }

  private updateMarker(position: LatitudeLongitudeModel, first?: boolean) {
    if(this._marker) {
      this.clearMarker();
    }
    this._marker = L.marker(L.latLng(position.latitude, position.longitude), { draggable: true, icon: ICON }).addTo(this._map);
    this._markerLocation = position;
    if(!first) {
      this._map.setView(L.latLng(position.latitude, position.longitude));
      this.updateModel();
    }
    this._marker!.on('dragend', () => {
      this._markerLocation = {latitude: this._marker!.getLatLng().lat, longitude: this._marker!.getLatLng().lng};
      this.updateModel()
    });    
  }

  private setZoom(level:number) {
    this._map.setZoom(level);
  }

  private updateModel() {
    this.dispatchEvent(new CustomEvent('mapdata-changed', {bubbles: true}))
  }
  private onMapClick(e: LeafletMouseEvent) {
    this.updateMarker({latitude:e.latlng.lat, longitude: e.latlng.lng} as LatitudeLongitudeModel);
  }
  private onMoveEnd() {
    this._boundingBox = {
      northEastCorner:{
        latitude: this._map.getBounds().getNorthEast().lat,
        longitude: this._map.getBounds().getNorthEast().lng
      },
      southWestCorner:{
        latitude: this._map.getBounds().getSouthWest().lat,
        longitude: this._map.getBounds().getSouthWest().lng
      }
    }
    this.updateModel();
  }
  private onZoomEnd() {
    this._zoom = this._map.getZoom();
    this.onMoveEnd();
  }


  render() {
    return html`<link rel="stylesheet" href="https://unpkg.com/leaflet@${L.version}/dist/leaflet.css" />
      <div class="map"></div>`;
  }


}