using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopService.Domain
{
    /// <summary>
    /// Магазин
    /// </summary>
    [Table("shop")]
    public class Shop 
    {
        /// <summary>
        /// Id магазина
        /// </summary>
        [Key, Column("id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Название магазина
        /// </summary>
        [Required, Column("name"), MaxLength(60)]
        public string? Name { get; set; }

        /// <summary>
        /// Магазин удалён
        /// </summary>
        [Required, Column("is_delete")]
        public bool IsDelete { get; set; } = false;


        /// <summary>
        /// Описание магазина
        /// </summary>
        [Required, Column("description")]
        public string Description { get; set; } = string.Empty;


        /// <summary>
        /// Контактная инфомрация о магазине
        /// </summary>
        [Required, Column("contact_info")]
        public string ContactInfo { get; set; } = string.Empty;

        /// <summary>
        /// Адресс магазина
        /// </summary>
        [Required, Column("adress")]
        public string Adress { get; set; } = string.Empty;



        /// <summary>
        /// Вернуть Id объекта
        /// </summary>
        /// <returns>Возвращает id объекта из бд</returns>
        public int GetPrimaryKey()
        {
            return Id;
        }

     
    }
}
