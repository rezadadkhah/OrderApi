using OrderApi.Data.Models;
using OrderApi.Dtos;

namespace OrderApi.Repositories
{
    public interface IProductRepository
    {
        Task CreateProductAsync(CreateProductDto product);
        Task<bool> CheckProductTitleExistenceAsync(string title);
        Task<Product?> GetProductByIdAsync(Guid id);
        Task<Product?> GetProductByIdWithLockAsync(Guid id);
        Task UpdateProductAsync(Product product);
        Task CreateOrderAsync(Order order);
    }
}
