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
        // Open IndexedDB and retrieve the file

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
    try {
        // Attempting to retrieve files from IndexedDB
        const file = await getFileFromIndexedDB(filePath);
        const length = file.size;
        let rangeStart = 0;
        let rangeEnd = length - 1;
        let statusCode = 200;
        let reasonPhrase = "OK";
        let headers = {};

        // Check if the request contains a Range header
        const rangeHeader = request.headers.get('Range');
        if (rangeHeader) {
            statusCode = 206;
            reasonPhrase = "Partial Content";

            const rangeMatch = rangeHeader.match(/bytes=(\d+)-(\d+)?/);
            rangeStart = Number(rangeMatch[1]);
            rangeEnd = rangeMatch[2] ? Number(rangeMatch[2]) : rangeEnd;
            headers["Content-Range"] = `bytes ${rangeStart}-${rangeEnd}/${length}`;
            headers["Accept-Ranges"] = "bytes";
        }

        headers["Content-Length"] = rangeEnd - rangeStart + 1;
        // The second parameter of file.slice is the index of the first byte that will not be copied into the new Blob. So need + 1
        const response = new Response(file.slice(rangeStart, rangeEnd + 1), {
            status: statusCode,
            statusText: reasonPhrase,
            headers: headers
        });
        return response;
    } catch (e) {
        // Return a 404 response when an error occurs
        return new Response(e.message, { status: 404 });
    }
}
