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
    // 创建一个新的<a>元素
    const a = document.createElement("a");
    // 设置href属性为传入的URI
    a.href = uri;

    if (blank) {
        // 设置打开链接时使用新的标签页
        a.target = "_blank";
    }
    // 设置样式以使<a>元素不显示
    a.style.display = "none";
    if (typeof a.protocol === "undefined") {
        return false;
    }
    // 将<a>元素添加到文档中
    document.body.appendChild(a);
    // 触发点击事件打开新标签页
    a.click();
    // 移除<a>元素
    a.remove();
    return true;
};

export function isMailSupported() {
    var a = document.createElement("a");
    a.href = "mailto:test@example.com";
    return typeof a.protocol !== "undefined" && a.protocol === "mailto:";
}

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

        function handlePickFile() {
            const file = input.files[0];
            if (file) {
                if (suffix && !file.name.endsWith(suffix)) {
                    resolve("");
                }
                else {
                    // wasm模式，C#计算md5比较难，所以在js中计算
                    calculateMD5(file)
                        .then((md5) => {
                            var newFileName = `${md5}${file.name.substring(file.name.lastIndexOf("."))}`;
                            var newFile = new File([file], newFileName);
                            var reader = new FileReader();
                            reader.onload = function (event) {
                                // 获取文件内容
                                var contents = event.target.result;

                                // 写入 Emscripten 文件系统
                                var filePath = `cache/${newFile.name}`;
                                Module.FS.writeFile(filePath, new Uint8Array(contents), { encoding: 'binary' });
                                resolve(filePath);
                            };
                            reader.readAsArrayBuffer(newFile);
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

        function handleFocus() {
            setTimeout(() => {
                handlePickFile();
                // 移除事件监听器，确保代码只执行一次
                window.removeEventListener("focus", handleFocus);
                document.removeEventListener("visibilitychange", handleVisibilityChange);
            }, 200);
        }

        function handleVisibilityChange() {
            if (document.visibilityState === "visible") {
                handleFocus();
            } 
        }

        window.addEventListener("focus", handleFocus, { once: true });
        document.addEventListener("visibilitychange", handleVisibilityChange);
        input.click();
    });
}

function calculateMD5(pickfile) {
    return new Promise((resolve, reject) => {
        var blobSlice = File.prototype.slice || File.prototype.mozSlice || File.prototype.webkitSlice,
            file = pickfile,
            chunkSize = 2097152,                             // Read in chunks of 2MB
            chunks = Math.ceil(file.size / chunkSize),
            currentChunk = 0,
            spark = new SparkMD5.ArrayBuffer(),
            fileReader = new FileReader();

        fileReader.onload = function (e) {
            console.log('read chunk nr', currentChunk + 1, 'of', chunks);
            spark.append(e.target.result);                   // Append array buffer
            currentChunk++;

            if (currentChunk < chunks) {
                loadNext();
            } else {
                resolve(spark.end());
            }
        };

        fileReader.onerror = function (event) {
            reject("FileReader error: " + event.target.errorCode);
        };

        function loadNext() {
            var start = currentChunk * chunkSize,
                end = ((start + chunkSize) >= file.size) ? file.size : start + chunkSize;

            fileReader.readAsArrayBuffer(blobSlice.call(file, start, end));
        }

        loadNext();
    });
}
