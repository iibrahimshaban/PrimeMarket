using System.Text.Json.Serialization;

namespace PrimeMarket.Abstraction.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderStatus
{
    Pending,
    Confirmed,
    Shipped, 
    Delivered,
    Cancelled
}
