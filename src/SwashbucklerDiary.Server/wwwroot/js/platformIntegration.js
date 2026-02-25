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

async function shareFile(title, url, fileName, mimeType) {
    try {
        const res = await fetch(url);
        if (!res.ok) {
            console.error(`HTTP error: ${res.status}`);
            return false;
        }

        const fileData = await res.blob();
        // internalShareFile 已经处理了 Web Share API 支持和分享过程，并返回 boolean
        return await internalShareFile(title, fileData, fileName, mimeType);
    } catch (err) {
        console.error("Share failed", err);
        return false;
    }
}

function downloadFile(fileName, url) {
    internalDownloadFile(fileName, url);
}

async function chooseFiles(accept, fileExtensions, multiple = false) {
    const files = await internalChooseFiles(accept, multiple);

    if (files.length === 0) {
        return [];
    }

    const formData = new FormData();
    for (const file of files) {
        const ext = file.name.slice(file.name.lastIndexOf(".")).toLowerCase();
        if (fileExtensions.length && !fileExtensions.includes(ext)) continue;
        formData.append("files", file);
    }

    if (![...formData.keys()].length) return [];

    try {
        const res = await fetch("api/upload", {
            method: "POST",
            body: formData,
        });

        if (!res.ok) {
            console.error(`HTTP error: ${res.status}`);
        }

        return await res.json();
    } catch (err) {
        console.error("Upload failed", err);
        return [];
    }
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