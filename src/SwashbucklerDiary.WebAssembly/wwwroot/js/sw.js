document.addEventListener('swUpdate', function (e) {
    setTimeout(() => {
        const pwaSnackbar = document.querySelector(".m-snackbar--pwa > .m-snack__wrapper");
        if (!pwaSnackbar) return;

        if (window.getComputedStyle(document.getElementById('blazor-error-ui')).display !== 'none') {
            pwaSnackbar.style.marginBottom = "2.4rem";
        }

        pwaSnackbar.style.display = "flex";
    }, 5000)
});

window.swSkipWaiting = function () {
    navigator.serviceWorker.getRegistration().then(function (reg) {
        reg.waiting.postMessage({type: 'SKIP_WAITING'});
    });
}

window.swIgnoreUpdate = function () {
    const pwaSnackbar = document.querySelector(".m-snackbar--pwa > .m-snack__wrapper");
    pwaSnackbar.style.display = "none";
}

window.emitSwUpdate = function () {
    const event = new CustomEvent('swUpdate');
    document.dispatchEvent(event);
};

if ('serviceWorker' in navigator) {
    navigator.serviceWorker.addEventListener('controllerchange', function () {
        window.location.reload();
    });

    navigator.serviceWorker.register('service-worker.js', { updateViaCache: 'none' }).then(function (reg) {
        //安装成功，建议此处强刷新以立刻执行SW
        if (window.localStorage.getItem('install') != 'true') {
            window.localStorage.setItem('install', 'true');
            setTimeout(() => {
                window.location.search = `?time=${new Date().getTime()}`
            }, 1000);
            return;
        }

        if (reg.waiting) {
            emitSwUpdate();
            return;
        }

        reg.onupdatefound = function () {
            const installingWorker = reg.installing;
            installingWorker.onstatechange = function () {
                switch (installingWorker.state) {
                    case 'installed':
                        if (navigator.serviceWorker.controller) {
                            emitSwUpdate();
                        }
                        break;
                }
            };
        };
    }, /*catch*/ function (error) {
        console.error('Service worker registration failed:', error);
    });
}