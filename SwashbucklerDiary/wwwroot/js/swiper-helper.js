import '/npm/swiper/7.4.1/swiper-bundle.min.js';
export function initSwiper(dotNetCallbackRef, callbackMethod, dom, selector, index) {
    if (dom == null) {
        return;
    }

    dom.Swiper = new Swiper(selector, {
        observer: true,
        observeParents: true,
        observeSlideChildren: true,
        //autoHeight: true,//自动高度
        simulateTouch: false,//禁止鼠标模拟
        initialSlide: index,//设定初始化时slide的索引
        resistanceRatio: 0.7,
        on: {
            slideChangeTransitionStart: function () {
                dotNetCallbackRef.invokeMethodAsync(callbackMethod, this.activeIndex);
            },
        }
    });
}

export function slideTo(dom, value) {
    dom.Swiper.slideTo(value);
}
