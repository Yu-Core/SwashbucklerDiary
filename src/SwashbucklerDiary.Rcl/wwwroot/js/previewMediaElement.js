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
            launchFullscreen(event.target);
        }
    });
}

function launchFullscreen(element) {
    if (element.requestFullscreen) {
        element.requestFullscreen();
    } else if (element.webkitRequestFullscreen) {
        element.webkitRequestFullscreen();
    } else if (element.webkitEnterFullscreen) {
        element.webkitEnterFullscreen();
    } else if (element.mozRequestFullScreen) {
        element.mozRequestFullScreen();
    } else if (element.msRequestFullscreen) {
        element.msRequestFullscreen();
    }
}
