export function after() {
    handleToolbar();
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

//点击Vditor工具栏，输入框不失去焦点
function handleToolbar() {
    var toolbar = document.getElementsByClassName("vditor-toolbar")[0];
    if (toolbar == null) {
        console.log("Vditor toolbar does not exist");
        return;
    }

    //prevent Input Lose Focus
    toolbar.onmousedown = (e) => {
        e.preventDefault();
    };
}
