function preventDefaultOnmousedown(el) {
    if (el == null) {
        return;
    }

    el.onmousedown = (e) => {
        e.preventDefault();
    }
}

function disableConsoleLog() {
    window.console = {
        log: function () { }
    };
}