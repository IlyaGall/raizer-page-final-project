
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
    if (!Array.isArray(categories)) return;

    const menuContainer = document.getElementById('categoriesMenu');
    menuContainer.innerHTML = '';

    // 1. Удаляем дубликаты по ID
    const uniqueCategories = [...new Map(categories.map(cat => [cat.id, cat])).values()];

    // 2. Создаем хэш-таблицу и копируем все категории
    const categoriesMap = new Map();
    uniqueCategories.forEach(cat => {
        // Создаем полную копию объекта категории
        categoriesMap.set(cat.id, JSON.parse(JSON.stringify(cat)));
    });

    // 3. Строим иерархию
    const rootCategories = [];
    categoriesMap.forEach(cat => {
        if (cat.parentId === -1) {
            rootCategories.push(cat);
        } else if (categoriesMap.has(cat.parentId)) {
            const parent = categoriesMap.get(cat.parentId);
            // Инициализируем children если их нет
            if (!parent.children) parent.children = [];
            // Проверяем на дубликаты
            if (!parent.children.some(child => child.id === cat.id)) {
                parent.children.push(cat);
            }
        }
    });

    // 4. Рендерим дерево
    rootCategories.forEach(category => {
        menuContainer.appendChild(createCategoryElement(category));
    });

    console.log('Processed categories:', {
        originalCount: categories.length,
        uniqueCount: uniqueCategories.length,
        rootCategories: rootCategories
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

// Функция загрузки товаров по категории
function loadProductsByCategory(categoryId) {
    const initialContent = document.getElementById('initialContent');
    const cardsContainer = document.getElementById('cardsContainer');
    const loader = document.getElementById('loader');

    // Показываем loader
    loader.style.display = 'block';

    // Скрываем начальный контент
    initialContent.classList.remove('visible');
    initialContent.classList.add('hidden');

    // Скрываем предыдущие карточки
    cardsContainer.classList.remove('visible');
    cardsContainer.classList.add('hidden');
    cardsContainer.innerHTML = ''; // Очищаем контейнер

    // Отправляем запрос на сервер
    fetch(`?handler=ProductsByCategory&categoryId=${categoryId}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            // Обрабатываем полученные данные
            if (data && data.length > 0) {
                renderCards(data);
            } else {
                cardsContainer.innerHTML = '<p class="no-results">Товары не найдены</p>';
            }
        })
        .catch(error => {
            console.error('Error:', error);
            cardsContainer.innerHTML = '<p class="error-message">Произошла ошибка при загрузке товаров</p>';
        })
        .finally(() => {
            loader.style.display = 'none';
            cardsContainer.classList.remove('hidden');
            cardsContainer.classList.add('visible');
        });
}



// Обновленная функция createCategoryElement
function createCategoryElement(category) {
    const li = document.createElement('li');
    li.className = 'category-item';

    const link = document.createElement('a');
    link.href = '#';
    link.className = 'category-link';
    link.textContent = category.name;
    link.setAttribute('data-category-id', category.id);

    // Обработчик клика
    link.addEventListener('click', function (e) {
        e.preventDefault();
        e.stopPropagation();

        if (category.children && category.children.length > 0) {
            // Для категории с подкатегориями - раскрываем/скрываем
            li.classList.toggle('open');
            loadProductsByCategory(category.id); 
            // тут логиика открытия категории товара
        } else {
            // Для конечной категории - загружаем товары
            loadProductsByCategory(category.id);
        }
    });

    li.appendChild(link);

    // Добавляем подкатегории если они есть
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