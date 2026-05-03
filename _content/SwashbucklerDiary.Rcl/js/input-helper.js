export function insertText(element, text) {
    var startPos = element.selectionStart;
    var endPos = element.selectionEnd;
    var content = element.value;
    var newText = content.substring(0, startPos) + text + content.substring(endPos);
    element.value = newText;
    element.selectionStart = startPos + text.length;
    element.selectionEnd = startPos + text.length;
    element.focus();
    return newText;
}
