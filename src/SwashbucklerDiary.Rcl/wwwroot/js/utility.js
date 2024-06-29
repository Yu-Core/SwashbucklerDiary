function preventDefaultOnmousedown(el) {
    if (el == null) {
        return;
    }

    el.onmousedown = (e) => {
        e.preventDefault();
    }
}

function addToHead(html) {
    const element = document.createRange().createContextualFragment(html);
    document.getElementsByTagName('head')[0].appendChild(element);
}

function evaluateJavascript(fn) {
    let Fn = Function;
    return new Fn('return ' + fn)();
}
