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

    handleCustomRender(element);
    if (autoPlay) {
        handleAutoPlay(element);
    }

    renderMobileOutline(dotNetCallbackRef, element, outlineElement);
}

function handleCustomRender(element) {
    if (!element) {
        return;
    }
    element.querySelectorAll("a").forEach((aElement) => {
        const url = aElement.getAttribute("href");
        if (!url) {
            return;
        }

        if (url.startsWith("https://ds.163.com/webapp/hearthstone/group/detail")
            || url.startsWith("https://www.iyingdi.com/share/deck/deck.html")) {
            insertDeckIframe(aElement, url);
        } else if (url.startsWith("https://hs.gameyw.netease.com/mobile/deck-detail")) {
            const nextSibling = aElement.nextSibling;

            if (nextSibling && nextSibling.nodeType === Node.TEXT_NODE) {
                const textContent = nextSibling.textContent.trim();
                if (textContent && textContent.includes("lushibox")) {
                    aElement.href += textContent;
                    nextSibling.remove();
                }
            }

            const href = aElement.getAttribute("href");
            const url = new URL(href);

            const id = url.pathname.split('/').pop();
            const hero = url.searchParams.get('hero');
            const name = url.searchParams.get('name');
            const mode = url.searchParams.get('mode');

            const newUrl = `https://ds.163.com/webapp/hearthstone/group/detail/${id}?hero=${hero}&iswild=${mode}&cgname=${name}&source=list`;
            insertDeckIframe(aElement, newUrl);
        } else if (url.startsWith("https://www.iyingdi.com/web/tools")) {
            const regex = /https:\/\/www\.iyingdi\.com\/web\/tools\/([^\/]+)\/decks\/deckdetail\/(\d+)/;
            const match = url.match(regex);

            if (match && match[1] && match[2]) {
                const game = match[1];
                const id = match[2];
                const url = `https://www.iyingdi.com/share/deck/deck.html?game=${game}&id=${id}`;

                insertDeckIframe(aElement, url);
            }
        }
    });
}

function insertDeckIframe(aElement, url) {
    aElement.insertAdjacentHTML("afterend",
        `<iframe class="iframe__deck" src="${url}"></iframe>`);
    aElement.remove();
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
