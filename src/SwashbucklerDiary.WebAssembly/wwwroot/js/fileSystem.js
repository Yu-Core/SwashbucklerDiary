export function synchronizeFileWithIDBFS() {
    return new Promise((res, rej) => {
        Module.FS.mkdir('/appdata');
        Module.FS.mkdir('/cache');
        Module.FS.mount(Module.FS.filesystems.IDBFS, {}, '/appdata');

        Module.FS.syncfs(true, function (err) {
            // handle callback
            res();
            
            setInterval(() => {
                Module.FS.syncfs(function (err) {
                    // handle callback
                });
            }, 1000);
        });
    });
}

export function syncfs() {
    return new Promise((res, rej) => {
        Module.FS.syncfs(function (err) {
            // handle callback
            res();
        });
    });
}
