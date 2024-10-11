export function getScreenshotBase64(selector) {
    return new Promise((resolve, reject) => {
        html2canvas(document.querySelector(selector), {
            allowTaint: true,
            onclone: (cloned) => {
                Array.from(cloned.querySelectorAll('textarea')).forEach((textArea) => {
                    const div = cloned.createElement('div')
                    div.innerText = textArea.value
                    textArea.style.display = 'none'
                    textArea.parentElement.append(div)
                });
            },
        }).then(canvas => {
            resolve(canvas.toDataURL())
        });
    });
}