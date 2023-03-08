import '/npm/markdown-it/markdown-it-deflist.min.js';
import '/npm/markdown-it/markdown-it-emoji-min.js';
import '/npm/markdown-it/markdown-it-footnote.min.js';
/*import '/npm/markdown-it/markdown-it-head-attr.js';*/
import '/npm/markdown-it/markdown-it-sub.min.js';
import '/npm/markdown-it/markdown-it-sup.min.js';
/*import '/npm/markdown-it/markdown-it-table.js';*/
import '/npm/markdown-it/markdown-it-task-lists.min.js';

window.MasaBlazor.markdownItRules = function (parser) {
    parser.md.use(window.markdownitSub)
        .use(window.markdownitSup)
        /*.use(window.markdownItTable)*/
        .use(window.markdownitFootnote)
       /* .use(window.markdownitHeadAttr)*/
        .use(window.markdownitDeflist)
        .use(window.markdownitTaskLists)
        .use(window.markdownitEmoji);
}