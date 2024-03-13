export function initSwiper(dotNetObjectReference, callbackMethod, element, index) {
    if (!element) {
        return;
    }

    element.Swiper = new Swiper(element, {
        observer: true,
        observeParents: true,
        observeSlideChildren: true,
        //autoHeight: true,//自动高度
        simulateTouch: false,//禁止鼠标模拟
        initialSlide: index,//设定初始化时slide的索引
        resistanceRatio: 0.7,
        on: {
            slideChangeTransitionStart: function () {
                dotNetObjectReference.invokeMethodAsync(callbackMethod, this.activeIndex);
            },
        }
    });
}

export function slideTo(element, value) {
    if (!element) {
        return;
    }

    element.Swiper.slideTo(value);
}

export function dispose(element) {
    if (!element) {
        return;
    }

    element.Swiper.destroy(true, true);
}
