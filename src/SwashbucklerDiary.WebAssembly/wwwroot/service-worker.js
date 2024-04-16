// In development, always fetch from the network and do not enable offline support.
// This is because caching would make development more difficult (changes would not
// be reflected on the first load after each change).
importScripts('./sw-custom-path.js');

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
