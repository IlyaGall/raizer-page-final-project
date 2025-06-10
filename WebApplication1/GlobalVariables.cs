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
    }
}
