function preventDefaultOnmousedown(el) {
    if (el == null) {
        return;
    }

    el.onmousedown = (e) => {
        e.preventDefault();
    }
}

function setLanguage(lang) {
    document.documentElement.lang = lang;
}

function elementScrollTop(selector) {
    let el = document.querySelector(selector);
    if (el) {
        return el.scrollTop;
    } else {
        return 0;
    }
}

function addToHead(html) {
    const element = document.createRange().createContextualFragment(html);
    document.getElementsByTagName('head')[0].appendChild(element);
}
