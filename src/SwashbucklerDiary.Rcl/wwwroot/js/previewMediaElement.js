export function previewImage(dotNetCallbackRef, element) {
    if (!element) {
        return;
    }

    element.addEventListener('click', function (event) {
        if (event.target.tagName === 'IMG') {
            dotNetCallbackRef.invokeMethodAsync("PreviewImage", event.target.getAttribute('src'));
        }
    });
}

export function previewVideo(element) {
    if (!element) {
        return;
    }

    element.addEventListener('click', function (event) {
        if (event.target.tagName === 'VIDEO') {
            const video = event.target;
            makeVideoFullscreen(video);
            if (video.mozRequestFullScreen) {
                video.setAttribute('controls', '');
                document.addEventListener('fullscreenchange', function () {
                    video.removeAttribute('controls');
                }, { once: true });
            }
        }
    });
}

// video element Fullscreen
function makeVideoFullscreen(video) {
    if (video.requestFullscreen) {
        video.requestFullscreen();
    } else if (video.webkitEnterFullscreen) { /* MacCatalyst WebView */
        video.webkitEnterFullscreen();
    } else if (video.mozRequestFullScreen) { /* Firefox */
        video.mozRequestFullScreen();
    } else if (video.webkitRequestFullscreen) { /* Chrome, Safari & Opera */
        video.webkitRequestFullscreen();
    } else if (video.msRequestFullscreen) { /* IE/Edge */
        video.msRequestFullscreen();
    }
}
