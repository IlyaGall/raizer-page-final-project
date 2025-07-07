namespace OrderService.Order
{
    public class AddOrderDto
    {
        public int ShippingMethod { get; set; } = 0;
        public int PaymentMethod { get; set; } = 0;
        public string ArriveAddress { get; set; }
    }
}
