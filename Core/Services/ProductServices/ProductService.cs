using Core.Repos.ProductRepositories;
using Data.Models;
using System.ComponentModel.DataAnnotations;

namespace Core.Services.ProductServices
{
    public class ProductService : IProductService
    {
        private readonly IProductRepo _productRepo;

        public ProductService(IProductRepo productRepo)
        {
            _productRepo = productRepo;
        }




        public async Task<Product> CreateAsync(Product product)
        {
            if (product.Price <= 0)
                throw new ValidationException("Price must be greater than zero");

            if (product.StockQuantity < 0)
                throw new ValidationException("Stock cannot be negative");

            return await _productRepo.AddAsync(product);
        }



        public async Task<IEnumerable<Product>> GetByColourAsync(string colour)
        {
            return await _productRepo.GetByColourAsync(colour);
        }





        public async Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? colour = null,
            decimal? minPrice = null,
            string? sortBy = null,
            bool ascending = true)
        {
            return await _productRepo.GetPagedAsync(
                pageNumber,
                pageSize,
                colour,
                minPrice,
                sortBy,
                ascending);
        }




    }
}
