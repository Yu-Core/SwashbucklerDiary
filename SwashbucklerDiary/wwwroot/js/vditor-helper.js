/**
 * 点击Vditor工具栏，输入框不失去焦点
 * @returns
 */
export function preventInputLoseFocus() {
    var toolbar = document.getElementsByClassName("vditor-toolbar")[0];
    if (toolbar == null) {
        console.log("Vditor toolbar does not exist");
        return;
    }

    toolbar.onmousedown = (e) => {
        e.preventDefault();
    }
}
