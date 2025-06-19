import { fixOutlientClick } from './markdown/fixMarkdownOutline.js'

function preview(dotNetCallbackRef, previewElement, text, options, outlineElement) {
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
            if (outlineElement) {
                renderOutline(previewElement, outlineElement);
            }
            handlePreviewElement(previewElement);
            handleUrlHash();
            fixToc(previewElement);
            fixAnchorLink(previewElement);
            dotNetCallbackRef.invokeMethodAsync('After');
        }
    }
    Vditor.preview(previewElement, text, VditorOptions);
}

async function md2htmlPreview(dotNetCallbackRef, previewElement, text, options) {
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

    handlePreviewElement(previewElement);
    handleUrlHash();
    fixToc(previewElement);
    fixAnchorLink(previewElement);
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
                video.src = video.getAttribute("src") + '#t=0.1';
            }

            return;
        }

        const sources = video.querySelectorAll('source');

        sources.forEach(source => {
            if (!source.hasAttribute('src')) {
                return;
            }

            const url = new URL(source.src);
            if (!url.hash) {
                source.src = video.getAttribute("src") + '#t=0.1';
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

function handleUrlHash() {
    if (!location.hash) return;

    let anchor = location.hash.substring(1); // 直接从第一个字符后开始截取，跳过"#"
    anchor = decodeURIComponent(anchor);
    const targetElement = document.getElementById(anchor);
    if (targetElement) {
        targetElement.scrollIntoView();
    }
}

function fixToc(previewElement) {
    fixOutlientClick(previewElement, previewElement);
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

function fixAnchorLinkNavigate(dotNetCallbackRef, element) {
    element.addEventListener('click', function (event) {
        var link = event.target.closest('a[href]');
        if (!link) {
            return;
        }

        let href = link.getAttribute('href');
        if (href.startsWith('#')) {
            event.preventDefault();
            const hash = href;
            const url = new URL(window.location.href);
            url.hash = hash;
            dotNetCallbackRef.invokeMethodAsync('NavigateToReplace', url.toString());

            const targetElement = document.getElementById(hash.substring(1));
            if (targetElement) {
                setTimeout(() => {
                    targetElement.scrollIntoView();
                }, 100);
            }
        }
    });
}

export { preview, md2htmlPreview, renderLazyLoadingImage, renderOutline, fixAnchorLinkNavigate }
