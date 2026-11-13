using System.Text.Json.Serialization;

namespace Shop_API.Dto
{
    public class OrderItemDto
    {
        // продолжение пункта 6
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
        [JsonPropertyName("price")]
        public string Price { get; set; } = string.Empty;
    }
}
