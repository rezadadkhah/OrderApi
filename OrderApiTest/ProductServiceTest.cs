using Microsoft.EntityFrameworkCore;
using Moq;
using OrderApi.Data.Models;
using OrderApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using OrderApi.Dtos;
using Microsoft.Extensions.Caching.Distributed;
using OrderApi.Data;
using OrderApi.Repositories;
using OrderApi.Domain.Exceptions.YourProject.Domain.Exceptions;

namespace OrderApiTest
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IDistributedCache> _mockCache;
        private readonly OrderDbContext _dbContext;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockCache = new Mock<IDistributedCache>();
            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _dbContext = new OrderDbContext(options);
            _productService = new ProductService(_mockProductRepository.Object, _mockCache.Object, _dbContext);
        }
        [Fact]
        public async Task CreateProductAsync_ShouldThrowException_WhenProductTitleExists()
        {
            var productDto = new CreateProductDto { Title = "Existing Product" };
            _mockProductRepository.Setup(r => r.CheckProductTitleExistenceAsync(productDto.Title)).ReturnsAsync(true);
            
            var exception = await Assert.ThrowsAsync<BusinessException>(() => _productService.CreateProductAsync(productDto));
            Assert.Equal("Product Already Exists", exception.Message);
            _mockProductRepository.Verify(r => r.CreateProductAsync(It.IsAny<CreateProductDto>()), Times.Never);
        }
        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnProduct_WhenProductExistsInCache()
        {
            var productId = Guid.NewGuid();
            var cachedProduct = new GetProductDto
            {
                Id = productId,
                Title = "Cached Product",
                InventoryCount = 10,
                Price = 100,
            };

            var cacheKey = productId.ToString();
            var cachedProductJson = JsonSerializer.Serialize(cachedProduct);
            var cachedProductBytes = Encoding.UTF8.GetBytes(cachedProductJson);

            _mockCache.Setup(c => c.GetAsync(cacheKey, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedProductBytes);

            
            var result = await _productService.GetProductByIdAsync(productId);
            
            Assert.NotNull(result);
            Assert.Equal(cachedProduct.Title, result.Title);
            _mockProductRepository.Verify(r => r.GetProductByIdAsync(It.IsAny<Guid>()), Times.Never);
        }
    }
}
