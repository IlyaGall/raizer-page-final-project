namespace CommentService.BLL
{
    public class AddCommentDto
    {

        /// <summary>
        /// id пользователя
        /// </summary>
        public int UserId { get; set; }


        /// <summary>
        /// никнейм пользователя
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// id магазина
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// id продукта
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// id пользователя
        /// </summary>
        public string TextComment { get; set; } = string.Empty;
        /// <summary>
        /// Оценка выставляемая пользователем за товар
        /// </summary>
        public decimal Estimation { get; set; }

      
        public AddCommentDto(int userId,string userName, int shopId, int productId, string textComment, decimal estimation)
        {
            UserId = userId;
            UserName = userName;
            ShopId = shopId;
            ProductId = productId;
            TextComment = textComment;
            Estimation = estimation;
        }
    }
}
