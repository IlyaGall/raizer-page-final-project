namespace GlobalVariablesRP
{
    public static class GlobalVariables
    {
        /// <summary>
        /// Путь до ocelot
        /// </summary>
        public const string GATEWAY = "https://localhost:5011";

        #region Clusters

        /// <summary>
        /// Получить класстеры(все)
        /// </summary>
        public const string GET_CLUSTER = "/gateway/Cluster/GetAllElements";

        /// <summary>
        /// Для поиска кластера по ключевому слову
        /// </summary>
       // public const string GET_SEARCH_CLUSTER = "/gateway/Cluster/SearchProducts?keyWord=";
        public const string GET_SEARCH_KEYWORD_CLUSTER = "/gateway/Cluster/SerchClusterKeyWord?keyWord=";


        /// <summary>
        /// Для получения дочерних кластеров
        /// </summary>
        public const string GET_SERCH_CHILDREN_CLUSTER = "/gateway/Cluster/GetChildrenCluster?id=";



        #endregion


        #region product
        /// <summary>
        /// Получение списка товаров по кластеру
        /// </summary>
        public const string GET_PRODUCTS_BY_CLUSTER = "/gateway/Product/GetProductsByCluster?ClusterId=";



        /// <summary>
        /// Получение товара по id нужно для получение конкретного экземпляра 
        /// </summary>
        public const string GET_PRODUCT_ID = "/gateway/Product/GetProductsById?id=";
        // https://localhost:5011/gateway/Product/GetProductsById?id=1 - полный запрос

        /// <summary>
        /// Добавить продукт
        /// </summary>
        public const string POST_ADD_PRODUCT = "/gateway/Product/add";
        // https://localhost:5011/gateway/Product/GetProductsById?id=1 - полный запрос

        /// <summary>
        /// Удалить продукт
        /// </summary>
        public const string DELETE_PRODUCT = "/gateway/Product/Delete";


        /// <summary>
        /// Обновить продукт
        /// </summary>
        public const string UPDATE_PRODUCT = "/gateway/Product/Update";

        #endregion

        #region Comment

        /// <summary>
        /// Get comment by id endpoint
        /// </summary>
        public const string GET_COMMENT = "/gateway/Comment/";
        /// <summary>
        /// Add comment endpoint
        /// </summary>
        public const string ADD_COMMENT = "/gateway/Comment/Add";
        /// <summary>
        /// Update comment endpoint
        /// </summary>
        public const string UPDATE_COMMENT = "/gateway/Comment/Update";
        /// <summary>
        /// Delete comment endpoint
        /// </summary>
        public const string DELETE_COMMENT = "/gateway/Comment/Delete";
        /// <summary>
        /// Get comment list by product id endpoint
        /// </summary>
        public const string GET_COMMENTS_BY_PRODUCT = "/gateway/Comment/GetCommentsProduct?IdProduct=";

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
        public const string GET_INFO_USER = "/gateway/auth/GetUserId";

        /// <summary>
        /// Обновить информацию о пользователе
        /// </summary>
        public const string POST_UPDATE_INFO_USER = "gateway/auth/update";


        public const string GET_USER_ID = "/gateway/auth/GetSerchUser?id=";
        #endregion

        #region Owner

        /// <summary>
        /// Добавление владельца магазина
        /// </summary>
        public const string POST_ADD_OWNER_SHOP = "/gateway/Owner/AddOwnerShops";


        /// <summary>
        /// Добавление менеджера магазина 
        /// </summary>
        public const string POST_ADD_MANAGER_SHOP = "/gateway/Owner/AddManegerShops";


        /// <summary>
        /// Упращённый список магазинов пользователя
        /// </summary>
        public const string POST_LIST_SHOP_USER = "/gateway/Owner/GetShop";

        /// <summary>
        /// Получить список менеджеров
        /// </summary>
        public const string GET_MANAGER_SHOP = "/gateway/Owner/GetManager?idShop=";

        /// <summary>
        /// Удалить менеджера
        /// </summary>

        public static string DELETE_MANAGER_SHOP = "/gateway/Owner/DeleteManager";

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
        public const string PUT_SHOP_UPDATE = "/gateway/Shop/Update";

        /// <summary>
        /// Получить информацию о продутах магазина
        /// </summary>
        public const string GET_SHOP_PRODUCTS = "/gateway/Product/GetShopProducts?ShopId=";
        #endregion
    }
}
