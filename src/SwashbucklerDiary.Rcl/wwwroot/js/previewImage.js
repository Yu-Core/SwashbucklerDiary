export function previewImage(dotNetCallbackRef, callbackMethod, element) {
    element.addEventListener('click', function (event) {
        if (event.target.tagName === 'IMG') {
            dotNetCallbackRef.invokeMethodAsync(callbackMethod, event.target.getAttribute('src'));
        }
    });
}
