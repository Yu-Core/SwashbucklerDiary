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
            Module.FS.mkdir('/appdata');
            Module.FS.mkdir('/cache');
            // Mount IDBFS to MEMFS
            Module.FS.mount(Module.FS.filesystems.IDBFS, {}, '/appdata');
            Module.FS.mount(Module.FS.filesystems.IDBFS, {}, '/cache');
            // Synchronize IDBFS to MEMFS
            Module.FS.syncfs(true, () => {
                res();
                // Synchronize MEMFS content to IDBFS periodically
                setInterval(() => {
                    if (this.synchronizing) return;

                    this.synchronizing = true;
                    Module.FS.syncfs((err) => {
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
            Module.FS.syncfs((err) => {
                this.syncInProgress = false;
                res();
            });
        });
    },
    synchronizing: false
};
