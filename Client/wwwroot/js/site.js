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