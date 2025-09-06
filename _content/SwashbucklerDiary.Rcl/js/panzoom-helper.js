const _instances = new WeakMap();

export function init(el) {
    if (!el) {
        return;
    }

    const instance = panzoom(el, {
        maxZoom: 6,
        minZoom: 1,
        smoothScroll: false,
        zoomDoubleClickSpeed: 1,
        onTouch: function (e) {
            //This is necessary, otherwise the custom double - click event cannot be triggered on the touchscreen device
            return false; // tells the library to not preventDefault.
        },
        onDoubleClick: function (e) {
            reset(el);
            return true; // tells the library to not preventDefault, and not stop propagation
        }
    });

    _instances.set(el, instance);
}

export function reset(el) {
    if (!el) {
        return;
    }

    const instance = _instances.get(el);
    if (!instance) {
        return;
    }

    instance.zoomTo(0, 0, 1);
    instance.moveTo(0, 0);
    instance.zoomAbs(0, 0, 1);
}