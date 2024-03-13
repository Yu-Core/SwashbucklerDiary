export function init(dotNetHelper, containerId, option) {
    var container = document.getElementById(containerId);
    if (!container) return;
    
    let _options = {
        onChoose: function (evt) {
            dotNetHelper.invokeMethodAsync(`OnChoose`, convertDropEndEvent(evt));
        },
        onUnchoose: function (evt) {
            dotNetHelper.invokeMethodAsync(`OnUnchoose`, convertDropEndEvent(evt));
        },
        onStart: function (evt) {
            dotNetHelper.invokeMethodAsync(`OnStart`, convertDropEndEvent(evt));
        },
        onEnd: function (evt) {
            dotNetHelper.invokeMethodAsync(`OnDropEnd`, convertDropEndEvent(evt));
        },
        onAdd: function (evt) {
            dotNetHelper.invokeMethodAsync(`OnAdd`, convertDropEndEvent(evt));
        },
        onUpdate: function (evt) {
            dotNetHelper.invokeMethodAsync(`OnUpdate`, convertDropEndEvent(evt));
        },
        onSort: function (evt) {
            dotNetHelper.invokeMethodAsync(`OnSort`, convertDropEndEvent(evt));
        },
        onRemove: function (evt) {
            let param = convertDropEndEvent(evt);
            dotNetHelper.invokeMethodAsync(`OnRemove`, param);
            if (param.isClone)
                evt.clone.remove();
        },
        onMove: function (evt, originalEvent) {
            dotNetHelper.invokeMethodAsync(`OnMove`, convertDropMoveEvent(evt, originalEvent));
        },
        onClone: function (evt) {
            dotNetHelper.invokeMethodAsync(`OnClone`, convertDropEndEvent(evt));
        },
        onChange: function (evt) {
            dotNetHelper.invokeMethodAsync(`OnChange`, convertDropEndEvent(evt));
        },
    }

    if (option.group.pull !== undefined) {
        if (typeof option.group.pull === 'boolean' || option.group.pull == 'clone') {
        } else {
            option.group.pull = function () {
                dotNetHelper.invokeMethodAsync(`OnPull`)
            }
        }
    }

    if (option.group.put !== undefined) {
        if (typeof option.group.put === 'boolean' || option.group.put.join) {

        } else {
            option.group.put = function () {
                return dotNetHelper.invokeMethodAsync(`OnPut`);
            }
        }
    }

    if (Sortable) {
        var ops = Object.assign(_options, option)
        new Sortable(container, ops);
    }

    function convertDropEndEvent(event) {
        //console.log(event.type, event.oldIndex, event.newIndex, event.item, event.from, event.to, event);       
        if (!event || !(event.item || event.dragged) || !event.from) return null;
        var result = {
            parentId: event.from.id,
            newParentId: (event.to || {}).id || "",
            itemId: (event.item || event.dragged).id,
            oldIndex: event.oldIndex,
            newIndex: event.newIndex
        };
        if (result.oldIndex === undefined) result.oldIndex = -1;
        if (result.newIndex === undefined) result.newIndex = -1;

        if (result.oldIndex === null) result.oldIndex = -1;
        if (result.newIndex === null) result.newIndex = -1;

        if (event.pullMode == "clone") result.isClone = true
        return result;
    }

    function convertDropMoveEvent(event, dragEvent) {
        var result = convertDropEndEvent(event);
        var dragResult = {
            rect: event.draggedRect,
            moved: {
                clientX: dragEvent.clientX,
                clientY: dragEvent.clientY,
                screenX: dragEvent.screenX,
                screenY: dragEvent.screenY,
                offsetX: dragEvent.offsetX,
                offsetY: dragEvent.offsetY,
                pageX: dragEvent.pageX,
                pageY: dragEvent.pageY,
                layerX: dragEvent.layerX,
                layerY: dragEvent.layerY,
            }
        };
        return Object.assign(result, dragResult);
    }
}