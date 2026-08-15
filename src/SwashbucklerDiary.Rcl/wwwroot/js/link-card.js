const cache = new Map();
const pendingRequests = new Map();

const resolveUrl = (url, base) => {
    if (!url) {
        return '';
    }

    try {
        return new URL(url, base).href;
    } catch {
        return url;
    }
};

const isHttpUrl = (value) => {
    try {
        const url = new URL(value);
        return url.protocol === 'http:' || url.protocol === 'https:';
    } catch {
        return false;
    }
};

const getMetaContent = (doc, ...selectors) => {
    for (const selector of selectors) {
        const element = doc.querySelector(selector);
        const content = element?.getAttribute('content')?.trim();
        if (content) {
            return content;
        }
    }

    return null;
};

const getTitle = (doc) => {
    const title = getMetaContent(
        doc,
        'meta[property="og:title"]',
        'meta[name="og:title"]',
        'meta[name="twitter:title"]'
    );
    return title || doc.title?.trim() || null;
};

const getDescription = (doc) => {
    return getMetaContent(
        doc,
        'meta[property="og:description"]',
        'meta[name="og:description"]',
        'meta[name="description"]',
        'meta[name="twitter:description"]'
    );
};

const getImage = (doc, base) => {
    const image = getMetaContent(
        doc,
        'meta[property="og:image"]',
        'meta[name="og:image"]',
        'meta[name="twitter:image"]'
    );

    return image ? resolveUrl(image, base) : null;
};

const getFavicon = (doc, base) => {
    const iconElement = doc.querySelector('link[rel~="icon"]');
    const href = iconElement?.getAttribute('href');
    return resolveUrl(href || '/favicon.ico', base);
};

class LinkCardRender {
    constructor(options = {}) {
        this.options = {
            timeoutMs: 10000,
            ...options
        };
    }

    renderLinkCards(elements) {
        if (!elements) {
            return;
        }

        Array.from(elements).forEach((el) => {
            const href = el?.href;
            if (!isHttpUrl(href)) {
                return;
            }

            const cached = cache.get(href);
            if (cached) {
                this.renderCard(el, cached);
                return;
            }

            this.fetchAndRender(el, href);
        });
    }

    async fetchAndRender(el, url) {
        try {
            const html = await this.fetchLinkData(url);
            const data = this.parse(html, url);
            if (!data.description) {
                return;
            }

            cache.set(url, data);
            this.renderCard(el, data);
        } catch (error) {
            console.error('LinkCard Error:', error);
        }
    }

    async fetchLinkData(url) {
        const requestUrl = this.getRequestUrl(url);
        const existing = pendingRequests.get(requestUrl);
        if (existing) {
            return existing;
        }

        const requestPromise = (async () => {
            const controller = new AbortController();
            const timer = window.setTimeout(() => controller.abort(), this.options.timeoutMs);

            try {
                const response = await fetch(requestUrl, {
                    signal: controller.signal
                });

                if (!response.ok) {
                    throw new Error(`HTTP ${response.status} when fetching ${requestUrl}`);
                }

                return await response.text();
            } finally {
                window.clearTimeout(timer);
                pendingRequests.delete(requestUrl);
            }
        })();

        pendingRequests.set(requestUrl, requestPromise);
        return requestPromise;
    }

    renderCard(originalElement, data) {
        const card = this.createDOM(data);
        Array.from(originalElement.attributes).forEach((attr) => {
            card.setAttribute(attr.name, attr.value);
        });

        card.classList.add('link-card');
        card.href = data.url;

        originalElement.parentNode?.replaceChild(card, originalElement);
    }

    createDOM(data) {
        const card = document.createElement('a');

        const content = document.createElement('div');
        content.className = 'link-card__content';

        const title = document.createElement('div');
        title.className = 'link-card__title';
        title.textContent = data.title;

        const description = document.createElement('div');
        description.className = 'link-card__description';
        description.textContent = data.description;

        const urlRow = document.createElement('div');
        urlRow.className = 'link-card__url';

        const favicon = document.createElement('img');
        favicon.className = 'link-card__favicon';
        favicon.alt = 'favicon';

        if (data.favicon) {
            favicon.src = data.favicon;
            favicon.addEventListener('error', () => {
                if (this.options.faviconSrc) {
                    favicon.src = this.options.faviconSrc;
                } else {
                    favicon.style.display = 'none';
                }
            }, { once: true });
        } else {
            favicon.style.display = 'none';
        }

        const domainText = document.createElement('span');
        domainText.className = 'link-card__url-text';
        domainText.textContent = new URL(data.url).hostname;

        urlRow.append(favicon, domainText);
        content.append(title, description, urlRow);
        card.appendChild(content);

        if (data.image) {
            const imageWrap = document.createElement('div');
            imageWrap.className = 'link-card__image';

            const image = document.createElement('img');
            image.src = data.image;
            image.alt = data.title;
            image.addEventListener('error', () => {
                imageWrap.style.display = 'none';
            }, { once: true });

            imageWrap.appendChild(image);
            card.appendChild(imageWrap);
        }

        return card;
    }

    parse(html, url) {
        const result = {
            title: url,
            description: '',
            image: '',
            favicon: '',
            url
        };

        try {
            const parser = new DOMParser();
            const doc = parser.parseFromString(html, 'text/html');

            result.title = getTitle(doc) || url;

            const description = getDescription(doc);
            if (!description) {
                return result;
            }
            result.description = description;

            const urlObj = new URL(url);
            const base = `${urlObj.origin}${urlObj.pathname}`;

            const image = getImage(doc, base);
            if (image) {
                result.image = this.getRequestUrl(image);
            }

            const favicon = getFavicon(doc, base);
            if (favicon) {
                result.favicon = this.getRequestUrl(favicon);
            }
        } catch (error) {
            console.warn('LinkCard Error: Failed to parse', error);
        }

        return result;
    }

    getRequestUrl(url) {
        if (this.options.proxyUrl && isHttpUrl(url)) {
            return this.options.proxyUrl + encodeURIComponent(url);
        }

        return url;
    }
}

export function renderLinkCards(elements, options) {
    const renderer = new LinkCardRender(options);
    renderer.renderLinkCards(elements);
}
