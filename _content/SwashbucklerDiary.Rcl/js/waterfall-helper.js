export function recordScrollInfo(selector) {
    const element = document.querySelector(selector);
    if (element) {
        scrollListener(element, () => {
            if (element.disallowRecordScrollInfo) {
                return;
            }

            element.previousScrollInfo = {
                scrollHeight: element.scrollHeight,
                scrollTop: element.scrollTop
            };
        });
    }
}

export function stopRecordScrollInfo(selector) {
    const element = document.querySelector(selector);
    if (element) {
        element.disallowRecordScrollInfo = true;
    }

}

export function restoreScrollPosition(selector) {
    const element = document.querySelector(selector);
    if (element && element.previousScrollInfo) {
        anchorScroll(element, element.previousScrollInfo);
    }

    element.disallowRecordScrollInfo = false;
}

const scrollListener = (element, doSomething) => {
    let ticking = false;

    element.addEventListener("scroll", (event) => {
        if (!ticking) {
            window.requestAnimationFrame(() => {
                doSomething();
                ticking = false;
            });

            ticking = true;
        }
    });
}

const anchorScroll = (ele, scrollInfo) => {
    const scrollTop = scrollInfo.scrollTop + ele.scrollHeight - scrollInfo.scrollHeight;
    ele.scrollTo({
        top: scrollTop,
        left: 0,
        behavior: "auto",
    });
};
