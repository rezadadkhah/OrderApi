using Microsoft.EntityFrameworkCore;
using OrderApi.Data;
using OrderApi.Data.Models;
using OrderApi.Dtos;

namespace OrderApi.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly OrderDbContext _dbContext;

        public ProductRepository(OrderDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> CheckProductTitleExistenceAsync(string title)
        {
            return await _dbContext.Products.AnyAsync(c => c.Title == title);
        }

        public async Task<Product?> GetProductByIdAsync(Guid id)
        {
            return await _dbContext.Products.FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<Product?> GetProductByIdWithLockAsync(Guid id)
        {
            // i used raw query because there is no built-in config in ef core for locking entity in a transaction
            return await _dbContext.Products
                .FromSqlRaw("SELECT * FROM Products WITH (UPDLOCK, ROWLOCK) WHERE Id = {0}", id)
                .FirstOrDefaultAsync();
        }
        public async Task UpdateProductAsync(Product product)
        {
            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task CreateProductAsync(CreateProductDto product)
        {
            await _dbContext.Products.AddAsync(new Product()
            {
                Title = product.Title,
                InventoryCount = product.InventoryCount,
                Discount = product.Discount,
                Price = product.Price
            });
            await _dbContext.SaveChangesAsync();
        }

        public async Task CreateOrderAsync(Order order)
        {
            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();
        }


    }
}
