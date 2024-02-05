// In development, always fetch from the network and do not enable offline support.
// This is because caching would make development more difficult (changes would not
// be reflected on the first load after each change).
importScripts('./sw-IndexedDB.js');

const indexedDBUrlPrefix = getIndexedDBUrlPrefix();

self.addEventListener('fetch', event => {
    const requestUrl = event.request.url;
    if (event.request.method == "GET" && requestUrl.startsWith(indexedDBUrlPrefix)) {
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

function getIndexedDBUrlPrefix() {
    const pathname = self.location.pathname;
    const directory = pathname.substring(0, pathname.lastIndexOf('/') + 1);
    const newPathname = (directory + dbName).replace('//', '/');
    return self.location.origin + newPathname + '/';
}

function getIndexedDBFilePath(urlString) {
    const url = new URL(urlString);
    const urlPath = url.origin + url.pathname;
    return dbName + '/' + urlPath.replace(indexedDBUrlPrefix, '');
}
