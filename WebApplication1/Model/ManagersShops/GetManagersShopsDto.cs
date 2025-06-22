using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagersShopsService.BLL.Dto
{
    public class GetManagersShopsDto
    {
        public int Id { get; set; }

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


        /// <summary>
        /// Роль пользователя
        /// </summary>
        
        public string UserName { get; set; } = string.Empty;


        /// <summary>
        /// Роль пользователя
        /// </summary>
        public string RoleUser { get; set; } = string.Empty;

    }
}
