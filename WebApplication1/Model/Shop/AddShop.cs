using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Model.Shop
{
    public class AddShop
    {

        /// Название магазина
        [Required, MaxLength(60)]
        public string? Name { get; set; }


        /// Описание магазина
        [Required]
        public string Description { get; set; } = string.Empty;

        /// Контактная информация о магазине
        [Required]
        public string ContactInfo { get; set; } = string.Empty;

        /// Адрес магазина
        [Required]
        public string Adress { get; set; } = string.Empty;

       
    }
}
