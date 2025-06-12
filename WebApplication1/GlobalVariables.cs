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


        #endregion





    }
}
