const _instances = new WeakMap();

export function init(dotNetObjectReference, el, options) {
    if (!el) {
        return;
    }

    const instance = _instances.get(el);
    if (instance) {
        instance.destroy(true, true);
    }

    const swiper = new Swiper(el, options);
    swiper.on("slideChangeTransitionStart", function () {
        dotNetObjectReference.invokeMethodAsync("UpdateValue", this.activeIndex);
    });
    _instances.set(el, swiper);
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

export function destroy(el) {
    if (!el) {
        return;
    }

    const instance = _instances.get(el);
    if (!instance) {
        return;
    }

    instance.destroy(true, true);
}
