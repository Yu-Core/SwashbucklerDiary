// Caution! Be sure you understand the caveats before publishing an application with
// offline support. See https://aka.ms/blazor-offline-considerations

self.importScripts('./service-worker-assets.js');
self.addEventListener('install', event => event.waitUntil(onInstall(event)));
self.addEventListener('activate', event => event.waitUntil(onActivate(event)));
self.addEventListener('fetch', event => event.respondWith(onFetch(event)));

const cacheNamePrefix = 'offline-cache-';
const cacheName = `${cacheNamePrefix}${self.assetsManifest.version}`;
const offlineAssetsInclude = [ /\.dll$/, /\.pdb$/, /\.wasm/, /\.html/, /\.js$/, /\.json$/, /\.css$/, /\.woff$/, /\.png$/, /\.jpe?g$/, /\.gif$/, /\.ico$/, /\.blat$/, /\.dat$/ ];
const offlineAssetsExclude = [ /^service-worker\.js$/ ];

// Replace with your base path if you are hosting on a subfolder. Ensure there is a trailing '/'.
const base = "/";
const baseUrl = new URL(base, self.origin);
const manifestUrlList = self.assetsManifest.assets.map(asset => new URL(asset.url, baseUrl).href);

async function onInstall(event) {
    console.info('Service worker: Install');

    // Fetch and cache all matching items from the assets manifest
    const assetsRequests = self.assetsManifest.assets
        .filter(asset => offlineAssetsInclude.some(pattern => pattern.test(asset.url)))
        .filter(asset => !offlineAssetsExclude.some(pattern => pattern.test(asset.url)))
        .map(asset => new Request(asset.url, { integrity: asset.hash, cache: 'no-cache' }));
    await caches.open(cacheName).then(cache => cache.addAll(assetsRequests));
}

async function onActivate(event) {
    console.info('Service worker: Activate');

    // Delete unused caches
    const cacheKeys = await caches.keys();
    await Promise.all(cacheKeys
        .filter(key => key.startsWith(cacheNamePrefix) && key !== cacheName)
        .map(key => caches.delete(key)));
}

async function onFetch(event) {
    const requestUrl = event.request.url;
    const baseUrl = self.location.origin;
    if (event.request.method == "GET" && requestUrl.startsWith(`${baseUrl}${dbName}/`)) {
        const url = decodeURI(requestUrl);
        const filePath = url.replace(`${baseUrl}`, '');
        event.respondWith(
            handleIndexedDBFileRequest(event.request, filePath)
        );
    }

    let cachedResponse = null;
    if (event.request.method === 'GET') {
        // For all navigation requests, try to serve index.html from cache,
        // unless that request is for an offline resource.
        // If you need some URLs to be server-rendered, edit the following check to exclude those URLs
        const shouldServeIndexHtml = event.request.mode === 'navigate'
            && !manifestUrlList.some(url => url === event.request.url);

        const request = shouldServeIndexHtml ? 'index.html' : event.request;
        const cache = await caches.open(cacheName);
        cachedResponse = await cache.match(request);
    }

    return cachedResponse || fetch(event.request);
}

const dbName = "/appdata";
const storeName = "FILE_DATA";

function getFileFromIndexedDB(key) {
    return new Promise((resolve, reject) => {
        // 打开 IndexedDB，获取文件
        const request = indexedDB.open(dbName, 21);
        request.onerror = () => {
            reject('Database failed to open');
        };
        request.onsuccess = () => {
            const db = request.result;
            var transaction = db.transaction(storeName, 'readonly');
            var objectStore = transaction.objectStore(storeName);
            const fileRequest = objectStore.get(key);

            fileRequest.onsuccess = () => {
                const fileName = key.split('/').pop();
                const contents = fileRequest.result.contents;
                var file = new File([contents], fileName);
                resolve(file);
            };

            fileRequest.onerror = () => {
                reject('File retrieval failed');
            };
        };
    });
}

async function handleIndexedDBFileRequest(request, filePath) {
    // 检查请求是否包含 Range 头
    const rangeHeader = request.headers.get('Range');
    if (rangeHeader) {
        try {
            // 尝试从 IndexedDB 获取文件
            const file = await getFileFromIndexedDB(filePath);
            const size = file.size;
            const rangeMatch = rangeHeader.match(/bytes=(\d+)-(\d+)?/);
            const start = Number(rangeMatch[1]);
            const end = rangeMatch[2] ? Number(rangeMatch[2]) : size;
            //const contentLength = end - start + 1;
            const headers = {
                "Content-Range": `bytes ${start}-${end - 1}/${size}`,
                //"Accept-Ranges": "bytes",
                //"Content-Length": contentLength,
                //"Content-Type": file.type,
            };
            const response = new Response(file.slice(start, end), {
                status: 206,
                statusText: 'Partial Content',
                headers: headers
            });
            return response;
        } catch (error) {
            // 在出错时返回一个 404 响应
            return new Response('', { status: 404 });
        }
    } else {
        // 如果没有 Range 头，正常处理请求
        // ...
        try {
            const file = await getFileFromIndexedDB(filePath);
            const response = new Response(file, {
                headers: { 'Content-Type': file.type }
            });
            return response;
        } catch (e) {
            // 在出错时返回一个 404 响应
            return new Response('', { status: 404 });
        }
    }
}
