const dbNames = ["/appdata", "/cache"];
const storeName = "FILE_DATA";

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

function getFileFromIndexedDB(key) {
    return new Promise((resolve, reject) => {
        // 打开 IndexedDB，获取文件

        let dbName;
        for (var i = 0; i < dbNames.length; i++) {
            if (key.startsWith(dbNames[i] + '/')) {
                dbName = dbNames[i];
                break;
            }
        }
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
