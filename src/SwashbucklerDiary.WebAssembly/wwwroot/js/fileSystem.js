export function initFileSystem() {
    return new Promise((res, rej) => {
        //Create appdata folder
        Module.FS.mkdir('/appdata');
        //Create cache folder
        Module.FS.mkdir('/cache');
        //Mount IDBFS (indexDB) to MEMFS (memory)
        Module.FS.mount(Module.FS.filesystems.IDBFS, {}, '/appdata');
        //Synchronize the content of IDBFS to MEMFS
        Module.FS.syncfs(true, function (err) {
            // handle callback
            res();

            //Synchronize MEMFS content to IDBFS every 1 second
            setInterval(() => {
                Module.FS.syncfs(function (err) {
                    // handle callback
                });
            }, 1000);
        });
    });
}

//Synchronize the content of MEMFS to IDBFS for immediate use, such as intercepting requests from service worker and searching in indexDB
export function syncfs() {
    return new Promise((res, rej) => {
        Module.FS.syncfs(function (err) {
            // handle callback
            res();
        });
    });
}
