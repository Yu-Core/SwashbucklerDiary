function ElementVisible(dotNetCallbackRef, callbackMethod, parentSelector, childSelector) {
    var parent = document.querySelector(parentSelector);
    var child = document.querySelector(childSelector);
    let timeout;
    parent.addEventListener('scroll', function () {
        clearTimeout(timeout);
        timeout = setTimeout(function () {
            var parentScrollTop = parent.scrollTop;
            var childBottom = child.offsetTop + child.offsetHeight;
            var visible = childBottom > parentScrollTop;
            dotNetCallbackRef.invokeMethodAsync(callbackMethod, visible);
        }, 200);// 设置延迟时间，单位为毫秒
        
    });
}
