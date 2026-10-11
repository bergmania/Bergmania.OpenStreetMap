import {customElement, html, LitElement, property, state} from '@umbraco-cms/backoffice/external/lit';
import {UmbChangeEvent} from '@umbraco-cms/backoffice/event';
import {UmbElementMixin} from '@umbraco-cms/backoffice/element-api';
import {UmbPropertyEditorConfigCollection} from "@umbraco-cms/backoffice/property-editor";
import { UMB_CURRENT_USER_CONTEXT } from '@umbraco-cms/backoffice/current-user';
import { BoundingBoxModel, LatitudeLongitudeModel, OpenStreetMapModel, sameMarkers } from "./models.ts";
import './auto-suggest.element.ts';
import { UUIInputElement } from '@umbraco-cms/backoffice/external/uui';
import './bergmania-openstreetmap.ts';               
import type {BermaniaOpenstreetmap} from "./bergmania-openstreetmap.ts";     

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
    private _multiple: boolean = false;

    @state()
    private _scrollWheelZoom: boolean = false; 
    
    @state()
    private _tileLayer: string = '';

    @state()
    private _tileLayerAttribution: string = '';

    @state()
    private _defaultPosition?: OpenStreetMapModel; /** BE AWARE THAT THIS IS ACTUALLY DEFAULT VALUE */
    
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
        this._multiple = config?.getValueByAlias('allowMultipleMarkers') ?? false;
        this._scrollWheelZoom = config?.getValueByAlias('scrollWheelZoom') ?? false;
        this._tileLayer = config?.getValueByAlias('tileLayer') ?? '';
        this._tileLayerAttribution = config?.getValueByAlias('tileLayerAttribution') ?? '';
        this._defaultPosition = config?.getValueByAlias('defaultPosition');
    }
    

    /** Everything stored on the value, including values saved before multiple markers were supported. */
    private storedMarkers(): LatitudeLongitudeModel[] | undefined {
        if (this.value?.markers?.length) {
            return this.value.markers;
        }
        if (this.value?.marker) {
            return [this.value.marker];
        }
        // an explicitly cleared value should not fall back to the default position
        return this.value?.marker === null || this.value?.markers ? [] : undefined;
    }

    /** The markers shown on the map: only the first one unless multiple markers are allowed. */
    private get markers(): LatitudeLongitudeModel[] {
        const markers = this.storedMarkers()
            ?? this._defaultPosition?.markers
            ?? (this._defaultPosition?.marker ? [this._defaultPosition.marker] : []);
        return this._multiple ? markers : markers.slice(0, 1);
    }

    private setMarkers(markers: LatitudeLongitudeModel[], boundingBox?: BoundingBoxModel) {
        this.value = {
            ...this.value,
            zoom: this.value?.zoom ?? this._defaultPosition?.zoom,
            boundingBox: boundingBox ?? this.value?.boundingBox ?? this._defaultPosition?.boundingBox,
            marker: markers[0] ?? null,
            markers
        };
        if (this._multiple) {
            this.clearInputs();
        } else {
            this.inputLat = markers[0] ? ''+markers[0].latitude : '';
            this.inputLng = markers[0] ? ''+markers[0].longitude : '';
        }
        this.dispatchEvent(new UmbChangeEvent());
    }

    /** Adds a marker when multiple markers are allowed, otherwise replaces the current one. */
    private placeMarker(marker: LatitudeLongitudeModel, boundingBox: BoundingBoxModel) {
        this.setMarkers(this._multiple ? [...this.markers, marker] : [marker], boundingBox);
    }

    private clearInputs() {
        this.inputLat = '';
        this.inputLng = '';
        // the inputs keep what was typed when the bound value did not change
        ['inputLat', 'inputLng'].forEach(id => {
            const input = this.shadowRoot?.getElementById(id) as UUIInputElement | null;
            if (input) {
                input.value = '';
            }
        });
    }

    async firstUpdated(changes:any) {
        super.firstUpdated(changes);
        if(!this._multiple && this.value?.marker) { 
            this.inputLat = ''+this.value.marker?.latitude;
            this.inputLng = ''+this.value.marker?.longitude;
        }
    }

    searchSelected(e: CustomEvent) {
        const selected = (e.target as any).value;

        this.placeMarker(
            {
                latitude: selected.value.marker[1],
                longitude: selected.value.marker[0]
            },
            {
                southWestCorner: {
                    latitude:selected.value.bbox[1],
                    longitude:selected.value.bbox[0]
                },
                northEastCorner: {
                    latitude:selected.value.bbox[3],
                    longitude:selected.value.bbox[2]
                }
            }
        );
    }

    private setMarker() {
        const lat = Number.parseFloat((this.shadowRoot?.getElementById('inputLat') as UUIInputElement).value as string);
        const lng = Number.parseFloat((this.shadowRoot?.getElementById('inputLng') as UUIInputElement).value as string);
        if (!Number.isFinite(lat) || !Number.isFinite(lng)) {
            return;
        }
        this.placeMarker({ latitude: lat, longitude: lng }, this.boundingBoxFromPoint(lat, lng));
    }

    private handlePaste(e: ClipboardEvent) {
        const text = e.clipboardData?.getData('text');
        if (!text) return;

        const match = /^(-?\d+(?:\.\d+)?)\s*,\s*(-?\d+(?:\.\d+)?)$/.exec(text.trim());
        if (match) {
            e.preventDefault();
            const lat = Number.parseFloat(match[1]);
            const lng = Number.parseFloat(match[2]);
            this.placeMarker({ latitude: lat, longitude: lng }, this.boundingBoxFromPoint(lat, lng));
        }
    }

    private boundingBoxFromPoint(lat: number, lng: number) {
        const zoom = this.value?.zoom ?? 16;
        const offset = 180 / Math.pow(2, zoom);
        return {
            southWestCorner: { latitude: lat - offset, longitude: lng - offset },
            northEastCorner: { latitude: lat + offset, longitude: lng + offset }
        };
    }

    private async copyCoordinates(marker: LatitudeLongitudeModel) {
        await navigator.clipboard.writeText(`${marker.latitude},${marker.longitude}`);
    }

    private removeMarker(index: number) {
        this.setMarkers(this.markers.filter((_, i) => i !== index));
    }

    clearMarker() {
        this.setMarkers([]);
    }

    connectedCallback(): void {
        super.connectedCallback();
        this.searchLabel = this.localize.term('osm_searchPlaceholder');
    }

    updateData(e: CustomEvent) {
        const mapEle = e.target as BermaniaOpenstreetmap;
        // Moving or zooming the map leaves the stored markers alone, so the ones hidden by a
        // single-marker data type survive until the marker itself is changed.
        const markersChanged = !sameMarkers(mapEle.markers, this.markers);
        const markers = markersChanged ? mapEle.markers : this.storedMarkers() ?? mapEle.markers;
        this.value = {
            boundingBox: mapEle.boundingBox,
            zoom: mapEle.zoom,
            marker: markers[0] ?? null,
            markers
        };
        if(!this._multiple && this.value?.marker) { 
            this.inputLat = ''+this.value.marker?.latitude;
            this.inputLng = ''+this.value.marker?.longitude;
        }
        this.dispatchEvent(new UmbChangeEvent());
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
                if (!userContext) {
                    res(undefined);
                    return;
                }
                this.observe(userContext.currentUser, user => {
                    res(user?.languageIsoCode);
                })
            });
        });  
    }


    private renderMarkerList(markers: LatitudeLongitudeModel[]) {
        return html`
            <div style="clear:both;">
                ${markers.map((marker, index) => html`
                    <div style="display:flex; align-items:center; gap:10px;">
                        <strong>${index + 1}.</strong>
                        ${
                            this._showCoordinates
                                ? html`
                                    <span>
                                        <umb-localize key="osm_latitude"></umb-localize>: ${marker.latitude},
                                        <umb-localize key="osm_longitude"></umb-localize>: ${marker.longitude}
                                    </span>
                                    <uui-button @click=${() => this.copyCoordinates(marker)} look="default" compact label="Copy coordinates"><umb-icon name="icon-documents"></umb-icon></uui-button>
                                `
                                : html`<umb-localize key="osm_marker"></umb-localize>`
                        }
                        <uui-button @click=${() => this.removeMarker(index)} look="default" compact label=${this.localize.term('osm_removeMarker')} style="margin-left:auto;"><umb-icon name="icon-trash"></umb-icon></uui-button>
                    </div>
                `)}
                ${
                    this._allowClear && markers.length > 1
                        ? html`<uui-button @click=${this.clearMarker} look="secondary" label=${this.localize.term('osm_clearAll')}></uui-button>`
                        : ''
                }
            </div>
        `;
    }

    render() {
        const markers = this.markers;
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
                            <umb-localize key="osm_latitude"></umb-localize>: <uui-input label="Lat" @change=${this._multiple ? null : this.setMarker} @paste=${this.handlePaste} .value=${this.inputLat} id="inputLat" style="margin-right:10px;"></uui-input>
                            <umb-localize key="osm_longitude"></umb-localize>: <uui-input label="Lng" @change=${this._multiple ? null : this.setMarker} @paste=${this.handlePaste} .value=${this.inputLng} id="inputLng"></uui-input>
                            ${
                                this._multiple
                                    ? html`<uui-button @click=${this.setMarker} look="secondary" label=${this.localize.term('osm_addMarker')} style="margin-left:10px;"></uui-button>`
                                    : ''
                            }
                        </div>
                    `
                    : ''
            }
    
            <bergmania-openstreetmap
                .zoom=${this.value?.zoom ? this.value.zoom : this._defaultPosition?.zoom!}
                .markers=${markers}
                .multiple=${this._multiple}
                .boundingBox=${this.value?.boundingBox ? this.value.boundingBox : this._defaultPosition?.boundingBox!}
                @mapdata-changed=${this.updateData}
                .tileLayerPath=${this._tileLayer}
                .scrollWheelZoom=${this._scrollWheelZoom}
                .tileLayerOptions=${this._tileLayerAttribution ? {attribution: this._tileLayerAttribution, referrerPolicy: 'strict-origin-when-cross-origin'} : null}
                style="margin-bottom:10px;"
            ></bergmania-openstreetmap>

            ${
                this._multiple
                    ? this.renderMarkerList(markers)
                    : (this._showCoordinates || this._allowClear ) && markers[0]
                        ? html`
                            <div style="clear:both;">
                                <umb-localize key="osm_latitude"></umb-localize>: ${markers[0].latitude},
                                <umb-localize key="osm_longitude"></umb-localize>: ${markers[0].longitude}
                                <uui-button @click=${() => this.copyCoordinates(markers[0])} look="default" compact label="Copy coordinates"><umb-icon name="icon-documents"></umb-icon></uui-button>
                                <uui-button @click=${this.clearMarker} look="default" compact label="Clear marker" style="float:right;"><umb-icon name="icon-trash"></umb-icon></uui-button>
                            </div>
                        `
                        : ''
            }

        `;
    }
} 