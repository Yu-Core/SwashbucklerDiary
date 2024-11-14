export function after(dotNetCallbackRef, domRef) {
    handleToolbar(dotNetCallbackRef, domRef);
}

export function focus(domRef) {
    domRef.Vditor.focus();
}

export function autofocus(domRef) {
    domRef.Vditor.focus();
    const focusElement = document.activeElement;
    let range = document.createRange();
    range.selectNodeContents(focusElement);
    range.collapse(false);
    let sel = window.getSelection();
    sel.removeAllRanges();
    sel.addRange(range);
}

export function upload(element, inputFileElement) {
    if (!element || !inputFileElement) return;
    const uploadElement = element.querySelector('input[type=file]');
    inputFileElement.files = uploadElement.files;
    inputFileElement.dispatchEvent(new CustomEvent('change'));
}

function handleToolbar(dotNetCallbackRef, domRef) {
    if (!domRef) {
        return;
    }
    //点击Vditor工具栏，输入框不失去焦点
    var toolbar = domRef.querySelector(".vditor-toolbar");
    if (!toolbar) {
        console.log("Vditor toolbar does not exist");
        return;
    }
    //prevent Input Lose Focus
    toolbar.onmousedown = (e) => {
        e.preventDefault();
    };

    const toolbarContent = document.createElement("div");
    toolbarContent.classList.add('vditor-toolbar-content');
    insertMiddleElement(toolbar, toolbarContent);
    const items = toolbarContent.querySelectorAll('.vditor-toolbar__item');
    const visibleItems = Array.from(items).filter(item => getComputedStyle(item).display !== 'none');
    if (visibleItems.length > 0) {
        const itemWidth = parseInt(getComputedStyle(visibleItems[0]).width);
        const vditorToolbarWidth = itemWidth * Math.round(visibleItems.length / 2) + 'px';
        toolbarContent.style.setProperty('--vditor-toolbar-width', vditorToolbarWidth);
    }

    const btnTable = toolbar.querySelector('button[data-type=table]');
    if (!btnTable) return;
    btnTable.addEventListener("click", () => {
        dotNetCallbackRef.invokeMethodAsync("OpenAddTableDialog");
    })
}

function insertMiddleElement(parent, destination) {
    while (parent.firstChild) {
        destination.appendChild(parent.firstChild);
    }
    parent.appendChild(destination);
}
