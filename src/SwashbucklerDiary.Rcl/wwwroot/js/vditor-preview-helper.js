export function previewVditor(dotNetCallbackRef, element, text, options, outline, outlineElement) {
    if (!element) {
        return;
    }

    if (!text) {
        element.innerHTML = '';
        return;
    }

    let VditorOptions = {
        ...options,
        after: () => {
            if (outline) {
                outlineRender(element, outlineElement);
            }
            handlePreviewElement(element);
            handleAnchorScroll();
            fixToc(element);
            fixAnchorLink(element);
            dotNetCallbackRef.invokeMethodAsync('After');
        }
    }
    Vditor.preview(element, text, VditorOptions);
}

function outlineRender(previewElement, outlineElement) {
    Vditor.outlineRender(previewElement, outlineElement);
    fixContents(outlineElement.firstElementChild, previewElement);
}

export function renderLazyLoadingImage(element) {
    if (!element) {
        return;
    }

    if (!("IntersectionObserver" in window)) {
        return;
    }

    if (window.vditorImageIntersectionObserver) {
        window.vditorImageIntersectionObserver.disconnect();
        element.querySelectorAll("img").forEach((imgElement) => {
            if (imgElement.getAttribute("data-src")) {
                imgElement.src = imgElement.getAttribute("data-src");
                imgElement.removeAttribute("data-src");
            }
        });
    }
}

function handlePreviewElement(previewElement) {
    handleVideo(previewElement);
    handleIframe(previewElement);
}

//fix Video Not Display First Frame
function handleVideo(element) {
    const videos = element.querySelectorAll("video");
    videos.forEach(video => {
        video.playsInline = "true";
        if (video.hasAttribute('src')) {
            const url = new URL(video.src);
            if (!url.hash) {
                video.src += '#t=0.1';
            }

            return;
        }

        const sources = video.querySelectorAll('source');

        sources.forEach(source => {
            const url = new URL(source.src);
            if (!url.hash) {
                source.src += '#t=0.1';
            }
        });
    });
}

//fix Iframe AllowFullscreen
function handleIframe(element) {
    const iframes = element.querySelectorAll("iframe");
    iframes.forEach(iframe => {
        iframe.allowFullscreen = true;
    });
}

function handleAnchorScroll() {
    if (!location.hash) return;

    const anchor = location.hash.substring(1); // 直接从第一个字符后开始截取，跳过"#"
    const targetElement = document.getElementById(anchor);
    if (targetElement) {
        targetElement.scrollIntoView();
    }
}

function fixContents(listenElement, previewElement) {
    if (!listenElement || !previewElement) {
        return;
    }

    listenElement.addEventListener('click', function (event) {
        const tocItem = event.target.closest('[data-target-id]');
        if (!tocItem) {
            return;
        }

        const targetId = tocItem.getAttribute('data-target-id');
        const targetElement = previewElement.querySelector('#' + targetId);
        if (targetElement) {
            setTimeout(() => {
                targetElement.scrollIntoView();
            }, 100);
        }
    });
}

function fixToc(previewElement) {
    fixContents(previewElement, previewElement);
}

function fixAnchorLink(element) {
    element.querySelectorAll("a").forEach(a => {
        const href = a.getAttribute('href');
        if (!href || !href.startsWith('#')) {
            return;
        }

        a.href = decodeURIComponent(href);
    });
}
