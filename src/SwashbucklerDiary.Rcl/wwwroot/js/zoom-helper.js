export function initZoom(element) {
    if (!element) {
        throw new Error('Element not found.');
    }

    const panzoom = Panzoom(element, {
        minScale: 1,
        maxScale: 5,
    });

    panzoom.zoom(1, { animate: true })

    element.parentElement.addEventListener('wheel', function (event) {
        panzoom.zoomWithWheel(event)
    })

    element.panzoom = panzoom;
}

export function reset(element) {
    if (!element) {
        throw new Error('Element not found.');
    }

    element.panzoom.reset();
}
