/**
 * 处理Windows拦截协议的临时办法
 * 
 */

function HandleCustomSchemeRender() {
    // 获取页面中所有的元素
    var elements = document.querySelectorAll("img, video, audio, source");

    // 遍历所有元素
    for (var i = 0; i < elements.length; i++) {
        var element = elements[i];

        var srcValue = element.getAttribute("src");

        // 检查src属性值是否以'appdata:///'开头
        if (srcValue.startsWith("appdata:///")) {
            // 替换src属性值中的'appdata:///'为'appdata/'
            var newSrcValue = srcValue.replace("appdata:///", "https://appdata/");

            // 更新元素的src属性值
            element.setAttribute("src", newSrcValue);
        }
    }
}
