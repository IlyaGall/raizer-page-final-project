namespace ManagersShopsService.BLL.Dto
{
    public class AddManagersShopsDto
    {
        /// <summary>
        /// Никнейм пользователя
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// id пользователя магазина
        /// </summary>
        public int UserId { get; set; }


        /// <summary>
        /// Роль пользователя
        /// </summary>
        public string RoleUser { get; set; } = string.Empty;

        /// <summary>
        /// Id созданного магазина
        /// </summary>
        public int ShopId { get; set; }

        /// <summary>
        /// Название магазина
        /// </summary>
        public string NameShop { get; set; } = string.Empty;
    }

}
