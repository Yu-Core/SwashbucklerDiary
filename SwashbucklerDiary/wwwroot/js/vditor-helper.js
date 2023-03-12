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