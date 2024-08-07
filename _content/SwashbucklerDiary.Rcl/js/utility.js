function preventDefaultOnmousedown(el) {
    if (el == null) {
        return;
    }

    el.onmousedown = (e) => {
        e.preventDefault();
    }
}

function evaluateJavascript(fn) {
    let Fn = Function;
    return new Fn('return ' + fn)();
}
