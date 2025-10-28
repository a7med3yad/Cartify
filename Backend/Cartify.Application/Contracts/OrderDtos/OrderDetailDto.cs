namespace Cartify.Application.Contracts.OrderDtos
{
    public class OrderDetailDto
    {
        public string OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
