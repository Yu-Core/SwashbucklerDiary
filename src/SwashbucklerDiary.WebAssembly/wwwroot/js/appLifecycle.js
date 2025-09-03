export const init = (dotnetObj) => {
    document.addEventListener('visibilitychange', () => {
        if (document.visibilityState !== 'visible') {
            dotnetObj.invokeMethodAsync("Stop");
        } else {
            dotnetObj.invokeMethodAsync("Resume");
        }
    });

    const args = sessionStorage.getItem('ActivationArguments');
    sessionStorage.removeItem('ActivationArguments');
    return args || null;
}

export const quit = () => {
    window.location.replace('about:blank');
    window.history.replaceState({}, document.title, window.location.pathname);
}
