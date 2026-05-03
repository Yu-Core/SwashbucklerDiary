export function init(selector, dotNetObj) {
    var betterSearch = new BetterSearchProxy({
        domContainer: selector,
        onChange: (prop, value, oldValue) => {
            if (prop === 'searchIndex') {
                dotNetObj.invokeMethodAsync("UpdateSearchIndex", value);
            } else if (prop === 'count') {
                dotNetObj.invokeMethodAsync("UpdateCount", value);
            }
        }
    });

    dotNetObj.invokeMethodAsync("UpdateSearchIndex", betterSearch.searchIndex);
    dotNetObj.invokeMethodAsync("UpdateCount", betterSearch.count);
    return betterSearch;
}

class BetterSearchProxy extends BetterSearch {
    constructor(opt) {
        super(opt);

        const callback = opt.onChange; // 获取回调函数

        // 使用 Proxy 监听 count 和 index 的变化
        return new Proxy(this, {
            set(target, prop, value) {
                const oldValue = target[prop];
                target[prop] = value;
                if ((prop === 'count' || prop === 'searchIndex') && value !== oldValue) {
                    callback && callback(prop, value, oldValue);
                }
                return true;
            }
        });
    }
}
