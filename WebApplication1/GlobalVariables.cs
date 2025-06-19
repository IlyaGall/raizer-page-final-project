namespace GlobalVariablesRP
{
    public class GlobalVariables
    {
        /// <summary>
        /// Путь до ocelot
        /// </summary>
        public const string GETWAY_OCELOT = "https://localhost:5011";


        #region запросы для продуктов
        /// <summary>
        /// Получение товара по id нужно для получение конкретного экземпляра 
        /// </summary>
        public const string GET_PRODUCT_ID = "/gateway/Product/GetProductsById?id=";
        // https://localhost:5011/gateway/Product/GetProductsById?id=1 - полный запрос



        #endregion

        #region авторизация
        
        /// <summary>
        /// Авторизация пользователя на сайте
        /// </summary>
        public const string POST_AUTH_USER = "gateway/auth/login";
        // https://localhost:5011/gateway/auth/login - полный запрос

        /// <summary>
        /// Получить информацию о пользователе
        /// </summary>
        public const string GET_INFO_USER = "gateway/auth/GetUserId";

        /// <summary>
        /// Обновить информацию о пользователе
        /// </summary>
        public const string POST_UPDATE_INFO_USER = "gateway/auth/update";

        #endregion

        #region Owner

        /// <summary>
        /// Добавление менеджера магазина (нужно при создании магазина в первый раз)
        /// </summary>
        public const string POST_ADD_OWNER_SHOP = "/gateway/Owner/AddManagersShopsDto";

        /// <summary>
        /// Упращённый список магазинов пользователя
        /// </summary>
        public const string POST_LIST_SHOP_USER = "/gateway/Owner/GetShop";
       

        #endregion

        #region shop
        /// <summary>
        /// Добавить магазин
        /// </summary>
        public const string POST_ADD_SHOP = "/gateway/Shop/add";

        /// <summary>
        /// Получить информацию о магазине
        /// </summary>
        public const string GET_INFO_SHOP = "/gateway/Shop/GetInfo?id=";

        /// <summary>
        /// Обновление название магазина
        /// </summary>
        public const string POST_UPDATE = "/gateway/Shop/Update";

        public const string GET_SHOP_PRODUCTS = "/gateway/Product/GetShopProducts?ShopId=";
        #endregion
    }
}
