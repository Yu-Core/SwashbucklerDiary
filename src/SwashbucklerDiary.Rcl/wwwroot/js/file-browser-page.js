export function recordScrollInfo(selectors) {
    const getScrollDetails = (ele) => {
        return {
            scrollHeight: ele.scrollHeight,
            scrollTop: ele.scrollTop
        };
    };

    const result = selectors.map(s => {
        const element = document.querySelector(s);
        return element ? getScrollDetails(element) : null
    });

    return JSON.stringify(result);
}

export function restoreScrollPosition(selectors, strScrollInfos) {
    const anchorScroll = (ele, scrollInfo) => {
        const scrollTop = scrollInfo.scrollTop + ele.scrollHeight - scrollInfo.scrollHeight;
        ele.scrollTo({
            top: scrollTop,
            left: 0,
            behavior: "auto",
        });
    };

    const scrollInfos = JSON.parse(strScrollInfos);
    for (var i = 0; i < selectors.length; i++) {
        const element = document.querySelector(selectors[i]);
        if (element && scrollInfos[i]) {
            anchorScroll(element, scrollInfos[i]);
        }
    }
}
