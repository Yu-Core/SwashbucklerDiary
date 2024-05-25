export function recordScrollInfo(selectors) {
    selectors.forEach(s => {
        const element = document.querySelector(s);
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
    });
}

export function stopRecordScrollInfo(selectors) {
    selectors.forEach(s => {
        const element = document.querySelector(s);
        if (element) {
            element.disallowRecordScrollInfo = true;
        }
    });
}

function scrollListener(element, doSomething) {
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

export function restoreScrollPosition(selectors) {
    const anchorScroll = (ele, scrollInfo) => {
        const scrollTop = scrollInfo.scrollTop + ele.scrollHeight - scrollInfo.scrollHeight;
        ele.scrollTo({
            top: scrollTop,
            left: 0,
            behavior: "auto",
        });

        ele.disallowRecordScrollInfo = false;
    };

    selectors.forEach(s => {
        const element = document.querySelector(s);
        if (element && element.previousScrollInfo) {
            anchorScroll(element, element.previousScrollInfo);
        }
    });
}
