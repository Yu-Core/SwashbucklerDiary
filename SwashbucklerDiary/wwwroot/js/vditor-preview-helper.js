export function previewVditor(dotNetCallbackRef, element, text, options) {
    let VditorOptions = {
        ...options,
        after: () => {
            dotNetCallbackRef.invokeMethodAsync('After');
        }
    }
    Vditor.preview(element, text, VditorOptions);
}

export function copy(dotNetCallbackRef, callbackMethod) {
    var elements = document.querySelectorAll('.vditor-copy');
    for (var i = 0; i < elements.length; i++) {
        elements[i].addEventListener('click', function () {
            dotNetCallbackRef.invokeMethodAsync(callbackMethod);
        });
    }
}

export function fixLink(element) {
    var links = element.querySelectorAll("a"); // 获取所有a标签
    for (var i = 0; i < links.length; i++) {
        var href = links[i].getAttribute('href');
        if (href && !href.includes(':')) {
            href = "https://" + href;
            links[i].setAttribute("href", href); // 修改每个a标签的href属性
        }

    }
}

export function previewImage(dotNetCallbackRef, callbackMethod, parent) {
    var elements = parent.querySelectorAll("img");
    for (var i = 0; i < elements.length; i++) {
        elements[i].addEventListener('click', function () {
            dotNetCallbackRef.invokeMethodAsync(callbackMethod, this.src);
        });
    }
}
