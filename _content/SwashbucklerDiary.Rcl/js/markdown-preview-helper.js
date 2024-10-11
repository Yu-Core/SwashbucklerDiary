import { previewImage } from './previewMediaElement.js';

function after(dotNetCallbackRef, element) {
    if (!element) {
        return;
    }

    copy(dotNetCallbackRef, element);
    previewImage(dotNetCallbackRef, element);
    handleA(dotNetCallbackRef, element);
}

function copy(dotNetCallbackRef, element) {
    element.addEventListener('click', function (event) {
        if (event.target.closest('.vditor-copy')) {
            dotNetCallbackRef.invokeMethodAsync("Copy");
        }
    });
}

function handleA(dotNetCallbackRef, element) {
    element.addEventListener('click', function (event) {
        var link = event.target.closest('a[href]');
        if (!link) {
            return;
        }

        let href = link.getAttribute('href');
        if (href.startsWith('#')) {
            event.preventDefault();
            const url = location.origin + location.pathname + location.search + href;
            dotNetCallbackRef.invokeMethodAsync('NavigateToReplace', url);

            const targetElement = document.getElementById(href.substring(1));
            if (targetElement) {
                setTimeout(() => {
                    targetElement.scrollIntoView();
                }, 100);
            }
        }
    });
}

function customRender(element) {
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

export { after, customRender }
