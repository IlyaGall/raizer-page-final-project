namespace Model.Cart
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Count { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string ModelNumber { get; set;  }
    }
}
