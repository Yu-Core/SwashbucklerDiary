/**
 * 点击Vditor工具栏，输入框不失去焦点
 * @returns
 */
export function PreventInputLoseFocus() {
    var toolbar = document.getElementsByClassName("vditor-toolbar")[0];
    if (toolbar == null || toolbar == undefined || toolbar == "") {
        console.log("Vditor toolbar does not exist");
        return;
    }
    toolbar.onmousedown = (e) => {
        e.preventDefault();
    }
}

export function PreviewVditor(dotNetCallbackRef, element, text, options) {
    let VditorOptions = {
        ...options,
        after: () => {
            dotNetCallbackRef.invokeMethodAsync('After');
        }
    }
    Vditor.preview(element, text, VditorOptions);
}

export function Copy(dotNetCallbackRef, callbackMethod) {
    var elements = document.querySelectorAll('.vditor-copy');
    for (var i = 0; i < elements.length; i++) {
        elements[i].addEventListener('click', function () {
            dotNetCallbackRef.invokeMethodAsync(callbackMethod);
        });
    }
}

export function FixLink(element) {
    var links = element.querySelectorAll("a"); // 获取所有a标签
    for (var i = 0; i < links.length; i++) {
        var href = links[i].getAttribute('href');
        if (href && !href.includes(':')) {
            href = "https://" + href;
            links[i].setAttribute("href", href); // 修改每个a标签的href属性
        }

    }
}
