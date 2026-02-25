export const init = (dotnetObj) => {
    document.addEventListener('visibilitychange', () => {
        if (document.visibilityState !== 'visible') {
            dotnetObj.invokeMethodAsync("OnStopped");
        } else {
            dotnetObj.invokeMethodAsync("OnResumed");
        }
    });
}

export const quit = () => {
    window.location.replace('about:blank');
    window.history.replaceState({}, document.title, window.location.pathname);
}
