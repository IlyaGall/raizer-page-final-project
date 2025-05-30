       // Обработчик формы
        document.getElementById('searchForm').addEventListener('submit', function(e) {
            e.preventDefault(); // Предотвращаем стандартную отправку формы
            const searchValue = document.getElementById('searchInput').value.trim();

            if (searchValue.length > 0) {
                searchProducts(searchValue);
            }
        });

        // Функция для поиска товаров
        function searchProducts(query) {
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
            fetch(`?handler=Search&query=${encodeURIComponent(query)}`)
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
                    cardsContainer.innerHTML = '<p class="error-message">Произошла ошибка при поиске</p>';
                })
                .finally(() => {
                    loader.style.display = 'none';
                    cardsContainer.classList.remove('hidden');
                    cardsContainer.classList.add('visible');
                });
        }

        // Функция для отрисовки карточек
        function renderCards(products) {
            const cardsContainer = document.getElementById('cardsContainer');

            products.forEach((product, index) => {
                const card = document.createElement('div');
                card.className = 'card';
                card.innerHTML = `
                    <h3>${product.name}</h3>
                    <p>${product.description}</p>
                    <p class="price">Цена: $${product.price}</p>
                    <a href = "https://localhost:7100/Product?id=${product.id}">Перейти</a>

                `;

                // Добавляем анимацию появления с задержкой
                card.style.opacity = '0';
                card.style.transform = 'scale(0.95)';
                cardsContainer.appendChild(card);

                setTimeout(() => {
                    card.style.opacity = '1';
                    card.style.transform = 'scale(1)';
                }, index * 100);
            });
        }

        // Обработчик ввода (опционально, если нужно искать при вводе)
        document.getElementById('searchInput').addEventListener('input', function(e) {
            const searchValue = e.target.value.trim();
            if (searchValue.length === 0) {
                const initialContent = document.getElementById('initialContent');
                const cardsContainer = document.getElementById('cardsContainer');

                cardsContainer.classList.remove('visible');
                cardsContainer.classList.add('hidden');
                cardsContainer.innerHTML = '';

                initialContent.classList.remove('hidden');
                initialContent.classList.add('visible');
            }
        });
