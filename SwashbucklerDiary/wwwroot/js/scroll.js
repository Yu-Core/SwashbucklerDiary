function listenElementVisibility(dotNetCallbackRef, callbackMethod, parent, child) {
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
