import { previewImage } from './previewMediaElement.js';
import { fixMobileOutlientClick } from './markdown/fixMarkdownOutline.js'

function handleCopy(dotNetCallbackRef, element) {
    element.querySelectorAll(".vditor-copy span").forEach(span => {
        span.addEventListener('click', function (event) {
            dotNetCallbackRef.invokeMethodAsync("Copy");
        });
    })
}

function afterMarkdown(dotNetCallbackRef, element, autoPlay, outlineElement) {
    if (!element) {
        return;
    }

    handleCopy(dotNetCallbackRef, element);
    previewImage(dotNetCallbackRef, element);

    if (autoPlay) {
        handleAutoPlay(element);
    }

    renderMobileOutline(dotNetCallbackRef, element, outlineElement);
}

function handleAutoPlay(element) {
    if (!element) {
        return;
    }
    const mediaElement = element.querySelector('audio, video');
    if (mediaElement) {
        // play() possible error
        mediaElement.autoplay = true;
    }
}

function renderMobileOutline(dotNetCallbackRef, previewElement, outlineElement) {
    Vditor.outlineRender(previewElement, outlineElement);
    fixMobileOutlientClick(dotNetCallbackRef, outlineElement.firstElementChild, previewElement);
}

export { afterMarkdown }
