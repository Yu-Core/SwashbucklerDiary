(function () {
    const relativePath = (window.location.origin + window.location.pathname).replace(document.baseURI, '');
    if (relativePath.toLowerCase() == 'welcome') {
        return;
    }
    else {
        const firstSetLanguage = localStorage.getItem('FirstSetLanguage');
        const firstAgree = localStorage.getItem('FirstAgree');

        if (!(firstSetLanguage === 'true' && firstAgree === 'true')) {
            window.location.replace('welcome');
            return;
        }
    }

    if (relativePath.toLowerCase() == '') {
        const quickRecord = localStorage.getItem('QuickRecord');
        if (quickRecord === 'true') {
            window.location.replace('write');
            return;
        }
    }

    if (relativePath.toLowerCase() != 'applock') {
        const appLockNumberPassword = localStorage.getItem('AppLockNumberPassword');
        const appLockPatternPassword = localStorage.getItem('AppLockPatternPassword');
        const appLockBiometric = localStorage.getItem('AppLockBiometric');
        if (appLockNumberPassword
            || appLockPatternPassword
            || appLockBiometric === 'true') {
            const activationArguments = {
                Kind: 'Scheme', // 0=Launch, 1=Scheme, 2=Share
                Data: window.location.href
            }
            sessionStorage.setItem('ActivationArguments', JSON.stringify(activationArguments));

            window.location.replace('appLock');
        }
    }
}());
