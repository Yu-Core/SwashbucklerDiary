const cache = {};

const resolveUrl = (u, base) => {
    if (!u) return '';
    try {
        return new URL(u, base).href;
    } catch {
        return u;
    }
};

const getMetaContent = (doc, property) => {
    const element = doc.querySelector(`meta[property="og:${property}"], meta[name="og:${property}"]`);
    return element ? element.getAttribute('content') : null;
};

const getTitle = (doc) => {
    const ogTitle = getMetaContent(doc, 'title');
    if (ogTitle) return ogTitle;
    const docTitle = doc.title;
    if (docTitle && docTitle.length > 0) {
        return docTitle;
    }
    return null;
}

const getDescription = (doc) => {
    const ogDescription = getMetaContent(doc, 'description');
    if (ogDescription) return ogDescription;
    const metaDescriptionElement = doc.querySelector('meta[name="description"]');
    if (metaDescriptionElement && metaDescriptionElement.content.length > 0) {
        return metaDescriptionElement.content;
    }
    return null;
}

const getImage = (doc, base) => {
    const ogImage = getMetaContent(doc, 'image');
    if (ogImage) return resolveUrl(ogImage, base);
    return null;
}

const getFavicon = (doc, base) => {
    const iconElement = doc.querySelector('link[rel="icon"], link[rel="shortcut icon"]')
    if (iconElement) {
        const href = iconElement.getAttribute('href')
        return resolveUrl(href || 'favicon.ico', base);
    }
    return null;
}

function isNormalUrl(str) {
    try {
        const url = new URL(str);
        // 限制只允许 http/https
        return url.protocol === "http:" || url.protocol === "https:";
    } catch (e) {
        return false;
    }
}

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

            // 获取标题
            result.title = getTitle(doc, 'title') || url;

            // 获取描述
            const description = getDescription(doc, 'description');
            if (description) {
                result.description = description;
            } else {
                return result;
            }

            const urlObj = new URL(url);
            const base = urlObj.origin + urlObj.pathname;

            // 获取图片
            const image = getImage(doc, base);
            if (image) {
                result.image = this.getUrl(image);
            }

            // 获取网站图标
            const favicon = getFavicon(doc, base);
            if (favicon) {
                result.favicon = this.getUrl(favicon);
            }

        } catch (error) {
            console.warn('LinkCard Error: Failed to parse', error);
        }

        return result;
    }

    getUrl(url) {
        if (this.options.proxyUrl && isNormalUrl(url)) {
            return this.options.proxyUrl + encodeURIComponent(url);
        }

        return url;
    }
}
export function renderLinkCards(elements, options) {
    const renderer = new LinkCardRender(options);
    renderer.renderLinkCards(elements);
}
