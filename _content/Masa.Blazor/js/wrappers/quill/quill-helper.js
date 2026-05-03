const defaultUploadConfig = {
    action: '',
    methods: 'POST',
    token: '',
    tokenName: '',
    name: 'file',
    accept: 'image/png, image/gif, image/jpeg, image/bmp, image/x-icon',
    pathKey: 'path'
}
const defaultToolbarContainer = [
    ['bold', 'italic', 'underline', 'strike'],
    ['blockquote', 'code-block'],
    [{ 'header': 1 }, { 'header': 2 }],
    [{ 'list': 'ordered' }, { 'list': 'bullet' }],
    [{ 'script': 'sub' }, { 'script': 'super' }],
    [{ 'indent': '-1' }, { 'indent': '+1' }],
    [{ 'direction': 'rtl' }],
    [{ 'header': [1, 2, 3, 4, 5, 6, false] }],
    [{ 'color': [] }, { 'background': [] }],
    [{ 'font': [] }],
    [{ 'align': [] }],
    ['clean'],
    ['emoji'],
    ['link', 'image', 'video']
]

export function init(quillElement, obj, toolBarContainer, additionalModules, readOnly,
    placeholder, theme, isMarkdown, uploadConfig) {
    if (!Quill || !quillElement) return;

    let toolbar = {
        container: toolBarContainer.childNodes.length == 0 ? defaultToolbarContainer : toolBarContainer,
        handlers: {}
    };

    toolbar.handlers.image = function image() {
        let self = this;
        var contenteditable = self.quill.root.getAttribute('contenteditable');
        if (contenteditable == 'false') return;
        handlerImage(obj, self, uploadConfig);
    };

    let options = {
        modules: {
            toolbar: toolbar,
            "emoji-toolbar": true,
            "emoji-shortname": true,
        },
        placeholder: placeholder,
        readOnly: readOnly,
        theme: theme
    };
    
    if(additionalModules) {
        additionalModules.split(',').filter(item => !!item).forEach(item => {
            options.modules[item] = {}
        })
    }

    const editor = new Quill(quillElement, options);
    if (isMarkdown) {
        const markdownOptions = {};
        const quillMarkdown = new QuillMarkdown(editor, markdownOptions);
    }
    obj.invokeMethodAsync("HandleRenderedAsync");
    editor.on('selection-change', range => {
        if (!range) {
            obj.invokeMethodAsync('HandleOnBlurAsync');
        } else {
            obj.invokeMethodAsync('HandleOnFocusAsync');
        }
    })
    editor.on('text-change', (delta, oldDelta, source) => {
        const text = editor.getText();
        const html = editor.root.innerHTML;
        obj.invokeMethodAsync('HandleInputAsync', html);
    })
}
export function getContent(quillElement) {
    if (!quillElement) return
    return JSON.stringify(quillElement.__quill.getContents());
}
export function getText(quillElement) {
    if (!quillElement) return
    return quillElement.__quill.getText();
}
export function getHtml(quillElement) {
    if (!quillElement) return
    return quillElement.__quill.root.innerHTML;
}
export function setContent(quillElement, quillContent) {
    if (!quillElement) return
    const content = JSON.parse(quillContent);
    return quillElement.__quill.setContents(content, 'api');
}
export function setHtml(quillElement, quillHTMLContent) {
    if (!quillElement) return
    return quillElement.__quill.root.innerHTML = quillHTMLContent;
}
export function enableEditor(quillElement, mode) {
    if (!quillElement) return
    quillElement.__quill.enable(mode);
}
export function insertImage(quillElement, imageURL, editorIndex) {
    if (!quillElement) return
    let Delta = Quill.import('delta');

    if (!!!editorIndex && editorIndex != 0) {
        if (quillElement.__quill.getSelection() !== null) {
            editorIndex = quillElement.__quill.getSelection().index;
        }
        else {
            editorIndex = 0;
        }
    }

    return quillElement.__quill.updateContents(
        new Delta()
            .retain(editorIndex)
            .insert({ image: imageURL },
                { alt: imageURL }));
}
export function clearFile(element) {
    let fileInput = element.querySelector('input.ql-image[type=file]');
    fileInput.value = '';
}
export function uploadFilePic(quillElement, element, uploadConfig, index) {
    if (!quillElement) return
    let quill = quillElement.__quill
    let fileInput = element.querySelector('input.ql-image[type=file]')
    // Create formdata
    let formData = new FormData()
    let files = fileInput.files;
    formData.append(uploadConfig.name, files[index])
    let oReq = new XMLHttpRequest();
    oReq.onreadystatechange = function () {
        if (oReq.readyState == 4 && oReq.status == 200) {
            let json = JSON.parse(oReq.responseText);
            let pathKey = uploadConfig.pathKey || "path";
            let url = json[pathKey];
            let length = quill.getSelection().index;
            quill.insertEmbed(length, 'image', url);
            quill.setSelection(length + 1);
            index += 1;
            if (index < files.length) {
                uploadFilePic(quillElement, element, uploadConfig, index);
            }
            else {
                fileInput.value = '';
            }
        }
    }

    oReq.open(uploadConfig.methods, uploadConfig.action);
    // If a token is required and exists
    if (uploadConfig.token) {
        oReq.setRequestHeader(uploadConfig.tokenName, uploadConfig.token);
    }
    oReq.send(formData);
}
export default {
    init,
    getContent,
    getText,
    getHtml,
    setHtml,
    enableEditor,
    insertImage
}

function handlerImage(obj, self, uploadConfig) {
    let _uploadConfig = uploadConfig || defaultUploadConfig;
    let fileInput = self.container.querySelector('input.ql-image[type=file]')
    if (fileInput === null) {
        fileInput = document.createElement('input')
        fileInput.setAttribute('type', 'file')
        // Set picture parameter name
        if (_uploadConfig.name) {
            fileInput.setAttribute('name', _uploadConfig.name)
        }
        // You can set the format of uploaded pictures
        fileInput.setAttribute('accept', _uploadConfig.accept)
        fileInput.setAttribute('multiple', 'multiple')
        fileInput.classList.add('ql-image')
        // Listen to select file
        fileInput.addEventListener('change', function () {
            obj.invokeMethodAsync('HandleFileChanged', getFileInfo(fileInput.files));
        })

        self.container.appendChild(fileInput)
    }
    fileInput.click()
}

function getFileInfo(files) {
    if (files && files.length > 0) {
        let fileInfo = [];
        for (let i = 0; i < files.length; i++) {
            let file = files[i];
            const objectUrl = getObjectUrl(file);
            fileInfo.push({
                fileName: file.name,
                size: file.size,
                objectUrl: objectUrl,
                type: file.type
            });
        }

        return fileInfo;
    }
}

function getObjectUrl(file) {
    let url = null;
    if (window.URL != undefined) {
        url = window.URL.createObjectURL(file);
    } else if (window.webkitURL != undefined) {
        url = window.webkitURL.createObjectURL(file);
    }
    return url;
}
