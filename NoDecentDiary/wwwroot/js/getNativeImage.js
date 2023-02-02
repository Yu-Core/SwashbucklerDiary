
export async function streamToUrl(imageStream) {
    // 适配webview和web 
    const arrayBuffer = await imageStream.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    return (window.URL || window.webkitURL || window || {}).createObjectURL(blob);
}

export function revokeUrl(url) {
    (window.URL || window.webkitURL || window || {}).revokeObjectURL(url);
}
