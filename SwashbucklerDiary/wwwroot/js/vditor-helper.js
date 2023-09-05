/**
 * 点击Vditor工具栏，输入框不失去焦点
 * @returns
 */
export function preventInputLoseFocus() {
    var toolbar = document.getElementsByClassName("vditor-toolbar")[0];
    if (toolbar == null || toolbar == undefined || toolbar == "") {
        console.log("Vditor toolbar does not exist");
        return;
    }
    toolbar.onmousedown = (e) => {
        e.preventDefault();
    }
}

export function moveCursorForward(length) {
    const selection = window.getSelection();
    const range = selection.getRangeAt(0);
    const startContainer = range.startContainer;
    const startOffset = range.startOffset + length;
    const endContainer = range.endContainer;
    const endOffset = range.endOffset + length;

    if (startContainer.nodeType === Node.TEXT_NODE) {
        if (startOffset <= startContainer.length) {
            range.setStart(startContainer, startOffset);
        } else {
            const nextNode = startContainer.nextSibling;
            if (nextNode) {
                range.setStart(nextNode, startOffset - startContainer.length);
            }
        }
    }

    if (endContainer.nodeType === Node.TEXT_NODE) {
        if (endOffset <= endContainer.length) {
            range.setEnd(endContainer, endOffset);
        } else {
            const nextNode = endContainer.nextSibling;
            if (nextNode) {
                range.setEnd(nextNode, endOffset - endContainer.length);
            }
        }
    }

    selection.removeAllRanges();
    selection.addRange(range);
}



