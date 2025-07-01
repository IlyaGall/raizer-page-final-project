namespace GlobalVariablesRP
{
    public class GlobalVariables
    {
        /// <summary>
        /// Путь до ocelot
        /// </summary>
        public const string GETWAY_OCELOT = "https://localhost:5011";

        #region clusterSerch/cluster

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

        /// <summary>
        /// Получить информацию о корзине пользователя
        /// </summary>
        public const string GET_PRODUCTS_BY_USER = "/gateway/Product/GetProductsByIds?id=";

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
        /// Получить информацию о продуктах магазина
        /// </summary>
        public const string GET_SHOP_PRODUCTS = "/gateway/Product/GetShopProducts?ShopId=";

        #endregion

        #region cart
        /// <summary>
        /// Добавить продукт в корзину
        /// </summary>
        public const string POST_ADD_CART = "/gateway/Cart/AddCartProduct";

        /// <summary>
        /// Получить информацию о корзине пользователя
        /// </summary>
        public const string GET_INFO_CART = "/gateway/Cart/GetCartUser";

        /// <summary>
        /// Обновление количества единиц товара в корзине
        /// </summary>
        public const string PUT_CART_UPDATE = "/gateway/Cart/UpdateProduct";

        /// <summary>
        /// Удалить из корзины
        /// </summary>
        public const string DELETE_PRODUCT_CART = "/gateway/Cart/DeleteProduct";

        /// <summary>
        /// Удалить корзину
        /// </summary>
        public const string DELETE_ALL_PRODUCT_CART = "/gateway/Cart/DeleteAllProduct";
        #endregion

        #region order
        /// <summary>
        /// Оформить заказ
        /// </summary>
        public const string POST_ADD_ORDER = "/gateway/Order/AddOrder";

        /// <summary>
        /// Получить все заказы пользователя
        /// </summary>
        public const string GET_INFO_ALL_ORDER = "/gateway/Order/GetAllOrderUser";

        /// <summary>
        /// Получить заказ пользователя
        /// </summary>
        public const string GET_INFO_ORDER = "/gateway/Order/GetOrderUser";

        /// <summary>
        /// Обновление статуса заказа
        /// </summary>
        public const string PUT_ORDER_UPDATE = "/gateway/Order/UpdateOrderStatus";

        /// <summary>
        /// Удалить заказ
        /// </summary>
        public const string DELETE_ORDER = "/gateway/Order/DeleteOrder";
        #endregion
    }
}
