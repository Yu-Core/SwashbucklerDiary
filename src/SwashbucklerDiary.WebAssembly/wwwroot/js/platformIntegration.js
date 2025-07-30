export function openUri(uri) {
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
        a.target = "_blank";
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

export async function setClipboard(text) {
    if (navigator.clipboard) {
        try {
            await navigator.clipboard.writeText(text);
            return;
        } catch (e) {
            console.warn('Clipboard API fail，use execCommand:', e);
        }
    }

    execCommandCopy(text);
}

function execCommandCopy(text) {
    const textarea = document.createElement('textarea');
    textarea.value = text;

    textarea.style.position = 'fixed';
    textarea.style.opacity = '0';

    document.body.appendChild(textarea);

    textarea.select();
    textarea.setSelectionRange(0, textarea.value.length); // 兼容移动设备

    try {
        const success = document.execCommand('copy');
        if (!success) {
            console.warn('Copy failed, please check your browser permissions or try other methods');
        }
    } catch (err) {
        console.error('Copy failed:', err);
    } finally {
        document.body.removeChild(textarea);
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
            if (Blazor.runtime.Module.FS.analyzePath(path).exists) {
                const fileName = path.split('/').pop();
                const data = Blazor.runtime.Module.FS.readFile(path);
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
    const fileData = Blazor.runtime.Module.FS.readFile(path);
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

export function pickFilesAsync(accept, fileExtensions, multiple = false) {
    return new Promise((resolve) => {
        const input = document.createElement('input');
        input.type = 'file';
        input.accept = accept;
        input.multiple = multiple; // Set based on the parameter
        input.style.display = 'none';

        const handleFiles = async () => {
            const files = input.files;
            input.remove();
            const results = [];

            if (files.length === 0) {
                resolve(results);
                return;
            }

            for (const file of files) {
                const fileExtension = file.name.substring(file.name.lastIndexOf("."));
                if (fileExtensions && fileExtensions.indexOf(fileExtension) < 0) {
                    continue;
                }

                try {
                    const md5 = await calculateMD5.readFile(file);
                    const fileName = `${md5}${fileExtension}`;
                    const contents = await readFileAsArrayBuffer(file);

                    const filePath = `cache/${fileName}`;
                    Blazor.runtime.Module.FS.writeFile(filePath, new Uint8Array(contents), { encoding: 'binary' });
                    results.push(filePath);
                } catch {
                    results.push(""); // In case of error, add an empty string for the file
                }
            }

            resolve(results);
        };

        const handleVisibilityChange = () => {
            if (document.visibilityState === "visible") {
                afterChooseFiles();
            }
        };

        const afterChooseFiles = () => {
            window.removeEventListener("focus", afterChooseFiles);
            document.removeEventListener("touchstart", afterChooseFiles);
            document.removeEventListener("visibilitychange", handleVisibilityChange);
            input.removeEventListener("change", afterChooseFiles);

            setTimeout(handleFiles, 200);
        };

        window.addEventListener("focus", afterChooseFiles, { once: true });
        document.addEventListener("touchstart", afterChooseFiles, { once: true });
        document.addEventListener("visibilitychange", handleVisibilityChange);
        input.addEventListener("change", afterChooseFiles, { once: true });

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

// Utility function to read file as array buffer
function readFileAsArrayBuffer(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = () => resolve(reader.result);
        reader.onerror = reject;
        reader.readAsArrayBuffer(file);
    });
}

export async function isBiometricSupported() {
    // 检查浏览器支持
    if (!window.PublicKeyCredential || !navigator.credentials || !navigator.credentials.create) {
        return false;
    }

    // 检查设备支持
    const isAvailable = await PublicKeyCredential.isUserVerifyingPlatformAuthenticatorAvailable();
    if (!isAvailable) {
        return false;
    }

    return true;
}

export async function biometricAuthenticateAsync() {
    const isSupported = await isBiometricSupported();
    if (!isSupported) {
        return false;
    }

    try {
        const options = {
            publicKey: {
                challenge: new Uint8Array(32).buffer, // 随机挑战值
                rp: { id: window.location.hostname, name: document.title || 'This website' },
                user: {
                    id: new Uint8Array(16),
                    name: 'user@example.com',
                    displayName: 'visitor'
                },
                pubKeyCredParams: [{ type: "public-key", alg: -7 }],
                timeout: 60000,
                authenticatorSelection: {
                    authenticatorAttachment: "platform",
                    userVerification: "required"
                }
            }
        };
        await navigator.credentials.create(options);
        return true;
    } catch (error) {
        console.error('Validation failed:', error);
        return false;
    }
} 