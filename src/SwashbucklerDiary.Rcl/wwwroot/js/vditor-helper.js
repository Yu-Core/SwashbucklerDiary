export function after() {
    handleToolbar();
}

export function focus(domRef) {
    domRef.Vditor.focus();
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
