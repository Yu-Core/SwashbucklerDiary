import '/npm/html2canvas/1.4.1/dist/html2canvas.min.js';
export function getScreenshotBase64(element) {
    return new Promise((resolve, reject) => {
        html2canvas(document.querySelector(element), {
            allowTaint: true,
            useCORS: true,
            onclone: (clonedDocument) => {
                Array.from(clonedDocument.querySelectorAll('textarea')).forEach((textArea) => {
                    const div = clonedDocument.createElement('div')
                    div.innerText = textArea.value
                    textArea.style.display = 'none'
                    textArea.parentElement.append(div)
                })
            }
        }).then(canvas => {
            resolve(canvas.toDataURL())
        });
    });
}