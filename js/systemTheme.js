export const registerForSystemThemeChanged = (dotnetObj, callbackMethodName) => {
    const media = matchMedia('(prefers-color-scheme: dark)');
    if (media && dotnetObj) {
        media.addEventListener('change', (e) => {
            dotnetObj.invokeMethodAsync(callbackMethodName, e.matches);
        });
    }

    return media.matches;
}
