var screenHeight = window.innerHeight;
function raiseToolBar(domRef) {
    let el = domRef.getElementsByClassName("vditor-toolbar")[0];
    el.style.bottom = screenHeight * 0.35 + "px";
}
function reduceToolBar(domRef) {
    let el = domRef.getElementsByClassName("vditor-toolbar")[0];
    el.style.bottom = "0px";
}
