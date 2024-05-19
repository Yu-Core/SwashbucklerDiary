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

function addToHead(html) {
    const element = document.createRange().createContextualFragment(html);
    document.getElementsByTagName('head')[0].appendChild(element);
}
