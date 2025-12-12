import GestureUnlockRenderer  from '../../npm/fly-gesture-unlock@1.0.1/dist/fly-gesture-unlock.min.js'

const _instances = new WeakMap();

const getDefaultOptions = (primaryColor,errorColor)=> {
    return {
        anchorStatusStyles: {
            'not-selected': {
                // 锚点圆的边框宽、边框颜色、填充颜色
                anchorCircleFillColor: `rgba(${primaryColor}, 0.4)`,

                // 中心圆的边框宽、边框颜色、填充颜色
                centerCircleFillColor: `rgba(${primaryColor})`,
            },
            'selected': {
                // 锚点圆的边框宽、边框颜色、填充颜色
                anchorCircleFillColor: `rgba(${primaryColor}, 0.4)`,

                // 中心圆的边框宽、边框颜色、填充颜色
                centerCircleFillColor: `rgba(${primaryColor})`,
            },
            'error': {
                // 锚点圆的边框宽、边框颜色、填充颜色
                anchorCircleFillColor: `rgba(${errorColor}, 0.4)`,

                // 中心圆的边框宽、边框颜色、填充颜色
                centerCircleFillColor: `rgba(${errorColor})`,
            }

        },
        lineStatusStyles: {
            'normal': {
                lineColor: `rgba(${primaryColor})`,
                lineWidth: 4,
            },
            'error': {
                lineColor: `rgba(${errorColor})`,
                lineWidth: 4,
            }
        },
        config: {
            // 用于配置箭头
            arrow: {
                // 用于控制是否渲染连线上面的箭头
                show: false
            },
            // 用于控制连线是否会遮挡锚点
            isLineCoverAnchor: false,
            // 用于控制选中两个锚点的时候，其线段所经过的锚点会不会被自动选中
            isLineAutoSelect: true,
            // 用于控制锚点是否能够被重复选中
            isAnchorRepeatSelect: false
        }
    }
};

const getDefaultMatrixFactoryOptions = (container)=> {
    return {
        canvasSize: { width: container.clientWidth, height: container.clientHeight },
        padding: 30,
        matrix: { row: 3, column: 3 },
        anchor: { anchorCircleRadius: 20, centerCircleRadius: 8 },
    }
};

export function init(dotNetRef, container, options, matrixFactoryOptions) {
    if (!dotNetRef || !container) {
        return;
    }

    container.innerHTML = '';
    
    if (!options) {
        options = {};
    }

    if (!options.anchorDefines) {
        const defaultMatrixFactoryOptions = getDefaultMatrixFactoryOptions(container);
        options.anchorDefines = GestureUnlockRenderer.AnchorMatrixFactory({
            ...defaultMatrixFactoryOptions,
            matrixFactoryOptions
        });
    }

    const style = getComputedStyle(document.documentElement);
    const primaryColor = style.getPropertyValue('--m-theme-primary').trim();
    const errorColor = style.getPropertyValue('--m-theme-error').trim();
    const defaultOptions = getDefaultOptions(primaryColor, errorColor);

    const gestureUnlockRenderer = new GestureUnlockRenderer({
        ...defaultOptions,
        ...options,
        container: container,
        events: {
            end: (anchors) => {
                const pattern = anchors.map(anchor => anchor.id);
                dotNetRef.invokeMethodAsync('OnEnd', pattern);
            }
        }
    });

    _instances.set(container, gestureUnlockRenderer);
}

export function reset(container) {
    const instance = _instances.get(container);
    if (instance) {
        instance.reset();
    }
}

export function freeze(container) {
    const instance = _instances.get(container);
    if (instance) {
        instance.freeze();
    }
}

export function unFreeze(container) {
    const instance = _instances.get(container);
    if (instance) {
        instance.unFreeze();
    }
}

export function setStatus(container, status) {
    const instance = _instances.get(container);
    if (instance) {
        instance.setStatus(status);
    }
}
