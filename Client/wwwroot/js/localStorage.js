// Получение данных
window.getFromLocalStorage = async (key) => {
    return localStorage.getItem(key);
};

// Сохранение данных
window.setToLocalStorage = async (key, value) => {
    localStorage.setItem(key, value);
};