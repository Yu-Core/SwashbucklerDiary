
function openUri(uri, timeout = 1500) {
    return new Promise((resolve) => {
        let finished = false;

        const done = (result) => {
            if (finished) return;
            finished = true;
            cleanup();
            resolve(result);
        };

        const handlePageChange = () => {
            done(true);
        };

        const handleVisibilityChange = () => {
            // 页面进入后台，认为「可能」成功
            if (document.hidden) {
                done(true);
            }
        };

        const cleanup = () => {
            window.removeEventListener('blur', handlePageChange);
            window.removeEventListener('pagehide', handlePageChange);
            document.removeEventListener('visibilitychange', handleVisibilityChange);
        };

        window.addEventListener('blur', handlePageChange);
        document.addEventListener('visibilitychange', handlePageChange);
        window.addEventListener('pagehide', handleVisibilityChange);

        // 尝试打开
        const a = document.createElement('a');
        a.href = uri;
        a.target = '_blank';
        a.rel = 'noopener noreferrer';
        a.style.display = 'none';
        document.body.appendChild(a);
        a.click();
        a.remove();

        // 超时兜底
        setTimeout(() => {
            done(false);
        }, timeout);
    });
}

async function setClipboard(text) {
    if (!text && text !== '') return false;

    // 优先使用 Clipboard API
    if (
        navigator.clipboard &&
        typeof navigator.clipboard.writeText === 'function' &&
        window.isSecureContext
    ) {
        try {
            await navigator.clipboard.writeText(text);
            return true;
        } catch (err) {
            console.warn('Clipboard API failed, fallback to execCommand:', err);
        }
    }

    // fallback
    return execCommandCopy(text);
}

function execCommandCopy(text) {
    const textarea = document.createElement('textarea');

    textarea.value = text;
    textarea.readOnly = true;

    Object.assign(textarea.style, {
        position: 'fixed',
        top: '-9999px',
        left: '-9999px',
        opacity: '0'
    });

    document.body.appendChild(textarea);
    textarea.select();

    try {
        return document.execCommand('copy');
    } catch (err) {
        console.error('execCommand copy failed:', err);
        return false;
    } finally {
        document.body.removeChild(textarea);
    }
}

async function shareText(title, text) {
    if (!navigator.share) {
        console.warn('Web Share API is not supported in this browser');
        return false;
    }

    try {
        await navigator.share({ title, text });
        return true;
    } catch (err) {
        console.error('Share failed:', err);
        return false;
    }
}

async function internalShareFile(title, data, fileName, mimeType) {
    if (!navigator.share) {
        console.warn('Web Share API is not supported in this browser');
        return false;
    }

    try {
        const file = new File([data], fileName, { type: mimeType });
        const shareData = {
            title: title,
            files: [file]
        };

        if (navigator.canShare && !navigator.canShare(shareData)) {
            console.error('Cannot share this file type:', mimeType);
            return false;
        }

        await navigator.share(shareData);
        return true;

    } catch (error) {
        console.error('Share failed:', error);
        return false;
    }
}

function internalDownloadFile(fileName, url) {
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    a.style.display = 'none';

    document.body.appendChild(a);
    a.click();
    a.remove();
}

function internalChooseFiles(accept = '', multiple = false) {
    return new Promise((resolve) => {
        const input = document.createElement('input');
        input.type = 'file';
        input.accept = accept;
        input.multiple = multiple;
        input.style.display = 'none';
        document.body.appendChild(input);

        let finished = false;

        const cleanup = () => {
            window.removeEventListener('focus', onFinish);
            document.removeEventListener('touchstart', onFinish);
            document.removeEventListener('visibilitychange', onVisibilityChange);
            input.removeEventListener('change', onFinish);
            input.remove();
        };

        const onFinish = () => {
            if (finished) return;
            finished = true;
            cleanup();

            // 给系统一点时间把 files 填充完（iOS 必要）
            setTimeout(() => {
                resolve(input.files);
            }, 200);
        };

        const onVisibilityChange = () => {
            if (document.visibilityState === 'visible') {
                onFinish();
            }
        };

        window.addEventListener('focus', onFinish, { once: true });
        document.addEventListener('touchstart', onFinish, { once: true });
        document.addEventListener('visibilitychange', onVisibilityChange);
        input.addEventListener('change', onFinish, { once: true });

        input.click();
    });
}

async function isBiometricSupported() {
    if (!window.PublicKeyCredential || !navigator.credentials || !navigator.credentials.create) {
        console.log("Browser does not support WebAuthn or Credential API");
        return false;
    }

    try {
        const isAvailable = await PublicKeyCredential.isUserVerifyingPlatformAuthenticatorAvailable();
        if (!isAvailable) {
            console.log("Platform authenticator not available (device may not support biometrics or it is not enabled)");
            return false;
        }
    } catch (e) {
        console.error("Error while checking platform authenticator:", e);
        return false;
    }

    return true;
}

async function biometricAuthenticateAsync() {
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

export {
    openUri,
    setClipboard,
    shareText,
    internalShareFile,
    internalDownloadFile,
    internalChooseFiles,
    isBiometricSupported,
    biometricAuthenticateAsync
}