/**
 * 处理Windows拦截协议的临时办法
 * 
 */

function ImageRender() {
    var imgs = document.querySelectorAll("img");
    for (var i = 0; i < imgs.length; i++) {
        var src = imgs[i].getAttribute('src');
        if (src && src.includes('appdata:///')) {
            src = src.replace("appdata:///", "appdata/");
            imgs[i].setAttribute("src", src); 
        }
    }
}
