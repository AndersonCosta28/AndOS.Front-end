window.draggableFunctions = {
    listeners: {},
    startDrag: function (elementContainer, headerElement, dotnetHelper) {
        var initialMouseX, initialMouseY, initialElementX, initialElementY;
        var drag, endDrag, listener;
        var listenerId = Math.random().toString(36).substring(2);
        console.log(listenerId)
        if (!headerElement)
            return;

        drag = function (event) {
            var dx = event.clientX - initialMouseX;
            var dy = event.clientY - initialMouseY;

            elementContainer.style.left = initialElementX + dx + 'px';
            elementContainer.style.top = initialElementY + dy + 'px';
            event.stopPropagation();
            dotnetHelper.invokeMethodAsync('OnDrag', initialElementX + dx + 'px', initialElementY + dy + 'px');
        };

        endDrag = function (event) {
            document.removeEventListener('mousemove', drag);
            document.removeEventListener('mouseup', endDrag);
            event.stopPropagation();
        };

        listener = function (event) {
            initialMouseX = event.clientX;
            initialMouseY = event.clientY;
            initialElementX = elementContainer.offsetLeft;
            initialElementY = elementContainer.offsetTop;

            document.addEventListener('mousemove', drag);
            document.addEventListener('mouseup', endDrag);
            event.stopPropagation();
            dotnetHelper.invokeMethodAsync('OnClickWindowContainer');
        };

        headerElement.addEventListener('mousedown', listener);

        // Armazena a função de remoção de ouvintes no objeto global
        window.draggableFunctions.listeners[listenerId] = function removeListeners() {
            document.removeEventListener('mousemove', drag);
            document.removeEventListener('mouseup', endDrag);
            headerElement.removeEventListener('mousedown', listener);
        };

        // Retorna o identificador único
        return listenerId;
    },
    removeListeners: function (listenerId) {
        if (window.draggableFunctions.listeners[listenerId]) {
            window.draggableFunctions.listeners[listenerId]();
            delete window.draggableFunctions.listeners[listenerId];
        }
    }
};


window.resizeObserverInterop = {
    createObserver: function (element, dotnetHelper) {
        const resizeObserver = new ResizeObserver(entries => {
            for (let entry of entries) {
                dotnetHelper.invokeMethodAsync('OnResize', entry.contentRect.width + 'px', entry.contentRect.height + 'px');
            }
        });

        resizeObserver.observe(element);

        return resizeObserver;
    },
    disconnect: function (resizeObserver) {
        resizeObserver.disconnect();
    }
};