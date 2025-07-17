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

function forceRefresh() {
    if ('caches' in window) {
        caches.keys().then(cacheNames => {
            cacheNames.forEach(cacheName => {
                caches.delete(cacheName);
            });
        });
    }

    window.location.reload();
}

// 获取软键盘高度，Android WebView无效，Android Chrome、Edge、Firefox有效
(function () {
    if (window.visualViewport) {
        const setSoftKeyboardHeight = () => {
            const keyboardHeight = window.innerHeight - window.visualViewport.height;
            if (keyboardHeight > 0) {
                document.documentElement.style.setProperty('--soft-keyboard-height', `${keyboardHeight}px`);
            } else {
                document.documentElement.style.setProperty('--soft-keyboard-height', '0px');
            }
        }

        setSoftKeyboardHeight();

        window.visualViewport.addEventListener('resize', setSoftKeyboardHeight);
    }
}());
