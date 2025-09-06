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
            if (this.synchronizing) return res();
            this.synchronizing = true;
            const FS = Blazor.runtime.Module.FS;
            FS.syncfs((err) => {
                this.synchronizing = false;
                if (err) console.error(err);
                res();
            });
        });
    },
    synchronizing: false
};
