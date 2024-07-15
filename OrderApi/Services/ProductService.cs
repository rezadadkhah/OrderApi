using Microsoft.Extensions.Caching.Distributed;
using OrderApi.Domain.Exceptions.YourProject.Domain.Exceptions;
using OrderApi.Dtos;
using System.Text.Json;
using OrderApi.Data.Models;
using OrderApi.Repositories;
using OrderApi.Data;

namespace OrderApi.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IDistributedCache _cache;
        private readonly OrderDbContext _dbContext;

        public ProductService(IProductRepository productRepository, IDistributedCache cache, OrderDbContext dbContext)
        {
            _productRepository = productRepository;
            _cache = cache;
            _dbContext = dbContext;
        }
        public async Task<GetProductDto> GetProductByIdAsync(Guid id)
        {
            var cachedProduct = await GetCachedProduct(id);
            if (cachedProduct is not null)
                return cachedProduct;

            var productEntity = await _productRepository.GetProductByIdAsync(id);
            if (productEntity is null)
                throw new BusinessException("Product not found");

            var result = new GetProductDto
            {
                Id = productEntity.Id,
                InventoryCount = productEntity.InventoryCount,
                Price = productEntity.Price * (100 - productEntity.Discount) / 100,
                Title = productEntity.Title,
            };
            await CacheProduct(result);

            return result;
        }
        public async Task CreateProductAsync(CreateProductDto product)
        {
            if (await _productRepository.CheckProductTitleExistenceAsync(product.Title))
                throw new BusinessException("Product Already Exists");
            await _productRepository.CreateProductAsync(product);
        }
        public async Task IncreaseInventoryCount(IncreaseProductInventoryCountDto product)
        {
            await using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var productEntity = await _productRepository.GetProductByIdWithLockAsync(product.Id);
                    if (productEntity == null)
                    {
                        throw new BusinessException("Product not found");
                    }

                    productEntity.InventoryCount += product.InventoryCount;
                    await _productRepository.UpdateProductAsync(productEntity);

                    await UpdateCachedProduct(new GetProductDto()
                    {
                        Id = productEntity.Id,
                        InventoryCount = productEntity.InventoryCount,
                        Price = productEntity.Price * (100 - productEntity.Discount) / 100,
                        Title = productEntity.Title
                    });

                    await transaction.CommitAsync();
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
        public async Task BuyProduct(BuyProductDto buyProduct)
        {
            await using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var product = await _productRepository.GetProductByIdWithLockAsync(buyProduct.ProductId);
                    if (product == null)
                        throw new BusinessException("Product Already Exists");
                    if (product.InventoryCount - buyProduct. Count < 0)
                        throw new BusinessException("Not Enough Inventory Count");

                    product.InventoryCount -= buyProduct.Count;
                    await _productRepository.UpdateProductAsync(product);

                    await _productRepository.CreateOrderAsync(new Order()
                    {
                        BuyerId = buyProduct.BuyerUserId,
                        ProductId = buyProduct.ProductId,
                        CreationDate = DateTime.Now
                    });
                    await UpdateCachedProduct(new GetProductDto()
                    {
                        Id = product.Id,
                        InventoryCount = product.InventoryCount,
                        Price = product.Price * (100 - product.Discount) / 100,
                        Title = product.Title
                    });

                    await transaction.CommitAsync();
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    throw;
                }

            }
        }

        private async Task<GetProductDto?> GetCachedProduct(Guid id)
        {
            var redisResult = await _cache.GetStringAsync(id.ToString());
            return redisResult is null ? null : JsonSerializer.Deserialize<GetProductDto>(redisResult);
        }
        private async Task UpdateCachedProduct(GetProductDto updatedProduct)
        {
            var cachedProductJson = await _cache.GetStringAsync(updatedProduct.Id.ToString());
            if (cachedProductJson != null)
            {
                var cachedProduct = JsonSerializer.Deserialize<GetProductDto>(cachedProductJson);
                
                cachedProduct.InventoryCount = updatedProduct.InventoryCount;
                cachedProduct.Price = updatedProduct.Price;
                await _cache.SetStringAsync(updatedProduct.Id.ToString(),
                    JsonSerializer.Serialize(cachedProduct),
                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) });
                return;
            }

            await CacheProduct(updatedProduct);
        }
        private async Task CacheProduct(GetProductDto result)
        {
                await _cache.SetStringAsync(result.Id.ToString(), JsonSerializer.Serialize(result),
                    new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) });
        }
    }
}
