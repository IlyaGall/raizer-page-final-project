namespace ManagersShopsService.BLL.Dto
{
    public class GetManagersShopsDto
    {
        /// <summary>
        /// Id пользователя
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Id магазина
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// Название магазина
        /// </summary>
        public string NameShop { get; set; } = string.Empty;
    }
}
