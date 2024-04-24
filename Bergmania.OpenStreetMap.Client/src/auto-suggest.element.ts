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
        label, label > strong {
            display: block;
        }
        [type="search"] {
            border: 1px solid #AAA;
            font-size: 1rem;
            margin-block: 0.5rem;
            min-inline-size: 20rem;
            padding: 0.5rem 0.75rem;
            -webkit-appearance: none
        }

        [type="search"]::-webkit-search-cancel-button {
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

    protected firstUpdated(_changedProperties: PropertyValueMap<any> | Map<PropertyKey, unknown>): void {
        this.input = this.shadowRoot?.getElementById('searchField') as HTMLInputElement;
        this.list = this.shadowRoot?.getElementById('suggestions') as HTMLDataListElement;

        this.input.addEventListener('input', debounced(200, this.onEntry.bind(this)));
        // this.input.addEventListener('keyup', debounced(200, this.onKeyup.bind(this)));

        this.input.addEventListener('search', () => {
            console.log('search');
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
                type="text" 
                id="searchField" 
                list="suggestions" />
            <datalist id="suggestions"><option value=""></option></datalist>
        `;
    }
}