class ScrollListener {
    constructor(element, callback) {
        this.element = element;
        this.callback = callback;
        this.ticking = false;
        this.handleScroll = this.handleScroll.bind(this);
        this.element.addEventListener('scroll', this.handleScroll);
    }

    removeListening() {
        this.element.removeEventListener('scroll', this.handleScroll);
    }

    handleScroll() {
        if (!this.ticking) {
            window.requestAnimationFrame(() => {
                this.callback();
                this.ticking = false;
            });

            this.ticking = true;
        }
    }
}

export function init(selector, el, dotNetObjectReference) {
    const parent = document.querySelector(selector);
    if (!parent || !el) return;
    let show = parent.scrollTop > parent.clientHeight;
    dotNetObjectReference.invokeMethodAsync("UpdateShow", show);
    return new ScrollListener(parent, () => {
        const currentShow = parent.scrollTop > parent.clientHeight;
        if (show != currentShow) {
            show = currentShow;
            dotNetObjectReference.invokeMethodAsync("UpdateShow", show);
        }
    });
}
