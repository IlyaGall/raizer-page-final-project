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
    if (!Array.isArray(categories)) {
        console.error('Categories data is not an array');
        return;
    }

    // Удаляем дубликаты по id
    const uniqueCategories = categories.filter(
        (cat, index, self) => self.findIndex(c => c.id === cat.id) === index
    );

    const menuContainer = document.getElementById('categoriesMenu');
    menuContainer.innerHTML = '';

    // Создаем хэш-таблицу и заполняем children
    const categoriesMap = {};
    categories.forEach(cat => {
        categoriesMap[cat.id] = { ...cat, children: [] };
    });

    // Строим иерархию
    const rootCategories = [];
    categories.forEach(cat => {
        if (cat.parentId === -1) {
            rootCategories.push(categoriesMap[cat.id]);
        } else if (categoriesMap[cat.parentId]) {
            categoriesMap[cat.parentId].children.push(categoriesMap[cat.id]);
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

    // Один обработчик для всех действий
    link.addEventListener('click', function (e) {
        e.preventDefault();
        if (category.children && category.children.length > 0) {
            li.classList.toggle('open');
        } else {
            loadProductsByCategory(category.id);
        }
    });

    li.appendChild(link);

    // Добавляем подкатегории только если они есть
    if (category.children && category.children.length > 0) {
        li.classList.add('has-children');

        const subList = document.createElement('ul');
        subList.className = 'subcategory-list';

        // Убедимся, что children - массив уникальных элементов
        const uniqueChildren = Array.from(new Set(category.children.map(c => c.id)))
            .map(id => category.children.find(c => c.id === id));

        uniqueChildren.forEach(child => {
            subList.appendChild(createCategoryElement(child));
        });

        li.appendChild(subList);
    }

    return li;
}