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

export function imgDragAndDropForDesktop(element) {
    if (!element) {
        throw new Error('Element not found.');
    }

    let mouseX, mouseY, imgX, imgY;
    let isDragging = false;

    function handleMouseDown(event) {
        mouseX = event.clientX;
        mouseY = event.clientY;
        imgX = element.offsetLeft;
        imgY = element.offsetTop;
        isDragging = true;

        element.style.cursor = 'grabbing';

        document.addEventListener('mousemove', handleMouseMove);
        document.addEventListener('mouseup', handleMouseUp);
    }

    function handleMouseMove(event) {
        if (isDragging) {
            const deltaX = event.clientX - mouseX;
            const deltaY = event.clientY - mouseY;

            element.style.left = imgX + deltaX + 'px';
            element.style.top = imgY + deltaY + 'px';
        }
    }

    function handleMouseUp() {
        if (isDragging) {
            isDragging = false;
            element.style.cursor = 'grab';

            document.removeEventListener('mousemove', handleMouseMove);
            document.removeEventListener('mouseup', handleMouseUp);
        }
    }

    element.addEventListener('mousedown', handleMouseDown);

    element.addEventListener('dragstart', function (event) {
        event.preventDefault();
    });

    // Handle mouse leaving window
    window.addEventListener('mouseout', function (event) {
        if (event.target === document.body || event.target === document.documentElement) {
            handleMouseUp();
        }
    });
}

export function getStyle(element, styleProp) {
    if (element.currentStyle)
        return element.currentStyle[styleProp];
    else if (window.getComputedStyle)
        return document.defaultView.getComputedStyle(element, null).getPropertyValue(styleProp);
}
