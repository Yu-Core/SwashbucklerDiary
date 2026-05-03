export function observer(el, invoker) {
  if (!el) return

  const resizeObserver = new ResizeObserver((entries => {
    invoker.invokeMethodAsync('OnEllipsisChange', el.offsetWidth < el.scrollWidth)
  }))

  resizeObserver.observe(el);

  return resizeObserver;
}