namespace CartService.BLL
{
    public class AddCartDto
    {
        public int UserId { get; set; }        
        public int ProductId { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
    }
}
