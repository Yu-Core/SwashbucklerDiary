export function recordScrollInfo(selector) {
    const el = document.querySelector(selector);
    if (el) {
        scrollListener(el, () => {
            if (el.disallowRecordScrollInfo) {
                return;
            }

            el.previousScrollInfo = {
                scrollHeight: el.scrollHeight,
                scrollTop: el.scrollTop
            };
        });
    }
}

export function stopRecordScrollInfo(selector) {
    const el = document.querySelector(selector);
    if (el) {
        el.disallowRecordScrollInfo = true;
    }

}

export function restoreScrollPosition(selector) {
    const el = document.querySelector(selector);
    if (el && el.previousScrollInfo) {
        anchorScroll(el, el.previousScrollInfo);
    }

    el.disallowRecordScrollInfo = false;
}

const scrollListener = (el, doSomething) => {
    let ticking = false;

    el.addEventListener("scroll", (event) => {
        if (!ticking) {
            window.requestAnimationFrame(() => {
                doSomething();
                ticking = false;
            });

            ticking = true;
        }
    });
}

const anchorScroll = (el, scrollInfo) => {
    const scrollTop = scrollInfo.scrollTop + el.scrollHeight - scrollInfo.scrollHeight;
    el.scrollTo({
        top: scrollTop,
        left: 0,
        behavior: "auto",
    });
};
