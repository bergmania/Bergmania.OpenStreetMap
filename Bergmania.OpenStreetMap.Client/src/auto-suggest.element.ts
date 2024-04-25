import { LitElement, PropertyValueMap, css, customElement, html, property } from '@umbraco-cms/backoffice/external/lit';

function debounced(delay:number, fn:(...args:any)=> void) {
	let timerId:number|null;
	return function(...args:any[]) {
		if (timerId) clearTimeout(timerId);
		timerId = setTimeout(() => { fn(...args); timerId = null }, delay)
	}
}

@customElement('auto-suggest')
export class AutoSuggestElement extends LitElement {

    static styles? = [css`
        :host {
            display:block;
            width:100%;
            position: relative;
            display: inline-flex;
            align-items: stretch;
            height: var(--uui-input-height, var(--uui-size-11, 33px));
            text-align: left;
            box-sizing: border-box;
            background-color: var(--uui-input-background-color, var(--uui-color-surface, #fff));
            border: var(--uui-input-border-width, 1px) solid var(--uui-input-border-color, var(--uui-color-border, #d8d7d9));
            --uui-button-height: 100%;
            --auto-width-text-margin-right: 0;
            --auto-width-text-margin-left: 0;
        }
        :host(:focus-within) {
            border-color: var(--uui-input-border-color-focus, var(--uui-color-border-emphasis, #a1a1a1));
            outline: calc(2px* var(--uui-show-focus-outline, 1)) solid var(--uui-color-focus, #3879ff);
        }
        label, label > strong {
            display: block;
        }
        input {
            font-family: inherit;
            padding: var(--uui-size-1, 3px) var(--uui-size-space-3, 9px);
            font-size: inherit;
            color: inherit;
            border-radius: 0;
            box-sizing: border-box;
            border: none;
            background: none;
            width: 100%;
            height: inherit;
            text-align: inherit;
            outline: none;
        }
        input[type="search"] {
            -webkit-appearance: none
        }

        input[type="search"]::-webkit-search-cancel-button {
            --clear-icon: url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"><path d="M17.016 15.609l-3.609-3.609 3.609-3.609-1.406-1.406-3.609 3.609-3.609-3.609-1.406 1.406 3.609 3.609-3.609 3.609 1.406 1.406 3.609-3.609 3.609 3.609zM12 2.016q4.125 0 7.055 2.93t2.93 7.055-2.93 7.055-7.055 2.93-7.055-2.93-2.93-7.055 2.93-7.055 7.055-2.93z"/></svg>');
            background-color: currentColor;
            display: block;
            height: 1rem;
            mask-image: var(--clear-icon);
            width: 1rem;
            -webkit-appearance: none;
            -webkit-mask-image: var(--clear-icon);
        }
        [list]::-webkit-calendar-picker-indicator {
            display: none !important;
        }
    `]

    input!: HTMLInputElement;
    list!: HTMLDataListElement;
    data: any[] = [];

    value: any;

    @property()
    placeholder = "";

    protected firstUpdated(_changedProperties: PropertyValueMap<any> | Map<PropertyKey, unknown>): void {
        this.input = this.shadowRoot?.getElementById('searchField') as HTMLInputElement;
        this.list = this.shadowRoot?.getElementById('suggestions') as HTMLDataListElement;

        this.input.addEventListener('input', debounced(200, this.onEntry.bind(this)));
        // this.input.addEventListener('keyup', debounced(200, this.onKeyup.bind(this)));

        this.input.addEventListener('search', () => {
            this.input.value.length === 0  
                ? this.reset() 
                : ''
        });

    }

    async getSuggestion(term: string): Promise<any[]> {
        console.warn('You need to implement your own getSuggestion method. Searching for: '+term);
        return [];
    }

    reset() { 
        this.data = []; 
        this.list.innerHTML = `<option value="">`;  
        this.input.setCustomValidity('');
    }

    async onEntry(event:any) {
        const value = this.input.value.length >= this.input.minLength && this.input.value.toLowerCase();
		if (!value) return;

		/* “onselect” for datalist, work for both clicks and “Enter”: */
		if (event.inputType == "insertReplacementText" || event.inputType == null) {
			const option = [...this.list.options].filter(entry => entry.value === this.input.value)?.[0]
			if (option) {
				/* Dispatch the selected option as a custom event, reset the list */
                this.value = JSON.parse(option.dataset.obj as string);
				this.dispatchEvent(new CustomEvent('autoSuggestSelect', { }));
				this.reset();
			}
			return;
		}

        this.data = await this.getSuggestion(value);
        this.list.innerHTML = this.data.map(obj => `<option value="${obj.name}" data-obj='${obj ? JSON.stringify(obj):''}'>`).join('')

	}

    render() {
        return html`
            <input 
                autocapitalize="off"
                autocomplete="off"
                autocorrect="off"
                type="search" 
                id="searchField" 
                placeholder=${this.placeholder}
                list="suggestions" />
            <datalist id="suggestions"><option value=""></option></datalist>
        `;
    }
}