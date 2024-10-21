using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace MongoDBPerformance.API.BusinessObject
{
    public class Transaction
    {
        [JsonPropertyName("transactionId")]
        public string TransactionId { get; set; }

        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("totalAmount")]
        public decimal TotalAmount { get; set; }

        [JsonPropertyName("paymentMethod")]
        public string PaymentMethod { get; set; }

        [JsonPropertyName("shippingAddress")]
        public ShippingAddress ShippingAddress { get; set; }

        [JsonPropertyName("items")]
        public List<Item> Items { get; set; }

        public Transaction()
        {
            Items = new List<Item>();
        }
    }

    public class ShippingAddress
    {
        [JsonPropertyName("street")]
        public string Street { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("zipCode")]
        public string ZipCode { get; set; }
    }

    public class Item
    {
        [BsonElement("ProductId")]
        [JsonPropertyName("itemId")]
        public string ItemId { get; set; }

        [BsonElement("ProductName")]
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [BsonElement("Quantity")]
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [BsonElement("Price")]
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
    }
}
