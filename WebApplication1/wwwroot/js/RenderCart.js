// Функция для декодирования JWT токена
function parseJwt(token) {
    try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(
            atob(base64)
                .split('')
                .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
                .join('')
        );

        return JSON.parse(jsonPayload);
    } catch (e) {
        return null;
    }
}

// Получение данных пользователя
function getUserData() {
    const cookie = document.cookie
        .split('; ')
        .find(row => row.startsWith('jwt='));

    if (!cookie) return null;

    const token = cookie.split('=')[1];
    return parseJwt(token);
}

// Пример использования
const userData = getUserData();
if (userData) {
    console.log('User ID:', userData["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"]);
    console.log('Username:', userData["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"]);
    console.log('Full Name:', userData.FullName);
    console.log('Email:', userData.Email);
    console.log('Roles:', userData.role || []);
}



// Функция для проверки авторизации jwt
function isAuthenticated() {
    alert(document.cookie.includes('jwt='));
    return document.cookie.includes('jwt=');
}

function isAuthenticated1() {
    // Для отладки выводим все cookies
    console.log('Все cookies:', document.cookie);

    // Проверяем наличие jwt cookie
    const cookieExists = document.cookie.split(';').some(
        item => item.trim().startsWith('jwt=')
    );

    console.log('JWT cookie exists:', cookieExists);
    return cookieExists;
}

//Функция отслеживания на какой странице происходит сейчас находится пользователь
//Нужно, чтобы не не запускть скрипт с привязкой к дереву
//document.addEventListener('DOMContentLoaded', function () {
//    const page = document.body.getAttribute('data-page');
//    return page;
//    alert(page);
//    if (page === '/Index') {
//        /* Код для конкретной страницы*/
//    }
//});

//Функция отслеживания на какой странице происходит сейчас находится пользователь
//Нужно, чтобы не не запускть скрипт с привязкой к дереву
function getNamePAge() {
    return document.body.getAttribute('data-page').toString();

}

// Функция для отрисовки карточек
function renderCards(products) {
    const cardsContainer = document.getElementById('cardsContainer');
    if (!cardsContainer) {
        console.error('Контейнер для карточек не найден!');
        return;
    }

    // Очищаем контейнер перед добавлением новых карточек
    cardsContainer.innerHTML = '';

    products.forEach((product, index) => {
        const card = document.createElement('div');
        card.className = 'card';

        // Создаем элементы вручную вместо innerHTML
        const link = document.createElement('a');
        link.href = `https://localhost:7100/Product?id=${product.id}`;

        const title = document.createElement('h3');
        title.textContent = product.name || 'Без названия';

        const description = document.createElement('p');
        description.textContent = product.description || '';

        const price = document.createElement('p');
        price.className = 'price';
        price.textContent = `Цена: $${product.price || '0'}`;

        const cartBtn = document.createElement('button');
        cartBtn.className = 'cart-btn';
        cartBtn.textContent = 'Добавить в корзину';

        // Создаем кнопку избранного
        const favoriteBtn = document.createElement('button');
        favoriteBtn.className = 'favorite-btn';
        isAuthenticated1();
        if (isAuthenticated()) {
            favoriteBtn.innerHTML = '❤️ Добавить в избранное';
            favoriteBtn.dataset.productId = product.id;

            // Добавляем обработчик напрямую к кнопке
            favoriteBtn.addEventListener('click', async () => {
                try {
                    
                    const token = getCookie('jwt');
                    if (!token) {
                        alert('Требуется авторизация');
                        return;
                    }

                    const response = await fetch('/api/favorites', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json',
                            'Authorization': `Bearer ${token}`
                        },
                        body: JSON.stringify({ productId: product.id })
                    });

                    if (response.ok) {
                        alert('Товар добавлен в избранное!');
                    } else {
                        alert('Ошибка при добавлении в избранное');
                    }
                } catch (error) {
                    console.error('Error:', error);
                    alert('Произошла ошибка');
                }
            });
        }
        else
        {
            favoriteBtn.innerHTML = '🔒 Избранное';
            favoriteBtn.disabled = true;
            favoriteBtn.title = 'Требуется авторизация';
        }

        // Собираем карточку
        link.appendChild(title);
        link.appendChild(description);
        link.appendChild(price);

        card.appendChild(link);
        card.appendChild(cartBtn);
        card.appendChild(favoriteBtn);

        // Стилизация
        link.style.textDecoration = 'none';
        link.style.color = 'inherit';
        link.style.display = 'block';

        title.style.margin = '0 0 10px 0';
        title.style.color = '#333';
        title.style.fontSize = '1.2em';

        [description, price].forEach(p => {
            p.style.margin = '0 0 8px 0';
            p.style.color = '#666';
        });

        price.style.fontWeight = 'bold';
        price.style.color = '#000';

        // Анимация
        card.style.opacity = '0';
        card.style.transform = 'scale(0.95)';
        cardsContainer.appendChild(card);

        setTimeout(() => {
            card.style.opacity = '1';
            card.style.transform = 'scale(1)';
            card.style.transition = 'opacity 0.3s, transform 0.3s';
        }, index * 100);
    });
}

function getCookie(name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
}