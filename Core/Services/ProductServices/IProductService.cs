using Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Services.ProductServices
{
    internal interface IProductService
    {
        Task<Product> CreateAsync(Product product);
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
