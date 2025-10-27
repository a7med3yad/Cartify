namespace Cartify.Application.Contracts.OrderDtos
{
    public class OrderDto
    {
        public string OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentType { get; set; }
        public string CustomerName { get; set; }
    }
}
