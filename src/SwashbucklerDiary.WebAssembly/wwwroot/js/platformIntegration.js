export async function checkCameraPermission() {
    try {
        const permissionStatus = await navigator.permissions.query({ name: 'camera' });
        return permissionStatus.state === 'granted';
    } catch (error) {
        console.error('无法检查摄像头权限', error);
        return false;
    }
}

export function isCaptureSupported() {
    return false;
}

export async function tryCameraPermission() {
    try {
        // 使用 await 等待 Promise 解析
        const stream = await navigator.mediaDevices.getUserMedia({ video: true });
        // 摄像头权限已授予，可以关闭或处理stream
        stream.getTracks().forEach(track => track.stop());
        return true; // 返回 true 表示权限已授予
    } catch (error) {
        // 摄像头权限被拒绝或出现错误
        console.error('摄像头权限请求错误:', error);
        return false; // 返回 false 表示权限未授予或出错
    }
}

export function openUri(uri, blank) {
    return new Promise((resolve, reject) => {
        // 检测页面状态的标志
        let pageHiddenOrBlurred = false;

        // 处理页面状态变化的事件
        function handlePageChange() {
            pageHiddenOrBlurred = true;
            cleanup();
            // 页面状态变化，认为链接被打开了
            resolve(true);
        }

        // 移除事件监听并清理
        function cleanup() {
            window.removeEventListener('blur', handlePageChange);
            window.removeEventListener('pagehide', handlePageChange);
            document.removeEventListener('visibilitychange', handleVisibilityChange);
        }

        // 处理页面可见性变化的事件
        function handleVisibilityChange() {
            if (document.hidden) {
                handlePageChange();
            }
        }

        // 监听页面状态变化的事件
        window.addEventListener('blur', handlePageChange);
        window.addEventListener('pagehide', handlePageChange);
        document.addEventListener('visibilitychange', handleVisibilityChange);

        // 创建并点击<a>元素以尝试打开链接
        const a = document.createElement("a");
        a.href = uri;
        if (blank) {
            a.target = "_blank";
        }
        a.style.display = "none";
        document.body.appendChild(a);
        a.click();
        a.remove();

        // 设置超时检查
        setTimeout(() => {
            cleanup();
            if (!pageHiddenOrBlurred) {
                // 如果指定时间内页面状态没有变化，认为链接没有被打开
                reject(false);
            }
        }, 1000);
    });
};

export function capturePhotoAsync() {
    return '';
}

export function setClipboard(text) {
    if (navigator.clipboard) {
        try {
            navigator.clipboard.writeText(text)
        } catch (e) {
            execCommand()
        }
    } else {
        execCommand()
    }

    function execCommand() {
        let input = document.createElement('input');
        input.type = 'text';
        input.readOnly = true;
        input.value = text;

        document.body.appendChild(input);
        input.focus();
        input.select();
        if (document.execCommand('copy')) {
            document.execCommand('copy')
        }
        input.blur();
        input.remove();
    }
}

export async function shareTextAsync(title, text) {
    if (navigator.share) {
        try {
            await navigator.share({
                title: title,
                text: text
            })
            console.log('Thanks for sharing!');
        } catch (e) {
            console.error(e);
        }
    }
}

export async function shareFileAsync(title, path) {
    if (navigator.share) {
        try {
            if (Module.FS.analyzePath(path).exists) {
                const fileName = path.split('/').pop();
                const data = Module.FS.readFile(path);
                const file = new File([data], fileName, { type: 'image/png' });
                await navigator.share({
                    title: title,
                    files: [file]
                })
                console.log('Thanks for sharing!');
            }

        } catch (e) {
            console.error(e);
        }
    }
}

export function saveFileAsync(fileName, filePath) {
    const path = `/${filePath}`;
    const fileData = Module.FS.readFile(path);
    var blob = new Blob([fileData], { type: 'application/octet-stream' });
    const url = URL.createObjectURL(blob);

    const a = document.createElement('a');
    a.download = fileName;
    a.href = url;
    a.style.display = 'none';

    document.body.appendChild(a);
    a.click();
    a.remove();
    URL.revokeObjectURL(url);
}

export function pickFileAsync(accept, suffix) {
    return new Promise((resolve) => {
        const input = document.createElement('input');
        input.type = 'file';
        input.accept = accept;
        input.style.display = 'none';

        const handleSelectedFile = () => {
            const file = input.files[0];
            if (file) {
                if (suffix && !file.name.endsWith(suffix)) {
                    resolve("");
                }
                else {
                    // wasm模式，C#计算md5比较难，所以在js中计算
                    calculateMD5.readFile(file)
                        .then((md5) => {
                            var fileName = `${md5}${file.name.substring(file.name.lastIndexOf("."))}`;
                            var reader = new FileReader();
                            reader.onload = function (event) {
                                // 获取文件内容
                                var contents = event.target.result;

                                // 写入 Emscripten 文件系统
                                var filePath = `cache/${fileName}`;
                                Module.FS.writeFile(filePath, new Uint8Array(contents), { encoding: 'binary' });
                                resolve(filePath);
                            };
                            reader.readAsArrayBuffer(file);
                        })
                        .catch(() => {
                            resolve("");
                        });
                }
            } else {
                resolve("");
            }
            input.remove();
        }

        const afterChooseFile = () => {
            // 移除事件监听器，确保代码只执行一次
            window.removeEventListener("focus", afterChooseFile);
            document.removeEventListener("touchstart", afterChooseFile);
            document.removeEventListener("visibilitychange", handleVisibilityChange);
            input.removeEventListener("change", afterChooseFile);

            setTimeout(() => {
                handleSelectedFile();
                input.remove();
            }, 200);

        }

        const handleVisibilityChange = () => {
            if (document.visibilityState === "visible") {
                afterChooseFile();
            }
        }

        window.addEventListener("focus", afterChooseFile, { once: true });
        document.addEventListener("touchstart", afterChooseFile, { once: true });
        document.addEventListener("visibilitychange", handleVisibilityChange);
        input.addEventListener("change", afterChooseFile, { once: true });

        input.click();
    });
}

const calculateMD5 = {
    chunkSize: 64 * 1024 * 1024,
    fileReader: new FileReader(),
    hasher: null,

    hashChunk: function (chunk) {
        return new Promise((resolve, reject) => {
            this.fileReader.onload = async (e) => {
                const view = new Uint8Array(e.target.result);
                this.hasher.update(view);
                resolve();
            };

            this.fileReader.readAsArrayBuffer(chunk);
        });
    },

    readFile: async function (file) {
        if (this.hasher) {
            this.hasher.init();
        } else {
            this.hasher = await hashwasm.createMD5();
        }

        const chunkNumber = Math.floor(file.size / this.chunkSize);

        for (let i = 0; i <= chunkNumber; i++) {
            const chunk = file.slice(
                this.chunkSize * i,
                Math.min(this.chunkSize * (i + 1), file.size)
            );
            await this.hashChunk(chunk);
        }

        const hash = this.hasher.digest();
        return Promise.resolve(hash);
    }
};

