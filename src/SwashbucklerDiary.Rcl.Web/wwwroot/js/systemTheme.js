export const init = (dotnetObj) => {
    const media = matchMedia('(prefers-color-scheme: dark)');
    if (media && dotnetObj) {
        media.addEventListener('change', (e) => {
            dotnetObj.invokeMethodAsync('OnSystemThemeChanged', e.matches);
        });
    }

    return media.matches;
}
