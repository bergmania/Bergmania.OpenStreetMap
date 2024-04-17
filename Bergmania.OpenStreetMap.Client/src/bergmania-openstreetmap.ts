import {css, customElement, html, LitElement, property, state} from '@umbraco-cms/backoffice/external/lit';
import {UmbElementMixin} from '@umbraco-cms/backoffice/element-api';
import {UmbPropertyEditorConfigCollection} from "@umbraco-cms/backoffice/property-editor";
import {OpenStreetMapModel} from "./models.ts";


@customElement('bergmania-openstreetmap-property-editor')
export default class BergmaniaPropertyEditorUIOpenStreetMapElement extends UmbElementMixin(LitElement) {

    static styles = [
        css`

        `,
    ];
    @property()
    value: OpenStreetMapModel | undefined;

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
    
    // private onInput(e: InputEvent) {
    //   this.value = (e.target as HTMLInputElement).value;
    //   this.dispatchEvent(new CustomEvent('property-value-change'));
    // }

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

    render() {
        return html`
            <p>${JSON.stringify(this.value)}</p>
            <bergmania-openstreetmap
                    .showSearch="${this._showSearch}"
                    .showSetMarkerByCoordinates="${this._showSetMarkerByCoordinates}"
                    .showCoordinates="${this._showCoordinates}"
                    .allowClear="${this._allowClear}"
                    .scrollWheelZoom="${this._scrollWheelZoom}"
                    .tileLayer="${this._tileLayer}"
                    .tileLayerAttribution="${this._tileLayerAttribution}"
                    .defaultPosition="${this._defaultPosition}"
                    .position="${this.value}"
            ></bergmania-openstreetmap>`;
    }
}