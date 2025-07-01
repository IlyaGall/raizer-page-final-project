namespace OrderService.Order
{
    public class AddOrderDto
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
        public int ShippingMethod { get; set; }
        public int PaymentMethod { get; set; }
        public string ArriveAddress { get; set; }
        public int OrderStatus { get; set; }
    }
}
