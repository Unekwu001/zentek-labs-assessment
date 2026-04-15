using Xunit;
using Moq;
using FluentAssertions;
using Core.Services.ProductServices;
using Core.Repos.ProductRepositories;
using Core.Services.UploadServices;
using Data.Models;
using Microsoft.Extensions.Logging;
using Common.Dtos;
using System.ComponentModel.DataAnnotations;

public class ProductServiceTests
{
    private readonly Mock<IProductRepo> _productRepoMock = new();
    private readonly Mock<IUploadService> _uploadServiceMock = new();
    private readonly Mock<ILogger<ProductService>> _loggerMock = new();

    private ProductService CreateService()
    {
        return new ProductService(
            _productRepoMock.Object,
            _loggerMock.Object,
            _uploadServiceMock.Object
        );
    }


    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenPriceIsZeroOrLess()
    {
        var service = CreateService();

        var dto = new CreateProductDto
        {
            Price = 0,
            StockQuantity = 1
        };

        Func<Task> act = async () =>
            await service.CreateAsync(dto, "test@mail.com", Guid.NewGuid());

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Price must be greater than zero");
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenStockIsNegative()
    {
        var service = CreateService();

        var dto = new CreateProductDto
        {
            Price = 10,
            StockQuantity = -1
        };

        Func<Task> act = async () =>
            await service.CreateAsync(dto, "test@mail.com", Guid.NewGuid());

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Stock cannot be negative");
    }

    [Fact]
    public async Task CreateAsync_ShouldUploadImage_WhenBase64Provided()
    {
        var service = CreateService();

        var dto = new CreateProductDto
        {
            Name = "Test",
            Price = 10,
            StockQuantity = 1,
            Base64String = "base64"
        };

        _uploadServiceMock
            .Setup(x => x.UploadBase64ImageAsync(It.IsAny<string>()))
            .ReturnsAsync("http://image.com");

        _productRepoMock
            .Setup(x => x.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) => p);

        var result = await service.CreateAsync(dto, "test@mail.com", Guid.NewGuid());

        result.ImageUrl.Should().Be("http://image.com");
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateProductSuccessfully()
    {
        var service = CreateService();

        var dto = new CreateProductDto
        {
            Name = "Test Product",
            Price = 100,
            StockQuantity = 5,
            Colour = "Red"
        };

        _productRepoMock
            .Setup(x => x.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) => p);

        var result = await service.CreateAsync(dto, "seller@mail.com", Guid.NewGuid());

        result.Name.Should().Be("Test Product");
        result.Price.Should().Be(100);
        result.StockQuantity.Should().Be(5);
    }


    [Fact]
    public async Task GetByColourAsync_ShouldThrow_WhenColourIsEmpty()
    {
        var service = CreateService();

        Func<Task> act = async () =>
            await service.GetByColourAsync("");

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Colour filter cannot be empty");
    }

    [Fact]
    public async Task GetByColourAsync_ShouldThrow_WhenNoProductsFound()
    {
        var service = CreateService();

        _productRepoMock
            .Setup(x => x.GetByColourAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<Product>());

        Func<Task> act = async () =>
            await service.GetByColourAsync("Red");

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("No product with this color was found");
    }

    [Fact]
    public async Task GetByColourAsync_ShouldReturnProducts_WhenFound()
    {
        var service = CreateService();

        var products = new List<Product>
        {
            new Product { Name = "Red Shoe", Colour = "Red" , Price = 100, StockQuantity = 5 , CurrencyCode = "USD" , Description = "A stylish red shoe" , SellerEmail = "seller@mail.com" }
        };

        _productRepoMock
            .Setup(x => x.GetByColourAsync("Red"))
            .ReturnsAsync(products);

        var result = await service.GetByColourAsync("Red");

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnPagedResult()
    {
        var service = CreateService();

        var expectedProducts = new List<Product>
        {
            new Product { Name = "Red Shoe", Colour = "Red" , Price = 100, StockQuantity = 5 , CurrencyCode = "USD" , Description = "A stylish red shoe" , SellerEmail = "seller@mail.com" },
             new Product { Name = "Blue Shoe", Colour = "Blue" , Price = 100, StockQuantity = 5 , CurrencyCode = "USD" , Description = "A stylish blue shoe" , SellerEmail = "seller@mail.com" }
        };

        _productRepoMock
            .Setup(x => x.GetPagedAsync(1, 10, null, null, null, true))
            .ReturnsAsync((expectedProducts, 2));

        var result = await service.GetPagedAsync(1, 10);

        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
    }
}