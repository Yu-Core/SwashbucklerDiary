export function ElementVisible(dotNetCallbackRef, callbackMethod, parentSelector, childSelector) {
    var parent = document.querySelector(parentSelector);
    var child = document.querySelector(childSelector);
    parent.addEventListener('scroll', function () {
        var parentScrollTop = parent.scrollTop;
        var childBottom = child.offsetTop + child.offsetHeight;
        var visible = childBottom > parentScrollTop;
        dotNetCallbackRef.invokeMethodAsync(callbackMethod, visible);
    });
}
