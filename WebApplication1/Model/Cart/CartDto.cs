namespace CartService.BLL
{
    public class CartDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public decimal Count { get; set; }
        public string NameProduct { get; set; }
        public string ModelNumber { get; set; }
        public string Description { get; set; }
    }
}
