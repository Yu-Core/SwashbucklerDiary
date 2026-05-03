import { snapdom } from '../npm/snapdom@2.0.1/dist/snapdom.min.js';

export async function getScreenshotStream(selector, proxyUrl) {
    const el = document.querySelector(selector);
    if (!el) {
        return null;
    }

    const out = await snapdom(el, {
        type: 'png',
        useProxy: proxyUrl || 'https://api.allorigins.win/raw?url=',
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


