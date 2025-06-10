document.addEventListener('DOMContentLoaded', function ()
{
    // Получаем ID товара из URL
    const urlParams = new URLSearchParams(window.location.search);
    const productId = urlParams.get('id') || '@Model.RouteData.Values["id"]';
    if (productId)
    {
        loadProductInfo(productId);
    }
});

async function loadProductInfo(productId) {
    try
    {
        // Формируем URL для запроса
        const url = `?handler=ProductInfo&id=${encodeURIComponent(productId)}`;

        // Выполняем запрос к серверу
        const response = await fetch(url,
            {
                headers:
                {
                    'Accept': 'application/json',
                }
            });

        if (!response.ok)
        {
            throw new Error('Ошибка при загрузке данных');
        }

        const productData = await response.json();
        
        // Отрисовываем данные на странице
        renderProductInfo(productData);
    }
    catch (error)
    {
        console.error('Ошибка:', error);
        document.getElementById('productContainer').innerHTML =
            '<p class="text-danger">Не удалось загрузить информацию о товаре</p>';
    }
}

//Отрисовка информации о товаре
function renderProductInfo(product) {
    const container = document.getElementById('productContainer');
    //! Хоть атрибуты, я писал с большой буквы, но получил назад маленькие!
    container.innerHTML = `
                <h2>${product.name}</h2>
                <img src="${product.imageUrl}" alt="${product.name}" class="img-fluid">
                <p>${product.description}</p>
                <p class="price">Цена: ${product.price} руб.</p>
            `;
}