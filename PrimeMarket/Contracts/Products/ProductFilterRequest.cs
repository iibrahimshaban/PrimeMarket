using System.Text.Json.Serialization;

namespace PrimeMarket.Contracts.Products;

public class ProductFilterRequest
{
    public string? Search { get; init; }
    public int? CategoryId { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public bool? InStock { get; init; }
    public ProductSortBy SortBy { get; init; } = ProductSortBy.Newest;
    public bool IsDescending { get; init; } = true;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProductSortBy
{
    Popularity,
    Rating,   
    Price,       
    Newest        
}