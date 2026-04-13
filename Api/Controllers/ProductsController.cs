using Asp.Versioning;
using Common.Dtos;
using Core.Services.ProductServices;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : BaseController
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;
        public ProductsController(
            IProductService productService,
            ILogger<ProductsController> logger)
    
        {
            _productService = productService;
            _logger = logger;
        }



        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDto request)
        {
            try
            {
                var created = await _productService.CreateAsync(request, CurrentUserEmail, CurrentUserId);
                
                _logger.LogInformation("Product created successfully: {@Product}", created);

                return Ok(ApiResponse<Data.Models.Product>.Created(
                    created,
                    "Product created successfully"
                ));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning("Validation error while creating product: {ErrorMessage}", ex.Message);  
                return BadRequest(ApiResponse<Data.Models.Product>.ValidationError(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating product: {@CreateProductDto}", request);
                return StatusCode(500, ApiResponse<Data.Models.Product>.ServerError());
            }
        }




        [HttpGet("{colour}")]
        public async Task<IActionResult> GetByColour(string colour)
        {
            try
            {
                var products = await _productService.GetByColourAsync(colour);

                _logger.LogInformation("Products retrieved successfully for colour '{Colour}': {@Products}", colour, products);
                return Ok(ApiResponse<IEnumerable<Product>>.Successful(
                    products,
                    string.IsNullOrWhiteSpace(colour)
                        ? "All products retrieved successfully"
                        : $"Products with colour '{colour}' retrieved successfully"
                ));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning("Validation error while retrieving products by colour '{Colour}': {ErrorMessage}", colour, ex.Message);
                return BadRequest(ApiResponse<Data.Models.Product>.ValidationError(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred while retrieving products by colour '{Colour} {Error}'", colour, ex.Message);
                return StatusCode(500,
                    ApiResponse<IEnumerable<Product>>.ServerError());
            }
        }


        [HttpGet("all")]
        public async Task<IActionResult> GetPaged(
            int pageNumber = 1,
            int pageSize = 10,
            string? colour = null,
            decimal? minPrice = null,
            string? sortBy = null,
            bool ascending = true)
        {
            _logger.LogInformation(
                "HTTP GET Paged Products called. Page: {PageNumber}, Size: {PageSize}, Colour: {Colour}, MinPrice: {MinPrice}, SortBy: {SortBy}, Asc: {Ascending}",
                pageNumber, pageSize, colour, minPrice, sortBy, ascending);

            try
            {
                var result = await _productService.GetPagedAsync(
                    pageNumber,
                    pageSize,
                    colour,
                    minPrice,
                    sortBy,
                    ascending
                );

                var response = new
                {
                    result.TotalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    result.Items
                };

                _logger.LogInformation(
                    "Paged products fetched successfully. Count: {Count}",
                    result.TotalCount);

                return Ok(ApiResponse<object>.Successful(
                    response,
                    "Paged products retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error occurred while fetching paged products");

                return StatusCode(500,
                    ApiResponse<object>.ServerError());
            }
        }



    }
}
