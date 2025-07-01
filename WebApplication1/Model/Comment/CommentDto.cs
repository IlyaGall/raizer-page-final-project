namespace WebApplication1.Model.Comment
{
    public class CommentDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime DateCreated { get; set; }
        public int ShopId { get; set; }
        public int ProductId { get; set; }
        public string Text { get; set; }
        public int Estimation { get; set; }
    }
}
