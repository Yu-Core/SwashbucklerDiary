import {
    openUri,
    setClipboard,
    shareText,
    internalShareFile,
    internalDownloadFile,
    internalChooseFiles,
    isBiometricSupported,
    biometricAuthenticateAsync
} from "../_content/SwashbucklerDiary.Rcl.Web/js/platformIntegration.js"

const FS = Blazor.runtime.Module.FS;

async function shareFile(title, filePath, fileName, mimeType) {
    try {
        // 检查文件是否存在
        const pathInfo = FS.analyzePath(filePath);
        if (!pathInfo.exists) {
            console.warn(`The file does not exist: ${filePath}`);
            return false;
        }

        // 读取文件数据
        const fileData = FS.readFile(filePath);

        // 调用内部分享函数，并返回其布尔结果
        return await internalShareFile(title, fileData, fileName, mimeType);
    } catch (err) {
        console.error('Share failed:', err);
        return false;
    }
}

function downloadFile(fileName, filePath) {
    const pathInfo = FS.analyzePath(filePath);
    if (!pathInfo.exists) {
        console.warn(`The file does not exist: ${filePath}`);
        return;
    }

    const fileData = FS.readFile(filePath);
    var blob = new Blob([fileData], { type: 'application/octet-stream' });
    const url = URL.createObjectURL(blob);

    internalDownloadFile(fileName, url);

    URL.revokeObjectURL(url);
}

async function chooseFiles(dotNetObj, accept, fileExtensions, multiple = false) {
    const files = await internalChooseFiles(accept, multiple);

    const results = [];

    if (files.length === 0) {
        return results;
    }

    for (const file of files) {
        const fileExtension = file.name.substring(file.name.lastIndexOf("."));
        if (fileExtensions && fileExtensions.indexOf(fileExtension) < 0) {
            continue;
        }

        try {
            const fileName = file.name;
            const contents = await readFileAsArrayBuffer(file);

            const randomFolderName = await dotNetObj.invokeMethodAsync("RandomName");
            const folderPath = `/cache/${randomFolderName}`;
            FS.mkdir(folderPath);

            const filePath = `${folderPath}/${fileName}`;
            FS.writeFile(filePath, new Uint8Array(contents), { encoding: 'binary' });
            results.push(filePath);
        } catch (e) {
            console.error(e);
        }
    }

    return results;
}

// Utility function to read file as array buffer
function readFileAsArrayBuffer(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = () => resolve(reader.result);
        reader.onerror = reject;
        reader.readAsArrayBuffer(file);
    });
}

export {
    openUri,
    setClipboard,
    shareText,
    shareFile,
    downloadFile,
    chooseFiles,
    isBiometricSupported,
    biometricAuthenticateAsync
}