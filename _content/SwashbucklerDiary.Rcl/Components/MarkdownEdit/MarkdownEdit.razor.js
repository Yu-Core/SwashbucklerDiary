import { fixMobileOutlientClick } from '../../js/markdown/fixMarkdownOutline.js'

function after(dotNetCallbackRef, element, outlineElement, copyCutPatch) {
    handleToolbar(dotNetCallbackRef, element);
    fixMobileOutlientClick(dotNetCallbackRef, outlineElement, element);
    if (copyCutPatch) {
        fixCopyCut(dotNetCallbackRef, element);
    }
}

function focus(domRef) {
    const vditor = domRef.Vditor.vditor;
    const range = vditor[vditor.currentMode].range
    if (range) {
        const selection = window.getSelection();
        selection.removeAllRanges();
        selection.addRange(range);
    }

    domRef.Vditor.focus();
}

function focusToEnd(domRef) {
    domRef.Vditor.focus();
    const focusElement = document.activeElement;
    let range = document.createRange();
    range.selectNodeContents(focusElement);
    range.collapse(false);
    let sel = window.getSelection();
    sel.removeAllRanges();
    sel.addRange(range);
}

function upload(element, inputFileElement) {
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

function setMoblieOutline(element, outlineElement) {
    if (!element || !outlineElement) {
        return;
    }

    var outline = element.querySelector(".vditor-outline__content");
    if (!outline) {
        console.log("Vditor outline does not exist");
        return;
    }

    outlineElement.innerHTML = outline.innerHTML;
}

function fixCopyCut(dotNetCallbackRef, element) {
    const items = element.querySelectorAll('.vditor-reset');
    const copy = (event) => {
        event.preventDefault();
        
        const textData = event.clipboardData.getData('text/plain');
        if (textData) {
            setTimeout(() => {
                dotNetCallbackRef.invokeMethodAsync("SetClipboard", textData);
            }, 200);
        }
    };
    items.forEach(item => {
        item.addEventListener('copy', copy);
        item.addEventListener('cut', copy);
    });
}

export { after, focus, focusToEnd, upload, setMoblieOutline }
