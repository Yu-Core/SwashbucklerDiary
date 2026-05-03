window.WasmFileSystem = {
    init: async function () {
        const deleteDatabase = (dbName) => {
            return new Promise((res, rej) => {
                const request = indexedDB.deleteDatabase(dbName);
                request.onerror = () => rej();
                request.onsuccess = () => res();
            });
        };
        await deleteDatabase('/cache');

        return new Promise((res) => {
            // Create appdata and cache folders
            const FS = Blazor.runtime.Module.FS;
            FS.mkdir('/appdata');
            FS.mkdir('/cache');
            // Mount IDBFS to MEMFS
            FS.mount(FS.filesystems.IDBFS,{
                autoPersist: true
            }, '/appdata');
            FS.mount(FS.filesystems.IDBFS, {
                autoPersist: true
            }, '/cache');
            // Synchronize IDBFS to MEMFS
            FS.syncfs(true, (err) => {
                if (err) console.error(err);
                res();
            });
        });
    },
    syncfs: function () {
        return new Promise((res) => {
            const FS = Blazor.runtime.Module.FS;
            if (FS.syncFSRequests === 0) {
                FS.syncfs((err) => {
                    if (err) console.error(err);
                    res();
                });
            }
        });
    }
};
