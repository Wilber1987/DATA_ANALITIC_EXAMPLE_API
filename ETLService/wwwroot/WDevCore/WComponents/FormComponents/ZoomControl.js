//@ts-check

import { html } from "../../WModules/WComponentsTools.js";
import { css } from "../../WModules/WStyledRender.js";
/**
* @typedef {Object.<string, any>} ZoomControlConfig
* @property {HTMLElement[]} Nodes
*/

export class ZoomControl extends HTMLElement {

	/**
	 * @param {ZoomControlConfig} Config
	 */
	constructor(Config) {
		super();
		this.zoom = 1;
		/** @type {number} */
		this.minZoom = 0.5;
		/** @type {number} */
		this.maxZoom = 3;
		/** @type {number} */
		this.stepZoom = 0.1;
		/** @type {HTMLElement[]} */
		this.Nodes = Config.Nodes;
		this.attachShadow({ mode: 'open' });
		this.zoomSpam = html`<span>${(this.zoom * 100).toFixed(0)}%</span>`;
		this.shadowRoot?.append(this.CustomStyle);
	}

	connectedCallback() {
		this.render();
		this.addEventListener("wheel", this.handleWheel);
	}

	disconnectedCallback() {
		this.removeEventListener("wheel", this.handleWheel);
	}

	handleWheel = (event) => {
		event.preventDefault();
		this.zoom += event.deltaY > 0 ? -this.stepZoom : this.stepZoom;
		this.applyZoom();
	};

	zoomIn = () => {
		this.zoom = Math.min(this.zoom + this.stepZoom, this.maxZoom);
		this.applyZoom();
	}

	zoomOut = () => {
		this.zoom = Math.max(this.zoom - this.stepZoom, this.minZoom);
		this.applyZoom();
	}

	applyZoom = () => {
		this.zoomSpam.innerHTML = `${(this.zoom * 100).toFixed(0)}%`;
		this.Nodes.forEach(node => {
			node.style.zoom = `${this.zoom}`;
		});
	}

	render = () => {
		
		this.shadowRoot?.append(html`<div class="controls">
			<button onclick="${this.zoomOut}">-</button>
			${this.zoomSpam}
			<button onclick="${this.zoomIn}">+</button>
			<button onclick="${this.resetZoom}">🔄</button>
			
		</div>`);
	}
	resetZoom = () => {
		this.zoom = 1; // 🔄 Restablece el zoom a 100%
		this.applyZoom();
	}
	CustomStyle = css`
		:host {
			display: flex;
			flex-direction: column;
			align-items: center;
			gap: 10px;
		}
		.zoom-container {
			overflow: auto;
			border: 1px solid #ccc;
			padding: 10px;
			display: inline-block;
			transform-origin: center;
		}
		.controls {
			display: flex;
			gap: 5px;
			align-items: center;
		}
		button {
			padding: 5px 10px;
			border: none;
			background-color: #007bff;
			color: white;
			cursor: pointer;
			border-radius: 5px;
			font-weight: bold;
		}
		button:hover {
			background-color: #0056b3;
	}`;
}

customElements.define("zoom-control", ZoomControl);
