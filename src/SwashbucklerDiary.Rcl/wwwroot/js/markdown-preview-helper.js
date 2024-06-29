import { previewImage } from './previewMediaElement.js';

export function after(dotNetCallbackRef, element) {
    if (!element) {
        return;
    }

    copy(dotNetCallbackRef, element);
    previewImage(dotNetCallbackRef, element);
    handleA(dotNetCallbackRef, element);
}

function copy(dotNetCallbackRef, element) {
    element.addEventListener('click', function (event) {
        if (event.target.closest('.vditor-copy')) {
            dotNetCallbackRef.invokeMethodAsync("Copy");
        }
    });
}

function handleA(dotNetCallbackRef, element) {
    element.addEventListener('click', function (event) {
        var link = event.target.closest('a');
        if (!link || !link.hasAttribute('href')) {
            return;
        }

        let href = link.getAttribute('href');
        if (href.startsWith('#')) {
            event.preventDefault();
            const url = location.origin + location.pathname + location.search + href;
            dotNetCallbackRef.invokeMethodAsync('NavigateToReplace', url);
        }
    });
}
