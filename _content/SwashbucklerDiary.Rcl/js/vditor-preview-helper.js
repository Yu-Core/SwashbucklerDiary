import { fixOutlientClick } from './markdown/fixMarkdownOutline.js'

// Store event listeners to prevent duplicates
const eventListeners = new WeakMap();

function preview(dotNetCallbackRef, previewElement, text, options, patch) {
    if (!previewElement) {
        return;
    }

    if (!text) {
        previewElement.innerHTML = '';
        return;
    }

    let VditorOptions = {
        ...options,
        after: () => {
            processMediaElements(previewElement);
            if (patch) {
                handleUrlHash();
                fixToc(previewElement);
                fixAnchorLinks(previewElement);
                fixAnchorLinkNavigate(dotNetCallbackRef, previewElement);
            }

            dotNetCallbackRef.invokeMethodAsync('After');
        }
    }
    Vditor.preview(previewElement, text, VditorOptions);
}

async function md2htmlPreview(dotNetCallbackRef, previewElement, text, options, patch) {
    if (!previewElement) {
        return;
    }

    if (!text) {
        previewElement.innerHTML = '';
        return;
    }

    let html = await Vditor.md2html(text, options);
    previewElement.innerHTML = html;
    previewElement.classList.add("vditor-reset");
    if (options.theme) {
        Vditor.setContentTheme(options.theme.current, options.theme.path);
    }

    processMediaElements(previewElement);
    if (patch) {
        handleUrlHash();
        fixToc(previewElement);
        fixAnchorLinks(previewElement);
        fixAnchorLinkNavigate(dotNetCallbackRef, previewElement);
    }

    dotNetCallbackRef.invokeMethodAsync('After');
}

function renderOutline(previewElement, outlineElement) {
    if (!previewElement || !outlineElement) {
        return;
    }

    Vditor.outlineRender(previewElement, outlineElement);
    fixOutlientClick(outlineElement.firstElementChild, previewElement);
}

function renderLazyLoadingImage(element) {
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

function processMediaElements(previewElement) {
    processVideos(previewElement);
    processIframes(previewElement);
}

//fix Video Not Display First Frame
function processVideos(element) {
    element.querySelectorAll("video").forEach(video => {
        video.playsInline = true;

        if (video.src) {
            const url = new URL(video.src);
            if (!url.hash) {
                video.src = `${video.src}#t=0.1`;
            }
            return;
        }

        video.querySelectorAll('source[src]').forEach(source => {
            const url = new URL(source.src);
            if (!url.hash) {
                source.src = `${source.src}#t=0.1`;
            }
        });
    });
}

//fix Iframe AllowFullscreen
function processIframes(element) {
    element.querySelectorAll("iframe").forEach(iframe => {
        iframe.allowFullscreen = true;
    });
}

function handleUrlHash() {
    if (!location.hash) return;

    const anchor = decodeURIComponent(location.hash.substring(1));
    const targetElement = document.getElementById(anchor);
    if (targetElement) {
        targetElement.scrollIntoView({ behavior: "smooth" });
    }
}

function fixToc(previewElement) {
    fixOutlientClick(previewElement, previewElement);
}

function fixAnchorLinks(element) {
    element.querySelectorAll("a[href^='#']").forEach(a => {
        a.href = decodeURIComponent(a.getAttribute('href'));
    });
}

function fixAnchorLinkNavigate(dotNetCallbackRef, element) {
    const eventListener = eventListeners.get(element);
    if (eventListener) {
        return;
    }

    const clickHandler = (event) => {
        const link = event.target.closest("a[href^='#']");
        if (!link) {
            return;
        }

        event.preventDefault();
        const hash = link.getAttribute('href');
        const url = new URL(window.location.href);
        url.hash = hash;
        dotNetCallbackRef.invokeMethodAsync('NavigateToReplace', url.toString());

        const targetElement = document.getElementById(hash.substring(1));
        if (targetElement) {
            setTimeout(() => {
                targetElement.scrollIntoView({ behavior: "smooth" });
            }, 100);
        }
    }

    element.addEventListener('click', clickHandler);
    eventListeners.set(element, clickHandler);
}

export { preview, md2htmlPreview, renderLazyLoadingImage, renderOutline }
