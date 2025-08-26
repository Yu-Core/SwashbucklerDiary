import { previewImage } from './previewMediaElement.js';
import { fixMobileOutlientClick } from './markdown/fixMarkdownOutline.js'

function handleCopy(dotNetCallbackRef, element) {
    element.querySelectorAll(".vditor-copy span").forEach(span => {
        span.addEventListener('click', function (event) {
            dotNetCallbackRef.invokeMethodAsync("Copy");
        });
    })
}

function afterMarkdown(dotNetCallbackRef, element, autoPlay, outlineElement, linkBase) {
    if (!element) {
        return;
    }

    handleCopy(dotNetCallbackRef, element);
    previewImage(dotNetCallbackRef, element);

    if (autoPlay) {
        handleAutoPlay(element);
    }

    if (linkBase) {
        handleLinkBase(element, linkBase);
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

function handleLinkBase(element, linkBase) {
    if (!element) {
        return;
    }
    element.querySelectorAll("a").forEach(a => {
        let href = a.href;

        // 删除指定的基础路径
        if (href.startsWith(linkBase)) {
            href = href.substring(linkBase.length);

            if (href.startsWith('/')) {
                href = href.substring(1);
            }

            a.href = href;
        }
    })
}

function renderMobileOutline(dotNetCallbackRef, previewElement, outlineElement) {
    Vditor.outlineRender(previewElement, outlineElement);
    fixMobileOutlientClick(dotNetCallbackRef, outlineElement.firstElementChild, previewElement);
}

export { afterMarkdown }
