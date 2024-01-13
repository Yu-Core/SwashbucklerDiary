export function getScreenshotStream(dotNetCallbackRef, callbackMethod, selector) {
    return new Promise((resolve, reject) => {
        html2canvas(document.querySelector(selector), {
            allowTaint: true,
            onclone: (cloned) => {
                Array.from(cloned.querySelectorAll('textarea')).forEach((textArea) => {
                    const div = cloned.createElement('div');
                    div.innerText = textArea.value;
                    textArea.style.display = 'none';
                    textArea.parentElement.append(div);
                });
                return convertAllImagesToBase64(cloned, dotNetCallbackRef, callbackMethod);
            },
        }).then(canvas => {
            canvas.toBlob(blob => {
                blob.arrayBuffer().then(data => {
                    resolve(new Uint8Array(data));
                });
            })
        });
    });
}

//Fix screenshot not loading cross domain images
//https://github.com/niklasvh/html2canvas/issues/1614#issuecomment-468452767
function convertAllImagesToBase64(cloned, dotNetCallbackRef, callbackMethod) {
    const pendingImagesPromises = [];
    const pendingPromisesData = [];

    let allImages = cloned.getElementsByTagName('img');
    const images = Array.from(allImages).filter(img => {
        let imgURL = new URL(img.src);
        return imgURL.origin !== window.location.origin;
    });

    for (let i = 0; i < images.length; i += 1) {
        // First we create an empty promise for each image
        const promise = new Promise((resolve, reject) => {
            pendingPromisesData.push({
                index: i, resolve, reject,
            });
        });
        // We save the promise for later resolve them
        pendingImagesPromises.push(promise);
    }

    for (let i = 0; i < images.length; i += 1) {
        // We fetch the current image
        dotNetCallbackRef.invokeMethodAsync(callbackMethod, images[i].src)
            .then((data) => {
                const pending = pendingPromisesData.find((p) => p.index === i);
                images[i].src = data;
                pending.resolve(data);
            })
            .catch((e) => {
                const pending = pendingPromisesData.find((p) => p.index === i);
                pending.reject(e);
            });
    }

    // This will resolve only when all the promises resolve
    return Promise.all(pendingImagesPromises);
};
