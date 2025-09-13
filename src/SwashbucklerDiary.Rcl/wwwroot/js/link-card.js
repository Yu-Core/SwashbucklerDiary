const cache = {};

class LinkCardRender {
    constructor(options = {}) {
        this.options = {
            ...options
        };
    }

    renderLinkCards(elements) {
        elements.forEach(el => {
            const href = el.href;

            const data = cache[href];
            if (data) {
                this.renderer(el, data);
                return;
            }

            this.fetchLinkData(href, html => {
                const data = this.parse(html, href);
                if (data.description) {
                    cache[href] = data;
                    this.renderer(el, data);
                }
            })
        })
    }

    fetchLinkData(url, handle) {
        const requestUrl = this.getUrl(url);
        fetch(requestUrl)
            .then((result) => result.text())
            .then(handle)
            .catch((error) => {
                return console.error('LinkCard Error:', error)
            })
    }

    renderer(el, data) {
        const dom = this.createDOM(data)

        // Reset the attribute
        Array.from(el.attributes).forEach((attr) => {
            dom.setAttribute(attr.name, attr.value)
        })
        el.parentNode.replaceChild(dom, el)
    }

    createDOM(data) {
        const card = document.createElement('a');
        card.className = 'link-card';
        card.href = data.url;

        const domain = new URL(data.url).hostname;

        card.innerHTML = `
        <div class="link-card__content">
            <div class="link-card__title">${data.title}</div>
            <div class="link-card__description">${data.description}</div>
            <div class="link-card__url">
                <img class="link-card__favicon" src="${data.favicon}" alt="favicon" onerror="${this.options.faviconSrc ? `this.src='${this.options.faviconSrc}';this.removeAttribute('onerror');` : "this.display='none'"}">
                ${domain}
            </div>
        </div>
        ${data.image ? `
        <div class="link-card__image">
            <img src="${data.image}" alt="${data.title}" onerror="this.style.display='none'">
        </div>
        ` : ''}
    `;
        return card;
    }

    parse(html, url) {
        let result = {
            title: '',
            description: '',
            image: '',
            favicon: '',
            url: url
        };

        try {
            const parser = new DOMParser();
            const doc = parser.parseFromString(html, 'text/html');

            const getMetaContent = (property) => {
                const element = doc.querySelector(`meta[property="og:${property}"], meta[name="og:${property}"]`);
                return element ? element.getAttribute('content') : null;
            };

            const resolveUrl = (relativeUrl) => {
                return relativeUrl && !relativeUrl.startsWith('http')
                    ? new URL(relativeUrl, new URL(url).origin).href
                    : relativeUrl;
            };

            // 获取标题
            result.title = getMetaContent('title') || doc.querySelector('title')?.textContent || url;

            // 获取描述
            result.description = getMetaContent('description') ||
                doc.querySelector('meta[name="description"]')?.getAttribute('content');

            if (!result.description) {
                return result;
            }

            // 获取图片
            let image = getMetaContent('image');
            result.image = image ? this.getUrl(resolveUrl(image)) : '';

            // 获取网站图标
            let favicon = doc.querySelector('link[rel="icon"], link[rel="shortcut icon"]')?.getAttribute('href');
            favicon = resolveUrl(favicon || `${new URL(url).origin}/favicon.ico`);

            // 处理 favicon
            result.favicon = this.getUrl(favicon);

        } catch (error) {
            console.warn('LinkCard Error: Failed to parse', error);
        }

        return result;
    }


    getUrl(url) {
        if (this.options.proxyUrl) {
            return this.options.proxyUrl + encodeURIComponent(url);
        }

        return url;
    }
}
export function renderLinkCards(elements, options) {
    const renderer = new LinkCardRender(options);
    renderer.renderLinkCards(elements);
}
