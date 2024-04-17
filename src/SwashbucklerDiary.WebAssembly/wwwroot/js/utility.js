window.operatingSystem = () => {
    var userAgent = navigator.userAgent;

    // Windows

    if (userAgent.indexOf('Windows') != -1) {
        return "Windows";
    }

    // Android
    if (userAgent.indexOf("Android") != -1) {
        return "Android";
    }

    // iOS
    if (userAgent.indexOf("iPhone") != -1) {
        return "iOS";
    }

    // macOS
    if (userAgent.indexOf("Mac") != -1) {
        return "macOS";
    }

    // 没有匹配到主流操作系统
    return "unknown";
};

function setThemeColor(color) {
    if (document.querySelector('meta[name="theme-color"]')) {
        document.querySelector('meta[name="theme-color"]').setAttribute("content", color);
    } else {
        const meta = document.createElement("meta");
        meta.name = "theme-color";
        meta.content = color;
        document.getElementsByTagName('head')[0].appendChild(meta);
    }
    
}
