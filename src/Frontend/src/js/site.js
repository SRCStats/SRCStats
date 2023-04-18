// js vendors
import twemoji from 'twemoji';
import $ from 'jquery';
window.jQuery = $;
window.$ = $;
import { Collapse, Tooltip } from "bootstrap";

window.Dropdown = $.fn.dropdown;
import 'bootstrap-select';

// css vendors
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap-icons/font/bootstrap-icons.css';
import 'bootstrap-select/dist/css/bootstrap-select.css';

// css
import '../css/site.scss';

twemoji.parse(document.body);

const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]')
const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new Tooltip(tooltipTriggerEl))