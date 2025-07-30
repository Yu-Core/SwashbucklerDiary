export function init(selector) {
    var betterSearch = new BetterSearchProxy({
        domContainer: selector
    });
    return betterSearch;
}

const formatTextNode = function (el) {
    const parentNode = el.parentNode
    const afterNode = parentNode.childNodes[Array.apply(null, parentNode.childNodes).indexOf(el) - 1]
    const targetNode = parentNode.removeChild(el)
    let span = document.createElement('span')
    span.appendChild(targetNode)
    if (afterNode) {
        parentNode.appendChild(span)
        afterNode.after(span)
    } else {
        parentNode.prepend(span)
    }
    return targetNode
}
const regExpescape = function (str) {
    return str.replace(/[-\/\\^$*+?.()|[\]{}]/g, '\\$&')
}
const escapeHTML = (str) => {
    const div = document.createElement('div');
    div.innerText = str;
    return div.innerHTML;
}

let markId = 0;

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

    //重置搜索数据
    clear() {
        super.clear();
        markId = 0;
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

    //DOM染色
    markDom(el, value, isSame) {
        if (!el.parentNode || !value) return
        //如果父级下有多个子节点的话，说明该文本不是单独标签包含，需要处理下
        if (el.parentNode.childNodes.length > 1) {
            el = formatTextNode(el)
        }
        const reg = new RegExp(regExpescape(value), 'ig')
        const highlightList = el.data.match(reg) // 得出文本节点匹配到的字符串数组
        if (!highlightList) return
        const splitTextList = el.data.split(reg) // 分割多次匹配
        // 遍历分割的匹配数组，将匹配出的字符串加上.highlight并依次插入DOM, 同时给为匹配的template用于后续恢复
        el.parentNode.innerHTML = splitTextList.reduce(
            (html, splitText, i) => {
                const text =
                    html +
                    escapeHTML(splitText) +
                    (i < splitTextList.length - 1
                        ? `<mark class="search-highlight" mark-id="${markId}">${escapeHTML(highlightList[i])}</mark>`
                        : `<template search-highlight>${escapeHTML(el.data)}</template>`)
                if (isSame) markId++
                return text
            },
            ''
        )
    }
}
