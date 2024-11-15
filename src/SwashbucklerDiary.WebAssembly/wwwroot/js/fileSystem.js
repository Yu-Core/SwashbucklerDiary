window.MEMFileSystem = {
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
            FS.mount(FS.filesystems.IDBFS, {}, '/appdata');
            FS.mount(FS.filesystems.IDBFS, {}, '/cache');
            // Synchronize IDBFS to MEMFS
            FS.syncfs(true, () => {
                res();
                // Synchronize MEMFS content to IDBFS periodically
                setInterval(() => {
                    if (this.synchronizing) return;

                    this.synchronizing = true;
                    FS.syncfs((err) => {
                        this.synchronizing = false;
                    });
                }, 1000);
            });
        });
    },
    syncfs: function () {
        return new Promise((res) => {
            if (this.synchronizing) return res();
            this.syncInProgress = true;
            const FS = Blazor.runtime.Module.FS;
            FS.syncfs((err) => {
                this.syncInProgress = false;
                res();
            });
        });
    },
    synchronizing: false
};
