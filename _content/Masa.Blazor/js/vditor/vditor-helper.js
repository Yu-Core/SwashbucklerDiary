const defaultOptions = {
    tab: '\t',
    mode: 'sv',
    preview: {
        actions: []
    },
    outline: true,
    cache: {
        enable: false,
    },
    icon: 'material'
}


export function init(domRef, obj, value, options, isUploadHandler) {
    let vditorOptions = {
        ...defaultOptions,
        ...options,
    }
    if (vditorOptions.hasOwnProperty('toolbar')) {
        vditorOptions.toolbar.forEach(btn => {
            if (typeof btn == 'object') {
                btn.click = () => {
                    obj.invokeMethodAsync('HandleToolbarButtonClickAsync', btn.name);
                }
            }
        })
    }
    if (isUploadHandler) {
        let { upload } = vditorOptions;

        let handler = (files) => {
            obj.invokeMethodAsync('HandleFileChanged');
        }

        vditorOptions.upload = {
            ...upload,
            handler
        }
    }
    SetDefaultFileNameHandle(vditorOptions);
    domRef.Vditor = new Vditor(domRef, {
        ...vditorOptions,
        value,
        after: () => {
            obj.invokeMethodAsync('HandleRenderedAsync', value);
        },
        input: (value) => {
            obj.invokeMethodAsync('HandleInputAsync', value);
        },
        focus: (value) => {
            obj.invokeMethodAsync('HandleFocusAsync', value);
        },
        blur: (value) => {
            obj.invokeMethodAsync('HandleBlurAsync', value);
        },
        esc: (value) => {
            obj.invokeMethodAsync('HandleEscPressAsync', value);
        },
        ctrlEnter: (value) => {
            obj.invokeMethodAsync('HandleCtrlEnterPressAsync', value);
        },
        select: (value) => {
            obj.invokeMethodAsync('HandleSelectAsync', value);
        },
    })
}
export function getValue(domRef) {
    return domRef.Vditor.getValue();
}
export function getHtml(domRef) {
    return domRef.Vditor.getHTML();
}
export function setValue(domRef, value, clearStack = false) {
    domRef.Vditor.setValue(value, clearStack);
}
export function insertValue(domRef, value, render = true) {
    domRef.Vditor.insertValue(value, render);
}
export function destroy(domRef) {
    domRef && domRef.Vditor.destroy();
}
export function disabled(domRef) {
    domRef && domRef.Vditor.disabled();
}
export function enable(domRef) {
    domRef && domRef.Vditor.enable();
}
export function preview(domRef) {
    domRef && domRef.querySelector('[data-type=preview]').click();
}
function SetDefaultFileNameHandle(vditorOptions) {
    let { upload } = vditorOptions;
    if (upload) upload.filename || (upload.filename = (name) => name);
}
