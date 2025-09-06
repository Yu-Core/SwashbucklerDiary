const _instances = new WeakMap();

export function init(dotNetObjectReference, el, index) {
    if (!el) {
        return;
    }

    const instance = new Swiper(el, {
        observer: true,
        observeParents: true,
        observeSlideChildren: true,
        //autoHeight: true,//自动高度
        simulateTouch: false,//禁止鼠标模拟
        initialSlide: index,//设定初始化时slide的索引
        resistanceRatio: 0.7,
        speed: 250,
        on: {
            slideChangeTransitionStart: function () {
                dotNetObjectReference.invokeMethodAsync("UpdateValue", this.activeIndex);
            },
        }
    });

    _instances.set(el, instance);
}

export function slideTo(el, value) {
    if (!el) {
        return;
    }

    const instance = _instances.get(el);
    if (!instance) {
        return;
    }

    instance.slideTo(value);
}

export function dispose(el) {
    if (!el) {
        return;
    }

    const instance = _instances.get(el);
    if (!instance) {
        return;
    }

    instance.destroy(true, true);
}
