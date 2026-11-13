using System.Text.Json.Serialization;

namespace Shop_API.Dto
{
    public class OrderDto
    {
        // 6. Определяем свойства с нужным именем и типом данных
        [JsonPropertyName("order_id")]
        public int OrderId { get; set; }
        [JsonPropertyName("customer")]
        public string Customer { get; set; } = string.Empty;
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; } = string.Empty;
        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;
        [JsonPropertyName("items_count")]
        public int ItemsCount { get; set; }
        [JsonPropertyName("total")]
        public string Total { get; set; } = string.Empty;
        [JsonPropertyName("items")]
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }
}
