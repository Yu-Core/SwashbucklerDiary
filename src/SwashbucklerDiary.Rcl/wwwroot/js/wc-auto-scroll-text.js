class AutoScrollText extends HTMLElement {
    static get observedAttributes() {
        return ['scroll-speed', 'gap', 'animation-gap', 'fit-content'];
    }

    constructor() {
        super();
        this.attachShadow({ mode: 'open' });
        this._isScrolling = false;
        this._prevScrollingState = false;
        this._resizeObserver = null;
        this._mutationObserver = null;
        this._scrollSpeed = 60; // 默认值：60像素/秒
        this._gap = 20;  // 默认值：20像素间隔
        this._animationGap = 1;  // 默认值：1秒
        this._fitContent = null;
        this._paused = false;
    }

    connectedCallback() {
        this.render();
        this.setupScroll();
        this.setupResizeObserver();
        this.setupMutationObserver();
    }

    disconnectedCallback() {
        if (this._resizeObserver) {
            this._resizeObserver.disconnect();
        }
        if (this._mutationObserver) {
            this._mutationObserver.disconnect();
        }
    }

    attributeChangedCallback(name, oldValue, newValue) {
        if (oldValue === newValue) return;

        if (name === 'scroll-speed') {
            this._scrollSpeed = parseFloat(newValue) || 60;
        } else if (name === 'gap') {
            this._gap = parseFloat(newValue) || 20;
        } else if (name === 'animation-gap') {
            this._animationGap = parseFloat(newValue) || 1;
        } else if (name === 'fit-content') {
            this._fitContent = newValue;
        }

        // 仅在滚动状态下更新动画
        if (this._isScrolling) {
            this.setupScroll();
        }
    }

    setupMutationObserver() {
        // 监听内容变化
        this._mutationObserver = new MutationObserver(() => {
            this.render();
            this.setupScroll();
        });

        // 配置观察文本节点和子节点的变化
        this._mutationObserver.observe(this, {
            characterData: true,
            childList: true,
            subtree: true
        });
    }

    setupResizeObserver() {
        // 监听容器大小变化
        this._resizeObserver = new ResizeObserver(entries => {
            this.render();
            this.setupScroll();
        });

        this._resizeObserver.observe(this);
    }

    render() {
        const text = this.textContent.trim();
        const hostWidth = this._fitContent ? "fit-content" : "100%";
        const hostAfterContent = this._fitContent ? text : "0";

        this.shadowRoot.innerHTML = `
                    <style>
                        :host {
                            display: block;
                            width: ${hostWidth};
                            overflow: hidden;
                            position: relative;
                        }

                        :host::after{
                            content: "${hostAfterContent}";
                            white-space: nowrap;
                            display: block;
                            visibility: hidden;
                        }
                        
                        .container {
                            display: inline-flex;
                            justify-content: inherit;
                            align-items: center;
                            min-width: 100%;
                            height: 100%;
                            white-space: nowrap;
                            position: absolute;
                            left: 0;
                            top: 0;
                            will-change: transform;
                        }
                        
                        .gap {
                            display: inline-block;
                        }
                    </style>
                    
                    <div class="container">
                        <span class="text">${text}</span>
                        <span class="gap"></span>
                        <span class="text">${text}</span>
                    </div>
                `;
    }

    setupScroll() {
        const container = this.shadowRoot.querySelector('.container');
        const textSpan = this.shadowRoot.querySelector('.text');
        const gapSpan = this.shadowRoot.querySelector('.gap');

        // 清除旧动画
        container.style.animation = 'none';

        // 设置间隔大小
        gapSpan.style.width = `${this._gap}px`;

        // 计算容器总宽度
        const containerWidth = this.offsetWidth;
        const textWidth = textSpan ? textSpan.offsetWidth : 0;

        // 确定是否需要滚动
        this._isScrolling = textWidth > containerWidth;

        // 仅在文本宽度大于容器宽度时滚动
        if (this._isScrolling) {
            // 计算总滚动距离
            const totalDistance = textWidth + this._gap;

            // 计算动画时间（总距离 / 速度）
            const animationDuration = totalDistance / this._scrollSpeed + this._animationGap;

            // 不包含动画间隔时间的百分比
            const totalProgressPercentage = ((animationDuration - this._animationGap) / animationDuration) * 100;

            // 关键帧动画
            const keyframes = `
                        @keyframes scroll {
                            0% { transform: translateX(0); }
                            ${totalProgressPercentage}%, 100% { transform: translateX(-${totalDistance}px); }
                        }
                    `;

            const style = document.createElement('style');
            style.textContent = keyframes;
            this.shadowRoot.appendChild(style);

            // 应用动画
            container.style.animation = `scroll ${animationDuration}s linear infinite`;
            gapSpan.style.display = 'inline-block';
            container.querySelectorAll('.text')[1].style.display = 'inline';

            // 恢复播放状态
            if (this._paused) {
                container.style.animationPlayState = 'paused';
            } else {
                container.style.animationPlayState = 'running';
            }
        } else {
            // 文本宽度小于容器宽度时不滚动
            container.style.transform = 'none';
            container.style.animation = 'none';
            gapSpan.style.display = 'none';
            container.querySelectorAll('.text')[1].style.display = 'none';
        }

        this._prevScrollingState = this._isScrolling;
    }

    pause() {
        if (!this._isScrolling) return false;

        const container = this.shadowRoot.querySelector('.container');
        if (this._paused) {
            container.style.animationPlayState = 'running';
            this._paused = false;
            return false;
        } else {
            container.style.animationPlayState = 'paused';
            this._paused = true;
            return true;
        }
    }

    resume() {
        if (!this._isScrolling) return;

        const container = this.shadowRoot.querySelector('.container');
        container.style.animationPlayState = 'running';
        this._paused = false;
    }

    restart() {
        if (!this._isScrolling) return;

        const container = this.shadowRoot.querySelector('.container');
        // 重新开始动画
        container.style.animation = 'none';
        setTimeout(() => {
            this.setupScroll();
        }, 10);

        this.resume();
    }
}
if (!customElements.get('wc-auto-scroll-text')) { customElements.define('wc-auto-scroll-text', AutoScrollText); }