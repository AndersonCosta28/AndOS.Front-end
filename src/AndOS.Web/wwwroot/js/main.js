document.addEventListener('contextmenu', (event) => {
    event.preventDefault();
    event.stopPropagation();
});
function addClickOutsideListener(element, dotNetObject) {

    document.addEventListener('mouseup', (event) => {
        if (element == null)
            return;
        var isClickInside = element.__internalId == null ? false : element.contains(event.target);
        var isControlPressed = event.ctrlKey;
        var isShiftPressed = event.shiftKey;
        var buttonPressed = event.button;
        if (!isClickInside)
            dotNetObject.invokeMethodAsync('ClickOutSide', isControlPressed, isShiftPressed, buttonPressed);
    });
}

window.blurElement = function (element) {
    element.blur();
};

window.eventHandlers = {
    stopEventPropagation: function (event) {
        event.stopPropagation();
    }
};
window.getDimensions = (element) => {
    const rect = element.getBoundingClientRect();
    return {
        X: rect.width,
        Y: rect.height,
    };
};

window.getPosition = (element) => {
    const rect = element.getBoundingClientRect();

    return {
        X: rect.left,
        Y: rect.top
    }
}