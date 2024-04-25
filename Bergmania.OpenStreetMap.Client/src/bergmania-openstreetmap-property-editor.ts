import {customElement, html, LitElement, property, state} from '@umbraco-cms/backoffice/external/lit';
import {UmbElementMixin} from '@umbraco-cms/backoffice/element-api';
import {UmbPropertyEditorConfigCollection} from "@umbraco-cms/backoffice/property-editor";
import { UMB_CURRENT_USER_CONTEXT } from '@umbraco-cms/backoffice/current-user';
import {OpenStreetMapModel} from "./models.ts";
import './auto-suggest.element.ts';
import './bergmania-openstreetmap.ts';
import { BermaniaOpenstreetmap } from './bergmania-openstreetmap.ts';


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
    private inputLat?: string;

    @state()
    private inputLng?: string;

    searchLabel = 'Type to search';

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
        this.inputLat = ''+this.value.marker?.latitude;
        this.inputLng = ''+this.value.marker?.longitude;
    }

    searchSelected(e: CustomEvent) {
        const selected = (e.target as any).value as any;

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
    }

    clearMarker(e: CustomEvent) {
        this.value = {
            ...this.value,
            marker: null
        };
        this.inputLat = '';
        this.inputLng = '';

    }

    connectedCallback(): void {
        super.connectedCallback();
        this.searchLabel = this.localize.term('osm_searchPlaceholder');
    }

    updateData(e: CustomEvent) {
        const mapEle = e.target as BermaniaOpenstreetmap;
        this.value = {
            boundingBox: mapEle.boundingBox,
            zoom: mapEle.zoom,
            marker: mapEle.markerLocation
        };
        this.inputLat = ''+this.value.marker?.latitude;
        this.inputLng = ''+this.value.marker?.longitude;
        this.dispatchEvent(new CustomEvent('property-value-change'));
    }

    async getSuggestion(term: string) {
        const limit = 5;
        const language = await this.getUserLanguage();
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

    
    private async getUserLanguage() {
        return new Promise(res => {
            this.getContext(UMB_CURRENT_USER_CONTEXT).then(userContext => {
                this.observe(userContext.currentUser, user => {
                    res(user?.languageIsoCode);
                })
            });
        });  
    }


    render() {
        return html`
            ${
                this._showSearch 
                    ? html`
                        <auto-suggest 
                            id="searchField" 
                            placeholder=${this.searchLabel}
                            @autoSuggestSelect=${this.searchSelected} 
                            .getSuggestion=${this.getSuggestion.bind(this)}
                            style="display:block; width:100%; margin-bottom:10px;"
                        ></auto-suggest>
                    `
                    : ''
            }

            ${
                this._showSetMarkerByCoordinates 
                    ? html`
                        <div style="margin-bottom:10px;">
                            <umb-localize key="osm_latitude"></umb-localize>: <uui-input label="Lat" @change=${this.setMarker} .value=${this.inputLat} id="inputLat" style="margin-right:10px;"></uui-input>
                            <umb-localize key="osm_longitude"></umb-localize>: <uui-input label="Lng" @change=${this.setMarker} .value=${this.inputLng} id="inputLng"></uui-input>
                        </div>
                    `
                    : ''
            }
    
            <bergmania-openstreetmap
                .zoom=${this.value.zoom || 0}
                .markerLocation=${this.value.marker!}
                .boundingBox=${this.value.boundingBox!}
                @mapdata-changed=${this.updateData}
                .tileLayerPath=${this._tileLayer}
                .scrollWheelZoom=${this._scrollWheelZoom}
                .tileLayerOptions=${this._tileLayerAttribution ? {attribution: this._tileLayerAttribution} : null}
                style="margin-bottom:10px;"
            ></bergmania-openstreetmap>

            ${
                (this._showCoordinates || this._allowClear ) && this.value.marker
                    ? html`
                        <div style="clear:both;">
                            <umb-localize key="osm_latitude"></umb-localize>: ${this.value.marker?.latitude}, 
                            <umb-localize key="osm_longitude"></umb-localize>: ${this.value.marker?.longitude}
                            <uui-button @click=${this.clearMarker} .disabled=${!this.value?.marker} look="outline" style="float:right;"><umb-localize key="osm_clear"></umb-localize></uui-button>
                        </div>
                    `
                    : ''
            }


        `;
    }
} 