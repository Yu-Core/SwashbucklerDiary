function fixOutlientClick(listenElement, previewElement) {
    if (!listenElement || !previewElement) {
        return;
    }

    listenElement.addEventListener('click', (event) => {
        scrollToTargetItem(previewElement, event.target);
    });
}

function scrollToTargetItem(previewElement, element) {
    if (element.classList.contains("vditor-outline__action")) return;

    const tocItem = element.closest('[data-target-id]');
    if (!tocItem) {
        return;
    }

    const targetId = tocItem.getAttribute('data-target-id');
    if (!targetId) {
        return;
    }

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
        if (event.target.classList.contains("vditor-outline__action")) {
            return;
        }

        dotNetCallbackRef.invokeMethodAsync('CloseMobileOutline');
        setTimeout(() => {
            scrollToTargetItem(previewElement, event.target);
        }, 200);
    });
}

export { fixOutlientClick, fixMobileOutlientClick }