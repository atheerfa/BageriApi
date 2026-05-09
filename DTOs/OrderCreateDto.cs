namespace BageriApi.DTOs
{
    public class OrderCreateDto
    {
        public int CustomerId { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}