export function initZoom(selector) {
    var el = document.querySelector(selector);
    if (el == null) {
        return;
    }

    el.panzoom = panzoom(el, {
        maxZoom: 6,
        minZoom: 1,
        smoothScroll: false,
        zoomDoubleClickSpeed: 1,
        onTouch: function (e) {
            //This is necessary, otherwise the custom double - click event cannot be triggered on the touchscreen device
            return false; // tells the library to not preventDefault.
        },
        onDoubleClick: function (e) {
            reset(selector);
            return true; // tells the library to not preventDefault, and not stop propagation
        }
    });
}

export function reset(selector) {
    var el = document.querySelector(selector);
    if (el == null) {
        return;
    }

    el.panzoom.zoomTo(0, 0, 1);
    el.panzoom.moveTo(0, 0);
    el.panzoom.zoomAbs(0, 0, 1);
}