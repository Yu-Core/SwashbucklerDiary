export const init = (dotnetObj) => {
    document.addEventListener('visibilitychange', () => {
        if (document.visibilityState !== 'visible') {
            dotnetObj.invokeMethodAsync("Stop");
        } else {
            dotnetObj.invokeMethodAsync("Resume");
        }
    });

    const activationArgumentsString = sessionStorage.getItem('ActivationArguments');
    if (activationArgumentsString) {
        dotnetObj.invokeMethodAsync("SetActivationArgumentsFromJson", activationArgumentsString);
    }
}

export const quit = () => {
    window.location.replace('about:blank');
    window.history.replaceState({}, document.title, window.location.pathname);
}
