function addBackdropClickHandler(dotNetHelper) {
    const backdrop = document.querySelector('.modal-backdrop.show');
    if (backdrop) {
        backdrop.addEventListener('click', function(e) {
            if (e.target === this) { // Проверяем, что кликнули именно на бэкдроп
                dotNetHelper.invokeMethodAsync('HandleBackdropClick');
            }
        });
    }
}

// Удаляем обработчик
function removeBackdropClickHandler() {
    const backdrop = document.querySelector('.modal-backdrop.show');
    if (backdrop) {
        backdrop.replaceWith(backdrop.cloneNode(true));
    }
}

window.blazorHelpers = {
    addMenuCloseListener: function(dotNetHelper, menuElement) {
        document.addEventListener('click', function handler(e) {
            if (!menuElement.contains(e.target)) {
                document.removeEventListener('click', handler);
                dotNetHelper.invokeMethodAsync('CloseMenu');
            }
        });
    }
};