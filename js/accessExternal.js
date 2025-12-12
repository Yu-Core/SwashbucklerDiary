export function openAppStoreAppDetails(dictionary) {
    let appIds = new Map(Object.entries(dictionary));
    let os = window.operatingSystem();
    if (!appIds.has(os)) {
        return false;
    }

    let appId = appIds.get(os);
    return internalOpenAppStoreAppDetails(appId);
}

function internalOpenAppStoreAppDetails(appId) {
    let uri;
    let os = window.operatingSystem();
    if (os === 'Windows') {
        uri = 'ms-windows-store://pdp/?ProductId=' + appId;
    } else if (os === 'Android') {
        uri = 'market://details?id=' + appId;
    } else if (os === 'iOS' || os === 'macOS') {
        uri = 'itms-apps://itunes.apple.com/app/id' + appId;
    }

    const a = document.createElement("a");
    a.href = uri;
    a.style.display = "none";
    if (typeof a.protocol === "undefined") {
        return false;
    }
    document.body.appendChild(a);
    a.click();
    a.remove();
    return true;
}

