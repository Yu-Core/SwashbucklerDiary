import { snapdom } from '../npm/snapdom@2.0.1/dist/snapdom.min.js';

export async function getScreenshotStream(selector, proxyUrl) {
    const el = document.querySelector(selector);
    if (!el) {
        return null;
    }

    // 信笺元素使用自定义 Web 字体：snapdom 的 embedFonts 默认为 false，不开启时
    // @font-face 不会收进导出的 SVG，光栅化时字形回退系统楷体。按元素是否含信笺自动
    // 开启，并等待字体加载完成（避免固定延时既不可靠又无谓空等）。
    const embedFonts = !!el.querySelector('.letterpaper');
    if (embedFonts && document.fonts) {
        await document.fonts.ready;
    }

    const out = await snapdom(el, {
        type: 'png',
        useProxy: proxyUrl || 'https://api.allorigins.win/raw?url=',
        embedFonts,
        /*plugins: [{
            name: 'screenshot-plugin',
            async afterClone(context) {
                console.log(context.clone);
            },
        }]*/
    });
    const blob = await out.toBlob(null);
    const arrayBuffer = await blob.arrayBuffer();
    return new Uint8Array(arrayBuffer);
}


