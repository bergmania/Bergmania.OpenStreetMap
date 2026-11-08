import { css, html, LitElement, customElement } from '@umbraco-cms/backoffice/external/lit';
import {  Icon, LatLng, LeafletMouseEvent, Map, Marker, TileLayerOptions } from 'leaflet';

// @ts-ignore
import * as L from 'leaflet/dist/leaflet-src.esm.js';
import { BoundingBoxModel, MarkerModel, sameMarkers } from './models';

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

  static readonly styles = [css`
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

  private _markers: MarkerModel[] = [];
  set markers(markers: MarkerModel[]) {
    markers = markers ?? [];
    if(sameMarkers(markers, this._markers)) {
      return;
    }
    this._markers = markers;
    if(this._map) {
      this.renderMarkers();
    }
  };
  get markers() {
    return this._markers;
  }

  private _multiple: boolean = false;
  set multiple(value: boolean) {
    if(value === this._multiple) {
      return;
    }
    this._multiple = value;
    if(this._map) {
      this.renderMarkers();
    }
  }
  get multiple() {
    return this._multiple;
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
  tileLayerOptions: any = { attribution: 'Map data © OpenStreetMap contributors', referrerPolicy: 'strict-origin-when-cross-origin' };

  _map!: Map;
  _leafletMarkers: Marker[] = [];

  firstUpdated(_changedProperties: any) {
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

    this.renderMarkers();
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
    return L.tileLayer(tileLayePath || DEFAULT_TILELAYER, tileLayerOptions).addTo(this._map);
  }

  /**
   * Updates the Leaflet markers in place, reusing them by their place in the list, so typing a title
   * only changes that marker's label instead of recreating every marker.
   */
  private renderMarkers() {
    while(this._leafletMarkers.length > this._markers.length) {
      this._leafletMarkers.pop()!.remove();
    }
    this._markers.forEach((position, index) => {
      const latLng = L.latLng(position.latitude, position.longitude);
      let marker = this._leafletMarkers[index];
      if(!marker) {
        marker = this.createMarker(latLng, index);
        this._leafletMarkers.push(marker);
      } else if(!marker.getLatLng().equals(latLng)) {
        marker.setLatLng(latLng);
      }
      this.renderLabel(marker, position, index);
    });
  }

  private createMarker(latLng: LatLng, _index: number) {
    const marker: Marker = L.marker(latLng, { draggable: true, icon: ICON }).addTo(this._map);
    marker.on('dragend', () => {
      const currentIndex = this._leafletMarkers.indexOf(marker);
      if (currentIndex < 0) return;
      const moved = {latitude: marker.getLatLng().lat, longitude: marker.getLatLng().lng};
      this._markers = this._markers.map((existing, i) => i === currentIndex ? {...existing, ...moved} : existing);
      this.updateModel();
    });
    return marker;
  }

  /** Numbers the markers when there can be several, so they can be matched with the list below the map. */
  private renderLabel(marker: Marker, position: MarkerModel, index: number) {
    if(!this._multiple) {
      marker.unbindTooltip();
      return;
    }
    const text = position.title ? `${index + 1}. ${position.title}` : `${index + 1}`;
    const tooltip = marker.getTooltip();
    const current = tooltip?.getContent();
    if(current instanceof HTMLElement && current.textContent === text) {
      return;
    }
    // set as text, so the title is never treated as HTML
    const label = document.createElement('span');
    label.textContent = text;
    if(tooltip) {
      marker.setTooltipContent(label);
    } else {
      marker.bindTooltip(label, { permanent: true, direction: 'top', offset: [0, -41] });
    }
  }

  private setZoom(level:number) {
    this._map.setZoom(level);
  }

  private updateModel() {
    this.dispatchEvent(new CustomEvent('mapdata-changed', {bubbles: true}))
  }
  private onMapClick(e: LeafletMouseEvent) {
    const position: MarkerModel = {latitude: e.latlng.lat, longitude: e.latlng.lng};
    this._markers = this._multiple ? [...this._markers, position] : [position];
    this.renderMarkers();
    if(!this._multiple) {
      this._map.setView(e.latlng);
    }
    this.updateModel();
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