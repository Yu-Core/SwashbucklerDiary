export function previewVditor(dotNetCallbackRef, element, text, options) {
    let VditorOptions = {
        ...options,
        after: () => {
            fixLink(element);
            fixCopyDisplaySoftKeyboard(element);
            fixVideoNotDisplayFirstFrame(element);
            dotNetCallbackRef.invokeMethodAsync('After');
        }
    }
    Vditor.preview(element, text, VditorOptions);
}

export function copy(dotNetCallbackRef, callbackMethod, parent) {
    const elements = parent.querySelectorAll('.vditor-copy');
    for (var i = 0; i < elements.length; i++) {
        elements[i].addEventListener('click', function () {
            dotNetCallbackRef.invokeMethodAsync(callbackMethod);
        });
    }
}

export function previewImage(dotNetCallbackRef, callbackMethod, element) {
    const imgs = element.querySelectorAll("img");
    for (var i = 0; i < imgs.length; i++) {
        imgs[i].addEventListener('click', function () {
            dotNetCallbackRef.invokeMethodAsync(callbackMethod, this.getAttribute('src'));
        });
    }
}

//修复点击链接的一些错误
function fixLink(element) {
    const links = element.querySelectorAll("a"); // 获取所有a标签
    for (var i = 0; i < links.length; i++) {
        var href = links[i].getAttribute('href');
        if (href && !href.includes(':')) {
            href = "https://" + href;
            links[i].setAttribute("href", href); // 修改每个a标签的href属性
        }
    }
}

function fixCopyDisplaySoftKeyboard(element) {
    const textareas = element.querySelectorAll("textarea"); // 获取所有textarea标签
    for (var i = 0; i < textareas.length; i++) {
        textareas[i].readOnly = true;
    }
}

function fixVideoNotDisplayFirstFrame(element) {
    const videos = element.querySelectorAll("video");
    for (var i = 0; i < videos.length; i++) {
        videos[i].addEventListener('loadeddata', function () {
            const url = new URL(this.currentSrc);
            if (!url.hash) {
                videoElement.currentTime = 0;
            }
        });
    }
}
