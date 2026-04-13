using Common.Dtos;
using Core.Repos.ProductRepositories;
using Data.Models;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Core.Services.ProductServices
{
    public class ProductService : IProductService
    {
        private readonly IProductRepo _productRepo;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepo productRepo, ILogger<ProductService> logger)
        {
            _productRepo = productRepo;
            _logger = logger;
        }




        public async Task<Product> CreateAsync(CreateProductDto dto, string sellerEmail, Guid sellerId)
        {
            if (dto.Price <= 0)
                throw new ValidationException("Price must be greater than zero");

            if (dto.StockQuantity < 0)
                throw new ValidationException("Stock cannot be negative");

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description ?? string.Empty,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                Colour = dto.Colour ?? string.Empty,
                ImageUrl = dto.ImageUrl ?? string.Empty,
                SellerEmail = sellerEmail,
                CurrencyCode = "USD",
                SellerId = sellerId
            };

            var createdProduct = await _productRepo.AddAsync(product);

            _logger.LogInformation("Product created: {@Product}", createdProduct);

            return createdProduct;
        }




        public async Task<IEnumerable<Product>> GetByColourAsync(string colour)
        {
            if (string.IsNullOrWhiteSpace(colour))
            {
                _logger.LogWarning("Colour filter cannot be empty");
                throw new ValidationException("Colour filter cannot be empty");
            }
            
            _logger.LogInformation("Fetching products by colour: {Colour}", colour);

            var products = await _productRepo.GetByColourAsync(colour);
            if (products.Count() < 1 )
            {
                throw new ValidationException("No product with this color was found");

            }
            return products;

        }





        public async Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? colour = null,
            decimal? minPrice = null,
            string? sortBy = null,
            bool ascending = true)
        {

            _logger.LogInformation(
            "Fetching paged products. Page: {PageNumber}, Size: {PageSize}, Colour: {Colour}, MinPrice: {MinPrice}",
            pageNumber, pageSize, colour, minPrice);

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
