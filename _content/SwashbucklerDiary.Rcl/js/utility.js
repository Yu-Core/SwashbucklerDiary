// 通用的元素选择函数
function getElements(target) {
    if (target == null) {
        return [];
    }
    // 将单个元素或选择器转换为数组
    const elements = Array.isArray(target) ? target : [target];

    // 收集所有选中的元素
    const selectedElements = [];
    elements.forEach(item => {
        // 如果是选择器，使用 document.querySelectorAll 获取元素
        const foundElements = typeof item === 'string' ? document.querySelectorAll(item) : [item];
        selectedElements.push(...foundElements);
    });
    return selectedElements;
}

// 防止默认的 mousedown 事件
function preventDefaultOnmousedown(target) {
    const elements = getElements(target);
    elements.forEach(element => {
        if (element) {
            element.addEventListener("mousedown", e => {
                e.preventDefault();
            });
        }
    });
}

function evaluateJavascript(fn) {
    return new Function('return ' + fn)();
}
