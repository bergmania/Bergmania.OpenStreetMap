import {customElement, html, LitElement, property, state} from '@umbraco-cms/backoffice/external/lit';
import {UmbElementMixin} from '@umbraco-cms/backoffice/element-api';
import {UmbPropertyEditorConfigCollection} from "@umbraco-cms/backoffice/property-editor";
// import { UMB_CURRENT_USER_CONTEXT } from '@umbraco-cms/backoffice/current-user';
import {OpenStreetMapModel} from "./models.ts";
import './auto-suggest.element.ts';

import './bergmania-openstreetmap.ts';
import { BermaniaOpenstreetmap } from './bergmania-openstreetmap.ts';
import { AutoSuggestElement } from './auto-suggest.element.ts';


@customElement('bergmania-openstreetmap-property-editor')
export default class BergmaniaPropertyEditorUIOpenStreetMapElement extends UmbElementMixin(LitElement) {

    @property()
    value: Partial<OpenStreetMapModel> = {};

    @state()
    private _showSearch: boolean = false;

    @state()
    private _showSetMarkerByCoordinates: boolean = false;

    @state()
    private _showCoordinates: boolean = false;

    @state()
    private _allowClear: boolean = false;

    @state()
    private _scrollWheelZoom: boolean = false; 
    
    @state()
    private _tileLayer: string = '';

    @state()
    private _tileLayerAttribution: string = '';

    @state()
    private _defaultPosition?: OpenStreetMapModel;
    
    @state()
    private inputLat?: number;

    @state()
    private inputLng?: number;

    public set config(config: UmbPropertyEditorConfigCollection | undefined) {
        this._showSearch = config?.getValueByAlias('showSearch') ?? false;
        this._showSetMarkerByCoordinates = config?.getValueByAlias('showSetMarkerByCoordinates') ?? false;
        this._showCoordinates = config?.getValueByAlias('showCoordinates') ?? false;
        this._allowClear = config?.getValueByAlias('allowClear') ?? false;
        this._scrollWheelZoom = config?.getValueByAlias('scrollWheelZoom') ?? false;
        this._tileLayer = config?.getValueByAlias('tileLayer') ?? '';
        this._tileLayerAttribution = config?.getValueByAlias('tileLayerAttribution') ?? '';
        this._defaultPosition = config?.getValueByAlias('defaultPosition');
    }

    async firstUpdated(changes:any) {
        super.firstUpdated(changes);
        this.inputLat = this.value.marker?.latitude;
        this.inputLng = this.value.marker?.longitude;
    }

    searchSelected(e: CustomEvent) {
        const selected = e.target!.value as any;

        this.value = {
            ...this.value,
            marker: {
                latitude: selected.value.marker[1],
                longitude: selected.value.marker[0]
            },
            boundingBox: {
                southWestCorner: {
                    latitude:selected.value.bbox[1],
                    longitude:selected.value.bbox[0]
                },
                northEastCorner: {
                    latitude:selected.value.bbox[3],
                    longitude:selected.value.bbox[2]
                }
            }
        }
    }

    private setMarker(e: CustomEvent) {
        this.value = {
            ...this.value,
            marker: {
                latitude: (this.shadowRoot?.getElementById('inputLat')! as any).value,
                longitude: (this.shadowRoot?.getElementById('inputLng')! as any).value
            }
        }
        console.log('setMarker', e, this);
    }

    clearMarker(e: CustomEvent) {
        console.log(e, this);
    }

    updateData(e: CustomEvent) {
        const mapEle = e.target as BermaniaOpenstreetmap;
        this.value = {
            boundingBox: mapEle.boundingBox,
            zoom: mapEle.zoom,
            marker: mapEle.markerLocation
        };
        console.log('updateData',e);
        this.inputLat = this.value.marker?.latitude;
        this.inputLng = this.value.marker?.longitude;
        this.dispatchEvent(new CustomEvent('property-value-change'));
    }

    async getSuggestion(term: string) {
        //TODO get this from somewhere
        const limit = 5;
        const language = 'da-dk';
        const api = `https://nominatim.openstreetmap.org/search?format=geojson&limit=${limit}&q=${encodeURI(term)}&accept-language=${language}`;

        return fetch(api).then(res=>res.json()).then(results => {
            return results.features.map((obj:any) => {
                return {
                    name: obj.properties.display_name,
                    value: {
                        marker: obj.geometry.coordinates,
                        bbox: obj.bbox
                    }
                }
            })
        });
    }

    
    // private async getUserLanguage() {
    //     return new Promise(res => {
    //         this.getContext(UMB_CURRENT_USER_CONTEXT).then(userContext => {
    //             this.observe(userContext.currentUser, user => {
    //                 res(user?.languageIsoCode);
    //             })
    //         });
    //     });  
    // }


    render() {
        return html`
            <p>${JSON.stringify(this.value)}</p>

            ${
                this._showSearch 
                    ? html`
                        <auto-suggest id="searchField" @autoSuggestSelect=${this.searchSelected} .getSuggestion=${this.getSuggestion}></auto-suggest>
                    `
                    : ''
            }

            ${
                this._showSetMarkerByCoordinates 
                    ? html`
                        <uui-input label="[LOCALIZE] Lat" @change=${this.setMarker} .value=${this.inputLat} id="inputLat"></uui-input>
                        <uui-input label="[LOCALIZE] Lng" @change=${this.setMarker} .value=${this.inputLng} id="inputLng"></uui-input>
                    `
                    : ''
            }
    
            <bergmania-openstreetmap
                .zoom=${this.value.zoom || 0}
                .markerLocation=${this.value.marker!}
                .boundingBox=${this.value.boundingBox!}
                @mapdata-changed=${this.updateData}
            ></bergmania-openstreetmap>


            ${
                this._showCoordinates || this._allowClear 
                    ? html`
                        [LOCALIZE] Lat: ${this.value.marker?.latitude}, 
                        [LOCALIZE] Lng: ${this.value.marker?.longitude}<br/>
                        <uui-button @click=${this.clearMarker} .disabled=${!this.value?.marker}>[LOCALIZE] Clear</uui-button>
                    `
                    : ''
            }


        `;
    }
}