export const init = (dotnetObj, onStop, onResume) => {
    document.addEventListener('visibilitychange', () => {
        if (document.visibilityState !== 'visible') {
            dotnetObj.invokeMethodAsync(onStop);
        } else {
            dotnetObj.invokeMethodAsync(onResume);
        }
    });
}

export const quit = () => {
    window.location.replace('about:blank');
    window.history.replaceState({}, document.title, window.location.pathname);
}
