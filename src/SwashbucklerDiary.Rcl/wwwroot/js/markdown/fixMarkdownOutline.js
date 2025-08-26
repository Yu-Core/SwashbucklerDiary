function fixOutlientClick(listenElement, previewElement) {
    if (!listenElement || !previewElement) {
        return;
    }

    listenElement.addEventListener('click', (event) => {
        scrollToTargetItem(previewElement, event.target);
    });
}

function scrollToTargetItem(previewElement, element) {
    const tocItem = element.closest('[data-target-id]');
    if (!tocItem) {
        return;
    }

    const targetId = tocItem.getAttribute('data-target-id');
    const targetElement = previewElement.querySelector('#' + targetId);
    if (targetElement) {
        setTimeout(() => {
            targetElement.scrollIntoView({ behavior: "smooth" });
        }, 100);
    }
}

function fixMobileOutlientClick(dotNetCallbackRef, listenElement, previewElement) {
    if (!listenElement || !previewElement) {
        return;
    }

    listenElement.addEventListener('click', (event) => {
        dotNetCallbackRef.invokeMethodAsync('CloseMobileOutline');
        setTimeout(() => {
            scrollToTargetItem(previewElement, event.target);
        }, 200);
    });
}

export { fixOutlientClick, fixMobileOutlientClick }