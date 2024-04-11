// In development, always fetch from the network and do not enable offline support.
// This is because caching would make development more difficult (changes would not
// be reflected on the first load after each change).
importScripts('./sw-IndexedDB.js');

self.addEventListener('fetch', event => {
    const requestUrl = event.request.url;
    if (event.request.method == "GET" && intercept(requestUrl)) {
        const url = decodeURI(requestUrl);
        const filePath = getIndexedDBFilePath(url);
        event.respondWith(
            handleIndexedDBFileRequest(event.request, filePath)
        );
    }
});

self.addEventListener('message', event => {
    if (event.data && event.data.type === 'SKIP_WAITING') {
        self.skipWaiting();
    }
})

function intercept(requestUrl) {
    if (!requestUrl.startsWith(self.location.origin)) {
        return false;
    }

    const href = self.location.href;
    const directory = href.substring(0, href.lastIndexOf('/'));
    for (var i = 0; i < dbNames.length; i++) {
        const prefix = directory + dbNames[i] + '/';
        if (requestUrl.startsWith(prefix)) {
            return true;
        }
    }
    return false;
}

function getIndexedDBFilePath(urlString) {
    const url = new URL(urlString);
    const urlPath = url.origin + url.pathname; //Exclude hash and search
    const href = self.location.href;
    return urlPath.substring(href.lastIndexOf('/'), urlPath.length);
}
