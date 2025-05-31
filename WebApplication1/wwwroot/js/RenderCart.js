
// Функция для проверки авторизации jwt
function isAuthenticated() {
    return document.cookie.includes('jwt=');
}

// Функция для отрисовки карточек

function renderCards(products) {
    const cardsContainer = document.getElementById('cardsContainer');

    products.forEach((product, index) => {
        const card = document.createElement('div');
        card.className = 'card';

        const favoriteButton = isAuthenticated()
            ? `<button class="favorite-btn" data-product-id="${product.id}">❤️ Добавить в избранное</button>`
            : `<button disabled title="Требуется авторизация">🔒 Избранное</button>`;


        card.innerHTML = `
                <a href = "https://localhost:7100/Product?id=${product.id}">
                    <h3>${product.name}</h3>
                    <p>${product.description}</p>
                    <p class="price">Цена: $${product.price}</p>
                 </a>
                 <button>Добавить в корзину</button>
                 <button>Добавить в избранное</button>
                  ${favoriteButton}
                `;


        // Добавляем анимацию появления с задержкой
        card.style.opacity = '0';
        card.style.transform = 'scale(0.95)';
        cardsContainer.appendChild(card);

        // Стилизация элементов внутри ссылки
        const link = card.querySelector('a');
        link.style.textDecoration = 'none';
        link.style.color = 'inherit';
        link.style.display = 'block';

        const title = link.querySelector('h3');
        title.style.margin = '0 0 10px 0';
        title.style.color = '#333';
        title.style.fontSize = '1.2em';

        const paragraphs = link.querySelectorAll('p');
        paragraphs.forEach(p => {
            p.style.margin = '0 0 8px 0';
            p.style.color = '#666';
        });

        const price = link.querySelector('.price');
        price.style.fontWeight = 'bold';
        price.style.color = '#000';

        setTimeout(() => {
            card.style.opacity = '1';
            card.style.transform = 'scale(1)';
        }, index * 100);

  
    });

   


}


function getCookie(name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
}