export function insertText(element, text) {
    var textarea = element;
    var startPos = textarea.selectionStart;
    var endPos = textarea.selectionEnd;
    var content = textarea.value;
    var newText = content.substring(0, startPos) + text + content.substring(endPos);
    textarea.value = newText;
    textarea.selectionStart = startPos + text.length;
    textarea.selectionEnd = startPos + text.length;
    textarea.focus();
    return newText;
}
