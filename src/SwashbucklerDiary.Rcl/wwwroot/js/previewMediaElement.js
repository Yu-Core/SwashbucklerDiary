export function previewImage(dotNetCallbackRef, callbackMethod, element) {
    element.addEventListener('click', function (event) {
        if (event.target.tagName === 'IMG') {
            dotNetCallbackRef.invokeMethodAsync(callbackMethod, event.target.getAttribute('src'));
        }
    });
}

export function previewVideo(element) {
    element.addEventListener('click', function (event) {
        if (event.target.tagName === 'VIDEO') {
            if (event.target.requestFullscreen) {
                event.target.requestFullscreen();
            }
        }
    });
}
