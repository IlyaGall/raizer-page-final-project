// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Загружаем категории при загрузке страницы
document.addEventListener('DOMContentLoaded', function () {
    loadCategories();
});

// Функция загрузки категорий
function loadCategories() {
    fetch('?handler=Categories')
        .then(response => response.json())
        .then(data => {
            renderCategories(data);
        })
        .catch(error => {
            console.error('Error loading categories:', error);
            document.getElementById('categoriesMenu').innerHTML =
                '<p class="error">Ошибка загрузки категорий</p>';
        });
}

// Функция рендеринга категорий
function renderCategories(categories) {
    const menuContainer = document.getElementById('categoriesMenu');
    menuContainer.innerHTML = '';

    // Создаем хэш-таблицу для быстрого доступа
    const categoriesMap = {};
    categories.forEach(cat => {
        categoriesMap[cat.id] = cat;
        cat.children = []; // Добавляем массив для детей
    });

    // Строим иерархию
    const rootCategories = [];
    categories.forEach(cat => {
        if (cat.parentId === -1) {
            rootCategories.push(cat);
        } else {
            if (categoriesMap[cat.parentId]) {
                categoriesMap[cat.parentId].children.push(cat);
            }
        }
    });

    // Рендерим дерево
    rootCategories.forEach(category => {
        menuContainer.appendChild(createCategoryElement(category));
    });
}

// Рекурсивная функция создания элемента категории
function createCategoryElement(category) {
    const li = document.createElement('li');
    li.className = 'category-item';

    const link = document.createElement('a');
    link.href = '#';
    link.className = 'category-link';
    link.textContent = category.name;
    link.setAttribute('data-category-id', category.id);
    link.addEventListener('click', function (e) {
        e.preventDefault();
        loadProductsByCategory(category.id);
    });

    li.appendChild(link);

    if (category.children && category.children.length > 0) {
        const subList = document.createElement('ul');
        subList.className = 'subcategory-list';

        category.children.forEach(child => {
            subList.appendChild(createCategoryElement(child));
        });

        li.appendChild(subList);
    }
    // Добавляем логику для сворачивания/разворачивания подкатегорий
    if (category.children && category.children.length > 0) {
        li.classList.add('has-children');

        // Добавляем обработчик для toggle подкатегорий
        link.addEventListener('click', function (e) {
            e.preventDefault();
            li.classList.toggle('open');
        });

        const subList = document.createElement('ul');
        subList.className = 'subcategory-list';

        category.children.forEach(child => {
            subList.appendChild(createCategoryElement(child));
        });

        li.appendChild(subList);
    }
    return li;
}

