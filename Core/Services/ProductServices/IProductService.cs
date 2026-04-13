using Common.Dtos;
using Data.Models;

namespace Core.Services.ProductServices
{
    public interface IProductService
    {
        Task<Product> CreateAsync(CreateProductDto dto, Guid userId);
        Task<IEnumerable<Product>> GetByColourAsync(string colour);
        Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? colour = null,
        decimal? minPrice = null,
        string? sortBy = null,
        bool ascending = true);
    }
}
