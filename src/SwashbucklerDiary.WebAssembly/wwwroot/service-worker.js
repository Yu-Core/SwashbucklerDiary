// In development, always fetch from the network and do not enable offline support.
// This is because caching would make development more difficult (changes would not
// be reflected on the first load after each change).
importScripts('./sw-IndexedDB.js');

self.addEventListener('fetch', event => {
    const requestUrl = event.request.url;
    const baseUrl = self.location.origin;
    if (event.request.method == "GET" && requestUrl.startsWith(`${baseUrl}${dbName}/`)) {
        const url = decodeURI(requestUrl);
        const filePath = url.replace(`${baseUrl}`, '');
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
