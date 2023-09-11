import '/npm/zoom/zoom.min.js';
export function initZoom(selector) {
    var el = document.querySelector(selector);
    if (el == null) {
        return;
    }

    el.zm = new Zoom(el, {
        rotate: false,
        minZoom: 1,
        maxZoom: 4,
        pan: false
    });
}

export function reset(selector) {
    var el = document.querySelector(selector);
    if (el == null) {
        return;
    }

    el.zm.reset();
}
