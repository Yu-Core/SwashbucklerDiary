(function () {
    const route = '/' + (window.location.origin + window.location.pathname).replace(document.baseURI, '');
    const firstSetLanguage = localStorage.getItem('FirstSetLanguage');
    const firstAgree = localStorage.getItem('FirstAgree');
    if (!(firstSetLanguage === 'true' && firstAgree === 'true')) {
        if (route != '/welcome') {
            window.location.replace('welcome');
        }

        return;
    }

    if (route == '/') {
        const quickRecord = localStorage.getItem('QuickRecord');
        if (quickRecord === 'true') {
            window.location.replace('write');
            return;
        }
    }

    if (route != '/appLock') {
        const appLockNumberPassword = localStorage.getItem('AppLockNumberPassword');
        const appLockPatternPassword = localStorage.getItem('AppLockPatternPassword');
        const appLockBiometric = localStorage.getItem('AppLockBiometric');
        if (appLockNumberPassword
            || appLockPatternPassword
            || appLockBiometric === 'true') {
            const returnUrl = window.location.href.replace(document.baseURI, '');
            window.location.replace(`appLock?returnUrl=${encodeURIComponent(returnUrl)}`);
        }
    }
}());
