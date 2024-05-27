export function recordScrollInfo(selectors) {
    selectors.forEach(s => {
        const element = document.querySelector(s);
        if (element) {
            scrollListener(element, () => {
                if (disallowRecordScrollInfo) {
                    return;
                }

                element.previousScrollInfo = {
                    scrollHeight: element.scrollHeight,
                    scrollTop: element.scrollTop
                };
            });
        }
    });
}

export function stopRecordScrollInfo() {
    disallowRecordScrollInfo = true;
}

export function restoreScrollPosition(selectors) {
    selectors.forEach(s => {
        const element = document.querySelector(s);
        if (element && element.previousScrollInfo) {
            anchorScroll(element, element.previousScrollInfo);
        }
    });

    disallowRecordScrollInfo = false;
}

let disallowRecordScrollInfo = false;

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
