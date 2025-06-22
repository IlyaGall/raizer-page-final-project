namespace ProductService.BLL
{
    public class AddProductDto
    {
        /// <summary>
        /// ClusterId магазина
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// ClusterId кластера
        /// </summary>
        public int ClusterId { get; set; }
        /// <summary>
        /// Название продукта
        /// </summary>
        public string NameProduct { get; set; } = "";
        /// <summary>
        /// Описание продукта
        /// </summary>
        public string Description { get; set; } = "";
        /// <summary>
        /// Цена товара
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Штрихкод
        /// </summary>
        public long Barcode { get; set; } =1234;
        /// <summary>
        /// Номер модели
        /// </summary>
        public string ModelNumber { get; set; } = "";
    }
}
