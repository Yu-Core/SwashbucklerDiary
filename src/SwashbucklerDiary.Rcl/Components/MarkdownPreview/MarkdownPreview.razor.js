import { previewImage } from '../../js/previewMediaElement.js';
import { fixMobileOutlientClick } from '../../js/markdown/fixMarkdownOutline.js'
import { renderOutline } from './VditorMarkdownPreview.razor.js'
import { renderLinkCards } from '../../js/link-card.js'

function handleCopy(dotNetCallbackRef, element) {
    element.querySelectorAll(".vditor-copy span").forEach(span => {
        span.addEventListener('click', function (event) {
            dotNetCallbackRef.invokeMethodAsync("Copy");
        });
    })
}

function afterMarkdown(dotNetCallbackRef, element, options) {
    if (!element) {
        return;
    }

    handleCopy(dotNetCallbackRef, element);
    previewImage(dotNetCallbackRef, element);

    if (options.autoPlay) {
        handleAutoPlay(element);
    }

    if (options.linkBase) {
        handleLinkBase(element, options.linkBase);
    }

    renderOutline(element, options.outlineElement);
    renderMobileOutline(dotNetCallbackRef, element, options.moblieOutlineElement);

    if (options.linkCard) {
        handleLinkCard(element, options.proxyUrl);
    }
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

function renderMobileOutline(dotNetCallbackRef, previewElement, moblieOutlineElement) {
    if (!previewElement || !moblieOutlineElement) {
        return;
    }

    Vditor.outlineRender(previewElement, moblieOutlineElement);
    fixMobileOutlientClick(dotNetCallbackRef, moblieOutlineElement.firstElementChild, previewElement);
}

function handleLinkCard(element, proxyUrl) {
    if (!element) return;
    const options = {
        proxyUrl: 'https://api.allorigins.win/raw?url=',
        faviconSrc: "_content/SwashbucklerDiary.Rcl/img/link.svg"
    };
    if (proxyUrl) {
        options.proxyUrl = proxyUrl;
    }

    const baseURI = document.baseURI;
    const links = Array.from(element.querySelectorAll("a"))
        .filter(link => {
            const href = link.href;
            return href && href.startsWith('http') && !href.startsWith(baseURI);
        });
    renderLinkCards(links, options);
}

export { afterMarkdown, renderOutline }
