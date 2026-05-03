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

(function () {
    if (window.visualViewport) {
        const updateObstruction = () => {
            document.documentElement.style.setProperty('--safe-area-height', `${window.visualViewport.height}px`);

            const keyboardHeight = window.innerHeight - window.visualViewport.height;
            if (keyboardHeight > 0) {
                document.documentElement.style.setProperty('--soft-keyboard-height', `${keyboardHeight}px`);
            } else {
                document.documentElement.style.setProperty('--soft-keyboard-height', '0px');
            }
        }

        updateObstruction();

        window.visualViewport.addEventListener('resize', updateObstruction);
        window.visualViewport.addEventListener('scroll', updateObstruction);
    }
}());

// Setting document.title doesn't change the tab's text after pressing back in the browser
// https://stackoverflow.com/questions/72982365/
(function () {
    window.addEventListener('popstate', function (event) {
        const title = document.title;
        document.title = '';
        document.title = title;
    });
}());
