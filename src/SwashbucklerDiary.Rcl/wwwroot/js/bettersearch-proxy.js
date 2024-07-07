export function init(selector) {
    var betterSearch = new BetterSearchProxy({
        domContainer: selector
    });
    return betterSearch;
}

class BetterSearchProxy extends BetterSearch {
    constructor(opt) {
        super(opt);
    }

    updateProperties() {
        const roperties = {
            "count": this.count,
            "searchIndex": this.searchIndex
        };
        return roperties;
    }

    initStyle() {
        if (document.querySelector('style#betterSearch')) return
        const css = '.__better-search-current-select{background: #FF9632 !important;}'
        let style = document.createElement('style')
        style.setAttribute('id', 'betterSearch')
        style.appendChild(document.createTextNode(css))
        document.head.appendChild(style)
    }

    goNode(index) {
        let domElements = this.domList[index];
        if (!domElements) return;
        let lastElement = domElements[domElements.length - 1];
        if (lastElement) {
            if (lastElement.scrollIntoViewIfNeeded) {
                lastElement.scrollIntoViewIfNeeded();
            }
            else {
                lastElement.scrollIntoView();
            }
        }
        this.addSelectClass(domElements);
    }

    removeSelectClass(dom) {
        if (!dom) return;
        super.removeSelectClass(dom);
    }
}
