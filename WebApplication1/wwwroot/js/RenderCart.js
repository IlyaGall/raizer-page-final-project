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

// Получение данных пользователя из localStorage
function getUserData() {
    const token = localStorage.getItem('jwtToken');
    if (!token) return null;
    return parseJwt(token);
}

// Пример использования
const userData = getUserData();


if (userData) {
    console.log('User ID:', userData["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"]);
    console.log('Username:', userData["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"]);
    console.log('Full Name:', userData.FullName);
    console.log('Email:', userData.Email);
    console.log('Roles:', userData.role || []


    );
}

// Функция для проверки авторизации
function isAuthenticated() {
    return localStorage.getItem('jwtToken') !== null;
}

// Функция для получения текущей страницы
function getCurrentPage() {
    return document.body.getAttribute('data-page') || window.location.pathname;
}

// Функция для отрисовки карточек товаров
function renderCards(products) {
    const cardsContainer = document.getElementById('cardsContainer');
    if (!cardsContainer) {
        console.error('Контейнер для карточек не найден!');
        return;
    }

    cardsContainer.innerHTML = '';

    products.forEach((product, index) => {
        const card = document.createElement('div');
        card.className = 'card';

        const link = document.createElement('a');
        link.href = `/Product?id=${product.id}`;

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

        const favoriteBtn = document.createElement('button');
        favoriteBtn.className = 'favorite-btn';

        if (isAuthenticated()) {
            favoriteBtn.innerHTML = '❤️ Добавить в избранное';
            favoriteBtn.dataset.productId = product.id;

            favoriteBtn.addEventListener('click', async () => {
                try {
                    const token = localStorage.getItem('jwtToken');
                    if (!token) {
                        alert('Требуется авторизация');
                        return;
                    }

                    fetch('https://localhost:7171/api/Shop/1')
                        .then(response => {
                            if (!response.ok) {
                                throw new Error('Сетевая ошибка11');
                            }
                            alert(response.json());
                            return response.json();
                        })
                        .then(data => console.log(data))
                        .catch(error => console.error('Ошибка:', error));



                    
                    const response = await fetch('https://host.docker.internal:7052/api/Favorite/Add', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json',
                            //'Authorization': `Bearer ${token}`
                        },
                        body: JSON.stringify({
                            UserId: userData["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"],  // Можно получить из декодированного JWT токена
                            ProductId: product.id
                        })
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
        } else {
            favoriteBtn.innerHTML = '🔒 Избранное';
            favoriteBtn.disabled = true;
            favoriteBtn.title = 'Требуется авторизация';
        }

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

// Функция для выхода из системы
function logout() {
    localStorage.removeItem('jwtToken');
    window.location.href = '/Login';
}