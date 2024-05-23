import { previewImage as internalPreviewImage } from './previewMediaElement.js';

export function after(dotNetCallbackRef, element) {
    if (!element) {
        return;
    }

    copy(dotNetCallbackRef, element);
    previewImage(dotNetCallbackRef, element);
}

function copy(dotNetCallbackRef, element) {
    element.addEventListener('click', function (event) {
        if (event.target.parentElement.parentElement.classList.contains('vditor-copy')) {
            dotNetCallbackRef.invokeMethodAsync("Copy");
        }
    });
}
function previewImage(dotNetCallbackRef, element) {
    return internalPreviewImage(dotNetCallbackRef, element);
}
