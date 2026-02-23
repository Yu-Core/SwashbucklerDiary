export const init = (dotnetObj) => {
    const media = matchMedia('(prefers-color-scheme: dark)');
    if (media && dotnetObj) {
        media.addEventListener('change', (e) => {
            dotnetObj.invokeMethodAsync('SystemThemeChange', e.matches);
        });
    }

    return media.matches;
}
