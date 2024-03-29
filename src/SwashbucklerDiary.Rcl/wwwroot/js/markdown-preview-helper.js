import { previewImage as internalPreviewImage } from './previewMediaElement.js';

export function copy(dotNetCallbackRef, callbackMethod, element) {
    element.addEventListener('click', function (event) {
        if (event.target.parentElement.parentElement.classList.contains('vditor-copy')) {
            dotNetCallbackRef.invokeMethodAsync(callbackMethod);
        }  
    });
}

export function previewImage(dotNetCallbackRef, callbackMethod, element) {
    return internalPreviewImage(dotNetCallbackRef, callbackMethod, element);
}
