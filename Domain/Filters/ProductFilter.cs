namespace Domain.Filters;

public class ProductFilter : BaseFilter
{
    public Guid? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? OnlyFeatured { get; set; }
}