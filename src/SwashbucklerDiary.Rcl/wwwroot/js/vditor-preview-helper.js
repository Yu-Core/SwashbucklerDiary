export function previewVditor(dotNetCallbackRef, element, text, options) {
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
            handlePreviewElement(element, dotNetCallbackRef);
            handleAnchorScroll();
            dotNetCallbackRef.invokeMethodAsync('After');
        }
    }
    Vditor.preview(element, text, VditorOptions);
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

function handlePreviewElement(previewElement, dotNetCallbackRef) {
    handleA(previewElement, dotNetCallbackRef);
    handleVideo(previewElement);
    handleIframe(previewElement);
}

//修复点击链接的一些错误
function handleA(element, dotNetCallbackRef) {
    const links = element.querySelectorAll("a"); // 获取所有a标签
    links.forEach(link => {
        let href = link.getAttribute('href');
        if (href && !href.includes(':')) {
            if (href.startsWith('#')) {
                link.addEventListener('click', function (event) {
                    event.preventDefault();
                    const anchor = href.split('#')[1];
                    const url = location.origin + location.pathname + location.search + href;
                    //can not use history.replaceState(null, '', url);
                    dotNetCallbackRef.invokeMethodAsync('ReplaceUrl', url);
                    const targetElement = document.getElementById(anchor);
                    if (targetElement) {
                        targetElement.scrollIntoView();
                    }
                });
            } else {
                href = "https://" + href;
            }
            link.setAttribute("href", href); // 修改每个a标签的href属性
        };
    });
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
    if (location.hash) {
        const anchor = location.hash.split('#')[1];
        if (anchor) {
            const targetElement = document.getElementById(anchor);
            if (targetElement) {
                targetElement.scrollIntoView();
            }
        }
    }
}
