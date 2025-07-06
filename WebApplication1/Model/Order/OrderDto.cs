namespace OrderService.BLL.Dto.Order
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime ArriveDate { get; set; }
        public int ShippingMethod { get; set; }
        public int PaymentMethod { get; set; }
        public string ArriveAddress { get; set; }
        public int OrderStatus { get; set; }
        public DateTime DateOrderStatus { get; set; }
    }
}
