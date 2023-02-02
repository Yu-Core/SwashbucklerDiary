/*出自 https://www.cnblogs.com/hejiale010426/p/17073079.html，有修改*/
/** 将byte转url对象 */
export async function streamToUrl(imageStream) {
    // 适配webview和web 
    const arrayBuffer = await imageStream.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    return (window.URL || window.webkitURL || window || {}).createObjectURL(blob);
}
/**
* 释放url对象，因为createObjectURL创建的对象一直会存在可能会占用过多的内存，请注意释放
*/
export function revokeUrl(url) {
    (window.URL || window.webkitURL || window || {}).revokeObjectURL(url);
}
