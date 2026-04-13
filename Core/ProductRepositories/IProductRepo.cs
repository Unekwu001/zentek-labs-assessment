using Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.ProductRepositories
{
    public interface IProductRepo
    {
        public interface IProductRepository
        {
            Task<Product> AddAsync(Product product);
            Task<Product?> GetByIdAsync(Guid id);
            Task UpdateAsync(Product product);
            Task DeleteAsync(Guid id);
            Task<bool> ExistsAsync(Guid id);
            Task<IEnumerable<Product>> GetAllAsync();
            Task<IEnumerable<Product>> GetByColourAsync(string colour);
            Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);
            Task<IEnumerable<Product>> SearchAsync(string searchTerm);       
            Task<IEnumerable<Product>> GetByMultipleColoursAsync(IEnumerable<string> colours);
            Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(
                int pageNumber,
                int pageSize,
                string? colour = null,
                decimal? minPrice = null,
                string? sortBy = null,
                bool ascending = true);

            Task<IEnumerable<Product>> GetLowStockAsync(int threshold = 10);    
            Task AddRangeAsync(IEnumerable<Product> products);
        }
    }
}
