export function init(dotNetObjectReference, element, index) {
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
                dotNetObjectReference.invokeMethodAsync("UpdateValue", this.activeIndex);
            },
        }
    });
}

export function slideTo(element, value) {
    if (!element || !element.Swiper) {
        return;
    }

    element.Swiper.slideTo(value);
}

export function dispose(element) {
    if (!element || !element.Swiper) {
        return;
    }

    element.Swiper.destroy(true, true);
}
